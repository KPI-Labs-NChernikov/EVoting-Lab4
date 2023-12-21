using Algorithms.ElGamal;
using System.Security.Cryptography;

var elGamalGenerator = new ElGamalKeysGenerator();
using var rng = RandomNumberGenerator.Create();
var signer = new ElGamalSignatureProvider();
for (var i = 0; i < 100; i++)
{
    var keys = elGamalGenerator.Generate();
    var data = new byte[] { 1, 2, 3, 4 };
    var signature = signer.Sign(data, keys.PrivateKey);
    var verified = signer.Verify(data, signature, keys.PublicKey);
    Console.WriteLine(verified);
}
