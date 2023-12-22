namespace Modelling.Models;
public sealed class Candidate : IEquatable<Candidate>
{
    public int Id { get; }

    public string FullName { get; }

    public Candidate(int id, string fullName)
    {
        Id = id;
        FullName = fullName;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Candidate);
    }

    public bool Equals(Candidate? other)
    {
        if (other is null)
        {
            return false;
        }

        return Id == other.Id && FullName == other.FullName;
    }

    public static bool operator ==(Candidate? obj1, Candidate? obj2)
    {
        if (obj1 is null && obj2 is null)
        {
            return true;
        }
        if (obj1 is null)
        {
            return false;
        }

        return obj1.Equals(obj2);
    }

    public static bool operator !=(Candidate? obj1, Candidate? obj2) => !(obj1 == obj2);

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, FullName);
    }
}
