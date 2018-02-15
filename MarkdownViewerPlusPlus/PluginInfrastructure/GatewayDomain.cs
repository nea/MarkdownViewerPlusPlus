// NPP plugin platform for .Net v0.94.00 by Kasper B. Graversen etc.
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Kbg.NppPluginNET.PluginInfrastructure
{
    /// <summary>
    /// Colours are set using the RGB format (Red, Green, Blue). The intensity of each colour is set in the range 0 to 255.
    /// If you have three such intensities, they are combined as: red | (green &lt;&lt; 8) | (blue &lt;&lt; 16).
    /// If you set all intensities to 255, the colour is white. If you set all intensities to 0, the colour is black.
    /// When you set a colour, you are making a request. What you will get depends on the capabilities of the system and the current screen mode.
    /// </summary>
    public class Colour
    {
        public readonly int Red, Green, Blue;

        public Colour(int rgb)
		{
			Red = rgb & 0xFF;
			Green = (rgb >> 8) & 0xFF;
			Blue = (rgb >> 16) & 0xFF;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="red">a number 0-255</param>
        /// <param name="green">a number 0-255</param>
        /// <param name="blue">a number 0-255</param>
        public Colour(int red, int green, int blue)
        {
            if(red > 255 || red < 0)
                throw new ArgumentOutOfRangeException("red", "must be 0-255");
            if(green > 255 || green < 0)
                throw new ArgumentOutOfRangeException("green", "must be 0-255");
            if(blue > 255 || blue < 0)
                throw new ArgumentOutOfRangeException("blue", "must be 0-255");
            Red = red;
            Green = green;
            Blue = blue;
        }

        public int Value
        {
            get { return Red + (Green << 8) + (Blue << 16); }
        }
    }

    /// <summary>
    /// Positions within the Scintilla document refer to a character or the gap before that character.
    /// The first character in a document is 0, the second 1 and so on. If a document contains nLen characters, the last character is numbered nLen-1. The caret exists between character positions and can be located from before the first character (0) to after the last character (nLen).
    ///
    /// There are places where the caret can not go where two character bytes make up one character.
    /// This occurs when a DBCS character from a language like Japanese is included in the document or when line ends are marked with the CP/M
    /// standard of a carriage return followed by a line feed.The INVALID_POSITION constant(-1) represents an invalid position within the document.
    ///
    /// All lines of text in Scintilla are the same height, and this height is calculated from the largest font in any current style.This restriction
    /// is for performance; if lines differed in height then calculations involving positioning of text would require the text to be styled first.
    ///
    /// If you use messages, there is nothing to stop you setting a position that is in the middle of a CRLF pair, or in the middle of a 2 byte character.
    /// However, keyboard commands will not move the caret into such positions.
    /// </summary>
    public class Position : IEquatable<Position>
    {
        private readonly int pos;

        public Position(int pos)
        {
            this.pos = pos;
        }

        public int Value
        {
            get { return pos; }
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position(a.pos + b.pos);
        }

        public static Position operator -(Position a, Position b)
        {
            return new Position(a.pos - b.pos);
        }

        public static bool operator ==(Position a, Position b)
        {
	        if (ReferenceEquals(a, b))
		        return true;
			if (ReferenceEquals(a, null))
				return false;
			if (ReferenceEquals(b, null))
				return false;
			return  a.pos == b.pos;
        }

        public static bool operator !=(Position a, Position b)
        {
            return !(a == b);
        }

        public static bool operator >(Position a, Position b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(Position a, Position b)
        {
            return a.Value < b.Value;
        }

        public static Position Min(Position a, Position b)
        {
            if (a < b)
                return a;
            return b;
        }

		public static Position Max(Position a, Position b)
		{
			if (a > b)
				return a;
			return b;
		}

		public override string ToString()
        {
            return "Postion: " + pos;
        }

        public bool Equals(Position other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return pos == other.pos;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Position)obj);
        }

        public override int GetHashCode()
        {
            return pos;
        }
    }

    /// <summary>
    /// Class containing key and modifiers
    ///
    /// The key code is a visible or control character or a key from the SCK_* enumeration, which contains:
    /// SCK_ADD, SCK_BACK, SCK_DELETE, SCK_DIVIDE, SCK_DOWN, SCK_END, SCK_ESCAPE, SCK_HOME, SCK_INSERT, SCK_LEFT, SCK_MENU, SCK_NEXT(Page Down), SCK_PRIOR(Page Up), S
    /// CK_RETURN, SCK_RIGHT, SCK_RWIN, SCK_SUBTRACT, SCK_TAB, SCK_UP, and SCK_WIN.
    ///
    /// The modifiers are a combination of zero or more of SCMOD_ALT, SCMOD_CTRL, SCMOD_SHIFT, SCMOD_META, and SCMOD_SUPER.
    /// On OS X, the Command key is mapped to SCMOD_CTRL and the Control key to SCMOD_META.SCMOD_SUPER is only available on GTK+ which is commonly the Windows key.
    /// If you are building a table, you might want to use SCMOD_NORM, which has the value 0, to mean no modifiers.
    /// </summary>
    public class KeyModifier
    {
        private readonly int value;

        /// <summary>
        /// The key code is a visible or control character or a key from the SCK_* enumeration, which contains:
        /// SCK_ADD, SCK_BACK, SCK_DELETE, SCK_DIVIDE, SCK_DOWN, SCK_END, SCK_ESCAPE, SCK_HOME, SCK_INSERT, SCK_LEFT, SCK_MENU, SCK_NEXT(Page Down), SCK_PRIOR(Page Up),
        /// SCK_RETURN, SCK_RIGHT, SCK_RWIN, SCK_SUBTRACT, SCK_TAB, SCK_UP, and SCK_WIN.
        ///
        /// The modifiers are a combination of zero or more of SCMOD_ALT, SCMOD_CTRL, SCMOD_SHIFT, SCMOD_META, and SCMOD_SUPER.
        /// On OS X, the Command key is mapped to SCMOD_CTRL and the Control key to SCMOD_META.SCMOD_SUPER is only available on GTK+ which is commonly the Windows key.
        /// If you are building a table, you might want to use SCMOD_NORM, which has the value 0, to mean no modifiers.
        /// </summary>
        public KeyModifier(SciMsg SCK_KeyCode, SciMsg SCMOD_modifier)
        {
            value = (int) SCK_KeyCode | ((int) SCMOD_modifier << 16);
        }

        public int Value
        {
            get { return Value; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CharacterRange
    {
        public CharacterRange(int cpmin, int cpmax) { cpMin = cpmin; cpMax = cpmax; }
        public int cpMin;
        public int cpMax;
    }

    public class Cells
    {
        char[] charactersAndStyles;

        public Cells(char[] charactersAndStyles)
        {
            this.charactersAndStyles = charactersAndStyles;
        }

        public char[] Value { get { return charactersAndStyles; } }
    }

    public class TextRange : IDisposable
    {
        Sci_TextRange _sciTextRange;
        IntPtr _ptrSciTextRange;
        bool _disposed = false;

        public TextRange(CharacterRange chrRange, int stringCapacity)
        {
            _sciTextRange.chrg = chrRange;
            _sciTextRange.lpstrText = Marshal.AllocHGlobal(stringCapacity);
        }
        public TextRange(int cpmin, int cpmax, int stringCapacity)
        {
            _sciTextRange.chrg.cpMin = cpmin;
            _sciTextRange.chrg.cpMax = cpmax;
            _sciTextRange.lpstrText = Marshal.AllocHGlobal(stringCapacity);
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Sci_TextRange
        {
            public CharacterRange chrg;
            public IntPtr lpstrText;
        }

        public IntPtr NativePointer { get { _initNativeStruct(); return _ptrSciTextRange; } }

        public string lpstrText { get { _readNativeStruct(); return Marshal.PtrToStringAnsi(_sciTextRange.lpstrText); } }

        public CharacterRange chrg { get { _readNativeStruct(); return _sciTextRange.chrg; } set { _sciTextRange.chrg = value; _initNativeStruct(); } }

        void _initNativeStruct()
        {
            if (_ptrSciTextRange == IntPtr.Zero)
                _ptrSciTextRange = Marshal.AllocHGlobal(Marshal.SizeOf(_sciTextRange));
            Marshal.StructureToPtr(_sciTextRange, _ptrSciTextRange, false);
        }

        void _readNativeStruct()
        {
            if (_ptrSciTextRange != IntPtr.Zero)
                _sciTextRange = (Sci_TextRange)Marshal.PtrToStructure(_ptrSciTextRange, typeof(Sci_TextRange));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_sciTextRange.lpstrText != IntPtr.Zero) Marshal.FreeHGlobal(_sciTextRange.lpstrText);
                if (_ptrSciTextRange != IntPtr.Zero) Marshal.FreeHGlobal(_ptrSciTextRange);
                _disposed = true;
            }
        }

        ~TextRange()
        {
            Dispose();
        }
    }


    /* ++Autogenerated -- start of section automatically generated from Scintilla.iface */
    /* --Autogenerated -- end of section automatically generated from Scintilla.iface */

}
