using System;
using System.Linq;
using System.Globalization;
using System.IO;

namespace NUnicode.Database
{
	public enum EastAsianWidthKind
	{
		Full,
		Half,
		Wide,
		Narrow,
		Ambiguous,
		Neutral,
	}

	public class EastAsianWidth
	{
		class Map
		{
			public int Start;
			public int End;
			public byte Value;
		}

		static readonly byte [] eaw;
		static readonly Map [] map;

		static EastAsianWidth ()
		{
			var stream = typeof (EastAsianWidth).Assembly.GetManifestResourceStream ("EastAsianWidth.dat");
			eaw = new byte [stream.Length];
			stream.Read (eaw, 0, eaw.Length);
			stream = typeof (EastAsianWidth).Assembly.GetManifestResourceStream ("EastAsianWidth.opt");
			Func<string,int> parse = s => int.Parse (s, NumberStyles.HexNumber);
			string line = new StreamReader (stream).ReadToEnd ().Trim ();
			map = line
				.Split (';')
				.Select (l => l.Split ('='))
				.Select (arr => new {Range = arr [0].Split ('-'), Value = arr [1]})
				.Select (m => new Map () {Start = parse (m.Range [0]), End = parse (m.Range [1]), Value = (byte) m.Value [0]})
				.ToArray ();
		}

		static byte GetValue (int c)
		{
			foreach (var m in map)
				if (m.Start <= c && c <= m.End)
					return m.Value;
			return eaw [c];
		}

		public static EastAsianWidthKind GetKind (char c)
		{
			return GetKind ((int) c);
		}

		public static EastAsianWidthKind GetKind (int c)
		{
			var ret = GetValue (c);
			switch (ret) {
			case (byte) 'F':
				return EastAsianWidthKind.Full;
			case (byte) 'H':
				return EastAsianWidthKind.Half;
			case (byte) 'W':
				return EastAsianWidthKind.Wide;
			case (byte) 'a':
				return EastAsianWidthKind.Narrow;
			case (byte) 'A':
				return EastAsianWidthKind.Ambiguous;
			case (byte) 'N':
				return EastAsianWidthKind.Neutral;
			}
			throw new InvalidOperationException ();
		}

		public static bool IsFullWidth (char c, bool isEastAsian)
		{
			return IsFullWidth ((int) c, isEastAsian);
		}

		public static bool IsFullWidth (int c, bool isEastAsian)
		{
			switch (GetKind (c)) {
			case EastAsianWidthKind.Ambiguous:
				return isEastAsian;
			case EastAsianWidthKind.Full:
			case EastAsianWidthKind.Wide:
				return true;
			default:
				return false;
			}
		}
	}
}

