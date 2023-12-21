namespace Algorithms.Abstractions;
public interface ISignatureProvider<TKey>
{
    byte[] Sign(byte[] data, TKey privateKey);
    bool Verify(byte[] data, byte[] signature, TKey publicKey);
}
