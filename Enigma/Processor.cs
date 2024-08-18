using System.Security.Cryptography;

namespace Enigma;

public class Processor
{
    public string EncryptorSimple(string data)
    {
        byte[] dataArray = System.Text.Encoding.UTF8.GetBytes(data);
        return Convert.ToBase64String(dataArray);
    }

    public string DecryptorSimple(string data)
    {
        byte[] encryptedArray = Convert.FromBase64String(data);
        return System.Text.Encoding.UTF8.GetString(encryptedArray);
    }

    public string EncryptorHash<T>(string data) where T : HashAlgorithm
    {
        byte[] dataArray = System.Text.Encoding.UTF8.GetBytes(data);
        T algorithm = Activator.CreateInstance<T>();
        byte[] encryptedArray = algorithm.ComputeHash(dataArray);
        return Convert.ToBase64String(encryptedArray);
    }

    public string EncryptorSymmetric<T>(string data, T algorithmInstance) where T : SymmetricAlgorithm
    {
        ICryptoTransform transformer = algorithmInstance.CreateEncryptor(KeyCase.Instance.Key, KeyCase.Instance.Vektor);
        string returnValue = string.Empty;
        using (MemoryStream ms = new MemoryStream())
        {
            using (CryptoStream cs = new CryptoStream(ms, transformer, CryptoStreamMode.Write))
            {
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(data);
                }
            }
            returnValue = Convert.ToBase64String(ms.ToArray());
        }
        return returnValue;
    }

    public string DecryptorSymmetric<T>(string data, T algoritmInstance) where T : SymmetricAlgorithm
    {
        byte[] encryptedArray = Convert.FromBase64String(data);
        ICryptoTransform transformer = algoritmInstance.CreateDecryptor(KeyCase.Instance.Key, KeyCase.Instance.Vektor);
        string returnValue = string.Empty;
        using (MemoryStream ms = new MemoryStream(encryptedArray))
        {
            using (CryptoStream cs = new CryptoStream(ms, transformer, CryptoStreamMode.Read))
            {
                using (StreamReader sr = new StreamReader(cs))
                {
                    returnValue = sr.ReadToEnd();
                }
            }
        }
        return returnValue;
    }

}