using System;

namespace g3
{
	public static class Units
	{
		public static bool IsMetric(Units.Linear t)
		{
			return t > Units.Linear.UnknownUnits && t < (Units.Linear)50;
		}

		public static double GetMetricPower(Units.Linear t)
		{
			if (t > Units.Linear.UnknownUnits && t < (Units.Linear)50)
			{
				return (double)t - 20.0;
			}
			throw new Exception("Units.GetMetricPower: input unit is not metric!");
		}

		public static double ToMeters(Units.Linear t)
		{
			if (t > Units.Linear.UnknownUnits && t < (Units.Linear)50)
			{
				double metricPower = Units.GetMetricPower(t);
				return Math.Pow(10.0, metricPower);
			}
			if (t <= Units.Linear.Feet)
			{
				if (t == Units.Linear.Inches)
				{
					return 0.0254;
				}
				if (t == Units.Linear.Feet)
				{
					return 0.3048;
				}
			}
			else
			{
				if (t == Units.Linear.Yards)
				{
					return 0.9144;
				}
				if (t == Units.Linear.Miles)
				{
					return 1609.34;
				}
			}
			throw new Exception("Units.ToMeters: input unit is not handled!");
		}

		public static double MetersTo(Units.Linear t)
		{
			if (t > Units.Linear.UnknownUnits && t < (Units.Linear)50)
			{
				double metricPower = Units.GetMetricPower(t);
				return Math.Pow(10.0, -metricPower);
			}
			if (t <= Units.Linear.Feet)
			{
				if (t == Units.Linear.Inches)
				{
					return 39.37007874015748;
				}
				if (t == Units.Linear.Feet)
				{
					return 3.280839895013123;
				}
			}
			else
			{
				if (t == Units.Linear.Yards)
				{
					return 1.0936132983377078;
				}
				if (t == Units.Linear.Miles)
				{
					return 0.0006213727366498068;
				}
			}
			throw new Exception("Units.ToMeters: input unit is not handled!");
		}

		public static double Convert(Units.Linear from, Units.Linear to)
		{
			if (from == to)
			{
				return 1.0;
			}
			if (Units.IsMetric(from) && Units.IsMetric(to))
			{
				double metricPower = Units.GetMetricPower(from);
				double metricPower2 = Units.GetMetricPower(to);
				double y = metricPower - metricPower2;
				return Math.Pow(10.0, y);
			}
			return Units.ToMeters(from) * Units.MetersTo(to);
		}

		public static Units.Linear ParseLinear(string units)
		{
			if (units == "nm")
			{
				return Units.Linear.Nanometers;
			}
			if (units == "um")
			{
				return Units.Linear.Microns;
			}
			if (units == "mm")
			{
				return Units.Linear.Millimeters;
			}
			if (units == "cm")
			{
				return Units.Linear.Centimeters;
			}
			if (units == "m")
			{
				return Units.Linear.Meters;
			}
			if (units == "km")
			{
				return Units.Linear.Kilometers;
			}
			if (units == "in")
			{
				return Units.Linear.Inches;
			}
			if (units == "ft")
			{
				return Units.Linear.Feet;
			}
			if (units == "yd")
			{
				return Units.Linear.Yards;
			}
			if (units == "mi")
			{
				return Units.Linear.Miles;
			}
			return Units.Linear.UnknownUnits;
		}

		public static string GetShortString(Units.Linear unit)
		{
			if (unit <= Units.Linear.Kilometers)
			{
				if (unit <= Units.Linear.Nanometers)
				{
					if (unit == Units.Linear.UnknownUnits)
					{
						return "??";
					}
					if (unit == Units.Linear.Nanometers)
					{
						return "nm";
					}
				}
				else
				{
					switch (unit)
					{
					case Units.Linear.Microns:
						return "um";
					case (Units.Linear)15:
					case (Units.Linear)16:
					case (Units.Linear)19:
						break;
					case Units.Linear.Millimeters:
						return "mm";
					case Units.Linear.Centimeters:
						return "cm";
					case Units.Linear.Meters:
						return "m";
					default:
						if (unit == Units.Linear.Kilometers)
						{
							return "km";
						}
						break;
					}
				}
			}
			else if (unit <= Units.Linear.Feet)
			{
				if (unit == Units.Linear.Inches)
				{
					return "in";
				}
				if (unit == Units.Linear.Feet)
				{
					return "ft";
				}
			}
			else
			{
				if (unit == Units.Linear.Yards)
				{
					return "yd";
				}
				if (unit == Units.Linear.Miles)
				{
					return "mi";
				}
			}
			throw new Exception("Units.GetShortString: unhandled unit type!");
		}

		public enum Angular
		{
			Degrees,
			Radians
		}

		public enum Linear
		{
			UnknownUnits,
			Nanometers = 11,
			Microns = 14,
			Millimeters = 17,
			Centimeters,
			Meters = 20,
			Kilometers = 23,
			Inches = 105,
			Feet = 109,
			Yards,
			Miles = 115
		}
	}
}
