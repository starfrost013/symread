

namespace symread
{
    /// <summary>
    /// Handles command line parsing.
    /// 
    /// Default: -f4 -text 
    /// </summary>
    internal static class CommandLine
    {
        /// <summary>
        /// General flags
        /// </summary>
        internal static CommandLineFlags Flags;

        /// <summary>
        /// File format version to read
        /// </summary>
        internal static SymFileFormatVer Version;

        /// <summary>
        /// Input filename
        /// </summary>
        internal static string? InputFile;

        /// <summary>
        /// Output filename (optional)
        /// </summary>
        internal static string? OutputFile;

        [Flags]
        internal enum CommandLineFlags
        { 
            /// <summary>
            /// Be quiet. Don't output anything.
            /// </summary>
            BeQuiet = 1,

            /// <summary>
            /// Be loud. Print everything!
            /// </summary>
            BeLoud = 1 << 1,

            /// <summary>
            /// Convert MAP to SYM file
            /// </summary>
            Map2Sym = 1 << 2,

            /// <summary>
            /// Convert SYM to MAP file
            /// </summary>
            Sym2Map = 1 << 3,   
        }

        internal enum SymFileFormatVer
        {
            /// <summary>
            /// Used to indicate the symver has not been specified.
            /// </summary>
            SYMVER_NOT_SPECIFIED = 0,

            /// <summary>
            /// Minimum sentinel value
            /// </summary>
            SYMVER_MIN = 3,

            /// <summary>
            /// SYMDEB 3.x format
            /// Windows 1.0 Alpha
            /// MT-DOS 5/29/84 (hopefully)
            /// </summary>
            SYMDEB_V3 = 3,

            /// <summary>
            /// SymDeb V"3.Windows" and V4
            /// Windows 1.0x (and Windows 2.0, when it was called "Windows 1.5")
            /// MT-DOS 4.0 (build 6.7)
            /// OS/2 1986 and early 1987 builds
            /// probably also very early word for windows and excel for windows?
            /// </summary>
            SYMDEB_V4 = 4,

            /// <summary>
            /// Symdeb V5 (Branded as "Windows Version 2.00"/"Windows Version 3.00", but MapSym version is 5.x
            /// Windows 2.x, Windows 3.0
            /// OS/2 final?
            /// MT-DOS 4.1?
            /// </summary>
            SYMDEB_V5 = 5,

            /// <summary>
            /// Maximum sentinel value
            /// </summary>
            SYMVER_MAX = 5,
        }

        /// <summary>
        /// Parses the command line.
        /// </summary>
        /// <param name="args">The CL args to parse.</param>
        internal static void Parse(string[] args)
        {
            foreach (string arg in args)
            {
                // parse -f (file format version)
                if (arg.Contains("-f", StringComparison.InvariantCultureIgnoreCase))
                {
                    string[] arg_split = arg.Split(new char[]{'0','1','2','3','4','5','6','7','8','9'}, StringSplitOptions.RemoveEmptyEntries);

                    if (arg_split.Count() < 2)
                    {
                        PrintLoud("Warning: No format version provided. Defauling to V4", ConsoleColor.Yellow);
                        Version = SymFileFormatVer.SYMDEB_V4;
                    }

                    // by this point it must be 0-9 due to the split
                    Version = (SymFileFormatVer)Convert.ToInt32(arg_split[1]);

                    if (Version < SymFileFormatVer.SYMVER_MIN
                        || Version > SymFileFormatVer.SYMVER_MAX)
                    {
                        PrintLoud($"Warning: Invalid format version {Version} provided. Defauling to V4", ConsoleColor.Yellow);
                        Version = SymFileFormatVer.SYMDEB_V4;
                    }

                } // in the case of loud vs. quiet, loud takes priority
                else if (arg.Contains("-quiet", StringComparison.InvariantCultureIgnoreCase))
                {
                    // ignore multiple and nuke loud if quiet provided
                    if (Flags.HasFlag(CommandLineFlags.BeLoud)) Flags -= CommandLineFlags.BeQuiet;
                    Flags |= CommandLineFlags.BeQuiet;
                }
                else if (arg.Contains("-loud", StringComparison.InvariantCultureIgnoreCase))
                {
                    // ignore multiple and nuke quiet if loud provided
                    if (Flags.HasFlag(CommandLineFlags.BeQuiet)) Flags -= CommandLineFlags.BeQuiet;
                    Flags |= CommandLineFlags.BeLoud;
                }
                else if (arg.Contains("-text", StringComparison.InvariantCultureIgnoreCase))
                {
                    // turn off map2sym if -text provided
                    if (Flags.HasFlag(CommandLineFlags.Map2Sym)) Flags -= CommandLineFlags.Map2Sym;
                    if (Flags.HasFlag(CommandLineFlags.Sym2Map)) Flags -= CommandLineFlags.Sym2Map;
                }
                else if (arg.Contains("-sym", StringComparison.InvariantCultureIgnoreCase))
                {
                    // -sym takes precedence over -text, but not -map, if they are all provided
                    if (Flags.HasFlag(CommandLineFlags.Map2Sym)) Flags -= CommandLineFlags.Map2Sym;
                    Flags |= CommandLineFlags.Sym2Map;
                }
                else if (arg.Contains("-map", StringComparison.InvariantCultureIgnoreCase))
                {
                    // turn on map2sym if -map provided
                    // -map takes precedence over -sym and -text if they are all provided
                    Flags |= CommandLineFlags.Map2Sym;
                }
                else // allow input and output files to be anywhere
                {
                    if (InputFile != null)
                    {
                        OutputFile = arg;
                    }
                    else
                    {
                        InputFile = arg;
                    }
                }
            }

            // Validate the provided CL args

            if (args.Length < 1) PrintError(SYMREAD_STRING_ERROR_NO_ARGS, ConsoleColor.Red, true);
            if (InputFile == null) PrintError(SYMREAD_STRING_ERROR_INPUT_NO_FILE_PROVIDED, ConsoleColor.Red, true);
            if (!File.Exists(InputFile)) PrintError(SYMREAD_STRING_ERROR_INPUT_INVALID_FILE_PROVIDED, ConsoleColor.Red, true);
            // output is required for everything afaik
            if (OutputFile == null) PrintError(SYMREAD_STRING_ERROR_OUTPUT_NO_FILE_PROVIDED, ConsoleColor.Red, true);
            if (!File.Exists(OutputFile)) PrintError(SYMREAD_STRING_ERROR_OUTPUT_INVALID_FILE_PROVIDED, ConsoleColor.Red, true);
        }
    }
}
