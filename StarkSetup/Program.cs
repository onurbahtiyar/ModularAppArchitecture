using Enigma;
using Newtonsoft.Json.Linq;
using System;
using System.Data.SqlClient;
using System.Security.Cryptography;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WindowHeight = 40;

        string[] art = new string[]
        {
            "                    .^!7?JY555555YJ?7!^.                    ",
            "              .:!J5GGBBGGGGGGGGGGGGGGBBGG5J!:.              ",
            "          .^?5GGGGGGGBBGGGGGGGGGGGGGGBBGGGGGGG5?^.          ",
            "         ~GGGGGGGGP5YJPGGGGGGGGGGGGGGPJY5PGGGGGGGG~         ",
            "        .GGGGPYJ7~^^::YGGGGGGGGGGGGGGY::^^~7JYPGGGG.        ",
            "        JGY7~^^:::^^^:?GGGGGGGGGGGGGG?:^^^:::^^~7YGJ        ",
            "       .GG!::^^^^^^^^:!GGGGGGGGGGGGGG!:^^^^^^^^::!GG.       ",
            "       7GG?:^^^^^^^^^^~PGGGGGGGGGGGGP~^^^^^^^^^^:?GG7       ",
            "       PGGJ^^^^^^^^^^^^5GGGGGGGGGGGG5^^^^^^^^^^^^JGGP       ",
            "      .GGGJ^^^^^^^^^^^:?GGGGGGGGGGGG?:^^^^^^^^^^^JGGG.      ",
            "      ~GGGJ~^^^^^^^^^^^~GGGGGGGGGGGG~^^^^^^^^^^^^JGGG~      ",
            "      7GGGY~^^^^^^^^^^^^JGGGGGGGGGGJ^^^^^^^^^^^^~YGGG7      ",
            "      ?GGGY~^^^^^^^^^^^^^~!!!!!!!!~^^^^^^^^^^^^^~YGGG?      ",
            "      ?GGG7^^^^^^^^^^^^^^^::::::::^^^^^^^^^^^^^^^7GGG?      ",
            "      7B5?^^!~^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^~!^^?5B7      ",
            "      ~B?.^7.:^~~~~~^^^^^^^^^^^^^^^^^^^^~~~~~^:.7^.?B~      ",
            "      .G?:^~~.   ..::^^^!^^^^^^^^^^!^^^::..   .~~^:?G.      ",
            "       5Y~:^^~~^.......:!^^^^^^^^^^!:.......^~^^^:~55       ",
            "       ~BP7:^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^:7PB~       ",
            "        PGGY^:^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^:^YGGP        ",
            "        ^GGB#7^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^7#BGG^        ",
            "         YGGB@J^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^J@BGGY         ",
            "         .GGG#@5^^^^^^^^^^^^^^^^^^^^^^^^^^^^5@#GGG.         ",
            "          ~GGG#@5^~^^^^^^^^^^^^^^^^^^^^^^~^5@#GGG~          ",
            "           JGGG&@?^~^^^^^^^^^^^^^^^^^^^^~^?@&GGGJ           ",
            "            5GGG&B:^~^::::::::::::::::^~^:B&GGG5            ",
            "            .PGGGJ!^^YP55555555555555PY^^!JGGGP.            ",
            "             :GGY:^7##5JJJJJJJJJJJJJJ5##7^:YGG:             ",
            "              ^GY^:^~:^^:::^^^^^^:::^^:~^:^YG^              ",
            "               :YP?^^~7J??J5GPPG5J??J7~^^?PY:               ",
            "                 :5PPGGBBGGGGGGGGGGBBGGPP5:                 ",
            "                   ^5GGGGGGGGGGGGGGGGGG5^                   ",
            "                     ..................                     "
        };

        foreach (string line in art)
        {
            Console.WriteLine(line);
            Thread.Sleep(10);
        }

        Console.ResetColor();
        Console.WriteLine();

        Thread.Sleep(500);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("This application was developed by ");

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Onur Bahtiyar.");

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("GitHub: ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("https://github.com/onurbahtiyar");

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("LinkedIn: ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("https://www.linkedin.com/in/onur-bahtiyar\n");

        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine("-----------------------------------------------------");

        Console.ResetColor();
        Console.WriteLine();

        string connectionString;
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Please enter the ConnectionString: ");
            Console.ResetColor();
            connectionString = Console.ReadLine();

            if (TestConnectionString(connectionString))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Connection successful!");
                Console.ResetColor();
                break;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Connection failed. Please enter a valid ConnectionString.");
                Console.ResetColor();
            }
        }

        string aesKey, aesIV;
        bool isAesGenerated = false;
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Do you have an AES Key and IV? (yes/no): ");
            Console.ResetColor();
            string hasAesKey = Console.ReadLine()?.Trim().ToLower();

            if (hasAesKey == "yes")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Please enter the AES Key: ");
                Console.ResetColor();
                aesKey = Console.ReadLine();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Please enter the AES IV: ");
                Console.ResetColor();
                aesIV = Console.ReadLine();

                if (IsValidAesKey(aesKey) && IsValidAesIV(aesIV))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("AES Key and IV are valid.");
                    Console.ResetColor();
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid AES Key or IV. Please enter valid values.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Generating new AES Key and IV...");
                Console.ResetColor();

                using (Aes aes = Aes.Create())
                {
                    aesKey = Convert.ToBase64String(aes.Key);
                    aesIV = Convert.ToBase64String(aes.IV);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("AES Key: " + aesKey);
                Console.WriteLine("AES IV: " + aesIV);
                Console.ResetColor();

                isAesGenerated = true;
                break;
            }
        }

        string tokenKey;
        bool isTokenGenerated = false;
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Do you have a Token Key (512 Bit)? (yes/no): ");
            Console.ResetColor();
            string hasTokenKey = Console.ReadLine()?.Trim().ToLower();

            if (hasTokenKey == "yes")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Please enter the Token Key: ");
                Console.ResetColor();
                tokenKey = Console.ReadLine();

                if (IsValidTokenKey(tokenKey))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Token Key is valid.");
                    Console.ResetColor();
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid Token Key. Please enter a valid Token Key. The key must be at least 64 bytes (88 Base64 characters) long.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Generating a new Token Key...");
                Console.ResetColor();

                using (var rng = new RNGCryptoServiceProvider())
                {
                    byte[] tokenKeyBytes = new byte[64];
                    rng.GetBytes(tokenKeyBytes);
                    tokenKey = Convert.ToBase64String(tokenKeyBytes);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Token Key: " + tokenKey);
                Console.ResetColor();

                isTokenGenerated = true;
                break;
            }
        }

        if (isAesGenerated || isTokenGenerated)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n[!] Important: Store these keys securely. Do not lose them!\n");
            Console.ResetColor();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Please enter the name of your project: ");
        Console.ResetColor();
        string projectName = Console.ReadLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n-----------------------------------");
        Console.WriteLine("Summary:");
        Console.WriteLine("ConnectionString: " + connectionString);
        Console.WriteLine("AES Key: " + aesKey);
        Console.WriteLine("AES IV: " + aesIV);
        Console.WriteLine("Token Key: " + tokenKey);
        Console.WriteLine("Project Name: " + projectName);
        Console.WriteLine("-----------------------------------");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nSetup completed successfully!");
        Console.ResetColor();

        UpdateAppSettings(connectionString, aesKey, aesIV, projectName, tokenKey);
    }

    private static bool TestConnectionString(string connectionString)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidTokenKey(string tokenKey)
    {
        if (string.IsNullOrEmpty(tokenKey))
            return false;

        try
        {
            byte[] keyBytes = Convert.FromBase64String(tokenKey);
            return keyBytes.Length >= 64;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool IsValidAesKey(string base64Key)
    {
        try
        {
            byte[] keyBytes = Convert.FromBase64String(base64Key);
            return keyBytes.Length == 16 || keyBytes.Length == 24 || keyBytes.Length == 32;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidAesIV(string base64IV)
    {
        try
        {
            byte[] ivBytes = Convert.FromBase64String(base64IV);
            return ivBytes.Length == 16;
        }
        catch
        {
            return false;
        }
    }

    private static void UpdateAppSettings(string connectionString, string aesKey, string aesIV, string projectName, string tokenKey)
    {
        string webApiPath = GetWebApiAppSettingsPath();

        if (!File.Exists(webApiPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"appsettings.json file not found: {webApiPath}");
            Console.ResetColor();
            return;
        }

        var aesKeyBytes = Convert.FromBase64String(aesKey);
        var aesIVBytes = Convert.FromBase64String(aesIV);
        KeyCase.Instance.SetAesKeys(aesKeyBytes, aesIVBytes);

        string encryptedConnectionString = EncryptData(connectionString);
        string encryptedTokenKey = EncryptData(tokenKey);

        var json = File.ReadAllText(webApiPath);
        var jObject = JObject.Parse(json);

        var connectionStringsSection = jObject["ConnectionStrings"];
        if (connectionStringsSection != null)
        {
            connectionStringsSection["DatabaseConnection"] = encryptedConnectionString;
        }

        var aesSettingsSection = jObject["AesSettings"];
        if (aesSettingsSection != null)
        {
            aesSettingsSection["Key"] = aesKey;
            aesSettingsSection["Vektor"] = aesIV;
        }

        var jwtSettingsSection = jObject["JwtSettings"];
        if (jwtSettingsSection != null)
        {
            jwtSettingsSection["SecurityKey"] = encryptedTokenKey;
        }

        var projectSettingsSection = jObject["ProjectSettings"];
        if (projectSettingsSection != null)
        {
            projectSettingsSection["ProjectName"] = projectName;
        }
        else
        {
            jObject["ProjectSettings"] = new JObject
        {
            { "ProjectName", projectName },
            { "ProjectVersion", "1.0.0" }
        };
        }

        File.WriteAllText(webApiPath, jObject.ToString());

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nappsettings.json file updated successfully!");
        Console.ResetColor();
    }

    private static string EncryptData(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = KeyCase.Instance.Key;
            aesAlg.IV = KeyCase.Instance.Vektor;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new System.IO.MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }

    private static string GetWebApiAppSettingsPath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        var solutionDirectory = Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName;

        var webApiPath = Path.Combine(solutionDirectory, "WebApi", "appsettings.json");

        return webApiPath;
    }
}