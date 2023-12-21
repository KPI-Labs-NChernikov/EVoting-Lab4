namespace Algorithms.Abstractions;
public interface IPaddingProvider
{
    byte[] GeneratePadding(int size);
}
