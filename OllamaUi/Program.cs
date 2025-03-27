using System.Diagnostics;

namespace OllamaUi;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        
        ApplicationConfiguration.Initialize();
        if (args.Length > 0 && args[0].Equals("version"))
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = version?.ToString();
            Trace.WriteLine("Version: " + versionString);
            return;
        }
        Application.Run(new Form1());
    }
}