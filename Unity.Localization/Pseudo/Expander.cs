using System;
using System.Collections.Generic;

namespace UnityEngine.Localization.Pseudo
{
	[Serializable]
	public class Expander : IPseudoLocalizationMethod
	{
		public List<Expander.ExpansionRule> ExpansionRules
		{
			get
			{
				return this.m_ExpansionRules;
			}
		}

		public Expander.InsertLocation Location
		{
			get
			{
				return this.m_Location;
			}
			set
			{
				this.m_Location = value;
			}
		}

		public List<char> PaddingCharacters
		{
			get
			{
				return this.m_PaddingCharacters;
			}
		}

		public int MinimumStringLength
		{
			get
			{
				return this.m_MinimumStringLength;
			}
			set
			{
				this.m_MinimumStringLength = Mathf.Max(0, value);
			}
		}

		public Expander()
		{
			this.AddCharacterRange('!', '~');
		}

		public Expander(char paddingCharacter)
		{
			this.PaddingCharacters.Add(paddingCharacter);
		}

		public Expander(char start, char end)
		{
			this.AddCharacterRange(start, end);
		}

		public void AddCharacterRange(char start, char end)
		{
			for (char c = start; c < end; c += '\u0001')
			{
				this.PaddingCharacters.Add(c);
			}
		}

		public void SetConstantExpansion(float expansion)
		{
			if (this.m_ExpansionRules != null)
			{
				this.m_ExpansionRules.Clear();
			}
			this.AddExpansionRule(0, int.MaxValue, expansion);
		}

		public void AddExpansionRule(int minCharacters, int maxCharacters, float expansion)
		{
			if (this.m_ExpansionRules == null)
			{
				this.m_ExpansionRules = new List<Expander.ExpansionRule>();
			}
			this.m_ExpansionRules.Add(new Expander.ExpansionRule(minCharacters, maxCharacters, expansion));
		}

		internal float GetExpansionForLength(int length)
		{
			foreach (Expander.ExpansionRule expansionRule in this.ExpansionRules)
			{
				if (expansionRule.InRange(length))
				{
					return expansionRule.ExpansionAmount;
				}
			}
			return 0f;
		}

		public void Transform(Message message)
		{
			int length = message.Length;
			int num = Mathf.Max(length, this.MinimumStringLength);
			int num2 = Mathf.CeilToInt(this.GetExpansionForLength(num) * (float)num);
			if (num2 > 0)
			{
				num2 += num - length;
				char[] array = new char[num2];
				Random.InitState(this.GetRandomSeed(message.Original));
				for (int i = 0; i < num2; i++)
				{
					array[i] = this.PaddingCharacters[Random.Range(0, this.PaddingCharacters.Count)];
				}
				this.AddPaddingToMessage(message, array);
			}
		}

		private void AddPaddingToMessage(Message message, char[] padding)
		{
			MessageFragment messageFragment = null;
			MessageFragment messageFragment2 = null;
			string original = new string(padding);
			if (this.Location == Expander.InsertLocation.Start)
			{
				messageFragment = message.CreateTextFragment(original);
			}
			else if (this.Location == Expander.InsertLocation.End)
			{
				messageFragment2 = message.CreateTextFragment(original);
			}
			else
			{
				int num = Mathf.FloorToInt((float)padding.Length * 0.5f);
				messageFragment = message.CreateTextFragment(original, 0, num);
				messageFragment2 = message.CreateTextFragment(original, num, padding.Length - 1);
			}
			if (messageFragment != null)
			{
				message.Fragments.Insert(0, messageFragment);
			}
			if (messageFragment2 != null)
			{
				message.Fragments.Add(messageFragment2);
			}
		}

		private int GetRandomSeed(string input)
		{
			return input.GetHashCode();
		}

		[SerializeField]
		private List<Expander.ExpansionRule> m_ExpansionRules = new List<Expander.ExpansionRule>
		{
			new Expander.ExpansionRule(0, 10, 2f),
			new Expander.ExpansionRule(10, 20, 1f),
			new Expander.ExpansionRule(20, 30, 0.8f),
			new Expander.ExpansionRule(30, 50, 0.6f),
			new Expander.ExpansionRule(50, 70, 0.7f),
			new Expander.ExpansionRule(70, int.MaxValue, 0.3f)
		};

		[SerializeField]
		private Expander.InsertLocation m_Location = Expander.InsertLocation.End;

		[SerializeField]
		private int m_MinimumStringLength = 1;

		[SerializeField]
		private List<char> m_PaddingCharacters = new List<char>();

		public enum InsertLocation
		{
			Start,
			End,
			Both
		}

		[Serializable]
		public struct ExpansionRule : IComparable<Expander.ExpansionRule>
		{
			public int MinCharacters
			{
				get
				{
					return this.m_MinCharacters;
				}
				set
				{
					this.m_MinCharacters = Mathf.Max(0, value);
				}
			}

			public int MaxCharacters
			{
				get
				{
					return this.m_MaxCharacters;
				}
				set
				{
					this.m_MaxCharacters = Mathf.Max(0, value);
				}
			}

			public float ExpansionAmount
			{
				get
				{
					return this.m_ExpansionAmount;
				}
				set
				{
					this.m_ExpansionAmount = Mathf.Max(0f, value);
				}
			}

			public ExpansionRule(int minCharacters, int maxCharacters, float expansion)
			{
				this.m_MinCharacters = Mathf.Max(0, minCharacters);
				this.m_MaxCharacters = Mathf.Max(0, maxCharacters);
				this.m_ExpansionAmount = Mathf.Max(0f, expansion);
			}

			internal bool InRange(int length)
			{
				return length >= this.MinCharacters && length < this.MaxCharacters;
			}

			public int CompareTo(Expander.ExpansionRule other)
			{
				return this.MinCharacters.CompareTo(other.MinCharacters);
			}

			[SerializeField]
			private int m_MinCharacters;

			[SerializeField]
			private int m_MaxCharacters;

			[SerializeField]
			private float m_ExpansionAmount;
		}
	}
}
