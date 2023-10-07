namespace symread
{
    /// <summary>
    /// Provides some custom standardout functions
    /// </summary>
    internal static class StandardOut
    {
        /// <summary>
        /// Print to console only when loud verbosity is selected.
        /// </summary>
        /// <param name="stdin">Text to print.</param>
        /// <param name="color">The color to print the text in.</param>
        /// <remarks>Does nothing if <see cref="CommandLine.CommandLineFlags.BeLoud"/> is not set.</remarks>
        internal static void PrintQuiet(string stdin, ConsoleColor color = ConsoleColor.Gray)
        {
            if (CommandLine.Flags.HasFlag(CommandLine.CommandLineFlags.BeLoud)) PrintCore(stdin, color);
        }

        /// <summary>
        /// Print to console only when normal or loud verbosity is selected.
        /// </summary>
        /// <param name="stdin">Text to print.</param>
        /// <param name="color">The color to print the text in.</param>
        /// <remarks>Does nothing if <see cref="CommandLine.CommandLineFlags.BeQuiet"/> is set.</remarks>
        internal static void Print(string stdin, ConsoleColor color = ConsoleColor.Gray)
        {
            if (!CommandLine.Flags.HasFlag(CommandLine.CommandLineFlags.BeQuiet)) PrintCore(stdin, color);
        }

        /// <summary>
        /// Print to console on all verbosities.
        /// </summary>
        /// <param name="stdin">Text to print.</param>
        /// <param name="color">The color to print the text in.</param>
        internal static void PrintLoud(string stdin, ConsoleColor color = ConsoleColor.Gray) => PrintCore(stdin, color);

        /// <summary>
        /// <para>Print to console on all verbosities, and then exits the program.</para>
        /// Used for fatal errors, such as command line validation failure.
        /// </summary>
        /// <param name="stdin">Text to print.</param>
        /// <param name="color">The color to print the text in.</param>
        /// <param name="printHelp">Print the help text before exiting. If this parameter is set to <c>true</c>,
        /// the <paramref name="color"/> parameter is ignored when printing the help text, and the help text will always be printed in gray.</param>
        internal static void PrintError(string stdin, ConsoleColor color = ConsoleColor.Gray, bool printHelp = false)
        {
            PrintLoud($"ERROR: {stdin}\nSymRead must exit.", color);
            if (printHelp) PrintHelp(ConsoleColor.Gray);
            Environment.Exit(1);
        }

        /// <summary>
        /// <para>Print to console on all verbosities, and then exits the program.</para>
        /// <para>Used by the <see cref="PrintError(string, ConsoleColor, bool)"/> method, but may be used in the future by other methods,
        /// so it's a custom mmethod..</para>
        /// </summary>
        /// <param name="color">The color to print the help text in.</param>
        private static void PrintHelp(ConsoleColor color = ConsoleColor.Gray) => PrintCore(SYMREAD_STRING_HELP, color);

        /// <summary>
        /// Simply calls <see cref="Console.WriteLine"/>
        /// </summary>
        /// <param name="stdin"><para>The object to print. Always a string, in our case, because we use string substitution in all cases,</para>
        /// but of type <c>object</c> for future use.</param>
        /// <param name="color">The color to print the text in.</param>
        private static void PrintCore(object stdin, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(stdin);
            Console.ResetColor();
        }
    }
}
