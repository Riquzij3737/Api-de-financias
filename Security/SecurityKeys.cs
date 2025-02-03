using System;
using System.Text;

namespace FacinorasContaminadospeloodio.Security
{
    public class SecurityKeys
    {
        public byte[] key { get; set; }
        public byte[] iv { get; set; }

        public SecurityKeys()
        {
            // Lê as variáveis de ambiente
            string keyEnv = Environment.GetEnvironmentVariable("CRYPKEY");
            string ivEnv = Environment.GetEnvironmentVariable("CRIPIV");

            // Garante que a chave tenha 32 bytes e o IV tenha 16 bytes
            if (keyEnv.Length < 32)
                throw new Exception("A chave CRYPKEY precisa ter pelo menos 32 caracteres.");

            if (ivEnv.Length < 16)
                throw new Exception("O vetor de inicialização CRIPIV precisa ter pelo menos 16 caracteres.");

            // Converte as variáveis de ambiente para byte arrays
            key = Encoding.UTF8.GetBytes(keyEnv.Substring(0, 32));  // Truncar para 32 bytes
            iv = Encoding.UTF8.GetBytes(ivEnv.Substring(0, 16));    // Truncar para 16 bytes
        }
    }
}