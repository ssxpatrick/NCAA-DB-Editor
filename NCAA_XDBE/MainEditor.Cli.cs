namespace DB_EDITOR
{
    // Everything the command line needs lives in this partial file so the rest of
    // MainEditor.cs / ExportTool.cs / ImportTool.cs stays close to the original.
    public partial class MainEditor : Form
    {
        /// <summary>
        /// True whenever the app was launched with CLI arguments (see Program.cs). While true,
        /// ShowMessage() prints to the console and auto-answers "Yes"/"OK" instead of popping
        /// up a MessageBox that nobody is there to click - which would otherwise hang the
        /// process forever in an unattended run.
        /// </summary>
        public bool CliMode { get; private set; } = false;

        // When set (CLI mode only), these override where ExportAllTables()/ImportAllTables()
        // read/write files, instead of the exe's own folder. Left empty, both keep their
        // existing GUI behavior untouched. Consumed inside ExportTool.cs / ImportTool.cs.
        private string cliExportAllDir = "";
        private string cliImportAllDir = "";

        /// <summary>
        /// Entry point used by Program.cs when the app is launched with command line arguments.
        /// Runs open -> import-all -> export-all -> save, in that order, against the already-
        /// constructed (but never shown) MainEditor instance. Returns a process exit code.
        /// </summary>
        public int RunCliCommands(CliOptions options)
        {
            CliMode = true;
            tabDelimited = options.TabDelimited;

            if (!File.Exists(options.OpenPath))
            {
                Console.WriteLine($"Error: file not found: {options.OpenPath}");
                return 2;
            }

            if (!OpenFile(options.OpenPath))
            {
                Console.WriteLine("Error: failed to open database file.");
                return 2;
            }
            Console.WriteLine($"Opened: {options.OpenPath}");

            if (!string.IsNullOrEmpty(options.ImportAllDir))
            {
                if (!Directory.Exists(options.ImportAllDir))
                {
                    Console.WriteLine($"Error: import directory not found: {options.ImportAllDir}");
                    return 3;
                }

                cliImportAllDir = options.ImportAllDir;
                ImportAllTables();
                cliImportAllDir = "";
                Console.WriteLine($"Imported all tables from: {options.ImportAllDir}");
            }

            if (!string.IsNullOrEmpty(options.ExportAllDir))
            {
                Directory.CreateDirectory(options.ExportAllDir);

                cliExportAllDir = options.ExportAllDir;
                ExportAllTables();
                cliExportAllDir = "";
                Console.WriteLine($"Exported all tables to: {options.ExportAllDir}");
            }

            if (options.Save)
            {
                string savePath = string.IsNullOrEmpty(options.SavePath) ? options.OpenPath : options.SavePath;
                if (!SaveFile(savePath))
                {
                    Console.WriteLine("Error: failed to save database file.");
                    return 5;
                }
                Console.WriteLine($"Saved: {savePath}");
            }

            if (dbIndex != -1) CloseDB(dbIndex);
            if (dbIndex2 == 1) CloseDB(dbIndex2);

            return 0;
        }

        /// <summary>
        /// Central place for every dialog the Open/Save/ExportAll/ImportAll flow shows. In CLI
        /// mode this prints to the console and auto-answers instead of blocking on a MessageBox
        /// that has no user to click it - the caller's chosen answer ("Yes" for confirmations)
        /// is returned immediately. In the normal GUI it behaves exactly like a direct
        /// MessageBox.Show call did before.
        /// </summary>
        private DialogResult ShowMessage(string text, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            if (CliMode)
            {
                Console.WriteLine(string.IsNullOrEmpty(caption) ? text : $"[{caption}] {text}");
                return (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
                    ? DialogResult.Yes
                    : DialogResult.OK;
            }

            return MessageBox.Show(text, caption, buttons, icon);
        }
    }
}
