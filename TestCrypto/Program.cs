

// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography.X509Certificates;
using NeuCrypto;

Console.WriteLine("Hello, World!");

CryptoProcess cryptoProcess = new CryptoProcess("DOPCrypto");

DateTime start = DateTime.Now;  
for(int i = 0; i < 1000; i++)
{
    // Replace with the text you want to encrypt
    string originalText = $"Hello{i}, this is some sensitive information.";

    // Encrypt the text
    byte[] encryptedData = cryptoProcess.RSAEncryptText(originalText);

    // Decrypt the text
    string decryptedText = cryptoProcess.RSADecryptText(encryptedData);
}

TimeSpan timeSpan = DateTime.Now - start;
Console.WriteLine($"Press any key to exit...{timeSpan.TotalSeconds} seconds.");