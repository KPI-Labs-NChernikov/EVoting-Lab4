using Algorithms.Abstractions;
using Algorithms.Common;
using FluentResults;
using Modelling.Constants;
using Org.BouncyCastle.Crypto;

namespace Modelling.Models;
public sealed class Voter
{
    public string FullName { get; }

    public readonly IEnumerable<Candidate> _candidates = [];

    public AsymmetricKeyParameter SignaturePublicKey { get; }
    private readonly AsymmetricKeyParameter _signaturePrivateKey;

    public AsymmetricKeyParameter EncryptionPublicKey { get; }
    private readonly AsymmetricKeyParameter _encryptionPrivateKey;

    private readonly ISignatureProvider<AsymmetricKeyParameter> _signatureProvider;
    private readonly IComplexEncryptionProvider<AsymmetricKeyParameter> _encryptionProvider;

    private readonly IObjectToByteArrayTransformer _transformer;

    private readonly IPaddingProvider _paddingProvider;

    private readonly IRandomProvider _randomProvider;

    public List<Voter> Voters { get; set; } = [];

    private Stack<byte[]> _randomStrings = new();
    private Stack<byte[]> _ballotsAsByteArrays = new();

    public Voter(
        string fullName,
        IEnumerable<Candidate> candidates,
        Keys<AsymmetricKeyParameter> signatureKeys,
        Keys<AsymmetricKeyParameter> encryptionKeys,
        ISignatureProvider<AsymmetricKeyParameter> signatureProvider,
        IComplexEncryptionProvider<AsymmetricKeyParameter> encryptionProvider,
        IObjectToByteArrayTransformer transformer,
        IPaddingProvider paddingProvider,
        IRandomProvider randomProvider)
    {
        FullName = fullName;

        _candidates = candidates;

        SignaturePublicKey = signatureKeys.PublicKey;
        _signaturePrivateKey = signatureKeys.PrivateKey;

        EncryptionPublicKey = encryptionKeys.PublicKey;
        _encryptionPrivateKey = encryptionKeys.PrivateKey;

        _signatureProvider = signatureProvider;
        _encryptionProvider = encryptionProvider;
        _transformer = transformer;
        _paddingProvider = paddingProvider;
        _randomProvider = randomProvider;
    }

    public byte[] PrepareBallot(int candidateId)
    {
        _randomStrings = new();
        _ballotsAsByteArrays = new();

        var ballot = new Ballot(candidateId);
        var ballotAsByteArray = _transformer.Transform(ballot);
        var padding = _paddingProvider.GeneratePadding(PaddingConstants.PaddingSize);
        _randomStrings.Push(padding);
        _ballotsAsByteArrays.Push(ballotAsByteArray);
        ballotAsByteArray = ballotAsByteArray.FastConcat(padding);

        foreach (var voter in Voters.AsEnumerable().Reverse())
        {
            ballotAsByteArray = _transformer.Transform(_encryptionProvider.Encrypt(ballotAsByteArray, voter.EncryptionPublicKey));
            _ballotsAsByteArrays.Push(ballotAsByteArray);
        }

        foreach (var voter in Voters.AsEnumerable().Reverse())
        {
            padding = _paddingProvider.GeneratePadding(PaddingConstants.PaddingSize);
            _randomStrings.Push(padding);
            if (voter != Voters.Last())
            {
                _ballotsAsByteArrays.Push(ballotAsByteArray);
            }
            ballotAsByteArray = ballotAsByteArray.FastConcat(padding);
            ballotAsByteArray = _transformer.Transform(_encryptionProvider.Encrypt(ballotAsByteArray, voter.EncryptionPublicKey));
        }

        return ballotAsByteArray;
    }

    public Result<IReadOnlyList<byte[]>> Decrypt(IEnumerable<byte[]> ballots)
    {
        if (ballots.Count() != Voters.Count)
        {
            return Result.Fail(new Error("Some ballots are missing or there are too many ones."));
        }

        var decryptedBallots = ballots
            .Select(b =>
            {
                return Result.Try(() => _encryptionProvider.Decrypt(_transformer.ReverseTransform<ComplexEncryptedData>(b), _encryptionPrivateKey),
                    e => new Error("Message cannot be decrypted.").CausedBy(e));
            }).ToList();

        var firstError = decryptedBallots.FirstOrDefault(r => r.IsFailed);
        if (firstError is not null)
        {
            return firstError.ToResult();
        }

        var randomString = _randomStrings.Pop();
        var myBallot = decryptedBallots
            .FirstOrDefault(b => b.Value.AsSpan().Slice(b.Value.Length - PaddingConstants.PaddingSize, PaddingConstants.PaddingSize)
                .SequenceEqual(randomString));
        if (myBallot is null)
        {
            return Result.Fail(new Error($"The ballot of voter {FullName} was not found."));
        }

        var ballotAsByteArrays = _ballotsAsByteArrays.Pop();
        if (!myBallot.Value.AsSpan().Slice(0, myBallot.Value.Length - PaddingConstants.PaddingSize).SequenceEqual(ballotAsByteArrays))
        {
            return Result.Fail(new Error($"The ballot of voter {FullName} was changed."));
        }

        var decryptedBallotsWithoutPadding = decryptedBallots
            .Select(b => b.Value.AsSpan().Slice(0, b.Value.Length - PaddingConstants.PaddingSize).ToArray())
            .ToList();

        foreach (var voter in Voters)
        {
            if (voter == this)
            {
                continue;
            }

            voter.RemoveLastRandomString();
            voter.RemoveLastBallot();
        }

        return Result.Ok((IReadOnlyList<byte[]>)_randomProvider.Shuffle(decryptedBallotsWithoutPadding).ToList());
    }

