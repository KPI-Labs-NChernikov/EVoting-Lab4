using Algorithms.AESWithRSA;
using Algorithms.ElGamal;
using Algorithms.RSA;
using System.Security.Cryptography;
using System.Text;

var rsaKeysGenerator = new RSAKeysGenerator();
var keys = rsaKeysGenerator.Generate();
var complexEncryptionService = new ComplexEncryptionService();
var data = Encoding.UTF8.GetBytes("Hello world qe3i2sadioo3340923^^*%^");
var encryptedData = complexEncryptionService.Encrypt(data, keys.PublicKey);
var decryptedData = complexEncryptionService.Decrypt(encryptedData, keys.PrivateKey);
Console.WriteLine();
//var elGamalGenerator = new ElGamalKeysGenerator();
//using var rng = RandomNumberGenerator.Create();
//var signer = new ElGamalSignatureProvider();
//for (var i = 0; i < 100; i++)
//{
//    var keys = elGamalGenerator.Generate();
//    var data = Encoding.UTF8.GetBytes("Hello world qe3i2sadioo3340923^^*%^"); //new byte[] { 1, 2, 3, 4 };
//    var signature = signer.Sign(data, keys.PrivateKey);
//    Console.WriteLine(signature.Length);
//    var verified = signer.Verify(data, signature, keys.PublicKey);
//    if (!verified)
//    {
//        Console.ForegroundColor = ConsoleColor.Red;
//    }
//    Console.WriteLine(verified);
//}
