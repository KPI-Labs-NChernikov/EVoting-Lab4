using Algorithms.Abstractions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;

namespace Algorithms.RSA;
public sealed class RSAEncryptionProvider : IEncryptionProvider<AsymmetricKeyParameter>
{
    public byte[] Encrypt(byte[] data, AsymmetricKeyParameter publicKey)
    {
        var engine = new RsaEngine();
        engine.Init(true, publicKey);

        var processed = engine.ProcessBlock(data, 0, data.Length);

        return processed;
    }

    public byte[] Decrypt(byte[] data, AsymmetricKeyParameter privateKey)
    {
        var engine = new RsaEngine();
        engine.Init(false, privateKey);

        var processed = engine.ProcessBlock(data, 0, data.Length);

        return processed;
    }

    private static byte[] AddPadding(byte[] data, byte[] padding)
    {
        byte[] result = new byte[data.Length + padding.Length];
        Buffer.BlockCopy(data, 0, result, 0, data.Length);
        Buffer.BlockCopy(padding, 0, result, data.Length, padding.Length);
        return result;
    }

    private static byte[] RemovePadding(byte[] data, byte[] padding)
    {
        var dataSpan = data.AsSpan();
        var dataEndsWithPadding = dataSpan.Slice(data.Length - padding.Length, padding.Length).SequenceEqual(padding);
        if (!dataEndsWithPadding)
        {
            throw new InvalidOperationException("The padding is incorrect.");
        }

        return dataSpan[..(data.Length - padding.Length)].ToArray();
    }
}
