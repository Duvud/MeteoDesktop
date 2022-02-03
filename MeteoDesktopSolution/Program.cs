using MeteoDesktopSolution.Data;

namespace MeteoDesktopSolution;

static class Program
{
    private static Task cosa;

    public static Form1 Form1
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
        
    }
}