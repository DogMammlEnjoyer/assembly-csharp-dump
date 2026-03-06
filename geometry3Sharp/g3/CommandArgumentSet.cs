using System;
using System.Collections.Generic;

namespace g3
{
	public class CommandArgumentSet
	{
		public void Register(string argument, int defaultValue)
		{
			this.Integers.Add(argument, defaultValue);
		}

		public void Register(string argument, float defaultValue)
		{
			this.Floats.Add(argument, defaultValue);
		}

		public void Register(string argument, bool defaultValue)
		{
			this.Flags.Add(argument, defaultValue);
		}

		public void Register(string argument, string defaultValue)
		{
			this.Strings.Add(argument, defaultValue);
		}

		public bool Saw(string argument)
		{
			return this.SawArguments.Contains(argument);
		}

		public bool Validate(string sParam, params string[] values)
		{
			if (!this.Strings.ContainsKey(sParam))
			{
				throw new Exception("Argument set does not contain " + sParam);
			}
			string a = this.Strings[sParam];
			for (int i = 0; i < values.Length; i++)
			{
				if (a == values[i])
				{
					return true;
				}
			}
			return false;
		}

		public bool Parse(string[] arguments)
		{
			int num = arguments.Length;
			int i = 0;
			while (i < num)
			{
				string text = arguments[i];
				i++;
				if (this.Integers.ContainsKey(text))
				{
					this.SawArguments.Add(text);
					if (i == num)
					{
						this.error_missing_argument(text);
						return false;
					}
					string text2 = arguments[i];
					int value;
					if (!int.TryParse(text2, out value))
					{
						this.error_invalid_value(text, text2);
						return false;
					}
					this.Integers[text] = value;
					i++;
				}
				else if (this.Floats.ContainsKey(text))
				{
					this.SawArguments.Add(text);
					if (i == num)
					{
						this.error_missing_argument(text);
						return false;
					}
					string text3 = arguments[i];
					float value2;
					if (!float.TryParse(text3, out value2))
					{
						this.error_invalid_value(text, text3);
						return false;
					}
					this.Floats[text] = value2;
					i++;
				}
				else if (this.Strings.ContainsKey(text))
				{
					this.SawArguments.Add(text);
					if (i == num)
					{
						this.error_missing_argument(text);
						return false;
					}
					string value3 = arguments[i];
					this.Strings[text] = value3;
					i++;
				}
				else if (this.Flags.ContainsKey(text))
				{
					this.SawArguments.Add(text);
					this.Flags[text] = true;
				}
				else
				{
					this.Filenames.Add(text);
				}
			}
			return true;
		}

		protected virtual void error_missing_argument(string arg)
		{
			Console.WriteLine("argument {0} is missing value", arg);
		}

		protected virtual void error_invalid_value(string arg, string value)
		{
			Console.WriteLine("argument {0} has invalid value {1}", arg, value);
		}

		public Dictionary<string, int> Integers = new Dictionary<string, int>();

		public Dictionary<string, float> Floats = new Dictionary<string, float>();

		public Dictionary<string, string> Strings = new Dictionary<string, string>();

		public Dictionary<string, bool> Flags = new Dictionary<string, bool>();

		public HashSet<string> SawArguments = new HashSet<string>();

		public List<string> Filenames = new List<string>();
	}
}
