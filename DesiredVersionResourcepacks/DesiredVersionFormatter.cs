using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class DesiredVersionFormatter
{
    static string currentPath = "";
    static string supportedExtensions = "*.zip";
    static string[] folder_resourcepacks;
    static string[] archive_resourcepacks;
    static int formatNumber;
    static Dictionary<Version[], int> versions = new Dictionary<Version[], int>
    {
        [new Version[]{new Version("1.6.1"), new Version("1.8.9") }] = 1,
        [new Version[]{new Version("1.9"), new Version("1.10.2") }] = 2,
        [new Version[]{new Version("1.11"), new Version("1.12.2") }] = 3,
        [new Version[]{new Version("1.13"), new Version("1.14.4") }] = 4,
        [new Version[]{new Version("1.15"), new Version("1.16.1") }] = 5,
        [new Version[]{new Version("1.16.2"), new Version("1.16.5") }] = 6,
        [new Version[]{new Version("1.17"), new Version("1.17.1") }] = 7,
        [new Version[]{new Version("1.18"), new Version("1.18.2") }] = 8,
        [new Version[]{new Version("1.19"), new Version("1.19.2") }] = 9,
        [new Version[]{new Version("1.19.3"), new Version("1.19.3") }] = 12,
    };
    static Version version;

    // Протестить
    public static bool SetPath(string path)
    {
        if (Directory.Exists(path + "\\.minecraft\\resourcepacks"))
        {
            currentPath = path;
            return true;
        }
        Console.WriteLine("Папка не найдена, проверьте правильность пути и введите ещё раз");
        return false;
    }

    public static bool FindMinecraft()
    {
        var defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.minecraft\\resourcepacks";
        if (!Directory.Exists(defaultPath))
        {
            Console.WriteLine("Путь по умолчанию не найден, введите путь к папке .minecraft \n Пример: C:\\Users\\USER\\AppData\\Roaming\\.minecraft");
            while (!SetPath(Console.ReadLine()));
            return false;
        }
        currentPath = defaultPath;
        GetResourcepacks();
        return true;
    }
    
    // Доделать архивы
    private static void GetResourcepacks()
    {
        folder_resourcepacks = Directory.GetDirectories(currentPath, "*").ToArray();
        archive_resourcepacks = Directory.GetFiles(currentPath, "*.*").Where(s => supportedExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
        UpdatePackMCMETAFile();
    }

    private static void UpdatePackMCMETAFile()
    {
        string packMCMETA;
        Regex regex = new Regex(@"""pack_format"" *: *\d");
        try
        {
            foreach (string packPath in folder_resourcepacks)
            {
                if (File.Exists(packPath + "\\pack.mcmeta"))
                {
                    packMCMETA = File.ReadAllText(packPath + "\\pack.mcmeta");
                    packMCMETA = regex.Replace(packMCMETA, $"\"pack_format\":{formatNumber}");
                    File.WriteAllText(packPath + "\\pack.mcmeta", packMCMETA);
                    Console.WriteLine($"Формат ресурспака {Path.GetFileName(packPath)} обновлён");
                }
            }
        }
        catch(Exception ex) { Console.WriteLine("Что-то пошло не так( \n" + ex.Message); }

        try
        {
            foreach (string packPath in archive_resourcepacks)
            {
                packMCMETA = ArchiveHandler.ReadFile(packPath);
                if (packMCMETA != string.Empty)
                {
                    packMCMETA = regex.Replace(packMCMETA, $"\"pack_format\":{formatNumber}");
                    ArchiveHandler.WriteFile(packMCMETA, packPath);
                    Console.WriteLine($"Формат ресурспака {Path.GetFileName(packPath)} обновлён");
                }
            }
        }
        catch (Exception ex) { Console.WriteLine("Что-то пошло не так( \n" + ex.Message); }
    }

    public static bool SelectDesiredVersion(string versionString)
    {
        Regex regex = new Regex(@"^\d{1,3}\.\d{1,3}(?:\.\d{1,3})?$");
        if (!regex.IsMatch(versionString)) 
        {
            Console.WriteLine("Неверная версия, введите правильную версию:");
            return false;
        }
        version = Version.Parse(versionString);
        SetFormatNumber();
        return true;
    }

    private static void SetFormatNumber() => formatNumber = versions.Where(v => version >= v.Key[0] && version <= v.Key[1]).FirstOrDefault().Value;
}
    