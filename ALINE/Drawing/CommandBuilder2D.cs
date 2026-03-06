using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Drawing
{
	public struct CommandBuilder2D
	{
		public CommandBuilder2D(CommandBuilder draw, bool xy)
		{
			this.draw = draw;
			this.xy = xy;
		}

		public unsafe void Line(float2 a, float2 b)
		{
			this.draw.Reserve<CommandBuilder.LineData>();
			UnsafeAppendBuffer* buffer = this.draw.buffer;
			int length = buffer->Length;
			int length2 = length + 4 + 24;
			byte* ptr = buffer->Ptr + length;
			*(int*)ptr = 5;
			CommandBuilder.LineData* ptr2 = (CommandBuilder.LineData*)(ptr + 4);
			if (this.xy)
			{
				ptr2->a = new float3(a, 0f);
				ptr2->b = new float3(b, 0f);
			}
			else
			{
				ptr2->a = new float3(a.x, 0f, a.y);
				ptr2->b = new float3(b.x, 0f, b.y);
			}
			buffer->Length = length2;
		}

		public unsafe void Line(float2 a, float2 b, Color color)
		{
			this.draw.Reserve<Color32, CommandBuilder.LineData>();
			UnsafeAppendBuffer* buffer = this.draw.buffer;
			int length = buffer->Length;
			int length2 = length + 4 + 24 + 4;
			byte* ptr = buffer->Ptr + length;
			*(int*)ptr = 261;
			*(int*)(ptr + 4) = (int)CommandBuilder.ConvertColor(color);
			CommandBuilder.LineData* ptr2 = (CommandBuilder.LineData*)(ptr + 8);
			if (this.xy)
			{
				ptr2->a = new float3(a, 0f);
				ptr2->b = new float3(b, 0f);
			}
			else
			{
				ptr2->a = new float3(a.x, 0f, a.y);
				ptr2->b = new float3(b.x, 0f, b.y);
			}
			buffer->Length = length2;
		}

		public void Line(float3 a, float3 b)
		{
			this.draw.Line(a, b);
		}

		public void Circle(float2 center, float radius, float startAngle = 0f, float endAngle = 6.2831855f)
		{
			this.Circle(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), radius, startAngle, endAngle);
		}

		public void Circle(float3 center, float radius, float startAngle = 0f, float endAngle = 6.2831855f)
		{
			if (this.xy)
			{
				this.draw.PushMatrix(CommandBuilder2D.XZ_TO_XY_MATRIX);
				this.draw.CircleXZInternal(new float3(center.x, center.z, center.y), radius, startAngle, endAngle);
				this.draw.PopMatrix();
				return;
			}
			this.draw.CircleXZInternal(center, radius, startAngle, endAngle);
		}

		public void SolidCircle(float2 center, float radius, float startAngle = 0f, float endAngle = 6.2831855f)
		{
			this.SolidCircle(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), radius, startAngle, endAngle);
		}

		public void SolidCircle(float3 center, float radius, float startAngle = 0f, float endAngle = 6.2831855f)
		{
			if (this.xy)
			{
				this.draw.PushMatrix(CommandBuilder2D.XZ_TO_XY_MATRIX);
			}
			this.draw.SolidCircleXZInternal(new float3(center.x, -center.z, center.y), radius, startAngle, endAngle);
			if (this.xy)
			{
				this.draw.PopMatrix();
			}
		}

		public void WirePill(float2 a, float2 b, float radius)
		{
			this.WirePill(a, b - a, math.length(b - a), radius);
		}

		public void WirePill(float2 position, float2 direction, float length, float radius)
		{
			direction = math.normalizesafe(direction, default(float2));
			if (radius <= 0f)
			{
				this.Line(position, position + direction * length);
				return;
			}
			if (length <= 0f || math.all(direction == 0f))
			{
				this.Circle(position, radius, 0f, 6.2831855f);
				return;
			}
			float4x4 matrix;
			if (this.xy)
			{
				matrix = new float4x4(new float4(direction, 0f, 0f), new float4(math.cross(new float3(direction, 0f), CommandBuilder2D.XY_UP), 0f), new float4(0f, 0f, 1f, 0f), new float4(position, 0f, 1f));
			}
			else
			{
				matrix = new float4x4(new float4(direction.x, 0f, direction.y, 0f), new float4(0f, 1f, 0f, 0f), new float4(math.cross(new float3(direction.x, 0f, direction.y), CommandBuilder2D.XZ_UP), 0f), new float4(position.x, 0f, position.y, 1f));
			}
			this.draw.PushMatrix(matrix);
			this.Circle(new float2(0f, 0f), radius, 1.5707964f, 4.712389f);
			this.Line(new float2(0f, -radius), new float2(length, -radius));
			this.Circle(new float2(length, 0f), radius, -1.5707964f, 1.5707964f);
			this.Line(new float2(0f, radius), new float2(length, radius));
			this.draw.PopMatrix();
		}

		[BurstDiscard]
		public void Polyline(List<Vector2> points, bool cycle = false)
		{
			for (int i = 0; i < points.Count - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Count > 1)
			{
				this.Line(points[points.Count - 1], points[0]);
			}
		}

		[BurstDiscard]
		public void Polyline(Vector2[] points, bool cycle = false)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Length > 1)
			{
				this.Line(points[points.Length - 1], points[0]);
			}
		}

		[BurstDiscard]
		public void Polyline(float2[] points, bool cycle = false)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Length > 1)
			{
				this.Line(points[points.Length - 1], points[0]);
			}
		}

		public void Polyline(NativeArray<float2> points, bool cycle = false)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Length > 1)
			{
				this.Line(points[points.Length - 1], points[0]);
			}
		}

		public void Cross(float2 position, float size = 1f)
		{
			size *= 0.5f;
			this.Line(position - new float2(size, 0f), position + new float2(size, 0f));
			this.Line(position - new float2(0f, size), position + new float2(0f, size));
		}

		public void WireRectangle(float3 center, float2 size)
		{
			this.draw.WirePlane(center, this.xy ? CommandBuilder2D.XY_TO_XZ_ROTATION : CommandBuilder2D.XZ_TO_XZ_ROTATION, size);
		}

		public void WireRectangle(Rect rect)
		{
			float2 @float = rect.min;
			float2 float2 = rect.max;
			this.Line(new float2(@float.x, @float.y), new float2(float2.x, @float.y));
			this.Line(new float2(float2.x, @float.y), new float2(float2.x, float2.y));
			this.Line(new float2(float2.x, float2.y), new float2(@float.x, float2.y));
			this.Line(new float2(@float.x, float2.y), new float2(@float.x, @float.y));
		}

		public void SolidRectangle(Rect rect)
		{
			this.draw.SolidPlane(new float3(rect.center.x, rect.center.y, 0f), this.xy ? CommandBuilder2D.XY_TO_XZ_ROTATION : CommandBuilder2D.XZ_TO_XZ_ROTATION, new float2(rect.width, rect.height));
		}

		public void WireGrid(float2 center, int2 cells, float2 totalSize)
		{
			this.draw.WireGrid(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), this.xy ? CommandBuilder2D.XY_TO_XZ_ROTATION : CommandBuilder2D.XZ_TO_XZ_ROTATION, cells, totalSize);
		}

		public void WireGrid(float3 center, int2 cells, float2 totalSize)
		{
			this.draw.WireGrid(center, this.xy ? CommandBuilder2D.XY_TO_XZ_ROTATION : CommandBuilder2D.XZ_TO_XZ_ROTATION, cells, totalSize);
		}

		[BurstDiscard]
		public CommandBuilder.ScopeMatrix WithMatrix(Matrix4x4 matrix)
		{
			return this.draw.WithMatrix(matrix);
		}

		[BurstDiscard]
		public CommandBuilder.ScopeMatrix WithMatrix(float3x3 matrix)
		{
			return this.draw.WithMatrix(matrix);
		}

		[BurstDiscard]
		public CommandBuilder.ScopeColor WithColor(Color color)
		{
			return this.draw.WithColor(color);
		}

		[BurstDiscard]
		public CommandBuilder.ScopePersist WithDuration(float duration)
		{
			return this.draw.WithDuration(duration);
		}

		[BurstDiscard]
		public CommandBuilder.ScopeLineWidth WithLineWidth(float pixels, bool automaticJoins = true)
		{
			return this.draw.WithLineWidth(pixels, automaticJoins);
		}

		[BurstDiscard]
		public CommandBuilder.ScopeMatrix InLocalSpace(Transform transform)
		{
			return this.draw.InLocalSpace(transform);
		}

		[BurstDiscard]
		public CommandBuilder.ScopeMatrix InScreenSpace(Camera camera)
		{
			return this.draw.InScreenSpace(camera);
		}

		public void PushMatrix(Matrix4x4 matrix)
		{
			this.draw.PushMatrix(matrix);
		}

		public void PushMatrix(float4x4 matrix)
		{
			this.draw.PushMatrix(matrix);
		}

		public void PushSetMatrix(Matrix4x4 matrix)
		{
			this.draw.PushSetMatrix(matrix);
		}

		public void PushSetMatrix(float4x4 matrix)
		{
			this.draw.PushSetMatrix(matrix);
		}

		public void PopMatrix()
		{
			this.draw.PopMatrix();
		}

		public void PushColor(Color color)
		{
			this.draw.PushColor(color);
		}

		public void PopColor()
		{
			this.draw.PopColor();
		}

		public void PushDuration(float duration)
		{
			this.draw.PushDuration(duration);
		}

		public void PopDuration()
		{
			this.draw.PopDuration();
		}

		[Obsolete("Renamed to PushDuration for consistency")]
		public void PushPersist(float duration)
		{
			this.draw.PushPersist(duration);
		}

		[Obsolete("Renamed to PopDuration for consistency")]
		public void PopPersist()
		{
			this.draw.PopPersist();
		}

		public void PushLineWidth(float pixels, bool automaticJoins = true)
		{
			this.draw.PushLineWidth(pixels, automaticJoins);
		}

		public void PopLineWidth()
		{
			this.draw.PopLineWidth();
		}

		public void Line(Vector3 a, Vector3 b)
		{
			this.draw.Line(a, b);
		}

		public void Line(Vector2 a, Vector2 b)
		{
			this.Line(this.xy ? new Vector3(a.x, a.y, 0f) : new Vector3(a.x, 0f, a.y), this.xy ? new Vector3(b.x, b.y, 0f) : new Vector3(b.x, 0f, b.y));
		}

		public void Line(Vector3 a, Vector3 b, Color color)
		{
			this.draw.Line(a, b, color);
		}

		public void Line(Vector2 a, Vector2 b, Color color)
		{
			this.Line(this.xy ? new Vector3(a.x, a.y, 0f) : new Vector3(a.x, 0f, a.y), this.xy ? new Vector3(b.x, b.y, 0f) : new Vector3(b.x, 0f, b.y), color);
		}

		public void Ray(float3 origin, float3 direction)
		{
			this.draw.Ray(origin, direction);
		}

		public void Ray(float2 origin, float2 direction)
		{
			this.Ray(this.xy ? new float3(origin, 0f) : new float3(origin.x, 0f, origin.y), this.xy ? new float3(direction, 0f) : new float3(direction.x, 0f, direction.y));
		}

		public void Ray(Ray ray, float length)
		{
			this.draw.Ray(ray, length);
		}

		public void Arc(float3 center, float3 start, float3 end)
		{
			this.draw.Arc(center, start, end);
		}

		public void Arc(float2 center, float2 start, float2 end)
		{
			this.Arc(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), this.xy ? new float3(start, 0f) : new float3(start.x, 0f, start.y), this.xy ? new float3(end, 0f) : new float3(end.x, 0f, end.y));
		}

		[Obsolete("Use Draw.xy.Circle instead")]
		public void CircleXY(float3 center, float radius, float startAngle = 0f, float endAngle = 6.2831855f)
		{
			this.draw.CircleXY(center, radius, startAngle, endAngle);
		}

		[Obsolete("Use Draw.xy.Circle instead")]
		public void CircleXY(float2 center, float radius, float startAngle = 0f, float endAngle = 6.2831855f)
		{
			this.CircleXY(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), radius, startAngle, endAngle);
		}

		public void SolidArc(float3 center, float3 start, float3 end)
		{
			this.draw.SolidArc(center, start, end);
		}

		public void SolidArc(float2 center, float2 start, float2 end)
		{
			this.SolidArc(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), this.xy ? new float3(start, 0f) : new float3(start.x, 0f, start.y), this.xy ? new float3(end, 0f) : new float3(end.x, 0f, end.y));
		}

		[BurstDiscard]
		public void Polyline(List<Vector3> points, bool cycle = false)
		{
			this.draw.Polyline(points, cycle);
		}

		[BurstDiscard]
		public void Polyline(Vector3[] points, bool cycle = false)
		{
			this.draw.Polyline(points, cycle);
		}

		[BurstDiscard]
		public void Polyline(float3[] points, bool cycle = false)
		{
			this.draw.Polyline(points, cycle);
		}

		public void Polyline(NativeArray<float3> points, bool cycle = false)
		{
			this.draw.Polyline(points, cycle);
		}

		public void DashedLine(float3 a, float3 b, float dash, float gap)
		{
			this.draw.DashedLine(a, b, dash, gap);
		}

		public void DashedLine(float2 a, float2 b, float dash, float gap)
		{
			this.DashedLine(this.xy ? new float3(a, 0f) : new float3(a.x, 0f, a.y), this.xy ? new float3(b, 0f) : new float3(b.x, 0f, b.y), dash, gap);
		}

		public void DashedPolyline(List<Vector3> points, float dash, float gap)
		{
			this.draw.DashedPolyline(points, dash, gap);
		}

		public void Cross(float3 position, float size = 1f)
		{
			this.draw.Cross(position, size);
		}

		public void Bezier(float3 p0, float3 p1, float3 p2, float3 p3)
		{
			this.draw.Bezier(p0, p1, p2, p3);
		}

		public void Bezier(float2 p0, float2 p1, float2 p2, float2 p3)
		{
			this.Bezier(this.xy ? new float3(p0, 0f) : new float3(p0.x, 0f, p0.y), this.xy ? new float3(p1, 0f) : new float3(p1.x, 0f, p1.y), this.xy ? new float3(p2, 0f) : new float3(p2.x, 0f, p2.y), this.xy ? new float3(p3, 0f) : new float3(p3.x, 0f, p3.y));
		}

		public void CatmullRom(List<Vector3> points)
		{
			this.draw.CatmullRom(points);
		}

		public void CatmullRom(float3 p0, float3 p1, float3 p2, float3 p3)
		{
			this.draw.CatmullRom(p0, p1, p2, p3);
		}

		public void CatmullRom(float2 p0, float2 p1, float2 p2, float2 p3)
		{
			this.CatmullRom(this.xy ? new float3(p0, 0f) : new float3(p0.x, 0f, p0.y), this.xy ? new float3(p1, 0f) : new float3(p1.x, 0f, p1.y), this.xy ? new float3(p2, 0f) : new float3(p2.x, 0f, p2.y), this.xy ? new float3(p3, 0f) : new float3(p3.x, 0f, p3.y));
		}

		public void Arrow(float3 from, float3 to)
		{
			this.ArrowRelativeSizeHead(from, to, this.xy ? CommandBuilder2D.XY_UP : CommandBuilder2D.XZ_UP, 0.2f);
		}

		public void Arrow(float2 from, float2 to)
		{
			this.Arrow(this.xy ? new float3(from, 0f) : new float3(from.x, 0f, from.y), this.xy ? new float3(to, 0f) : new float3(to.x, 0f, to.y));
		}

		public void Arrow(float3 from, float3 to, float3 up, float headSize)
		{
			this.draw.Arrow(from, to, up, headSize);
		}

		public void Arrow(float2 from, float2 to, float2 up, float headSize)
		{
			this.Arrow(this.xy ? new float3(from, 0f) : new float3(from.x, 0f, from.y), this.xy ? new float3(to, 0f) : new float3(to.x, 0f, to.y), this.xy ? new float3(up, 0f) : new float3(up.x, 0f, up.y), headSize);
		}

		public void ArrowRelativeSizeHead(float3 from, float3 to, float3 up, float headFraction)
		{
			this.draw.ArrowRelativeSizeHead(from, to, up, headFraction);
		}

		public void ArrowRelativeSizeHead(float2 from, float2 to, float2 up, float headFraction)
		{
			this.ArrowRelativeSizeHead(this.xy ? new float3(from, 0f) : new float3(from.x, 0f, from.y), this.xy ? new float3(to, 0f) : new float3(to.x, 0f, to.y), this.xy ? new float3(up, 0f) : new float3(up.x, 0f, up.y), headFraction);
		}

		public void Arrowhead(float3 center, float3 direction, float radius)
		{
			this.Arrowhead(center, direction, this.xy ? CommandBuilder2D.XY_UP : CommandBuilder2D.XZ_UP, radius);
		}

		public void Arrowhead(float2 center, float2 direction, float radius)
		{
			this.Arrowhead(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), this.xy ? new float3(direction, 0f) : new float3(direction.x, 0f, direction.y), radius);
		}

		public void Arrowhead(float3 center, float3 direction, float3 up, float radius)
		{
			this.draw.Arrowhead(center, direction, up, radius);
		}

		public void Arrowhead(float2 center, float2 direction, float2 up, float radius)
		{
			this.Arrowhead(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), this.xy ? new float3(direction, 0f) : new float3(direction.x, 0f, direction.y), this.xy ? new float3(up, 0f) : new float3(up.x, 0f, up.y), radius);
		}

		public void ArrowheadArc(float3 origin, float3 direction, float offset, float width = 60f)
		{
			if (!math.any(direction))
			{
				return;
			}
			if (offset < 0f)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (offset == 0f)
			{
				return;
			}
			Quaternion q = Quaternion.LookRotation(direction, this.xy ? CommandBuilder2D.XY_UP : CommandBuilder2D.XZ_UP);
			this.PushMatrix(Matrix4x4.TRS(origin, q, Vector3.one));
			float num = 1.5707964f - width * 0.008726646f;
			float num2 = 1.5707964f + width * 0.008726646f;
			this.draw.CircleXZInternal(float3.zero, offset, num, num2);
			float3 a = new float3(math.cos(num), 0f, math.sin(num)) * offset;
			float3 b = new float3(math.cos(num2), 0f, math.sin(num2)) * offset;
			float3 @float = new float3(0f, 0f, 1.4142f * offset);
			this.Line(a, @float);
			this.Line(@float, b);
			this.PopMatrix();
		}

		public void ArrowheadArc(float2 origin, float2 direction, float offset, float width = 60f)
		{
			this.ArrowheadArc(this.xy ? new float3(origin, 0f) : new float3(origin.x, 0f, origin.y), this.xy ? new float3(direction, 0f) : new float3(direction.x, 0f, direction.y), offset, width);
		}

		public void WireTriangle(float3 a, float3 b, float3 c)
		{
			this.draw.WireTriangle(a, b, c);
		}

		public void WireTriangle(float2 a, float2 b, float2 c)
		{
			this.WireTriangle(this.xy ? new float3(a, 0f) : new float3(a.x, 0f, a.y), this.xy ? new float3(b, 0f) : new float3(b.x, 0f, b.y), this.xy ? new float3(c, 0f) : new float3(c.x, 0f, c.y));
		}

		public void WireRectangle(float3 center, quaternion rotation, float2 size)
		{
			this.draw.WireRectangle(center, rotation, size);
		}

		public void WireRectangle(float2 center, quaternion rotation, float2 size)
		{
			this.WireRectangle(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), rotation, size);
		}

		public void WireTriangle(float3 center, quaternion rotation, float radius)
		{
			this.draw.WireTriangle(center, rotation, radius);
		}

		public void WireTriangle(float2 center, quaternion rotation, float radius)
		{
			this.WireTriangle(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), rotation, radius);
		}

		public void SolidTriangle(float3 a, float3 b, float3 c)
		{
			this.draw.SolidTriangle(a, b, c);
		}

		public void SolidTriangle(float2 a, float2 b, float2 c)
		{
			this.SolidTriangle(this.xy ? new float3(a, 0f) : new float3(a.x, 0f, a.y), this.xy ? new float3(b, 0f) : new float3(b.x, 0f, b.y), this.xy ? new float3(c, 0f) : new float3(c.x, 0f, c.y));
		}

		public void Label2D(float3 position, string text, float sizeInPixels = 14f)
		{
			this.draw.Label2D(position, text, sizeInPixels);
		}

		public void Label2D(float2 position, string text, float sizeInPixels = 14f)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), text, sizeInPixels);
		}

		public void Label2D(float3 position, string text, float sizeInPixels, LabelAlignment alignment)
		{
			this.draw.Label2D(position, text, sizeInPixels, alignment);
		}

		public void Label2D(float2 position, string text, float sizeInPixels, LabelAlignment alignment)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), text, sizeInPixels, alignment);
		}

		public void Label2D(float3 position, ref FixedString32Bytes text, float sizeInPixels = 14f)
		{
			this.draw.Label2D(position, ref text, sizeInPixels);
		}

		public void Label2D(float2 position, ref FixedString32Bytes text, float sizeInPixels = 14f)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels);
		}

		public void Label2D(float3 position, ref FixedString64Bytes text, float sizeInPixels = 14f)
		{
			this.draw.Label2D(position, ref text, sizeInPixels);
		}

		public void Label2D(float2 position, ref FixedString64Bytes text, float sizeInPixels = 14f)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels);
		}

		public void Label2D(float3 position, ref FixedString128Bytes text, float sizeInPixels = 14f)
		{
			this.draw.Label2D(position, ref text, sizeInPixels);
		}

		public void Label2D(float2 position, ref FixedString128Bytes text, float sizeInPixels = 14f)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels);
		}

		public void Label2D(float3 position, ref FixedString512Bytes text, float sizeInPixels = 14f)
		{
			this.draw.Label2D(position, ref text, sizeInPixels);
		}

		public void Label2D(float2 position, ref FixedString512Bytes text, float sizeInPixels = 14f)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels);
		}

		public void Label2D(float3 position, ref FixedString32Bytes text, float sizeInPixels, LabelAlignment alignment)
		{
			this.draw.Label2D(position, ref text, sizeInPixels, alignment);
		}

		public void Label2D(float2 position, ref FixedString32Bytes text, float sizeInPixels, LabelAlignment alignment)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels, alignment);
		}

		public void Label2D(float3 position, ref FixedString64Bytes text, float sizeInPixels, LabelAlignment alignment)
		{
			this.draw.Label2D(position, ref text, sizeInPixels, alignment);
		}

		public void Label2D(float2 position, ref FixedString64Bytes text, float sizeInPixels, LabelAlignment alignment)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels, alignment);
		}

		public void Label2D(float3 position, ref FixedString128Bytes text, float sizeInPixels, LabelAlignment alignment)
		{
			this.draw.Label2D(position, ref text, sizeInPixels, alignment);
		}

		public void Label2D(float2 position, ref FixedString128Bytes text, float sizeInPixels, LabelAlignment alignment)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels, alignment);
		}

		public void Label2D(float3 position, ref FixedString512Bytes text, float sizeInPixels, LabelAlignment alignment)
		{
			this.draw.Label2D(position, ref text, sizeInPixels, alignment);
		}

		public void Label2D(float2 position, ref FixedString512Bytes text, float sizeInPixels, LabelAlignment alignment)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels, alignment);
		}

		public void Ray(float3 origin, float3 direction, Color color)
		{
			this.draw.Ray(origin, direction, color);
		}

		public void Ray(float2 origin, float2 direction, Color color)
		{
			this.Ray(this.xy ? new float3(origin, 0f) : new float3(origin.x, 0f, origin.y), this.xy ? new float3(direction, 0f) : new float3(direction.x, 0f, direction.y), color);
		}

		public void Ray(Ray ray, float length, Color color)
		{
			this.draw.Ray(ray, length, color);
		}

		public void Arc(float3 center, float3 start, float3 end, Color color)
		{
			this.draw.Arc(center, start, end, color);
		}

		public void Arc(float2 center, float2 start, float2 end, Color color)
		{
			this.Arc(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), this.xy ? new float3(start, 0f) : new float3(start.x, 0f, start.y), this.xy ? new float3(end, 0f) : new float3(end.x, 0f, end.y), color);
		}

		[Obsolete("Use Draw.xy.Circle instead")]
		public void CircleXY(float3 center, float radius, float startAngle, float endAngle, Color color)
		{
			this.draw.CircleXY(center, radius, startAngle, endAngle, color);
		}

		[Obsolete("Use Draw.xy.Circle instead")]
		public void CircleXY(float3 center, float radius, Color color)
		{
			this.CircleXY(center, radius, 0f, 6.2831855f, color);
		}

		[Obsolete("Use Draw.xy.Circle instead")]
		public void CircleXY(float2 center, float radius, float startAngle, float endAngle, Color color)
		{
			this.CircleXY(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), radius, startAngle, endAngle, color);
		}

		[Obsolete("Use Draw.xy.Circle instead")]
		public void CircleXY(float2 center, float radius, Color color)
		{
			this.CircleXY(center, radius, 0f, 6.2831855f, color);
		}

		public void SolidArc(float3 center, float3 start, float3 end, Color color)
		{
			this.draw.SolidArc(center, start, end, color);
		}

		public void SolidArc(float2 center, float2 start, float2 end, Color color)
		{
			this.SolidArc(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), this.xy ? new float3(start, 0f) : new float3(start.x, 0f, start.y), this.xy ? new float3(end, 0f) : new float3(end.x, 0f, end.y), color);
		}

		[BurstDiscard]
		public void Polyline(List<Vector3> points, bool cycle, Color color)
		{
			this.draw.Polyline(points, cycle, color);
		}

		[BurstDiscard]
		public void Polyline(List<Vector3> points, Color color)
		{
			this.Polyline(points, false, color);
		}

		[BurstDiscard]
		public void Polyline(Vector3[] points, bool cycle, Color color)
		{
			this.draw.Polyline(points, cycle, color);
		}

		[BurstDiscard]
		public void Polyline(Vector3[] points, Color color)
		{
			this.Polyline(points, false, color);
		}

		[BurstDiscard]
		public void Polyline(float3[] points, bool cycle, Color color)
		{
			this.draw.Polyline(points, cycle, color);
		}

		[BurstDiscard]
		public void Polyline(float3[] points, Color color)
		{
			this.Polyline(points, false, color);
		}

		public void Polyline(NativeArray<float3> points, bool cycle, Color color)
		{
			this.draw.Polyline(points, cycle, color);
		}

		public void Polyline(NativeArray<float3> points, Color color)
		{
			this.Polyline(points, false, color);
		}

		public void DashedLine(float3 a, float3 b, float dash, float gap, Color color)
		{
			this.draw.DashedLine(a, b, dash, gap, color);
		}

		public void DashedLine(float2 a, float2 b, float dash, float gap, Color color)
		{
			this.DashedLine(this.xy ? new float3(a, 0f) : new float3(a.x, 0f, a.y), this.xy ? new float3(b, 0f) : new float3(b.x, 0f, b.y), dash, gap, color);
		}

		public void DashedPolyline(List<Vector3> points, float dash, float gap, Color color)
		{
			this.draw.DashedPolyline(points, dash, gap, color);
		}

		public void Cross(float3 position, float size, Color color)
		{
			this.draw.Cross(position, size, color);
		}

		public void Cross(float3 position, Color color)
		{
			this.Cross(position, 1f, color);
		}

		public void Bezier(float3 p0, float3 p1, float3 p2, float3 p3, Color color)
		{
			this.draw.Bezier(p0, p1, p2, p3, color);
		}

		public void Bezier(float2 p0, float2 p1, float2 p2, float2 p3, Color color)
		{
			this.Bezier(this.xy ? new float3(p0, 0f) : new float3(p0.x, 0f, p0.y), this.xy ? new float3(p1, 0f) : new float3(p1.x, 0f, p1.y), this.xy ? new float3(p2, 0f) : new float3(p2.x, 0f, p2.y), this.xy ? new float3(p3, 0f) : new float3(p3.x, 0f, p3.y), color);
		}

		public void CatmullRom(List<Vector3> points, Color color)
		{
			this.draw.CatmullRom(points, color);
		}

		public void CatmullRom(float3 p0, float3 p1, float3 p2, float3 p3, Color color)
		{
			this.draw.CatmullRom(p0, p1, p2, p3, color);
		}

		public void CatmullRom(float2 p0, float2 p1, float2 p2, float2 p3, Color color)
		{
			this.CatmullRom(this.xy ? new float3(p0, 0f) : new float3(p0.x, 0f, p0.y), this.xy ? new float3(p1, 0f) : new float3(p1.x, 0f, p1.y), this.xy ? new float3(p2, 0f) : new float3(p2.x, 0f, p2.y), this.xy ? new float3(p3, 0f) : new float3(p3.x, 0f, p3.y), color);
		}

		public void Arrow(float3 from, float3 to, Color color)
		{
			this.ArrowRelativeSizeHead(from, to, this.xy ? CommandBuilder2D.XY_UP : CommandBuilder2D.XZ_UP, 0.2f, color);
		}

		public void Arrow(float2 from, float2 to, Color color)
		{
			this.Arrow(this.xy ? new float3(from, 0f) : new float3(from.x, 0f, from.y), this.xy ? new float3(to, 0f) : new float3(to.x, 0f, to.y), color);
		}

		public void Arrow(float3 from, float3 to, float3 up, float headSize, Color color)
		{
			this.draw.Arrow(from, to, up, headSize, color);
		}

		public void Arrow(float2 from, float2 to, float2 up, float headSize, Color color)
		{
			this.Arrow(this.xy ? new float3(from, 0f) : new float3(from.x, 0f, from.y), this.xy ? new float3(to, 0f) : new float3(to.x, 0f, to.y), this.xy ? new float3(up, 0f) : new float3(up.x, 0f, up.y), headSize, color);
		}

		public void ArrowRelativeSizeHead(float3 from, float3 to, float3 up, float headFraction, Color color)
		{
			this.draw.ArrowRelativeSizeHead(from, to, up, headFraction, color);
		}

		public void ArrowRelativeSizeHead(float2 from, float2 to, float2 up, float headFraction, Color color)
		{
			this.ArrowRelativeSizeHead(this.xy ? new float3(from, 0f) : new float3(from.x, 0f, from.y), this.xy ? new float3(to, 0f) : new float3(to.x, 0f, to.y), this.xy ? new float3(up, 0f) : new float3(up.x, 0f, up.y), headFraction, color);
		}

		public void Arrowhead(float3 center, float3 direction, float radius, Color color)
		{
			this.Arrowhead(center, direction, this.xy ? CommandBuilder2D.XY_UP : CommandBuilder2D.XZ_UP, radius, color);
		}

		public void Arrowhead(float2 center, float2 direction, float radius, Color color)
		{
			this.Arrowhead(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), this.xy ? new float3(direction, 0f) : new float3(direction.x, 0f, direction.y), radius, color);
		}

		public void Arrowhead(float3 center, float3 direction, float3 up, float radius, Color color)
		{
			this.draw.Arrowhead(center, direction, up, radius, color);
		}

		public void Arrowhead(float2 center, float2 direction, float2 up, float radius, Color color)
		{
			this.Arrowhead(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), this.xy ? new float3(direction, 0f) : new float3(direction.x, 0f, direction.y), this.xy ? new float3(up, 0f) : new float3(up.x, 0f, up.y), radius, color);
		}

		public void ArrowheadArc(float3 origin, float3 direction, float offset, float width, Color color)
		{
			if (!math.any(direction))
			{
				return;
			}
			if (offset < 0f)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (offset == 0f)
			{
				return;
			}
			this.draw.PushColor(color);
			Quaternion q = Quaternion.LookRotation(direction, this.xy ? CommandBuilder2D.XY_UP : CommandBuilder2D.XZ_UP);
			this.PushMatrix(Matrix4x4.TRS(origin, q, Vector3.one));
			float num = 1.5707964f - width * 0.008726646f;
			float num2 = 1.5707964f + width * 0.008726646f;
			this.draw.CircleXZInternal(float3.zero, offset, num, num2);
			float3 a = new float3(math.cos(num), 0f, math.sin(num)) * offset;
			float3 b = new float3(math.cos(num2), 0f, math.sin(num2)) * offset;
			float3 @float = new float3(0f, 0f, 1.4142f * offset);
			this.Line(a, @float);
			this.Line(@float, b);
			this.PopMatrix();
			this.draw.PopColor();
		}

		public void ArrowheadArc(float3 origin, float3 direction, float offset, Color color)
		{
			this.ArrowheadArc(origin, direction, offset, 60f, color);
		}

		public void ArrowheadArc(float2 origin, float2 direction, float offset, float width, Color color)
		{
			this.ArrowheadArc(this.xy ? new float3(origin, 0f) : new float3(origin.x, 0f, origin.y), this.xy ? new float3(direction, 0f) : new float3(direction.x, 0f, direction.y), offset, width, color);
		}

		public void ArrowheadArc(float2 origin, float2 direction, float offset, Color color)
		{
			this.ArrowheadArc(origin, direction, offset, 60f, color);
		}

		public void WireTriangle(float3 a, float3 b, float3 c, Color color)
		{
			this.draw.WireTriangle(a, b, c, color);
		}

		public void WireTriangle(float2 a, float2 b, float2 c, Color color)
		{
			this.WireTriangle(this.xy ? new float3(a, 0f) : new float3(a.x, 0f, a.y), this.xy ? new float3(b, 0f) : new float3(b.x, 0f, b.y), this.xy ? new float3(c, 0f) : new float3(c.x, 0f, c.y), color);
		}

		public void WireRectangle(float3 center, quaternion rotation, float2 size, Color color)
		{
			this.draw.WireRectangle(center, rotation, size, color);
		}

		public void WireRectangle(float2 center, quaternion rotation, float2 size, Color color)
		{
			this.WireRectangle(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), rotation, size, color);
		}

		public void WireTriangle(float3 center, quaternion rotation, float radius, Color color)
		{
			this.draw.WireTriangle(center, rotation, radius, color);
		}

		public void WireTriangle(float2 center, quaternion rotation, float radius, Color color)
		{
			this.WireTriangle(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), rotation, radius, color);
		}

		public void SolidTriangle(float3 a, float3 b, float3 c, Color color)
		{
			this.draw.SolidTriangle(a, b, c, color);
		}

		public void SolidTriangle(float2 a, float2 b, float2 c, Color color)
		{
			this.SolidTriangle(this.xy ? new float3(a, 0f) : new float3(a.x, 0f, a.y), this.xy ? new float3(b, 0f) : new float3(b.x, 0f, b.y), this.xy ? new float3(c, 0f) : new float3(c.x, 0f, c.y), color);
		}

		public void Label2D(float3 position, string text, float sizeInPixels, Color color)
		{
			this.draw.Label2D(position, text, sizeInPixels, color);
		}

		public void Label2D(float3 position, string text, Color color)
		{
			this.Label2D(position, text, 14f, color);
		}

		public void Label2D(float2 position, string text, float sizeInPixels, Color color)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), text, sizeInPixels, color);
		}

		public void Label2D(float2 position, string text, Color color)
		{
			this.Label2D(position, text, 14f, color);
		}

		public void Label2D(float3 position, string text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.draw.Label2D(position, text, sizeInPixels, alignment, color);
		}

		public void Label2D(float2 position, string text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), text, sizeInPixels, alignment, color);
		}

		public void Label2D(float3 position, ref FixedString32Bytes text, float sizeInPixels, Color color)
		{
			this.draw.Label2D(position, ref text, sizeInPixels, color);
		}

		public void Label2D(float3 position, ref FixedString32Bytes text, Color color)
		{
			this.Label2D(position, ref text, 14f, color);
		}

		public void Label2D(float2 position, ref FixedString32Bytes text, float sizeInPixels, Color color)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels, color);
		}

		public void Label2D(float2 position, ref FixedString32Bytes text, Color color)
		{
			this.Label2D(position, ref text, 14f, color);
		}

		public void Label2D(float3 position, ref FixedString64Bytes text, float sizeInPixels, Color color)
		{
			this.draw.Label2D(position, ref text, sizeInPixels, color);
		}

		public void Label2D(float3 position, ref FixedString64Bytes text, Color color)
		{
			this.Label2D(position, ref text, 14f, color);
		}

		public void Label2D(float2 position, ref FixedString64Bytes text, float sizeInPixels, Color color)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels, color);
		}

		public void Label2D(float2 position, ref FixedString64Bytes text, Color color)
		{
			this.Label2D(position, ref text, 14f, color);
		}

		public void Label2D(float3 position, ref FixedString128Bytes text, float sizeInPixels, Color color)
		{
			this.draw.Label2D(position, ref text, sizeInPixels, color);
		}

		public void Label2D(float3 position, ref FixedString128Bytes text, Color color)
		{
			this.Label2D(position, ref text, 14f, color);
		}

		public void Label2D(float2 position, ref FixedString128Bytes text, float sizeInPixels, Color color)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels, color);
		}

		public void Label2D(float2 position, ref FixedString128Bytes text, Color color)
		{
			this.Label2D(position, ref text, 14f, color);
		}

		public void Label2D(float3 position, ref FixedString512Bytes text, float sizeInPixels, Color color)
		{
			this.draw.Label2D(position, ref text, sizeInPixels, color);
		}

		public void Label2D(float3 position, ref FixedString512Bytes text, Color color)
		{
			this.Label2D(position, ref text, 14f, color);
		}

		public void Label2D(float2 position, ref FixedString512Bytes text, float sizeInPixels, Color color)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels, color);
		}

		public void Label2D(float2 position, ref FixedString512Bytes text, Color color)
		{
			this.Label2D(position, ref text, 14f, color);
		}

		public void Label2D(float3 position, ref FixedString32Bytes text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.draw.Label2D(position, ref text, sizeInPixels, alignment, color);
		}

		public void Label2D(float2 position, ref FixedString32Bytes text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels, alignment, color);
		}

		public void Label2D(float3 position, ref FixedString64Bytes text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.draw.Label2D(position, ref text, sizeInPixels, alignment, color);
		}

		public void Label2D(float2 position, ref FixedString64Bytes text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels, alignment, color);
		}

		public void Label2D(float3 position, ref FixedString128Bytes text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.draw.Label2D(position, ref text, sizeInPixels, alignment, color);
		}

		public void Label2D(float2 position, ref FixedString128Bytes text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels, alignment, color);
		}

		public void Label2D(float3 position, ref FixedString512Bytes text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.draw.Label2D(position, ref text, sizeInPixels, alignment, color);
		}

		public void Label2D(float2 position, ref FixedString512Bytes text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.Label2D(this.xy ? new float3(position, 0f) : new float3(position.x, 0f, position.y), ref text, sizeInPixels, alignment, color);
		}

		public void Line(float3 a, float3 b, Color color)
		{
			this.draw.Line(a, b, color);
		}

		public void Circle(float2 center, float radius, float startAngle, float endAngle, Color color)
		{
			this.Circle(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), radius, startAngle, endAngle, color);
		}

		public void Circle(float2 center, float radius, Color color)
		{
			this.Circle(center, radius, 0f, 6.2831855f, color);
		}

		public void Circle(float3 center, float radius, float startAngle, float endAngle, Color color)
		{
			this.draw.PushColor(color);
			if (this.xy)
			{
				this.draw.PushMatrix(CommandBuilder2D.XZ_TO_XY_MATRIX);
				this.draw.CircleXZInternal(new float3(center.x, center.z, center.y), radius, startAngle, endAngle);
				this.draw.PopMatrix();
			}
			else
			{
				this.draw.CircleXZInternal(center, radius, startAngle, endAngle);
			}
			this.draw.PopColor();
		}

		public void Circle(float3 center, float radius, Color color)
		{
			this.Circle(center, radius, 0f, 6.2831855f, color);
		}

		public void SolidCircle(float2 center, float radius, float startAngle, float endAngle, Color color)
		{
			this.SolidCircle(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), radius, startAngle, endAngle, color);
		}

		public void SolidCircle(float2 center, float radius, Color color)
		{
			this.SolidCircle(center, radius, 0f, 6.2831855f, color);
		}

		public void SolidCircle(float3 center, float radius, float startAngle, float endAngle, Color color)
		{
			this.draw.PushColor(color);
			if (this.xy)
			{
				this.draw.PushMatrix(CommandBuilder2D.XZ_TO_XY_MATRIX);
			}
			this.draw.SolidCircleXZInternal(new float3(center.x, -center.z, center.y), radius, startAngle, endAngle);
			if (this.xy)
			{
				this.draw.PopMatrix();
			}
			this.draw.PopColor();
		}

		public void SolidCircle(float3 center, float radius, Color color)
		{
			this.SolidCircle(center, radius, 0f, 6.2831855f, color);
		}

		public void WirePill(float2 a, float2 b, float radius, Color color)
		{
			this.WirePill(a, b - a, math.length(b - a), radius, color);
		}

		public void WirePill(float2 position, float2 direction, float length, float radius, Color color)
		{
			this.draw.PushColor(color);
			direction = math.normalizesafe(direction, default(float2));
			if (radius <= 0f)
			{
				this.Line(position, position + direction * length);
			}
			else if (length <= 0f || math.all(direction == 0f))
			{
				this.Circle(position, radius, 0f, 6.2831855f);
			}
			else
			{
				float4x4 matrix;
				if (this.xy)
				{
					matrix = new float4x4(new float4(direction, 0f, 0f), new float4(math.cross(new float3(direction, 0f), CommandBuilder2D.XY_UP), 0f), new float4(0f, 0f, 1f, 0f), new float4(position, 0f, 1f));
				}
				else
				{
					matrix = new float4x4(new float4(direction.x, 0f, direction.y, 0f), new float4(0f, 1f, 0f, 0f), new float4(math.cross(new float3(direction.x, 0f, direction.y), CommandBuilder2D.XZ_UP), 0f), new float4(position.x, 0f, position.y, 1f));
				}
				this.draw.PushMatrix(matrix);
				this.Circle(new float2(0f, 0f), radius, 1.5707964f, 4.712389f);
				this.Line(new float2(0f, -radius), new float2(length, -radius));
				this.Circle(new float2(length, 0f), radius, -1.5707964f, 1.5707964f);
				this.Line(new float2(0f, radius), new float2(length, radius));
				this.draw.PopMatrix();
			}
			this.draw.PopColor();
		}

		[BurstDiscard]
		public void Polyline(List<Vector2> points, bool cycle, Color color)
		{
			this.draw.PushColor(color);
			for (int i = 0; i < points.Count - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Count > 1)
			{
				this.Line(points[points.Count - 1], points[0]);
			}
			this.draw.PopColor();
		}

		[BurstDiscard]
		public void Polyline(List<Vector2> points, Color color)
		{
			this.Polyline(points, false, color);
		}

		[BurstDiscard]
		public void Polyline(Vector2[] points, bool cycle, Color color)
		{
			this.draw.PushColor(color);
			for (int i = 0; i < points.Length - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Length > 1)
			{
				this.Line(points[points.Length - 1], points[0]);
			}
			this.draw.PopColor();
		}

		[BurstDiscard]
		public void Polyline(Vector2[] points, Color color)
		{
			this.Polyline(points, false, color);
		}

		[BurstDiscard]
		public void Polyline(float2[] points, bool cycle, Color color)
		{
			this.draw.PushColor(color);
			for (int i = 0; i < points.Length - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Length > 1)
			{
				this.Line(points[points.Length - 1], points[0]);
			}
			this.draw.PopColor();
		}

		[BurstDiscard]
		public void Polyline(float2[] points, Color color)
		{
			this.Polyline(points, false, color);
		}

		public void Polyline(NativeArray<float2> points, bool cycle, Color color)
		{
			this.draw.PushColor(color);
			for (int i = 0; i < points.Length - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Length > 1)
			{
				this.Line(points[points.Length - 1], points[0]);
			}
			this.draw.PopColor();
		}

		public void Polyline(NativeArray<float2> points, Color color)
		{
			this.Polyline(points, false, color);
		}

		public void Cross(float2 position, float size, Color color)
		{
			this.draw.PushColor(color);
			size *= 0.5f;
			this.Line(position - new float2(size, 0f), position + new float2(size, 0f));
			this.Line(position - new float2(0f, size), position + new float2(0f, size));
			this.draw.PopColor();
		}

		public void Cross(float2 position, Color color)
		{
			this.Cross(position, 1f, color);
		}

		public void WireRectangle(float3 center, float2 size, Color color)
		{
			this.draw.WirePlane(center, this.xy ? CommandBuilder2D.XY_TO_XZ_ROTATION : CommandBuilder2D.XZ_TO_XZ_ROTATION, size, color);
		}

		public void WireRectangle(Rect rect, Color color)
		{
			this.draw.PushColor(color);
			float2 @float = rect.min;
			float2 float2 = rect.max;
			this.Line(new float2(@float.x, @float.y), new float2(float2.x, @float.y));
			this.Line(new float2(float2.x, @float.y), new float2(float2.x, float2.y));
			this.Line(new float2(float2.x, float2.y), new float2(@float.x, float2.y));
			this.Line(new float2(@float.x, float2.y), new float2(@float.x, @float.y));
			this.draw.PopColor();
		}

		public void SolidRectangle(Rect rect, Color color)
		{
			this.draw.SolidPlane(new float3(rect.center.x, rect.center.y, 0f), this.xy ? CommandBuilder2D.XY_TO_XZ_ROTATION : CommandBuilder2D.XZ_TO_XZ_ROTATION, new float2(rect.width, rect.height), color);
		}

		public void WireGrid(float2 center, int2 cells, float2 totalSize, Color color)
		{
			this.draw.WireGrid(this.xy ? new float3(center, 0f) : new float3(center.x, 0f, center.y), this.xy ? CommandBuilder2D.XY_TO_XZ_ROTATION : CommandBuilder2D.XZ_TO_XZ_ROTATION, cells, totalSize, color);
		}

		public void WireGrid(float3 center, int2 cells, float2 totalSize, Color color)
		{
			this.draw.WireGrid(center, this.xy ? CommandBuilder2D.XY_TO_XZ_ROTATION : CommandBuilder2D.XZ_TO_XZ_ROTATION, cells, totalSize, color);
		}

		private CommandBuilder draw;

		private bool xy;

		private static readonly float3 XY_UP = new float3(0f, 0f, 1f);

		private static readonly float3 XZ_UP = new float3(0f, 1f, 0f);

		private static readonly quaternion XY_TO_XZ_ROTATION = quaternion.RotateX(-1.5707964f);

		private static readonly quaternion XZ_TO_XZ_ROTATION = quaternion.identity;

		private static readonly float4x4 XZ_TO_XY_MATRIX = new float4x4(new float4(1f, 0f, 0f, 0f), new float4(0f, 0f, 1f, 0f), new float4(0f, 1f, 0f, 0f), new float4(0f, 0f, 0f, 1f));
	}
}
