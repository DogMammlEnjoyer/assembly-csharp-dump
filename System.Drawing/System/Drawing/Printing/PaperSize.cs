using System;
using System.Globalization;

namespace System.Drawing.Printing
{
	/// <summary>Specifies the size of a piece of paper.</summary>
	[Serializable]
	public class PaperSize
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.PaperSize" /> class.</summary>
		public PaperSize()
		{
			this._kind = PaperKind.Custom;
			this._name = string.Empty;
			this._createdByDefaultConstructor = true;
		}

		internal PaperSize(PaperKind kind, string name, int width, int height)
		{
			this._kind = kind;
			this._name = name;
			this._width = width;
			this._height = height;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.PaperSize" /> class.</summary>
		/// <param name="name">The name of the paper.</param>
		/// <param name="width">The width of the paper, in hundredths of an inch.</param>
		/// <param name="height">The height of the paper, in hundredths of an inch.</param>
		public PaperSize(string name, int width, int height)
		{
			this._kind = PaperKind.Custom;
			this._name = name;
			this._width = width;
			this._height = height;
		}

		/// <summary>Gets or sets the height of the paper, in hundredths of an inch.</summary>
		/// <returns>The height of the paper, in hundredths of an inch.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Drawing.Printing.PaperSize.Kind" /> property is not set to <see cref="F:System.Drawing.Printing.PaperKind.Custom" />.</exception>
		public int Height
		{
			get
			{
				return this._height;
			}
			set
			{
				if (this._kind != PaperKind.Custom && !this._createdByDefaultConstructor)
				{
					throw new ArgumentException(SR.Format("PaperSize cannot be changed unless the Kind property is set to Custom.", Array.Empty<object>()));
				}
				this._height = value;
			}
		}

		/// <summary>Gets the type of paper.</summary>
		/// <returns>One of the <see cref="T:System.Drawing.Printing.PaperKind" /> values.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Drawing.Printing.PaperSize.Kind" /> property is not set to <see cref="F:System.Drawing.Printing.PaperKind.Custom" />.</exception>
		public PaperKind Kind
		{
			get
			{
				if (this._kind <= PaperKind.PrcEnvelopeNumber10Rotated && this._kind != (PaperKind)48 && this._kind != (PaperKind)49)
				{
					return this._kind;
				}
				return PaperKind.Custom;
			}
		}

		/// <summary>Gets or sets the name of the type of paper.</summary>
		/// <returns>The name of the type of paper.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Drawing.Printing.PaperSize.Kind" /> property is not set to <see cref="F:System.Drawing.Printing.PaperKind.Custom" />.</exception>
		public string PaperName
		{
			get
			{
				return this._name;
			}
			set
			{
				if (this._kind != PaperKind.Custom && !this._createdByDefaultConstructor)
				{
					throw new ArgumentException(SR.Format("PaperSize cannot be changed unless the Kind property is set to Custom.", Array.Empty<object>()));
				}
				this._name = value;
			}
		}

		/// <summary>Gets or sets an integer representing one of the <see cref="T:System.Drawing.Printing.PaperSize" /> values or a custom value.</summary>
		/// <returns>An integer representing one of the <see cref="T:System.Drawing.Printing.PaperSize" /> values, or a custom value.</returns>
		public int RawKind
		{
			get
			{
				return (int)this._kind;
			}
			set
			{
				this._kind = (PaperKind)value;
			}
		}

		/// <summary>Gets or sets the width of the paper, in hundredths of an inch.</summary>
		/// <returns>The width of the paper, in hundredths of an inch.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Drawing.Printing.PaperSize.Kind" /> property is not set to <see cref="F:System.Drawing.Printing.PaperKind.Custom" />.</exception>
		public int Width
		{
			get
			{
				return this._width;
			}
			set
			{
				if (this._kind != PaperKind.Custom && !this._createdByDefaultConstructor)
				{
					throw new ArgumentException(SR.Format("PaperSize cannot be changed unless the Kind property is set to Custom.", Array.Empty<object>()));
				}
				this._width = value;
			}
		}

		/// <summary>Provides information about the <see cref="T:System.Drawing.Printing.PaperSize" /> in string form.</summary>
		/// <returns>A string.</returns>
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[PaperSize ",
				this.PaperName,
				" Kind=",
				this.Kind.ToString(),
				" Height=",
				this.Height.ToString(CultureInfo.InvariantCulture),
				" Width=",
				this.Width.ToString(CultureInfo.InvariantCulture),
				"]"
			});
		}

		private PaperKind _kind;

		private string _name;

		private int _width;

		private int _height;

		private bool _createdByDefaultConstructor;
	}
}
