using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class TurnArrowVisuals : MonoBehaviour
	{
		public float Radius
		{
			get
			{
				return this._radius;
			}
		}

		public float Margin
		{
			get
			{
				return this._margin;
			}
		}

		public float TrailLength
		{
			get
			{
				return this._trailLength;
			}
		}

		public float MaxAngle
		{
			get
			{
				return this._maxAngle;
			}
		}

		public float RailGap
		{
			get
			{
				return this._railGap;
			}
		}

		public float SqueezeLength
		{
			get
			{
				return this._squeezeLength;
			}
		}

		public Color DisabledColor
		{
			get
			{
				return this._disabledColor;
			}
			set
			{
				this._disabledColor = value;
			}
		}

		public Color EnabledColor
		{
			get
			{
				return this._enabledColor;
			}
			set
			{
				this._enabledColor = value;
			}
		}

		public Color HighligtedColor
		{
			get
			{
				return this._highligtedColor;
			}
			set
			{
				this._highligtedColor = value;
			}
		}

		public bool HighLight
		{
			get
			{
				return this._highLight;
			}
			set
			{
				this._highLight = value;
			}
		}

		public float Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		public float Progress
		{
			get
			{
				return this._progress;
			}
			set
			{
				this._progress = value;
			}
		}

		public bool FollowArrow
		{
			get
			{
				return this._followArrow;
			}
			set
			{
				this._followArrow = value;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.InitializeVisuals();
			this.DisableVisuals();
			this.EndStart(ref this._started);
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.DisableVisuals();
			}
		}

		public void DisableVisuals()
		{
			this._leftTrail.enabled = false;
			this._rightTrail.enabled = false;
			this._leftArrow.enabled = false;
			this._rightArrow.enabled = false;
			this._leftRail.enabled = false;
			this._rightRail.enabled = false;
		}

		private void InitializeVisuals()
		{
			TubePoint[] points = this.InitializeSegment(new Vector2(this._margin, this._maxAngle + this._squeezeLength));
			this._leftTrail.RenderTube(points, Space.Self);
			this._rightTrail.RenderTube(points, Space.Self);
			TubePoint[] points2 = this.InitializeSegment(new Vector2(this._margin, this._maxAngle));
			this._leftRail.RenderTube(points2, Space.Self);
			this._rightRail.RenderTube(points2, Space.Self);
		}

		public void UpdateVisual()
		{
			this.UpdateArrows(this.Value);
			this.UpdateColors(this.HighLight, this.Value);
		}

		private void UpdateArrows(float value)
		{
			float num = Mathf.Lerp(0f, this._maxAngle, Mathf.Abs(value));
			bool flag = value < 0f;
			bool followArrow = this._followArrow;
			float num2 = Mathf.Lerp(0f, this._squeezeLength, this._progress);
			this._leftTrail.enabled = true;
			this._rightTrail.enabled = true;
			this._leftArrow.enabled = true;
			this._rightArrow.enabled = true;
			this._rightRail.enabled = !flag;
			this._leftRail.enabled = flag;
			num = Mathf.Max(num, this._trailLength);
			this.UpdateArrowPosition(flag ? this._trailLength : (num + num2), this._rightArrow.transform);
			this.RotateTrail((followArrow && !flag) ? (num - this._trailLength) : 0f, this._rightTrail);
			this.UpdateTrail(flag ? this._trailLength : ((followArrow ? this._trailLength : num) + num2), this._rightTrail);
			this.UpdateArrowPosition((!flag) ? (-this._trailLength) : (-num - num2), this._leftArrow.transform);
			this.RotateTrail((followArrow && flag) ? (-num + this._trailLength) : 0f, this._leftTrail);
			this.UpdateTrail((!flag) ? this._trailLength : ((followArrow ? this._trailLength : num) + num2), this._leftTrail);
			this.UpdateRail(num, num2, flag ? this._leftRail : this._rightRail);
		}

		private void UpdateArrowPosition(float angle, Transform arrow)
		{
			Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
			arrow.localPosition = quaternion * Vector3.forward * this._radius;
			arrow.localRotation = quaternion * TurnArrowVisuals._rotationCorrectionLeft;
		}

		private void RotateTrail(float angle, TubeRenderer trail)
		{
			trail.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
		}

		private void UpdateTrail(float angle, TubeRenderer trail)
		{
			float num = this._maxAngle + this._squeezeLength;
			float totalLength = trail.TotalLength;
			float num2 = -100f;
			float num3 = (num - angle - this._margin) / num;
			trail.StartFadeThresold = totalLength * num2;
			trail.EndFadeThresold = totalLength * num3;
			trail.InvertThreshold = false;
			trail.RedrawFadeThresholds();
		}

		private void UpdateRail(float angle, float extra, TubeRenderer rail)
		{
			float totalLength = rail.TotalLength;
			float num = (angle - this._trailLength - this._margin) / this._maxAngle;
			float num2 = (this._maxAngle - angle - extra - this._margin) / this._maxAngle;
			float num3 = this._railGap + rail.Feather;
			rail.StartFadeThresold = totalLength * num - num3;
			rail.EndFadeThresold = totalLength * num2 - num3;
			rail.InvertThreshold = true;
			rail.RedrawFadeThresholds();
		}

		private void UpdateColors(bool isSelection, float value)
		{
			this._leftMaterialBlock.MaterialPropertyBlock.SetColor(TurnArrowVisuals._colorShaderPropertyID, (value < 0f) ? (isSelection ? this._highligtedColor : this._enabledColor) : this._disabledColor);
			this._rightMaterialBlock.MaterialPropertyBlock.SetColor(TurnArrowVisuals._colorShaderPropertyID, (value > 0f) ? (isSelection ? this._highligtedColor : this._enabledColor) : this._disabledColor);
			this._leftMaterialBlock.UpdateMaterialPropertyBlock();
			this._rightMaterialBlock.UpdateMaterialPropertyBlock();
		}

		private TubePoint[] InitializeSegment(Vector2 minMax)
		{
			float x = minMax.x;
			int num = Mathf.RoundToInt(Mathf.Repeat(minMax.y - x, 360f) / 1f);
			TubePoint[] array = new TubePoint[num];
			float num2 = 1f / (float)num;
			for (int i = 0; i < num; i++)
			{
				Quaternion quaternion = Quaternion.AngleAxis((float)(-(float)i) * 1f - x, Vector3.up);
				array[i] = new TubePoint
				{
					position = quaternion * Vector3.forward * this._radius,
					rotation = quaternion * TurnArrowVisuals._rotationCorrectionLeft,
					relativeLength = (float)i * num2
				};
			}
			return array;
		}

		public void InjectAllTurnArrowVisuals(Renderer leftArrow, Renderer rightArrow, TubeRenderer leftRail, TubeRenderer rightRail, TubeRenderer leftTrail, TubeRenderer rightTrail, MaterialPropertyBlockEditor leftMaterialBlock, MaterialPropertyBlockEditor rightMaterialBlock, float radius, float margin, float trailLength, float maxAngle, float railGap, float squeezeLength)
		{
			this.InjectLeftArrow(leftArrow);
			this.InjectRightArrow(rightArrow);
			this.InjectLeftRail(leftRail);
			this.InjectRightRail(rightRail);
			this.InjectLeftTrail(leftTrail);
			this.InjectRightTrail(rightTrail);
			this.InjectLeftMaterialBlock(leftMaterialBlock);
			this.InjectRightMaterialBlock(rightMaterialBlock);
			this.InjectRadius(radius);
			this.InjectMargin(margin);
			this.InjectTrailLength(trailLength);
			this.InjectMaxAngle(maxAngle);
			this.InjectRailGap(railGap);
			this.InjectSqueezeLength(squeezeLength);
		}

		public void InjectLeftArrow(Renderer leftArrow)
		{
			this._leftArrow = leftArrow;
		}

		public void InjectRightArrow(Renderer rightArrow)
		{
			this._rightArrow = rightArrow;
		}

		public void InjectLeftRail(TubeRenderer leftRail)
		{
			this._leftRail = leftRail;
		}

		public void InjectRightRail(TubeRenderer rightRail)
		{
			this._rightRail = rightRail;
		}

		public void InjectLeftTrail(TubeRenderer leftTrail)
		{
			this._leftTrail = leftTrail;
		}

		public void InjectRightTrail(TubeRenderer rightTrail)
		{
			this._rightTrail = rightTrail;
		}

		public void InjectLeftMaterialBlock(MaterialPropertyBlockEditor leftMaterialBlock)
		{
			this._leftMaterialBlock = leftMaterialBlock;
		}

		public void InjectRightMaterialBlock(MaterialPropertyBlockEditor rightMaterialBlock)
		{
			this._rightMaterialBlock = rightMaterialBlock;
		}

		public void InjectRadius(float radius)
		{
			this._radius = radius;
		}

		public void InjectMargin(float margin)
		{
			this._margin = margin;
		}

		public void InjectTrailLength(float trailLength)
		{
			this._trailLength = trailLength;
		}

		public void InjectMaxAngle(float maxAngle)
		{
			this._maxAngle = maxAngle;
		}

		public void InjectRailGap(float railGap)
		{
			this._railGap = railGap;
		}

		public void InjectSqueezeLength(float squeezeLength)
		{
			this._squeezeLength = squeezeLength;
		}

		[Header("Visual renderers")]
		[Tooltip("Renderer for the Left arrow cone")]
		[SerializeField]
		private Renderer _leftArrow;

		[Tooltip("Renderer for the Right arrow cone")]
		[SerializeField]
		private Renderer _rightArrow;

		[Tooltip("TubeRenderer that will draw the rail of the left arrow")]
		[SerializeField]
		private TubeRenderer _leftRail;

		[Tooltip("TubeRenderer that will draw the rail of the right arrow")]
		[SerializeField]
		private TubeRenderer _rightRail;

		[Tooltip("TubeRenderer that will draw the trail of the right arrow")]
		[SerializeField]
		private TubeRenderer _leftTrail;

		[Tooltip("TubeRenderer that will draw the trail of the right arrow")]
		[SerializeField]
		private TubeRenderer _rightTrail;

		[Tooltip("Material block for the left arrow items so they can be controller")]
		[SerializeField]
		private MaterialPropertyBlockEditor _leftMaterialBlock;

		[Tooltip("Material block for the right arrow items so they can be controller")]
		[SerializeField]
		private MaterialPropertyBlockEditor _rightMaterialBlock;

		[Header("Visual parameters")]
		[Tooltip("Radius of the circle in which the arrows are circunscribed")]
		[SerializeField]
		private float _radius = 0.07f;

		[Tooltip("Gap, in degrees, left between the arrows")]
		[SerializeField]
		private float _margin = 2f;

		[Tooltip("Length, in degrees, of the trail of the arrows")]
		[SerializeField]
		private float _trailLength = 15f;

		[Tooltip("Max angle, in degrees, the arrows can follow when highlighted")]
		[SerializeField]
		private float _maxAngle = 45f;

		[Tooltip("Length of the transparent gap in the rail left by the arrow")]
		[SerializeField]
		private float _railGap = 0.005f;

		[Tooltip("Length, in degrees, that the arrows can grow when highlighted")]
		[SerializeField]
		private float _squeezeLength = 5f;

		[Header("Visual controllers")]
		[Tooltip("Color of the arrow when not active")]
		[SerializeField]
		private Color _disabledColor = new Color(1f, 1f, 1f, 0.2f);

		[Tooltip("Color of the arrow when active")]
		[SerializeField]
		private Color _enabledColor = new Color(1f, 1f, 1f, 0.6f);

		[Tooltip("Color of the arrow when highlighted")]
		[SerializeField]
		private Color _highligtedColor = new Color(1f, 1f, 1f, 1f);

		[Tooltip("If true, the current active arrow will")]
		[SerializeField]
		private bool _highLight;

		[Tooltip("This value controls wich arrow is active, <0 for the left and >0 for the right")]
		[SerializeField]
		private float _value;

		[Tooltip("Indicates how much the active arrow must grow")]
		[SerializeField]
		private float _progress;

		[Tooltip("Indicates wheter the active arrow should follow the rail")]
		[SerializeField]
		private bool _followArrow;

		private const float _degreesPerSegment = 1f;

		private static readonly Quaternion _rotationCorrectionLeft = Quaternion.Euler(0f, -90f, 0f);

		private static readonly int _colorShaderPropertyID = Shader.PropertyToID("_Color");

		protected bool _started;
	}
}
