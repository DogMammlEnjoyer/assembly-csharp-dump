using System;
using System.ComponentModel;

namespace System.Drawing.Drawing2D
{
	/// <summary>Represents a series of connected lines and curves. This class cannot be inherited.</summary>
	public sealed class GraphicsPath : MarshalByRefObject, ICloneable, IDisposable
	{
		private GraphicsPath(IntPtr ptr)
		{
			this.nativePath = ptr;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> class with a <see cref="P:System.Drawing.Drawing2D.GraphicsPath.FillMode" /> value of <see cref="F:System.Drawing.Drawing2D.FillMode.Alternate" />.</summary>
		public GraphicsPath()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCreatePath(FillMode.Alternate, out this.nativePath));
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> class with the specified <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration.</summary>
		/// <param name="fillMode">The <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration that determines how the interior of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> is filled.</param>
		public GraphicsPath(FillMode fillMode)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCreatePath(fillMode, out this.nativePath));
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> class with the specified <see cref="T:System.Drawing.Drawing2D.PathPointType" /> and <see cref="T:System.Drawing.Point" /> arrays.</summary>
		/// <param name="pts">An array of <see cref="T:System.Drawing.Point" /> structures that defines the coordinates of the points that make up this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</param>
		/// <param name="types">An array of <see cref="T:System.Drawing.Drawing2D.PathPointType" /> enumeration elements that specifies the type of each corresponding point in the <paramref name="pts" /> array.</param>
		public GraphicsPath(Point[] pts, byte[] types) : this(pts, types, FillMode.Alternate)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> array with the specified <see cref="T:System.Drawing.Drawing2D.PathPointType" /> and <see cref="T:System.Drawing.PointF" /> arrays.</summary>
		/// <param name="pts">An array of <see cref="T:System.Drawing.PointF" /> structures that defines the coordinates of the points that make up this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</param>
		/// <param name="types">An array of <see cref="T:System.Drawing.Drawing2D.PathPointType" /> enumeration elements that specifies the type of each corresponding point in the <paramref name="pts" /> array.</param>
		public GraphicsPath(PointF[] pts, byte[] types) : this(pts, types, FillMode.Alternate)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> class with the specified <see cref="T:System.Drawing.Drawing2D.PathPointType" /> and <see cref="T:System.Drawing.Point" /> arrays and with the specified <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration element.</summary>
		/// <param name="pts">An array of <see cref="T:System.Drawing.Point" /> structures that defines the coordinates of the points that make up this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</param>
		/// <param name="types">An array of <see cref="T:System.Drawing.Drawing2D.PathPointType" /> enumeration elements that specifies the type of each corresponding point in the <paramref name="pts" /> array.</param>
		/// <param name="fillMode">A <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration that specifies how the interiors of shapes in this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> are filled.</param>
		public GraphicsPath(Point[] pts, byte[] types, FillMode fillMode)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			if (pts.Length != types.Length)
			{
				throw new ArgumentException("Invalid parameter passed. Number of points and types must be same.");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCreatePath2I(pts, types, pts.Length, fillMode, out this.nativePath));
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> array with the specified <see cref="T:System.Drawing.Drawing2D.PathPointType" /> and <see cref="T:System.Drawing.PointF" /> arrays and with the specified <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration element.</summary>
		/// <param name="pts">An array of <see cref="T:System.Drawing.PointF" /> structures that defines the coordinates of the points that make up this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</param>
		/// <param name="types">An array of <see cref="T:System.Drawing.Drawing2D.PathPointType" /> enumeration elements that specifies the type of each corresponding point in the <paramref name="pts" /> array.</param>
		/// <param name="fillMode">A <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration that specifies how the interiors of shapes in this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> are filled.</param>
		public GraphicsPath(PointF[] pts, byte[] types, FillMode fillMode)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			if (pts.Length != types.Length)
			{
				throw new ArgumentException("Invalid parameter passed. Number of points and types must be same.");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCreatePath2(pts, types, pts.Length, fillMode, out this.nativePath));
		}

		/// <summary>Creates an exact copy of this path.</summary>
		/// <returns>The <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> this method creates, cast as an object.</returns>
		public object Clone()
		{
			IntPtr ptr;
			GDIPlus.CheckStatus(GDIPlus.GdipClonePath(this.nativePath, out ptr));
			return new GraphicsPath(ptr);
		}

		/// <summary>Releases all resources used by this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
		~GraphicsPath()
		{
			this.Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (this.nativePath != IntPtr.Zero)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipDeletePath(this.nativePath));
				this.nativePath = IntPtr.Zero;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration that determines how the interiors of shapes in this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> are filled.</summary>
		/// <returns>A <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration that specifies how the interiors of shapes in this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> are filled.</returns>
		public FillMode FillMode
		{
			get
			{
				FillMode result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPathFillMode(this.nativePath, out result));
				return result;
			}
			set
			{
				if (value < FillMode.Alternate || value > FillMode.Winding)
				{
					throw new InvalidEnumArgumentException("FillMode", (int)value, typeof(FillMode));
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetPathFillMode(this.nativePath, value));
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Drawing2D.PathData" /> that encapsulates arrays of points (<paramref name="points" />) and types (<paramref name="types" />) for this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <returns>A <see cref="T:System.Drawing.Drawing2D.PathData" /> that encapsulates arrays for both the points and types for this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</returns>
		public PathData PathData
		{
			get
			{
				int num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPointCount(this.nativePath, out num));
				PointF[] points = new PointF[num];
				byte[] types = new byte[num];
				if (num > 0)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipGetPathPoints(this.nativePath, points, num));
					GDIPlus.CheckStatus(GDIPlus.GdipGetPathTypes(this.nativePath, types, num));
				}
				return new PathData
				{
					Points = points,
					Types = types
				};
			}
		}

		/// <summary>Gets the points in the path.</summary>
		/// <returns>An array of <see cref="T:System.Drawing.PointF" /> objects that represent the path.</returns>
		public PointF[] PathPoints
		{
			get
			{
				int num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPointCount(this.nativePath, out num));
				if (num == 0)
				{
					throw new ArgumentException("PathPoints");
				}
				PointF[] array = new PointF[num];
				GDIPlus.CheckStatus(GDIPlus.GdipGetPathPoints(this.nativePath, array, num));
				return array;
			}
		}

		/// <summary>Gets the types of the corresponding points in the <see cref="P:System.Drawing.Drawing2D.GraphicsPath.PathPoints" /> array.</summary>
		/// <returns>An array of bytes that specifies the types of the corresponding points in the path.</returns>
		public byte[] PathTypes
		{
			get
			{
				int num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPointCount(this.nativePath, out num));
				if (num == 0)
				{
					throw new ArgumentException("PathTypes");
				}
				byte[] array = new byte[num];
				GDIPlus.CheckStatus(GDIPlus.GdipGetPathTypes(this.nativePath, array, num));
				return array;
			}
		}

		/// <summary>Gets the number of elements in the <see cref="P:System.Drawing.Drawing2D.GraphicsPath.PathPoints" /> or the <see cref="P:System.Drawing.Drawing2D.GraphicsPath.PathTypes" /> array.</summary>
		/// <returns>An integer that specifies the number of elements in the <see cref="P:System.Drawing.Drawing2D.GraphicsPath.PathPoints" /> or the <see cref="P:System.Drawing.Drawing2D.GraphicsPath.PathTypes" /> array.</returns>
		public int PointCount
		{
			get
			{
				int result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPointCount(this.nativePath, out result));
				return result;
			}
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativePath;
			}
			set
			{
				this.nativePath = value;
			}
		}

		/// <summary>Appends an elliptical arc to the current figure.</summary>
		/// <param name="rect">A <see cref="T:System.Drawing.Rectangle" /> that represents the rectangular bounds of the ellipse from which the arc is taken.</param>
		/// <param name="startAngle">The starting angle of the arc, measured in degrees clockwise from the x-axis.</param>
		/// <param name="sweepAngle">The angle between <paramref name="startAngle" /> and the end of the arc.</param>
		public void AddArc(Rectangle rect, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathArcI(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle));
		}

		/// <summary>Appends an elliptical arc to the current figure.</summary>
		/// <param name="rect">A <see cref="T:System.Drawing.RectangleF" /> that represents the rectangular bounds of the ellipse from which the arc is taken.</param>
		/// <param name="startAngle">The starting angle of the arc, measured in degrees clockwise from the x-axis.</param>
		/// <param name="sweepAngle">The angle between <paramref name="startAngle" /> and the end of the arc.</param>
		public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathArc(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle));
		}

		/// <summary>Appends an elliptical arc to the current figure.</summary>
		/// <param name="x">The x-coordinate of the upper-left corner of the rectangular region that defines the ellipse from which the arc is drawn.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the rectangular region that defines the ellipse from which the arc is drawn.</param>
		/// <param name="width">The width of the rectangular region that defines the ellipse from which the arc is drawn.</param>
		/// <param name="height">The height of the rectangular region that defines the ellipse from which the arc is drawn.</param>
		/// <param name="startAngle">The starting angle of the arc, measured in degrees clockwise from the x-axis.</param>
		/// <param name="sweepAngle">The angle between <paramref name="startAngle" /> and the end of the arc.</param>
		public void AddArc(int x, int y, int width, int height, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathArcI(this.nativePath, x, y, width, height, startAngle, sweepAngle));
		}

		/// <summary>Appends an elliptical arc to the current figure.</summary>
		/// <param name="x">The x-coordinate of the upper-left corner of the rectangular region that defines the ellipse from which the arc is drawn.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the rectangular region that defines the ellipse from which the arc is drawn.</param>
		/// <param name="width">The width of the rectangular region that defines the ellipse from which the arc is drawn.</param>
		/// <param name="height">The height of the rectangular region that defines the ellipse from which the arc is drawn.</param>
		/// <param name="startAngle">The starting angle of the arc, measured in degrees clockwise from the x-axis.</param>
		/// <param name="sweepAngle">The angle between <paramref name="startAngle" /> and the end of the arc.</param>
		public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathArc(this.nativePath, x, y, width, height, startAngle, sweepAngle));
		}

		/// <summary>Adds a cubic Bézier curve to the current figure.</summary>
		/// <param name="pt1">A <see cref="T:System.Drawing.Point" /> that represents the starting point of the curve.</param>
		/// <param name="pt2">A <see cref="T:System.Drawing.Point" /> that represents the first control point for the curve.</param>
		/// <param name="pt3">A <see cref="T:System.Drawing.Point" /> that represents the second control point for the curve.</param>
		/// <param name="pt4">A <see cref="T:System.Drawing.Point" /> that represents the endpoint of the curve.</param>
		public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathBezierI(this.nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y));
		}

		/// <summary>Adds a cubic Bézier curve to the current figure.</summary>
		/// <param name="pt1">A <see cref="T:System.Drawing.PointF" /> that represents the starting point of the curve.</param>
		/// <param name="pt2">A <see cref="T:System.Drawing.PointF" /> that represents the first control point for the curve.</param>
		/// <param name="pt3">A <see cref="T:System.Drawing.PointF" /> that represents the second control point for the curve.</param>
		/// <param name="pt4">A <see cref="T:System.Drawing.PointF" /> that represents the endpoint of the curve.</param>
		public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathBezier(this.nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y));
		}

		/// <summary>Adds a cubic Bézier curve to the current figure.</summary>
		/// <param name="x1">The x-coordinate of the starting point of the curve.</param>
		/// <param name="y1">The y-coordinate of the starting point of the curve.</param>
		/// <param name="x2">The x-coordinate of the first control point for the curve.</param>
		/// <param name="y2">The y-coordinate of the first control point for the curve.</param>
		/// <param name="x3">The x-coordinate of the second control point for the curve.</param>
		/// <param name="y3">The y-coordinate of the second control point for the curve.</param>
		/// <param name="x4">The x-coordinate of the endpoint of the curve.</param>
		/// <param name="y4">The y-coordinate of the endpoint of the curve.</param>
		public void AddBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathBezierI(this.nativePath, x1, y1, x2, y2, x3, y3, x4, y4));
		}

		/// <summary>Adds a cubic Bézier curve to the current figure.</summary>
		/// <param name="x1">The x-coordinate of the starting point of the curve.</param>
		/// <param name="y1">The y-coordinate of the starting point of the curve.</param>
		/// <param name="x2">The x-coordinate of the first control point for the curve.</param>
		/// <param name="y2">The y-coordinate of the first control point for the curve.</param>
		/// <param name="x3">The x-coordinate of the second control point for the curve.</param>
		/// <param name="y3">The y-coordinate of the second control point for the curve.</param>
		/// <param name="x4">The x-coordinate of the endpoint of the curve.</param>
		/// <param name="y4">The y-coordinate of the endpoint of the curve.</param>
		public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathBezier(this.nativePath, x1, y1, x2, y2, x3, y3, x4, y4));
		}

		/// <summary>Adds a sequence of connected cubic Bézier curves to the current figure.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.Point" /> structures that represents the points that define the curves.</param>
		public void AddBeziers(params Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathBeziersI(this.nativePath, points, points.Length));
		}

		/// <summary>Adds a sequence of connected cubic Bézier curves to the current figure.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the curves.</param>
		public void AddBeziers(PointF[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathBeziers(this.nativePath, points, points.Length));
		}

		/// <summary>Adds an ellipse to the current path.</summary>
		/// <param name="rect">A <see cref="T:System.Drawing.RectangleF" /> that represents the bounding rectangle that defines the ellipse.</param>
		public void AddEllipse(RectangleF rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathEllipse(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height));
		}

		/// <summary>Adds an ellipse to the current path.</summary>
		/// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.</param>
		/// <param name="y">The y-coordinate of the upper left corner of the bounding rectangle that defines the ellipse.</param>
		/// <param name="width">The width of the bounding rectangle that defines the ellipse.</param>
		/// <param name="height">The height of the bounding rectangle that defines the ellipse.</param>
		public void AddEllipse(float x, float y, float width, float height)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathEllipse(this.nativePath, x, y, width, height));
		}

		/// <summary>Adds an ellipse to the current path.</summary>
		/// <param name="rect">A <see cref="T:System.Drawing.Rectangle" /> that represents the bounding rectangle that defines the ellipse.</param>
		public void AddEllipse(Rectangle rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathEllipseI(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height));
		}

		/// <summary>Adds an ellipse to the current path.</summary>
		/// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.</param>
		/// <param name="width">The width of the bounding rectangle that defines the ellipse.</param>
		/// <param name="height">The height of the bounding rectangle that defines the ellipse.</param>
		public void AddEllipse(int x, int y, int width, int height)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathEllipseI(this.nativePath, x, y, width, height));
		}

		/// <summary>Appends a line segment to this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="pt1">A <see cref="T:System.Drawing.Point" /> that represents the starting point of the line.</param>
		/// <param name="pt2">A <see cref="T:System.Drawing.Point" /> that represents the endpoint of the line.</param>
		public void AddLine(Point pt1, Point pt2)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathLineI(this.nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y));
		}

		/// <summary>Appends a line segment to this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="pt1">A <see cref="T:System.Drawing.PointF" /> that represents the starting point of the line.</param>
		/// <param name="pt2">A <see cref="T:System.Drawing.PointF" /> that represents the endpoint of the line.</param>
		public void AddLine(PointF pt1, PointF pt2)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathLine(this.nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y));
		}

		/// <summary>Appends a line segment to the current figure.</summary>
		/// <param name="x1">The x-coordinate of the starting point of the line.</param>
		/// <param name="y1">The y-coordinate of the starting point of the line.</param>
		/// <param name="x2">The x-coordinate of the endpoint of the line.</param>
		/// <param name="y2">The y-coordinate of the endpoint of the line.</param>
		public void AddLine(int x1, int y1, int x2, int y2)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathLineI(this.nativePath, x1, y1, x2, y2));
		}

		/// <summary>Appends a line segment to this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="x1">The x-coordinate of the starting point of the line.</param>
		/// <param name="y1">The y-coordinate of the starting point of the line.</param>
		/// <param name="x2">The x-coordinate of the endpoint of the line.</param>
		/// <param name="y2">The y-coordinate of the endpoint of the line.</param>
		public void AddLine(float x1, float y1, float x2, float y2)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathLine(this.nativePath, x1, y1, x2, y2));
		}

		/// <summary>Appends a series of connected line segments to the end of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.Point" /> structures that represents the points that define the line segments to add.</param>
		public void AddLines(Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			if (points.Length == 0)
			{
				throw new ArgumentException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathLine2I(this.nativePath, points, points.Length));
		}

		/// <summary>Appends a series of connected line segments to the end of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the line segments to add.</param>
		public void AddLines(PointF[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			if (points.Length == 0)
			{
				throw new ArgumentException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathLine2(this.nativePath, points, points.Length));
		}

		/// <summary>Adds the outline of a pie shape to this path.</summary>
		/// <param name="rect">A <see cref="T:System.Drawing.Rectangle" /> that represents the bounding rectangle that defines the ellipse from which the pie is drawn.</param>
		/// <param name="startAngle">The starting angle for the pie section, measured in degrees clockwise from the x-axis.</param>
		/// <param name="sweepAngle">The angle between <paramref name="startAngle" /> and the end of the pie section, measured in degrees clockwise from <paramref name="startAngle" />.</param>
		public void AddPie(Rectangle rect, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathPie(this.nativePath, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, startAngle, sweepAngle));
		}

		/// <summary>Adds the outline of a pie shape to this path.</summary>
		/// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie is drawn.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie is drawn.</param>
		/// <param name="width">The width of the bounding rectangle that defines the ellipse from which the pie is drawn.</param>
		/// <param name="height">The height of the bounding rectangle that defines the ellipse from which the pie is drawn.</param>
		/// <param name="startAngle">The starting angle for the pie section, measured in degrees clockwise from the x-axis.</param>
		/// <param name="sweepAngle">The angle between <paramref name="startAngle" /> and the end of the pie section, measured in degrees clockwise from <paramref name="startAngle" />.</param>
		public void AddPie(int x, int y, int width, int height, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathPieI(this.nativePath, x, y, width, height, startAngle, sweepAngle));
		}

		/// <summary>Adds the outline of a pie shape to this path.</summary>
		/// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie is drawn.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie is drawn.</param>
		/// <param name="width">The width of the bounding rectangle that defines the ellipse from which the pie is drawn.</param>
		/// <param name="height">The height of the bounding rectangle that defines the ellipse from which the pie is drawn.</param>
		/// <param name="startAngle">The starting angle for the pie section, measured in degrees clockwise from the x-axis.</param>
		/// <param name="sweepAngle">The angle between <paramref name="startAngle" /> and the end of the pie section, measured in degrees clockwise from <paramref name="startAngle" />.</param>
		public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathPie(this.nativePath, x, y, width, height, startAngle, sweepAngle));
		}

		/// <summary>Adds a polygon to this path.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.Point" /> structures that defines the polygon to add.</param>
		public void AddPolygon(Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathPolygonI(this.nativePath, points, points.Length));
		}

		/// <summary>Adds a polygon to this path.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.PointF" /> structures that defines the polygon to add.</param>
		public void AddPolygon(PointF[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathPolygon(this.nativePath, points, points.Length));
		}

		/// <summary>Adds a rectangle to this path.</summary>
		/// <param name="rect">A <see cref="T:System.Drawing.Rectangle" /> that represents the rectangle to add.</param>
		public void AddRectangle(Rectangle rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathRectangleI(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height));
		}

		/// <summary>Adds a rectangle to this path.</summary>
		/// <param name="rect">A <see cref="T:System.Drawing.RectangleF" /> that represents the rectangle to add.</param>
		public void AddRectangle(RectangleF rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathRectangle(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height));
		}

		/// <summary>Adds a series of rectangles to this path.</summary>
		/// <param name="rects">An array of <see cref="T:System.Drawing.Rectangle" /> structures that represents the rectangles to add.</param>
		public void AddRectangles(Rectangle[] rects)
		{
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			if (rects.Length == 0)
			{
				throw new ArgumentException("rects");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathRectanglesI(this.nativePath, rects, rects.Length));
		}

		/// <summary>Adds a series of rectangles to this path.</summary>
		/// <param name="rects">An array of <see cref="T:System.Drawing.RectangleF" /> structures that represents the rectangles to add.</param>
		public void AddRectangles(RectangleF[] rects)
		{
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			if (rects.Length == 0)
			{
				throw new ArgumentException("rects");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathRectangles(this.nativePath, rects, rects.Length));
		}

		/// <summary>Appends the specified <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to this path.</summary>
		/// <param name="addingPath">The <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to add.</param>
		/// <param name="connect">A Boolean value that specifies whether the first figure in the added path is part of the last figure in this path. A value of <see langword="true" /> specifies that (if possible) the first figure in the added path is part of the last figure in this path. A value of <see langword="false" /> specifies that the first figure in the added path is separate from the last figure in this path.</param>
		public void AddPath(GraphicsPath addingPath, bool connect)
		{
			if (addingPath == null)
			{
				throw new ArgumentNullException("addingPath");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathPath(this.nativePath, addingPath.nativePath, connect));
		}

		/// <summary>Gets the last point in the <see cref="P:System.Drawing.Drawing2D.GraphicsPath.PathPoints" /> array of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <returns>A <see cref="T:System.Drawing.PointF" /> that represents the last point in this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</returns>
		public PointF GetLastPoint()
		{
			PointF result;
			GDIPlus.CheckStatus(GDIPlus.GdipGetPathLastPoint(this.nativePath, out result));
			return result;
		}

		/// <summary>Adds a closed curve to this path. A cardinal spline curve is used because the curve travels through each of the points in the array.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.Point" /> structures that represents the points that define the curve.</param>
		public void AddClosedCurve(Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathClosedCurveI(this.nativePath, points, points.Length));
		}

		/// <summary>Adds a closed curve to this path. A cardinal spline curve is used because the curve travels through each of the points in the array.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the curve.</param>
		public void AddClosedCurve(PointF[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathClosedCurve(this.nativePath, points, points.Length));
		}

		/// <summary>Adds a closed curve to this path. A cardinal spline curve is used because the curve travels through each of the points in the array.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.Point" /> structures that represents the points that define the curve.</param>
		/// <param name="tension">A value between from 0 through 1 that specifies the amount that the curve bends between points, with 0 being the smallest curve (sharpest corner) and 1 being the smoothest curve.</param>
		public void AddClosedCurve(Point[] points, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathClosedCurve2I(this.nativePath, points, points.Length, tension));
		}

		/// <summary>Adds a closed curve to this path. A cardinal spline curve is used because the curve travels through each of the points in the array.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the curve.</param>
		/// <param name="tension">A value between from 0 through 1 that specifies the amount that the curve bends between points, with 0 being the smallest curve (sharpest corner) and 1 being the smoothest curve.</param>
		public void AddClosedCurve(PointF[] points, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathClosedCurve2(this.nativePath, points, points.Length, tension));
		}

		/// <summary>Adds a spline curve to the current figure. A cardinal spline curve is used because the curve travels through each of the points in the array.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.Point" /> structures that represents the points that define the curve.</param>
		public void AddCurve(Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurveI(this.nativePath, points, points.Length));
		}

		/// <summary>Adds a spline curve to the current figure. A cardinal spline curve is used because the curve travels through each of the points in the array.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the curve.</param>
		public void AddCurve(PointF[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve(this.nativePath, points, points.Length));
		}

		/// <summary>Adds a spline curve to the current figure.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.Point" /> structures that represents the points that define the curve.</param>
		/// <param name="tension">A value that specifies the amount that the curve bends between control points. Values greater than 1 produce unpredictable results.</param>
		public void AddCurve(Point[] points, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve2I(this.nativePath, points, points.Length, tension));
		}

		/// <summary>Adds a spline curve to the current figure.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the curve.</param>
		/// <param name="tension">A value that specifies the amount that the curve bends between control points. Values greater than 1 produce unpredictable results.</param>
		public void AddCurve(PointF[] points, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve2(this.nativePath, points, points.Length, tension));
		}

		/// <summary>Adds a spline curve to the current figure.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.Point" /> structures that represents the points that define the curve.</param>
		/// <param name="offset">The index of the element in the <paramref name="points" /> array that is used as the first point in the curve.</param>
		/// <param name="numberOfSegments">A value that specifies the amount that the curve bends between control points. Values greater than 1 produce unpredictable results.</param>
		/// <param name="tension">A value that specifies the amount that the curve bends between control points. Values greater than 1 produce unpredictable results.</param>
		public void AddCurve(Point[] points, int offset, int numberOfSegments, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve3I(this.nativePath, points, points.Length, offset, numberOfSegments, tension));
		}

		/// <summary>Adds a spline curve to the current figure.</summary>
		/// <param name="points">An array of <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the curve.</param>
		/// <param name="offset">The index of the element in the <paramref name="points" /> array that is used as the first point in the curve.</param>
		/// <param name="numberOfSegments">The number of segments used to draw the curve. A segment can be thought of as a line connecting two points.</param>
		/// <param name="tension">A value that specifies the amount that the curve bends between control points. Values greater than 1 produce unpredictable results.</param>
		public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve3(this.nativePath, points, points.Length, offset, numberOfSegments, tension));
		}

		/// <summary>Empties the <see cref="P:System.Drawing.Drawing2D.GraphicsPath.PathPoints" /> and <see cref="P:System.Drawing.Drawing2D.GraphicsPath.PathTypes" /> arrays and sets the <see cref="T:System.Drawing.Drawing2D.FillMode" /> to <see cref="F:System.Drawing.Drawing2D.FillMode.Alternate" />.</summary>
		public void Reset()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipResetPath(this.nativePath));
		}

		/// <summary>Reverses the order of points in the <see cref="P:System.Drawing.Drawing2D.GraphicsPath.PathPoints" /> array of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		public void Reverse()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipReversePath(this.nativePath));
		}

		/// <summary>Applies a transform matrix to this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="matrix">A <see cref="T:System.Drawing.Drawing2D.Matrix" /> that represents the transformation to apply.</param>
		public void Transform(Matrix matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipTransformPath(this.nativePath, matrix.nativeMatrix));
		}

		/// <summary>Adds a text string to this path.</summary>
		/// <param name="s">The <see cref="T:System.String" /> to add.</param>
		/// <param name="family">A <see cref="T:System.Drawing.FontFamily" /> that represents the name of the font with which the test is drawn.</param>
		/// <param name="style">A <see cref="T:System.Drawing.FontStyle" /> enumeration that represents style information about the text (bold, italic, and so on). This must be cast as an integer (see the example code later in this section).</param>
		/// <param name="emSize">The height of the em square box that bounds the character.</param>
		/// <param name="origin">A <see cref="T:System.Drawing.Point" /> that represents the point where the text starts.</param>
		/// <param name="format">A <see cref="T:System.Drawing.StringFormat" /> that specifies text formatting information, such as line spacing and alignment.</param>
		[MonoTODO("The StringFormat parameter is ignored when using libgdiplus.")]
		public void AddString(string s, FontFamily family, int style, float emSize, Point origin, StringFormat format)
		{
			this.AddString(s, family, style, emSize, new Rectangle
			{
				X = origin.X,
				Y = origin.Y
			}, format);
		}

		/// <summary>Adds a text string to this path.</summary>
		/// <param name="s">The <see cref="T:System.String" /> to add.</param>
		/// <param name="family">A <see cref="T:System.Drawing.FontFamily" /> that represents the name of the font with which the test is drawn.</param>
		/// <param name="style">A <see cref="T:System.Drawing.FontStyle" /> enumeration that represents style information about the text (bold, italic, and so on). This must be cast as an integer (see the example code later in this section).</param>
		/// <param name="emSize">The height of the em square box that bounds the character.</param>
		/// <param name="origin">A <see cref="T:System.Drawing.PointF" /> that represents the point where the text starts.</param>
		/// <param name="format">A <see cref="T:System.Drawing.StringFormat" /> that specifies text formatting information, such as line spacing and alignment.</param>
		[MonoTODO("The StringFormat parameter is ignored when using libgdiplus.")]
		public void AddString(string s, FontFamily family, int style, float emSize, PointF origin, StringFormat format)
		{
			this.AddString(s, family, style, emSize, new RectangleF
			{
				X = origin.X,
				Y = origin.Y
			}, format);
		}

		/// <summary>Adds a text string to this path.</summary>
		/// <param name="s">The <see cref="T:System.String" /> to add.</param>
		/// <param name="family">A <see cref="T:System.Drawing.FontFamily" /> that represents the name of the font with which the test is drawn.</param>
		/// <param name="style">A <see cref="T:System.Drawing.FontStyle" /> enumeration that represents style information about the text (bold, italic, and so on). This must be cast as an integer (see the example code later in this section).</param>
		/// <param name="emSize">The height of the em square box that bounds the character.</param>
		/// <param name="layoutRect">A <see cref="T:System.Drawing.Rectangle" /> that represents the rectangle that bounds the text.</param>
		/// <param name="format">A <see cref="T:System.Drawing.StringFormat" /> that specifies text formatting information, such as line spacing and alignment.</param>
		[MonoTODO("The layoutRect and StringFormat parameters are ignored when using libgdiplus.")]
		public void AddString(string s, FontFamily family, int style, float emSize, Rectangle layoutRect, StringFormat format)
		{
			if (family == null)
			{
				throw new ArgumentException("family");
			}
			IntPtr format2 = (format == null) ? IntPtr.Zero : format.NativeObject;
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathStringI(this.nativePath, s, s.Length, family.NativeFamily, style, emSize, ref layoutRect, format2));
		}

		/// <summary>Adds a text string to this path.</summary>
		/// <param name="s">The <see cref="T:System.String" /> to add.</param>
		/// <param name="family">A <see cref="T:System.Drawing.FontFamily" /> that represents the name of the font with which the test is drawn.</param>
		/// <param name="style">A <see cref="T:System.Drawing.FontStyle" /> enumeration that represents style information about the text (bold, italic, and so on). This must be cast as an integer (see the example code later in this section).</param>
		/// <param name="emSize">The height of the em square box that bounds the character.</param>
		/// <param name="layoutRect">A <see cref="T:System.Drawing.RectangleF" /> that represents the rectangle that bounds the text.</param>
		/// <param name="format">A <see cref="T:System.Drawing.StringFormat" /> that specifies text formatting information, such as line spacing and alignment.</param>
		[MonoTODO("The layoutRect and StringFormat parameters are ignored when using libgdiplus.")]
		public void AddString(string s, FontFamily family, int style, float emSize, RectangleF layoutRect, StringFormat format)
		{
			if (family == null)
			{
				throw new ArgumentException("family");
			}
			IntPtr format2 = (format == null) ? IntPtr.Zero : format.NativeObject;
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathString(this.nativePath, s, s.Length, family.NativeFamily, style, emSize, ref layoutRect, format2));
		}

		/// <summary>Clears all markers from this path.</summary>
		public void ClearMarkers()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipClearPathMarkers(this.nativePath));
		}

		/// <summary>Closes all open figures in this path and starts a new figure. It closes each open figure by connecting a line from its endpoint to its starting point.</summary>
		public void CloseAllFigures()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipClosePathFigures(this.nativePath));
		}

		/// <summary>Closes the current figure and starts a new figure. If the current figure contains a sequence of connected lines and curves, the method closes the loop by connecting a line from the endpoint to the starting point.</summary>
		public void CloseFigure()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipClosePathFigure(this.nativePath));
		}

		/// <summary>Converts each curve in this path into a sequence of connected line segments.</summary>
		public void Flatten()
		{
			this.Flatten(null, 0.25f);
		}

		/// <summary>Applies the specified transform and then converts each curve in this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> into a sequence of connected line segments.</summary>
		/// <param name="matrix">A <see cref="T:System.Drawing.Drawing2D.Matrix" /> by which to transform this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> before flattening.</param>
		public void Flatten(Matrix matrix)
		{
			this.Flatten(matrix, 0.25f);
		}

		/// <summary>Converts each curve in this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> into a sequence of connected line segments.</summary>
		/// <param name="matrix">A <see cref="T:System.Drawing.Drawing2D.Matrix" /> by which to transform this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> before flattening.</param>
		/// <param name="flatness">Specifies the maximum permitted error between the curve and its flattened approximation. A value of 0.25 is the default. Reducing the flatness value will increase the number of line segments in the approximation.</param>
		public void Flatten(Matrix matrix, float flatness)
		{
			IntPtr matrix2 = (matrix == null) ? IntPtr.Zero : matrix.nativeMatrix;
			GDIPlus.CheckStatus(GDIPlus.GdipFlattenPath(this.nativePath, matrix2, flatness));
		}

		/// <summary>Returns a rectangle that bounds this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <returns>A <see cref="T:System.Drawing.RectangleF" /> that represents a rectangle that bounds this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</returns>
		public RectangleF GetBounds()
		{
			return this.GetBounds(null, null);
		}

		/// <summary>Returns a rectangle that bounds this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when this path is transformed by the specified <see cref="T:System.Drawing.Drawing2D.Matrix" />.</summary>
		/// <param name="matrix">The <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a transformation to be applied to this path before the bounding rectangle is calculated. This path is not permanently transformed; the transformation is used only during the process of calculating the bounding rectangle.</param>
		/// <returns>A <see cref="T:System.Drawing.RectangleF" /> that represents a rectangle that bounds this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</returns>
		public RectangleF GetBounds(Matrix matrix)
		{
			return this.GetBounds(matrix, null);
		}

		/// <summary>Returns a rectangle that bounds this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when the current path is transformed by the specified <see cref="T:System.Drawing.Drawing2D.Matrix" /> and drawn with the specified <see cref="T:System.Drawing.Pen" />.</summary>
		/// <param name="matrix">The <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a transformation to be applied to this path before the bounding rectangle is calculated. This path is not permanently transformed; the transformation is used only during the process of calculating the bounding rectangle.</param>
		/// <param name="pen">The <see cref="T:System.Drawing.Pen" /> with which to draw the <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</param>
		/// <returns>A <see cref="T:System.Drawing.RectangleF" /> that represents a rectangle that bounds this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</returns>
		public RectangleF GetBounds(Matrix matrix, Pen pen)
		{
			IntPtr matrix2 = (matrix == null) ? IntPtr.Zero : matrix.nativeMatrix;
			IntPtr pen2 = (pen == null) ? IntPtr.Zero : pen.NativePen;
			RectangleF result;
			GDIPlus.CheckStatus(GDIPlus.GdipGetPathWorldBounds(this.nativePath, out result, matrix2, pen2));
			return result;
		}

		/// <summary>Indicates whether the specified point is contained within (under) the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified <see cref="T:System.Drawing.Pen" />.</summary>
		/// <param name="point">A <see cref="T:System.Drawing.Point" /> that specifies the location to test.</param>
		/// <param name="pen">The <see cref="T:System.Drawing.Pen" /> to test.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified <see cref="T:System.Drawing.Pen" />; otherwise, <see langword="false" />.</returns>
		public bool IsOutlineVisible(Point point, Pen pen)
		{
			return this.IsOutlineVisible(point.X, point.Y, pen, null);
		}

		/// <summary>Indicates whether the specified point is contained within (under) the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified <see cref="T:System.Drawing.Pen" />.</summary>
		/// <param name="point">A <see cref="T:System.Drawing.PointF" /> that specifies the location to test.</param>
		/// <param name="pen">The <see cref="T:System.Drawing.Pen" /> to test.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified <see cref="T:System.Drawing.Pen" />; otherwise, <see langword="false" />.</returns>
		public bool IsOutlineVisible(PointF point, Pen pen)
		{
			return this.IsOutlineVisible(point.X, point.Y, pen, null);
		}

		/// <summary>Indicates whether the specified point is contained within (under) the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified <see cref="T:System.Drawing.Pen" />.</summary>
		/// <param name="x">The x-coordinate of the point to test.</param>
		/// <param name="y">The y-coordinate of the point to test.</param>
		/// <param name="pen">The <see cref="T:System.Drawing.Pen" /> to test.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified <see cref="T:System.Drawing.Pen" />; otherwise, <see langword="false" />.</returns>
		public bool IsOutlineVisible(int x, int y, Pen pen)
		{
			return this.IsOutlineVisible(x, y, pen, null);
		}

		/// <summary>Indicates whether the specified point is contained within (under) the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified <see cref="T:System.Drawing.Pen" />.</summary>
		/// <param name="x">The x-coordinate of the point to test.</param>
		/// <param name="y">The y-coordinate of the point to test.</param>
		/// <param name="pen">The <see cref="T:System.Drawing.Pen" /> to test.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified <see cref="T:System.Drawing.Pen" />; otherwise, <see langword="false" />.</returns>
		public bool IsOutlineVisible(float x, float y, Pen pen)
		{
			return this.IsOutlineVisible(x, y, pen, null);
		}

		/// <summary>Indicates whether the specified point is contained within (under) the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified <see cref="T:System.Drawing.Pen" /> and using the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="pt">A <see cref="T:System.Drawing.Point" /> that specifies the location to test.</param>
		/// <param name="pen">The <see cref="T:System.Drawing.Pen" /> to test.</param>
		/// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> for which to test visibility.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> as drawn with the specified <see cref="T:System.Drawing.Pen" />; otherwise, <see langword="false" />.</returns>
		public bool IsOutlineVisible(Point pt, Pen pen, Graphics graphics)
		{
			return this.IsOutlineVisible(pt.X, pt.Y, pen, graphics);
		}

		/// <summary>Indicates whether the specified point is contained within (under) the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified <see cref="T:System.Drawing.Pen" /> and using the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="pt">A <see cref="T:System.Drawing.PointF" /> that specifies the location to test.</param>
		/// <param name="pen">The <see cref="T:System.Drawing.Pen" /> to test.</param>
		/// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> for which to test visibility.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within (under) the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> as drawn with the specified <see cref="T:System.Drawing.Pen" />; otherwise, <see langword="false" />.</returns>
		public bool IsOutlineVisible(PointF pt, Pen pen, Graphics graphics)
		{
			return this.IsOutlineVisible(pt.X, pt.Y, pen, graphics);
		}

		/// <summary>Indicates whether the specified point is contained within (under) the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified <see cref="T:System.Drawing.Pen" /> and using the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="x">The x-coordinate of the point to test.</param>
		/// <param name="y">The y-coordinate of the point to test.</param>
		/// <param name="pen">The <see cref="T:System.Drawing.Pen" /> to test.</param>
		/// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> for which to test visibility.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> as drawn with the specified <see cref="T:System.Drawing.Pen" />; otherwise, <see langword="false" />.</returns>
		public bool IsOutlineVisible(int x, int y, Pen pen, Graphics graphics)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			IntPtr graphics2 = (graphics == null) ? IntPtr.Zero : graphics.nativeObject;
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsOutlineVisiblePathPointI(this.nativePath, x, y, pen.NativePen, graphics2, out result));
			return result;
		}

		/// <summary>Indicates whether the specified point is contained within (under) the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified <see cref="T:System.Drawing.Pen" /> and using the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="x">The x-coordinate of the point to test.</param>
		/// <param name="y">The y-coordinate of the point to test.</param>
		/// <param name="pen">The <see cref="T:System.Drawing.Pen" /> to test.</param>
		/// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> for which to test visibility.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within (under) the outline of this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> as drawn with the specified <see cref="T:System.Drawing.Pen" />; otherwise, <see langword="false" />.</returns>
		public bool IsOutlineVisible(float x, float y, Pen pen, Graphics graphics)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			IntPtr graphics2 = (graphics == null) ? IntPtr.Zero : graphics.nativeObject;
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsOutlineVisiblePathPoint(this.nativePath, x, y, pen.NativePen, graphics2, out result));
			return result;
		}

		/// <summary>Indicates whether the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="point">A <see cref="T:System.Drawing.Point" /> that represents the point to test.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(Point point)
		{
			return this.IsVisible(point.X, point.Y, null);
		}

		/// <summary>Indicates whether the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="point">A <see cref="T:System.Drawing.PointF" /> that represents the point to test.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(PointF point)
		{
			return this.IsVisible(point.X, point.Y, null);
		}

		/// <summary>Indicates whether the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="x">The x-coordinate of the point to test.</param>
		/// <param name="y">The y-coordinate of the point to test.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(int x, int y)
		{
			return this.IsVisible(x, y, null);
		}

		/// <summary>Indicates whether the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="x">The x-coordinate of the point to test.</param>
		/// <param name="y">The y-coordinate of the point to test.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(float x, float y)
		{
			return this.IsVisible(x, y, null);
		}

		/// <summary>Indicates whether the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="pt">A <see cref="T:System.Drawing.Point" /> that represents the point to test.</param>
		/// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> for which to test visibility.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(Point pt, Graphics graphics)
		{
			return this.IsVisible(pt.X, pt.Y, graphics);
		}

		/// <summary>Indicates whether the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="pt">A <see cref="T:System.Drawing.PointF" /> that represents the point to test.</param>
		/// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> for which to test visibility.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within this; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(PointF pt, Graphics graphics)
		{
			return this.IsVisible(pt.X, pt.Y, graphics);
		}

		/// <summary>Indicates whether the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />, using the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="x">The x-coordinate of the point to test.</param>
		/// <param name="y">The y-coordinate of the point to test.</param>
		/// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> for which to test visibility.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(int x, int y, Graphics graphics)
		{
			IntPtr graphics2 = (graphics == null) ? IntPtr.Zero : graphics.nativeObject;
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisiblePathPointI(this.nativePath, x, y, graphics2, out result));
			return result;
		}

		/// <summary>Indicates whether the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> in the visible clip region of the specified <see cref="T:System.Drawing.Graphics" />.</summary>
		/// <param name="x">The x-coordinate of the point to test.</param>
		/// <param name="y">The y-coordinate of the point to test.</param>
		/// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> for which to test visibility.</param>
		/// <returns>This method returns <see langword="true" /> if the specified point is contained within this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />; otherwise, <see langword="false" />.</returns>
		public bool IsVisible(float x, float y, Graphics graphics)
		{
			IntPtr graphics2 = (graphics == null) ? IntPtr.Zero : graphics.nativeObject;
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisiblePathPoint(this.nativePath, x, y, graphics2, out result));
			return result;
		}

		/// <summary>Sets a marker on this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		public void SetMarkers()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetPathMarker(this.nativePath));
		}

		/// <summary>Starts a new figure without closing the current figure. All subsequent points added to the path are added to this new figure.</summary>
		public void StartFigure()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipStartPathFigure(this.nativePath));
		}

		/// <summary>Applies a warp transform, defined by a rectangle and a parallelogram, to this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="destPoints">An array of <see cref="T:System.Drawing.PointF" /> structures that define a parallelogram to which the rectangle defined by <paramref name="srcRect" /> is transformed. The array can contain either three or four elements. If the array contains three elements, the lower-right corner of the parallelogram is implied by the first three points.</param>
		/// <param name="srcRect">A <see cref="T:System.Drawing.RectangleF" /> that represents the rectangle that is transformed to the parallelogram defined by <paramref name="destPoints" />.</param>
		[MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
		public void Warp(PointF[] destPoints, RectangleF srcRect)
		{
			this.Warp(destPoints, srcRect, null, WarpMode.Perspective, 0.25f);
		}

		/// <summary>Applies a warp transform, defined by a rectangle and a parallelogram, to this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="destPoints">An array of <see cref="T:System.Drawing.PointF" /> structures that define a parallelogram to which the rectangle defined by <paramref name="srcRect" /> is transformed. The array can contain either three or four elements. If the array contains three elements, the lower-right corner of the parallelogram is implied by the first three points.</param>
		/// <param name="srcRect">A <see cref="T:System.Drawing.RectangleF" /> that represents the rectangle that is transformed to the parallelogram defined by <paramref name="destPoints" />.</param>
		/// <param name="matrix">A <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a geometric transform to apply to the path.</param>
		[MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
		public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix)
		{
			this.Warp(destPoints, srcRect, matrix, WarpMode.Perspective, 0.25f);
		}

		/// <summary>Applies a warp transform, defined by a rectangle and a parallelogram, to this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="destPoints">An array of <see cref="T:System.Drawing.PointF" /> structures that defines a parallelogram to which the rectangle defined by <paramref name="srcRect" /> is transformed. The array can contain either three or four elements. If the array contains three elements, the lower-right corner of the parallelogram is implied by the first three points.</param>
		/// <param name="srcRect">A <see cref="T:System.Drawing.RectangleF" /> that represents the rectangle that is transformed to the parallelogram defined by <paramref name="destPoints" />.</param>
		/// <param name="matrix">A <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a geometric transform to apply to the path.</param>
		/// <param name="warpMode">A <see cref="T:System.Drawing.Drawing2D.WarpMode" /> enumeration that specifies whether this warp operation uses perspective or bilinear mode.</param>
		[MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
		public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode)
		{
			this.Warp(destPoints, srcRect, matrix, warpMode, 0.25f);
		}

		/// <summary>Applies a warp transform, defined by a rectangle and a parallelogram, to this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="destPoints">An array of <see cref="T:System.Drawing.PointF" /> structures that define a parallelogram to which the rectangle defined by <paramref name="srcRect" /> is transformed. The array can contain either three or four elements. If the array contains three elements, the lower-right corner of the parallelogram is implied by the first three points.</param>
		/// <param name="srcRect">A <see cref="T:System.Drawing.RectangleF" /> that represents the rectangle that is transformed to the parallelogram defined by <paramref name="destPoints" />.</param>
		/// <param name="matrix">A <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a geometric transform to apply to the path.</param>
		/// <param name="warpMode">A <see cref="T:System.Drawing.Drawing2D.WarpMode" /> enumeration that specifies whether this warp operation uses perspective or bilinear mode.</param>
		/// <param name="flatness">A value from 0 through 1 that specifies how flat the resulting path is. For more information, see the <see cref="M:System.Drawing.Drawing2D.GraphicsPath.Flatten" /> methods.</param>
		[MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
		public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode, float flatness)
		{
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			IntPtr matrix2 = (matrix == null) ? IntPtr.Zero : matrix.nativeMatrix;
			GDIPlus.CheckStatus(GDIPlus.GdipWarpPath(this.nativePath, matrix2, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, warpMode, flatness));
		}

		/// <summary>Adds an additional outline to the path.</summary>
		/// <param name="pen">A <see cref="T:System.Drawing.Pen" /> that specifies the width between the original outline of the path and the new outline this method creates.</param>
		[MonoTODO("GdipWidenPath isn't implemented in libgdiplus")]
		public void Widen(Pen pen)
		{
			this.Widen(pen, null, 0.25f);
		}

		/// <summary>Adds an additional outline to the <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.</summary>
		/// <param name="pen">A <see cref="T:System.Drawing.Pen" /> that specifies the width between the original outline of the path and the new outline this method creates.</param>
		/// <param name="matrix">A <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a transform to apply to the path before widening.</param>
		[MonoTODO("GdipWidenPath isn't implemented in libgdiplus")]
		public void Widen(Pen pen, Matrix matrix)
		{
			this.Widen(pen, matrix, 0.25f);
		}

		/// <summary>Replaces this <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> with curves that enclose the area that is filled when this path is drawn by the specified pen.</summary>
		/// <param name="pen">A <see cref="T:System.Drawing.Pen" /> that specifies the width between the original outline of the path and the new outline this method creates.</param>
		/// <param name="matrix">A <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a transform to apply to the path before widening.</param>
		/// <param name="flatness">A value that specifies the flatness for curves.</param>
		[MonoTODO("GdipWidenPath isn't implemented in libgdiplus")]
		public void Widen(Pen pen, Matrix matrix, float flatness)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (this.PointCount == 0)
			{
				return;
			}
			IntPtr matrix2 = (matrix == null) ? IntPtr.Zero : matrix.nativeMatrix;
			GDIPlus.CheckStatus(GDIPlus.GdipWidenPath(this.nativePath, pen.NativePen, matrix2, flatness));
		}

		private const float FlatnessDefault = 0.25f;

		internal IntPtr nativePath = IntPtr.Zero;
	}
}
