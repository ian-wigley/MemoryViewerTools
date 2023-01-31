using System;

namespace MemoryAndGraphicsViewer
{
    public class BitmapHeader : Chunk
    {
        public ushort width, height;
        public short x, y;
        public byte nPlanes;
        public byte masking;
        public byte compression;
        public byte pad1;
        public ushort transparentColour;
        public byte xAspect, yAspect;
        public short pageWidth, pageHeight;

        public BitmapHeader(byte[] name, string textualName) : base(name, textualName)
        {
            m_name = name;
            m_textualName = textualName;
            width = 320;
            height = 256;
            x = 0;
            y = 0;
            nPlanes = 1;
            masking = 0;
            compression = 0;
            pad1 = 0;
            transparentColour = 0;
            xAspect = 0;
            yAspect = 0;
            pageWidth = 320;
            pageHeight = 256;// 320;
        }

        public override void AddData(byte[] data, byte numPlanes)
        {
            nPlanes = numPlanes;
      
            byte[] dataSize = SwapBytes(BitConverter.GetBytes(20));

            // Update the size of the byte array - ** MAGIC NUMBER ** 20 - Size of the bytes needed for the header data
            m_data = new byte[m_name.Length + dataSize.Length + 20];

            int index = 0;
            for (int i = 0; i < m_name.Length; i++)
            {
                m_data[index++] = m_name[i];
            }

            for (int i = 0; i < dataSize.Length; i++)
            {
                m_data[index++] = dataSize[i];
            }

            ConvertUShortToTwoBytes(width, ref m_data, ref index);
            ConvertUShortToTwoBytes(height, ref m_data, ref index);
            ConvertShortToTwoBytes(x, ref m_data, ref index);
            ConvertShortToTwoBytes(y, ref m_data, ref index);
            m_data[index++] = nPlanes;
            m_data[index++] = masking;
            m_data[index++] = compression;
            m_data[index++] = pad1;
            ConvertUShortToTwoBytes(transparentColour, ref m_data, ref index);
            m_data[index++] = xAspect;
            m_data[index++] = yAspect;
            ConvertShortToTwoBytes(pageWidth, ref m_data, ref index);
            ConvertShortToTwoBytes(pageHeight, ref m_data, ref index);
        }
    }
}
