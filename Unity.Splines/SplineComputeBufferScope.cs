using System;
using Unity.Collections;

namespace UnityEngine.Splines
{
	public struct SplineComputeBufferScope<T> : IDisposable where T : ISpline
	{
		public SplineComputeBufferScope(T spline)
		{
			this.m_Spline = spline;
			this.m_KnotCount = 0;
			this.m_CurveBuffer = (this.m_LengthBuffer = null);
			this.m_Shader = null;
			this.m_Info = (this.m_Curves = (this.m_CurveLengths = null));
			this.m_Kernel = 0;
			this.Upload();
		}

		public void Bind(ComputeShader shader, int kernel, string info, string curves, string lengths)
		{
			if (shader == null)
			{
				throw new ArgumentNullException("shader");
			}
			if (string.IsNullOrEmpty(info))
			{
				throw new ArgumentNullException("info");
			}
			if (string.IsNullOrEmpty(curves))
			{
				throw new ArgumentNullException("curves");
			}
			if (string.IsNullOrEmpty(lengths))
			{
				throw new ArgumentNullException("lengths");
			}
			this.m_Shader = shader;
			this.m_Info = info;
			this.m_Curves = curves;
			this.m_CurveLengths = lengths;
			this.m_Kernel = kernel;
			this.m_Shader.SetVector(this.m_Info, this.Info);
			this.m_Shader.SetBuffer(this.m_Kernel, this.m_Curves, this.Curves);
			this.m_Shader.SetBuffer(this.m_Kernel, this.m_CurveLengths, this.CurveLengths);
		}

		public void Dispose()
		{
			ComputeBuffer curveBuffer = this.m_CurveBuffer;
			if (curveBuffer != null)
			{
				curveBuffer.Dispose();
			}
			ComputeBuffer lengthBuffer = this.m_LengthBuffer;
			if (lengthBuffer == null)
			{
				return;
			}
			lengthBuffer.Dispose();
		}

		public void Upload()
		{
			int count = this.m_Spline.Count;
			if (this.m_KnotCount != count)
			{
				this.m_KnotCount = this.m_Spline.Count;
				ComputeBuffer curveBuffer = this.m_CurveBuffer;
				if (curveBuffer != null)
				{
					curveBuffer.Dispose();
				}
				ComputeBuffer lengthBuffer = this.m_LengthBuffer;
				if (lengthBuffer != null)
				{
					lengthBuffer.Dispose();
				}
				this.m_CurveBuffer = new ComputeBuffer(this.m_KnotCount, 48);
				this.m_LengthBuffer = new ComputeBuffer(this.m_KnotCount, 4);
			}
			NativeArray<BezierCurve> data = new NativeArray<BezierCurve>(this.m_KnotCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<float> data2 = new NativeArray<float>(this.m_KnotCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < this.m_KnotCount; i++)
			{
				data[i] = this.m_Spline.GetCurve(i);
				data2[i] = this.m_Spline.GetCurveLength(i);
			}
			if (!string.IsNullOrEmpty(this.m_Info))
			{
				this.m_Shader.SetVector(this.m_Info, this.Info);
			}
			this.m_CurveBuffer.SetData<BezierCurve>(data);
			this.m_LengthBuffer.SetData<float>(data2);
			data.Dispose();
			data2.Dispose();
		}

		public Vector4 Info
		{
			get
			{
				return new Vector4((float)this.m_Spline.Count, (float)(this.m_Spline.Closed ? 1 : 0), this.m_Spline.GetLength(), 0f);
			}
		}

		public ComputeBuffer Curves
		{
			get
			{
				return this.m_CurveBuffer;
			}
		}

		public ComputeBuffer CurveLengths
		{
			get
			{
				return this.m_LengthBuffer;
			}
		}

		private T m_Spline;

		private int m_KnotCount;

		private ComputeBuffer m_CurveBuffer;

		private ComputeBuffer m_LengthBuffer;

		private ComputeShader m_Shader;

		private string m_Info;

		private string m_Curves;

		private string m_CurveLengths;

		private int m_Kernel;
	}
}
