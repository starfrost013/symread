
namespace symread
{
    /// <summary>
    /// Holds localisable strings for SymRead
    /// </summary>
    internal static class Strings
    {
        // Version info
        internal const int SYMREAD_VERSION_MAJOR = 2;
        internal const int SYMREAD_VERSION_MINOR = 0;
        internal const int SYMREAD_VERSION_REVISION = 0;

        // Branding and sign-on
        internal static string SYMREAD_STRING_BRANDING = $"SymRead v{SYMREAD_VERSION_MAJOR}.{SYMREAD_VERSION_MINOR}.{SYMREAD_VERSION_REVISION}";
        internal const string SYMREAD_STRING_DESCRIPTION = $"A program to read Symdeb format files, used by many early Microsoft operating systems and compilers from 1984 to 1990";

        // Error strings
        internal const string SYMREAD_STRING_ERROR_GENERIC = "An error occurred! Report it to starfrost...(starfrost#7777, or new way: thefrozenstar_)\nException info:\n\n";
        internal const string SYMREAD_STRING_ERROR_INPUT_NO_FILE_PROVIDED = "No input file provided!";
        internal const string SYMREAD_STRING_ERROR_INPUT_INVALID_FILE_PROVIDED = "The input file provided does not exist!";
        internal const string SYMREAD_STRING_ERROR_OUTPUT_NO_FILE_PROVIDED = "No output file provided!";
        internal const string SYMREAD_STRING_ERROR_OUTPUT_INVALID_FILE_PROVIDED = "The output file provided does not exist!";
        internal const string SYMREAD_STRING_ERROR_FILE_NOT_SYM = "File is not a .SYM file!";
        internal const string SYMREAD_STRING_ERROR_NO_ARGS = "No arguments provided.";

        // Warning strings
        internal const string SYMREAD_STRING_WARNING_NO_FORMAT_VERSION = "No format version provided. Defaulting to V4";
        internal const string SYMREAD_STRING_WARNING_INVALID_FORMAT_VERSION_START = "Warning: Invalid format version ";
        internal const string SYMREAD_STRING_WARNING_INVALID_FORMAT_VERSION_END = "provided. Defaulting to V4";

        // Header (segdef) strings
        internal const string SYMREAD_STRING_READING_HEADER = "\n** READING HEADER **\n";
        internal const string SYMREAD_STRING_NEXT_SEGMENT_POINTER = "Segment ptr to next symbol map (0 if end): ";
        internal const string SYMREAD_STRING_NUM_CONSTANTS = "Number of constants: ";
        internal const string SYMREAD_STRING_UNKNOWN_DATA_OFFSET = "Offset to unknown data for constants: 0x";
        internal const string SYMREAD_STRING_NUM_SEGMENTS = "Number of segments: ";
        internal const string SYMREAD_STRING_SEGMENT_CHAIN = "Relative pointer to segment chain: 0x";
        internal const string SYMREAD_STRING_MAX_SYMBOL_NAME_LENGTH_START = "Maximum length of symbol name: ";
        internal const string SYMREAD_STRING_MAX_SYMBOL_NAME_LENGTH_END = "characters";
        internal const string SYMREAD_STRING_BINARY_NAME = "Binary name";

        // Constants section strings
        internal const string SYMREAD_STRING_READING_CONSTANTS = "\n** READING CONSTANTS **\n";
        internal const string SYMREAD_STRING_CONSTANT = "START OF CODE + 0x";

        // Segment section strings
        internal const string SYMREAD_STRING_NEXT_SEGMENT = "Next segment ptr: (0x00 if last): 0x";
        internal const string SYMREAD_STRING_SYMBOL_DATA_SIZE = "Size of symbol data: ";
        internal const string SYMREAD_STRING_SEGMENT_NUMBER = "Segment number: ";

        internal const string SYMREAD_STRING_DONE = "\nDone!";

        internal const string SYMREAD_STRING_HELP = "Syntax:\n" +
            "symread [-f3/-f4/-f5] [-text/-map] [-quiet/loud] input file [output file]\n\n" +
            "Input file options:\n\n" +
            "-f: Format version. Must be followed by format version number between 3 and 5:\n" +
            "\t-f3: Symdeb v3.x (1984-85) - use for Windows 1.0 Alpha, MASM 3.x and MSC 3.x apps\n" +
            "\t-f4: Symdeb v3.Windows / v4.x (1985-87) - use for Windows 1.0x, OS/2 1986 and early 1987 builds, MT-DOS/Multitasking DOS 4.0, MASM 4.x and MSC 4.x apps\n" +
            "\t-f5: Symdeb v5 (1987-1990) - use for Windows 2.x, 3.0, later OS/2, possibly Multitasking DOS 4.1. Not included with MASM 5.x, but use for MASM 5.x and MSC 5.x apps\n\n" +
            "\tDefault value: 4\n\n" +
            "Verbosity options:\n\n" +
            "\t-quiet: Quiet verbosity. Only errors and warnings are printed.\n" +
            "\t-loud: Loud verbosity. Debug information is printed. Overrides -quiet if both are provided.\n\n" +
            "\tDefault value: normal verbosity (neither -quiet or -loud provided)\n\n" +
            "Output file options:\n\n" +
            "\t-map: Output MAP format\n" +
            "\t-sym: Output SYM format\n" +
            "\t-text: Output Symread 1.x text format\n" +
            "\tIf more than one file format is provided, a format is chosen according to this priority: -map, -sym, -text, in descending order.\n\n" +
            "\tDefault value: -text";
    }
}
