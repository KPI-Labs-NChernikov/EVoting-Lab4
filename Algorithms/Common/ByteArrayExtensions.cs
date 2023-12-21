namespace Algorithms.Common;
public static class ByteArrayExtensions
{
    public static byte[] FastConcat(this byte[] array, byte[] another)
    {
        byte[] result = new byte[array.Length + another.Length];
        Buffer.BlockCopy(array, 0, result, 0, array.Length);
        Buffer.BlockCopy(another, 0, result, array.Length, another.Length);
        return result;
    }
}
