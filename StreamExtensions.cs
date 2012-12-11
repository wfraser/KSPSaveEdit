using System.IO;
using System.Text;

namespace KSPSaveEdit
{
    public static class StreamExtensions
    {
        public static void Write(this Stream stream, string output)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(output);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void Write(this Stream stream, byte b)
        {
            stream.Write(new byte[] { b }, 0, 1);
        }
    }
}
