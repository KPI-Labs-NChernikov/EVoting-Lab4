using Algorithms;
using Algorithms.Abstractions;
using Algorithms.Common;
using Modelling.Models;

namespace Modelling.CustomTransformers;
public sealed class ModellingTransformer : IObjectToByteArrayTransformer
{
    public bool CanTransform(Type type)
    {
        return type == typeof(Ballot) 
            || type == typeof(SignedData<byte[]>) 
            || type == typeof(SignedData<ComplexEncryptedData>)
            || type == typeof(byte[])
            || type == typeof(ComplexEncryptedData);
    }

    private readonly GuidTransformer _guidTransformer = new ();

    public T? ReverseTransform<T>(byte[] data)
    {
        var span = data.AsSpan();
        if (typeof(T) == typeof(Ballot))
        {
            var candidateId = BitConverter.ToInt32(data);

            return (T)(object)new Ballot(candidateId);
        }
        if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(SignedData<>))
        {
            var actualData = GetType().GetMethod(nameof(ReverseTransform))!.MakeGenericMethod(typeof(T).GenericTypeArguments[0])
                .Invoke(this, new object[] { span.Slice(0, span.Length - PublicConstants.ElGamalSignatureSize).ToArray() });
            var signature = span.Slice(span.Length - PublicConstants.ElGamalSignatureSize, PublicConstants.ElGamalSignatureSize);
            return (T)Activator.CreateInstance(typeof(T), actualData!, signature.ToArray())!;
        }
        if (typeof(T) == typeof(byte[]))
        {
            return (T)(object)data;
        }
        if (typeof(T) == typeof(ComplexEncryptedData))
        {
            var keySize = UtilityMethods.BitsToBytes(PublicConstants.RsaKeySize);
            var key = span.Slice(0, keySize).ToArray();
            var encryptedData = span.Slice(keySize).ToArray();
            return (T)(object)new ComplexEncryptedData(key, encryptedData);
        }

        throw new NotSupportedException($"The type {typeof(T)} is not supported.");
    }

    public byte[] Transform(object obj)
    {
        if (obj.GetType() == typeof(Ballot))
        {
            var ballot = (Ballot)obj;
            return BitConverter.GetBytes(ballot.CandidateId);
        }
        if (obj.GetType().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(SignedData<>))
        {
            using var stream = new MemoryStream();
            stream.Write(Transform(obj.GetType().GetProperty("Data")!.GetValue(obj)!));
            stream.Write((byte[])obj.GetType().GetProperty("Signature")!.GetValue(obj)!);
            return stream.ToArray();
        }
        if (obj.GetType() == typeof(byte[]))
        {
            return (byte[])obj;
        }
        if (obj.GetType() == typeof(ComplexEncryptedData))
        {
            var complexEncryptedData = (ComplexEncryptedData)obj;
            return complexEncryptedData.EncryptedKey.FastConcat(complexEncryptedData.EncryptedData);
        }

        throw new NotSupportedException($"The type {obj.GetType()} is not supported.");
    }
}
