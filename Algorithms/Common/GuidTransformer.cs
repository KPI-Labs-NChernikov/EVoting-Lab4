using Algorithms.Abstractions;

namespace Algorithms.Common;
public class GuidTransformer : IObjectToByteArrayTransformer
{
    public bool CanTransform(Type type)
    {
        return type == typeof(Guid);
    }

    public T? ReverseTransform<T>(byte[] data)
    {
        return (T)(object)new Guid(data);
    }

    public byte[] Transform(object obj)
    {
        return ((Guid)obj).ToByteArray();
    }
}
