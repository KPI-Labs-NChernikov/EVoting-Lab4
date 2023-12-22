using Modelling.Models;

namespace Modelling.Extensions;
public static class LinqExtensions
{
    public static IOrderedEnumerable<CandidateResult> OrderByVotes(this IEnumerable<CandidateResult> candidatesVotingResults)
        => candidatesVotingResults.OrderByDescending(c => c.Votes);
}
