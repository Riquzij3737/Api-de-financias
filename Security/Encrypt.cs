using System.Security.Cryptography;
using System.Text;
using FacinorasContaminadospeloodio.Security;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FacinorasContaminadospeloodio.Security;

public class Encrypt : SecurityKeys
{
    public async Task<string> EncryptTextAsync(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        await swEncrypt.WriteAsync(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }
    
}