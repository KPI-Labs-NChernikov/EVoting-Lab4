namespace Modelling.Models;
public sealed class VotingResults : IEquatable<VotingResults>
{
    public SortedDictionary<int, CandidateResult> CandidatesResults { get; } = [];

    public ICollection<Ballot> Ballots { get; } = new List<Ballot>();

    public override bool Equals(object? obj)
    {
        return Equals(obj as VotingResults);
    }

    public bool Equals(VotingResults? other)
    {
        if (other is null)
        {
            return false;
        }

        if (CandidatesResults.Count != other.CandidatesResults.Count
            || Ballots.Count != other.Ballots.Count)
        {
            return false;
        }

        foreach (var (candidate, otherCandidate) in CandidatesResults.Zip(other.CandidatesResults))
        {
            if (!candidate.Equals(otherCandidate))
            {
                return false;
            }
        }

        foreach (var (ballot, otherBallot) in Ballots.Zip(other.Ballots))
        {
            if (ballot != otherBallot)
            {
                return false;
            }
        }

        return true;
    }

    public static bool operator ==(VotingResults? obj1, VotingResults? obj2)
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

    public static bool operator !=(VotingResults? obj1, VotingResults? obj2) => !(obj1 == obj2);

    public override int GetHashCode()
    {
        var builder = new HashCode();

        foreach (var candidateResult in CandidatesResults)
        {
            builder.Add(candidateResult);
        }

        foreach (var ballot in Ballots)
        {
            builder.Add(ballot);
        }

        return builder.ToHashCode();
    }
}
