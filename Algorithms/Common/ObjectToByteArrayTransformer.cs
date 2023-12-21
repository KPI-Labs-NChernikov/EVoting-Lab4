using Algorithms.Abstractions;
using System.Text.Json;

namespace Algorithms.Common;
public sealed class ObjectToByteArrayTransformer : IObjectToByteArrayTransformer
{
    public IList<IObjectToByteArrayTransformer> TypeTransformers { get; } = new List<IObjectToByteArrayTransformer>();

    public byte[] Transform(object obj)
    {
        var transformer = TypeTransformers.FirstOrDefault(t => t.CanTransform(obj.GetType()));
        if (transformer is not null)
        {
            return transformer!.Transform(obj);
        }

        return PublicConstants.Encoding.GetBytes(JsonSerializer.Serialize(obj));
    }

    public T? ReverseTransform<T>(byte[] data)
    {
        var transformer = TypeTransformers.FirstOrDefault(t => t.CanTransform(typeof(T)));
        if (transformer is not null)
        {
            return transformer!.ReverseTransform<T>(data);
        }

        return JsonSerializer.Deserialize<T>(PublicConstants.Encoding.GetString(data));
    }

    public bool CanTransform(Type type)
    {
        return true;
    }
}
