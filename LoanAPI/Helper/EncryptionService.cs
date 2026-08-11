using System.Security.Cryptography;
using System.Text;

namespace LoanAPI.Helper
{
    public class EncryptionService
    {
        private readonly string _masterSecret;

        public EncryptionService(IConfiguration config)
        {
            _masterSecret =
                config["Encryption:MasterKey"]?? throw new InvalidOperationException("Encryption master key is missing.");
        }

        private byte[] DeriveKey(int userId)
        {
            using var sha256 = SHA256.Create();

            string input = $"{_masterSecret}_{userId}";

            return sha256.ComputeHash(
                Encoding.UTF8.GetBytes(input)); 
        }

        public string Encrypt(string? plainText, int userId)
        {
            if (string.IsNullOrEmpty(plainText))
                return "";

            byte[] key = DeriveKey(userId);

            byte[] nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            byte[] plainBytes =Encoding.UTF8.GetBytes(plainText);

            byte[] cipherBytes =new byte[plainBytes.Length];

            byte[] tag = new byte[16];

            using var aes = new AesGcm(key, tag.Length);

            aes.Encrypt(nonce,plainBytes,cipherBytes,tag);

            byte[] combined =new byte[nonce.Length +tag.Length +cipherBytes.Length];

            Buffer.BlockCopy(nonce,0,combined,0,nonce.Length);

            Buffer.BlockCopy(tag,0,combined,nonce.Length,tag.Length);

            Buffer.BlockCopy(cipherBytes,0,combined,nonce.Length + tag.Length,cipherBytes.Length);

            return Convert.ToBase64String(combined);
        }

        public string Decrypt(string? cipherText, int userId)
        {
            if (string.IsNullOrEmpty(cipherText))
                return "";

            try
            {
                byte[] combined =
                    Convert.FromBase64String(cipherText);

                if (combined.Length < 28)
                    return "";

                byte[] key = DeriveKey(userId);

                byte[] nonce = new byte[12];

                byte[] tag = new byte[16];

                int cipherLength =combined.Length -nonce.Length -tag.Length;

                if (cipherLength < 0)
                    return "";

                byte[] cipherBytes =new byte[cipherLength];

                Buffer.BlockCopy(combined,0,nonce,0,nonce.Length);

                // Extract authentication tag
                Buffer.BlockCopy(combined,nonce.Length,tag,0,tag.Length);

                // Extract ciphertext
                Buffer.BlockCopy(combined,nonce.Length + tag.Length,cipherBytes,0,cipherBytes.Length);

                byte[] plainBytes =new byte[cipherBytes.Length];

                using var aes =new AesGcm(key, tag.Length);

                aes.Decrypt(nonce,cipherBytes,tag,plainBytes);

                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (FormatException)
            {
                return "";
            }
            catch (CryptographicException)
            {
                throw new CryptographicException(
                     "The encrypted data has been modified or is invalid.");
            }
        }
    }
}




// using System.Security.Cryptography;
// using System.Text;

// namespace LoanAPI.Helper
// {
//     //     SHA-256(MasterSecret + "_" + UserId)
//     // SHA-256 always produces exactly 32 bytes, which is exactly what AES-256 needs.
//     // per-user keys anywhere - the key is regenerated on the fly from the master secret
//     public class EncryptionService
//     {
//         private readonly string _masterSecret;

//         public EncryptionService(IConfiguration config)
//         {
//             _masterSecret = config["Encryption:MasterKey"]?? throw new InvalidOperationException("Encryption:MasterKey is missing from appsettings.json. See the Encryption section.");
//         }

//         private byte[] DeriveKey(int userId)
//         {
//             var input = $"{_masterSecret}_{userId}";
//             using var sha256 = SHA256.Create();
//             return sha256.ComputeHash(Encoding.UTF8.GetBytes(input)); //  32 bytes = AES-256
//         }

//         public string Encrypt(string? plainText, int userId)
//         {
//             if (string.IsNullOrEmpty(plainText))
//                 return "";

//             using var aes = Aes.Create();
//             aes.Key = DeriveKey(userId);
//             aes.GenerateIV(); // a fresh random IV every time // Initialization Vector

//             using var encryptor = aes.CreateEncryptor();
//             var plainBytes = Encoding.UTF8.GetBytes(plainText);
//             var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

//             var combined = new byte[aes.IV.Length + cipherBytes.Length];
//             Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
//             Buffer.BlockCopy(cipherBytes, 0, combined, aes.IV.Length, cipherBytes.Length);

//             return Convert.ToBase64String(combined);
//         }

//         public string Decrypt(string? cipherText, int userId)
//         {
//             if (string.IsNullOrEmpty(cipherText))
//                 return "";
  
//             try
//             {
//                 var combined = Convert.FromBase64String(cipherText);

//                 using var aes = Aes.Create();
//                 aes.Key = DeriveKey(userId);

//                 var iv = new byte[16];
//                 var cipherBytes = new byte[combined.Length - iv.Length];
//                 Buffer.BlockCopy(combined, 0, iv, 0, iv.Length);
//                 Buffer.BlockCopy(combined, iv.Length, cipherBytes, 0, cipherBytes.Length);
//                 aes.IV = iv;

//                 using var decryptor = aes.CreateDecryptor();
//                 var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
//                 return Encoding.UTF8.GetString(plainBytes);
//             }
//             catch
//             {
//                 return cipherText;
//             }
//         }
//     }
// }