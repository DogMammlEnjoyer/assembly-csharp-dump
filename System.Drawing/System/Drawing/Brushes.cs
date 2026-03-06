using System;

namespace System.Drawing
{
	/// <summary>Brushes for all the standard colors. This class cannot be inherited.</summary>
	public sealed class Brushes
	{
		private Brushes()
		{
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush AliceBlue
		{
			get
			{
				if (Brushes.aliceBlue == null)
				{
					Brushes.aliceBlue = new SolidBrush(Color.AliceBlue);
				}
				return Brushes.aliceBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush AntiqueWhite
		{
			get
			{
				if (Brushes.antiqueWhite == null)
				{
					Brushes.antiqueWhite = new SolidBrush(Color.AntiqueWhite);
				}
				return Brushes.antiqueWhite;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Aqua
		{
			get
			{
				if (Brushes.aqua == null)
				{
					Brushes.aqua = new SolidBrush(Color.Aqua);
				}
				return Brushes.aqua;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Aquamarine
		{
			get
			{
				if (Brushes.aquamarine == null)
				{
					Brushes.aquamarine = new SolidBrush(Color.Aquamarine);
				}
				return Brushes.aquamarine;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Azure
		{
			get
			{
				if (Brushes.azure == null)
				{
					Brushes.azure = new SolidBrush(Color.Azure);
				}
				return Brushes.azure;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Beige
		{
			get
			{
				if (Brushes.beige == null)
				{
					Brushes.beige = new SolidBrush(Color.Beige);
				}
				return Brushes.beige;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Bisque
		{
			get
			{
				if (Brushes.bisque == null)
				{
					Brushes.bisque = new SolidBrush(Color.Bisque);
				}
				return Brushes.bisque;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Black
		{
			get
			{
				if (Brushes.black == null)
				{
					Brushes.black = new SolidBrush(Color.Black);
				}
				return Brushes.black;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush BlanchedAlmond
		{
			get
			{
				if (Brushes.blanchedAlmond == null)
				{
					Brushes.blanchedAlmond = new SolidBrush(Color.BlanchedAlmond);
				}
				return Brushes.blanchedAlmond;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Blue
		{
			get
			{
				if (Brushes.blue == null)
				{
					Brushes.blue = new SolidBrush(Color.Blue);
				}
				return Brushes.blue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush BlueViolet
		{
			get
			{
				if (Brushes.blueViolet == null)
				{
					Brushes.blueViolet = new SolidBrush(Color.BlueViolet);
				}
				return Brushes.blueViolet;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Brown
		{
			get
			{
				if (Brushes.brown == null)
				{
					Brushes.brown = new SolidBrush(Color.Brown);
				}
				return Brushes.brown;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush BurlyWood
		{
			get
			{
				if (Brushes.burlyWood == null)
				{
					Brushes.burlyWood = new SolidBrush(Color.BurlyWood);
				}
				return Brushes.burlyWood;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush CadetBlue
		{
			get
			{
				if (Brushes.cadetBlue == null)
				{
					Brushes.cadetBlue = new SolidBrush(Color.CadetBlue);
				}
				return Brushes.cadetBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Chartreuse
		{
			get
			{
				if (Brushes.chartreuse == null)
				{
					Brushes.chartreuse = new SolidBrush(Color.Chartreuse);
				}
				return Brushes.chartreuse;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Chocolate
		{
			get
			{
				if (Brushes.chocolate == null)
				{
					Brushes.chocolate = new SolidBrush(Color.Chocolate);
				}
				return Brushes.chocolate;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Coral
		{
			get
			{
				if (Brushes.coral == null)
				{
					Brushes.coral = new SolidBrush(Color.Coral);
				}
				return Brushes.coral;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush CornflowerBlue
		{
			get
			{
				if (Brushes.cornflowerBlue == null)
				{
					Brushes.cornflowerBlue = new SolidBrush(Color.CornflowerBlue);
				}
				return Brushes.cornflowerBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Cornsilk
		{
			get
			{
				if (Brushes.cornsilk == null)
				{
					Brushes.cornsilk = new SolidBrush(Color.Cornsilk);
				}
				return Brushes.cornsilk;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Crimson
		{
			get
			{
				if (Brushes.crimson == null)
				{
					Brushes.crimson = new SolidBrush(Color.Crimson);
				}
				return Brushes.crimson;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Cyan
		{
			get
			{
				if (Brushes.cyan == null)
				{
					Brushes.cyan = new SolidBrush(Color.Cyan);
				}
				return Brushes.cyan;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkBlue
		{
			get
			{
				if (Brushes.darkBlue == null)
				{
					Brushes.darkBlue = new SolidBrush(Color.DarkBlue);
				}
				return Brushes.darkBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkCyan
		{
			get
			{
				if (Brushes.darkCyan == null)
				{
					Brushes.darkCyan = new SolidBrush(Color.DarkCyan);
				}
				return Brushes.darkCyan;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkGoldenrod
		{
			get
			{
				if (Brushes.darkGoldenrod == null)
				{
					Brushes.darkGoldenrod = new SolidBrush(Color.DarkGoldenrod);
				}
				return Brushes.darkGoldenrod;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkGray
		{
			get
			{
				if (Brushes.darkGray == null)
				{
					Brushes.darkGray = new SolidBrush(Color.DarkGray);
				}
				return Brushes.darkGray;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkGreen
		{
			get
			{
				if (Brushes.darkGreen == null)
				{
					Brushes.darkGreen = new SolidBrush(Color.DarkGreen);
				}
				return Brushes.darkGreen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkKhaki
		{
			get
			{
				if (Brushes.darkKhaki == null)
				{
					Brushes.darkKhaki = new SolidBrush(Color.DarkKhaki);
				}
				return Brushes.darkKhaki;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkMagenta
		{
			get
			{
				if (Brushes.darkMagenta == null)
				{
					Brushes.darkMagenta = new SolidBrush(Color.DarkMagenta);
				}
				return Brushes.darkMagenta;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkOliveGreen
		{
			get
			{
				if (Brushes.darkOliveGreen == null)
				{
					Brushes.darkOliveGreen = new SolidBrush(Color.DarkOliveGreen);
				}
				return Brushes.darkOliveGreen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkOrange
		{
			get
			{
				if (Brushes.darkOrange == null)
				{
					Brushes.darkOrange = new SolidBrush(Color.DarkOrange);
				}
				return Brushes.darkOrange;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkOrchid
		{
			get
			{
				if (Brushes.darkOrchid == null)
				{
					Brushes.darkOrchid = new SolidBrush(Color.DarkOrchid);
				}
				return Brushes.darkOrchid;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkRed
		{
			get
			{
				if (Brushes.darkRed == null)
				{
					Brushes.darkRed = new SolidBrush(Color.DarkRed);
				}
				return Brushes.darkRed;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkSalmon
		{
			get
			{
				if (Brushes.darkSalmon == null)
				{
					Brushes.darkSalmon = new SolidBrush(Color.DarkSalmon);
				}
				return Brushes.darkSalmon;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkSeaGreen
		{
			get
			{
				if (Brushes.darkSeaGreen == null)
				{
					Brushes.darkSeaGreen = new SolidBrush(Color.DarkSeaGreen);
				}
				return Brushes.darkSeaGreen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkSlateBlue
		{
			get
			{
				if (Brushes.darkSlateBlue == null)
				{
					Brushes.darkSlateBlue = new SolidBrush(Color.DarkSlateBlue);
				}
				return Brushes.darkSlateBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkSlateGray
		{
			get
			{
				if (Brushes.darkSlateGray == null)
				{
					Brushes.darkSlateGray = new SolidBrush(Color.DarkSlateGray);
				}
				return Brushes.darkSlateGray;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkTurquoise
		{
			get
			{
				if (Brushes.darkTurquoise == null)
				{
					Brushes.darkTurquoise = new SolidBrush(Color.DarkTurquoise);
				}
				return Brushes.darkTurquoise;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DarkViolet
		{
			get
			{
				if (Brushes.darkViolet == null)
				{
					Brushes.darkViolet = new SolidBrush(Color.DarkViolet);
				}
				return Brushes.darkViolet;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DeepPink
		{
			get
			{
				if (Brushes.deepPink == null)
				{
					Brushes.deepPink = new SolidBrush(Color.DeepPink);
				}
				return Brushes.deepPink;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DeepSkyBlue
		{
			get
			{
				if (Brushes.deepSkyBlue == null)
				{
					Brushes.deepSkyBlue = new SolidBrush(Color.DeepSkyBlue);
				}
				return Brushes.deepSkyBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DimGray
		{
			get
			{
				if (Brushes.dimGray == null)
				{
					Brushes.dimGray = new SolidBrush(Color.DimGray);
				}
				return Brushes.dimGray;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush DodgerBlue
		{
			get
			{
				if (Brushes.dodgerBlue == null)
				{
					Brushes.dodgerBlue = new SolidBrush(Color.DodgerBlue);
				}
				return Brushes.dodgerBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Firebrick
		{
			get
			{
				if (Brushes.firebrick == null)
				{
					Brushes.firebrick = new SolidBrush(Color.Firebrick);
				}
				return Brushes.firebrick;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush FloralWhite
		{
			get
			{
				if (Brushes.floralWhite == null)
				{
					Brushes.floralWhite = new SolidBrush(Color.FloralWhite);
				}
				return Brushes.floralWhite;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush ForestGreen
		{
			get
			{
				if (Brushes.forestGreen == null)
				{
					Brushes.forestGreen = new SolidBrush(Color.ForestGreen);
				}
				return Brushes.forestGreen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Fuchsia
		{
			get
			{
				if (Brushes.fuchsia == null)
				{
					Brushes.fuchsia = new SolidBrush(Color.Fuchsia);
				}
				return Brushes.fuchsia;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Gainsboro
		{
			get
			{
				if (Brushes.gainsboro == null)
				{
					Brushes.gainsboro = new SolidBrush(Color.Gainsboro);
				}
				return Brushes.gainsboro;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush GhostWhite
		{
			get
			{
				if (Brushes.ghostWhite == null)
				{
					Brushes.ghostWhite = new SolidBrush(Color.GhostWhite);
				}
				return Brushes.ghostWhite;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Gold
		{
			get
			{
				if (Brushes.gold == null)
				{
					Brushes.gold = new SolidBrush(Color.Gold);
				}
				return Brushes.gold;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Goldenrod
		{
			get
			{
				if (Brushes.goldenrod == null)
				{
					Brushes.goldenrod = new SolidBrush(Color.Goldenrod);
				}
				return Brushes.goldenrod;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Gray
		{
			get
			{
				if (Brushes.gray == null)
				{
					Brushes.gray = new SolidBrush(Color.Gray);
				}
				return Brushes.gray;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Green
		{
			get
			{
				if (Brushes.green == null)
				{
					Brushes.green = new SolidBrush(Color.Green);
				}
				return Brushes.green;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush GreenYellow
		{
			get
			{
				if (Brushes.greenYellow == null)
				{
					Brushes.greenYellow = new SolidBrush(Color.GreenYellow);
				}
				return Brushes.greenYellow;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Honeydew
		{
			get
			{
				if (Brushes.honeydew == null)
				{
					Brushes.honeydew = new SolidBrush(Color.Honeydew);
				}
				return Brushes.honeydew;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush HotPink
		{
			get
			{
				if (Brushes.hotPink == null)
				{
					Brushes.hotPink = new SolidBrush(Color.HotPink);
				}
				return Brushes.hotPink;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush IndianRed
		{
			get
			{
				if (Brushes.indianRed == null)
				{
					Brushes.indianRed = new SolidBrush(Color.IndianRed);
				}
				return Brushes.indianRed;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Indigo
		{
			get
			{
				if (Brushes.indigo == null)
				{
					Brushes.indigo = new SolidBrush(Color.Indigo);
				}
				return Brushes.indigo;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Ivory
		{
			get
			{
				if (Brushes.ivory == null)
				{
					Brushes.ivory = new SolidBrush(Color.Ivory);
				}
				return Brushes.ivory;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Khaki
		{
			get
			{
				if (Brushes.khaki == null)
				{
					Brushes.khaki = new SolidBrush(Color.Khaki);
				}
				return Brushes.khaki;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Lavender
		{
			get
			{
				if (Brushes.lavender == null)
				{
					Brushes.lavender = new SolidBrush(Color.Lavender);
				}
				return Brushes.lavender;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LavenderBlush
		{
			get
			{
				if (Brushes.lavenderBlush == null)
				{
					Brushes.lavenderBlush = new SolidBrush(Color.LavenderBlush);
				}
				return Brushes.lavenderBlush;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LawnGreen
		{
			get
			{
				if (Brushes.lawnGreen == null)
				{
					Brushes.lawnGreen = new SolidBrush(Color.LawnGreen);
				}
				return Brushes.lawnGreen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LemonChiffon
		{
			get
			{
				if (Brushes.lemonChiffon == null)
				{
					Brushes.lemonChiffon = new SolidBrush(Color.LemonChiffon);
				}
				return Brushes.lemonChiffon;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LightBlue
		{
			get
			{
				if (Brushes.lightBlue == null)
				{
					Brushes.lightBlue = new SolidBrush(Color.LightBlue);
				}
				return Brushes.lightBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LightCoral
		{
			get
			{
				if (Brushes.lightCoral == null)
				{
					Brushes.lightCoral = new SolidBrush(Color.LightCoral);
				}
				return Brushes.lightCoral;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LightCyan
		{
			get
			{
				if (Brushes.lightCyan == null)
				{
					Brushes.lightCyan = new SolidBrush(Color.LightCyan);
				}
				return Brushes.lightCyan;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LightGoldenrodYellow
		{
			get
			{
				if (Brushes.lightGoldenrodYellow == null)
				{
					Brushes.lightGoldenrodYellow = new SolidBrush(Color.LightGoldenrodYellow);
				}
				return Brushes.lightGoldenrodYellow;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LightGray
		{
			get
			{
				if (Brushes.lightGray == null)
				{
					Brushes.lightGray = new SolidBrush(Color.LightGray);
				}
				return Brushes.lightGray;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LightGreen
		{
			get
			{
				if (Brushes.lightGreen == null)
				{
					Brushes.lightGreen = new SolidBrush(Color.LightGreen);
				}
				return Brushes.lightGreen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LightPink
		{
			get
			{
				if (Brushes.lightPink == null)
				{
					Brushes.lightPink = new SolidBrush(Color.LightPink);
				}
				return Brushes.lightPink;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LightSalmon
		{
			get
			{
				if (Brushes.lightSalmon == null)
				{
					Brushes.lightSalmon = new SolidBrush(Color.LightSalmon);
				}
				return Brushes.lightSalmon;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LightSeaGreen
		{
			get
			{
				if (Brushes.lightSeaGreen == null)
				{
					Brushes.lightSeaGreen = new SolidBrush(Color.LightSeaGreen);
				}
				return Brushes.lightSeaGreen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LightSkyBlue
		{
			get
			{
				if (Brushes.lightSkyBlue == null)
				{
					Brushes.lightSkyBlue = new SolidBrush(Color.LightSkyBlue);
				}
				return Brushes.lightSkyBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LightSlateGray
		{
			get
			{
				if (Brushes.lightSlateGray == null)
				{
					Brushes.lightSlateGray = new SolidBrush(Color.LightSlateGray);
				}
				return Brushes.lightSlateGray;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LightSteelBlue
		{
			get
			{
				if (Brushes.lightSteelBlue == null)
				{
					Brushes.lightSteelBlue = new SolidBrush(Color.LightSteelBlue);
				}
				return Brushes.lightSteelBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LightYellow
		{
			get
			{
				if (Brushes.lightYellow == null)
				{
					Brushes.lightYellow = new SolidBrush(Color.LightYellow);
				}
				return Brushes.lightYellow;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Lime
		{
			get
			{
				if (Brushes.lime == null)
				{
					Brushes.lime = new SolidBrush(Color.Lime);
				}
				return Brushes.lime;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush LimeGreen
		{
			get
			{
				if (Brushes.limeGreen == null)
				{
					Brushes.limeGreen = new SolidBrush(Color.LimeGreen);
				}
				return Brushes.limeGreen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Linen
		{
			get
			{
				if (Brushes.linen == null)
				{
					Brushes.linen = new SolidBrush(Color.Linen);
				}
				return Brushes.linen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Magenta
		{
			get
			{
				if (Brushes.magenta == null)
				{
					Brushes.magenta = new SolidBrush(Color.Magenta);
				}
				return Brushes.magenta;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Maroon
		{
			get
			{
				if (Brushes.maroon == null)
				{
					Brushes.maroon = new SolidBrush(Color.Maroon);
				}
				return Brushes.maroon;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush MediumAquamarine
		{
			get
			{
				if (Brushes.mediumAquamarine == null)
				{
					Brushes.mediumAquamarine = new SolidBrush(Color.MediumAquamarine);
				}
				return Brushes.mediumAquamarine;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush MediumBlue
		{
			get
			{
				if (Brushes.mediumBlue == null)
				{
					Brushes.mediumBlue = new SolidBrush(Color.MediumBlue);
				}
				return Brushes.mediumBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush MediumOrchid
		{
			get
			{
				if (Brushes.mediumOrchid == null)
				{
					Brushes.mediumOrchid = new SolidBrush(Color.MediumOrchid);
				}
				return Brushes.mediumOrchid;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush MediumPurple
		{
			get
			{
				if (Brushes.mediumPurple == null)
				{
					Brushes.mediumPurple = new SolidBrush(Color.MediumPurple);
				}
				return Brushes.mediumPurple;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush MediumSeaGreen
		{
			get
			{
				if (Brushes.mediumSeaGreen == null)
				{
					Brushes.mediumSeaGreen = new SolidBrush(Color.MediumSeaGreen);
				}
				return Brushes.mediumSeaGreen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush MediumSlateBlue
		{
			get
			{
				if (Brushes.mediumSlateBlue == null)
				{
					Brushes.mediumSlateBlue = new SolidBrush(Color.MediumSlateBlue);
				}
				return Brushes.mediumSlateBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush MediumSpringGreen
		{
			get
			{
				if (Brushes.mediumSpringGreen == null)
				{
					Brushes.mediumSpringGreen = new SolidBrush(Color.MediumSpringGreen);
				}
				return Brushes.mediumSpringGreen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush MediumTurquoise
		{
			get
			{
				if (Brushes.mediumTurquoise == null)
				{
					Brushes.mediumTurquoise = new SolidBrush(Color.MediumTurquoise);
				}
				return Brushes.mediumTurquoise;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush MediumVioletRed
		{
			get
			{
				if (Brushes.mediumVioletRed == null)
				{
					Brushes.mediumVioletRed = new SolidBrush(Color.MediumVioletRed);
				}
				return Brushes.mediumVioletRed;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush MidnightBlue
		{
			get
			{
				if (Brushes.midnightBlue == null)
				{
					Brushes.midnightBlue = new SolidBrush(Color.MidnightBlue);
				}
				return Brushes.midnightBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush MintCream
		{
			get
			{
				if (Brushes.mintCream == null)
				{
					Brushes.mintCream = new SolidBrush(Color.MintCream);
				}
				return Brushes.mintCream;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush MistyRose
		{
			get
			{
				if (Brushes.mistyRose == null)
				{
					Brushes.mistyRose = new SolidBrush(Color.MistyRose);
				}
				return Brushes.mistyRose;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Moccasin
		{
			get
			{
				if (Brushes.moccasin == null)
				{
					Brushes.moccasin = new SolidBrush(Color.Moccasin);
				}
				return Brushes.moccasin;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush NavajoWhite
		{
			get
			{
				if (Brushes.navajoWhite == null)
				{
					Brushes.navajoWhite = new SolidBrush(Color.NavajoWhite);
				}
				return Brushes.navajoWhite;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Navy
		{
			get
			{
				if (Brushes.navy == null)
				{
					Brushes.navy = new SolidBrush(Color.Navy);
				}
				return Brushes.navy;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush OldLace
		{
			get
			{
				if (Brushes.oldLace == null)
				{
					Brushes.oldLace = new SolidBrush(Color.OldLace);
				}
				return Brushes.oldLace;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Olive
		{
			get
			{
				if (Brushes.olive == null)
				{
					Brushes.olive = new SolidBrush(Color.Olive);
				}
				return Brushes.olive;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush OliveDrab
		{
			get
			{
				if (Brushes.oliveDrab == null)
				{
					Brushes.oliveDrab = new SolidBrush(Color.OliveDrab);
				}
				return Brushes.oliveDrab;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Orange
		{
			get
			{
				if (Brushes.orange == null)
				{
					Brushes.orange = new SolidBrush(Color.Orange);
				}
				return Brushes.orange;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush OrangeRed
		{
			get
			{
				if (Brushes.orangeRed == null)
				{
					Brushes.orangeRed = new SolidBrush(Color.OrangeRed);
				}
				return Brushes.orangeRed;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Orchid
		{
			get
			{
				if (Brushes.orchid == null)
				{
					Brushes.orchid = new SolidBrush(Color.Orchid);
				}
				return Brushes.orchid;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush PaleGoldenrod
		{
			get
			{
				if (Brushes.paleGoldenrod == null)
				{
					Brushes.paleGoldenrod = new SolidBrush(Color.PaleGoldenrod);
				}
				return Brushes.paleGoldenrod;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush PaleGreen
		{
			get
			{
				if (Brushes.paleGreen == null)
				{
					Brushes.paleGreen = new SolidBrush(Color.PaleGreen);
				}
				return Brushes.paleGreen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush PaleTurquoise
		{
			get
			{
				if (Brushes.paleTurquoise == null)
				{
					Brushes.paleTurquoise = new SolidBrush(Color.PaleTurquoise);
				}
				return Brushes.paleTurquoise;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush PaleVioletRed
		{
			get
			{
				if (Brushes.paleVioletRed == null)
				{
					Brushes.paleVioletRed = new SolidBrush(Color.PaleVioletRed);
				}
				return Brushes.paleVioletRed;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush PapayaWhip
		{
			get
			{
				if (Brushes.papayaWhip == null)
				{
					Brushes.papayaWhip = new SolidBrush(Color.PapayaWhip);
				}
				return Brushes.papayaWhip;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush PeachPuff
		{
			get
			{
				if (Brushes.peachPuff == null)
				{
					Brushes.peachPuff = new SolidBrush(Color.PeachPuff);
				}
				return Brushes.peachPuff;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Peru
		{
			get
			{
				if (Brushes.peru == null)
				{
					Brushes.peru = new SolidBrush(Color.Peru);
				}
				return Brushes.peru;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Pink
		{
			get
			{
				if (Brushes.pink == null)
				{
					Brushes.pink = new SolidBrush(Color.Pink);
				}
				return Brushes.pink;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Plum
		{
			get
			{
				if (Brushes.plum == null)
				{
					Brushes.plum = new SolidBrush(Color.Plum);
				}
				return Brushes.plum;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush PowderBlue
		{
			get
			{
				if (Brushes.powderBlue == null)
				{
					Brushes.powderBlue = new SolidBrush(Color.PowderBlue);
				}
				return Brushes.powderBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Purple
		{
			get
			{
				if (Brushes.purple == null)
				{
					Brushes.purple = new SolidBrush(Color.Purple);
				}
				return Brushes.purple;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Red
		{
			get
			{
				if (Brushes.red == null)
				{
					Brushes.red = new SolidBrush(Color.Red);
				}
				return Brushes.red;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush RosyBrown
		{
			get
			{
				if (Brushes.rosyBrown == null)
				{
					Brushes.rosyBrown = new SolidBrush(Color.RosyBrown);
				}
				return Brushes.rosyBrown;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush RoyalBlue
		{
			get
			{
				if (Brushes.royalBlue == null)
				{
					Brushes.royalBlue = new SolidBrush(Color.RoyalBlue);
				}
				return Brushes.royalBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush SaddleBrown
		{
			get
			{
				if (Brushes.saddleBrown == null)
				{
					Brushes.saddleBrown = new SolidBrush(Color.SaddleBrown);
				}
				return Brushes.saddleBrown;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Salmon
		{
			get
			{
				if (Brushes.salmon == null)
				{
					Brushes.salmon = new SolidBrush(Color.Salmon);
				}
				return Brushes.salmon;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush SandyBrown
		{
			get
			{
				if (Brushes.sandyBrown == null)
				{
					Brushes.sandyBrown = new SolidBrush(Color.SandyBrown);
				}
				return Brushes.sandyBrown;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush SeaGreen
		{
			get
			{
				if (Brushes.seaGreen == null)
				{
					Brushes.seaGreen = new SolidBrush(Color.SeaGreen);
				}
				return Brushes.seaGreen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush SeaShell
		{
			get
			{
				if (Brushes.seaShell == null)
				{
					Brushes.seaShell = new SolidBrush(Color.SeaShell);
				}
				return Brushes.seaShell;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Sienna
		{
			get
			{
				if (Brushes.sienna == null)
				{
					Brushes.sienna = new SolidBrush(Color.Sienna);
				}
				return Brushes.sienna;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Silver
		{
			get
			{
				if (Brushes.silver == null)
				{
					Brushes.silver = new SolidBrush(Color.Silver);
				}
				return Brushes.silver;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush SkyBlue
		{
			get
			{
				if (Brushes.skyBlue == null)
				{
					Brushes.skyBlue = new SolidBrush(Color.SkyBlue);
				}
				return Brushes.skyBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush SlateBlue
		{
			get
			{
				if (Brushes.slateBlue == null)
				{
					Brushes.slateBlue = new SolidBrush(Color.SlateBlue);
				}
				return Brushes.slateBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush SlateGray
		{
			get
			{
				if (Brushes.slateGray == null)
				{
					Brushes.slateGray = new SolidBrush(Color.SlateGray);
				}
				return Brushes.slateGray;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Snow
		{
			get
			{
				if (Brushes.snow == null)
				{
					Brushes.snow = new SolidBrush(Color.Snow);
				}
				return Brushes.snow;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush SpringGreen
		{
			get
			{
				if (Brushes.springGreen == null)
				{
					Brushes.springGreen = new SolidBrush(Color.SpringGreen);
				}
				return Brushes.springGreen;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush SteelBlue
		{
			get
			{
				if (Brushes.steelBlue == null)
				{
					Brushes.steelBlue = new SolidBrush(Color.SteelBlue);
				}
				return Brushes.steelBlue;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Tan
		{
			get
			{
				if (Brushes.tan == null)
				{
					Brushes.tan = new SolidBrush(Color.Tan);
				}
				return Brushes.tan;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Teal
		{
			get
			{
				if (Brushes.teal == null)
				{
					Brushes.teal = new SolidBrush(Color.Teal);
				}
				return Brushes.teal;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Thistle
		{
			get
			{
				if (Brushes.thistle == null)
				{
					Brushes.thistle = new SolidBrush(Color.Thistle);
				}
				return Brushes.thistle;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Tomato
		{
			get
			{
				if (Brushes.tomato == null)
				{
					Brushes.tomato = new SolidBrush(Color.Tomato);
				}
				return Brushes.tomato;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Transparent
		{
			get
			{
				if (Brushes.transparent == null)
				{
					Brushes.transparent = new SolidBrush(Color.Transparent);
				}
				return Brushes.transparent;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Turquoise
		{
			get
			{
				if (Brushes.turquoise == null)
				{
					Brushes.turquoise = new SolidBrush(Color.Turquoise);
				}
				return Brushes.turquoise;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Violet
		{
			get
			{
				if (Brushes.violet == null)
				{
					Brushes.violet = new SolidBrush(Color.Violet);
				}
				return Brushes.violet;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Wheat
		{
			get
			{
				if (Brushes.wheat == null)
				{
					Brushes.wheat = new SolidBrush(Color.Wheat);
				}
				return Brushes.wheat;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush White
		{
			get
			{
				if (Brushes.white == null)
				{
					Brushes.white = new SolidBrush(Color.White);
				}
				return Brushes.white;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush WhiteSmoke
		{
			get
			{
				if (Brushes.whiteSmoke == null)
				{
					Brushes.whiteSmoke = new SolidBrush(Color.WhiteSmoke);
				}
				return Brushes.whiteSmoke;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush Yellow
		{
			get
			{
				if (Brushes.yellow == null)
				{
					Brushes.yellow = new SolidBrush(Color.Yellow);
				}
				return Brushes.yellow;
			}
		}

		/// <summary>Gets a system-defined <see cref="T:System.Drawing.Brush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Brush" /> object set to a system-defined color.</returns>
		public static Brush YellowGreen
		{
			get
			{
				if (Brushes.yellowGreen == null)
				{
					Brushes.yellowGreen = new SolidBrush(Color.YellowGreen);
				}
				return Brushes.yellowGreen;
			}
		}

		private static SolidBrush aliceBlue;

		private static SolidBrush antiqueWhite;

		private static SolidBrush aqua;

		private static SolidBrush aquamarine;

		private static SolidBrush azure;

		private static SolidBrush beige;

		private static SolidBrush bisque;

		private static SolidBrush black;

		private static SolidBrush blanchedAlmond;

		private static SolidBrush blue;

		private static SolidBrush blueViolet;

		private static SolidBrush brown;

		private static SolidBrush burlyWood;

		private static SolidBrush cadetBlue;

		private static SolidBrush chartreuse;

		private static SolidBrush chocolate;

		private static SolidBrush coral;

		private static SolidBrush cornflowerBlue;

		private static SolidBrush cornsilk;

		private static SolidBrush crimson;

		private static SolidBrush cyan;

		private static SolidBrush darkBlue;

		private static SolidBrush darkCyan;

		private static SolidBrush darkGoldenrod;

		private static SolidBrush darkGray;

		private static SolidBrush darkGreen;

		private static SolidBrush darkKhaki;

		private static SolidBrush darkMagenta;

		private static SolidBrush darkOliveGreen;

		private static SolidBrush darkOrange;

		private static SolidBrush darkOrchid;

		private static SolidBrush darkRed;

		private static SolidBrush darkSalmon;

		private static SolidBrush darkSeaGreen;

		private static SolidBrush darkSlateBlue;

		private static SolidBrush darkSlateGray;

		private static SolidBrush darkTurquoise;

		private static SolidBrush darkViolet;

		private static SolidBrush deepPink;

		private static SolidBrush deepSkyBlue;

		private static SolidBrush dimGray;

		private static SolidBrush dodgerBlue;

		private static SolidBrush firebrick;

		private static SolidBrush floralWhite;

		private static SolidBrush forestGreen;

		private static SolidBrush fuchsia;

		private static SolidBrush gainsboro;

		private static SolidBrush ghostWhite;

		private static SolidBrush gold;

		private static SolidBrush goldenrod;

		private static SolidBrush gray;

		private static SolidBrush green;

		private static SolidBrush greenYellow;

		private static SolidBrush honeydew;

		private static SolidBrush hotPink;

		private static SolidBrush indianRed;

		private static SolidBrush indigo;

		private static SolidBrush ivory;

		private static SolidBrush khaki;

		private static SolidBrush lavender;

		private static SolidBrush lavenderBlush;

		private static SolidBrush lawnGreen;

		private static SolidBrush lemonChiffon;

		private static SolidBrush lightBlue;

		private static SolidBrush lightCoral;

		private static SolidBrush lightCyan;

		private static SolidBrush lightGoldenrodYellow;

		private static SolidBrush lightGray;

		private static SolidBrush lightGreen;

		private static SolidBrush lightPink;

		private static SolidBrush lightSalmon;

		private static SolidBrush lightSeaGreen;

		private static SolidBrush lightSkyBlue;

		private static SolidBrush lightSlateGray;

		private static SolidBrush lightSteelBlue;

		private static SolidBrush lightYellow;

		private static SolidBrush lime;

		private static SolidBrush limeGreen;

		private static SolidBrush linen;

		private static SolidBrush magenta;

		private static SolidBrush maroon;

		private static SolidBrush mediumAquamarine;

		private static SolidBrush mediumBlue;

		private static SolidBrush mediumOrchid;

		private static SolidBrush mediumPurple;

		private static SolidBrush mediumSeaGreen;

		private static SolidBrush mediumSlateBlue;

		private static SolidBrush mediumSpringGreen;

		private static SolidBrush mediumTurquoise;

		private static SolidBrush mediumVioletRed;

		private static SolidBrush midnightBlue;

		private static SolidBrush mintCream;

		private static SolidBrush mistyRose;

		private static SolidBrush moccasin;

		private static SolidBrush navajoWhite;

		private static SolidBrush navy;

		private static SolidBrush oldLace;

		private static SolidBrush olive;

		private static SolidBrush oliveDrab;

		private static SolidBrush orange;

		private static SolidBrush orangeRed;

		private static SolidBrush orchid;

		private static SolidBrush paleGoldenrod;

		private static SolidBrush paleGreen;

		private static SolidBrush paleTurquoise;

		private static SolidBrush paleVioletRed;

		private static SolidBrush papayaWhip;

		private static SolidBrush peachPuff;

		private static SolidBrush peru;

		private static SolidBrush pink;

		private static SolidBrush plum;

		private static SolidBrush powderBlue;

		private static SolidBrush purple;

		private static SolidBrush red;

		private static SolidBrush rosyBrown;

		private static SolidBrush royalBlue;

		private static SolidBrush saddleBrown;

		private static SolidBrush salmon;

		private static SolidBrush sandyBrown;

		private static SolidBrush seaGreen;

		private static SolidBrush seaShell;

		private static SolidBrush sienna;

		private static SolidBrush silver;

		private static SolidBrush skyBlue;

		private static SolidBrush slateBlue;

		private static SolidBrush slateGray;

		private static SolidBrush snow;

		private static SolidBrush springGreen;

		private static SolidBrush steelBlue;

		private static SolidBrush tan;

		private static SolidBrush teal;

		private static SolidBrush thistle;

		private static SolidBrush tomato;

		private static SolidBrush transparent;

		private static SolidBrush turquoise;

		private static SolidBrush violet;

		private static SolidBrush wheat;

		private static SolidBrush white;

		private static SolidBrush whiteSmoke;

		private static SolidBrush yellow;

		private static SolidBrush yellowGreen;
	}
}
