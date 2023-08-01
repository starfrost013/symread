// See https://aka.ms/new-console-template for more information


try
{
    #region Globals 
    // localise here
    const string SYMREAD_STRING_BRANDING = $"SymRead (proof of concept version)";
    const string SYMREAD_STRING_DESCRIPTION = $"A program to read Windows Symdeb format files"; // extra newline
    const string SYMREAD_STRING_ERROR_NO_FILE_PROVIDED = "File not found!";
    const string SYMREAD_STRING_ERROR_FILE_NOT_SYM = "File is not a .SYM file!";
    const string SYMREAD_STRING_HELP = "Syntax: symread [file name]...";
    #endregion

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
        Console.WriteLine($"Number of *functions* (NOT symbols): {reader.ReadUInt16()}");       // Num of symbols in entire file:           0x00 (always lower than total symbol count)
        reader.ReadBytes(4);                                                                    // Currently unknown:                       0x02, 0x04
        ushort numberOfConstants = reader.ReadUInt16();                                         // Number of symbols for constants section  0x06
        Console.WriteLine($"Number of constants: {numberOfConstants}");
        Console.WriteLine($"Offset to unknown data for constants: 0x{reader.ReadUInt16():X}");  // Offset to beginning of unknown data:     0x08
        ushort numberOfSegments = reader.ReadUInt16();                                          // Number of segments:                      0x0A
        Console.WriteLine($"Number of segments: {numberOfSegments}");
        reader.ReadBytes(3);                                                                    // Unknown:                                 0x0C [word], 0x0E [byte]
        Console.WriteLine($"Binary name: {reader.ReadString()}");                              // Binary name:                             Length 0x0F, string 0x10..0x10+(length)
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

            Console.WriteLine($"ABSOLUTE ADDRESS 0x{address:X4}: {name}");                      // converting MAP files to SYM indicates these are absolute addresses from the start of the file
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
            reader.ReadBytes(2);                                                                // Unknown :(                               0x00
            ushort numberOfSymbols = reader.ReadUInt16();                                       // Number of symbols in segment:            0x02
            ushort sizeOfSymbolData = reader.ReadUInt16();                                      // Size of symbol data:                     0x04
            ushort realSegmentNumber = reader.ReadUInt16();
            Console.WriteLine($"\n** READING SEGMENT {realSegmentNumber} **\n");                  // Using numSegments doesn't matter as we got it from the header. But use the real segment number, just in case it's out of order on some binary.              
            Console.WriteLine($"Number of symbols: {numberOfSymbols}");                         // Print out all that metadata we just got...
            Console.WriteLine($"Size of symbol data: {sizeOfSymbolData}");

            // Next 12 bytes don't seem to matter, and are seemingly always 00 00 00 00 00 00 00 00 00 00 00 FF for every segment.
            reader.ReadBytes(12);
            Console.WriteLine($"Segment name: {reader.ReadString()}\n");

            for (int currentSymbol = 0; currentSymbol < numberOfSymbols; currentSymbol++)       // Iterate through every symbol.
            {
                ushort offset = reader.ReadUInt16();
                string symbolName = reader.ReadString();

                Console.WriteLine($"{realSegmentNumber:D4}:{offset:X4}: {symbolName}");
            }

            // (again) skip the unknown data for now...
            // incrementing for each functino, two bytes long, duplicated for num of funcs. maybe size but doesn't seem to add up :(... (all segments have this too)
            // SYMMAP also references line number data which you can strip but didn't haev any effect on the MAP i tried.
            reader.ReadBytes(2 * numberOfSymbols);

            while (reader.BaseStream.Position % 16 != 0) reader.ReadByte();                     // nasty kludge but number of 00s is not consistent - it's padded to nearest 0x10 
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Done!");
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
    Console.WriteLine($"An error occurred! Report it to starfrost...(starfrost#7777, or new way: thefrozenstar_)\nException info:\n\n{ex}");
}