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
using System.Net.NetworkInformation;

namespace PspDotNetParty
{
    public sealed class Utility
    {
        public const int HEADER_OFFSET = 0;//8;

        public static bool IsPspPacket(byte[] ethernetPacket, int offset) {
            return ethernetPacket.Length > 14 + offset && ethernetPacket[offset + 12] == 136 && ethernetPacket[offset + 13] == 200;
        }

        static PhysicalAddress _broadcastAddress;
        public static PhysicalAddress GetBroadcastAddress()
        {
            if (_broadcastAddress == null)
                _broadcastAddress = new PhysicalAddress(new byte[] { 255, 255, 255, 255, 255, 255 });
            return _broadcastAddress;
        }
    }
}
