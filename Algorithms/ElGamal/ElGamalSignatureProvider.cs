using Algorithms.Abstractions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System.Security.Cryptography;
using Algorithms.Common;
using Org.BouncyCastle.Security;

namespace Algorithms.ElGamal;
public sealed class ElGamalSignatureProvider : ISignatureProvider<AsymmetricKeyParameter>
{
    public byte[] Sign(byte[] data, AsymmetricKeyParameter privateKey)
    {
        var hash = SHA256.HashData(data);

        var key = (ElGamalPrivateKeyParameters)privateKey;

        var p = key.Parameters.P;
        var pSubOne = p.Subtract(BigInteger.One);

        var k = GenerateK(pSubOne);

        var r = key.Parameters.G.ModPow(k, p);

        var m = new BigInteger(1, hash, 0, hash.Length);

        if (m.CompareTo(p) >= 1)
        {
            throw new ArgumentException("Data is too big", nameof(data));
        }

        var s = m.Subtract(key.X.Multiply(r)).Multiply(k.ModInverse(pSubOne)).Mod(pSubOne);

        return s.Equals(BigInteger.Zero) 
            ? Sign(data, privateKey) 
            : NormalizeSignaturePart(r.ToByteArrayUnsigned()).FastConcat(NormalizeSignaturePart(s.ToByteArrayUnsigned()));
    }

    private static byte[] NormalizeSignaturePart(byte[] array)
    {
        var expectedLength = (InternalConstants.ElGamalKeySize + 8 - 1) / 8;
        if (expectedLength == array.Length)
        {
            return array;
        }

        byte[] result = new byte[expectedLength];
        var difference = expectedLength - array.Length;
        Buffer.BlockCopy(array, 0, result, difference, array.Length);
        return result;
    }

    private static BigInteger GenerateK(BigInteger pSubOne)
    {
        BigInteger k;
        do
        {
            k = new BigInteger(InternalConstants.ElGamalKeySize, new SecureRandom()).Mod(pSubOne);
        } while (k.CompareTo(BigInteger.One) <= 0 
            || !k.Gcd(pSubOne).Equals(BigInteger.One));

        return k;
    }

    public bool Verify(byte[] data, byte[] signature, AsymmetricKeyParameter publicKey)
    {
        var hash = SHA256.HashData(data);
        var key = (ElGamalPublicKeyParameters)publicKey;
        var signatureAsSpan = signature.AsSpan();
        var halfSignatureSize = signature.Length / 2;
        var r = new BigInteger(1, signatureAsSpan[..halfSignatureSize].ToArray());
        var s = new BigInteger(1, signatureAsSpan.Slice(halfSignatureSize, halfSignatureSize).ToArray());

        var p = key.Parameters.P;

        if (r.CompareTo(BigInteger.Zero) < 0 || r.CompareTo(p) >= 0 || s.CompareTo(BigInteger.Zero) < 0 || s.CompareTo(p.Subtract(BigInteger.One)) >= 0)
        {
            return false;
        }

        var m = new BigInteger(1, hash, 0, hash.Length);

        var leftPart = key.Y.ModPow(r, p).Multiply(r.ModPow(s, p)).Mod(p);
        var rightPart = key.Parameters.G.ModPow(m, p);

        return leftPart.Equals(rightPart);
    }
}
