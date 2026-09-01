using System.Runtime.InteropServices;

namespace DB_EDITOR
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Environment.ExitCode = RunCli(args);
                return;
            }

            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            ApplicationConfiguration.Initialize();
            Application.Run(new MainEditor());
        }

        // The app is built as a Windows (WinExe) app, so it has no console of its own.
        // Attaching to whatever console launched us lets Console.WriteLine show up there
        // for CLI runs, without ever popping a console window open for normal GUI launches.
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        private static int RunCli(string[] args)
        {
            AttachConsole(ATTACH_PARENT_PROCESS);

            CliOptions options;
            try
            {
                options = CliOptions.Parse(args);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine();
                Console.WriteLine(CliOptions.HelpText);
                return 1;
            }

            if (options.ShowHelp)
            {
                Console.WriteLine(CliOptions.HelpText);
                return 0;
            }

            // MainEditor is still constructed - InitializeComponent() runs, so every control
            // that ExportDB/ImportDB/etc. touch (progress bar, menu items, ...) really exists -
            // but Show()/Application.Run() is never called, so no window ever appears.
            using MainEditor editor = new MainEditor();
            return editor.RunCliCommands(options);
        }
    }
}
