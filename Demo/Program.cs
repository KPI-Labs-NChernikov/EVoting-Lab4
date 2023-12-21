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
    Console.WriteLine(signature.Length);
    var verified = signer.Verify(data, signature, keys.PublicKey);
    if (!verified)
    {
        Console.ForegroundColor = ConsoleColor.Red;
    }
    Console.WriteLine(verified);
}