    private void RemoveLastRandomString()
    {
        _randomStrings.TryPop(out var _);
    }

    private void RemoveLastBallot()
    {
        _ballotsAsByteArrays.TryPop(out var _);
    }

    public Result<SignedData<IReadOnlyList<byte[]>>> VerifyDecryptAndSign(SignedData<IReadOnlyList<byte[]>> ballots)
    {
        var previousVoter = Voters[Voters.FindIndex(v => v == this) - 1];

        var signatureIsAuthentic = _signatureProvider.Verify(_transformer.Transform(ballots.Data), ballots.Signature, previousVoter.SignaturePublicKey);
        if (!signatureIsAuthentic)
        {
            return Result.Fail($"Signature of voter {previousVoter.FullName} is not authentic.");
        }

        return DecryptAndSign(ballots.Data);
    }

    public Result<SignedData<IReadOnlyList<byte[]>>> DecryptAndSign(IEnumerable<byte[]> ballots)
    {
        if (ballots.Count() != Voters.Count)
        {
            return Result.Fail(new Error("Some ballots are missing or there are too many ones."));
        }

        var decryptedBallots = ballots
            .Select(b =>
            {
                return Result.Try(() => _encryptionProvider.Decrypt(_transformer.ReverseTransform<ComplexEncryptedData>(b), _encryptionPrivateKey),
                    e => new Error("Message cannot be decrypted.").CausedBy(e));
            }).ToList();

        var firstError = decryptedBallots.FirstOrDefault(r => r.IsFailed);
        if (firstError is not null)
        {
            return firstError.ToResult();
        }

        Result<byte[]>? myBallot = null;
        if (Voters.Last() == this)
        {
            var ballotAsByteArrays = _ballotsAsByteArrays.Peek();
            myBallot = decryptedBallots
                .FirstOrDefault(b => b.Value.AsSpan()
                    .SequenceEqual(ballotAsByteArrays.FastConcat(_randomStrings.Peek())));
        }
        else
        {
            var ballotAsByteArrays = _ballotsAsByteArrays.Pop();
            myBallot = decryptedBallots
                .FirstOrDefault(b => b.Value.AsSpan()
                    .SequenceEqual(ballotAsByteArrays));
        }


        if (myBallot is null)
        {
            return Result.Fail(new Error($"The ballot of voter {FullName} was not found."));
        }

        foreach (var voter in Voters)
        {
            if (voter == this || Voters.Last() == this)
            {
                continue;
            }

            voter.RemoveLastBallot();
        }

        ballots = _randomProvider.Shuffle(decryptedBallots.Select(b => b.Value)).ToList();

        var signedBallots = _signatureProvider.Sign(_transformer.Transform(ballots), _signaturePrivateKey);

        return Result.Ok(new SignedData<IReadOnlyList<byte[]>>(ballots.ToList(), signedBallots));
    }

    public Result<VotingResults> CompleteVoting(SignedData<IReadOnlyList<byte[]>> ballots)
    {
        var signer = Voters.Last();

        var signatureIsAuthentic = _signatureProvider.Verify(_transformer.Transform(ballots.Data), ballots.Signature, signer.SignaturePublicKey);
        if (!signatureIsAuthentic)
        {
            return Result.Fail($"Signature of voter {signer.FullName} is not authentic.");
        }

        var randomString = _randomStrings.Pop();
        var myBallot = ballots.Data
            .FirstOrDefault(b => b.AsSpan().Slice(b.Length - PaddingConstants.PaddingSize, PaddingConstants.PaddingSize)
                .SequenceEqual(randomString));
        if (myBallot is null)
        {
            return Result.Fail(new Error($"The ballot of voter {FullName} was not found."));
        }

        var ballotAsByteArrays = _ballotsAsByteArrays.Pop();
        if (!myBallot.AsSpan().Slice(0, myBallot.Length - PaddingConstants.PaddingSize).SequenceEqual(ballotAsByteArrays))
        {
            return Result.Fail(new Error($"The ballot of voter {FullName} was changed."));
        }

        var results = new VotingResults();
        foreach (var candidate in _candidates)
        {
            results.CandidatesResults.Add(candidate.Id, new(candidate));
        }
        foreach (var ballot in ballots.Data)
        {
            var ballotWithoutPadding = _transformer.ReverseTransform<Ballot>(ballot.AsSpan().Slice(0, myBallot.Length - PaddingConstants.PaddingSize).ToArray());
            if (ballotWithoutPadding is null )
            {
                return Result.Fail("Cannot retrieve a ballot.");
            }

            results.Ballots.Add( ballotWithoutPadding );
            var candidateWasFound = results.CandidatesResults.TryGetValue(ballotWithoutPadding.CandidateId, out var candidate);
            if (!candidateWasFound)
            {
                return Result.Fail("Candidate was not found.");
            }

            candidate!.Votes++;
        }

        return results;
    }
}
