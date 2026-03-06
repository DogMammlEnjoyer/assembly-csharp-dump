using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class TunnelingEffect : MonoBehaviour
	{
		public Vector3 AimingDirection
		{
			get
			{
				return this._aimingDirection;
			}
			set
			{
				this._aimingDirection = value;
			}
		}

		public bool UseAimingTarget
		{
			get
			{
				return this._useAimingTarget;
			}
			set
			{
				this._useAimingTarget = value;
			}
		}

		public float PlaneDistance
		{
			get
			{
				return this._planeDistance;
			}
			set
			{
				this._planeDistance = value;
			}
		}

		public Color MaskOuterColor
		{
			get
			{
				return this._maskOuterColor;
			}
			set
			{
				this._maskOuterColor = value;
			}
		}

		public Color MaskInnerColor
		{
			get
			{
				return this._maskInnerColor;
			}
			set
			{
				this._maskInnerColor = value;
			}
		}

		public float UserFOV
		{
			get
			{
				return this._userFOV;
			}
			set
			{
				this._userFOV = value;
			}
		}

		public float ExtraFeatheredFOV
		{
			get
			{
				return this._featheredFOV;
			}
			set
			{
				this._featheredFOV = value;
			}
		}

		public float AlphaStrength
		{
			get
			{
				return this._alphaStrength;
			}
			set
			{
				this._alphaStrength = value;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._meshTransform = this._meshFilter.gameObject.transform;
			this._meshRenderer = this._meshFilter.GetComponent<MeshRenderer>();
			this._maskMesh = new Mesh();
			this._maskMesh.SetVertices(TunnelingEffect._vertices);
			this._maskMesh.SetTriangles(TunnelingEffect._triangles, 0);
			this._maskMesh.SetUVs(0, TunnelingEffect._uv0);
			this._maskMesh.name = "Tunnel";
			this._meshFilter.sharedMesh = this._maskMesh;
			this._materialPropertyBlock = new MaterialPropertyBlock();
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._meshRenderer.enabled = true;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._meshRenderer.enabled = false;
			}
		}

		private void LateUpdate()
		{
			if (this._meshRenderer == null || this._meshTransform == null)
			{
				return;
			}
			Transform transform = base.transform;
			Pose pose = this._centerEyeCamera.transform.GetPose(Space.World);
			transform.SetPose(pose, Space.World);
			float num = Mathf.Tan(0.017453292f * this._centerEyeCamera.fieldOfView / 2f) * this._planeDistance * 2f;
			float num2 = num * this._centerEyeCamera.aspect;
			num2 += this.GetIPD();
			Vector2 vector = new Vector2(num2, num);
			vector *= 1.2f;
			this._meshTransform.localPosition = new Vector3(0f, 0f, this._planeDistance);
			this._meshTransform.localScale = new Vector3(vector.x * 0.5f, vector.y * 0.5f, 1f);
			float num3 = this.UserFOV * 0.5f * 0.017453292f;
			float value = Mathf.Cos(num3);
			float value2 = Mathf.Cos(num3 - this.ExtraFeatheredFOV * 0.017453292f);
			this._materialPropertyBlock.SetFloat(this._alphaID, this._alphaStrength);
			this._materialPropertyBlock.SetFloat(this._minRadiusID, value2);
			this._materialPropertyBlock.SetFloat(this._maxRadiusID, value);
			this._materialPropertyBlock.SetColor(this._maskColorInnerID, this._maskInnerColor);
			this._materialPropertyBlock.SetColor(this._maskColorOuterID, this._maskOuterColor);
			Vector3 vector2 = this._useAimingTarget ? this._aimingDirection : this._centerEyeCamera.transform.forward;
			this._materialPropertyBlock.SetVector(this._maskDirectionID, vector2.normalized);
			this._meshRenderer.SetPropertyBlock(this._materialPropertyBlock);
		}

		private float GetIPD()
		{
			return Vector3.Distance(this._leftEyeAnchor.position, this._rightEyeAnchor.position);
		}

		public void InjectAllTunnelingEffect(Transform leftEyeAnchor, Transform rightEyeAnchor, Camera centerEyeCamera, MeshFilter meshFilter)
		{
			this.InjectLeftEyeAnchor(leftEyeAnchor);
			this.InjectRightEyeAnchor(rightEyeAnchor);
			this.InjectCenterEyeCamera(centerEyeCamera);
			this.InjectMeshFilter(meshFilter);
		}

		public void InjectLeftEyeAnchor(Transform leftEyeAnchor)
		{
			this._leftEyeAnchor = leftEyeAnchor;
		}

		public void InjectRightEyeAnchor(Transform rightEyeAnchor)
		{
			this._rightEyeAnchor = rightEyeAnchor;
		}

		public void InjectCenterEyeCamera(Camera centerEyeCamera)
		{
			this._centerEyeCamera = centerEyeCamera;
		}

		public void InjectMeshFilter(MeshFilter meshFilter)
		{
			this._meshFilter = meshFilter;
		}

		[Header("Mask Setup")]
		[SerializeField]
		private Transform _leftEyeAnchor;

		[SerializeField]
		private Transform _rightEyeAnchor;

		[SerializeField]
		private Camera _centerEyeCamera;

		[SerializeField]
		private MeshFilter _meshFilter;

		[SerializeField]
		[Optional]
		private Vector3 _aimingDirection;

		[SerializeField]
		private bool _useAimingTarget;

		[Header("Mask State")]
		[SerializeField]
		private float _planeDistance;

		[Header("Mask Properties")]
		[SerializeField]
		private Color _maskOuterColor = Color.black;

		[SerializeField]
		private Color _maskInnerColor = Color.black;

		[SerializeField]
		[Range(0f, 360f)]
		private float _userFOV = 360f;

		[SerializeField]
		[Range(0f, 180f)]
		private float _featheredFOV = 10f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _alphaStrength = 1f;

		private readonly int _maskColorInnerID = Shader.PropertyToID("_ColorInner");

		private readonly int _maskColorOuterID = Shader.PropertyToID("_ColorOuter");

		private readonly int _maskDirectionID = Shader.PropertyToID("_Direction");

		private readonly int _minRadiusID = Shader.PropertyToID("_MinRadius");

		private readonly int _maxRadiusID = Shader.PropertyToID("_MaxRadius");

		private readonly int _alphaID = Shader.PropertyToID("_Alpha");

		private Mesh _maskMesh;

		private Transform _meshTransform;

		private MeshRenderer _meshRenderer;

		private MaterialPropertyBlock _materialPropertyBlock;

		protected bool _started;

		private static readonly Vector3[] _vertices = new Vector3[]
		{
			new Vector3(-1f, 1f, 0f),
			new Vector3(1f, 1f, 0f),
			new Vector3(-1f, -1f, 0f),
			new Vector3(1f, -1f, 0f)
		};

		private static readonly Vector3[] _uv0 = new Vector3[]
		{
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(0f, 0f),
			new Vector2(1f, 0f)
		};

		private static readonly int[] _triangles = new int[]
		{
			0,
			1,
			3,
			0,
			3,
			2
		};
	}
}
