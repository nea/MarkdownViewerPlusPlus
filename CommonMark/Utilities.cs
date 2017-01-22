using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark
{
    /// <summary>
    /// Reusable utility functions, not directly related to parsing or formatting data.
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Writes a warning to the Debug window.
        /// </summary>
        /// <param name="message">The message with optional formatting placeholders.</param>
        /// <param name="args">The arguments for the formatting placeholders.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Warning(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
                message = string.Format(System.Globalization.CultureInfo.InvariantCulture, message, args);

            System.Diagnostics.Debug.WriteLine(message, "Warning");
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsEscapableSymbol(char c)
        {
            // char.IsSymbol also works with Unicode symbols that cannot be escaped based on the specification.
            return (c > ' ' && c < '0') || (c > '9' && c < 'A') || (c > 'Z' && c < 'a') || (c > 'z' && c < 127) || c == '•';
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsAsciiLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsAsciiLetterOrDigit(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z');
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\f';
        }

        /// <summary>
        /// Checks if the given character is an Unicode space or punctuation character.
        /// </summary>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void CheckUnicodeCategory(char c, out bool space, out bool punctuation)
        {
            // This method does the same as would calling the two built-in methods:
            // // space = char.IsWhiteSpace(c);
            // // punctuation = char.IsPunctuation(c);
            //
            // The performance benefit for using this method is ~50% when calling only on ASCII characters
            // and ~12% when calling only on Unicode characters.

            if (c <= 'ÿ')
            {
                space = c == ' ' || (c >= '\t' && c <= '\r') || c == '\u00a0' || c == '\u0085';
                punctuation = (c >= 33 && c <= 47) || (c >= 58 && c <= 64) || (c >= 91 && c <= 96) || (c >= 123 && c <= 126);
            }
            else
            {
                var category = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                space = category == System.Globalization.UnicodeCategory.SpaceSeparator
                    || category == System.Globalization.UnicodeCategory.LineSeparator
                    || category == System.Globalization.UnicodeCategory.ParagraphSeparator;
                punctuation = !space &&
                    (category == System.Globalization.UnicodeCategory.ConnectorPunctuation
                    || category == System.Globalization.UnicodeCategory.DashPunctuation
                    || category == System.Globalization.UnicodeCategory.OpenPunctuation
                    || category == System.Globalization.UnicodeCategory.ClosePunctuation
                    || category == System.Globalization.UnicodeCategory.InitialQuotePunctuation
                    || category == System.Globalization.UnicodeCategory.FinalQuotePunctuation
                    || category == System.Globalization.UnicodeCategory.OtherPunctuation);
            }
        }

        /// <summary>
        /// Determines if the first line (ignoring the first <paramref name="startIndex"/>) of a string contains only spaces.
        /// </summary>
        public static bool IsFirstLineBlank(string source, int startIndex)
        {
            char c;
            var lastIndex = source.Length;
            
            while (startIndex < lastIndex)
            {
                c = source[startIndex];
                if (c == '\n')
                    return true;

                if (c != ' ')
                    return false;

                startIndex++;
            }

            return true;
        }
    }
}
