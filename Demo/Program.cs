using Algorithms.AESWithRSA;
using Algorithms.Common;
using Algorithms.ElGamal;
using Algorithms.RSA;
using Modelling.CustomTransformers;
using Modelling.Models;
using System.Security.Cryptography;
using System.Text;

var objectToByteArrayTransformer = new ObjectToByteArrayTransformer();
objectToByteArrayTransformer.TypeTransformers.Add(new GuidTransformer());
objectToByteArrayTransformer.TypeTransformers.Add(new ModellingTransformer());

var rsaKeysGenerator = new RSAKeysGenerator();
var keys = rsaKeysGenerator.Generate();
var complexEncryptionService = new ComplexEncryptionService();
var data = Encoding.UTF8.GetBytes("Hello world qe3i2sadioo3340923^^*%^");
var encryptedData = complexEncryptionService.Encrypt(data, keys.PublicKey);
var decryptedData = complexEncryptionService.Decrypt(encryptedData, keys.PrivateKey);

var elGamalGenerator = new ElGamalKeysGenerator();
using var rng = RandomNumberGenerator.Create();
var signer = new ElGamalSignatureProvider();
var elGamalKeys = elGamalGenerator.Generate();
var elGamalData = encryptedData.EncryptedData; //new byte[] { 1, 2, 3, 4 };
var signature = signer.Sign(elGamalData, elGamalKeys.PrivateKey);
Console.WriteLine(signature.Length);
var verified = signer.Verify(elGamalData, signature, elGamalKeys.PublicKey);
Console.WriteLine(verified);
var signedData = new SignedData<byte[]>(elGamalData, signature);
var transformed = objectToByteArrayTransformer.Transform(signedData);
var untransformed = objectToByteArrayTransformer.ReverseTransform<SignedData<byte[]>>(transformed);
verified = signer.Verify(untransformed!.Data, untransformed.Signature, elGamalKeys.PublicKey);
Console.WriteLine(verified);


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
