using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Meta.XR.ImmersiveDebugger.Gizmo
{
	internal class PolylineRenderer
	{
		private int Copies
		{
			get
			{
				if (!this._renderSinglePass)
				{
					return 1;
				}
				return 2;
			}
		}

		private int BufferSize
		{
			get
			{
				return this._maxLineCount * 2 * this.Copies;
			}
		}

		public float LineScaleFactor
		{
			get
			{
				return this._lineScaleFactor;
			}
			set
			{
				this._lineScaleFactor = value;
			}
		}

		public PolylineRenderer(Material material = null, bool renderSinglePass = true)
		{
			this._renderSinglePass = renderSinglePass;
			if (material == null)
			{
				material = new Material(Shader.Find("Custom/PolylineUnlit"));
			}
			this._material = new Material(material);
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			this._baseMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			Object.DestroyImmediate(gameObject);
			this._positions = new Vector4[this.BufferSize];
			this._colors = new Color[this.BufferSize];
			this._positionBuffer = new ComputeBuffer(this.BufferSize, 16);
			this._positionBuffer.SetData(this._positions);
			this._colorBuffer = new ComputeBuffer(this.BufferSize, 16);
			this._colorBuffer.SetData(this._colors);
			this._material.SetBuffer(this._positionBufferShaderID, this._positionBuffer);
			this._material.SetBuffer(this._colorBufferShaderID, this._colorBuffer);
			this._argsData = new uint[5];
			this._argsData[0] = this._baseMesh.GetIndexCount(0);
			this._argsData[1] = (uint)(this._maxLineCount * this.Copies);
			this._argsBuffer = new ComputeBuffer(1, this._argsData.Length * 4, ComputeBufferType.DrawIndirect);
			this._argsBuffer.SetData(this._argsData);
			this._positionsNeedUpdate = true;
			this._colorsNeedUpdate = true;
		}

		public void Cleanup()
		{
			this._positionBuffer.Release();
			this._colorBuffer.Release();
			this._argsBuffer.Release();
			if (Application.isPlaying)
			{
				Object.Destroy(this._material);
				return;
			}
			Object.DestroyImmediate(this._material);
		}

		public void SetLines(List<Vector4> positions, Color color)
		{
			this.SetPositions(positions.Count, positions);
			this.SetDrawCount(positions.Count / 2);
			this.SetColor(positions.Count, color);
		}

		public void SetLines(List<Vector4> positions, List<Color> colors, int maxCount = -1)
		{
			int num = (maxCount < 0) ? positions.Count : maxCount;
			this.SetPositions(num, positions);
			this.SetDrawCount(num / 2);
			this.SetColors(num, colors);
		}

		private void SetPositions(int count, List<Vector4> positions)
		{
			if (count * this.Copies > this._positions.Length)
			{
				this._maxLineCount = count / 2;
				this._positions = new Vector4[this.BufferSize];
				this._positionBuffer.Release();
				this._positionBuffer = new ComputeBuffer(this.BufferSize, 16);
				this._positionBuffer.SetData(this._positions);
			}
			this._bounds = default(Bounds);
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			for (int i = 0; i < count; i += 2)
			{
				for (int j = 0; j < 2; j++)
				{
					Vector4 vector3 = positions[i + j];
					for (int k = 0; k < this.Copies; k++)
					{
						this._positions[(i + k) * this.Copies + j] = vector3;
					}
					Vector3 a = vector3.w * Vector3.one;
					Vector3 a2 = vector3;
					Vector3 vector4 = a2 - a / 2f;
					Vector3 vector5 = a2 + a / 2f;
					if (i == 0)
					{
						vector = vector4;
						vector2 = vector5;
					}
					else
					{
						vector.x = Mathf.Min(vector4.x, vector.x);
						vector.y = Mathf.Min(vector4.y, vector.y);
						vector.z = Mathf.Min(vector4.z, vector.z);
						vector2.x = Mathf.Max(vector5.x, vector2.x);
						vector2.y = Mathf.Max(vector5.y, vector2.y);
						vector2.z = Mathf.Max(vector5.z, vector2.z);
					}
				}
			}
			this._bounds.SetMinMax(vector, vector2);
			this._positionsNeedUpdate = true;
		}

		private void SetColors(int count, List<Color> colors)
		{
			this.PrepareColorBuffer(count);
			for (int i = 0; i < count; i += 2)
			{
				for (int j = 0; j < 2; j++)
				{
					for (int k = 0; k < this.Copies; k++)
					{
						this._colors[(i + k) * this.Copies + j] = colors[i + j];
					}
				}
			}
			this._colorsNeedUpdate = true;
		}

		private void SetColor(int count, Color color)
		{
			this.PrepareColorBuffer(count);
			for (int i = 0; i < count; i += 2)
			{
				for (int j = 0; j < 2; j++)
				{
					for (int k = 0; k < this.Copies; k++)
					{
						this._colors[(i + k) * this.Copies + j] = color;
					}
				}
			}
			this._colorsNeedUpdate = true;
		}

		private void SetDrawCount(int c)
		{
			this._argsData[1] = (uint)(c * this.Copies);
			this._argsBuffer.SetData(this._argsData);
		}

		private void PrepareColorBuffer(int count)
		{
			if (count * this.Copies <= this._colors.Length)
			{
				return;
			}
			this._maxLineCount = count / 2;
			this._colors = new Color[this.BufferSize];
			this._colorBuffer.Release();
			this._colorBuffer = new ComputeBuffer(this.BufferSize, 16);
			this._colorBuffer.SetData(this._colors);
		}

		public void RenderLines()
		{
			if (this._positionsNeedUpdate)
			{
				this._positionBuffer.SetData(this._positions);
				this._material.SetBuffer(this._positionBufferShaderID, this._positionBuffer);
				this._positionsNeedUpdate = false;
			}
			if (this._colorsNeedUpdate)
			{
				this._colorBuffer.SetData(this._colors);
				this._material.SetBuffer(this._colorBufferShaderID, this._colorBuffer);
				this._colorsNeedUpdate = false;
			}
			this._material.SetFloat(this._scaleShaderID, this._lineScaleFactor);
			this._material.SetMatrix(this._localToWorldShaderID, this._matrix);
			Bounds bounds = new Bounds(this._matrix.MultiplyPoint(this._bounds.center), this._matrix.MultiplyVector(this._bounds.size));
			Graphics.DrawMeshInstancedIndirect(this._baseMesh, 0, this._material, bounds, this._argsBuffer, 0, null, ShadowCastingMode.On, true, 0, null, LightProbeUsage.BlendProbes);
		}

		public void SetTransform(Transform transform)
		{
			this._matrix = transform.localToWorldMatrix;
		}

		private Vector4[] _positions;

		private bool _positionsNeedUpdate;

		private Color[] _colors;

		private bool _colorsNeedUpdate;

		private Bounds _bounds;

		private Mesh _baseMesh;

		private Material _material;

		private bool _renderSinglePass;

		private ComputeBuffer _positionBuffer;

		private ComputeBuffer _colorBuffer;

		private ComputeBuffer _argsBuffer;

		private uint[] _argsData;

		private int _positionBufferShaderID = Shader.PropertyToID("_PositionBuffer");

		private int _colorBufferShaderID = Shader.PropertyToID("_ColorBuffer");

		private int _localToWorldShaderID = Shader.PropertyToID("_LocalToWorld");

		private int _scaleShaderID = Shader.PropertyToID("_Scale");

		private int _maxLineCount = 1;

		private Matrix4x4 _matrix = Matrix4x4.identity;

		private float _lineScaleFactor = 1f;
	}
}
