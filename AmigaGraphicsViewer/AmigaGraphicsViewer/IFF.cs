using System;
using System.Collections.Generic;
using System.IO;

namespace MemoryViewer
{
    public class IFF
    {
        private List<Chunk> chunks = new List<Chunk>();
        private byte[] chunkData = { };
        private byte[] imageData = { };
        private byte numBitplanes = 1;

        public IFF(byte[] bytes, byte numOfBitplanes)
        {
            numBitplanes = numOfBitplanes;
            if (bytes.Length > 0)
            {
                imageData = bytes;
            }
            else
            {
                imageData = new byte[40 * 256];
            }
        }

        public void BuildChunks()
        {
            chunks.Add(new Chunk(new byte[] { 0x46, 0x4f, 0x52, 0x4d }, "FORM"));
            chunks.Add(new Chunk(new byte[] { 0x49, 0x4c, 0x42, 0x4d }, "ILBM"));
            chunks.Add(new BitmapHeader(new byte[] { 0x42, 0x4d, 0x48, 0x44 }, "BMHD"));
            chunks.Add(new Chunk(new byte[] { 0x43, 0x4d, 0x41, 0x50 }, "CMAP"));
            chunks.Add(new Body(new byte[] { 0x42, 0x4f, 0x44, 0x59 }, "BODY"));
        }

        public void UpdateChunks(byte numOfBitplanes)
        {
            // Form + Size
            chunks[0].AddData(new byte[] { });
            chunks[0].CalculateChunkSize();

            // ILBM - No SIZE
            chunks[1].AddData(null);
            chunks[1].CalculateChunkSize();

            // BMHD + Size
            chunks[2].AddData(new byte[] { }, numOfBitplanes);
            chunks[2].CalculateChunkSize();

            // Add some colour ( R,G,B ) - 2,4,8,16,32 colours in Low Res 340x256
            chunks[3].AddData(new byte[] { 0xf0, 0xf0, 0xf0, 0x0, 0x0, 0x0 });
            chunks[3].CalculateChunkSize();

            // Add the empty body data for 1 bitplane
            chunks[4].AddData(imageData, numOfBitplanes);// new byte[40 * 256]);
            chunks[4].CalculateChunkSize();

            // Calculate ILBM FORM size (-12)
            int totalFormSize = 0;
            totalFormSize += chunks[0].ChunkSize;
            totalFormSize += chunks[1].ChunkSize;
            totalFormSize += chunks[2].ChunkSize;
            totalFormSize += chunks[3].ChunkSize;
            totalFormSize += chunks[4].ChunkSize - 12;


            chunks[0].ChunkSize = totalFormSize;
            chunks[0].UpdateFileSize(totalFormSize);


            // Set the 
            chunkData = new byte[totalFormSize + 12];
        }

        public void GetChunkData()
        {
            int index = 0;
            foreach (Chunk chunk in chunks)
            {
                for (int i = 0; i < chunk.ChunkData.Length; i++)
                {
                    chunkData[index++] = chunk.ChunkData[i];
                }
            }
        }

        public void WriteIFF(string filename)
        {
            File.WriteAllBytes(filename, chunkData);
        }


        private void ConvertUShortToTwoBytes(ushort input, ref byte[] bytes, ref int index)
        {
            byte[] ushortBytes = BitConverter.GetBytes(input);
            bytes[index++] = ushortBytes[1];
            bytes[index++] = ushortBytes[0];
        }

        private void ConvertShortToTwoBytes(short input, ref byte[] bytes, ref int index)
        {
            byte[] shortBytes = BitConverter.GetBytes(input);
            bytes[index++] = shortBytes[1];
            bytes[index++] = shortBytes[0];
        }


    }
}
