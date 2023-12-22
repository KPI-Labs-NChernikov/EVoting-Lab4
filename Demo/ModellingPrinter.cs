using Modelling.Extensions;
using Modelling.Models;

namespace Demo;
public sealed class ModellingPrinter
{
    public void PrintUsualVoting(IReadOnlyList<Voter> voters, Dictionary<Voter, int> votersWithCandidatesIds)
    {
        Console.WriteLine("Usual voting: ");
        var ballots = new List<byte[]>();
        foreach (var voter in voters)
        {
            voter.Voters = voters.ToList();
            var ballot = voter.PrepareBallot(votersWithCandidatesIds[voter]);
            ballots.Add(ballot);
            Console.WriteLine($"Voter {voter.FullName} prepared the ballot.");
        }
        IReadOnlyList<byte[]> currentBallots = ballots;
        foreach (var voter in voters)
        {
            var result = voter.Decrypt(currentBallots);
            if (!result.IsSuccess)
            {
                result.PrintErrorIfFailed();
                return;
            }
            else
            {
                Console.WriteLine($"Voter {voter.FullName} decrypted the ballots.");
            }
            currentBallots = result.Value;
        }
        SignedData<IReadOnlyList<byte[]>> signedBallots = null!;
        for (var i = 0; i < voters.Count; i++)
        {
            var voter = voters[i];
            var result = i == 0 ? voter.DecryptAndSign(currentBallots) : voter.VerifyDecryptAndSign(signedBallots);
            if (!result.IsSuccess)
            {
                result.PrintErrorIfFailed();
                return;
            }
            else
            {
                Console.WriteLine($"Voter {voter.FullName} decrypted and signed the ballots.");
            }
            signedBallots = result.Value;
        }
        var votingResults = new List<VotingResults>();
        foreach (var voter in voters)
        {
            var result = voter.CompleteVoting(signedBallots);
            if (!result.IsSuccess)
            {
                result.PrintErrorIfFailed();
                Console.WriteLine("Voting should be treated as not successful.");
                return;
            }
            else
            {
                Console.WriteLine($"Voter {voter.FullName} completed the voting.");
            }
            votingResults.Add(result.Value);
        }

        for (var i = 0; i < votingResults.Count - 1; i++)
        {
            Console.WriteLine($"results {i + 1} == results {i + 2}: {votingResults[i].Equals(votingResults[i+1])}");
        }

        PrintVotingResults(votingResults.First());
        Console.WriteLine();
    }

    public void PrintVotingWithExit(IReadOnlyList<Voter> voters, Dictionary<Voter, int> votersWithCandidatesIds)
    {
        Console.WriteLine("Voting with exit: ");
        var ballots = new List<byte[]>();
        foreach (var voter in voters)
        {
            voter.Voters = voters.ToList();
            var ballot = voter.PrepareBallot(votersWithCandidatesIds[voter]);
            ballots.Add(ballot);
            Console.WriteLine($"Voter {voter.FullName} prepared the ballot.");
        }
        IReadOnlyList<byte[]> currentBallots = ballots;
        foreach (var voter in voters)
        {
            var result = voter.Decrypt(currentBallots);
            if (!result.IsSuccess)
            {
                result.PrintErrorIfFailed();
                return;
            }
            else
            {
                Console.WriteLine($"Voter {voter.FullName} decrypted the ballots.");
            }
            var tempResults = result.Value.ToList();
            tempResults.Remove(tempResults.First());
            currentBallots = tempResults;
        }
        //SignedData<IReadOnlyList<byte[]>> signedBallots = null!;
        //for (var i = 0; i < voters.Count; i++)
        //{
        //    var voter = voters[i];
        //    var result = i == 0 ? voter.DecryptAndSign(currentBallots) : voter.VerifyDecryptAndSign(signedBallots);
        //    if (!result.IsSuccess)
        //    {
        //        result.PrintErrorIfFailed();
        //        return;
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Voter {voter.FullName} decrypted and signed the ballots.");
        //    }
        //    signedBallots = result.Value;
        //}
        //var votingResults = new List<VotingResults>();
        //foreach (var voter in voters)
        //{
        //    var result = voter.CompleteVoting(signedBallots);
        //    if (!result.IsSuccess)
        //    {
        //        result.PrintErrorIfFailed();
        //        Console.WriteLine("Voting should be treated as not successful.");
        //        return;
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Voter {voter.FullName} completed the voting.");
        //    }
        //    votingResults.Add(result.Value);
        //}

        //for (var i = 0; i < votingResults.Count - 1; i++)
        //{
        //    Console.WriteLine($"results {i + 1} == results {i + 2}: {votingResults[i].Equals(votingResults[i + 1])}");
        //}

        //PrintVotingResults(votingResults.First());
        Console.WriteLine();
    }


    private static void PrintVotingResults(VotingResults results)
    {
        Console.WriteLine("Results:");
        foreach (var candidate in results.CandidatesResults.Values.OrderByVotes())
        {
            Console.WriteLine($"{candidate.Candidate.FullName} (id: {candidate.Candidate.Id}): {candidate.Votes} votes");
        }
    }
}
