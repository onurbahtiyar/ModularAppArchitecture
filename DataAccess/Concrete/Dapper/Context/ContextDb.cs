using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace DataAccess.Concrete.Dapper.Context
{
    public static class ContextDb
    {
        private static IConfiguration _configuration;

        public static void Configure(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string ConnectionStringDefault
        {
            get
            {
                if (_configuration == null)
                {
                    throw new InvalidOperationException("Configuration has not been initialized. Please call Configure() method before using ConnectionStringDefault.");
                }

                var processor = new Enigma.Processor();
                var encryptedConnectionString = _configuration["ConnectionStrings:DatabaseConnection"];

                using (Aes aes = Aes.Create())
                {
                    encryptedConnectionString = processor.DecryptorSymmetric(encryptedConnectionString, aes);
                }

                return encryptedConnectionString;
            }
        }
    }
}