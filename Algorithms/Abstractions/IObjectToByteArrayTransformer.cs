namespace Algorithms.Abstractions;
public interface IObjectToByteArrayTransformer
{
    bool CanTransform(Type type);

    byte[] Transform(object obj);

    T? ReverseTransform<T>(byte[] data); 
}
