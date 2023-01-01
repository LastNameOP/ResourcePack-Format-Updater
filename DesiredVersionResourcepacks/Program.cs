class Program
{
    static void Main()
    {
        Console.WriteLine("Введите версию:");
        while(!DesiredVersionFormatter.SelectDesiredVersion(Console.ReadLine()));
        DesiredVersionFormatter.FindMinecraft();
        Console.ReadLine();
    }
}