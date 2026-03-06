using System;

namespace System.Drawing
{
	/// <summary>Provides methods for creating graphics buffers that can be used for double buffering.</summary>
	public sealed class BufferedGraphicsContext : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.BufferedGraphicsContext" /> class.</summary>
		public BufferedGraphicsContext()
		{
			this.max_buffer = Size.Empty;
		}

		/// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
		~BufferedGraphicsContext()
		{
		}

		/// <summary>Creates a graphics buffer of the specified size using the pixel format of the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="targetGraphics">The <see cref="T:System.Drawing.Graphics" /> to match the pixel format for the new buffer to.</param>
		/// <param name="targetRectangle">A <see cref="T:System.Drawing.Rectangle" /> indicating the size of the buffer to create.</param>
		/// <returns>A <see cref="T:System.Drawing.BufferedGraphics" /> that can be used to draw to a buffer of the specified dimensions.</returns>
		public BufferedGraphics Allocate(Graphics targetGraphics, Rectangle targetRectangle)
		{
			return new BufferedGraphics(targetGraphics, targetRectangle);
		}

		/// <summary>Creates a graphics buffer of the specified size using the pixel format of the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="targetDC">An <see cref="T:System.IntPtr" /> to a device context to match the pixel format of the new buffer to.</param>
		/// <param name="targetRectangle">A <see cref="T:System.Drawing.Rectangle" /> indicating the size of the buffer to create.</param>
		/// <returns>A <see cref="T:System.Drawing.BufferedGraphics" /> that can be used to draw to a buffer of the specified dimensions.</returns>
		[MonoTODO("The targetDC parameter has no equivalent in libgdiplus.")]
		public BufferedGraphics Allocate(IntPtr targetDC, Rectangle targetRectangle)
		{
			throw new NotImplementedException();
		}

		/// <summary>Releases all resources used by the <see cref="T:System.Drawing.BufferedGraphicsContext" />.</summary>
		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		/// <summary>Disposes of the current graphics buffer, if a buffer has been allocated and has not yet been disposed.</summary>
		public void Invalidate()
		{
		}

		/// <summary>Gets or sets the maximum size of the buffer to use.</summary>
		/// <returns>A <see cref="T:System.Drawing.Size" /> indicating the maximum size of the buffer dimensions.</returns>
		/// <exception cref="T:System.ArgumentException">The height or width of the size is less than or equal to zero.</exception>
		public Size MaximumBuffer
		{
			get
			{
				return this.max_buffer;
			}
			set
			{
				if (value.Width <= 0 || value.Height <= 0)
				{
					throw new ArgumentException("The height or width of the size is less than or equal to zero.");
				}
				this.max_buffer = value;
			}
		}

		private Size max_buffer;
	}
}
