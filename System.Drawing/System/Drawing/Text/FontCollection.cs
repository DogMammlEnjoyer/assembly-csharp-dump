using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Text
{
	/// <summary>Provides a base class for installed and private font collections.</summary>
	public abstract class FontCollection : IDisposable
	{
		internal FontCollection()
		{
			this._nativeFontCollection = IntPtr.Zero;
		}

		/// <summary>Releases all resources used by this <see cref="T:System.Drawing.Text.FontCollection" />.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Drawing.Text.FontCollection" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>Gets the array of <see cref="T:System.Drawing.FontFamily" /> objects associated with this <see cref="T:System.Drawing.Text.FontCollection" />.</summary>
		/// <returns>An array of <see cref="T:System.Drawing.FontFamily" /> objects.</returns>
		public FontFamily[] Families
		{
			get
			{
				int num = 0;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetFontCollectionFamilyCount(new HandleRef(this, this._nativeFontCollection), out num));
				IntPtr[] array = new IntPtr[num];
				int num2 = 0;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetFontCollectionFamilyList(new HandleRef(this, this._nativeFontCollection), num, array, out num2));
				FontFamily[] array2 = new FontFamily[num2];
				for (int i = 0; i < num2; i++)
				{
					IntPtr fntfamily;
					GDIPlus.GdipCloneFontFamily(new HandleRef(null, array[i]), out fntfamily);
					array2[i] = new FontFamily(fntfamily);
				}
				return array2;
			}
		}

		/// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
		~FontCollection()
		{
			this.Dispose(false);
		}

		internal IntPtr _nativeFontCollection;
	}
}
