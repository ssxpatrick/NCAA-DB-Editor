# NCAA Database Editor
DB/Save Editor for NCAA Football series on PS2/Xbox/PSP/GC Consoles

Forked version of [NCAA Database Editor](https://github.com/antdroidx/NCAA-DB-Editor) to add custom cli usage of the app

## DB Editor — Command Line Usage
 
```
NCAA_XDBE.exe --open <path> [options]
```
 
Running the exe with no arguments still launches the normal GUI. Any arguments switch it into CLI mode: it runs headlessly (no window), executes the requested actions, prints status to the console, and exits.
 
## Options
 
| Flag | Description |
|---|---|
| `--open <path>` | Path to the database file to open. **Required.** |
| `--import-all <dir>` | Import every table from CSV/TXT files in `<dir>`. Skips the `TEAM` table — use Addendum for that, same as the UI requires. |
| `--export-all <dir>` | Export every table to CSV/TXT files in `<dir>`. `<dir>` is created if it doesn't exist. |
| `--save [path]` | Save changes. If `[path]` is omitted, saves back to the file passed to `--open`. |
| `--tab-delimited` | Use tab-delimited `.txt` files instead of `.csv` for both `--import-all` and `--export-all`. |
| `--help` | Show help text. |
 
## Order of operations
 
If multiple actions are given in one call, they always run in this order:
 
1. Open
2. Import all
3. Export all
4. Save
## Dialogs
 
All confirmation dialogs are answered automatically ("Yes"/"OK") in CLI mode — there's no one there to click them. Status and error messages go to the console instead of a message box.
 
## Exit codes
 
| Code | Meaning |
|---|---|
| `0` | Success |
| `1` | Bad/unrecognized arguments |
| `2` | Failed to open the database file |
| `3` | Import directory missing or import failed |
| `5` | Save failed |
 
## Limitations
 
Only the primary database (`dbSelected 0`) is processed — files with a secondary off-season database aren't supported from the CLI yet.
 
## Examples
 
```bat
:: Export every table to CSV
NCAA_XDBE.exe --open C:\saves\dynasty.dat --export-all C:\out\csv
 
:: Import every table, then save
NCAA_XDBE.exe --open C:\saves\dynasty.dat --import-all C:\in\csv --save
 
:: Save a copy under a new name (no import/export)
NCAA_XDBE.exe --open C:\saves\dynasty.dat --save C:\saves\dynasty_copy.dat
 
:: Tab-delimited round trip
NCAA_XDBE.exe --open C:\saves\dynasty.dat --export-all C:\out\txt --tab-delimited
```


## Publish the application:

```bash
dotnet publish NCAA_XDBE/NCAA_XDBE.csproj -c Release
```

The package is created in the `publish/` directory.
