using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NeuCrypto
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    public class SSLCert
    {
        public string LastError { get; set; }
        public X509Certificate2 x509cert = null;
        public SSLCert()
        {
            LastError = "";
        }
        public SSLCert(string szSubjectName)
        {
            LastError = "";
            x509cert = GetCertFromMyStore(szSubjectName);
        }

        public X509Certificate2 GetCertFromMyStore(string szSubjectName)
        {
            X509Certificate2 certificate = null; 
            try
            {
                // Open the Current User's Personal (My) certificate store
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

                // Open the store for reading (ReadOnly)
                store.Open(OpenFlags.ReadOnly);

                // Find the certificate based on the subject (distinguished name)
                X509Certificate2Collection certificates = store.Certificates.Find(
                    X509FindType.FindBySubjectName,
                    szSubjectName,
                    validOnly: false
                );

                // Check if the certificate with the specified subject exists
                if (certificates.Count > 0)
                {
                    certificate = certificates[0];
                    // Use the certificate as needed (e.g., for encryption/decryption)

                    Console.WriteLine("Certificate Subject: " + certificate.Subject);
                    Console.WriteLine("Certificate Thumbprint: " + certificate.Thumbprint);
                    // You can access other properties of the certificate as needed
                }
                else
                {
                    Console.WriteLine("Certificate with subject '" + szSubjectName + "' not found.");
                    return null;
                }

                // Close the certificate store
                store.Close();

                return certificate;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return null;
            }
        }
        
    }

}
