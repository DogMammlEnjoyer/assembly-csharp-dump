using System;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	/// <summary>Describes the interior of a graphics shape composed of rectangles and paths. This class cannot be inherited.</summary>
	public sealed class Region : MarshalByRefObject, IDisposable
	{
		/// <summary>Initializes a new <see cref="T:System.Drawing.Region" />.</summary>
		public Region()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCreateRegion(out this.nativeRegion));
		}

		internal Region(IntPtr native)
		{
			this.nativeRegion = native;
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Region" /> with the specified <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="path">A <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> that defines the new <see cref="T:System.Drawing.Region" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is <see langword="null" />.</exception>
		public Region(GraphicsPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCreateRegionPath(path.nativePath, out this.nativeRegion));
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Region" /> from the specified <see cref="T:System.Drawing.Rectangle" /> structure.</summary>
		/// <param name="rect">A <see cref="T:System.Drawing.Rectangle" /> structure that defines the interior of the new <see cref="T:System.Drawing.Region" />.</param>
		public Region(Rectangle rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCreateRegionRectI(ref rect, out this.nativeRegion));
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Region" /> from the specified <see cref="T:System.Drawing.RectangleF" /> structure.</summary>
		/// <param name="rect">A <see cref="T:System.Drawing.RectangleF" /> structure that defines the interior of the new <see cref="T:System.Drawing.Region" />.</param>
		public Region(RectangleF rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCreateRegionRect(ref rect, out this.nativeRegion));
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Region" /> from the specified data.</summary>
		/// <param name="rgnData">A <see cref="T:System.Drawing.Drawing2D.RegionData" /> that defines the interior of the new <see cref="T:System.Drawing.Region" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="rgnData" /> is <see langword="null" />.</exception>
		public Region(RegionData rgnData)
		{
			if (rgnData == null)
			{
				throw new ArgumentNullException("rgnData");
			}
			if (rgnData.Data.Length == 0)
			{
				throw new ArgumentException("rgnData");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCreateRegionRgnData(rgnData.Data, rgnData.Data.Length, out this.nativeRegion));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to the union of itself and the specified <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="path">The <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to unite with this <see cref="T:System.Drawing.Region" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is <see langword="null" />.</exception>
		public void Union(GraphicsPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionPath(this.nativeRegion, path.nativePath, CombineMode.Union));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to the union of itself and the specified <see cref="T:System.Drawing.Rectangle" /> structure.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.Rectangle" /> structure to unite with this <see cref="T:System.Drawing.Region" />.</param>
		public void Union(Rectangle rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRectI(this.nativeRegion, ref rect, CombineMode.Union));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to the union of itself and the specified <see cref="T:System.Drawing.RectangleF" /> structure.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.RectangleF" /> structure to unite with this <see cref="T:System.Drawing.Region" />.</param>
		public void Union(RectangleF rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRect(this.nativeRegion, ref rect, CombineMode.Union));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to the union of itself and the specified <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="region">The <see cref="T:System.Drawing.Region" /> to unite with this <see cref="T:System.Drawing.Region" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="region" /> is <see langword="null" />.</exception>
		public void Union(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRegion(this.nativeRegion, region.NativeObject, CombineMode.Union));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to the intersection of itself with the specified <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="path">The <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to intersect with this <see cref="T:System.Drawing.Region" />.</param>
		public void Intersect(GraphicsPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionPath(this.nativeRegion, path.nativePath, CombineMode.Intersect));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to the intersection of itself with the specified <see cref="T:System.Drawing.Rectangle" /> structure.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.Rectangle" /> structure to intersect with this <see cref="T:System.Drawing.Region" />.</param>
		public void Intersect(Rectangle rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRectI(this.nativeRegion, ref rect, CombineMode.Intersect));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to the intersection of itself with the specified <see cref="T:System.Drawing.RectangleF" /> structure.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.RectangleF" /> structure to intersect with this <see cref="T:System.Drawing.Region" />.</param>
		public void Intersect(RectangleF rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRect(this.nativeRegion, ref rect, CombineMode.Intersect));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to the intersection of itself with the specified <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="region">The <see cref="T:System.Drawing.Region" /> to intersect with this <see cref="T:System.Drawing.Region" />.</param>
		public void Intersect(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRegion(this.nativeRegion, region.NativeObject, CombineMode.Intersect));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to contain the portion of the specified <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> that does not intersect with this <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="path">The <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to complement this <see cref="T:System.Drawing.Region" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is <see langword="null" />.</exception>
		public void Complement(GraphicsPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionPath(this.nativeRegion, path.nativePath, CombineMode.Complement));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to contain the portion of the specified <see cref="T:System.Drawing.Rectangle" /> structure that does not intersect with this <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.Rectangle" /> structure to complement this <see cref="T:System.Drawing.Region" />.</param>
		public void Complement(Rectangle rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRectI(this.nativeRegion, ref rect, CombineMode.Complement));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to contain the portion of the specified <see cref="T:System.Drawing.RectangleF" /> structure that does not intersect with this <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.RectangleF" /> structure to complement this <see cref="T:System.Drawing.Region" />.</param>
		public void Complement(RectangleF rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRect(this.nativeRegion, ref rect, CombineMode.Complement));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to contain the portion of the specified <see cref="T:System.Drawing.Region" /> that does not intersect with this <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="region">The <see cref="T:System.Drawing.Region" /> object to complement this <see cref="T:System.Drawing.Region" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="region" /> is <see langword="null" />.</exception>
		public void Complement(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRegion(this.nativeRegion, region.NativeObject, CombineMode.Complement));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to contain only the portion of its interior that does not intersect with the specified <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="path">The <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to exclude from this <see cref="T:System.Drawing.Region" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is <see langword="null" />.</exception>
		public void Exclude(GraphicsPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionPath(this.nativeRegion, path.nativePath, CombineMode.Exclude));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to contain only the portion of its interior that does not intersect with the specified <see cref="T:System.Drawing.Rectangle" /> structure.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.Rectangle" /> structure to exclude from this <see cref="T:System.Drawing.Region" />.</param>
		public void Exclude(Rectangle rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRectI(this.nativeRegion, ref rect, CombineMode.Exclude));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to contain only the portion of its interior that does not intersect with the specified <see cref="T:System.Drawing.RectangleF" /> structure.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.RectangleF" /> structure to exclude from this <see cref="T:System.Drawing.Region" />.</param>
		public void Exclude(RectangleF rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRect(this.nativeRegion, ref rect, CombineMode.Exclude));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to contain only the portion of its interior that does not intersect with the specified <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="region">The <see cref="T:System.Drawing.Region" /> to exclude from this <see cref="T:System.Drawing.Region" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="region" /> is <see langword="null" />.</exception>
		public void Exclude(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRegion(this.nativeRegion, region.NativeObject, CombineMode.Exclude));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to the union minus the intersection of itself with the specified <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="path">The <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to <see cref="Overload:System.Drawing.Region.Xor" /> with this <see cref="T:System.Drawing.Region" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is <see langword="null" />.</exception>
		public void Xor(GraphicsPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionPath(this.nativeRegion, path.nativePath, CombineMode.Xor));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to the union minus the intersection of itself with the specified <see cref="T:System.Drawing.Rectangle" /> structure.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.Rectangle" /> structure to <see cref="Overload:System.Drawing.Region.Xor" /> with this <see cref="T:System.Drawing.Region" />.</param>
		public void Xor(Rectangle rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRectI(this.nativeRegion, ref rect, CombineMode.Xor));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to the union minus the intersection of itself with the specified <see cref="T:System.Drawing.RectangleF" /> structure.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.RectangleF" /> structure to <see cref="M:System.Drawing.Region.Xor(System.Drawing.Drawing2D.GraphicsPath)" /> with this <see cref="T:System.Drawing.Region" />.</param>
		public void Xor(RectangleF rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRect(this.nativeRegion, ref rect, CombineMode.Xor));
		}

		/// <summary>Updates this <see cref="T:System.Drawing.Region" /> to the union minus the intersection of itself with the specified <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="region">The <see cref="T:System.Drawing.Region" /> to <see cref="Overload:System.Drawing.Region.Xor" /> with this <see cref="T:System.Drawing.Region" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="region" /> is <see langword="null" />.</exception>
		public void Xor(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRegion(this.nativeRegion, region.NativeObject, CombineMode.Xor));
		}

		/// <summary>Gets a <see cref="T:System.Drawing.RectangleF" /> structure that represents a rectangle that bounds this <see cref="T:System.Drawing.Region" /> on the drawing surface of a <see cref="T:System.Drawing.Graphics" /> object.</summary>
		/// <param name="g">The <see cref="T:System.Drawing.Graphics" /> on which this <see cref="T:System.Drawing.Region" /> is drawn.</param>
		/// <returns>A <see cref="T:System.Drawing.RectangleF" /> structure that represents the bounding rectangle for this <see cref="T:System.Drawing.Region" /> on the specified drawing surface.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="g" /> is <see langword="null" />.</exception>
		public RectangleF GetBounds(Graphics g)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			RectangleF result = default(Rectangle);
			GDIPlus.CheckStatus(GDIPlus.GdipGetRegionBounds(this.nativeRegion, g.NativeObject, ref result));
			return result;
		}

		/// <summary>Offsets the coordinates of this <see cref="T:System.Drawing.Region" /> by the specified amount.</summary>
		/// <param name="dx">The amount to offset this <see cref="T:System.Drawing.Region" /> horizontally.</param>
		/// <param name="dy">The amount to offset this <see cref="T:System.Drawing.Region" /> vertically.</param>
		public void Translate(int dx, int dy)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipTranslateRegionI(this.nativeRegion, dx, dy));
		}

		/// <summary>Offsets the coordinates of this <see cref="T:System.Drawing.Region" /> by the specified amount.</summary>
		/// <param name="dx">The amount to offset this <see cref="T:System.Drawing.Region" /> horizontally.</param>
		/// <param name="dy">The amount to offset this <see cref="T:System.Drawing.Region" /> vertically.</param>
		public void Translate(float dx, float dy)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipTranslateRegion(this.nativeRegion, dx, dy));
		}

		/// <summary>Tests whether the specified point is contained within this <see cref="T:System.Drawing.Region" /> object when drawn using the specified <see cref="T:System.Drawing.Graphics" /> object.</summary>
		/// <param name="x">The x-coordinate of the point to test.</param>
		/// <param name="y">The y-coordinate of the point to test.</param>
		/// <param name="g">A <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.</param>
		/// <returns>
		///   <see langword="true" /> when the specified point is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(int x, int y, Graphics g)
		{
			IntPtr graphics = (g == null) ? IntPtr.Zero : g.NativeObject;
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPointI(this.nativeRegion, x, y, graphics, out result));
			return result;
		}

		/// <summary>Tests whether any portion of the specified rectangle is contained within this <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="x">The x-coordinate of the upper-left corner of the rectangle to test.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the rectangle to test.</param>
		/// <param name="width">The width of the rectangle to test.</param>
		/// <param name="height">The height of the rectangle to test.</param>
		/// <returns>
		///   <see langword="true" /> when any portion of the specified rectangle is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(int x, int y, int width, int height)
		{
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRectI(this.nativeRegion, x, y, width, height, IntPtr.Zero, out result));
			return result;
		}

		/// <summary>Tests whether any portion of the specified rectangle is contained within this <see cref="T:System.Drawing.Region" /> when drawn using the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="x">The x-coordinate of the upper-left corner of the rectangle to test.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the rectangle to test.</param>
		/// <param name="width">The width of the rectangle to test.</param>
		/// <param name="height">The height of the rectangle to test.</param>
		/// <param name="g">A <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.</param>
		/// <returns>
		///   <see langword="true" /> when any portion of the specified rectangle is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(int x, int y, int width, int height, Graphics g)
		{
			IntPtr graphics = (g == null) ? IntPtr.Zero : g.NativeObject;
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRectI(this.nativeRegion, x, y, width, height, graphics, out result));
			return result;
		}

		/// <summary>Tests whether the specified <see cref="T:System.Drawing.Point" /> structure is contained within this <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="point">The <see cref="T:System.Drawing.Point" /> structure to test.</param>
		/// <returns>
		///   <see langword="true" /> when <paramref name="point" /> is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(Point point)
		{
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPointI(this.nativeRegion, point.X, point.Y, IntPtr.Zero, out result));
			return result;
		}

		/// <summary>Tests whether the specified <see cref="T:System.Drawing.PointF" /> structure is contained within this <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="point">The <see cref="T:System.Drawing.PointF" /> structure to test.</param>
		/// <returns>
		///   <see langword="true" /> when <paramref name="point" /> is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(PointF point)
		{
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPoint(this.nativeRegion, point.X, point.Y, IntPtr.Zero, out result));
			return result;
		}

		/// <summary>Tests whether the specified <see cref="T:System.Drawing.Point" /> structure is contained within this <see cref="T:System.Drawing.Region" /> when drawn using the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="point">The <see cref="T:System.Drawing.Point" /> structure to test.</param>
		/// <param name="g">A <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.</param>
		/// <returns>
		///   <see langword="true" /> when <paramref name="point" /> is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(Point point, Graphics g)
		{
			IntPtr graphics = (g == null) ? IntPtr.Zero : g.NativeObject;
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPointI(this.nativeRegion, point.X, point.Y, graphics, out result));
			return result;
		}

		/// <summary>Tests whether the specified <see cref="T:System.Drawing.PointF" /> structure is contained within this <see cref="T:System.Drawing.Region" /> when drawn using the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="point">The <see cref="T:System.Drawing.PointF" /> structure to test.</param>
		/// <param name="g">A <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.</param>
		/// <returns>
		///   <see langword="true" /> when <paramref name="point" /> is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(PointF point, Graphics g)
		{
			IntPtr graphics = (g == null) ? IntPtr.Zero : g.NativeObject;
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPoint(this.nativeRegion, point.X, point.Y, graphics, out result));
			return result;
		}

		/// <summary>Tests whether any portion of the specified <see cref="T:System.Drawing.Rectangle" /> structure is contained within this <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.Rectangle" /> structure to test.</param>
		/// <returns>This method returns <see langword="true" /> when any portion of <paramref name="rect" /> is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(Rectangle rect)
		{
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRectI(this.nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, IntPtr.Zero, out result));
			return result;
		}

		/// <summary>Tests whether any portion of the specified <see cref="T:System.Drawing.RectangleF" /> structure is contained within this <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.RectangleF" /> structure to test.</param>
		/// <returns>
		///   <see langword="true" /> when any portion of <paramref name="rect" /> is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(RectangleF rect)
		{
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRect(this.nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, IntPtr.Zero, out result));
			return result;
		}

		/// <summary>Tests whether any portion of the specified <see cref="T:System.Drawing.Rectangle" /> structure is contained within this <see cref="T:System.Drawing.Region" /> when drawn using the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.Rectangle" /> structure to test.</param>
		/// <param name="g">A <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.</param>
		/// <returns>
		///   <see langword="true" /> when any portion of the <paramref name="rect" /> is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(Rectangle rect, Graphics g)
		{
			IntPtr graphics = (g == null) ? IntPtr.Zero : g.NativeObject;
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRectI(this.nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, graphics, out result));
			return result;
		}

		/// <summary>Tests whether any portion of the specified <see cref="T:System.Drawing.RectangleF" /> structure is contained within this <see cref="T:System.Drawing.Region" /> when drawn using the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="rect">The <see cref="T:System.Drawing.RectangleF" /> structure to test.</param>
		/// <param name="g">A <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.</param>
		/// <returns>
		///   <see langword="true" /> when <paramref name="rect" /> is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(RectangleF rect, Graphics g)
		{
			IntPtr graphics = (g == null) ? IntPtr.Zero : g.NativeObject;
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRect(this.nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, graphics, out result));
			return result;
		}

		/// <summary>Tests whether the specified point is contained within this <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="x">The x-coordinate of the point to test.</param>
		/// <param name="y">The y-coordinate of the point to test.</param>
		/// <returns>
		///   <see langword="true" /> when the specified point is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(float x, float y)
		{
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPoint(this.nativeRegion, x, y, IntPtr.Zero, out result));
			return result;
		}

		/// <summary>Tests whether the specified point is contained within this <see cref="T:System.Drawing.Region" /> when drawn using the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="x">The x-coordinate of the point to test.</param>
		/// <param name="y">The y-coordinate of the point to test.</param>
		/// <param name="g">A <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.</param>
		/// <returns>
		///   <see langword="true" /> when the specified point is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(float x, float y, Graphics g)
		{
			IntPtr graphics = (g == null) ? IntPtr.Zero : g.NativeObject;
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPoint(this.nativeRegion, x, y, graphics, out result));
			return result;
		}

		/// <summary>Tests whether any portion of the specified rectangle is contained within this <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="x">The x-coordinate of the upper-left corner of the rectangle to test.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the rectangle to test.</param>
		/// <param name="width">The width of the rectangle to test.</param>
		/// <param name="height">The height of the rectangle to test.</param>
		/// <returns>
		///   <see langword="true" /> when any portion of the specified rectangle is contained within this <see cref="T:System.Drawing.Region" /> object; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(float x, float y, float width, float height)
		{
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRect(this.nativeRegion, x, y, width, height, IntPtr.Zero, out result));
			return result;
		}

		/// <summary>Tests whether any portion of the specified rectangle is contained within this <see cref="T:System.Drawing.Region" /> when drawn using the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="x">The x-coordinate of the upper-left corner of the rectangle to test.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the rectangle to test.</param>
		/// <param name="width">The width of the rectangle to test.</param>
		/// <param name="height">The height of the rectangle to test.</param>
		/// <param name="g">A <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.</param>
		/// <returns>
		///   <see langword="true" /> when any portion of the specified rectangle is contained within this <see cref="T:System.Drawing.Region" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(float x, float y, float width, float height, Graphics g)
		{
			IntPtr graphics = (g == null) ? IntPtr.Zero : g.NativeObject;
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRect(this.nativeRegion, x, y, width, height, graphics, out result));
			return result;
		}

		/// <summary>Tests whether this <see cref="T:System.Drawing.Region" /> has an empty interior on the specified drawing surface.</summary>
		/// <param name="g">A <see cref="T:System.Drawing.Graphics" /> that represents a drawing surface.</param>
		/// <returns>
		///   <see langword="true" /> if the interior of this <see cref="T:System.Drawing.Region" /> is empty when the transformation associated with <paramref name="g" /> is applied; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="g" /> is <see langword="null" />.</exception>
		public bool IsEmpty(Graphics g)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsEmptyRegion(this.nativeRegion, g.NativeObject, out result));
			return result;
		}

		/// <summary>Tests whether this <see cref="T:System.Drawing.Region" /> has an infinite interior on the specified drawing surface.</summary>
		/// <param name="g">A <see cref="T:System.Drawing.Graphics" /> that represents a drawing surface.</param>
		/// <returns>
		///   <see langword="true" /> if the interior of this <see cref="T:System.Drawing.Region" /> is infinite when the transformation associated with <paramref name="g" /> is applied; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="g" /> is <see langword="null" />.</exception>
		public bool IsInfinite(Graphics g)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsInfiniteRegion(this.nativeRegion, g.NativeObject, out result));
			return result;
		}

		/// <summary>Initializes this <see cref="T:System.Drawing.Region" /> to an empty interior.</summary>
		public void MakeEmpty()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetEmpty(this.nativeRegion));
		}

		/// <summary>Initializes this <see cref="T:System.Drawing.Region" /> object to an infinite interior.</summary>
		public void MakeInfinite()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetInfinite(this.nativeRegion));
		}

		/// <summary>Tests whether the specified <see cref="T:System.Drawing.Region" /> is identical to this <see cref="T:System.Drawing.Region" /> on the specified drawing surface.</summary>
		/// <param name="region">The <see cref="T:System.Drawing.Region" /> to test.</param>
		/// <param name="g">A <see cref="T:System.Drawing.Graphics" /> that represents a drawing surface.</param>
		/// <returns>
		///   <see langword="true" /> if the interior of region is identical to the interior of this region when the transformation associated with the <paramref name="g" /> parameter is applied; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="g" /> or <paramref name="region" /> is <see langword="null" />.</exception>
		public bool Equals(Region region, Graphics g)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsEqualRegion(this.nativeRegion, region.NativeObject, g.NativeObject, out result));
			return result;
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Region" /> from a handle to the specified existing GDI region.</summary>
		/// <param name="hrgn">A handle to an existing <see cref="T:System.Drawing.Region" />.</param>
		/// <returns>The new <see cref="T:System.Drawing.Region" />.</returns>
		public static Region FromHrgn(IntPtr hrgn)
		{
			if (hrgn == IntPtr.Zero)
			{
				throw new ArgumentException("hrgn");
			}
			IntPtr native;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateRegionHrgn(hrgn, out native));
			return new Region(native);
		}

		/// <summary>Returns a Windows handle to this <see cref="T:System.Drawing.Region" /> in the specified graphics context.</summary>
		/// <param name="g">The <see cref="T:System.Drawing.Graphics" /> on which this <see cref="T:System.Drawing.Region" /> is drawn.</param>
		/// <returns>A Windows handle to this <see cref="T:System.Drawing.Region" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="g" /> is <see langword="null" />.</exception>
		public IntPtr GetHrgn(Graphics g)
		{
			if (g == null)
			{
				return this.nativeRegion;
			}
			IntPtr zero = IntPtr.Zero;
			GDIPlus.CheckStatus(GDIPlus.GdipGetRegionHRgn(this.nativeRegion, g.NativeObject, ref zero));
			return zero;
		}

		/// <summary>Returns a <see cref="T:System.Drawing.Drawing2D.RegionData" /> that represents the information that describes this <see cref="T:System.Drawing.Region" />.</summary>
		/// <returns>A <see cref="T:System.Drawing.Drawing2D.RegionData" /> that represents the information that describes this <see cref="T:System.Drawing.Region" />.</returns>
		public RegionData GetRegionData()
		{
			int num;
			GDIPlus.CheckStatus(GDIPlus.GdipGetRegionDataSize(this.nativeRegion, out num));
			byte[] array = new byte[num];
			int num2;
			GDIPlus.CheckStatus(GDIPlus.GdipGetRegionData(this.nativeRegion, array, num, out num2));
			return new RegionData(array);
		}

		/// <summary>Returns an array of <see cref="T:System.Drawing.RectangleF" /> structures that approximate this <see cref="T:System.Drawing.Region" /> after the specified matrix transformation is applied.</summary>
		/// <param name="matrix">A <see cref="T:System.Drawing.Drawing2D.Matrix" /> that represents a geometric transformation to apply to the region.</param>
		/// <returns>An array of <see cref="T:System.Drawing.RectangleF" /> structures that approximate this <see cref="T:System.Drawing.Region" /> after the specified matrix transformation is applied.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="matrix" /> is <see langword="null" />.</exception>
		public RectangleF[] GetRegionScans(Matrix matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			int num;
			GDIPlus.CheckStatus(GDIPlus.GdipGetRegionScansCount(this.nativeRegion, out num, matrix.NativeObject));
			if (num == 0)
			{
				return new RectangleF[0];
			}
			RectangleF[] array = new RectangleF[num];
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf<RectangleF>(array[0]) * num);
			try
			{
				GDIPlus.CheckStatus(GDIPlus.GdipGetRegionScans(this.nativeRegion, intPtr, out num, matrix.NativeObject));
			}
			finally
			{
				GDIPlus.FromUnManagedMemoryToRectangles(intPtr, array);
			}
			return array;
		}

		/// <summary>Transforms this <see cref="T:System.Drawing.Region" /> by the specified <see cref="T:System.Drawing.Drawing2D.Matrix" />.</summary>
		/// <param name="matrix">The <see cref="T:System.Drawing.Drawing2D.Matrix" /> by which to transform this <see cref="T:System.Drawing.Region" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="matrix" /> is <see langword="null" />.</exception>
		public void Transform(Matrix matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipTransformRegion(this.nativeRegion, matrix.NativeObject));
		}

		/// <summary>Creates an exact copy of this <see cref="T:System.Drawing.Region" />.</summary>
		/// <returns>The <see cref="T:System.Drawing.Region" /> that this method creates.</returns>
		public Region Clone()
		{
			IntPtr native;
			GDIPlus.CheckStatus(GDIPlus.GdipCloneRegion(this.nativeRegion, out native));
			return new Region(native);
		}

		/// <summary>Releases all resources used by this <see cref="T:System.Drawing.Region" />.</summary>
		public void Dispose()
		{
			this.DisposeHandle();
			GC.SuppressFinalize(this);
		}

		private void DisposeHandle()
		{
			if (this.nativeRegion != IntPtr.Zero)
			{
				GDIPlus.GdipDeleteRegion(this.nativeRegion);
				this.nativeRegion = IntPtr.Zero;
			}
		}

		/// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
		~Region()
		{
			this.DisposeHandle();
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativeRegion;
			}
			set
			{
				this.nativeRegion = value;
			}
		}

		/// <summary>Releases the handle of the <see cref="T:System.Drawing.Region" />.</summary>
		/// <param name="regionHandle">The handle to the <see cref="T:System.Drawing.Region" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="regionHandle" /> is <see langword="null" />.</exception>
		public void ReleaseHrgn(IntPtr regionHandle)
		{
			if (regionHandle == IntPtr.Zero)
			{
				throw new ArgumentNullException("regionHandle");
			}
			Status status = Status.Ok;
			if (GDIPlus.RunningOnUnix())
			{
				status = GDIPlus.GdipDeleteRegion(regionHandle);
			}
			else if (!GDIPlus.DeleteObject(regionHandle))
			{
				status = Status.InvalidParameter;
			}
			GDIPlus.CheckStatus(status);
		}

		private IntPtr nativeRegion = IntPtr.Zero;
	}
}
