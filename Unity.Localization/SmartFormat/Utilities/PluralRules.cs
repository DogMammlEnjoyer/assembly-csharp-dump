using System;
using System.Collections.Generic;

namespace UnityEngine.Localization.SmartFormat.Utilities
{
	public static class PluralRules
	{
		private static PluralRules.PluralRuleDelegate Singular
		{
			get
			{
				return (decimal n, int c) => 0;
			}
		}

		private static PluralRules.PluralRuleDelegate DualOneOther
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (c == 2)
					{
						if (!(n == 1m))
						{
							return 1;
						}
						return 0;
					}
					else if (c == 3)
					{
						if (n == 0m)
						{
							return 0;
						}
						if (!(n == 1m))
						{
							return 2;
						}
						return 1;
					}
					else
					{
						if (c != 4)
						{
							return -1;
						}
						if (n < 0m)
						{
							return 0;
						}
						if (n == 0m)
						{
							return 1;
						}
						if (!(n == 1m))
						{
							return 3;
						}
						return 2;
					}
				};
			}
		}

		private static PluralRules.PluralRuleDelegate DualWithZero
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (!(n == 0m) && !(n == 1m))
					{
						return 1;
					}
					return 0;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate DualFromZeroToTwo
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (!(n == 0m) && !(n == 1m))
					{
						return 1;
					}
					return 0;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate TripleOneTwoOther
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n == 1m)
					{
						return 0;
					}
					if (!(n == 2m))
					{
						return 2;
					}
					return 1;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate RussianSerboCroatian
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n % 10m == 1m && n % 100m != 11m)
					{
						return 0;
					}
					if (!(n % 10m).Between(2m, 4m) || (n % 100m).Between(12m, 14m))
					{
						return 2;
					}
					return 1;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Arabic
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n == 0m)
					{
						return 0;
					}
					if (n == 1m)
					{
						return 1;
					}
					if (n == 2m)
					{
						return 2;
					}
					if ((n % 100m).Between(3m, 10m))
					{
						return 3;
					}
					if (!(n % 100m).Between(11m, 99m))
					{
						return 5;
					}
					return 4;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Breton
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n == 0m)
					{
						return 0;
					}
					if (n == 1m)
					{
						return 1;
					}
					if (n == 2m)
					{
						return 2;
					}
					if (n == 3m)
					{
						return 3;
					}
					if (!(n == 6m))
					{
						return 5;
					}
					return 4;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Czech
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n == 1m)
					{
						return 0;
					}
					if (!n.Between(2m, 4m))
					{
						return 2;
					}
					return 1;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Welsh
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n == 0m)
					{
						return 0;
					}
					if (n == 1m)
					{
						return 1;
					}
					if (n == 2m)
					{
						return 2;
					}
					if (n == 3m)
					{
						return 3;
					}
					if (!(n == 6m))
					{
						return 5;
					}
					return 4;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Manx
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (!(n % 10m).Between(1m, 2m) && !(n % 20m == 0m))
					{
						return 1;
					}
					return 0;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Langi
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n == 0m)
					{
						return 0;
					}
					if (!(n > 0m) || !(n < 2m))
					{
						return 2;
					}
					return 1;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Lithuanian
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n % 10m == 1m && !(n % 100m).Between(11m, 19m))
					{
						return 0;
					}
					if (!(n % 10m).Between(2m, 9m) || (n % 100m).Between(11m, 19m))
					{
						return 2;
					}
					return 1;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Latvian
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n == 0m)
					{
						return 0;
					}
					if (!(n % 10m == 1m) || !(n % 100m != 11m))
					{
						return 2;
					}
					return 1;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Macedonian
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (!(n % 10m == 1m) || !(n != 11m))
					{
						return 1;
					}
					return 0;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Moldavian
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n == 1m)
					{
						return 0;
					}
					if (!(n == 0m) && (!(n != 1m) || !(n % 100m).Between(1m, 19m)))
					{
						return 2;
					}
					return 1;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Maltese
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n == 1m)
					{
						return 0;
					}
					if (n == 0m || (n % 100m).Between(2m, 10m))
					{
						return 1;
					}
					if (!(n % 100m).Between(11m, 19m))
					{
						return 3;
					}
					return 2;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Polish
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n == 1m)
					{
						return 0;
					}
					if ((n % 10m).Between(2m, 4m) && !(n % 100m).Between(12m, 14m))
					{
						return 1;
					}
					if (!(n % 10m).Between(0m, 1m) && !(n % 10m).Between(5m, 9m) && !(n % 100m).Between(12m, 14m))
					{
						return 3;
					}
					return 2;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Romanian
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n == 1m)
					{
						return 0;
					}
					if (!(n == 0m) && !(n % 100m).Between(1m, 19m))
					{
						return 2;
					}
					return 1;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Tachelhit
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n >= 0m && n <= 1m)
					{
						return 0;
					}
					if (!n.Between(2m, 10m))
					{
						return 2;
					}
					return 1;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Slovak
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n == 1m)
					{
						return 0;
					}
					if (!n.Between(2m, 4m))
					{
						return 2;
					}
					return 1;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate Slovenian
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (n % 100m == 1m)
					{
						return 0;
					}
					if (n % 100m == 2m)
					{
						return 1;
					}
					if (!(n % 100m).Between(3m, 4m))
					{
						return 3;
					}
					return 2;
				};
			}
		}

		private static PluralRules.PluralRuleDelegate CentralMoroccoTamazight
		{
			get
			{
				return delegate(decimal n, int c)
				{
					if (!n.Between(0m, 1m) && !n.Between(11m, 99m))
					{
						return 1;
					}
					return 0;
				};
			}
		}

		public static PluralRules.PluralRuleDelegate GetPluralRule(string twoLetterIsoLanguageName)
		{
			PluralRules.PluralRuleDelegate result;
			if (!PluralRules.IsoLangToDelegate.TryGetValue(twoLetterIsoLanguageName, out result))
			{
				throw new ArgumentException("IsoLangToDelegate not found for " + twoLetterIsoLanguageName, "twoLetterIsoLanguageName");
			}
			return result;
		}

		private static bool Between(this decimal value, decimal min, decimal max)
		{
			return value % 1m == 0m && value >= min && value <= max;
		}

		public static readonly Dictionary<string, PluralRules.PluralRuleDelegate> IsoLangToDelegate = new Dictionary<string, PluralRules.PluralRuleDelegate>
		{
			{
				"az",
				PluralRules.Singular
			},
			{
				"bm",
				PluralRules.Singular
			},
			{
				"bo",
				PluralRules.Singular
			},
			{
				"dz",
				PluralRules.Singular
			},
			{
				"fa",
				PluralRules.Singular
			},
			{
				"hu",
				PluralRules.Singular
			},
			{
				"id",
				PluralRules.Singular
			},
			{
				"ig",
				PluralRules.Singular
			},
			{
				"ii",
				PluralRules.Singular
			},
			{
				"ja",
				PluralRules.Singular
			},
			{
				"jv",
				PluralRules.Singular
			},
			{
				"ka",
				PluralRules.Singular
			},
			{
				"kde",
				PluralRules.Singular
			},
			{
				"kea",
				PluralRules.Singular
			},
			{
				"km",
				PluralRules.Singular
			},
			{
				"kn",
				PluralRules.Singular
			},
			{
				"ko",
				PluralRules.Singular
			},
			{
				"ms",
				PluralRules.Singular
			},
			{
				"my",
				PluralRules.Singular
			},
			{
				"root",
				PluralRules.Singular
			},
			{
				"sah",
				PluralRules.Singular
			},
			{
				"ses",
				PluralRules.Singular
			},
			{
				"sg",
				PluralRules.Singular
			},
			{
				"th",
				PluralRules.Singular
			},
			{
				"to",
				PluralRules.Singular
			},
			{
				"vi",
				PluralRules.Singular
			},
			{
				"wo",
				PluralRules.Singular
			},
			{
				"yo",
				PluralRules.Singular
			},
			{
				"zh",
				PluralRules.Singular
			},
			{
				"af",
				PluralRules.DualOneOther
			},
			{
				"bem",
				PluralRules.DualOneOther
			},
			{
				"bg",
				PluralRules.DualOneOther
			},
			{
				"bn",
				PluralRules.DualOneOther
			},
			{
				"brx",
				PluralRules.DualOneOther
			},
			{
				"ca",
				PluralRules.DualOneOther
			},
			{
				"cgg",
				PluralRules.DualOneOther
			},
			{
				"chr",
				PluralRules.DualOneOther
			},
			{
				"da",
				PluralRules.DualOneOther
			},
			{
				"de",
				PluralRules.DualOneOther
			},
			{
				"dv",
				PluralRules.DualOneOther
			},
			{
				"ee",
				PluralRules.DualOneOther
			},
			{
				"el",
				PluralRules.DualOneOther
			},
			{
				"en",
				PluralRules.DualOneOther
			},
			{
				"eo",
				PluralRules.DualOneOther
			},
			{
				"es",
				PluralRules.DualOneOther
			},
			{
				"et",
				PluralRules.DualOneOther
			},
			{
				"eu",
				PluralRules.DualOneOther
			},
			{
				"fi",
				PluralRules.DualOneOther
			},
			{
				"fo",
				PluralRules.DualOneOther
			},
			{
				"fur",
				PluralRules.DualOneOther
			},
			{
				"fy",
				PluralRules.DualOneOther
			},
			{
				"gl",
				PluralRules.DualOneOther
			},
			{
				"gsw",
				PluralRules.DualOneOther
			},
			{
				"gu",
				PluralRules.DualOneOther
			},
			{
				"ha",
				PluralRules.DualOneOther
			},
			{
				"haw",
				PluralRules.DualOneOther
			},
			{
				"he",
				PluralRules.DualOneOther
			},
			{
				"is",
				PluralRules.DualOneOther
			},
			{
				"it",
				PluralRules.DualOneOther
			},
			{
				"kk",
				PluralRules.DualOneOther
			},
			{
				"kl",
				PluralRules.DualOneOther
			},
			{
				"ku",
				PluralRules.DualOneOther
			},
			{
				"lb",
				PluralRules.DualOneOther
			},
			{
				"lg",
				PluralRules.DualOneOther
			},
			{
				"lo",
				PluralRules.DualOneOther
			},
			{
				"mas",
				PluralRules.DualOneOther
			},
			{
				"ml",
				PluralRules.DualOneOther
			},
			{
				"mn",
				PluralRules.DualOneOther
			},
			{
				"mr",
				PluralRules.DualOneOther
			},
			{
				"nah",
				PluralRules.DualOneOther
			},
			{
				"nb",
				PluralRules.DualOneOther
			},
			{
				"ne",
				PluralRules.DualOneOther
			},
			{
				"nl",
				PluralRules.DualOneOther
			},
			{
				"nn",
				PluralRules.DualOneOther
			},
			{
				"no",
				PluralRules.DualOneOther
			},
			{
				"nyn",
				PluralRules.DualOneOther
			},
			{
				"om",
				PluralRules.DualOneOther
			},
			{
				"or",
				PluralRules.DualOneOther
			},
			{
				"pa",
				PluralRules.DualOneOther
			},
			{
				"pap",
				PluralRules.DualOneOther
			},
			{
				"ps",
				PluralRules.DualOneOther
			},
			{
				"pt",
				PluralRules.DualOneOther
			},
			{
				"rm",
				PluralRules.DualOneOther
			},
			{
				"saq",
				PluralRules.DualOneOther
			},
			{
				"so",
				PluralRules.DualOneOther
			},
			{
				"sq",
				PluralRules.DualOneOther
			},
			{
				"ssy",
				PluralRules.DualOneOther
			},
			{
				"sw",
				PluralRules.DualOneOther
			},
			{
				"sv",
				PluralRules.DualOneOther
			},
			{
				"syr",
				PluralRules.DualOneOther
			},
			{
				"ta",
				PluralRules.DualOneOther
			},
			{
				"te",
				PluralRules.DualOneOther
			},
			{
				"tk",
				PluralRules.DualOneOther
			},
			{
				"tr",
				PluralRules.DualOneOther
			},
			{
				"ur",
				PluralRules.DualOneOther
			},
			{
				"wae",
				PluralRules.DualOneOther
			},
			{
				"xog",
				PluralRules.DualOneOther
			},
			{
				"zu",
				PluralRules.DualOneOther
			},
			{
				"ak",
				PluralRules.DualWithZero
			},
			{
				"am",
				PluralRules.DualWithZero
			},
			{
				"bh",
				PluralRules.DualWithZero
			},
			{
				"fil",
				PluralRules.DualWithZero
			},
			{
				"guw",
				PluralRules.DualWithZero
			},
			{
				"hi",
				PluralRules.DualWithZero
			},
			{
				"ln",
				PluralRules.DualWithZero
			},
			{
				"mg",
				PluralRules.DualWithZero
			},
			{
				"nso",
				PluralRules.DualWithZero
			},
			{
				"ti",
				PluralRules.DualWithZero
			},
			{
				"tl",
				PluralRules.DualWithZero
			},
			{
				"wa",
				PluralRules.DualWithZero
			},
			{
				"ff",
				PluralRules.DualFromZeroToTwo
			},
			{
				"fr",
				PluralRules.DualFromZeroToTwo
			},
			{
				"kab",
				PluralRules.DualFromZeroToTwo
			},
			{
				"ga",
				PluralRules.TripleOneTwoOther
			},
			{
				"iu",
				PluralRules.TripleOneTwoOther
			},
			{
				"ksh",
				PluralRules.TripleOneTwoOther
			},
			{
				"kw",
				PluralRules.TripleOneTwoOther
			},
			{
				"se",
				PluralRules.TripleOneTwoOther
			},
			{
				"sma",
				PluralRules.TripleOneTwoOther
			},
			{
				"smi",
				PluralRules.TripleOneTwoOther
			},
			{
				"smj",
				PluralRules.TripleOneTwoOther
			},
			{
				"smn",
				PluralRules.TripleOneTwoOther
			},
			{
				"sms",
				PluralRules.TripleOneTwoOther
			},
			{
				"be",
				PluralRules.RussianSerboCroatian
			},
			{
				"bs",
				PluralRules.RussianSerboCroatian
			},
			{
				"hr",
				PluralRules.RussianSerboCroatian
			},
			{
				"ru",
				PluralRules.RussianSerboCroatian
			},
			{
				"sh",
				PluralRules.RussianSerboCroatian
			},
			{
				"sr",
				PluralRules.RussianSerboCroatian
			},
			{
				"uk",
				PluralRules.RussianSerboCroatian
			},
			{
				"ar",
				PluralRules.Arabic
			},
			{
				"br",
				PluralRules.Breton
			},
			{
				"cs",
				PluralRules.Czech
			},
			{
				"cy",
				PluralRules.Welsh
			},
			{
				"gv",
				PluralRules.Manx
			},
			{
				"lag",
				PluralRules.Langi
			},
			{
				"lt",
				PluralRules.Lithuanian
			},
			{
				"lv",
				PluralRules.Latvian
			},
			{
				"mb",
				PluralRules.Macedonian
			},
			{
				"mo",
				PluralRules.Moldavian
			},
			{
				"mt",
				PluralRules.Maltese
			},
			{
				"pl",
				PluralRules.Polish
			},
			{
				"ro",
				PluralRules.Romanian
			},
			{
				"shi",
				PluralRules.Tachelhit
			},
			{
				"sk",
				PluralRules.Slovak
			},
			{
				"sl",
				PluralRules.Slovenian
			},
			{
				"tzm",
				PluralRules.CentralMoroccoTamazight
			}
		};

		public delegate int PluralRuleDelegate(decimal value, int pluralCount);
	}
}
