using System;

namespace MemoryAndGraphicsViewer
{
    public class Chunk
    {
        // Name of the Chunk (byte format)
        protected byte[] m_name = { };
        protected string m_textualName = "";

        protected byte[] m_data = { };
        protected byte m_padding = 0;

        // Size of the Chunk
        protected int m_size = 0;

        public int ChunkSize { get { return m_size; } set { m_size = value; } }
        public byte[] ChunkData { get { return m_data; }}

        public Chunk(byte[] name, string textualName)
        {
            m_name = name;
            m_textualName = textualName;
        }

        public virtual void AddData(byte[] data, byte numPlanes) { }

        public virtual void AddData(byte[] data)
        {
            byte[] dataSize = { };
            if (null != data)
            {
                dataSize = SwapBytes(BitConverter.GetBytes(data.Length));
            }
            else
            {
                data = new byte[0];
            }
            // Update the size of the byte array
            m_data = new byte[m_name.Length + dataSize.Length + data.Length];

            int index = 0;
            for (int i = 0; i < m_name.Length; i++)
            {
                m_data[index++] = m_name[i];
            }

            for (int i = 0; i < dataSize.Length; i++)
            {
                m_data[index++] = dataSize[i];
            }

            for (int i = 0; i < data.Length; i++)
            {
                m_data[index++] = data[i];
            }
        }

        public void UpdateFileSize(int fileSize)
        {
            byte[] dataSize = SwapBytes(BitConverter.GetBytes(fileSize));
            m_data[m_data.Length - 4] = dataSize[0];
            m_data[m_data.Length - 3] = dataSize[1];
            m_data[m_data.Length - 2] = dataSize[2];
            m_data[m_data.Length - 1] = dataSize[3];
        }


        public void CalculateChunkSize()
        {
            m_size = m_data.Length;
        }

        public void ConvertChunkToBytes()
        {
        }

        protected byte[] SwapBytes(byte[] input)
        {
            return new byte[] { input[3], input[2], input[1], input[0] };
        }

        protected void ConvertUShortToTwoBytes(ushort input, ref byte[] bytes, ref int index)
        {
            byte[] ushortBytes = BitConverter.GetBytes(input);
            bytes[index++] = ushortBytes[1];
            bytes[index++] = ushortBytes[0];
        }

        protected void ConvertShortToTwoBytes(short input, ref byte[] bytes, ref int index)
        {
            byte[] shortBytes = BitConverter.GetBytes(input);
            bytes[index++] = shortBytes[1];
            bytes[index++] = shortBytes[0];
        }


    }
}
