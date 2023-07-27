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

        public int GetCertFromMyStore(string szSubjectName, bool bLocalMachine)
        {
            x509cert = null; 
            try
            {
                int iStoreLocation = (bLocalMachine) ? (int)StoreLocation.LocalMachine : (int)StoreLocation.CurrentUser;

                // Open the Current User's Personal (My) certificate store
                X509Store store = new X509Store(StoreName.My, (StoreLocation)iStoreLocation);

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
                    x509cert = certificates[0];
                }
                else
                {
                    LastError = "Certificate with subject name '" + szSubjectName + "' not found.";
                    return -1;
                }

                // Close the certificate store
                store.Close();

                return 0;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return -1;
            }
        }
        
    }

}
