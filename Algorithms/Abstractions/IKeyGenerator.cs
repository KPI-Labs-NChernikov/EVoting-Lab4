using Algorithms.Common;

namespace Algorithms.Abstractions;
public interface IKeyGenerator<TKey>
{
    Keys<TKey> Generate();
}
