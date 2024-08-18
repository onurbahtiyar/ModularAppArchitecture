using Microsoft.Extensions.Configuration;
using System.IO;

namespace Enigma;

public sealed class KeyCase
{
    private KeyCase() { }

    private static readonly object lockObject = new object();
    private static KeyCase _instance;

    public static KeyCase Instance
    {
        get
        {
            lock (lockObject)
            {
                if (_instance == null)
                {
                    _instance = new KeyCase();
                }
            }
            return _instance;
        }
    }

    public byte[] Key { get; private set; }
    public byte[] Vektor { get; private set; }

    public void SetAesKeys(byte[] key, byte[] vektor)
    {
        Key = key;
        Vektor = vektor;
    }

    public void LoadFromConfiguration(IConfiguration configuration)
    {
        string keyBase64 = configuration["AesSettings:Key"];
        string vektorBase64 = configuration["AesSettings:Vektor"];

        Key = Convert.FromBase64String(keyBase64);
        Vektor = Convert.FromBase64String(vektorBase64);
    }
}
