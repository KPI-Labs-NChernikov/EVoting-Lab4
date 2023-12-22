using Algorithms.Abstractions;
using Algorithms.Common;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using System.Security.Cryptography;

namespace Algorithms.AESWithRSA;
public sealed class ComplexEncryptionService : IComplexEncryptionProvider<AsymmetricKeyParameter>
{
    public ComplexEncryptedData Encrypt(byte[] data, AsymmetricKeyParameter publicKey)
    {
        using var aes = Aes.Create();
        aes.KeySize = PublicConstants.AesKeySize;
        aes.BlockSize = PublicConstants.AesBlockSize;
        aes.GenerateKey();
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        csEncrypt.Write(data, 0, data.Length);
        csEncrypt.FlushFinalBlock();
        var encryptedData = msEncrypt.ToArray();

        var engine = new RsaEngine();
        engine.Init(true, publicKey);
        var aesKey = aes.Key.FastConcat(aes.IV);
        var encryptedAesKey = engine.ProcessBlock(aesKey, 0, aesKey.Length);

        return new ComplexEncryptedData(encryptedAesKey, encryptedData);
    }

    public byte[] Decrypt(ComplexEncryptedData data, AsymmetricKeyParameter privateKey)
    {
        var engine = new RsaEngine();
        engine.Init(false, privateKey);
        var decryptedAesKey = engine.ProcessBlock(data.EncryptedKey, 0, data.EncryptedKey.Length);

        using var aes = Aes.Create();
        aes.KeySize = PublicConstants.AesKeySize;
        aes.BlockSize = PublicConstants.AesBlockSize;
        var aesKeyAndIVSpan = decryptedAesKey.AsSpan();
        aes.IV = aesKeyAndIVSpan.Slice(aesKeyAndIVSpan.Length - UtilityMethods.BitsToBytes(PublicConstants.AesBlockSize), UtilityMethods.BitsToBytes(PublicConstants.AesBlockSize)).ToArray();
        aes.Key = aesKeyAndIVSpan.Slice(0, aesKeyAndIVSpan.Length - UtilityMethods.BitsToBytes(PublicConstants.AesBlockSize)).ToArray();

        using var decryptor = aes.CreateDecryptor();
        using var msPlain = new MemoryStream();
        using var csDecrypt = new CryptoStream(msPlain, decryptor, CryptoStreamMode.Write);
        csDecrypt.Write(data.EncryptedData, 0, data.EncryptedData.Length);
        csDecrypt.FlushFinalBlock();

        return msPlain.ToArray();
    }
}
