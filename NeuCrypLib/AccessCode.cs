using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeuCrypto
{
    public class AccessCode
    {
        public static string GenerateAccessCode()
        {
            // Replace this secret key with your own
            string secretKey = "SECRET-KEY";

            // Get today's date
            DateTime currentDate = DateTime.Now.Date;

            // Combine secret key and date
            string combinedString = secretKey + currentDate.ToString("yyyyMMdd");

            string code = "";

            // Compute a hash of the combined string
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedString));

                // Take the first 4 bytes of the hash and convert them to an integer
                int dayCode = BitConverter.ToInt32(hashBytes, 0) % 10000;

                if(dayCode < 0)
                    dayCode = dayCode * -1;

                // Format the code as a 4-digit string
                code = dayCode.ToString("D4");
            }

            return code;
        }
    }
}
