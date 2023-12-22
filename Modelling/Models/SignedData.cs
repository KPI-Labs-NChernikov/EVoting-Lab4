namespace Modelling.Models;
public sealed class SignedData<T>
{
    public T Data { get; }

    public byte[] Signature { get; }

    public SignedData(T data, byte[] signature)
    {
        Data = data;
        Signature = signature;
    }
}
