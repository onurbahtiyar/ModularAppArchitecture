using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Enigma;
using Microsoft.Extensions.Configuration;

public class EntitySetupAutomation
{
    public static void ScaffoldAllTables(string projectDirectory, string connectionString, string dbContextClassName)
    {
        string entitiesProjectPath = Path.Combine(projectDirectory, "Entities");
        string outputPath = Path.Combine(entitiesProjectPath, "Concrete", "EntityFramework", "Entities");
        string contextDirPath = Path.Combine(entitiesProjectPath, "Concrete", "EntityFramework", "Context");
        string entitiesProjectFilePath = Path.Combine(entitiesProjectPath, "Entities.csproj");

        string command = $"dotnet ef dbcontext scaffold \"{connectionString}\" Microsoft.EntityFrameworkCore.SqlServer --project \"{entitiesProjectFilePath}\" --output-dir \"{outputPath}\" --context-dir \"{contextDirPath}\" --context \"{dbContextClassName}\" --data-annotations --force";
        string output = ExecuteCommand(command, true);

        EditScaffoldedFiles(outputPath);
        RemoveOnConfiguringMethod($"{contextDirPath}\\ContextDb.cs");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Entity skeletons generated for all tables.");
        Console.ResetColor();
    }

    public static void RemoveOnConfiguringMethod(string dbContextFilePath)
    {
        var textLines = File.ReadAllLines(dbContextFilePath).ToList();
        int startIndex = textLines.FindIndex(line => line.Contains("protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)"));
        if (startIndex != -1)
        {
            int endIndex = startIndex + 1;
            while (endIndex <= textLines.Count && !textLines[endIndex].Contains("=>"))
                endIndex++;

            textLines.RemoveRange(startIndex, endIndex - startIndex + 1);
            File.WriteAllLines(dbContextFilePath, textLines);
        }
    }

    private static void EditScaffoldedFiles(string outputPath)
    {
        var files = Directory.GetFiles(outputPath, "*.cs", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            string content = File.ReadAllText(file);

            if (!content.Contains("using Core.Entities;"))
            {
                content = "using Core.Entities;\n" + content;
            }

            content = System.Text.RegularExpressions.Regex.Replace(content,
                @"public partial class (\w+)\s*(?:\r?\n)?\s*{",
                "public class $1 : IEntity {");


            File.WriteAllText(file, content);
            Console.WriteLine($"{file} modified.");
        }
    }

    private static void GenerateDalAndRegistration(string projectDirectory, string entityName, string dbContextClassName)
    {
        GenerateDal(projectDirectory, entityName, dbContextClassName);
        AddRegistration(projectDirectory, entityName);
    }

    private static string ExecuteCommand(string command, bool waitForExit)
    {
        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();

        var outputBuilder = new StringBuilder();
        while (!process.StandardOutput.EndOfStream)
        {
            string line = process.StandardOutput.ReadLine();
            Console.WriteLine(line);
            outputBuilder.AppendLine(line);
        }

        if (waitForExit)
        {
            process.WaitForExit();
        }

        return outputBuilder.ToString();
    }

    private static void GenerateDal(string projectDirectory, string entityName, string dbContextClassName)
    {
        string dalInterfacePath = Path.Combine(projectDirectory, "DataAccess", "Abstract", $"I{entityName}Dal.cs");
        string dalConcretePath = Path.Combine(projectDirectory, "DataAccess", "Concrete", "EntityFramework", $"Ef{entityName}Dal.cs");

        string dalInterfaceCode = $@"
using Core.DataAccess;
using Entities.Concrete.EntityFramework.Entities;

namespace DataAccess.Abstract;
public interface I{entityName}Dal: IEntityRepository<{entityName}>
{{
}}
";
        File.WriteAllText(dalInterfacePath, dalInterfaceCode);

        string dalConcreteCode = $@"
using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete.EntityFramework.Context;
using Entities.Concrete.EntityFramework.Entities;

namespace DataAccess.Concrete.EntityFramework;
public class Ef{entityName}Dal: EfEntityRepositoryBase<{entityName}, {dbContextClassName}>, I{entityName}Dal
{{
    public Ef{entityName}Dal({dbContextClassName} context) : base(context)
    {{
    }}
}}
";
        File.WriteAllText(dalConcretePath, dalConcreteCode);
        Console.WriteLine($"{entityName} DAL classes created.");
    }

    private static void AddRegistration(string projectDirectory, string entityName)
    {
        string modulePath = Path.Combine(projectDirectory, "Business", "DependencyRepository", "Autofac", "AutofacBusinessModule.cs");

        string moduleContent = File.ReadAllText(modulePath);
        string dalName = $"Ef{entityName}Dal";
        string dalInterfaceName = $"I{entityName}Dal";

        string newRegistration = $"\n            builder.RegisterType<{dalName}>().As<{dalInterfaceName}>().InstancePerLifetimeScope();\n";

        int lastIndex = moduleContent.LastIndexOf("builder.RegisterType<");

        int endOfBlockIndex = moduleContent.IndexOf(";", lastIndex);

        moduleContent = moduleContent.Insert(endOfBlockIndex + 1, newRegistration);
        File.WriteAllText(modulePath, moduleContent);

        Console.WriteLine($"{entityName} registered in AutofacBusinessModule.cs.");
    }

    private static string FindSolutionDirectory()
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        DirectoryInfo directoryInfo = new DirectoryInfo(baseDirectory);

        while (directoryInfo != null && !directoryInfo.GetFiles("*.sln").Any())
        {
            directoryInfo = directoryInfo.Parent;
        }

        return directoryInfo?.FullName;
    }

    private static void Main(string[] args)
    {
        Console.WriteLine("Please select the operation type:");
        Console.WriteLine("1. Scaffold for All Tables");
        Console.WriteLine("2. Operations Other than Scaffold");
        Console.Write("Your choice: ");
        string operationChoice = Console.ReadLine();

        string projectDirectory = FindSolutionDirectory();

        if (projectDirectory == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Project root directory not found.");
            Console.ResetColor();
            return;
        }

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(projectDirectory, "WebApi"))
            .AddJsonFile("appsettings.json")
            .Build();

        KeyCase.Instance.LoadFromConfiguration(configuration);

        string connectionString = configuration.GetConnectionString("DatabaseConnection");

        Enigma.Processor processor = new Enigma.Processor();

        try
        {
            using (Aes aes = Aes.Create())
            {
                connectionString = processor.DecryptorSymmetric(connectionString, aes);
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error decrypting connection string: {ex.Message}");
            Console.ResetColor();
            return;
        }

        string dbContextClassName = "ContextDb";

        switch (operationChoice)
        {
            case "1":
                ScaffoldAllTables(projectDirectory, connectionString, dbContextClassName);
                break;
            case "2":
                Console.Write("Please enter the model name: ");
                string entityName = Console.ReadLine();
                GenerateDalAndRegistration(projectDirectory, entityName, dbContextClassName);
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Invalid selection. Please choose 1 or 2.");
                Console.ResetColor();
                break;
        }
    }
}