namespace BinToAssembly
{
    public class Branch : OpCode
    {
        public Branch(
            string code,
            string name,
            int numberOfBytes,
            string prefix,
            string firstfix,
            string midfix,
            string suffix,
            string methodName
            ) : base(code, name, numberOfBytes, prefix, firstfix, midfix, suffix, methodName)
        {
        }


        public new string Detail(ref int filePosition, byte[] binaryFileData)
        {
            string elementOne = "";
            string elementTwo = "";
            string elementThree = "";
            string elementFour = "";
            return "";
        }
    }
}
