namespace BinarySerializer.Ubisoft.CPA
{
    public class SNA_MontrealXorProcessor : XorProcessor
    {
		public SNA_MontrealXorProcessor(int multiply, int add) {
			Multiply = multiply;
			Add = add;
		}

		public int Multiply { get; set; }
		public int Add { get; set; }

		public int CurrentIndex { get; set; } = 0;


		public byte XORByte(byte b) {
			b = (byte)(BitHelpers.ExtractBits64(b + Multiply * CurrentIndex + Add, 8, 0));
			CurrentIndex++;
			return b;
		}

		public override void ProcessBytes(byte[] buffer, int offset, int count) {
			int end = offset + count;
			for (int i = offset; i < end; i++) {
				buffer[i] = XORByte(buffer[i]);
			}
		}
	}
}