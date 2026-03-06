using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class LocomotionTutorialTurnVisual : MonoBehaviour
	{
		public float VerticalOffset
		{
			get
			{
				return this._verticalOffset;
			}
			set
			{
				this._verticalOffset = value;
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

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.InitializeVisuals();
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			this._leftTrail.enabled = true;
			this._rightTrail.enabled = true;
			this._leftArrow.enabled = true;
			this._rightArrow.enabled = true;
		}

		protected virtual void OnDisable()
		{
			this._leftTrail.enabled = false;
			this._rightTrail.enabled = false;
			this._leftArrow.enabled = false;
			this._rightArrow.enabled = false;
		}

		protected virtual void Update()
		{
			this.UpdateArrows();
			this.UpdateColors();
		}

		private void InitializeVisuals()
		{
			TubePoint[] points = this.InitializeSegment(new Vector2(this._margin, this._maxAngle + this._squeezeLength));
			this._leftTrail.RenderTube(points, Space.Self);
			this._rightTrail.RenderTube(points, Space.Self);
		}

		private void UpdateArrows()
		{
			float value = this._value;
			float num = Mathf.Lerp(0f, this._maxAngle, Mathf.Abs(value));
			bool flag = value < 0f;
			bool flag2 = value > 0f;
			bool flag3 = false;
			float num2 = Mathf.Lerp(0f, this._squeezeLength, this._progress);
			num = Mathf.Max(num, this._trailLength);
			this.UpdateArrowPosition(flag2 ? (num + num2) : this._trailLength, this._rightArrow.transform);
			this.RotateTrail((flag3 && flag2) ? (num - this._trailLength) : 0f, this._rightTrail);
			this.UpdateTrail(flag2 ? ((flag3 ? this._trailLength : num) + num2) : this._trailLength, this._rightTrail);
			this.UpdateArrowPosition(flag ? (-num - num2) : (-this._trailLength), this._leftArrow.transform);
			this.RotateTrail((flag3 && flag) ? (-num + this._trailLength) : 0f, this._leftTrail);
			this.UpdateTrail(flag ? ((flag3 ? this._trailLength : num) + num2) : this._trailLength, this._leftTrail);
		}

		private void UpdateArrowPosition(float angle, Transform arrow)
		{
			Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
			arrow.localPosition = quaternion * Vector3.forward * this._radius;
			arrow.localRotation = quaternion * LocomotionTutorialTurnVisual._rotationCorrectionLeft;
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

		private void UpdateColors()
		{
			bool flag = Mathf.Abs(this._progress) >= 1f;
			bool flag2 = this._value < 0f;
			bool flag3 = this._value > 0f;
			Color color = flag ? this._highligtedColor : this._enabledColor;
			this._leftMaterialBlock.MaterialPropertyBlock.SetColor(LocomotionTutorialTurnVisual._colorShaderPropertyID, flag2 ? color : this._disabledColor);
			this._rightMaterialBlock.MaterialPropertyBlock.SetColor(LocomotionTutorialTurnVisual._colorShaderPropertyID, flag3 ? color : this._disabledColor);
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
					rotation = quaternion * LocomotionTutorialTurnVisual._rotationCorrectionLeft,
					relativeLength = (float)i * num2
				};
			}
			return array;
		}

		[SerializeField]
		[Range(-1f, 1f)]
		private float _value;

		[SerializeField]
		[Range(0f, 1f)]
		private float _progress;

		[Header("Visual renderers")]
		[SerializeField]
		private Renderer _leftArrow;

		[SerializeField]
		private Renderer _rightArrow;

		[SerializeField]
		private TubeRenderer _leftTrail;

		[SerializeField]
		private TubeRenderer _rightTrail;

		[SerializeField]
		private MaterialPropertyBlockEditor _leftMaterialBlock;

		[SerializeField]
		private MaterialPropertyBlockEditor _rightMaterialBlock;

		[Header("Visual parameters")]
		[SerializeField]
		private float _verticalOffset = 0.02f;

		[SerializeField]
		private float _radius = 0.07f;

		[SerializeField]
		private float _margin = 2f;

		[SerializeField]
		private float _trailLength = 15f;

		[SerializeField]
		private float _maxAngle = 45f;

		[SerializeField]
		private float _railGap = 0.005f;

		[SerializeField]
		private float _squeezeLength = 5f;

		[SerializeField]
		private Color _disabledColor = new Color(1f, 1f, 1f, 0.2f);

		[SerializeField]
		private Color _enabledColor = new Color(1f, 1f, 1f, 0.6f);

		[SerializeField]
		private Color _highligtedColor = new Color(1f, 1f, 1f, 1f);

		private const float _degreesPerSegment = 1f;

		private static readonly Quaternion _rotationCorrectionLeft = Quaternion.Euler(0f, -90f, 0f);

		private static readonly int _colorShaderPropertyID = Shader.PropertyToID("_Color");

		protected bool _started;
	}
}
