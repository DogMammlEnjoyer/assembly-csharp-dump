using System;

namespace Sirenix.OdinInspector
{
	public sealed class RequiredListLengthAttribute : Attribute
	{
		public int MinLength
		{
			get
			{
				return this.minLength;
			}
			set
			{
				this.minLength = value;
				this.minLengthIsSet = true;
			}
		}

		public int MaxLength
		{
			get
			{
				return this.maxLength;
			}
			set
			{
				this.maxLength = value;
				this.maxLengthIsSet = true;
			}
		}

		public bool MinLengthIsSet
		{
			get
			{
				return this.minLengthIsSet;
			}
		}

		public bool MaxLengthIsSet
		{
			get
			{
				return this.maxLengthIsSet;
			}
		}

		public bool PrefabKindIsSet
		{
			get
			{
				return this.prefabKindIsSet;
			}
		}

		public PrefabKind PrefabKind
		{
			get
			{
				return this.prefabKind;
			}
			set
			{
				this.prefabKind = value;
				this.prefabKindIsSet = true;
			}
		}

		public RequiredListLengthAttribute()
		{
		}

		public RequiredListLengthAttribute(int fixedLength)
		{
			this.MinLength = fixedLength;
			this.MaxLength = fixedLength;
		}

		public RequiredListLengthAttribute(int minLength, int maxLength)
		{
			this.MinLength = minLength;
			this.MaxLength = maxLength;
		}

		public RequiredListLengthAttribute(int minLength, string maxLengthGetter)
		{
			this.MinLength = minLength;
			this.MaxLengthGetter = maxLengthGetter;
		}

		public RequiredListLengthAttribute(string fixedLengthGetter)
		{
			this.MinLengthGetter = fixedLengthGetter;
			this.MaxLengthGetter = fixedLengthGetter;
		}

		public RequiredListLengthAttribute(string minLengthGetter, string maxLengthGetter)
		{
			this.MinLengthGetter = minLengthGetter;
			this.MaxLengthGetter = maxLengthGetter;
		}

		public RequiredListLengthAttribute(string minLengthGetter, int maxLength)
		{
			this.MinLengthGetter = minLengthGetter;
			this.MaxLength = maxLength;
		}

		private PrefabKind prefabKind;

		private bool prefabKindIsSet;

		private int minLength;

		private int maxLength;

		private bool minLengthIsSet;

		private bool maxLengthIsSet;

		public string MinLengthGetter;

		public string MaxLengthGetter;
	}
}
