namespace BinToAssembly
{
    public class BaseOpCode
    {
        protected int m_numberOfBytes = 0;
        protected string m_name = "";
        protected string m_prefix = "";
        protected string m_firstfix = "";
        protected string m_midfix = "";
        protected string m_suffix = "";
        protected string m_dataSize = "";
        protected string pad = "    ";

        public string m_codeOne { get; set; }
        public string m_codeTwo { get; set; }
        public string m_code { get; set; }
        public string Code { get { return m_code; } }
        public string Name { get { return m_name; } }
        public int NumberOfBytes { get { return m_numberOfBytes; } }
        public string Prefix { get { return m_prefix; } }
        public string Firstfix { get { return m_firstfix; } }
        public string Midfix { get { return m_midfix; } }
        public string Suffix { get { return m_suffix; } }

        public string Detail(ref int filePosition, byte[] binaryFileData)
        {
            string elementOne = "";
            string elementTwo = "";
            string elementThree = "";

            filePosition += 2;

            if (NumberOfBytes == 2)
            {
                elementOne = GetTwoShorts(ref filePosition, binaryFileData);
            }

            if (NumberOfBytes == 4)
            {
                elementOne = GetTwoShorts(ref filePosition, binaryFileData);
                elementTwo = GetTwoShorts(ref filePosition, binaryFileData);
            }

            if (NumberOfBytes == 6)
            {
                elementOne = GetTwoShorts(ref filePosition, binaryFileData);
                elementTwo = GetTwoShorts(ref filePosition, binaryFileData);
                elementThree = GetTwoShorts(ref filePosition, binaryFileData);
            }

            // Temporary fixes
            if (NumberOfBytes == 8)
            {
                filePosition += 8;
            }
            if (NumberOfBytes == 10)
            {
                filePosition += 10;
            }
            if (NumberOfBytes == 12)
            {
                filePosition += 12;
            }

            string binOne = !elementOne.Equals("") ? elementOne : pad;
            string binTwo = !elementTwo.Equals("") ? elementTwo : pad;

            if (Code.Equals("0777"))
            {
                elementOne = "";
            }

            string returnLine = " " + binOne + " " + binTwo + "          " + Name + " " + Prefix + elementOne + Firstfix + elementTwo + Midfix + elementThree + Suffix;
            return returnLine;
        }

        protected string GetTwoShorts(ref int filePosition, byte[] binaryFileData)
        {
            return ((short)binaryFileData[filePosition++]).ToString("X2") +
                    ((short)binaryFileData[filePosition++]).ToString("X2");
        }
    }
}
