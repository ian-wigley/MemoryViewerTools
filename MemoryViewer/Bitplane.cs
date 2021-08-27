namespace MemoryViewer
{
    public class Bitplane
    {
        // Each Bitplane is defined to 320 x 256 pixels (bits)
        // Therefore 40 x 256 bytes
        private byte[] bitplaneData = new byte[40 * 256];

        public byte[] GetBitPlanData { get { return bitplaneData; } }

        public Bitplane()
        {
        }

        public void AddData(byte[] data)
        {
            if (data.Length <= bitplaneData.Length)
            {
                bitplaneData = data;
            }
        }
    }
}
