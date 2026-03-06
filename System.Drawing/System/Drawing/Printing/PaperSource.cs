using System;

namespace System.Drawing.Printing
{
	/// <summary>Specifies the paper tray from which the printer gets paper.</summary>
	[Serializable]
	public class PaperSource
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.PaperSource" /> class.</summary>
		public PaperSource()
		{
			this._kind = PaperSourceKind.Custom;
			this._name = string.Empty;
		}

		internal PaperSource(PaperSourceKind kind, string name)
		{
			this._kind = kind;
			this._name = name;
		}

		/// <summary>Gets the paper source.</summary>
		/// <returns>One of the <see cref="T:System.Drawing.Printing.PaperSourceKind" /> values.</returns>
		public PaperSourceKind Kind
		{
			get
			{
				if (this._kind >= (PaperSourceKind)256)
				{
					return PaperSourceKind.Custom;
				}
				return this._kind;
			}
		}

		/// <summary>Gets or sets the integer representing one of the <see cref="T:System.Drawing.Printing.PaperSourceKind" /> values or a custom value.</summary>
		/// <returns>The integer value representing one of the <see cref="T:System.Drawing.Printing.PaperSourceKind" /> values or a custom value.</returns>
		public int RawKind
		{
			get
			{
				return (int)this._kind;
			}
			set
			{
				this._kind = (PaperSourceKind)value;
			}
		}

		/// <summary>Gets or sets the name of the paper source.</summary>
		/// <returns>The name of the paper source.</returns>
		public string SourceName
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		/// <summary>Provides information about the <see cref="T:System.Drawing.Printing.PaperSource" /> in string form.</summary>
		/// <returns>A string.</returns>
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[PaperSource ",
				this.SourceName,
				" Kind=",
				this.Kind.ToString(),
				"]"
			});
		}

		private string _name;

		private PaperSourceKind _kind;
	}
}
