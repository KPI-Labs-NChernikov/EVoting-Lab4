namespace Modelling.Models;
public sealed class Ballot
{
    public int CandidateId { get; }

    public Ballot(int candidateId)
    {
        CandidateId = candidateId;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Ballot);
    }

    public bool Equals(Ballot? other)
    {
        if (other is null)
        {
            return false;
        }

        return CandidateId == other.CandidateId;
    }

    public static bool operator ==(Ballot? obj1, Ballot? obj2)
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

    public static bool operator !=(Ballot? obj1, Ballot? obj2) => !(obj1 == obj2);

    public override int GetHashCode()
    {
        return HashCode.Combine(CandidateId);
    }
}
