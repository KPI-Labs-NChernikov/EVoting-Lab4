using Algorithms.Abstractions;
using System.Security.Cryptography;

namespace Algorithms.Common;
public sealed class PaddingProvider : IPaddingProvider
{
    private readonly RandomNumberGenerator _cryptoRandom;

    public PaddingProvider(RandomNumberGenerator cryptoRandom)
    {
        _cryptoRandom = cryptoRandom;
    }

    public byte[] GeneratePadding(int size)
    {
        var result = new byte[size];
        _cryptoRandom.GetBytes(result, 0, result.Length);
        return result;
    }
}
