using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D
{
	/// <summary>Encapsulates a custom user-defined line cap.</summary>
	public class CustomLineCap : MarshalByRefObject, ICloneable, IDisposable
	{
		internal static CustomLineCap CreateCustomLineCapObject(IntPtr cap)
		{
			return new CustomLineCap(cap);
		}

		internal CustomLineCap()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> class with the specified outline and fill.</summary>
		/// <param name="fillPath">A <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> object that defines the fill for the custom cap.</param>
		/// <param name="strokePath">A <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> object that defines the outline of the custom cap.</param>
		public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath) : this(fillPath, strokePath, LineCap.Flat)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> class from the specified existing <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration with the specified outline and fill.</summary>
		/// <param name="fillPath">A <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> object that defines the fill for the custom cap.</param>
		/// <param name="strokePath">A <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> object that defines the outline of the custom cap.</param>
		/// <param name="baseCap">The line cap from which to create the custom cap.</param>
		public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap) : this(fillPath, strokePath, baseCap, 0f)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> class from the specified existing <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration with the specified outline, fill, and inset.</summary>
		/// <param name="fillPath">A <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> object that defines the fill for the custom cap.</param>
		/// <param name="strokePath">A <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> object that defines the outline of the custom cap.</param>
		/// <param name="baseCap">The line cap from which to create the custom cap.</param>
		/// <param name="baseInset">The distance between the cap and the line.</param>
		public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap, float baseInset)
		{
			IntPtr nativeLineCap;
			int num = GDIPlus.GdipCreateCustomLineCap(new HandleRef(fillPath, (fillPath == null) ? IntPtr.Zero : fillPath.nativePath), new HandleRef(strokePath, (strokePath == null) ? IntPtr.Zero : strokePath.nativePath), baseCap, baseInset, out nativeLineCap);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			this.SetNativeLineCap(nativeLineCap);
		}

		internal CustomLineCap(IntPtr nativeLineCap)
		{
			this.SetNativeLineCap(nativeLineCap);
		}

		internal void SetNativeLineCap(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			this.nativeCap = new SafeCustomLineCapHandle(handle);
		}

		/// <summary>Releases all resources used by this <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> object.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (this._disposed)
			{
				return;
			}
			if (disposing && this.nativeCap != null)
			{
				this.nativeCap.Dispose();
			}
			this._disposed = true;
		}

		/// <summary>Allows an <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> to attempt to free resources and perform other cleanup operations before the <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> is reclaimed by garbage collection.</summary>
		~CustomLineCap()
		{
			this.Dispose(false);
		}

		/// <summary>Creates an exact copy of this <see cref="T:System.Drawing.Drawing2D.CustomLineCap" />.</summary>
		/// <returns>The <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> this method creates, cast as an object.</returns>
		public object Clone()
		{
			return this.CoreClone();
		}

		internal virtual object CoreClone()
		{
			IntPtr cap;
			int num = GDIPlus.GdipCloneCustomLineCap(new HandleRef(this, this.nativeCap), out cap);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return CustomLineCap.CreateCustomLineCapObject(cap);
		}

		/// <summary>Sets the caps used to start and end lines that make up this custom cap.</summary>
		/// <param name="startCap">The <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration used at the beginning of a line within this cap.</param>
		/// <param name="endCap">The <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration used at the end of a line within this cap.</param>
		public void SetStrokeCaps(LineCap startCap, LineCap endCap)
		{
			int num = GDIPlus.GdipSetCustomLineCapStrokeCaps(new HandleRef(this, this.nativeCap), startCap, endCap);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Gets the caps used to start and end lines that make up this custom cap.</summary>
		/// <param name="startCap">The <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration used at the beginning of a line within this cap.</param>
		/// <param name="endCap">The <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration used at the end of a line within this cap.</param>
		public void GetStrokeCaps(out LineCap startCap, out LineCap endCap)
		{
			int num = GDIPlus.GdipGetCustomLineCapStrokeCaps(new HandleRef(this, this.nativeCap), out startCap, out endCap);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Drawing.Drawing2D.LineJoin" /> enumeration that determines how lines that compose this <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> object are joined.</summary>
		/// <returns>The <see cref="T:System.Drawing.Drawing2D.LineJoin" /> enumeration this <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> object uses to join lines.</returns>
		public LineJoin StrokeJoin
		{
			get
			{
				LineJoin result;
				int num = GDIPlus.GdipGetCustomLineCapStrokeJoin(new HandleRef(this, this.nativeCap), out result);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
				return result;
			}
			set
			{
				int num = GDIPlus.GdipSetCustomLineCapStrokeJoin(new HandleRef(this, this.nativeCap), value);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration on which this <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> is based.</summary>
		/// <returns>The <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration on which this <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> is based.</returns>
		public LineCap BaseCap
		{
			get
			{
				LineCap result;
				int num = GDIPlus.GdipGetCustomLineCapBaseCap(new HandleRef(this, this.nativeCap), out result);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
				return result;
			}
			set
			{
				int num = GDIPlus.GdipSetCustomLineCapBaseCap(new HandleRef(this, this.nativeCap), value);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
			}
		}

		/// <summary>Gets or sets the distance between the cap and the line.</summary>
		/// <returns>The distance between the beginning of the cap and the end of the line.</returns>
		public float BaseInset
		{
			get
			{
				float result;
				int num = GDIPlus.GdipGetCustomLineCapBaseInset(new HandleRef(this, this.nativeCap), out result);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
				return result;
			}
			set
			{
				int num = GDIPlus.GdipSetCustomLineCapBaseInset(new HandleRef(this, this.nativeCap), value);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
			}
		}

		/// <summary>Gets or sets the amount by which to scale this <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> Class object with respect to the width of the <see cref="T:System.Drawing.Pen" /> object.</summary>
		/// <returns>The amount by which to scale the cap.</returns>
		public float WidthScale
		{
			get
			{
				float result;
				int num = GDIPlus.GdipGetCustomLineCapWidthScale(new HandleRef(this, this.nativeCap), out result);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
				return result;
			}
			set
			{
				int num = GDIPlus.GdipSetCustomLineCapWidthScale(new HandleRef(this, this.nativeCap), value);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
			}
		}

		internal SafeCustomLineCapHandle nativeCap;

		private bool _disposed;
	}
}
