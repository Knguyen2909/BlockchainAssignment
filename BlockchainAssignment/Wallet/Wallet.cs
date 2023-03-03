using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace BlockchainAssignment.Wallet
{
    /// <summary>
    /// Below is the code for generating key pairs and signatures, and validating signatures and private keys
    /// </summary>
    class Wallet
    {
        public string publicID; //public id viewable to all (created using private key)


        public Wallet(out string privateKey)
        {
            privateKey = string.Empty;
            
            byte[] pubKey;
            byte[] privKey;


            CngKeyCreationParameters keyCreationParameters = new CngKeyCreationParameters();
            keyCreationParameters.ExportPolicy = CngExportPolicies.AllowPlaintextExport;
            keyCreationParameters.KeyUsage = CngKeyUsages.Signing;

            CngKey key = CngKey.Create(CngAlgorithm.ECDsaP256, null, keyCreationParameters);

            byte[] KeyBlob = key.Export(CngKeyBlobFormat.EccPrivateBlob); 
            byte[] pubBlob = key.Export(CngKeyBlobFormat.EccPublicBlob);
            pubKey = KeyBlob.Skip(8).Take(KeyBlob.Length - 40).ToArray();
            privKey = KeyBlob.Skip(72).Take(KeyBlob.Length).ToArray();
            publicID = Convert.ToBase64String(pubKey);
            privateKey = Convert.ToBase64String(privKey); 
            }

        public static bool ValidatePrivateKey(String privateKey, String publicID)
        {
            string testHash = "0000abc1e11b8d37c1e1232a2ea6d290cddb0c678058c37aa766f813cbbb366e"; //just a random string to create a sig with

            if (privateKey.Length != 44 || publicID.Length != 88)
            {
                return false;
            }

            string sig = CreateSignature(publicID, privateKey, testHash);

            return ValidateSignature(publicID, testHash, sig);

        }


        /// <summary>
        /// Validates if a signiture is legitimate.
        /// publicID is the Id of the wallet making the transaction.
        /// datahash is the hash of transaction
        /// datasig is the hash created by the private key and datahash
        /// The datasig can be validated with the above parameters.
        /// </summary>
        /// <param name="publicID"></param>
        /// <param name="datahash"></param>
        /// <param name="datasig"></param>
        /// <returns></returns>
        public static bool ValidateSignature(string publicID, string datahash, string datasig)
        {
            if (publicID.Equals("Mine Rewards"))
            {
                publicID = "QfF3+9GgTxyGLvb+ScOAI6nJxBh8IyZbeD0r6BJBMyabZmyuP82yrSLKMq/F05OG0VZ4gg63uHFZUKzCu3wZuA==";
            }

            if (publicID.Length != 88 || datasig.Equals("null"))
            {
                return false;
            }
            CngKey key = createKey(publicID);

            if (key == null)
            {
                return false;
            }

            ECDsaCng dsa = new ECDsaCng(key);
            return dsa.VerifyData(Convert.FromBase64String(datahash), Convert.FromBase64String(datasig));

        }

        public static string CreateSignature(string publicID, string privateKey, string datahash) //need to have checks that publicID and privatekey are correct before using this method
        {
            CngKey key = createKey(publicID, privateKey);
            if (key == null)
            {
                return "null";
            }
            byte[] datahashByte = Convert.FromBase64String(datahash);

            ECDsaCng dsa = new ECDsaCng(key);
            byte[] byteSig = dsa.SignData(datahashByte);
            return Convert.ToBase64String(byteSig);
        }

        private static CngKey createKey(string publicID, string privateKey = "")
        {
            try
            {
                if (publicID.Equals("Mine Rewards") && privateKey.Equals(string.Empty))
                {
                    publicID = "QfF3+9GgTxyGLvb+ScOAI6nJxBh8IyZbeD0r6BJBMyabZmyuP82yrSLKMq/F05OG0VZ4gg63uHFZUKzCu3wZuA==";
                    privateKey = "mkT1Iu3YF4NSruHBptVytyDkNcxwemrkclndJH0+73o=";
                }
                CngKey key;
                byte[] keyByte = new byte[] { 69, 67, 83, 49, 32, 0, 0, 0 }; //first 8 bytes always same
                byte[] publicBytes = Convert.FromBase64String(publicID);
                byte[] keyByteCombine1 = new byte[72];
                keyByte.CopyTo(keyByteCombine1, 0);
                publicBytes.CopyTo(keyByteCombine1, keyByte.Length);

                if (!privateKey.Equals(string.Empty))
                {
                    keyByteCombine1[3] = 50; //must be set to 50 to be a private block
                    byte[] privateBytes = Convert.FromBase64String(privateKey);
                    byte[] keyByteCombine2 = new byte[104];
                    keyByteCombine1.CopyTo(keyByteCombine2, 0);
                    privateBytes.CopyTo(keyByteCombine2, keyByteCombine1.Length);

                    key = CngKey.Import(keyByteCombine2, CngKeyBlobFormat.EccPrivateBlob);
                    return key;
                }
                key = CngKey.Import(keyByteCombine1, CngKeyBlobFormat.EccPublicBlob);
                return key;
            }

            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
                return null;
            }
        }
    }
}