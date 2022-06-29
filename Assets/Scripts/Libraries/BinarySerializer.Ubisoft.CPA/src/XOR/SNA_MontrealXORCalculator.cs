namespace BinarySerializer.Ubisoft.CPA
{
    public class SNA_MontrealXORCalculator : IXORCalculator 
    {
		public SNA_MontrealXORCalculator(int multiply, int add) {
			Multiply = multiply;
			Add = add;
		}

		public int Multiply { get; set; }
		public int Add { get; set; }

		public int CurrentIndex { get; set; } = 0;

        public byte XORByte(byte b) 
        {
			b = (byte)(BitHelpers.ExtractBits64(b + Multiply * CurrentIndex + Add, 8, 0)); 
			CurrentIndex++;
			return b;
        }
	}
}