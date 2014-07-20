using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

public class Driver
{
	public static void Main (string [] args)
	{
		new Driver ().Run (args);
	}

	struct RangeMap<T>
	{
		public int Start;
		public int End;
		public T Value;
	}

	List<RangeMap<char>> ranges = new List<RangeMap<char>> ();
	char [] eaw = new char [0x10FFFF];

	// list of optimizible mappings.
	RangeMap<char> [] optmap = {
		new RangeMap<char> () {Start = 0x2F800, End = 0x2FA1D, Value = 'W'},
		new RangeMap<char> () {Start = 0x4DC0, End = 0x4DFF, Value = 'N'},
		new RangeMap<char> () {Start = 0xD7B0, End = 0xD7C6, Value = 'N'},
		new RangeMap<char> () {Start = 0xD7CB, End = 0xD7FB, Value = 'N'},
		new RangeMap<char> () {Start = 0xE0001, End = 0xE0001, Value = 'N'},
		new RangeMap<char> () {Start = 0xE0020, End = 0xE007F, Value = 'N'},
		new RangeMap<char> () {Start = 0xE0100, End = 0xE01EF, Value = 'A'},
		};

	public void Run (string [] args)
	{
		string text = null;
		string output = "EastAsianWidth.dat";
		string mapoutput = "EastAsianWidth.opt";
		string source = args.Length == 0 && File.Exists ("EastAsianWidth.txt") ? "EastAsianWidth.txt" : args.FirstOrDefault ();
		if (source != null)
			text = File.ReadAllText (source);
		else {
			string univer = args.Length > 0 ? args [0] : "UCD/latest";

			string url = string.Format ("http://www.unicode.org/Public/{0}/ucd/EastAsianWidth.txt", univer);

			text = new WebClient ().DownloadString (url);
		}

		var lines = text.Split ('\n');
		Func<string,string> first = s => s.Substring (0, s.IndexOf ('.'));
		Func<string,string> last = s => s.Substring (s.LastIndexOf ('.') + 1);
		Func<string,int> parse = s => int.Parse (s, NumberStyles.HexNumber);
		foreach (var p in lines
				.Select (x => x.Split ('#'))
				.Select (arr => arr [0].Trim ())
				.Where (x => x.Contains (';'))
				.Select (x => x.Split (';'))) {
			if (p [0].Contains ('.'))
				ranges.Add (new RangeMap<char> () {Start = parse (first (p [0])), End = parse (last (p [0])), Value = p [1].Last ()});
			else
				eaw [parse (p [0])] = p [1].Last ();
		}
		// Apply mapping optimizer - for predefined optimizible list, fill '\0' (only if the mapping was correct)
		foreach (var m in optmap) {
			bool invalid = false;
			for (int i = m.Start; i <= m.End; i++)
				if (eaw [i] != m.Value) {
					Console.Error.WriteLine ("Invalid optimization, at {0:X06}", i);
					invalid = true;
				}
			if (!invalid)
				for (int i = m.Start; i <= m.End; i++)
					eaw [i] = '\0';
		}
		// calculate max index. After this, mapping is not generated.
		int maxIndex = 0;
		for (int i = eaw.Length - 1;;i--) {
			if (eaw [i] != '\0') {
				Console.Error.WriteLine ("max EAW index: {0:X06}", i);
				maxIndex = i;
				break;
			}
		}
		using (var fs = File.CreateText (output)) {
			for (int i = 0; i <= maxIndex; i++)
				fs.Write (eaw [i]);
		}
		using (var fs = File.CreateText (mapoutput)) {
			string allRanges = string.Join (";", ranges.Concat (optmap)
				.OrderBy (m => m.Start)
				.Select (m => string.Format ("{0:X06}-{1:X06}={2}", m.Start, m.End, m.Value))
				);
			fs.WriteLine (allRanges);
		}
	}
}

