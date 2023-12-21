using Algorithms.Common;

namespace Algorithms.Abstractions;
public interface IComplexEncryptionProvider<TKey>
{
    ComplexEncryptedData Encrypt(byte[] data, TKey publicKey);
    byte[] Decrypt(ComplexEncryptedData data, TKey privateKey);
}
