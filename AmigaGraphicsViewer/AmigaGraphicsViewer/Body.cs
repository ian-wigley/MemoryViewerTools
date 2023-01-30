using System.Collections.Generic;

namespace MemoryViewer
{
    public class Body : Chunk
    {
        private List<Bitplane> bitplane = new List<Bitplane>();
        public Body(byte[] name, string textualName) : base(name, textualName)
        {
        }

        public override void AddData(byte[] data, byte numOfBitplanes)
        {
            for (int i = 0; i < numOfBitplanes; i++)
            {
                bitplane.Add(new Bitplane());
            }

            AddData(data);

        }
    }
}
