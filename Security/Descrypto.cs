using System.Security.Cryptography;
using System.Text;

namespace FacinorasContaminadospeloodio.Security;

public class Descrypto : SecurityKeys
{
    public async Task<string> Decrypt(string cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            SecurityKeys keys = new SecurityKeys();
            
            aesAlg.Key = keys.key;
            aesAlg.IV = keys.iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return await srDecrypt.ReadToEndAsync();
                    }
                }
            }
        }
    }
}