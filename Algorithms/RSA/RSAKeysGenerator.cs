using Algorithms.Abstractions;
using Algorithms.Common;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Algorithms.RSA;
public sealed class RSAKeysGenerator : IKeyGenerator<AsymmetricKeyParameter>
{
    public Keys<AsymmetricKeyParameter> Generate()
    {
        var internalKeyPairGenerator = new RsaKeyPairGenerator();
        internalKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), PublicConstants.RsaKeySize));
        var keyPair = internalKeyPairGenerator.GenerateKeyPair();
        var privateKey = (RsaPrivateCrtKeyParameters)keyPair.Private;
        var publicKey = (RsaKeyParameters)keyPair.Public;
        return new Keys<AsymmetricKeyParameter>(publicKey,privateKey);
    }
}
