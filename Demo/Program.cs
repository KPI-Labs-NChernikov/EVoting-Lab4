using Algorithms.AESWithRSA;
using Algorithms.Common;
using Algorithms.ElGamal;
using Algorithms.RSA;
using Demo;
using Modelling.CustomTransformers;
using Modelling.Models;
using System.Security.Cryptography;
using System.Text;

var objectToByteArrayTransformer = new ObjectToByteArrayTransformer();
objectToByteArrayTransformer.TypeTransformers.Add(new GuidTransformer());
objectToByteArrayTransformer.TypeTransformers.Add(new ModellingTransformer());

var encryptionProvider = new ComplexEncryptionService();
var encryptionKeyGenerator = new RSAKeysGenerator();

var signatureProvider = new ElGamalSignatureProvider();
var signatureKeyGenerator = new ElGamalKeysGenerator();

using var rng = RandomNumberGenerator.Create();
var paddingProvider = new PaddingProvider(rng);
var randomProvider = new RandomProvider();

var dataFactory = new DemoDataFactory(encryptionProvider, encryptionKeyGenerator, signatureProvider, signatureKeyGenerator, objectToByteArrayTransformer, paddingProvider, randomProvider);
var candidates = dataFactory.CreateCandidates();
var voters = dataFactory.CreateVoters(candidates);

var printer = new ModellingPrinter();
printer.PrintUsualVoting(voters, dataFactory.CreateVotersWithCandidateIds(voters));
printer.PrintVotingWithExit(voters, dataFactory.CreateVotersWithCandidateIds(voters));