using System.IO;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
    /// <summary>
    /// The encoder for Tonic Trouble .sna data files
    /// </summary>
    public class SNA_TTWindowEncoder : IStreamEncoder
    {
        public string Name => "SNA_TTWindowEncoding";

        private static void Process(Stream inputStream, Stream outputStream, bool isDecoding)
        {
            byte[] originalMaskBytes = { 0x41, 0x59, 0xBE, 0xC7, 0x0D, 0x99, 0x1C, 0xA3, 0x75, 0x3F };
            byte[] maskBytes = originalMaskBytes.ToArray();
            uint currentMaskByte = 0;

            // Get the length
            long length = inputStream.Length - inputStream.Position;

            // Enumerate every byte
            for (long i = 0; i < length; i++)
            {
                // Read the byte
                byte b = (byte)inputStream.ReadByte();

                // Decode the byte
                byte decodedByte = (byte)(b ^ (maskBytes[currentMaskByte]));
                maskBytes[currentMaskByte] = (byte)(originalMaskBytes[currentMaskByte] + (isDecoding ? b : decodedByte));
                currentMaskByte = (uint)((currentMaskByte + 1) % maskBytes.Length);
                b = decodedByte;

                // Write the byte
                outputStream.WriteByte(b);
            }
        }

        public void DecodeStream(Stream input, Stream output)
        {
            Process(input, output, true);
        }

        public void EncodeStream(Stream input, Stream output)
        {
            Process(input, output, false);
        }
    }
}