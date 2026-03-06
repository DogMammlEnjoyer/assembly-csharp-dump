using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace g3
{
	public class SVGWriter
	{
		public SVGWriter()
		{
			this.Objects = new List<object>();
			this.Bounds = AxisAlignedBox2d.Empty;
			this.DefaultPolygonStyle = SVGWriter.Style.Outline("grey", 1f);
			this.DefaultPolylineStyle = SVGWriter.Style.Outline("cyan", 1f);
			this.DefaultCircleStyle = SVGWriter.Style.Filled("green", "black", 1f);
			this.DefaultArcStyle = SVGWriter.Style.Outline("magenta", 1f);
			this.DefaultLineStyle = SVGWriter.Style.Outline("black", 1f);
			this.DefaultDGraphStyle = SVGWriter.Style.Outline("blue", 1f);
		}

		public void SetDefaultLineWidth(float width)
		{
			this.DefaultPolygonStyle.stroke_width = width;
			this.DefaultPolylineStyle.stroke_width = width;
			this.DefaultCircleStyle.stroke_width = width;
			this.DefaultArcStyle.stroke_width = width;
			this.DefaultLineStyle.stroke_width = width;
			this.DefaultDGraphStyle.stroke_width = width;
		}

		public void AddPolygon(Polygon2d poly)
		{
			this.Objects.Add(poly);
			this.Bounds.Contain(poly.Bounds);
		}

		public void AddPolygon(Polygon2d poly, SVGWriter.Style style)
		{
			this.Objects.Add(poly);
			this.Styles[poly] = style;
			this.Bounds.Contain(poly.Bounds);
		}

		public void AddBox(AxisAlignedBox2d box)
		{
			this.AddBox(box, this.DefaultPolygonStyle);
		}

		public void AddBox(AxisAlignedBox2d box, SVGWriter.Style style)
		{
			Polygon2d polygon2d = new Polygon2d();
			for (int i = 0; i < 4; i++)
			{
				polygon2d.AppendVertex(box.GetCorner(i));
			}
			this.AddPolygon(polygon2d, style);
		}

		public void AddPolyline(PolyLine2d poly)
		{
			this.Objects.Add(poly);
			this.Bounds.Contain(poly.Bounds);
		}

		public void AddPolyline(PolyLine2d poly, SVGWriter.Style style)
		{
			this.Objects.Add(poly);
			this.Styles[poly] = style;
			this.Bounds.Contain(poly.Bounds);
		}

		public void AddGraph(DGraph2 graph)
		{
			this.Objects.Add(graph);
			this.Bounds.Contain(graph.GetBounds());
		}

		public void AddGraph(DGraph2 graph, SVGWriter.Style style)
		{
			this.Objects.Add(graph);
			this.Styles[graph] = style;
			this.Bounds.Contain(graph.GetBounds());
		}

		public void AddCircle(Circle2d circle)
		{
			this.Objects.Add(circle);
			this.Bounds.Contain(circle.Bounds);
		}

		public void AddCircle(Circle2d circle, SVGWriter.Style style)
		{
			this.Objects.Add(circle);
			this.Styles[circle] = style;
			this.Bounds.Contain(circle.Bounds);
		}

		public void AddArc(Arc2d arc)
		{
			this.Objects.Add(arc);
			this.Bounds.Contain(arc.Bounds);
		}

		public void AddArc(Arc2d arc, SVGWriter.Style style)
		{
			this.Objects.Add(arc);
			this.Styles[arc] = style;
			this.Bounds.Contain(arc.Bounds);
		}

		public void AddLine(Segment2d segment)
		{
			this.Objects.Add(new Segment2dBox(segment));
			this.Bounds.Contain(segment.P0);
			this.Bounds.Contain(segment.P1);
		}

		public void AddLine(Segment2d segment, SVGWriter.Style style)
		{
			Segment2dBox segment2dBox = new Segment2dBox(segment);
			this.Objects.Add(segment2dBox);
			this.Styles[segment2dBox] = style;
			this.Bounds.Contain(segment.P0);
			this.Bounds.Contain(segment.P1);
		}

		public void AddComplex(PlanarComplex complex)
		{
			this.Objects.Add(complex);
			this.Bounds.Contain(complex.Bounds());
		}

		public IOWriteResult Write(string sFilename)
		{
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			IOWriteResult result;
			try
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
				using (StreamWriter streamWriter = new StreamWriter(sFilename))
				{
					if (streamWriter.BaseStream == null)
					{
						return new IOWriteResult(IOCode.FileAccessError, "Could not open file " + sFilename + " for writing");
					}
					this.write_header_1_1(streamWriter);
					foreach (object obj in this.Objects)
					{
						if (obj is Polygon2d)
						{
							this.write_polygon(obj as Polygon2d, streamWriter);
						}
						else if (obj is PolyLine2d)
						{
							this.write_polyline(obj as PolyLine2d, streamWriter);
						}
						else if (obj is Circle2d)
						{
							this.write_circle(obj as Circle2d, streamWriter);
						}
						else if (obj is Arc2d)
						{
							this.write_arc(obj as Arc2d, streamWriter);
						}
						else if (obj is Segment2dBox)
						{
							this.write_line(obj as Segment2dBox, streamWriter);
						}
						else if (obj is DGraph2)
						{
							this.write_graph(obj as DGraph2, streamWriter);
						}
						else
						{
							if (!(obj is PlanarComplex))
							{
								throw new Exception("SVGWriter.Write: unknown object type " + obj.GetType().ToString());
							}
							this.write_complex(obj as PlanarComplex, streamWriter);
						}
					}
					streamWriter.WriteLine("</svg>");
				}
				Thread.CurrentThread.CurrentCulture = currentCulture;
				result = IOWriteResult.Ok;
			}
			catch (Exception ex)
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
				result = new IOWriteResult(IOCode.WriterError, "Unknown error : exception : " + ex.Message);
			}
			return result;
		}

		public static void QuickWrite(List<GeneralPolygon2d> polygons, string sPath, double line_width = 1.0)
		{
			SVGWriter svgwriter = new SVGWriter();
			SVGWriter.Style style = SVGWriter.Style.Outline("black", 2f * (float)line_width);
			SVGWriter.Style style2 = SVGWriter.Style.Outline("green", 2f * (float)line_width);
			SVGWriter.Style style3 = SVGWriter.Style.Outline("red", (float)line_width);
			foreach (GeneralPolygon2d generalPolygon2d in polygons)
			{
				if (generalPolygon2d.Outer.IsClockwise)
				{
					svgwriter.AddPolygon(generalPolygon2d.Outer, style);
				}
				else
				{
					svgwriter.AddPolygon(generalPolygon2d.Outer, style2);
				}
				foreach (Polygon2d poly in generalPolygon2d.Holes)
				{
					svgwriter.AddPolygon(poly, style3);
				}
			}
			svgwriter.Write(sPath);
		}

		public static void QuickWrite(DGraph2 graph, string sPath, double line_width = 1.0)
		{
			SVGWriter svgwriter = new SVGWriter();
			SVGWriter.Style style = SVGWriter.Style.Outline("black", (float)line_width);
			svgwriter.AddGraph(graph, style);
			svgwriter.Write(sPath);
		}

		public static void QuickWrite(List<GeneralPolygon2d> polygons1, string color1, float width1, List<GeneralPolygon2d> polygons2, string color2, float width2, string sPath)
		{
			SVGWriter svgwriter = new SVGWriter();
			SVGWriter.Style style = SVGWriter.Style.Outline(color1, width1);
			SVGWriter.Style style2 = SVGWriter.Style.Outline(color1, width1 / 2f);
			foreach (GeneralPolygon2d generalPolygon2d in polygons1)
			{
				svgwriter.AddPolygon(generalPolygon2d.Outer, style);
				foreach (Polygon2d poly in generalPolygon2d.Holes)
				{
					svgwriter.AddPolygon(poly, style2);
				}
			}
			SVGWriter.Style style3 = SVGWriter.Style.Outline(color2, width2);
			SVGWriter.Style style4 = SVGWriter.Style.Outline(color2, width2 / 2f);
			foreach (GeneralPolygon2d generalPolygon2d2 in polygons2)
			{
				svgwriter.AddPolygon(generalPolygon2d2.Outer, style3);
				foreach (Polygon2d poly2 in generalPolygon2d2.Holes)
				{
					svgwriter.AddPolygon(poly2, style4);
				}
			}
			svgwriter.Write(sPath);
		}

		protected virtual Vector2d MapPt(Vector2d v)
		{
			if (this.FlipY)
			{
				return new Vector2d(v.x, this.Bounds.Min.y + (this.Bounds.Max.y - v.y));
			}
			return v;
		}

		private void write_header_1_1(StreamWriter w)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<svg ");
			stringBuilder.Append("version=\"1.1\" ");
			stringBuilder.Append("xmlns=\"http://www.w3.org/2000/svg\" ");
			stringBuilder.Append("xmlns:xlink=\"http://www.w3.org/1999/xlink\" ");
			stringBuilder.Append("x=\"0px\" y=\"0px\" ");
			stringBuilder.Append(string.Format("viewBox=\"{0} {1} {2} {3}\" ", new object[]
			{
				Math.Round(this.Bounds.Min.x - this.BoundsPad, this.Precision),
				Math.Round(this.Bounds.Min.y - this.BoundsPad, this.Precision),
				Math.Round(this.Bounds.Width + 2.0 * this.BoundsPad, this.Precision),
				Math.Round(this.Bounds.Height + 2.0 * this.BoundsPad, this.Precision)
			}));
			stringBuilder.Append('>');
			w.WriteLine(stringBuilder);
		}

		private void write_polygon(Polygon2d poly, StreamWriter w)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<polygon points=\"");
			for (int i = 0; i < poly.VertexCount; i++)
			{
				Vector2d vector2d = this.MapPt(poly[i]);
				stringBuilder.Append(Math.Round(vector2d.x, this.Precision));
				stringBuilder.Append(',');
				stringBuilder.Append(Math.Round(vector2d.y, this.Precision));
				if (i < poly.VertexCount - 1)
				{
					stringBuilder.Append(' ');
				}
			}
			stringBuilder.Append("\" ");
			this.append_style(stringBuilder, poly, ref this.DefaultPolygonStyle);
			stringBuilder.Append(" />");
			w.WriteLine(stringBuilder);
		}

		private void write_polyline(PolyLine2d poly, StreamWriter w)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<polyline points=\"");
			for (int i = 0; i < poly.VertexCount; i++)
			{
				Vector2d vector2d = this.MapPt(poly[i]);
				stringBuilder.Append(Math.Round(vector2d.x, this.Precision));
				stringBuilder.Append(',');
				stringBuilder.Append(Math.Round(vector2d.y, this.Precision));
				if (i < poly.VertexCount - 1)
				{
					stringBuilder.Append(' ');
				}
			}
			stringBuilder.Append("\" ");
			this.append_style(stringBuilder, poly, ref this.DefaultPolylineStyle);
			stringBuilder.Append(" />");
			w.WriteLine(stringBuilder);
		}

		private void write_graph(DGraph2 graph, StreamWriter w)
		{
			string value = this.get_style(graph, ref this.DefaultDGraphStyle);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int eID in graph.EdgeIndices())
			{
				Segment2d edgeSegment = graph.GetEdgeSegment(eID);
				stringBuilder.Append("<line ");
				Vector2d vector2d = this.MapPt(edgeSegment.P0);
				Vector2d vector2d2 = this.MapPt(edgeSegment.P1);
				this.append_property("x1", vector2d.x, stringBuilder, true);
				this.append_property("y1", vector2d.y, stringBuilder, true);
				this.append_property("x2", vector2d2.x, stringBuilder, true);
				this.append_property("y2", vector2d2.y, stringBuilder, true);
				stringBuilder.Append(value);
				stringBuilder.Append(" />");
				stringBuilder.AppendLine();
			}
			w.WriteLine(stringBuilder);
		}

		private void write_circle(Circle2d circle, StreamWriter w)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<circle ");
			Vector2d vector2d = this.MapPt(circle.Center);
			this.append_property("cx", vector2d.x, stringBuilder, true);
			this.append_property("cy", vector2d.y, stringBuilder, true);
			this.append_property("r", circle.Radius, stringBuilder, true);
			this.append_style(stringBuilder, circle, ref this.DefaultCircleStyle);
			stringBuilder.Append(" />");
			w.WriteLine(stringBuilder);
		}

		private void write_arc(Arc2d arc, StreamWriter w)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Vector2d vector2d = this.MapPt(arc.P0);
			Vector2d vector2d2 = this.MapPt(arc.P1);
			stringBuilder.Append("<path ");
			stringBuilder.Append("d=\"");
			stringBuilder.Append("M");
			stringBuilder.Append(Math.Round(vector2d.x, this.Precision));
			stringBuilder.Append(",");
			stringBuilder.Append(Math.Round(vector2d.y, this.Precision));
			stringBuilder.Append(" ");
			stringBuilder.Append("A");
			stringBuilder.Append(Math.Round(arc.Radius, this.Precision));
			stringBuilder.Append(",");
			stringBuilder.Append(Math.Round(arc.Radius, this.Precision));
			stringBuilder.Append(" ");
			stringBuilder.Append("0 ");
			int value = (arc.AngleEndDeg - arc.AngleStartDeg > 180.0) ? 1 : 0;
			int value2 = arc.IsReversed ? 1 : 0;
			stringBuilder.Append(value);
			stringBuilder.Append(",");
			stringBuilder.Append(value2);
			stringBuilder.Append(Math.Round(vector2d2.x, this.Precision));
			stringBuilder.Append(",");
			stringBuilder.Append(Math.Round(vector2d2.y, this.Precision));
			stringBuilder.Append("\" ");
			this.append_style(stringBuilder, arc, ref this.DefaultArcStyle);
			stringBuilder.Append(" />");
			w.WriteLine(stringBuilder);
		}

		private void write_line(Segment2dBox segbox, StreamWriter w)
		{
			Segment2d segment2d = segbox;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<line ");
			Vector2d vector2d = this.MapPt(segment2d.P0);
			Vector2d vector2d2 = this.MapPt(segment2d.P1);
			this.append_property("x1", vector2d.x, stringBuilder, true);
			this.append_property("y1", vector2d.y, stringBuilder, true);
			this.append_property("x2", vector2d2.x, stringBuilder, true);
			this.append_property("y2", vector2d2.y, stringBuilder, true);
			this.append_style(stringBuilder, segbox, ref this.DefaultLineStyle);
			stringBuilder.Append(" />");
			w.WriteLine(stringBuilder);
		}

		private void write_complex(PlanarComplex complex, StreamWriter w)
		{
			foreach (PlanarComplex.Element element in complex.ElementsItr())
			{
				foreach (IParametricCurve2d parametricCurve2d in CurveUtils2.Flatten(element.source))
				{
					if (parametricCurve2d is Segment2d)
					{
						this.write_line(new Segment2dBox((Segment2d)parametricCurve2d), w);
					}
					else if (parametricCurve2d is Circle2d)
					{
						this.write_circle(parametricCurve2d as Circle2d, w);
					}
					else if (parametricCurve2d is Polygon2DCurve)
					{
						this.write_polygon((parametricCurve2d as Polygon2DCurve).Polygon, w);
					}
					else if (parametricCurve2d is PolyLine2DCurve)
					{
						this.write_polyline((parametricCurve2d as PolyLine2DCurve).Polyline, w);
					}
					else if (parametricCurve2d is Arc2d)
					{
						this.write_arc(parametricCurve2d as Arc2d, w);
					}
				}
			}
		}

		private void append_property(string name, double val, StringBuilder b, bool trailSpace = true)
		{
			b.Append(name);
			b.Append("=\"");
			b.Append(Math.Round(val, this.Precision));
			if (trailSpace)
			{
				b.Append("\" ");
				return;
			}
			b.Append("\"");
		}

		private void append_style(StringBuilder b, object o, ref SVGWriter.Style defaultStyle)
		{
			SVGWriter.Style style;
			if (!this.Styles.TryGetValue(o, out style))
			{
				style = defaultStyle;
			}
			b.Append("style=\"");
			b.Append(style.ToString());
			b.Append("\"");
		}

		private string get_style(object o, ref SVGWriter.Style defaultStyle)
		{
			SVGWriter.Style style;
			if (!this.Styles.TryGetValue(o, out style))
			{
				style = defaultStyle;
			}
			return "style=\"" + style.ToString() + "\"";
		}

		public bool FlipY = true;

		private Dictionary<object, SVGWriter.Style> Styles = new Dictionary<object, SVGWriter.Style>();

		public SVGWriter.Style DefaultPolygonStyle;

		public SVGWriter.Style DefaultPolylineStyle;

		public SVGWriter.Style DefaultDGraphStyle;

		public SVGWriter.Style DefaultCircleStyle;

		public SVGWriter.Style DefaultArcStyle;

		public SVGWriter.Style DefaultLineStyle;

		private List<object> Objects;

		private AxisAlignedBox2d Bounds;

		public int Precision = 3;

		public double BoundsPad = 10.0;

		public struct Style
		{
			public static SVGWriter.Style Filled(string fillCol, string strokeCol = "", float strokeWidth = 0f)
			{
				return new SVGWriter.Style
				{
					fill = fillCol,
					stroke = strokeCol,
					stroke_width = strokeWidth
				};
			}

			public static SVGWriter.Style Outline(string strokeCol, float strokeWidth)
			{
				return new SVGWriter.Style
				{
					fill = "none",
					stroke = strokeCol,
					stroke_width = strokeWidth
				};
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this.fill.Length > 0)
				{
					stringBuilder.Append("fill:");
					stringBuilder.Append(this.fill);
					stringBuilder.Append(';');
				}
				if (this.stroke.Length > 0)
				{
					stringBuilder.Append("stroke:");
					stringBuilder.Append(this.stroke);
					stringBuilder.Append(';');
				}
				if (this.stroke_width > 0f)
				{
					stringBuilder.Append("stroke-width:");
					stringBuilder.Append(this.stroke_width);
					stringBuilder.Append(";");
				}
				return stringBuilder.ToString();
			}

			public string fill;

			public string stroke;

			public float stroke_width;

			public static readonly SVGWriter.Style Default = new SVGWriter.Style
			{
				fill = "none",
				stroke = "black",
				stroke_width = 1f
			};
		}
	}
}
