
using SingletonExample;

public class Program
{
    public static void Main(string[] args)
    {
        Cache.Instance.Add("username", "john_doe");
        Console.WriteLine($"Username from cache: {Cache.Instance.Get("username")}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}