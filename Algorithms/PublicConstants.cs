using System.Text;

namespace Algorithms;
public static class PublicConstants
{
    public const int ElGamalSignatureSize = 256;
    public const int IntSize = sizeof(int);
    public const int GuidSize = 16;
    public static readonly Encoding Encoding = Encoding.UTF8;
    public const int AesKeySize = 256;
    public const int AesBlockSize = 128;
    public const int RsaKeySize = 1024;
}
