using System;
using NUnicode.Database;
using NUnit.Framework;

namespace NUnicode.Database.Tests
{
	[TestFixture]
	public class EastAsianWidthTest
	{
		[Test]
		public void GetKind ()
		{
			Func<char,EastAsianWidthKind> f = EastAsianWidth.GetKind;
			Action<char,EastAsianWidthKind> t = (c, ret) => Assert.AreEqual (ret, f (c), string.Format ("{0:X06}", (int) c));
			Func<int,EastAsianWidthKind> f2 = EastAsianWidth.GetKind;
			Action<int,EastAsianWidthKind> t2 = (c, ret) => Assert.AreEqual (ret, f2 (c), string.Format ("{0:X06}", (int) c));

			t ('\0', EastAsianWidthKind.Neutral);
			t (' ', EastAsianWidthKind.Narrow);
			t ('\u1100', EastAsianWidthKind.Wide);
			t ('\u00A1', EastAsianWidthKind.Ambiguous);
			t2 (0xE007F, EastAsianWidthKind.Neutral);
			t2 (0xE01EF, EastAsianWidthKind.Ambiguous);
		}

		[Test]
		public void IsFullWidth ()
		{
			Func<char,bool,bool> f = EastAsianWidth.IsFullWidth;
			Action<char,bool,bool> t = (c, isA, ret) => Assert.AreEqual (ret, f (c, isA), string.Format ("{0:X06}-isAsian:{1}", (int) c, isA));
			Func<int,bool,bool> f2 = EastAsianWidth.IsFullWidth;
			Action<int,bool,bool> t2 = (c, isA, ret) => Assert.AreEqual (ret, f2 (c, isA), string.Format ("{0:X06}-isAsian:{1}", (int) c, isA));

			t ('0', false, false);
			t ('0', true, false);
			t (' ', false, false);
			t (' ', true, false);
			t ('\u00A1', false, false);
			t ('\u00A1', true, true);
			t ('\u1100', false, true);
			t ('\u1100', true, true);
			t2 (0xE007F, false, false);
			t2 (0xE007F, true, false);
			t2 (0xE01EF, false, false);
			t2 (0xE01EF, true, true);
		}
	}
}

