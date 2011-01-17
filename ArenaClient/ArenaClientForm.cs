/*
Copyright (C) 2010,2011 monte

This file is part of PSP NetParty.

PSP NetParty is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Socket = System.Net.Sockets.Socket;
using System.Net;
using System.Threading;
using SharpPcap;
using System.Net.NetworkInformation;
using PspDotNetParty;
using PspDotNetParty.Client;

namespace ArenaClient
{
    delegate void LogAppender(string message, Color color, CheckBox notDisplay);

    public partial class ArenaClientForm : Form
    {
        const string SERVER_LOGIN = "ログイン";
        const string SERVER_LOGOUT = "ログアウト";

        IniParser MyIniParser = new IniParser();

        IAsyncClient ArenaSessionClient;
        AsyncUdpClient ArenaTunnelClient;

        List<LivePcapDevice> LanAdapterList = new List<LivePcapDevice>();
        LivePcapDevice CurrentPcapDevice;

        Dictionary<string, TraficStatistics> MyMacAddresses = new Dictionary<string, TraficStatistics>();
        Dictionary<string, TraficStatistics> RemoteMacAddresses = new Dictionary<string, TraficStatistics>();

        string LoginUserName;

        //bool ConnectingToServer = false;
        bool FormIsClosing = false;

        enum OperationMode { Offline, ConnectingToServer, Portal, ArenaLobby, PlayRoomMaster, PlayRoomParticipant };
        OperationMode CurrentOperationMode;

        bool TunnelIsLinked = false;

        IPEndPoint ServerIpEndPoint;
        //string RawServerAddress;

        int ServerListCount = 0;
        int ServerAddressComboBoxIndex = 0;

        List<string> ServerHistory = new List<string>();
        const int MaxServerHistory = 10;

        public ArenaClientForm()
        {
            InitializeComponent();

            MyIniParser = new IniParser();

            string strWidth = MyIniParser.GetSetting(IniConstants.SECTION_SETTINGS, IniConstants.CLIENT_WINDOW_WIDTH, 800);
            string strHeight = MyIniParser.GetSetting(IniConstants.SECTION_SETTINGS, IniConstants.CLIENT_WINDOW_HEIGHT, 500);
            int width, height;
            if (int.TryParse(strWidth, out width))
                this.Width = width;
            if (int.TryParse(strHeight, out height))
                this.Height = height;
            LoginUserNameTextBox.Text = MyIniParser.GetSetting(IniConstants.SECTION_SETTINGS, IniConstants.CLIENT_LOGIN_NAME, string.Empty);
            //ServerAddressComboBox.Text = MyIniParser.GetSetting(IniConstants.SECTION_SETTINGS, IniConstants.CLIENT_SERVER_ADDRESS, "127.0.0.1:30000");

            string[] serverList = MyIniParser.GetSetting(IniConstants.SECTION_SETTINGS, IniConstants.CLIENT_SERVER_LIST, string.Empty).Split(',');
            foreach (string s in serverList)
            {
                if (s != string.Empty)
                {
                    ServerAddressComboBox.Items.Add(s);
                    ServerListCount++;
                }
            }

            if (ServerListCount > 0)
            {
                ServerAddressComboBox.SelectedIndex = 0;
                ServerAddressComboBox.Text = (string)ServerAddressComboBox.SelectedItem;
            }
            else
            {
                ServerAddressComboBox.Text = string.Empty;
            }

            ServerAddressComboBox.Items.Add("----------履歴----------");

            serverList = MyIniParser.GetSetting(IniConstants.SECTION_SETTINGS, IniConstants.CLIENT_SERVER_HISTORY, string.Empty).Split(',');
            foreach (string s in serverList)
            {
                if (s != string.Empty)
                {
                    if (ServerHistory.Count >= MaxServerHistory) break;

                    ServerAddressComboBox.Items.Add(s);
                    ServerHistory.Add(s);
                }
            }
            ServerHistory.Reverse();

            GoTo(OperationMode.Offline);

            ArenaSessionClient = new AsyncTcpClient(new SessionHandler(this));
            ArenaTunnelClient = new AsyncUdpClient(new TunnelHandler(this));

            //UdpPortTextBox_TextChanged(null, null);

            ResetServerStatusBar();
            RefreshLanAdapterList();

            AppendToChatTextBoxDelegate = delegate(string message, Color color, CheckBox notDisplay)
            {
                if (notDisplay != null && notDisplay.Checked) return;
                try
                {
                    ChatLogTextBox.AppendText(message);
                    ChatLogTextBox.AppendText(Environment.NewLine);
                    //ChatLogTextBox.ScrollToCaret();

                }
                catch (Exception ex)
                {
                    logTextBox.AppendText(message);
                    logTextBox.AppendText(ex.ToString());
                }
            };

            string software = String.Format("{0}  アリーナクライアント バージョン: {1}", ApplicationConstants.APP_NAME, ApplicationConstants.VERSION);
            AppendToChatTextBoxDelegate(software, Color.DarkGreen, null);
            AppendToChatTextBoxDelegate("プロトコル: " + Protocol1Constants.PROTOCOL_NUMBER, Color.DarkGreen, null);
            ThreadPool.QueueUserWorkItem(new WaitCallback(PendingLogPollingLoop));
        }

        private void ArenaClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MyIniParser.SetSetting(IniConstants.SECTION_SETTINGS, IniConstants.CLIENT_WINDOW_WIDTH, this.Width.ToString());
            MyIniParser.SetSetting(IniConstants.SECTION_SETTINGS, IniConstants.CLIENT_WINDOW_HEIGHT, this.Height.ToString());

            ServerHistory.Reverse();
            StringBuilder sb = new StringBuilder();
            foreach (string s in ServerHistory)
            {
                sb.Append(s).Append(',');
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            MyIniParser.SetSetting(IniConstants.SECTION_SETTINGS, IniConstants.CLIENT_SERVER_HISTORY, sb.ToString());

            MyIniParser.SaveSettings();

            FormIsClosing = true;
            LivePcapDeviceList list = LivePcapDeviceList.Instance;
            foreach (LivePcapDevice d in list)
                if (d.Opened) try
                    {
                        if (d.Started) d.StopCapture();
                        d.Close();
                    }
                    catch (PcapException)
                    {
                        //ex.ToString();
                    }
        }

        int CurrentLogLineCount = 0;
        object QueueLock = new object();
        List<string> PendingLogs = new List<string>();

        void PendingLogPollingLoop(object o)
        {
            while (!FormIsClosing)
            {
                bool shouldSleep = true;

                lock (QueueLock)
                    if (PendingLogs.Count > 0)
                        shouldSleep = false;

                if (shouldSleep)
                {
                    Thread.Sleep(250);
                    continue;
                }

                List<string> swap;
                lock (QueueLock)
                {
                    swap = PendingLogs;
                    PendingLogs = new List<string>();
                }

                string message = null;
                MethodInvoker action = new MethodInvoker(delegate()
                {
                    //CurrentLogLineCount++;
                    //if (CurrentLogLineCount > 300)
                    //{
                    //    logTextBox.Clear();
                    //    CurrentLogLineCount = 0;
                    //}

                    logTextBox.AppendText(message);
                    logTextBox.AppendText(Environment.NewLine);
                    logTextBox.ScrollToCaret();
                });

                foreach (string s in swap)
                {
                    message = s;
                    Invoke(action);
                }
            }
        }

        void RefreshLanAdapterList()
        {
            lanAdapterComboBox.Items.Clear();
            LanAdapterList.Clear();

            lanAdapterComboBox.Items.Add("選択されていません");

            LivePcapDeviceList list = LivePcapDeviceList.Instance;
            foreach (LivePcapDevice d in list)
            {
                string friendlyName = ClientUtility.GetDeviceDescription(d.Name);
                if (friendlyName == null)
                    friendlyName = d.Description;
                lanAdapterComboBox.Items.Add(friendlyName);
                LanAdapterList.Add(d);
            }

            //lanAdapterComboBox.Items.Add("リストを更新...");

            lanAdapterComboBox.SelectedIndex = 0;
            toggleCaptureLanAdapterButton.Enabled = false;
        }

        void ResetServerStatusBar()
        {
            serverAddressStripStatusLabel.Text = "サーバーに接続していません";
            serverStatusStripStatusLabel.Text = "";
        }

        void MaintainingNatConnectionLoop(object o)
        {
            while (ArenaTunnelClient.Connected)
            {
                ArenaTunnelClient.Send(" "); // packet for maintaining nat conection
                Thread.Sleep(20000);
            }
        }

        void PingLoop(object o)
        {
            while (ArenaSessionClient.Connected)
            {
                ArenaSessionClient.Send(Protocol1Constants.COMMAND_PING + " " + System.DateTime.Now.Ticks);

                Thread.Sleep(5000);
            }
        }

        void PacketMonitorLoop(object o)
        {
            const int milliseconds = 1000;
            const int notActiveTime = 100000000;
            const string format = "F1";
            List<string> obsoleteMacAddresses = new List<string>();

            MethodInvoker action = new MethodInvoker(
                delegate()
                {
                    lock (MyMacAddresses)
                    {
                        long deadlineTicks = System.DateTime.Now.Ticks - notActiveTime;

                        foreach (KeyValuePair<string, TraficStatistics> p in MyMacAddresses)
                        {
                            TraficStatistics stats = p.Value;
                            if (stats.lastModified < deadlineTicks)
                            {
                                obsoleteMacAddresses.Add(p.Key);
                                continue;
                            }

                            double inBps = ((double)stats.currentInBytes) * 8 / 1000;
                            double outBps = ((double)stats.currentOutBytes) * 8 / 1000;

                            int index = PacketMonitorMyPspListView.Items.IndexOfKey(p.Key);
                            if (index != -1)
                            {
                                ListViewItem item = PacketMonitorMyPspListView.Items[index];
                                item.SubItems[1].Text = inBps.ToString(format);
                                item.SubItems[2].Text = outBps.ToString(format);
                                item.SubItems[3].Text = stats.totalInBytes.ToString();
                                item.SubItems[4].Text = stats.totalOutBytes.ToString();
                            }
                            else
                            {
                                ListViewItem item = PacketMonitorMyPspListView.Items.Add(p.Key, p.Key, 0);
                                item.SubItems.Add(inBps.ToString(format));
                                item.SubItems.Add(outBps.ToString(format));
                                item.SubItems.Add(stats.totalInBytes.ToString());
                                item.SubItems.Add(stats.totalOutBytes.ToString());
                            }

                            stats.currentInBytes = 0;
                            stats.currentOutBytes = 0;
                        }

                        foreach (string mac in obsoleteMacAddresses)
                        {
                            MyMacAddresses.Remove(mac);
                            PacketMonitorMyPspListView.Items.RemoveByKey(mac);
                        }
                        obsoleteMacAddresses.Clear();
                    }

                    lock (RemoteMacAddresses)
                    {
                        long deadlineTicks = System.DateTime.Now.Ticks - notActiveTime;

                        foreach (KeyValuePair<string, TraficStatistics> p in RemoteMacAddresses)
                        {
                            TraficStatistics stats = p.Value;
                            if (stats.lastModified < deadlineTicks)
                            {
                                obsoleteMacAddresses.Add(p.Key);
                                continue;
                            }

                            double inBps = ((double)stats.currentInBytes) * 8 / 1000;
                            double outBps = ((double)stats.currentOutBytes) * 8 / 1000;

                            int index = PacketMonitorRemotePspListView.Items.IndexOfKey(p.Key);
                            if (index != -1)
                            {
                                ListViewItem item = PacketMonitorRemotePspListView.Items[index];
                                item.SubItems[1].Text = inBps.ToString(format);
                                item.SubItems[2].Text = outBps.ToString(format);
                                item.SubItems[3].Text = stats.totalInBytes.ToString();
                                item.SubItems[4].Text = stats.totalOutBytes.ToString();
                            }
                            else
                            {
                                ListViewItem item = PacketMonitorRemotePspListView.Items.Add(p.Key, p.Key, 0);
                                item.SubItems.Add(inBps.ToString(format));
                                item.SubItems.Add(outBps.ToString(format));
                                item.SubItems.Add(stats.totalInBytes.ToString());
                                item.SubItems.Add(stats.totalOutBytes.ToString());
                            }

                            stats.currentInBytes = 0;
                            stats.currentOutBytes = 0;
                        }

                        foreach (string mac in obsoleteMacAddresses)
                        {
                            RemoteMacAddresses.Remove(mac);
                            PacketMonitorRemotePspListView.Items.RemoveByKey(mac);
                        }
                        obsoleteMacAddresses.Clear();
                    }
                });

            while (TunnelIsLinked)
            {
                Invoke(action);
                Thread.Sleep(milliseconds);
            }
        }

        void SendChat()
        {
            if (CommandTextBox.Text != string.Empty)
            {
                ArenaSessionClient.Send(Protocol1Constants.COMMAND_CHAT + " " + CommandTextBox.Text);
                CommandTextBox.Text = string.Empty;
            }
        }

        void CommandTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendChat();
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
            }
            else
            {
            }
        }

        void CommandSubmitButton_Click(object sender, EventArgs e)
        {
            SendChat();
        }

        LogAppender AppendToChatTextBoxDelegate;
        public void AppendToChatTextBox(string message, Color color, CheckBox notDisplay = null)
        {
            if (InvokeRequired)
                Invoke(AppendToChatTextBoxDelegate, message, color, notDisplay);
            else
                AppendToChatTextBoxDelegate(message, color, null);
        }

        //LogAppender AppendToLogTextBoxDelegate;
        public void AppendToLogTextBox(string message)
        {
            //Console.WriteLine(message);
            lock (QueueLock)
                PendingLogs.Add(message);

            //Invoke(AppendToLogTextBoxDelegate, message, color);
        }

        void ServerLoginButton_Click(object sender, EventArgs e)
        {
            if (CurrentOperationMode == OperationMode.Offline)
            {
                if (LoginUserNameTextBox.Text == string.Empty)
                {
                    MainTabControl.SelectedTab = ConfigTagPage;
                    LoginUserNameTextBox.Focus();
                    LoginUserNameAlertLabel.Visible = true;
                    return;
                }

                string endpoint = ServerAddressComboBox.Text;
                string[] tokens = endpoint.Split(':');

                IPAddress ipaddress;
                int port;
                if (tokens.Length == 2 && int.TryParse(tokens[1], out port))
                {
                    if (IPAddress.TryParse(tokens[0], out ipaddress))
                    {
                        ServerIpEndPoint = new IPEndPoint(ipaddress, port);
                    }
                    else
                    {
                        try
                        {
                            IPHostEntry info = Dns.GetHostEntry(tokens[0]);
                            ServerIpEndPoint = new IPEndPoint(info.AddressList[0], port);
                        }
                        catch (Exception)
                        {
                            // invalid host name
                            return;
                        }
                    }

                    ServerLoginButton.Enabled = false;
                    ServerAddressComboBox.Enabled = false;

                    ArenaSessionClient.Connect(ServerIpEndPoint);
                    LoginUserName = LoginUserNameTextBox.Text;
                    //RawServerAddress = ServerAddressComboBox.Text;
                }
                else
                {
                    // invalid address
                }
            }
            else
            {
                ArenaSessionClient.Send(Protocol1Constants.COMMAND_LOGOUT);
                ArenaTunnelClient.Disconnect();

                ServerLoginButton.Text = SERVER_LOGIN;
                ServerAddressComboBox.Enabled = true;
            }
        }

        void ReplacePlayerList(string[] players)
        {
            Invoke((MethodInvoker)delegate()
            {
                playersTreeView.Nodes.Clear();
                foreach (string p in players)
                {
                    playersTreeView.Nodes.Add(p, p);
                }
            });
        }

        void AddPlayer(string player)
        {
            Invoke((MethodInvoker)delegate()
            {
                playersTreeView.Nodes.Add(player, player);
            });
        }

        void RemovePlayer(string player)
        {
            Invoke((MethodInvoker)delegate()
            {
                playersTreeView.Nodes.RemoveByKey(player);
            });
        }

        void UpdatePing(string player, int ping)
        {
            Invoke((MethodInvoker)delegate()
            {
                int index = playersTreeView.Nodes.IndexOfKey(player);
                if (index != -1)
                {
                    TreeNode n = playersTreeView.Nodes[index];
                    n.Text = player + " (" + ping + ")";
                }
            });
        }

        void ClearRoomList()
        {
            Invoke((MethodInvoker)delegate()
            {
                RoomListView.Items.Clear();
            });
        }

        void AddRoom(string args)
        {
            string[] tokens = args.Split(' ');

            string masterName = tokens[0];
            string maxPlayers = tokens[1];
            string currentPlayers = tokens[2];
            string hasPassword = tokens[3];
            string title = tokens[4];

            RemovePlayer(masterName);

            Invoke((MethodInvoker)delegate()
            {
                ListViewItem item = RoomListView.Items.Add(masterName, masterName, 0);
                item.SubItems.Add(title);
                item.SubItems.Add(currentPlayers + " / " + maxPlayers);
                item.SubItems.Add(hasPassword == "Y" ? "有" : "");
            });
        }

        void UpdateRoom(string args)
        {
            string[] tokens = args.Split(' ');

            string masterName = tokens[0];
            string maxPlayers = tokens[1];
            string currentPlayers = tokens[2];
            string hasPassword = tokens[3];
            string title = tokens[4];

            Invoke((MethodInvoker)delegate()
            {
                int index = RoomListView.Items.IndexOfKey(masterName);
                if (index != -1)
                {
                    ListViewItem item = RoomListView.Items[index];
                    item.SubItems[1].Text = title;
                    item.SubItems[2].Text = currentPlayers + " / " + maxPlayers;
                    item.SubItems[3].Text = hasPassword == "Y" ? "有" : "";
                }

                if (CurrentOperationMode == OperationMode.PlayRoomParticipant && masterName == RoomMasterNameTextBox.Text)
                {
                    int max;
                    if (int.TryParse(maxPlayers, out max))
                        RoomMaxPlayerNumericUpDown.Value = max;
                    RoomTitleTextBox.Text = title;
                    RoomDescriptionTextBox.Text = tokens[5];
                    RoomPasswordTextBox.Text = "";
                }
            });
        }

        void RefreshRoomList(string args)
        {
            string[] tokens = args.Split(' ');

            Invoke(new MethodInvoker(
                delegate()
                {
                    RoomListView.Items.Clear();

                    for (int i = 0; i < tokens.Length; i += 5)
                    {
                        string masterName = tokens[i];
                        string maxPlayers = tokens[i + 1];
                        string currentPlayers = tokens[i + 2];
                        string hasPassword = tokens[i + 3];
                        string title = tokens[i + 4];

                        ListViewItem item = RoomListView.Items.Add(masterName, masterName, 0);
                        item.SubItems.Add(title);
                        item.SubItems.Add(currentPlayers + " / " + maxPlayers);
                        item.SubItems.Add(hasPassword == "Y" ? "有" : "");
                    }
                }));
        }

        void RemoveRoom(string room)
        {
            Invoke((MethodInvoker)delegate()
            {
                RoomListView.Items.RemoveByKey(room);
            });
        }

        void UpdateServerAddress(string type, string address = null)
        {
            MethodInvoker action = (MethodInvoker)delegate()
            {
                if (address == null) address = ServerAddressComboBox.Text;
                serverAddressStripStatusLabel.Text =
                    String.Format("サーバーアドレス  {1}  ({0}) ", type, address);
            };
            Invoke(action);
        }

        void GoTo(OperationMode mode)
        {
            MethodInvoker action = new MethodInvoker(delegate()
            {
                CurrentOperationMode = mode;

                switch (mode)
                {
                    case OperationMode.Portal:
                        ArenaServerListView.Items.Clear();
                        RoomListView.Items.Clear();
                        LocationTabControl.SelectedTab = PortalTabPage;
                        EnableRoomEditFormItems(false);
                        break;
                    case OperationMode.ArenaLobby:
                        ArenaServerListView.Items.Clear();
                        RoomListView.Items.Clear();
                        LocationTabControl.SelectedTab = ArenaTabPage;
                        RoomCreateEditButton.Text = "部屋を作成";
                        RoomCreateEditButton.Enabled = true;
                        RoomCloseExitButton.Text = "部屋を閉じる";
                        RoomCloseExitButton.Enabled = false;
                        RoomMasterNameTextBox.Text = LoginUserName;
                        EnableRoomEditFormItems(true);
                        break;
                    case OperationMode.PlayRoomMaster:
                        LocationTabControl.SelectedTab = PlayRoomTabPage;
                        RoomCreateEditButton.Text = "部屋を修正";
                        RoomCreateEditButton.Enabled = true;
                        RoomCloseExitButton.Text = "部屋を閉じる";
                        RoomCloseExitButton.Enabled = true;
                        RoomMasterNameTextBox.Text = LoginUserName;
                        EnableRoomEditFormItems(true);
                        break;
                    case OperationMode.PlayRoomParticipant:
                        LocationTabControl.SelectedTab = PlayRoomTabPage;
                        RoomCreateEditButton.Enabled = false;
                        RoomCloseExitButton.Text = "退室する";
                        RoomCloseExitButton.Enabled = true;
                        EnableRoomEditFormItems(false);
                        break;
                    case OperationMode.Offline:
                        ArenaServerListView.Items.Clear();
                        RoomListView.Items.Clear();
                        playersTreeView.Nodes.Clear();

                        RoomCreateEditButton.Enabled = false;
                        RoomCloseExitButton.Enabled = false;
                        EnableRoomEditFormItems(false);

                        ServerLoginButton.Text = SERVER_LOGIN;
                        ServerLoginButton.Enabled = true;

                        ServerAddressComboBox.Enabled = true;
                        playersTreeView.Nodes.Clear();

                        ResetServerStatusBar();
                        LoginUserNameTextBox.Enabled = true;
                        break;
                    case OperationMode.ConnectingToServer:
                        LoginUserNameTextBox.Enabled = false;
                        MainTabControl.SelectedTab = chatTagPage;
                        break;
                    default:
                        break;
                }
            });

            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        void EnableRoomEditFormItems(bool enable)
        {
            RoomTitleTextBox.Enabled = enable;
            RoomPasswordTextBox.Enabled = enable;
            RoomMaxPlayerNumericUpDown.Enabled = enable;
            RoomDescriptionTextBox.Enabled = enable;
        }

        class SessionHandler : IAsyncClientHandler
        {
            ArenaClientForm Form = null;

            delegate void MessageDelegate(string args);
            Dictionary<string, MessageDelegate> Delegators = new Dictionary<string, MessageDelegate>();

            public SessionHandler(ArenaClientForm form)
            {
                this.Form = form;

                Delegators[Protocol1Constants.SERVER_PORTAL] = new MessageDelegate(ServerPortalHandler);
                Delegators[Protocol1Constants.SERVER_ARENA] = new MessageDelegate(ServerArenaHandler);
                Delegators[Protocol1Constants.COMMAND_LOGIN] = new MessageDelegate(LoginHandler);
                Delegators[Protocol1Constants.COMMAND_CHAT] = new MessageDelegate(ChatHandler);
                Delegators[Protocol1Constants.COMMAND_PINGBACK] = new MessageDelegate(PingBackHandler);
                Delegators[Protocol1Constants.COMMAND_INFORM_PING] = new MessageDelegate(InformPingHandler);
                Delegators[Protocol1Constants.COMMAND_SERVER_STATUS] = new MessageDelegate(ServerStatusHandler);
                Delegators[Protocol1Constants.NOTIFY_USER_ENTERED] = new MessageDelegate(NotifyUserEnteredHandler);
                Delegators[Protocol1Constants.NOTIFY_USER_EXITED] = new MessageDelegate(NotifyUserExitedHandler);
                Delegators[Protocol1Constants.NOTIFY_USER_LIST] = new MessageDelegate(NotifyUserListHandler);
                Delegators[Protocol1Constants.NOTIFY_ROOM_CREATED] = new MessageDelegate(NotifyRoomCreatedHandler);
                Delegators[Protocol1Constants.NOTIFY_ROOM_DELETED] = new MessageDelegate(NotifyRoomDeletedHandler);
                Delegators[Protocol1Constants.NOTIFY_ROOM_LIST] = new MessageDelegate(NotifyRoomListHandler);
                Delegators[Protocol1Constants.NOTIFY_ROOM_UPDATED] = new MessageDelegate(NotifyRoomUpdatedHandler);
                //Delegators[Protocol1Constants.NOTIFY_ROOM_PLAYER_KICKED] = new MessageDelegate(NotifyRoomKickedHandler);
                Delegators[Protocol1Constants.COMMAND_ROOM_CREATE] = new MessageDelegate(CommandRoomCreateHandler);
                Delegators[Protocol1Constants.COMMAND_ROOM_UPDATE] = new MessageDelegate(CommandRoomUpdateHandler);
                Delegators[Protocol1Constants.COMMAND_ROOM_DELETE] = new MessageDelegate(CommandRoomDeleteHandler);
                Delegators[Protocol1Constants.COMMAND_ROOM_ENTER] = new MessageDelegate(CommandRoomEnterHandler);
                Delegators[Protocol1Constants.COMMAND_ROOM_EXIT] = new MessageDelegate(CommandRoomExitHandler);
                Delegators[Protocol1Constants.COMMAND_ROOM_KICK_PLAYER] = new MessageDelegate(CommandRoomKickPlayerHandler);
                Delegators[Protocol1Constants.COMMAND_INFORM_TUNNEL_UDP_PORT] = new MessageDelegate(InformTunnelPortHandler);
                Delegators[Protocol1Constants.ERROR_VERSION_MISMATCH] = new MessageDelegate(ErrorVersionMismatch);
                Delegators[Protocol1Constants.ERROR_LOGIN_DUPLICATED_NAME] = new MessageDelegate(ErrorLoginDuplicatedNameHandler);
                Delegators[Protocol1Constants.ERROR_LOGIN_BEYOND_CAPACITY] = new MessageDelegate(ErrorLoginBeyondCapacityHandler);
                Delegators[Protocol1Constants.NOTIFY_ROOM_PASSWORD_REQUIRED] = new MessageDelegate(NotifyRoomPasswordRequiredHandler);
                Delegators[Protocol1Constants.ERROR_ROOM_ENTER_PASSWORD_FAIL] = new MessageDelegate(ErrorRoomEnterPasswordFailHandler);
                Delegators[Protocol1Constants.ERROR_ROOM_ENTER_BEYOND_CAPACITY] = new MessageDelegate(ErrorRoomEnterBeyondCapacityHandler);
                Delegators[Protocol1Constants.ERROR_ROOM_CREATE_BEYOND_LIMIT] = new MessageDelegate(ErrorRoomCreateBeyondLimitHandler);
            }

            public void ConnectCallback(IPEndPoint localEndPoint)
            {
                Form.GoTo(OperationMode.ConnectingToServer);
                Form.AppendToChatTextBox("サーバーに接続しました", Color.Blue);

                Form.Invoke(new MethodInvoker(
                    delegate()
                    {
                        Form.ServerLoginButton.Text = SERVER_LOGOUT;
                        Form.ServerLoginButton.Enabled = true;

                        string server = Form.ServerAddressComboBox.Text;

                        int index = Form.ServerHistory.IndexOf(server);

                        if (index == -1)
                        {
                            if (Form.ServerHistory.Count >= MaxServerHistory)
                            {
                                Form.ServerHistory.RemoveAt(0);
                                Form.ServerHistory.Add(server);

                                Form.ServerAddressComboBox.Items.RemoveAt(Form.ServerAddressComboBox.Items.Count - 1);
                            }
                            else
                            {
                                Form.ServerHistory.Add(server);
                            }

                            Form.ServerAddressComboBox.Items.Insert(Form.ServerListCount + 1, server);
                        }
                        else if (index != Form.ServerHistory.Count - 1)
                        {
                            Form.ServerHistory.RemoveAt(index);
                            Form.ServerHistory.Add(server);

                            int comboboxIndex = Form.ServerListCount + (Form.ServerHistory.Count - index);
                            Form.ServerAddressComboBox.Items.RemoveAt(comboboxIndex);
                            Form.ServerAddressComboBox.Items.Insert(Form.ServerListCount + 1, server);
                            Form.ServerAddressComboBox.SelectedIndex = Form.ServerListCount + 1;
                        }
                    }));

                Form.ArenaSessionClient.Send(Protocol1Constants.COMMAND_VERSION + " " + Protocol1Constants.PROTOCOL_NUMBER);
                Form.ArenaSessionClient.Send(Protocol1Constants.COMMAND_LOGIN + " " + Form.LoginUserName);
            }
            public void DisconnectCallback()
            {
                string log;
                if (Form.CurrentOperationMode == OperationMode.Offline)
                    log = "サーバーにアクセスできません";
                else
                    log = "サーバーと切断しました";

                Form.ArenaTunnelClient.Disconnect();
                Form.GoTo(OperationMode.Offline);

                Form.AppendToChatTextBox(log, Color.Blue);
            }
            public void ReadCallback(PacketData data)
            {
                try
                {
                    string[] messages = data.Messages;
                    foreach (string message in messages)
                    {
                        int commandEndIndex = message.IndexOf(' ');
                        string command, argument;
                        if (commandEndIndex > 0)
                        {
                            command = message.Substring(0, commandEndIndex);
                            argument = message.Substring(commandEndIndex + 1);
                        }
                        else
                        {
                            command = message;
                            argument = string.Empty;
                        }
                        //Form.AppendToLogTextBox(message, Color.Red);

                        if (Delegators.ContainsKey(command))
                        {
                            MessageDelegate action = Delegators[command];
                            action(argument);
                        }
                        else
                        {
                            Form.AppendToLogTextBox(message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Form.AppendToLogTextBox(e.ToString());
                }
            }
            public void SendCallback(int bytesSent)
            {
            }
            public void Log(string message)
            {
                //Form.AppendToLogTextBox(message, Color.Red);
            }

            void ErrorVersionMismatch(string args)
            {
                string message = String.Format("サーバーとのプロトコルナンバーが一致ないので接続できません サーバー:{0} クライアント:{1}"
                    , args, Protocol1Constants.PROTOCOL_NUMBER);
                Form.AppendToChatTextBox(message, Color.Red);
            }

            void ServerPortalHandler(string args)
            {
                Form.UpdateServerAddress("ポータル");
                Form.GoTo(OperationMode.Portal);
            }

            void ServerArenaHandler(string args)
            {
                Form.UpdateServerAddress("アリーナ");
                Form.GoTo(OperationMode.ArenaLobby);
            }

            void LoginHandler(string args)
            {
                Form.ArenaTunnelClient.Connect(Form.ServerIpEndPoint);
                Form.AppendToChatTextBox(Form.LoginUserName + " が入室しました", Color.Gray);

                ThreadPool.QueueUserWorkItem(new WaitCallback(Form.PingLoop));
            }

            void ChatHandler(string args)
            {
                string chat = args;
                Form.AppendToChatTextBox(chat, Color.Black);
            }

            void PingBackHandler(string args)
            {
                long packetTicks;
                if (Int64.TryParse(args, out packetTicks))
                {
                    int ping = (int)(System.DateTime.Now.Ticks - packetTicks) / 10000;
                    //Form.AppendToChatTextBox(String.Format("PING: {0}", ping), Color.Green);

                    Form.UpdatePing(Form.LoginUserName, ping);
                    Form.ArenaSessionClient.Send(Protocol1Constants.COMMAND_INFORM_PING + " " + ping);
                }
            }

            void InformPingHandler(string args)
            {
                //Form.AppendToLogTextBox(args, Color.Black);
                string[] tokens = args.Split(' ');
                if (tokens.Length == 2)
                {
                    string player = tokens[0];
                    string strPing = tokens[1];
                    int ping;
                    if (int.TryParse(strPing, out ping))
                    {
                        Form.UpdatePing(player, ping);
                    }
                }
            }

            void ServerStatusHandler(string args)
            {
                string[] values = args.Split(' ');
                Form.Invoke((MethodInvoker)delegate()
                {
                    Form.serverStatusStripStatusLabel.Text = String.Format(
                        "参加者数: {0} / {1}     部屋数: {2} / {3}",
                        values[0], values[1], values[2], values[3]);
                });
            }

            void NotifyUserEnteredHandler(string args)
            {
                string name = args;
                Form.AddPlayer(name);
                Form.AppendToChatTextBox(name + " が入室しました", Color.Gray, Form.ChatLogNotDisplayRoomActionCheckBox);
            }

            void NotifyUserExitedHandler(string args)
            {
                string name = args;
                Form.RemovePlayer(name);
                Form.AppendToChatTextBox(name + " が退室しました", Color.Gray, Form.ChatLogNotDisplayRoomActionCheckBox);
            }

            void NotifyUserListHandler(string args)
            {
                string[] players = args.Split(' ');
                Form.ReplacePlayerList(players);
            }

            void NotifyRoomCreatedHandler(string args)
            {
                Form.AddRoom(args);
            }

            void NotifyRoomDeletedHandler(string args)
            {
                string room = args;
                Form.RemoveRoom(room);
            }

            void NotifyRoomListHandler(string args)
            {
                if (args.Length > 0)
                {
                    Form.RefreshRoomList(args);
                }
                else
                    Form.ClearRoomList();
            }

            void NotifyRoomUpdatedHandler(string args)
            {
                Form.UpdateRoom(args);
            }

            void CommandRoomCreateHandler(string args)
            {
                Form.ReplacePlayerList(new string[] { Form.LoginUserName });

                Form.AppendToChatTextBox("部屋を作成しました", Color.Gray);
                Form.GoTo(OperationMode.PlayRoomMaster);
            }

            void CommandRoomUpdateHandler(string args)
            {
                Form.AppendToChatTextBox("部屋情報を修正しました", Color.Gray);
            }

            void CommandRoomDeleteHandler(string args)
            {
                if (Form.CurrentOperationMode == OperationMode.PlayRoomMaster)
                {
                    Form.AppendToChatTextBox("部屋を閉じました", Color.Gray);
                }
                else if (Form.CurrentOperationMode == OperationMode.PlayRoomParticipant)
                {
                    Form.AppendToChatTextBox("部屋が閉じられました", Color.Gray);
                }

                Form.GoTo(OperationMode.ArenaLobby);
            }

            void CommandRoomEnterHandler(string args)
            {
                string[] tokens = args.Split(' ');
                string masterName = tokens[0];

                Form.AppendToChatTextBox(masterName + " 部屋に入りました", Color.Gray);
                Form.GoTo(OperationMode.PlayRoomParticipant);

                Form.Invoke(new MethodInvoker(
                    delegate()
                    {
                        Form.RoomMaxPlayerNumericUpDown.Value = Decimal.Parse(tokens[1]);
                        Form.RoomMasterNameTextBox.Text = masterName;
                        Form.RoomTitleTextBox.Text = tokens[2];
                        Form.RoomDescriptionTextBox.Text = tokens[3];
                        Form.RoomPasswordTextBox.Text = "";
                    }));
            }

            void CommandRoomExitHandler(string args)
            {
                Form.GoTo(OperationMode.ArenaLobby);

                //Form.Invoke(new MethodInvoker(
                //    delegate()
                //    {
                //        Form.RoomCreateEditButton.Enabled = true;
                //        Form.RoomCloseExitButton.Enabled = false;
                //        Form.LocationTabControl.SelectedTab = Form.arenaTagPage;
                //    }));

                Form.AppendToChatTextBox("ロビーに出ました", Color.Gray);
            }

            void CommandRoomKickPlayerHandler(string args)
            {
                if (args == Form.LoginUserName)
                {
                    Form.GoTo(OperationMode.ArenaLobby);
                    Form.AppendToChatTextBox("部屋から追い出されました", Color.Purple);
                }
                else
                {
                    Form.RemovePlayer(args);
                    if (Form.CurrentOperationMode == OperationMode.PlayRoomMaster)
                        Form.AppendToChatTextBox(args + " を部屋から追い出しました", Color.Purple);
                    else
                        Form.AppendToChatTextBox(args + " は部屋から追い出されました", Color.Purple);
                }
            }

            void InformTunnelPortHandler(string args)
            {
                Form.TunnelIsLinked = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(Form.MaintainingNatConnectionLoop));
                ThreadPool.QueueUserWorkItem(new WaitCallback(Form.PacketMonitorLoop));
                Form.AppendToLogTextBox("トンネル通信の接続が開始しました");
            }

            void ErrorLoginDuplicatedNameHandler(string args)
            {
                Form.AppendToChatTextBox("同名のユーザーが既にログインしているのでログインできません", Color.Red);
            }

            void ErrorLoginBeyondCapacityHandler(string args)
            {
                Form.AppendToChatTextBox("サーバーの最大人数を超えたのでログインできません", Color.Red);
            }

            void NotifyRoomPasswordRequiredHandler(string masterName)
            {
                Form.Invoke(new MethodInvoker(
                    delegate()
                    {
                        PasswordDialog d = new PasswordDialog();
                        //d.Text = "部屋にパスワードが設定されています";
                        DialogResult res = d.ShowDialog();

                        string password = string.Empty;
                        if (res == DialogResult.OK)
                        {
                            password = d.Password.Trim();
                        }

                        d.Dispose();

                        if (password == string.Empty)
                        {
                            Form.AppendToChatTextBox("入室をキャンセルしました", Color.Purple);
                        }
                        else
                        {
                            string message = String.Format("{0} {1} {2}",
                                Protocol1Constants.COMMAND_ROOM_ENTER, masterName, password);
                            Form.ArenaSessionClient.Send(message);
                        }
                    }));
            }

            void ErrorRoomEnterPasswordFailHandler(string args)
            {
                Form.AppendToChatTextBox("部屋パスワードが一致しないので入室できません", Color.Purple);
            }

            void ErrorRoomEnterBeyondCapacityHandler(string args)
            {
                Form.AppendToChatTextBox("部屋が満室なので入れません", Color.Purple);
            }

            void ErrorRoomCreateBeyondLimitHandler(string args)
            {
                Form.AppendToChatTextBox("部屋数が上限に達しましたので部屋を作成できません", Color.Purple);
                Form.Invoke(new MethodInvoker(delegate()
                    {
                        Form.RoomCloseExitButton.Enabled = false;
                        Form.RoomCloseExitButton.Enabled = true;
                    }));
            }
        }

        private void LanAdapterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lanAdapterComboBox.SelectedIndex;
            if (index == 0)
            {
                toggleCaptureLanAdapterButton.Enabled = false;
            }
            //else if (index == lanAdapterComboBox.Items.Count - 1)
            //{
            //    RefreshLanAdapterList();
            //}
            else
            {
                toggleCaptureLanAdapterButton.Enabled = true;
            }
        }

        void ProcessCapturedPacket(PacketDotNet.Packet packet)
        {
            if (!(packet is PacketDotNet.EthernetPacket)) return;
            var eth = (PacketDotNet.EthernetPacket)packet;

            byte[] bytes = eth.Bytes;
            if (!Utility.IsPspPacket(bytes, Utility.HEADER_OFFSET))
            {
                // PSPのパケットではないのでスルー
                return;
            }
            string src = eth.SourceHwAddress.ToString();
            string dest = eth.DestinationHwAddress.ToString();

            // サーバーから送られてきた他プレイヤーのPSPパケットか判別
            if (RemoteMacAddresses.ContainsKey(src))
            {
                //string log = String.Format("PSP Packet: サーバーから自PSPへ転送するパケットなのでスルーします: {0} -> {1}", src, desc);
                //AppendToLogTextBox(log, Color.DeepPink);
                return;
            }

            bool packetToRemotePSP = true;
            TraficStatistics myStats;
            lock (MyMacAddresses)
            {
                if (MyMacAddresses.ContainsKey(src))
                {
                    myStats = MyMacAddresses[src];
                }
                else
                {
                    myStats = new TraficStatistics();
                    MyMacAddresses[src] = myStats;
                }

                // 手元に送信先MACアドレスのPSPがあるか判別
                if (MyMacAddresses.ContainsKey(dest))
                {
                    packetToRemotePSP = false;
                    // not need tunneling
                    //string log = String.Format("PSP Packet: Local trafic from {0} -> {1}", src, desc);
                    //AppendToLogTextBox(log, Color.Purple);
                }
            }

            if (packetToRemotePSP && TunnelIsLinked)
            {
                TraficStatistics remoteStats;
                lock (RemoteMacAddresses)
                {
                    if (RemoteMacAddresses.ContainsKey(dest))
                    {
                        remoteStats = RemoteMacAddresses[dest];
                    }
                    else
                    {
                        remoteStats = new TraficStatistics();
                        RemoteMacAddresses[dest] = remoteStats;
                    }
                }

                //string log = String.Format("PSP Packet: {0} -> {1} Size: {2}", src, desc, bytes.Length);
                if (CurrentOperationMode == OperationMode.PlayRoomMaster || CurrentOperationMode == OperationMode.PlayRoomParticipant)
                {
                    myStats.lastModified = System.DateTime.Now.Ticks;
                    myStats.currentOutBytes += bytes.Length;
                    myStats.totalOutBytes += bytes.Length;

                    remoteStats.lastModified = System.DateTime.Now.Ticks;
                    remoteStats.currentOutBytes += bytes.Length;
                    remoteStats.totalOutBytes += bytes.Length;

                    if (Utility.HEADER_OFFSET == 0)
                    {
                        //AppendToLogTextBox(System.DateTime.Now.Ticks.ToString(), Color.Black);
                        ArenaTunnelClient.Send(bytes.ToArray());
                    }
                    //else
                    //{
                    //    //long timestamp = Environment.TickCount;
                    //    long timestamp = System.DateTime.Now.Ticks;
                    //    AppendToLogTextBox(timestamp.ToString());
                    //    byte[] ticks = BitConverter.GetBytes(timestamp);
                    //    ArenaTunnelClient.Send(ticks.Concat(bytes).ToArray());
                    //}
                }
            }
        }

        void LanAdapterOnPacketArrival(object sender, CaptureEventArgs e)
        {
            try
            {
                PacketDotNet.Packet packet = PacketDotNet.Packet.ParsePacket(e.Packet);
                ProcessCapturedPacket(packet);

            }
            catch (Exception) { }
        }

        void BackgroundStopCapture(object o)
        {
            LivePcapDevice device = (LivePcapDevice)o;
            try
            {
                device.StopCapture();
            }
            catch (PcapException) { }

            if (!FormIsClosing)
                Invoke((MethodInvoker)delegate()
                {
                    toggleCaptureLanAdapterButton.Text = "PSPと通信開始";
                    lanAdapterComboBox.Enabled = true;
                    toggleCaptureLanAdapterButton.Enabled = true;
                });
        }

        bool PacketCapturing = false;
        void LanAdapterPacketCaptureLoop(object o)
        {
            PacketDotNet.RawPacket rawPacket;
            while (PacketCapturing)
            {
                try
                {
                    while ((rawPacket = CurrentPcapDevice.GetNextPacket()) != null)
                    {
                        //if (!PacketCapturing) break;

                        PacketDotNet.Packet packet = PacketDotNet.Packet.ParsePacket(rawPacket);
                        ProcessCapturedPacket(packet);
                    }
                }
                catch (Exception)
                {
                    //break;
                }
            }

            if (!FormIsClosing)
                Invoke((MethodInvoker)delegate()
                {
                    toggleCaptureLanAdapterButton.Text = "PSPと通信開始";
                    lanAdapterComboBox.Enabled = true;
                    toggleCaptureLanAdapterButton.Enabled = true;
                });
        }

        private void ToggleCaptureLanAdapterButton_Click(object sender, EventArgs e)
        {
            int index = lanAdapterComboBox.SelectedIndex;
            if (index == 0 || index >= lanAdapterComboBox.Items.Count) return;

            CurrentPcapDevice = LanAdapterList[index - 1];
            //if (CurrentPcapDevice.Started)
            if (PacketCapturing)
            {
                toggleCaptureLanAdapterButton.Enabled = false;
                //ThreadPool.QueueUserWorkItem(new WaitCallback(BackgroundStopCapture), CurrentPcapDevice);
                PacketCapturing = false;
            }
            else
            {
                try
                {
                    lanAdapterComboBox.Enabled = false;

                    if (!CurrentPcapDevice.Opened)
                    {
                        CurrentPcapDevice.Open(DeviceMode.Promiscuous, 1);
                        //CurrentPcapDevice.OnPacketArrival += new PacketArrivalEventHandler(LanAdapterOnPacketArrival);
                    }
                }
                catch (Exception ex)
                {
                    AppendToLogTextBox(ex.ToString());
                    lanAdapterComboBox.Enabled = true;
                    return;
                }

                //CurrentPcapDevice.StartCapture();

                PacketCapturing = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(LanAdapterPacketCaptureLoop), CurrentPcapDevice);

                toggleCaptureLanAdapterButton.Text = "PSPと通信停止";
            }
        }

        const string MAC_BROADCAST_ADDRESS = "FFFFFFFFFFFF";

        class TunnelHandler : IAsyncClientHandler
        {
            ArenaClientForm Form = null;
            public TunnelHandler(ArenaClientForm form) { this.Form = form; }

            public void ConnectCallback(IPEndPoint localEndPoint)
            {
                Form.ArenaTunnelClient.Send(" "); // packet for linking tunnel to tcp session
            }
            public void ReadCallback(PacketData data)
            {
                byte[] packet = data.RawBytes;
                if (Utility.IsPspPacket(packet, Utility.HEADER_OFFSET))
                {
                    PhysicalAddress srcMac = new PhysicalAddress(packet.Skip(Utility.HEADER_OFFSET + 6).Take(6).ToArray());
                    string src = srcMac.ToString();

                    PhysicalAddress destMac = new PhysicalAddress(packet.Skip(Utility.HEADER_OFFSET).Take(6).ToArray());
                    string dest = destMac.ToString();

                    TraficStatistics remoteStats;
                    // 再度パケットキャプチャされた時にサーバーへ再送しないために記録
                    lock (Form.RemoteMacAddresses)
                    {
                        //long ticks = Utility.HEADER_OFFSET == 0 ? System.DateTime.Now.Ticks : BitConverter.ToInt64(packet, 0);
                        if (Form.RemoteMacAddresses.ContainsKey(src))
                        {
                            remoteStats = Form.RemoteMacAddresses[src];
                            //if (lastTicks > ticks) // 古いパケットはスルー
                            //    return;
                        }
                        else
                        {
                            remoteStats = new TraficStatistics();
                            Form.RemoteMacAddresses[src] = remoteStats;
                        }
                    }

                    //PhysicalAddress destMac = new PhysicalAddress(packet.Skip(Utility.HEADER_OFFSET).Take(6).ToArray());
                    //string log = String.Format("Packet To My PSP: {0} -> {1} Size={2}", src, destMac, packet.Length);
                    //Form.AppendToLogTextBox(log, Color.Black);

                    if (Form.CurrentPcapDevice != null && Form.PacketCapturing)// Form.CurrentPcapDevice.Started)
                    {
                        remoteStats.lastModified = System.DateTime.Now.Ticks;
                        remoteStats.currentInBytes += packet.Length;
                        remoteStats.totalInBytes += packet.Length;

                        TraficStatistics myStats = null;
                        lock (Form.MyMacAddresses)
                            if (Form.MyMacAddresses.ContainsKey(dest))
                            {
                                myStats = Form.MyMacAddresses[dest];
                                //dest = MAC_BROADCAST_ADDRESS;
                                //packet[0] = 0xFF;
                                //packet[1] = 0xFF;
                                //packet[2] = 0xFF;
                                //packet[3] = 0xFF;
                                //packet[4] = 0xFF;
                                //packet[5] = 0xFF;
                            }
                            else if (dest != MAC_BROADCAST_ADDRESS)
                            {
                                myStats = new TraficStatistics();
                                Form.MyMacAddresses[dest] = myStats;
                            }

                        if (myStats == null) // dest == 'FFFFFFFFFFFF"
                        {
                            lock (Form.RemoteMacAddresses)
                            {
                                if (Form.RemoteMacAddresses.ContainsKey(dest))
                                {
                                    myStats = Form.RemoteMacAddresses[dest];
                                }
                                else
                                {
                                    myStats = new TraficStatistics();
                                    Form.RemoteMacAddresses[dest] = myStats;
                                }
                            }
                        }

                        myStats.lastModified = System.DateTime.Now.Ticks;
                        myStats.currentInBytes += packet.Length;
                        myStats.totalInBytes += packet.Length;

                        if (Utility.HEADER_OFFSET == 0)
                            Form.CurrentPcapDevice.SendPacket(packet);
                        else
                            Form.CurrentPcapDevice.SendPacket(packet.Skip(Utility.HEADER_OFFSET).ToArray());
                    }
                }
                else
                {
                    string routerTunnelPort = data.Messages[0];
                    //Form.AppendToLogTextBox("UDP : Informed my tunnel port " + routerTunnelPort, Color.Black);
                    int port;
                    if (Int32.TryParse(routerTunnelPort, out port))
                    {
                        Form.ArenaSessionClient.Send(Protocol1Constants.COMMAND_INFORM_TUNNEL_UDP_PORT + " " + port);
                    }
                }
            }
            public void SendCallback(int bytesSent)
            {
            }
            public void DisconnectCallback()
            {
                Form.TunnelIsLinked = false;
                Form.AppendToLogTextBox("トンネル通信の接続が終了しました");
            }
            public void Log(string message)
            {
                //Form.AppendToLogTextBox(message, Color.Blue);
            }
        }

        private void RoomCreateEditButton_Click(object sender, EventArgs e)
        {
            if (CurrentOperationMode == OperationMode.ArenaLobby)
            {
                string title = RoomTitleTextBox.Text.Trim();
                if (title == string.Empty)
                {
                    AppendToChatTextBox("部屋名を入力してください", Color.Red);
                    RoomTitleTextBox.Focus();
                    return;
                }

                //RoomCreateEditButton.Enabled = false;

                StringBuilder sb = new StringBuilder();
                sb.Append(Protocol1Constants.COMMAND_ROOM_CREATE);
                sb.Append(' ');
                sb.Append(RoomMaxPlayerNumericUpDown.Value);
                sb.Append(' ');
                sb.Append(title);
                sb.Append(" \"");
                sb.Append(RoomDescriptionTextBox.Text.Trim());
                sb.Append("\" \"");
                sb.Append(RoomPasswordTextBox.Text);
                sb.Append("\"");

                ArenaSessionClient.Send(sb.ToString());
                RoomCloseExitButton.Enabled = true;
            }
            else if (CurrentOperationMode == OperationMode.PlayRoomMaster)
            {
                string title = RoomTitleTextBox.Text.Trim();
                if (title == string.Empty)
                {
                    AppendToChatTextBox("部屋名を入力してください", Color.Red);
                    RoomTitleTextBox.Focus();
                    return;
                }

                //RoomCreateEditButton.Enabled = false;

                StringBuilder sb = new StringBuilder();
                sb.Append(Protocol1Constants.COMMAND_ROOM_UPDATE);
                sb.Append(' ');
                sb.Append(RoomMaxPlayerNumericUpDown.Value);
                sb.Append(' ');
                sb.Append(title);
                sb.Append(" \"");
                sb.Append(RoomDescriptionTextBox.Text.Trim());
                sb.Append("\" \"");
                sb.Append(RoomPasswordTextBox.Text);
                sb.Append("\"");

                ArenaSessionClient.Send(sb.ToString());
            }
        }

        private void RoomCloseExitButton_Click(object sender, EventArgs e)
        {
            RoomCloseExitButton.Enabled = false;
            switch (CurrentOperationMode)
            {
                case OperationMode.PlayRoomMaster:
                    ArenaSessionClient.Send(Protocol1Constants.COMMAND_ROOM_DELETE);
                    RoomCreateEditButton.Enabled = true;
                    break;
                case OperationMode.PlayRoomParticipant:
                    ArenaSessionClient.Send(Protocol1Constants.COMMAND_ROOM_EXIT);
                    break;
            }
        }

        private void locationTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //switch (CurrentLocaltionMode)
            //{
            //    case LocaltionMode.Portal:
            //        break;
            //    case LocaltionMode.Arena:
            //        if (locationTabControl.SelectedTab == arenaTagPage)
            //            locationTabControl.SelectedTab = arenaTagPage;
            //        break;
            //    case LocaltionMode.PlayRoom:
            //        if (locationTabControl.SelectedTab != playRoomTagPage)
            //            locationTabControl.SelectedTab = playRoomTagPage;
            //        break;
            //}
        }

        private void RoomListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (RoomListView.Items.Count == 0 || CurrentOperationMode != OperationMode.ArenaLobby)
                return;

            ListViewItem item = RoomListView.GetItemAt(e.X, e.Y);
            ArenaSessionClient.Send(Protocol1Constants.COMMAND_ROOM_ENTER + " " + item.Name);
        }

        private void ServerAddressComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Space)
            {
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                ServerLoginButton_Click(sender, null);
                e.Handled = true;
            }
        }

        private void ServerAddressComboBox_TextChanged(object sender, EventArgs e)
        {
            ServerAddressComboBox.Text = ServerAddressComboBox.Text.Replace(" ", "");
        }

        private void LoginUserNameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Space)
            {
                e.Handled = true;
            }
        }

        private void LoginUserNameTextBox_TextChanged(object sender, EventArgs e)
        {
            LoginUserNameTextBox.Text = LoginUserNameTextBox.Text.Replace(" ", "");
            MyIniParser.SetSetting(IniConstants.SECTION_SETTINGS, IniConstants.CLIENT_LOGIN_NAME, LoginUserNameTextBox.Text);

            LoginUserNameAlertLabel.Visible = LoginUserNameTextBox.Text.Length == 0;

            if (LoginUserNameTextBox.Text.Length > 0)
                this.Text = LoginUserNameTextBox.Text + " - " + ApplicationConstants.APP_NAME;
            else
                this.Text = ApplicationConstants.APP_NAME;
        }

        private void RoomTitleTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Space)
            {
                e.Handled = true;
            }
        }

        private void RoomTitleTextBox_TextChanged(object sender, EventArgs e)
        {
            RoomTitleTextBox.Text = RoomTitleTextBox.Text.Replace(" ", "");
        }

        private void RoomPasswordTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Space)
            {
                e.Handled = true;
            }
        }

        private void RoomPasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            RoomPasswordTextBox.Text = RoomPasswordTextBox.Text.Replace(" ", "");
        }

        private void RoomDescriptionTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Space)
            {
                e.Handled = true;
            }
        }

        private void RoomDescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            RoomDescriptionTextBox.Text = RoomDescriptionTextBox.Text.Replace(" ", "");
        }

        private void PlayerListContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == KickPlayer)
            {
                TreeNode p = playersTreeView.SelectedNode;
                if (p != null)
                {
                    ArenaSessionClient.Send(Protocol1Constants.COMMAND_ROOM_KICK_PLAYER + " " + p.Name);
                }
            }
        }

        private void playersTreeView_MouseUp(object sender, MouseEventArgs e)
        {
            TreeNode p = playersTreeView.GetNodeAt(e.X, e.Y);
            if (e.Button == MouseButtons.Right)
            {
                if (p != null && p.Name != RoomMasterNameTextBox.Text && CurrentOperationMode == OperationMode.PlayRoomMaster)
                {
                    PlayerListContextMenu.Show(playersTreeView, new Point(e.X, e.Y));
                }
            }

            if (p != null)
                playersTreeView.SelectedNode = p;
        }

        private void PacketMonitorMyPspClearButton_Click(object sender, EventArgs e)
        {
            lock (MyMacAddresses)
                foreach (KeyValuePair<string, TraficStatistics> p in MyMacAddresses)
                {
                    TraficStatistics stats = p.Value;
                    stats.ClearTotal();
                }
        }

        private void PacketMonitorRemotePspClearButton_Click(object sender, EventArgs e)
        {
            lock (RemoteMacAddresses)
                foreach (KeyValuePair<string, TraficStatistics> p in RemoteMacAddresses)
                {
                    TraficStatistics stats = p.Value;
                    stats.ClearTotal();
                }
        }

        private void ServerAddressComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ServerAddressComboBox.SelectedIndex == ServerListCount)
            {
                ServerAddressComboBox.SelectedIndex = ServerAddressComboBoxIndex;
            }
            else
            {
                ServerAddressComboBoxIndex = ServerAddressComboBox.SelectedIndex;
            }
        }
    }
}
