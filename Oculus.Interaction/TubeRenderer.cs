using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Oculus.Interaction
{
	public class TubeRenderer : MonoBehaviour
	{
		public int RenderQueue
		{
			get
			{
				return this._renderQueue;
			}
			set
			{
				this._renderQueue = value;
			}
		}

		public Vector2 RenderOffset
		{
			get
			{
				return this._renderOffset;
			}
			set
			{
				this._renderOffset = value;
			}
		}

		public float Radius
		{
			get
			{
				return this._radius;
			}
			set
			{
				this._radius = value;
			}
		}

		public Gradient Gradient
		{
			get
			{
				return this._gradient;
			}
			set
			{
				this._gradient = value;
			}
		}

		public Color Tint
		{
			get
			{
				return this._tint;
			}
			set
			{
				this._tint = value;
			}
		}

		public float ProgressFade
		{
			get
			{
				return this._progressFade;
			}
			set
			{
				this._progressFade = value;
			}
		}

		public float StartFadeThresold
		{
			get
			{
				return this._startFadeThresold;
			}
			set
			{
				this._startFadeThresold = value;
			}
		}

		public float EndFadeThresold
		{
			get
			{
				return this._endFadeThresold;
			}
			set
			{
				this._endFadeThresold = value;
			}
		}

		public bool InvertThreshold
		{
			get
			{
				return this._invertThreshold;
			}
			set
			{
				this._invertThreshold = value;
			}
		}

		public float Feather
		{
			get
			{
				return this._feather;
			}
			set
			{
				this._feather = value;
			}
		}

		public bool MirrorTexture
		{
			get
			{
				return this._mirrorTexture;
			}
			set
			{
				this._mirrorTexture = value;
			}
		}

		public float Progress { get; set; }

		public float TotalLength
		{
			get
			{
				return this._totalLength;
			}
		}

		protected virtual void Reset()
		{
			this._filter = base.GetComponent<MeshFilter>();
			this._renderer = base.GetComponent<MeshRenderer>();
		}

		protected virtual void Awake()
		{
			this._hidden = base.enabled;
		}

		protected virtual void OnEnable()
		{
			this._renderer.enabled = !this._hidden;
		}

		protected virtual void OnDisable()
		{
			this._renderer.enabled = false;
		}

		public void RenderTube(TubePoint[] points, Space space = Space.Self)
		{
			int num = points.Length;
			if (num != this._initializedSteps)
			{
				this.InitializeMeshData(num);
				this._initializedSteps = num;
			}
			this._vertsData = new NativeArray<TubeRenderer.VertexLayout>(this._vertsCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
			this.UpdateMeshData(points, space);
			this._renderer.enabled = base.enabled;
			this._hidden = false;
		}

		public void Hide()
		{
			this._renderer.enabled = false;
			this._hidden = true;
		}

		public void Show()
		{
			this._renderer.enabled = true;
			this._hidden = false;
		}

		private void InitializeMeshData(int steps)
		{
			this._dataLayout = new VertexAttributeDescriptor[]
			{
				new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
				new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, 0),
				new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 0)
			};
			this._vertsCount = this.SetVertexCount(steps, this._divisions, this._bevel);
			SubMeshDescriptor desc = new SubMeshDescriptor(0, this._tris.Length, MeshTopology.Triangles);
			this._mesh = new Mesh();
			this._mesh.SetVertexBufferParams(this._vertsCount, this._dataLayout);
			this._mesh.SetIndexBufferParams(this._tris.Length, IndexFormat.UInt32);
			this._mesh.SetIndexBufferData<int>(this._tris, 0, 0, this._tris.Length, MeshUpdateFlags.Default);
			this._mesh.subMeshCount = 1;
			this._mesh.SetSubMesh(0, desc, MeshUpdateFlags.Default);
			this._filter.mesh = this._mesh;
		}

		private void UpdateMeshData(TubePoint[] points, Space space)
		{
			TubeRenderer.<>c__DisplayClass76_0 CS$<>8__locals1;
			CS$<>8__locals1.space = space;
			int num = points.Length;
			float num2 = 0f;
			Vector3 b = Vector3.zero;
			Pose identity = Pose.identity;
			Pose identity2 = Pose.identity;
			Pose identity3 = Pose.identity;
			Pose pose = base.transform.GetPose(Space.World);
			CS$<>8__locals1.inverseRootRotation = Quaternion.Inverse(pose.rotation);
			CS$<>8__locals1.rootPositionScaled = new Vector3(pose.position.x / base.transform.lossyScale.x, pose.position.y / base.transform.lossyScale.y, pose.position.z / base.transform.lossyScale.z);
			float num3 = (CS$<>8__locals1.space == Space.World) ? base.transform.lossyScale.x : 1f;
			TubeRenderer.<UpdateMeshData>g__TransformPose|76_0(points[0], ref identity2, ref CS$<>8__locals1);
			TubeRenderer.<UpdateMeshData>g__TransformPose|76_0(points[points.Length - 1], ref identity3, ref CS$<>8__locals1);
			this.BevelCap(identity2, false, 0);
			for (int i = 0; i < num; i++)
			{
				TubeRenderer.<UpdateMeshData>g__TransformPose|76_0(points[i], ref identity, ref CS$<>8__locals1);
				Vector3 position = identity.position;
				Quaternion rotation = identity.rotation;
				float relativeLength = points[i].relativeLength;
				Color c = this.Gradient.Evaluate(relativeLength) * this._tint;
				if (i > 0)
				{
					num2 += Vector3.Distance(position, b);
				}
				b = position;
				if ((float)i / ((float)num - 1f) < this.Progress)
				{
					c.a *= this.ProgressFade;
				}
				this._layout.color = c;
				this.WriteCircle(position, rotation, this._radius, i + this._bevel, relativeLength);
			}
			this.BevelCap(identity3, true, this._bevel + num);
			this._mesh.bounds = new Bounds((identity2.position + identity3.position) * 0.5f, identity3.position - identity2.position);
			this._mesh.SetVertexBufferData<TubeRenderer.VertexLayout>(this._vertsData, 0, 0, this._vertsData.Length, 0, MeshUpdateFlags.DontRecalculateBounds);
			this._totalLength = num2 * num3;
			this.RedrawFadeThresholds();
		}

		public void RedrawFadeThresholds()
		{
			float num = this.StartFadeThresold / this._totalLength;
			float num2 = (this.StartFadeThresold + this.Feather) / this._totalLength;
			float w = (this._totalLength - this.EndFadeThresold) / this._totalLength;
			float z = (this._totalLength - this.EndFadeThresold - this.Feather) / this._totalLength;
			this._renderer.material.SetVector(TubeRenderer._fadeLimitsShaderID, new Vector4(this._invertThreshold ? num2 : num, this._invertThreshold ? num : num2, z, w));
			this._renderer.material.SetFloat(TubeRenderer._fadeSignShaderID, (float)(this._invertThreshold ? -1 : 1));
			this._renderer.material.renderQueue = this._renderQueue;
			this._renderer.material.SetFloat(TubeRenderer._offsetFactorShaderPropertyID, this._renderOffset.x);
			this._renderer.material.SetFloat(TubeRenderer._offsetUnitsShaderPropertyID, this._renderOffset.y);
		}

		private void BevelCap(in Pose pose, bool end, int indexOffset)
		{
			Vector3 position = pose.position;
			Quaternion rotation = pose.rotation;
			for (int i = 0; i < this._bevel; i++)
			{
				float num = Mathf.InverseLerp(-1f, (float)(this._bevel + 1), (float)i);
				if (end)
				{
					num = 1f - num;
				}
				float d = Mathf.Sqrt(1f - num * num);
				Vector3 point = position + (float)(end ? 1 : -1) * (rotation * Vector3.forward) * this._radius * d;
				this.WriteCircle(point, rotation, this._radius * num, i + indexOffset, (float)(end ? 1 : 0));
			}
		}

		private void WriteCircle(Vector3 point, Quaternion rotation, float width, int index, float progress)
		{
			Color c = this.Gradient.Evaluate(progress) * this._tint;
			if (progress < this.Progress)
			{
				c.a *= this.ProgressFade;
			}
			this._layout.color = c;
			for (int i = 0; i <= this._divisions; i++)
			{
				float f = 6.2831855f * (float)i / (float)this._divisions;
				Vector3 point2 = new Vector3(Mathf.Sin(f), Mathf.Cos(f), 0f);
				Vector3 a = rotation * point2;
				this._layout.pos = point + a * width;
				if (this._mirrorTexture)
				{
					float num = (float)i / (float)this._divisions * 2f;
					if ((float)i >= (float)this._divisions * 0.5f)
					{
						num = 2f - num;
					}
					this._layout.uv = new Vector2(num, progress);
				}
				else
				{
					this._layout.uv = new Vector2((float)i / (float)this._divisions, progress);
				}
				int index2 = index * (this._divisions + 1) + i;
				this._vertsData[index2] = this._layout;
			}
		}

		private int SetVertexCount(int positionCount, int divisions, int bevelCap)
		{
			bevelCap *= 2;
			int num = divisions + 1;
			int num2 = (positionCount + bevelCap) * num;
			int num3 = (positionCount - 1 + bevelCap) * divisions * 6;
			int num4 = (divisions - 2) * 3;
			int num5 = num3 + num4 * 2;
			this._tris = new int[num5];
			for (int i = 0; i < positionCount - 1 + bevelCap; i++)
			{
				for (int j = 0; j < divisions; j++)
				{
					int num6 = i * num + j;
					int num7 = (i + 1) * num + j;
					int num8 = (i * divisions + j) * 6;
					this._tris[num8] = num6;
					this._tris[num8 + 1] = (this._tris[num8 + 4] = num7);
					this._tris[num8 + 2] = (this._tris[num8 + 3] = num6 + 1);
					this._tris[num8 + 5] = num7 + 1;
				}
			}
			this.<SetVertexCount>g__Cap|80_0(num3, 0, divisions - 1, true);
			this.<SetVertexCount>g__Cap|80_0(num3 + num4, num2 - divisions, num2 - 1, false);
			return num2;
		}

		public void InjectAllTubeRenderer(MeshFilter filter, MeshRenderer renderer, int divisions, int bevel)
		{
			this.InjectFilter(filter);
			this.InjectRenderer(renderer);
			this.InjectDivisions(divisions);
			this.InjectBevel(bevel);
		}

		public void InjectFilter(MeshFilter filter)
		{
			this._filter = filter;
		}

		public void InjectRenderer(MeshRenderer renderer)
		{
			this._renderer = renderer;
		}

		public void InjectDivisions(int divisions)
		{
			this._divisions = divisions;
		}

		public void InjectBevel(int bevel)
		{
			this._bevel = bevel;
		}

		[CompilerGenerated]
		internal static void <UpdateMeshData>g__TransformPose|76_0(in TubePoint tubePoint, ref Pose pose, ref TubeRenderer.<>c__DisplayClass76_0 A_2)
		{
			if (A_2.space == Space.Self)
			{
				pose.position = tubePoint.position;
				pose.rotation = tubePoint.rotation;
				return;
			}
			pose.position = A_2.inverseRootRotation * (tubePoint.position - A_2.rootPositionScaled);
			pose.rotation = A_2.inverseRootRotation * tubePoint.rotation;
		}

		[CompilerGenerated]
		private void <SetVertexCount>g__Cap|80_0(int t, int firstVert, int lastVert, bool clockwise = false)
		{
			for (int i = firstVert + 1; i < lastVert; i++)
			{
				this._tris[t++] = firstVert;
				this._tris[t++] = (clockwise ? i : (i + 1));
				this._tris[t++] = (clockwise ? (i + 1) : i);
			}
		}

		[Tooltip("The Mesh Filter that's included in the ReticleLine prefab.")]
		[SerializeField]
		private MeshFilter _filter;

		[Tooltip("The Mesh Renderer that's included in the ReticleLine prefab.")]
		[SerializeField]
		private MeshRenderer _renderer;

		[Tooltip("The number of divisions to use when calculating the tube mesh's vertices.")]
		[SerializeField]
		private int _divisions = 6;

		[Tooltip("The number of bevels to use when calculating the tube mesh's vertices.")]
		[SerializeField]
		private int _bevel = 4;

		[Tooltip("Unity shader queue that determines when the tube is rendered. Defaults to -1, which uses the render queue of the shader.")]
		[SerializeField]
		private int _renderQueue = -1;

		[SerializeField]
		private Vector2 _renderOffset = Vector2.zero;

		[Tooltip("The thickness of the tube.")]
		[SerializeField]
		private float _radius = 0.005f;

		[Tooltip("The gradient of the tube.")]
		[SerializeField]
		private Gradient _gradient;

		[Tooltip("The color of the tube.")]
		[SerializeField]
		private Color _tint = Color.white;

		[SerializeField]
		[Range(0f, 1f)]
		private float _progressFade = 0.2f;

		[Tooltip("Defines the length of the transparent portion at the beginning of the tube. The higher the value, the longer the transparent portion.")]
		[SerializeField]
		private float _startFadeThresold = 0.2f;

		[Tooltip("Defines the length of the transparent portion at the end of the tube. The higher the value, the longer the transparent portion.")]
		[SerializeField]
		private float _endFadeThresold = 0.2f;

		[Tooltip("Should the transparent portion of the tube be in the middle instead of at the beginning and end?")]
		[SerializeField]
		private bool _invertThreshold;

		[SerializeField]
		private float _feather = 0.2f;

		[SerializeField]
		private bool _mirrorTexture;

		private VertexAttributeDescriptor[] _dataLayout;

		private NativeArray<TubeRenderer.VertexLayout> _vertsData;

		private TubeRenderer.VertexLayout _layout;

		private Mesh _mesh;

		private int[] _tris;

		private int _initializedSteps = -1;

		private int _vertsCount;

		private float _totalLength;

		private bool _hidden;

		private static readonly int _fadeLimitsShaderID = Shader.PropertyToID("_FadeLimit");

		private static readonly int _fadeSignShaderID = Shader.PropertyToID("_FadeSign");

		private static readonly int _offsetFactorShaderPropertyID = Shader.PropertyToID("_OffsetFactor");

		private static readonly int _offsetUnitsShaderPropertyID = Shader.PropertyToID("_OffsetUnits");

		private struct VertexLayout
		{
			public Vector3 pos;

			public Color32 color;

			public Vector2 uv;
		}
	}
}
