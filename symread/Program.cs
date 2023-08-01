// See https://aka.ms/new-console-template for more information

using Starlib.Utilities;

#region Globals
const string SYMREAD_STRING_BRANDING = $"SymRead 0.1.0 /a/lpha";
const string SYMREAD_STRING_DESCRIPTION = $"A program to read Windows Symdeb format files";
const string SYMREAD_STRING_ERROR_NO_FILE_PROVIDED = "No file provided!";
const string SYMREAD_STRING_ERROR_FILE_NOT_SYM = "File is not a .SYM file!";
const string SYMREAD_STRING_HELP = "Syntax: symread [file name]...";
#endregion

#region Sign-on
Logger.Settings.KeepOldLogs = false;

Logger.Init();

Logger.Log(SYMREAD_STRING_BRANDING));
Logger.Log(SYMREAD_STRING_DESCRIPTION);

#endregion

#region Argument parsing
if (!File.Exists(args[0]))
{
    Logger.LogError(SYMREAD_STRING_ERROR_NO_FILE_PROVIDED, 1, LoggerSeverity.Error);
    PrintHelpAndExit(1);
}

if (!args[0].Contains(".sym"))
{
    Logger.LogError(SYMREAD_STRING_ERROR_FILE_NOT_SYM, 2, LoggerSeverity.Error);
}
#endregion

string fileName = args[0];


#region Utility functions

void PrintHelpAndExit(int exitCode)
{
    Logger.Log(SYMREAD_STRING_HELP, ConsoleColor.White, false);
    Environment.Exit(exitCode);
}

#endregion