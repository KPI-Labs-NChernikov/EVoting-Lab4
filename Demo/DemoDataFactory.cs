using Algorithms.Abstractions;
using Modelling.Models;
using Org.BouncyCastle.Crypto;

namespace Demo;
public sealed class DemoDataFactory
{
    private readonly IComplexEncryptionProvider<AsymmetricKeyParameter> _encryptionProvider;
    private readonly IKeyGenerator<AsymmetricKeyParameter> _encryptionKeyGenerator;

    private readonly ISignatureProvider<AsymmetricKeyParameter> _signatureProvider;
    private readonly IKeyGenerator<AsymmetricKeyParameter> _signatureKeyGenerator;

    private readonly IObjectToByteArrayTransformer _transformer;

    private readonly IPaddingProvider _paddingProvider;

    private readonly IRandomProvider _randomProvider;

    public DemoDataFactory(IComplexEncryptionProvider<AsymmetricKeyParameter> encryptionProvider, IKeyGenerator<AsymmetricKeyParameter> encryptionKeyGenerator, ISignatureProvider<AsymmetricKeyParameter> signatureProvider, IKeyGenerator<AsymmetricKeyParameter> keysignatureGenerator, IObjectToByteArrayTransformer objectToByteArrayTransformer, IPaddingProvider paddingProvider, IRandomProvider randomProvider)
    {
        _encryptionProvider = encryptionProvider;
        _encryptionKeyGenerator = encryptionKeyGenerator;
        _signatureProvider = signatureProvider;
        _signatureKeyGenerator = keysignatureGenerator;
        _transformer = objectToByteArrayTransformer;
        _paddingProvider = paddingProvider;
        _randomProvider = randomProvider;
    }

    public IReadOnlyList<Candidate> CreateCandidates()
    {
        return new List<Candidate>
        {
            new (1, "Ishaan Allison"),
            new (2, "Oliver Mendez")
        };
    }

    public IReadOnlyList<Voter> CreateVoters(IEnumerable<Candidate> candidates)
    {
        return new List<Voter>
        {
            new ("Jasper Lambert", candidates, _signatureKeyGenerator.Generate(), _encryptionKeyGenerator.Generate(), _signatureProvider, _encryptionProvider, _transformer, _paddingProvider, _randomProvider),
            new ("Jonty Levine", candidates, _signatureKeyGenerator.Generate(), _encryptionKeyGenerator.Generate(), _signatureProvider, _encryptionProvider, _transformer, _paddingProvider, _randomProvider),
            new ("Nathaniel Middleton", candidates, _signatureKeyGenerator.Generate(), _encryptionKeyGenerator.Generate(), _signatureProvider, _encryptionProvider, _transformer, _paddingProvider, _randomProvider),
            new ("Nathan Bass", candidates, _signatureKeyGenerator.Generate(), _encryptionKeyGenerator.Generate(), _signatureProvider, _encryptionProvider, _transformer, _paddingProvider, _randomProvider),
        };
    }

    public Dictionary<Voter, int> CreateVotersWithCandidateIds(IReadOnlyList<Voter> voters)
    {
        var dictionary = new Dictionary<Voter, int>();
        for (var i = 0; i < voters.Count; i++)
        {
            var candidateId = (i % 7 + 1) switch
            {
                1 => 1,
                2 => 1,

                3 => 2,
                4 => 1,
                //5 => 3,
                //6 => 3,
                //7 => 3,

                _ => throw new InvalidOperationException("Negative and zero voters' ids are not supported in this method.")
            };
            dictionary.Add(voters[i], candidateId);
        }
        return dictionary;
    }
}
