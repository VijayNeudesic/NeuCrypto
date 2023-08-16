using NeuCrypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEncryption
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            CryptoProcess cryptoProcess = new CryptoProcess();

            cryptoProcess.InitAll(".\\");
            string sztext = cryptoProcess.EncryptString("Hello, World!");

            //cryptoProcess.BulkEncryptDBTable("VBANSAL01\\SQLEXPRESS", "TestBulkLoad", "BulkRecords", "column2", "column2", "");
            cryptoProcess.BulkDecryptDBTable("VBANSAL01\\SQLEXPRESS", "TestBulkLoad", "BulkRecords", "column2", "column2", "", "1234");

            //cryptoProcess.BulkEncryptDBTable("", @"F:\dev\dop\test\DOP_MASTER_BE.mdb", "01_Deacons", "NameOfSpouse", "PersonID", "");

            cryptoProcess.BulkEncryptDBTable("", @"F:\dev\dop\test\aca.accdb", "BA_ACA_ALL", "SSN,Firstname", "ID", "");
            cryptoProcess.BulkDecryptDBTable("", @"F:\dev\dop\test\aca.accdb", "BA_ACA_ALL", "SSN,Firstname", "ID", "", "1234");

            /* cryptoProcess.InitRSA("DOPCrypto");

             //stress testing the encryption/decryption process
             DateTime start = DateTime.Now;
             for (int i = 0; i < 10000; i++)
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

             for (int i = 0; i < 10000; i++)
             {
                 string originalText = $"Hello{i}, this is some{i + 1} sensitive information{i + 2}.";

                 // Encrypt the text
                 string encryptedText = cryptoProcess.EncryptTextAES(originalText);

                 // Decrypt the text
                 string decryptedText = cryptoProcess.DecryptTextAES(encryptedText);
             }


             timeSpan = DateTime.Now - start;

             Console.WriteLine($"Press any key to exit AES...{timeSpan.TotalSeconds} seconds.");*/
        }
    }
}
