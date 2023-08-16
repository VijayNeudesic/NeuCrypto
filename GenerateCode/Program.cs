using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeuCrypto
{
    public class Program
    {
        static void Main(string[] args)
        {
            NeuCrypto.CryptoProcess process = new NeuCrypto.CryptoProcess();
            string code = process.GenerateAccessCode();
            //NeuCrypto.Encryptor encryptor = new NeuCrypto.Encryptor();
            //string code = encryptor.GenerateAccessCode();
            Console.WriteLine($"Generated code for today: {code}");
        }
    }
}






