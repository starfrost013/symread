/*  SymRead
 *  A program to read SYMDEB (Microsoft Symbolic Debugging Utility) / WDEB386 (Windows Debugger for 80386+) symbol (.SYM) files. 
 *  
 *  © 2023 starfrost
 *  Licensed under the MIT License (see version.txt)
 */

#region Globals 

const int SYMREAD_VERSION_MAJOR = 1;
const int SYMREAD_VERSION_MINOR = 1;
const int SYMREAD_VERSION_REVISION = 0;

// localise here
// not a const because it substitutes in the version number
string SYMREAD_STRING_BRANDING = $"SymRead (proof of concept {SYMREAD_VERSION_MAJOR}.{SYMREAD_VERSION_MINOR}.{SYMREAD_VERSION_REVISION})";
const string SYMREAD_STRING_DESCRIPTION = $"A program to read Windows Symdeb format files"; // extra newline
const string SYMREAD_STRING_ERROR_NO_FILE_PROVIDED = "File not found!";
const string SYMREAD_STRING_ERROR_FILE_NOT_SYM = "File is not a .SYM file!";
const string SYMREAD_STRING_HELP = "Syntax: symread [file name]...";
const string SYMREAD_STRING_ERROR = "An error occurred! Report it to starfrost...(starfrost#7777, or new way: thefrozenstar_)\nException info:\n\n";
#endregion

try
{
    #region Argument parsing
    if (args.Length < 1
        || !File.Exists(args[0]))
    {
        Console.WriteLine(SYMREAD_STRING_ERROR_NO_FILE_PROVIDED);
        PrintHelpAndExit(1);
    }

    if (!args[0].Contains(".sym", StringComparison.InvariantCultureIgnoreCase))
    {
        Console.WriteLine(SYMREAD_STRING_ERROR_FILE_NOT_SYM);
        PrintHelpAndExit(2);
    }
    #endregion

    #region Sign-on
    Console.WriteLine(SYMREAD_STRING_BRANDING);
    Console.WriteLine(SYMREAD_STRING_DESCRIPTION);
    #endregion

    var fileName = args[0];

    using (var reader = new BinaryReader(new FileStream(fileName, FileMode.Open)))
    {
        #region Reading header
        Console.WriteLine("\n** READING HEADER **\n");
        Console.WriteLine($"Segment ptr to next symbol map (0 if end) {reader.ReadUInt16()}");  // Pointer to next symbol map:           0x00 (always lower than total symbol count)
        reader.ReadBytes(4);                                                                    // Currently unknown:                       0x02, 0x04
        ushort numberOfConstants = reader.ReadUInt16();                                         // Number of symbols for constants section  0x06
        Console.WriteLine($"Number of constants: {numberOfConstants}");
        Console.WriteLine($"Offset to unknown data for constants: 0x{reader.ReadUInt16():X}");  // Offset to beginning of unknown data:     0x08
        ushort numberOfSegments = reader.ReadUInt16();                                          // Number of segments:                      0x0A
        Console.WriteLine($"Number of segments: {numberOfSegments}");    
        Console.WriteLine($"Relative pointer to segment chain: 0x{reader.ReadUInt16():X}");     // Pointer to segment chain (relative)      0x0C
        Console.WriteLine($"Maximum length of symbol name: {reader.ReadByte()} characters");    // Maximum length of symbol name            0x0E [byte]
        Console.WriteLine($"Binary name: {reader.ReadString()}");                               // Binary name:                             Length 0x0F, string 0x10..0x10+(length)
        #endregion

        #region Reading constants section
        // Segment 0 is a bit weird. It holds constants that are referenced with absolute addresses.
        // We just handle it separately lol.

        Console.WriteLine("\n** READING CONSTANTS SEGMENT **\n");
        reader.ReadByte();                                                                      // random extra byte here. idk why. might as well put a nice string...

        for (int numConstant = 0; numConstant < numberOfConstants; numConstant++)
        {
            ushort address = reader.ReadUInt16();                                               // its over if you wanted over 64kb of constants
            string name = reader.ReadString();

            Console.WriteLine($"START OF CODE + 0x{address:X4}: {name}");                      // converting MAP files to SYM indicates these are absolute addresses from the start of the file
        }
        #endregion

        // skip unknown data (for now)
        // incrementing for each functino, two bytes long, duplicated for num of funcs. maybe size but doesn't seem to add up :(... (all segments have this too)
        // SYMMAP also references line number data which you can strip but didn't haev any effect on the MAP i tried.
        reader.ReadBytes(2 * numberOfConstants);

        while (reader.BaseStream.Position % 16 != 0) reader.ReadByte();                         // nasty kludge but number of 00s is not consistent - it's padded to nearest 0x10 
                                                                                                // so wait until that offset...(only for constants???)
        #region Reading segments

        for (int currentSegment = 0; currentSegment < numberOfSegments; currentSegment++)       // iterate through all of the segments.
        {                                                                                       // **SEGMENT HEADER**
            Console.WriteLine($"Next segment pointer: (0x00 if last): 0x{reader.ReadUInt16():X}");   // Pointer to next segment                  0x00
            ushort numberOfSymbols = reader.ReadUInt16();                                       // Number of symbols in segment:            0x02
            ushort sizeOfSymbolData = reader.ReadUInt16();                                      // Size of symbol data:                     0x04
            ushort realSegmentNumber = reader.ReadUInt16();
            // Next 12 bytes don't seem to matter, and are seemingly always 00 00 00 00 00 00 00 00 00 00 00 FF for every segment.
            reader.ReadBytes(12);

            string segmentName = reader.ReadString();                                           // Segment name:                            0x14 length: 0x15 for everything else,

            Console.WriteLine($"\n** READING SEGMENT {segmentName} **\n");                      // Using numSegments doesn't matter as we got it from the header. But use the real segment number, just in case it's out of order on some binary.              
            Console.WriteLine($"Number of symbols: {numberOfSymbols}");                         // Print out all that metadata we just got...
            Console.WriteLine($"Size of symbol data: {sizeOfSymbolData}");
            Console.WriteLine($"Segment number: {realSegmentNumber}\n");

            for (int currentSymbol = 0; currentSymbol < numberOfSymbols; currentSymbol++)       // Iterate through every symbol.
            {
                ushort offset = reader.ReadUInt16();
                string symbolName = reader.ReadString();

                Console.WriteLine($"{segmentName}:{offset:X4}\t {symbolName}");
            }

            // (again) skip the unknown data for now...
            // incrementing for each functino, two bytes long, duplicated for num of funcs. maybe size but doesn't seem to add up :(... (all segments have this too)
            // SYMMAP also references line number data which you can strip but didn't haev any effect on the MAP i tried.
            reader.ReadBytes(2 * numberOfSymbols);

            while (reader.BaseStream.Position % 16 != 0) reader.ReadByte();                     // nasty kludge but number of 00s is not consistent - it's padded to nearest 0x10 
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nDone!");
        Console.ResetColor();

        #endregion
    }

    #region Utility functions

    void PrintHelpAndExit(int exitCode)
    {
        Console.WriteLine(SYMREAD_STRING_HELP, ConsoleColor.White, false);
        Environment.Exit(exitCode);
    }

    #endregion
}
catch (Exception ex)
{
    Console.WriteLine($"{SYMREAD_STRING_ERROR}{ex}");
}