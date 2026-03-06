using System;

namespace System.Drawing
{
	/// <summary>Provides a graphics buffer for double buffering.</summary>
	public sealed class BufferedGraphics : IDisposable
	{
		private BufferedGraphics()
		{
		}

		internal BufferedGraphics(Graphics targetGraphics, Rectangle targetRectangle)
		{
			this.size = targetRectangle;
			this.target = targetGraphics;
			this.membmp = new Bitmap(this.size.Width, this.size.Height);
		}

		/// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
		~BufferedGraphics()
		{
			this.Dispose(false);
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Graphics" /> object that outputs to the graphics buffer.</summary>
		/// <returns>A <see cref="T:System.Drawing.Graphics" /> object that outputs to the graphics buffer.</returns>
		public Graphics Graphics
		{
			get
			{
				if (this.source == null)
				{
					this.source = Graphics.FromImage(this.membmp);
				}
				return this.source;
			}
		}

		/// <summary>Releases all resources used by the <see cref="T:System.Drawing.BufferedGraphics" /> object.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			if (this.membmp != null)
			{
				this.membmp.Dispose();
				this.membmp = null;
			}
			if (this.source != null)
			{
				this.source.Dispose();
				this.source = null;
			}
			this.target = null;
		}

		/// <summary>Writes the contents of the graphics buffer to the default device.</summary>
		public void Render()
		{
			this.Render(this.target);
		}

		/// <summary>Writes the contents of the graphics buffer to the specified <see cref="T:System.Drawing.Graphics" /> object.</summary>
		/// <param name="target">A <see cref="T:System.Drawing.Graphics" /> object to which to write the contents of the graphics buffer.</param>
		public void Render(Graphics target)
		{
			if (target == null)
			{
				return;
			}
			target.DrawImage(this.membmp, this.size);
		}

		/// <summary>Writes the contents of the graphics buffer to the device context associated with the specified <see cref="T:System.IntPtr" /> handle.</summary>
		/// <param name="targetDC">An <see cref="T:System.IntPtr" /> that points to the device context to which to write the contents of the graphics buffer.</param>
		[MonoTODO("The targetDC parameter has no equivalent in libgdiplus.")]
		public void Render(IntPtr targetDC)
		{
			throw new NotImplementedException();
		}

		private Rectangle size;

		private Bitmap membmp;

		private Graphics target;

		private Graphics source;
	}
}
