namespace DB_EDITOR
{
    /// <summary>
    /// Parsed representation of the command line arguments the app accepts. See
    /// <see cref="HelpText"/> for the full flag list and examples.
    /// </summary>
    public class CliOptions
    {
        public string OpenPath { get; private set; } = "";
        public string ExportAllDir { get; private set; } = "";
        public string ImportAllDir { get; private set; } = "";
        public bool Save { get; private set; } = false;
        public string SavePath { get; private set; } = "";
        public bool TabDelimited { get; private set; } = false;
        public bool ShowHelp { get; private set; } = false;

        public static CliOptions Parse(string[] args)
        {
            var options = new CliOptions();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                switch (arg.ToLowerInvariant())
                {
                    case "--help":
                    case "-h":
                    case "/?":
                        options.ShowHelp = true;
                        break;

                    case "--open":
                        options.OpenPath = RequireValue(args, ref i, "--open");
                        break;

                    case "--export-all":
                        options.ExportAllDir = RequireValue(args, ref i, "--export-all");
                        break;

                    case "--import-all":
                        options.ImportAllDir = RequireValue(args, ref i, "--import-all");
                        break;

                    case "--save":
                        options.Save = true;
                        // --save optionally takes a path; only consume the next token as a
                        // path if there is one and it doesn't look like another flag.
                        if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                        {
                            options.SavePath = args[++i];
                        }
                        break;

                    case "--tab-delimited":
                        options.TabDelimited = true;
                        break;

                    default:
                        throw new ArgumentException($"Unrecognized argument: {arg}");
                }
            }

            if (!options.ShowHelp && string.IsNullOrEmpty(options.OpenPath))
                throw new ArgumentException("--open <path> is required.");

            return options;
        }

        private static string RequireValue(string[] args, ref int i, string flagName)
        {
            if (i + 1 >= args.Length)
                throw new ArgumentException($"{flagName} requires a value.");
            return args[++i];
        }

        public const string HelpText =
@"NCAA Next DB Editor - command line usage

  DB_EDITOR.exe --open <path> [options]

Options:
  --open <path>          Path to the database file to open. Required.
  --import-all <dir>     Import every table from CSV/TXT files in <dir>.
                          (Skips the TEAM table - use the app's Addendum
                          feature for TEAM, same as the UI requires.)
  --export-all <dir>     Export every table to CSV/TXT files in <dir>.
                          <dir> is created if it doesn't already exist.
  --save [path]          Save changes. If [path] is omitted, saves back
                          to the file passed to --open.
  --tab-delimited        Use tab-delimited .txt files instead of .csv,
                          for both --import-all and --export-all.
  --help                 Show this help text.

Notes:
  - When both --import-all and --export-all are given, import runs
    first, then export, then --save (if present) runs last.
  - All confirmation dialogs are answered automatically ('Yes'/'OK') in
    CLI mode, since there's no one there to click them - nothing will
    hang waiting for input. Status and error messages are printed to
    the console instead of shown in a message box.
  - Only the primary database (dbSelected 0) is processed; files with
    a secondary off-season database are not yet supported from the CLI.

Examples:
  DB_EDITOR.exe --open C:\saves\dynasty.dat --export-all C:\out\csv
  DB_EDITOR.exe --open C:\saves\dynasty.dat --import-all C:\in\csv --save
  DB_EDITOR.exe --open C:\saves\dynasty.dat --save C:\saves\dynasty_copy.dat
";
    }
}
