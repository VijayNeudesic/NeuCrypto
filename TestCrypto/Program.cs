

// See https://aka.ms/new-console-template for more information
using NeuCrypto;

Console.WriteLine("Hello, World!");

CryptoProcess cryptoProcess = new CryptoProcess();


cryptoProcess.InitAll(@"./", false);
cryptoProcess.BulkEncryptAccessDBTable(@"F:\dev\dop\test\aca.accdb", "BA_ACA_ALL", "SSN|s,Firstname|s", "ID|i", "");



/*
cryptoProcess.InitRSA("DOPCrypto");

//stress testing the encryption/decryption process
DateTime start = DateTime.Now;  
for(int i = 0; i < 10000; i++)
{
    string originalText = $"Hello{i}, this is some{i + 1} sensitive information{i + 2}.";

    // Encrypt the text
    string encryptedData = cryptoProcess.EncryptTextRSA(originalText);

    // Decrypt the text
    string decryptedTxt = cryptoProcess.DecryptTextRSA(encryptedData);
}

TimeSpan timeSpan = DateTime.Now - start;

Console.WriteLine($"Press any key to exit RSA...{timeSpan.TotalSeconds} seconds.");


//stress testing the encryption/decryption process
start = DateTime.Now;

string key = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF"; // 64 characters (256 bits)
string iv = "0123456789ABCDEF0123456789ABCDEF"; // 32 characters (128 bits)

cryptoProcess.InitAES(key, iv); 

for(int i = 0; i < 10000; i++)
{
    string originalText = $"Hello{i}, this is some{i + 1} sensitive information{i + 2}.";

    // Encrypt the text
    string encryptedText = cryptoProcess.EncryptTextAES(originalText);

    // Decrypt the text
    string decryptedText = cryptoProcess.DecryptTextAES(encryptedText);
}


timeSpan = DateTime.Now - start;

Console.WriteLine($"Press any key to exit AES...{timeSpan.TotalSeconds} seconds.");*/

