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
using System.Linq;
using System.Text;

namespace PspDotNetParty
{
    public class PacketData
    {
        byte[] _rawBytes;
        int count;
        public byte[] RawBytes
        {
            get
            {
                byte[] buff = new byte[count];
                Array.Copy(_rawBytes, 0, buff, 0, buff.Length);
                return buff;
            }
        }

        public string[] Messages
        {
            get
            {
                string messages = Encoding.UTF8.GetString(_rawBytes, 0, count).Trim();
                return messages.Split(Protocol1Constants.MESSAGE_SEPARATOR);
            }
        }

        public PacketData(byte[] rawData, int count)
        {
            this._rawBytes = rawData;
            this.count = count;
        }
    }
}
