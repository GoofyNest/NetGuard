﻿using System.IO;

namespace SilkroadSecurityAPI
{
    internal class PacketWriter : BinaryWriter
    {
        private readonly MemoryStream m_ms;

        public PacketWriter()
        {
            m_ms = new MemoryStream();
            this.OutStream = m_ms;
        }

        public byte[] GetBytes()
        {
            return m_ms.ToArray();
        }
    }
}