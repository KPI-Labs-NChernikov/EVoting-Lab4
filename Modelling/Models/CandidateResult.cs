namespace Modelling.Models;
public sealed class CandidateResult : IEquatable<CandidateResult>
{
    public Candidate Candidate { get; }

    public int Votes { get; set; }

    public CandidateResult(Candidate candidate)
    {
        Candidate = candidate;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CandidateResult);
    }

    public bool Equals(CandidateResult? other)
    {
        if (other is null)
        {
            return false;
        }

        return Candidate == other.Candidate && Votes == other.Votes;
    }

    public static bool operator ==(CandidateResult? obj1, CandidateResult? obj2)
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

    public static bool operator !=(CandidateResult? obj1, CandidateResult? obj2) => !(obj1 == obj2);

    public override int GetHashCode()
    {
        return HashCode.Combine(Candidate, Votes);
    }
}
