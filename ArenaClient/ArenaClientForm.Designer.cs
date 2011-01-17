namespace ArenaClient
{
    partial class ArenaClientForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "192.168.1.2:30000",
            "下位用サーバです",
            "11/31",
            "5 / 10"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "192.168.0.1:30000",
            "上位鯖",
            "1 / 4",
            "1 / 1"}, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "mhp.server.com",
            "あああ",
            "112 / 200",
            "37 / 50"}, -1);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "test",
            "HR3キークエ",
            "3 / 4"}, -1);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "helloworld",
            "HR2　まったりクエ回し",
            "4 / 4"}, -1);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
            "foobar",
            "ジンオウガ素材集め",
            "2 / 4"}, -1);
            this.CommandTextBox = new System.Windows.Forms.TextBox();
            this.CommandSubmitButton = new System.Windows.Forms.Button();
            this.ServerLoginButton = new System.Windows.Forms.Button();
            this.ServerAddressComboBox = new System.Windows.Forms.ComboBox();
            this.labelServerAddress = new System.Windows.Forms.Label();
            this.lanAdapterComboBox = new System.Windows.Forms.ComboBox();
            this.toggleCaptureLanAdapterButton = new System.Windows.Forms.Button();
            this.labelLanAdapter = new System.Windows.Forms.Label();
            this.playerListLabel = new System.Windows.Forms.Label();
            this.LocationTabControl = new System.Windows.Forms.TabControl();
            this.PortalTabPage = new System.Windows.Forms.TabPage();
            this.ArenaServerListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ArenaTabPage = new System.Windows.Forms.TabPage();
            this.RoomListView = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PlayRoomTabPage = new System.Windows.Forms.TabPage();
            this.RoomDescriptionLabel = new System.Windows.Forms.Label();
            this.RoomTitleLabel = new System.Windows.Forms.Label();
            this.RoomPasswordTextBox = new System.Windows.Forms.TextBox();
            this.RoomPasswordLabel = new System.Windows.Forms.Label();
            this.RoomMasterNameTextBox = new System.Windows.Forms.TextBox();
            this.roomMasterLabel = new System.Windows.Forms.Label();
            this.maxParticipantsLabel = new System.Windows.Forms.Label();
            this.RoomMaxPlayerNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.RoomDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.RoomTitleTextBox = new System.Windows.Forms.TextBox();
            this.RoomCreateEditButton = new System.Windows.Forms.Button();
            this.RoomCloseExitButton = new System.Windows.Forms.Button();
            this.PacketMonitorTabPage = new System.Windows.Forms.TabPage();
            this.PacketMonitorSplitContainer = new System.Windows.Forms.SplitContainer();
            this.PacketMonitorMyPspClearButton = new System.Windows.Forms.Button();
            this.PacketMonitorMyPspLabel = new System.Windows.Forms.Label();
            this.PacketMonitorMyPspListView = new System.Windows.Forms.ListView();
            this.MyPspMacColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyPspInSpeedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyPspOutSpeedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyPspInBytesColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyPspOutBytesColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PacketMonitorRemotePspClearButton = new System.Windows.Forms.Button();
            this.PacketMonitorRemotePspLabel = new System.Windows.Forms.Label();
            this.PacketMonitorRemotePspListView = new System.Windows.Forms.ListView();
            this.RemotePspMacColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RemotePspInSpeedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RemotePspOutSpeedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RemotePspInBytesColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RemotePspOutBytesColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.playersTreeView = new System.Windows.Forms.TreeView();
            this.PlayerListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.KickPlayer = new System.Windows.Forms.ToolStripMenuItem();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.chatTagPage = new System.Windows.Forms.TabPage();
            this.chatSplitContainer = new System.Windows.Forms.SplitContainer();
            this.ChatLogTextBox = new System.Windows.Forms.TextBox();
            this.ConfigTagPage = new System.Windows.Forms.TabPage();
            this.ChatLogNotDisplayRoomActionCheckBox = new System.Windows.Forms.CheckBox();
            this.LoginUserNameAlertLabel = new System.Windows.Forms.Label();
            this.LoginUserNameLabel = new System.Windows.Forms.Label();
            this.LoginUserNameTextBox = new System.Windows.Forms.TextBox();
            this.logViewTagPage = new System.Windows.Forms.TabPage();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.serverAddressStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.serverStatusStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainLocationSplitContainer = new System.Windows.Forms.SplitContainer();
            this.LocationTabControl.SuspendLayout();
            this.PortalTabPage.SuspendLayout();
            this.ArenaTabPage.SuspendLayout();
            this.PlayRoomTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RoomMaxPlayerNumericUpDown)).BeginInit();
            this.PacketMonitorTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PacketMonitorSplitContainer)).BeginInit();
            this.PacketMonitorSplitContainer.Panel1.SuspendLayout();
            this.PacketMonitorSplitContainer.Panel2.SuspendLayout();
            this.PacketMonitorSplitContainer.SuspendLayout();
            this.PlayerListContextMenu.SuspendLayout();
            this.MainTabControl.SuspendLayout();
            this.chatTagPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chatSplitContainer)).BeginInit();
            this.chatSplitContainer.Panel1.SuspendLayout();
            this.chatSplitContainer.Panel2.SuspendLayout();
            this.chatSplitContainer.SuspendLayout();
            this.ConfigTagPage.SuspendLayout();
            this.logViewTagPage.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainLocationSplitContainer)).BeginInit();
            this.MainLocationSplitContainer.Panel1.SuspendLayout();
            this.MainLocationSplitContainer.Panel2.SuspendLayout();
            this.MainLocationSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // CommandTextBox
            // 
            this.CommandTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CommandTextBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.CommandTextBox.Location = new System.Drawing.Point(1, 370);
            this.CommandTextBox.MaxLength = 1000;
            this.CommandTextBox.Name = "CommandTextBox";
            this.CommandTextBox.Size = new System.Drawing.Size(460, 22);
            this.CommandTextBox.TabIndex = 15;
            this.CommandTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CommandTextBox_KeyPress);
            // 
            // CommandSubmitButton
            // 
            this.CommandSubmitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CommandSubmitButton.Location = new System.Drawing.Point(463, 370);
            this.CommandSubmitButton.Name = "CommandSubmitButton";
            this.CommandSubmitButton.Size = new System.Drawing.Size(44, 23);
            this.CommandSubmitButton.TabIndex = 16;
            this.CommandSubmitButton.Text = "発言";
            this.CommandSubmitButton.UseVisualStyleBackColor = true;
            this.CommandSubmitButton.Click += new System.EventHandler(this.CommandSubmitButton_Click);
            // 
            // ServerLoginButton
            // 
            this.ServerLoginButton.Location = new System.Drawing.Point(241, 4);
            this.ServerLoginButton.Name = "ServerLoginButton";
            this.ServerLoginButton.Size = new System.Drawing.Size(72, 21);
            this.ServerLoginButton.TabIndex = 2;
            this.ServerLoginButton.Text = "ログイン";
            this.ServerLoginButton.UseVisualStyleBackColor = true;
            this.ServerLoginButton.Click += new System.EventHandler(this.ServerLoginButton_Click);
            // 
            // ServerAddressComboBox
            // 
            this.ServerAddressComboBox.DropDownHeight = 500;
            this.ServerAddressComboBox.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ServerAddressComboBox.FormattingEnabled = true;
            this.ServerAddressComboBox.IntegralHeight = false;
            this.ServerAddressComboBox.Location = new System.Drawing.Point(47, 4);
            this.ServerAddressComboBox.Name = "ServerAddressComboBox";
            this.ServerAddressComboBox.Size = new System.Drawing.Size(188, 21);
            this.ServerAddressComboBox.TabIndex = 1;
            this.ServerAddressComboBox.SelectedIndexChanged += new System.EventHandler(this.ServerAddressComboBox_SelectedIndexChanged);
            this.ServerAddressComboBox.TextChanged += new System.EventHandler(this.ServerAddressComboBox_TextChanged);
            this.ServerAddressComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ServerAddressComboBox_KeyPress);
            // 
            // labelServerAddress
            // 
            this.labelServerAddress.AutoSize = true;
            this.labelServerAddress.Location = new System.Drawing.Point(3, 9);
            this.labelServerAddress.Name = "labelServerAddress";
            this.labelServerAddress.Size = new System.Drawing.Size(41, 12);
            this.labelServerAddress.TabIndex = 9;
            this.labelServerAddress.Text = "アドレス";
            // 
            // lanAdapterComboBox
            // 
            this.lanAdapterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lanAdapterComboBox.FormattingEnabled = true;
            this.lanAdapterComboBox.Location = new System.Drawing.Point(391, 5);
            this.lanAdapterComboBox.Name = "lanAdapterComboBox";
            this.lanAdapterComboBox.Size = new System.Drawing.Size(300, 20);
            this.lanAdapterComboBox.TabIndex = 3;
            this.lanAdapterComboBox.SelectedIndexChanged += new System.EventHandler(this.LanAdapterComboBox_SelectedIndexChanged);
            // 
            // toggleCaptureLanAdapterButton
            // 
            this.toggleCaptureLanAdapterButton.Enabled = false;
            this.toggleCaptureLanAdapterButton.Location = new System.Drawing.Point(696, 4);
            this.toggleCaptureLanAdapterButton.Margin = new System.Windows.Forms.Padding(0);
            this.toggleCaptureLanAdapterButton.Name = "toggleCaptureLanAdapterButton";
            this.toggleCaptureLanAdapterButton.Size = new System.Drawing.Size(93, 21);
            this.toggleCaptureLanAdapterButton.TabIndex = 4;
            this.toggleCaptureLanAdapterButton.Text = "PSPと通信開始";
            this.toggleCaptureLanAdapterButton.UseVisualStyleBackColor = true;
            this.toggleCaptureLanAdapterButton.Click += new System.EventHandler(this.ToggleCaptureLanAdapterButton_Click);
            // 
            // labelLanAdapter
            // 
            this.labelLanAdapter.AutoSize = true;
            this.labelLanAdapter.Location = new System.Drawing.Point(323, 9);
            this.labelLanAdapter.Name = "labelLanAdapter";
            this.labelLanAdapter.Size = new System.Drawing.Size(64, 12);
            this.labelLanAdapter.TabIndex = 13;
            this.labelLanAdapter.Text = "無線アダプタ";
            // 
            // playerListLabel
            // 
            this.playerListLabel.AutoSize = true;
            this.playerListLabel.Location = new System.Drawing.Point(3, 3);
            this.playerListLabel.Name = "playerListLabel";
            this.playerListLabel.Size = new System.Drawing.Size(115, 12);
            this.playerListLabel.TabIndex = 16;
            this.playerListLabel.Text = "参加者一覧  (Ping値)";
            // 
            // LocationTabControl
            // 
            this.LocationTabControl.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.LocationTabControl.Controls.Add(this.PortalTabPage);
            this.LocationTabControl.Controls.Add(this.ArenaTabPage);
            this.LocationTabControl.Controls.Add(this.PlayRoomTabPage);
            this.LocationTabControl.Controls.Add(this.PacketMonitorTabPage);
            this.LocationTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LocationTabControl.Location = new System.Drawing.Point(0, 0);
            this.LocationTabControl.Multiline = true;
            this.LocationTabControl.Name = "LocationTabControl";
            this.LocationTabControl.SelectedIndex = 0;
            this.LocationTabControl.Size = new System.Drawing.Size(272, 422);
            this.LocationTabControl.TabIndex = 5;
            this.LocationTabControl.TabStop = false;
            this.LocationTabControl.SelectedIndexChanged += new System.EventHandler(this.locationTabControl_SelectedIndexChanged);
            // 
            // PortalTabPage
            // 
            this.PortalTabPage.Controls.Add(this.ArenaServerListView);
            this.PortalTabPage.Location = new System.Drawing.Point(4, 4);
            this.PortalTabPage.Name = "PortalTabPage";
            this.PortalTabPage.Size = new System.Drawing.Size(264, 397);
            this.PortalTabPage.TabIndex = 1;
            this.PortalTabPage.Text = "ポータル";
            this.PortalTabPage.UseVisualStyleBackColor = true;
            // 
            // ArenaServerListView
            // 
            this.ArenaServerListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader4,
            this.columnHeader2,
            this.columnHeader3});
            this.ArenaServerListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ArenaServerListView.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ArenaServerListView.FullRowSelect = true;
            this.ArenaServerListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
            this.ArenaServerListView.Location = new System.Drawing.Point(0, 0);
            this.ArenaServerListView.Name = "ArenaServerListView";
            this.ArenaServerListView.Size = new System.Drawing.Size(264, 397);
            this.ArenaServerListView.TabIndex = 5;
            this.ArenaServerListView.TileSize = new System.Drawing.Size(225, 60);
            this.ArenaServerListView.UseCompatibleStateImageBehavior = false;
            this.ArenaServerListView.View = System.Windows.Forms.View.Tile;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "アドレス";
            this.columnHeader1.Width = 118;
            // 
            // columnHeader4
            // 
            this.columnHeader4.DisplayIndex = 3;
            this.columnHeader4.Text = "備考";
            // 
            // columnHeader2
            // 
            this.columnHeader2.DisplayIndex = 1;
            this.columnHeader2.Text = "参加人数";
            this.columnHeader2.Width = 63;
            // 
            // columnHeader3
            // 
            this.columnHeader3.DisplayIndex = 2;
            this.columnHeader3.Text = "部屋数";
            // 
            // ArenaTabPage
            // 
            this.ArenaTabPage.Controls.Add(this.RoomListView);
            this.ArenaTabPage.Location = new System.Drawing.Point(4, 4);
            this.ArenaTabPage.Name = "ArenaTabPage";
            this.ArenaTabPage.Size = new System.Drawing.Size(264, 397);
            this.ArenaTabPage.TabIndex = 0;
            this.ArenaTabPage.Text = "アリーナロビー";
            this.ArenaTabPage.UseVisualStyleBackColor = true;
            // 
            // RoomListView
            // 
            this.RoomListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.RoomListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RoomListView.FullRowSelect = true;
            this.RoomListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem4,
            listViewItem5,
            listViewItem6});
            this.RoomListView.Location = new System.Drawing.Point(0, 0);
            this.RoomListView.MultiSelect = false;
            this.RoomListView.Name = "RoomListView";
            this.RoomListView.Size = new System.Drawing.Size(264, 397);
            this.RoomListView.TabIndex = 6;
            this.RoomListView.UseCompatibleStateImageBehavior = false;
            this.RoomListView.View = System.Windows.Forms.View.Details;
            this.RoomListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.RoomListView_MouseDoubleClick);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "部屋主";
            this.columnHeader5.Width = 76;
            // 
            // columnHeader6
            // 
            this.columnHeader6.DisplayIndex = 2;
            this.columnHeader6.Text = "部屋名";
            this.columnHeader6.Width = 99;
            // 
            // columnHeader7
            // 
            this.columnHeader7.DisplayIndex = 3;
            this.columnHeader7.Text = "定員";
            this.columnHeader7.Width = 47;
            // 
            // columnHeader8
            // 
            this.columnHeader8.DisplayIndex = 1;
            this.columnHeader8.Text = "鍵";
            this.columnHeader8.Width = 25;
            // 
            // PlayRoomTabPage
            // 
            this.PlayRoomTabPage.Controls.Add(this.RoomDescriptionLabel);
            this.PlayRoomTabPage.Controls.Add(this.RoomTitleLabel);
            this.PlayRoomTabPage.Controls.Add(this.RoomPasswordTextBox);
            this.PlayRoomTabPage.Controls.Add(this.RoomPasswordLabel);
            this.PlayRoomTabPage.Controls.Add(this.RoomMasterNameTextBox);
            this.PlayRoomTabPage.Controls.Add(this.roomMasterLabel);
            this.PlayRoomTabPage.Controls.Add(this.maxParticipantsLabel);
            this.PlayRoomTabPage.Controls.Add(this.RoomMaxPlayerNumericUpDown);
            this.PlayRoomTabPage.Controls.Add(this.RoomDescriptionTextBox);
            this.PlayRoomTabPage.Controls.Add(this.RoomTitleTextBox);
            this.PlayRoomTabPage.Controls.Add(this.RoomCreateEditButton);
            this.PlayRoomTabPage.Controls.Add(this.RoomCloseExitButton);
            this.PlayRoomTabPage.Location = new System.Drawing.Point(4, 4);
            this.PlayRoomTabPage.Name = "PlayRoomTabPage";
            this.PlayRoomTabPage.Size = new System.Drawing.Size(264, 397);
            this.PlayRoomTabPage.TabIndex = 2;
            this.PlayRoomTabPage.Text = "プレイルーム";
            this.PlayRoomTabPage.UseVisualStyleBackColor = true;
            // 
            // RoomDescriptionLabel
            // 
            this.RoomDescriptionLabel.AutoSize = true;
            this.RoomDescriptionLabel.Location = new System.Drawing.Point(2, 138);
            this.RoomDescriptionLabel.Name = "RoomDescriptionLabel";
            this.RoomDescriptionLabel.Size = new System.Drawing.Size(93, 12);
            this.RoomDescriptionLabel.TabIndex = 34;
            this.RoomDescriptionLabel.Text = "部屋の紹介・備考";
            // 
            // RoomTitleLabel
            // 
            this.RoomTitleLabel.AutoSize = true;
            this.RoomTitleLabel.Location = new System.Drawing.Point(10, 59);
            this.RoomTitleLabel.Name = "RoomTitleLabel";
            this.RoomTitleLabel.Size = new System.Drawing.Size(41, 12);
            this.RoomTitleLabel.TabIndex = 33;
            this.RoomTitleLabel.Text = "部屋名";
            // 
            // RoomPasswordTextBox
            // 
            this.RoomPasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RoomPasswordTextBox.BackColor = System.Drawing.Color.White;
            this.RoomPasswordTextBox.Enabled = false;
            this.RoomPasswordTextBox.ForeColor = System.Drawing.Color.Black;
            this.RoomPasswordTextBox.Location = new System.Drawing.Point(60, 80);
            this.RoomPasswordTextBox.MaxLength = 100;
            this.RoomPasswordTextBox.Name = "RoomPasswordTextBox";
            this.RoomPasswordTextBox.Size = new System.Drawing.Size(201, 19);
            this.RoomPasswordTextBox.TabIndex = 10;
            this.RoomPasswordTextBox.TextChanged += new System.EventHandler(this.RoomPasswordTextBox_TextChanged);
            this.RoomPasswordTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RoomPasswordTextBox_KeyPress);
            // 
            // RoomPasswordLabel
            // 
            this.RoomPasswordLabel.AutoSize = true;
            this.RoomPasswordLabel.Location = new System.Drawing.Point(4, 84);
            this.RoomPasswordLabel.Name = "RoomPasswordLabel";
            this.RoomPasswordLabel.Size = new System.Drawing.Size(52, 12);
            this.RoomPasswordLabel.TabIndex = 31;
            this.RoomPasswordLabel.Text = "パスワード";
            // 
            // RoomMasterNameTextBox
            // 
            this.RoomMasterNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RoomMasterNameTextBox.BackColor = System.Drawing.Color.White;
            this.RoomMasterNameTextBox.Enabled = false;
            this.RoomMasterNameTextBox.ForeColor = System.Drawing.Color.Black;
            this.RoomMasterNameTextBox.Location = new System.Drawing.Point(60, 31);
            this.RoomMasterNameTextBox.Name = "RoomMasterNameTextBox";
            this.RoomMasterNameTextBox.Size = new System.Drawing.Size(201, 19);
            this.RoomMasterNameTextBox.TabIndex = 30;
            this.RoomMasterNameTextBox.TabStop = false;
            // 
            // roomMasterLabel
            // 
            this.roomMasterLabel.AutoSize = true;
            this.roomMasterLabel.Location = new System.Drawing.Point(10, 34);
            this.roomMasterLabel.Name = "roomMasterLabel";
            this.roomMasterLabel.Size = new System.Drawing.Size(41, 12);
            this.roomMasterLabel.TabIndex = 29;
            this.roomMasterLabel.Text = "部屋主";
            // 
            // maxParticipantsLabel
            // 
            this.maxParticipantsLabel.AutoSize = true;
            this.maxParticipantsLabel.Location = new System.Drawing.Point(4, 109);
            this.maxParticipantsLabel.Name = "maxParticipantsLabel";
            this.maxParticipantsLabel.Size = new System.Drawing.Size(53, 12);
            this.maxParticipantsLabel.TabIndex = 26;
            this.maxParticipantsLabel.Text = "制限人数";
            // 
            // RoomMaxPlayerNumericUpDown
            // 
            this.RoomMaxPlayerNumericUpDown.BackColor = System.Drawing.Color.White;
            this.RoomMaxPlayerNumericUpDown.Enabled = false;
            this.RoomMaxPlayerNumericUpDown.ForeColor = System.Drawing.Color.Black;
            this.RoomMaxPlayerNumericUpDown.Location = new System.Drawing.Point(60, 105);
            this.RoomMaxPlayerNumericUpDown.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.RoomMaxPlayerNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.RoomMaxPlayerNumericUpDown.Name = "RoomMaxPlayerNumericUpDown";
            this.RoomMaxPlayerNumericUpDown.ReadOnly = true;
            this.RoomMaxPlayerNumericUpDown.Size = new System.Drawing.Size(43, 19);
            this.RoomMaxPlayerNumericUpDown.TabIndex = 11;
            this.RoomMaxPlayerNumericUpDown.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // RoomDescriptionTextBox
            // 
            this.RoomDescriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RoomDescriptionTextBox.BackColor = System.Drawing.Color.White;
            this.RoomDescriptionTextBox.Enabled = false;
            this.RoomDescriptionTextBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.RoomDescriptionTextBox.ForeColor = System.Drawing.Color.Black;
            this.RoomDescriptionTextBox.Location = new System.Drawing.Point(1, 152);
            this.RoomDescriptionTextBox.MaxLength = 1500;
            this.RoomDescriptionTextBox.Multiline = true;
            this.RoomDescriptionTextBox.Name = "RoomDescriptionTextBox";
            this.RoomDescriptionTextBox.Size = new System.Drawing.Size(260, 242);
            this.RoomDescriptionTextBox.TabIndex = 12;
            this.RoomDescriptionTextBox.TextChanged += new System.EventHandler(this.RoomDescriptionTextBox_TextChanged);
            this.RoomDescriptionTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RoomDescriptionTextBox_KeyPress);
            // 
            // RoomTitleTextBox
            // 
            this.RoomTitleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RoomTitleTextBox.BackColor = System.Drawing.Color.White;
            this.RoomTitleTextBox.Enabled = false;
            this.RoomTitleTextBox.ForeColor = System.Drawing.Color.Black;
            this.RoomTitleTextBox.Location = new System.Drawing.Point(60, 55);
            this.RoomTitleTextBox.MaxLength = 100;
            this.RoomTitleTextBox.Name = "RoomTitleTextBox";
            this.RoomTitleTextBox.Size = new System.Drawing.Size(201, 19);
            this.RoomTitleTextBox.TabIndex = 9;
            this.RoomTitleTextBox.TextChanged += new System.EventHandler(this.RoomTitleTextBox_TextChanged);
            this.RoomTitleTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RoomTitleTextBox_KeyPress);
            // 
            // RoomCreateEditButton
            // 
            this.RoomCreateEditButton.Enabled = false;
            this.RoomCreateEditButton.Location = new System.Drawing.Point(134, 3);
            this.RoomCreateEditButton.Name = "RoomCreateEditButton";
            this.RoomCreateEditButton.Size = new System.Drawing.Size(92, 22);
            this.RoomCreateEditButton.TabIndex = 8;
            this.RoomCreateEditButton.Text = "部屋を作成";
            this.RoomCreateEditButton.UseVisualStyleBackColor = true;
            this.RoomCreateEditButton.Click += new System.EventHandler(this.RoomCreateEditButton_Click);
            // 
            // RoomCloseExitButton
            // 
            this.RoomCloseExitButton.Enabled = false;
            this.RoomCloseExitButton.Location = new System.Drawing.Point(3, 3);
            this.RoomCloseExitButton.Name = "RoomCloseExitButton";
            this.RoomCloseExitButton.Size = new System.Drawing.Size(92, 22);
            this.RoomCloseExitButton.TabIndex = 7;
            this.RoomCloseExitButton.Text = "部屋を閉じる";
            this.RoomCloseExitButton.UseVisualStyleBackColor = true;
            this.RoomCloseExitButton.Click += new System.EventHandler(this.RoomCloseExitButton_Click);
            // 
            // PacketMonitorTabPage
            // 
            this.PacketMonitorTabPage.Controls.Add(this.PacketMonitorSplitContainer);
            this.PacketMonitorTabPage.Location = new System.Drawing.Point(4, 4);
            this.PacketMonitorTabPage.Name = "PacketMonitorTabPage";
            this.PacketMonitorTabPage.Size = new System.Drawing.Size(264, 397);
            this.PacketMonitorTabPage.TabIndex = 3;
            this.PacketMonitorTabPage.Text = "通信モニタ";
            this.PacketMonitorTabPage.UseVisualStyleBackColor = true;
            // 
            // PacketMonitorSplitContainer
            // 
            this.PacketMonitorSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PacketMonitorSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.PacketMonitorSplitContainer.Name = "PacketMonitorSplitContainer";
            this.PacketMonitorSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // PacketMonitorSplitContainer.Panel1
            // 
            this.PacketMonitorSplitContainer.Panel1.Controls.Add(this.PacketMonitorMyPspClearButton);
            this.PacketMonitorSplitContainer.Panel1.Controls.Add(this.PacketMonitorMyPspLabel);
            this.PacketMonitorSplitContainer.Panel1.Controls.Add(this.PacketMonitorMyPspListView);
            this.PacketMonitorSplitContainer.Panel1MinSize = 50;
            // 
            // PacketMonitorSplitContainer.Panel2
            // 
            this.PacketMonitorSplitContainer.Panel2.Controls.Add(this.PacketMonitorRemotePspClearButton);
            this.PacketMonitorSplitContainer.Panel2.Controls.Add(this.PacketMonitorRemotePspLabel);
            this.PacketMonitorSplitContainer.Panel2.Controls.Add(this.PacketMonitorRemotePspListView);
            this.PacketMonitorSplitContainer.Panel2MinSize = 50;
            this.PacketMonitorSplitContainer.Size = new System.Drawing.Size(264, 397);
            this.PacketMonitorSplitContainer.SplitterDistance = 143;
            this.PacketMonitorSplitContainer.TabIndex = 0;
            // 
            // PacketMonitorMyPspClearButton
            // 
            this.PacketMonitorMyPspClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PacketMonitorMyPspClearButton.Location = new System.Drawing.Point(207, 1);
            this.PacketMonitorMyPspClearButton.Name = "PacketMonitorMyPspClearButton";
            this.PacketMonitorMyPspClearButton.Size = new System.Drawing.Size(53, 19);
            this.PacketMonitorMyPspClearButton.TabIndex = 2;
            this.PacketMonitorMyPspClearButton.Text = "クリア";
            this.PacketMonitorMyPspClearButton.UseVisualStyleBackColor = true;
            this.PacketMonitorMyPspClearButton.Click += new System.EventHandler(this.PacketMonitorMyPspClearButton_Click);
            // 
            // PacketMonitorMyPspLabel
            // 
            this.PacketMonitorMyPspLabel.AutoSize = true;
            this.PacketMonitorMyPspLabel.Location = new System.Drawing.Point(7, 6);
            this.PacketMonitorMyPspLabel.Name = "PacketMonitorMyPspLabel";
            this.PacketMonitorMyPspLabel.Size = new System.Drawing.Size(60, 12);
            this.PacketMonitorMyPspLabel.TabIndex = 0;
            this.PacketMonitorMyPspLabel.Text = "自分のPSP";
            // 
            // PacketMonitorMyPspListView
            // 
            this.PacketMonitorMyPspListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PacketMonitorMyPspListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.MyPspMacColumnHeader,
            this.MyPspInSpeedColumnHeader,
            this.MyPspOutSpeedColumnHeader,
            this.MyPspInBytesColumnHeader,
            this.MyPspOutBytesColumnHeader});
            this.PacketMonitorMyPspListView.FullRowSelect = true;
            this.PacketMonitorMyPspListView.Location = new System.Drawing.Point(1, 23);
            this.PacketMonitorMyPspListView.Name = "PacketMonitorMyPspListView";
            this.PacketMonitorMyPspListView.Size = new System.Drawing.Size(260, 117);
            this.PacketMonitorMyPspListView.TabIndex = 1;
            this.PacketMonitorMyPspListView.UseCompatibleStateImageBehavior = false;
            this.PacketMonitorMyPspListView.View = System.Windows.Forms.View.Details;
            // 
            // MyPspMacColumnHeader
            // 
            this.MyPspMacColumnHeader.Text = "MACアドレス";
            this.MyPspMacColumnHeader.Width = 100;
            // 
            // MyPspInSpeedColumnHeader
            // 
            this.MyPspInSpeedColumnHeader.Text = "In (Kbps)";
            this.MyPspInSpeedColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.MyPspInSpeedColumnHeader.Width = 65;
            // 
            // MyPspOutSpeedColumnHeader
            // 
            this.MyPspOutSpeedColumnHeader.Text = "Out (Kbps)";
            this.MyPspOutSpeedColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.MyPspOutSpeedColumnHeader.Width = 68;
            // 
            // MyPspInBytesColumnHeader
            // 
            this.MyPspInBytesColumnHeader.Text = "In 累積バイト";
            this.MyPspInBytesColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.MyPspInBytesColumnHeader.Width = 90;
            // 
            // MyPspOutBytesColumnHeader
            // 
            this.MyPspOutBytesColumnHeader.Text = "Out 累積バイト";
            this.MyPspOutBytesColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.MyPspOutBytesColumnHeader.Width = 90;
            // 
            // PacketMonitorRemotePspClearButton
            // 
            this.PacketMonitorRemotePspClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PacketMonitorRemotePspClearButton.Location = new System.Drawing.Point(207, 0);
            this.PacketMonitorRemotePspClearButton.Name = "PacketMonitorRemotePspClearButton";
            this.PacketMonitorRemotePspClearButton.Size = new System.Drawing.Size(53, 19);
            this.PacketMonitorRemotePspClearButton.TabIndex = 2;
            this.PacketMonitorRemotePspClearButton.Text = "クリア";
            this.PacketMonitorRemotePspClearButton.UseVisualStyleBackColor = true;
            this.PacketMonitorRemotePspClearButton.Click += new System.EventHandler(this.PacketMonitorRemotePspClearButton_Click);
            // 
            // PacketMonitorRemotePspLabel
            // 
            this.PacketMonitorRemotePspLabel.AutoSize = true;
            this.PacketMonitorRemotePspLabel.Location = new System.Drawing.Point(5, 5);
            this.PacketMonitorRemotePspLabel.Name = "PacketMonitorRemotePspLabel";
            this.PacketMonitorRemotePspLabel.Size = new System.Drawing.Size(60, 12);
            this.PacketMonitorRemotePspLabel.TabIndex = 0;
            this.PacketMonitorRemotePspLabel.Text = "相手のPSP";
            // 
            // PacketMonitorRemotePspListView
            // 
            this.PacketMonitorRemotePspListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PacketMonitorRemotePspListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.RemotePspMacColumnHeader,
            this.RemotePspInSpeedColumnHeader,
            this.RemotePspOutSpeedColumnHeader,
            this.RemotePspInBytesColumnHeader,
            this.RemotePspOutBytesColumnHeader});
            this.PacketMonitorRemotePspListView.FullRowSelect = true;
            this.PacketMonitorRemotePspListView.Location = new System.Drawing.Point(1, 22);
            this.PacketMonitorRemotePspListView.Name = "PacketMonitorRemotePspListView";
            this.PacketMonitorRemotePspListView.Size = new System.Drawing.Size(260, 225);
            this.PacketMonitorRemotePspListView.TabIndex = 1;
            this.PacketMonitorRemotePspListView.UseCompatibleStateImageBehavior = false;
            this.PacketMonitorRemotePspListView.View = System.Windows.Forms.View.Details;
            // 
            // RemotePspMacColumnHeader
            // 
            this.RemotePspMacColumnHeader.Text = "MACアドレス";
            this.RemotePspMacColumnHeader.Width = 100;
            // 
            // RemotePspInSpeedColumnHeader
            // 
            this.RemotePspInSpeedColumnHeader.Text = "In (Kbps)";
            this.RemotePspInSpeedColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.RemotePspInSpeedColumnHeader.Width = 65;
            // 
            // RemotePspOutSpeedColumnHeader
            // 
            this.RemotePspOutSpeedColumnHeader.Text = "Out (Kbps)";
            this.RemotePspOutSpeedColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.RemotePspOutSpeedColumnHeader.Width = 68;
            // 
            // RemotePspInBytesColumnHeader
            // 
            this.RemotePspInBytesColumnHeader.Text = "In 累積バイト";
            this.RemotePspInBytesColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.RemotePspInBytesColumnHeader.Width = 90;
            // 
            // RemotePspOutBytesColumnHeader
            // 
            this.RemotePspOutBytesColumnHeader.Text = "Out 累積バイト";
            this.RemotePspOutBytesColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.RemotePspOutBytesColumnHeader.Width = 90;
            // 
            // playersTreeView
            // 
            this.playersTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.playersTreeView.Location = new System.Drawing.Point(2, 18);
            this.playersTreeView.Name = "playersTreeView";
            this.playersTreeView.Size = new System.Drawing.Size(148, 348);
            this.playersTreeView.TabIndex = 14;
            this.playersTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.playersTreeView_MouseUp);
            // 
            // PlayerListContextMenu
            // 
            this.PlayerListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.KickPlayer});
            this.PlayerListContextMenu.Name = "PlayerListContextMenu";
            this.PlayerListContextMenu.Size = new System.Drawing.Size(100, 26);
            this.PlayerListContextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.PlayerListContextMenu_ItemClicked);
            // 
            // KickPlayer
            // 
            this.KickPlayer.Name = "KickPlayer";
            this.KickPlayer.Size = new System.Drawing.Size(99, 22);
            this.KickPlayer.Text = "キック";
            // 
            // MainTabControl
            // 
            this.MainTabControl.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.MainTabControl.Controls.Add(this.chatTagPage);
            this.MainTabControl.Controls.Add(this.ConfigTagPage);
            this.MainTabControl.Controls.Add(this.logViewTagPage);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(0, 0);
            this.MainTabControl.Multiline = true;
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(516, 422);
            this.MainTabControl.TabIndex = 6;
            // 
            // chatTagPage
            // 
            this.chatTagPage.Controls.Add(this.chatSplitContainer);
            this.chatTagPage.Controls.Add(this.CommandTextBox);
            this.chatTagPage.Controls.Add(this.CommandSubmitButton);
            this.chatTagPage.Location = new System.Drawing.Point(4, 4);
            this.chatTagPage.Name = "chatTagPage";
            this.chatTagPage.Padding = new System.Windows.Forms.Padding(3);
            this.chatTagPage.Size = new System.Drawing.Size(508, 397);
            this.chatTagPage.TabIndex = 0;
            this.chatTagPage.Text = "チャット";
            this.chatTagPage.UseVisualStyleBackColor = true;
            // 
            // chatSplitContainer
            // 
            this.chatSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chatSplitContainer.Location = new System.Drawing.Point(1, 0);
            this.chatSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.chatSplitContainer.Name = "chatSplitContainer";
            // 
            // chatSplitContainer.Panel1
            // 
            this.chatSplitContainer.Panel1.Controls.Add(this.ChatLogTextBox);
            this.chatSplitContainer.Panel1MinSize = 200;
            // 
            // chatSplitContainer.Panel2
            // 
            this.chatSplitContainer.Panel2.Controls.Add(this.playerListLabel);
            this.chatSplitContainer.Panel2.Controls.Add(this.playersTreeView);
            this.chatSplitContainer.Panel2MinSize = 150;
            this.chatSplitContainer.Size = new System.Drawing.Size(507, 366);
            this.chatSplitContainer.SplitterDistance = 353;
            this.chatSplitContainer.TabIndex = 19;
            // 
            // ChatLogTextBox
            // 
            this.ChatLogTextBox.BackColor = System.Drawing.Color.White;
            this.ChatLogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChatLogTextBox.Font = new System.Drawing.Font("MS UI Gothic", 10.5F);
            this.ChatLogTextBox.Location = new System.Drawing.Point(0, 0);
            this.ChatLogTextBox.Multiline = true;
            this.ChatLogTextBox.Name = "ChatLogTextBox";
            this.ChatLogTextBox.ReadOnly = true;
            this.ChatLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ChatLogTextBox.Size = new System.Drawing.Size(353, 366);
            this.ChatLogTextBox.TabIndex = 0;
            // 
            // ConfigTagPage
            // 
            this.ConfigTagPage.Controls.Add(this.ChatLogNotDisplayRoomActionCheckBox);
            this.ConfigTagPage.Controls.Add(this.LoginUserNameAlertLabel);
            this.ConfigTagPage.Controls.Add(this.LoginUserNameLabel);
            this.ConfigTagPage.Controls.Add(this.LoginUserNameTextBox);
            this.ConfigTagPage.Location = new System.Drawing.Point(4, 4);
            this.ConfigTagPage.Name = "ConfigTagPage";
            this.ConfigTagPage.Padding = new System.Windows.Forms.Padding(3);
            this.ConfigTagPage.Size = new System.Drawing.Size(508, 397);
            this.ConfigTagPage.TabIndex = 1;
            this.ConfigTagPage.Text = "設定";
            this.ConfigTagPage.UseVisualStyleBackColor = true;
            // 
            // ChatLogNotDisplayRoomActionCheckBox
            // 
            this.ChatLogNotDisplayRoomActionCheckBox.AutoSize = true;
            this.ChatLogNotDisplayRoomActionCheckBox.Location = new System.Drawing.Point(2, 47);
            this.ChatLogNotDisplayRoomActionCheckBox.Name = "ChatLogNotDisplayRoomActionCheckBox";
            this.ChatLogNotDisplayRoomActionCheckBox.Size = new System.Drawing.Size(226, 16);
            this.ChatLogNotDisplayRoomActionCheckBox.TabIndex = 20;
            this.ChatLogNotDisplayRoomActionCheckBox.Text = "チャットログに入退室メッセージを表示しない";
            this.ChatLogNotDisplayRoomActionCheckBox.UseVisualStyleBackColor = true;
            // 
            // LoginUserNameAlertLabel
            // 
            this.LoginUserNameAlertLabel.AutoSize = true;
            this.LoginUserNameAlertLabel.ForeColor = System.Drawing.Color.Red;
            this.LoginUserNameAlertLabel.Location = new System.Drawing.Point(63, 29);
            this.LoginUserNameAlertLabel.Name = "LoginUserNameAlertLabel";
            this.LoginUserNameAlertLabel.Size = new System.Drawing.Size(141, 12);
            this.LoginUserNameAlertLabel.TabIndex = 18;
            this.LoginUserNameAlertLabel.Text = "ユーザー名を入力してください";
            this.LoginUserNameAlertLabel.Visible = false;
            // 
            // LoginUserNameLabel
            // 
            this.LoginUserNameLabel.AutoSize = true;
            this.LoginUserNameLabel.Location = new System.Drawing.Point(2, 13);
            this.LoginUserNameLabel.Name = "LoginUserNameLabel";
            this.LoginUserNameLabel.Size = new System.Drawing.Size(57, 12);
            this.LoginUserNameLabel.TabIndex = 1;
            this.LoginUserNameLabel.Text = "ユーザー名";
            // 
            // LoginUserNameTextBox
            // 
            this.LoginUserNameTextBox.Location = new System.Drawing.Point(63, 10);
            this.LoginUserNameTextBox.MaxLength = 100;
            this.LoginUserNameTextBox.Name = "LoginUserNameTextBox";
            this.LoginUserNameTextBox.Size = new System.Drawing.Size(140, 19);
            this.LoginUserNameTextBox.TabIndex = 17;
            this.LoginUserNameTextBox.Text = "NoName";
            this.LoginUserNameTextBox.TextChanged += new System.EventHandler(this.LoginUserNameTextBox_TextChanged);
            this.LoginUserNameTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LoginUserNameTextBox_KeyPress);
            // 
            // logViewTagPage
            // 
            this.logViewTagPage.Controls.Add(this.logTextBox);
            this.logViewTagPage.Location = new System.Drawing.Point(4, 4);
            this.logViewTagPage.Name = "logViewTagPage";
            this.logViewTagPage.Size = new System.Drawing.Size(508, 397);
            this.logViewTagPage.TabIndex = 2;
            this.logViewTagPage.Text = "ログ";
            this.logViewTagPage.UseVisualStyleBackColor = true;
            // 
            // logTextBox
            // 
            this.logTextBox.BackColor = System.Drawing.Color.White;
            this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTextBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logTextBox.Location = new System.Drawing.Point(0, 0);
            this.logTextBox.Margin = new System.Windows.Forms.Padding(30, 3, 3, 3);
            this.logTextBox.MaxLength = 2147483647;
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logTextBox.Size = new System.Drawing.Size(508, 397);
            this.logTextBox.TabIndex = 0;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverAddressStripStatusLabel,
            this.serverStatusStripStatusLabel,
            this.toolStripStatusLabel1});
            this.statusStrip.Location = new System.Drawing.Point(0, 451);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(792, 22);
            this.statusStrip.TabIndex = 20;
            this.statusStrip.Text = "statusStrip1";
            // 
            // serverAddressStripStatusLabel
            // 
            this.serverAddressStripStatusLabel.Name = "serverAddressStripStatusLabel";
            this.serverAddressStripStatusLabel.Size = new System.Drawing.Size(81, 17);
            this.serverAddressStripStatusLabel.Text = "Server Address";
            // 
            // serverStatusStripStatusLabel
            // 
            this.serverStatusStripStatusLabel.Name = "serverStatusStripStatusLabel";
            this.serverStatusStripStatusLabel.Size = new System.Drawing.Size(38, 17);
            this.serverStatusStripStatusLabel.Text = "Status";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(658, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "通信量: Up 215 kbps  Down 427 kbps";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MainLocationSplitContainer
            // 
            this.MainLocationSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MainLocationSplitContainer.Location = new System.Drawing.Point(0, 28);
            this.MainLocationSplitContainer.Name = "MainLocationSplitContainer";
            // 
            // MainLocationSplitContainer.Panel1
            // 
            this.MainLocationSplitContainer.Panel1.Controls.Add(this.LocationTabControl);
            this.MainLocationSplitContainer.Panel1MinSize = 238;
            // 
            // MainLocationSplitContainer.Panel2
            // 
            this.MainLocationSplitContainer.Panel2.Controls.Add(this.MainTabControl);
            this.MainLocationSplitContainer.Panel2MinSize = 250;
            this.MainLocationSplitContainer.Size = new System.Drawing.Size(792, 422);
            this.MainLocationSplitContainer.SplitterDistance = 272;
            this.MainLocationSplitContainer.TabIndex = 21;
            // 
            // ArenaClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 473);
            this.Controls.Add(this.MainLocationSplitContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.ServerAddressComboBox);
            this.Controls.Add(this.labelLanAdapter);
            this.Controls.Add(this.toggleCaptureLanAdapterButton);
            this.Controls.Add(this.lanAdapterComboBox);
            this.Controls.Add(this.labelServerAddress);
            this.Controls.Add(this.ServerLoginButton);
            this.MinimumSize = new System.Drawing.Size(800, 350);
            this.Name = "ArenaClientForm";
            this.Text = "PSP.NetParty";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ArenaClientForm_FormClosing);
            this.LocationTabControl.ResumeLayout(false);
            this.PortalTabPage.ResumeLayout(false);
            this.ArenaTabPage.ResumeLayout(false);
            this.PlayRoomTabPage.ResumeLayout(false);
            this.PlayRoomTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RoomMaxPlayerNumericUpDown)).EndInit();
            this.PacketMonitorTabPage.ResumeLayout(false);
            this.PacketMonitorSplitContainer.Panel1.ResumeLayout(false);
            this.PacketMonitorSplitContainer.Panel1.PerformLayout();
            this.PacketMonitorSplitContainer.Panel2.ResumeLayout(false);
            this.PacketMonitorSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PacketMonitorSplitContainer)).EndInit();
            this.PacketMonitorSplitContainer.ResumeLayout(false);
            this.PlayerListContextMenu.ResumeLayout(false);
            this.MainTabControl.ResumeLayout(false);
            this.chatTagPage.ResumeLayout(false);
            this.chatTagPage.PerformLayout();
            this.chatSplitContainer.Panel1.ResumeLayout(false);
            this.chatSplitContainer.Panel1.PerformLayout();
            this.chatSplitContainer.Panel2.ResumeLayout(false);
            this.chatSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chatSplitContainer)).EndInit();
            this.chatSplitContainer.ResumeLayout(false);
            this.ConfigTagPage.ResumeLayout(false);
            this.ConfigTagPage.PerformLayout();
            this.logViewTagPage.ResumeLayout(false);
            this.logViewTagPage.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.MainLocationSplitContainer.Panel1.ResumeLayout(false);
            this.MainLocationSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainLocationSplitContainer)).EndInit();
            this.MainLocationSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox CommandTextBox;
        private System.Windows.Forms.Button CommandSubmitButton;
        private System.Windows.Forms.Button ServerLoginButton;
        private System.Windows.Forms.ComboBox ServerAddressComboBox;
        private System.Windows.Forms.Label labelServerAddress;
        private System.Windows.Forms.ComboBox lanAdapterComboBox;
        private System.Windows.Forms.Button toggleCaptureLanAdapterButton;
        private System.Windows.Forms.Label labelLanAdapter;
        private System.Windows.Forms.Label playerListLabel;
        private System.Windows.Forms.TabControl LocationTabControl;
        private System.Windows.Forms.TabPage PortalTabPage;
        private System.Windows.Forms.TabPage ArenaTabPage;
        private System.Windows.Forms.TreeView playersTreeView;
        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage ConfigTagPage;
        private System.Windows.Forms.TabPage logViewTagPage;
        private System.Windows.Forms.ListView ArenaServerListView;
        private System.Windows.Forms.ListView RoomListView;
        private System.Windows.Forms.TabPage chatTagPage;
        private System.Windows.Forms.Label LoginUserNameLabel;
        private System.Windows.Forms.TextBox LoginUserNameTextBox;
        private System.Windows.Forms.SplitContainer chatSplitContainer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel serverAddressStripStatusLabel;
        private System.Windows.Forms.TabPage PlayRoomTabPage;
        private System.Windows.Forms.TextBox RoomMasterNameTextBox;
        private System.Windows.Forms.Label roomMasterLabel;
        private System.Windows.Forms.Label maxParticipantsLabel;
        private System.Windows.Forms.NumericUpDown RoomMaxPlayerNumericUpDown;
        private System.Windows.Forms.TextBox RoomDescriptionTextBox;
        private System.Windows.Forms.TextBox RoomTitleTextBox;
        private System.Windows.Forms.Button RoomCreateEditButton;
        private System.Windows.Forms.Button RoomCloseExitButton;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripStatusLabel serverStatusStripStatusLabel;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.TextBox RoomPasswordTextBox;
        private System.Windows.Forms.Label RoomPasswordLabel;
        private System.Windows.Forms.Label RoomTitleLabel;
        private System.Windows.Forms.Label RoomDescriptionLabel;
        private System.Windows.Forms.Label LoginUserNameAlertLabel;
        private System.Windows.Forms.SplitContainer MainLocationSplitContainer;
        private System.Windows.Forms.CheckBox ChatLogNotDisplayRoomActionCheckBox;
        private System.Windows.Forms.TextBox ChatLogTextBox;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ContextMenuStrip PlayerListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem KickPlayer;
        private System.Windows.Forms.ListView PacketMonitorMyPspListView;
        private System.Windows.Forms.Label PacketMonitorMyPspLabel;
        private System.Windows.Forms.ListView PacketMonitorRemotePspListView;
        private System.Windows.Forms.Label PacketMonitorRemotePspLabel;
        private System.Windows.Forms.ColumnHeader MyPspMacColumnHeader;
        private System.Windows.Forms.ColumnHeader MyPspInSpeedColumnHeader;
        private System.Windows.Forms.ColumnHeader MyPspOutSpeedColumnHeader;
        private System.Windows.Forms.Button PacketMonitorRemotePspClearButton;
        private System.Windows.Forms.Button PacketMonitorMyPspClearButton;
        private System.Windows.Forms.ColumnHeader RemotePspMacColumnHeader;
        private System.Windows.Forms.ColumnHeader RemotePspInSpeedColumnHeader;
        private System.Windows.Forms.ColumnHeader RemotePspOutSpeedColumnHeader;
        private System.Windows.Forms.ColumnHeader MyPspInBytesColumnHeader;
        private System.Windows.Forms.ColumnHeader MyPspOutBytesColumnHeader;
        private System.Windows.Forms.ColumnHeader RemotePspInBytesColumnHeader;
        private System.Windows.Forms.ColumnHeader RemotePspOutBytesColumnHeader;
        private System.Windows.Forms.TabPage PacketMonitorTabPage;
        private System.Windows.Forms.SplitContainer PacketMonitorSplitContainer;
    }
}