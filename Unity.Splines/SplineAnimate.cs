using System;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	[AddComponentMenu("Splines/Spline Animate")]
	[ExecuteInEditMode]
	public class SplineAnimate : SplineComponent
	{
		[Obsolete("Use Container instead.", false)]
		public SplineContainer splineContainer
		{
			get
			{
				return this.Container;
			}
		}

		public SplineContainer Container
		{
			get
			{
				return this.m_Target;
			}
			set
			{
				this.m_Target = value;
				if (base.enabled && this.m_Target != null && this.m_Target.Splines != null)
				{
					for (int i = 0; i < this.m_Target.Splines.Count; i++)
					{
						this.OnSplineChange(this.m_Target.Splines[i], -1, SplineModification.Default);
					}
				}
				this.UpdateStartOffsetT();
			}
		}

		[Obsolete("Use PlayOnAwake instead.", false)]
		public bool playOnAwake
		{
			get
			{
				return this.PlayOnAwake;
			}
		}

		public bool PlayOnAwake
		{
			get
			{
				return this.m_PlayOnAwake;
			}
			set
			{
				this.m_PlayOnAwake = value;
			}
		}

		[Obsolete("Use Loop instead.", false)]
		public SplineAnimate.LoopMode loopMode
		{
			get
			{
				return this.Loop;
			}
		}

		public SplineAnimate.LoopMode Loop
		{
			get
			{
				return this.m_LoopMode;
			}
			set
			{
				this.m_LoopMode = value;
			}
		}

		[Obsolete("Use AnimationMethod instead.", false)]
		public SplineAnimate.Method method
		{
			get
			{
				return this.AnimationMethod;
			}
		}

		public SplineAnimate.Method AnimationMethod
		{
			get
			{
				return this.m_Method;
			}
			set
			{
				this.m_Method = value;
			}
		}

		[Obsolete("Use Duration instead.", false)]
		public float duration
		{
			get
			{
				return this.Duration;
			}
		}

		public float Duration
		{
			get
			{
				return this.m_Duration;
			}
			set
			{
				if (this.m_Method == SplineAnimate.Method.Time)
				{
					this.m_Duration = Mathf.Max(0f, value);
					this.CalculateMaxSpeed();
				}
			}
		}

		[Obsolete("Use MaxSpeed instead.", false)]
		public float maxSpeed
		{
			get
			{
				return this.MaxSpeed;
			}
		}

		public float MaxSpeed
		{
			get
			{
				return this.m_MaxSpeed;
			}
			set
			{
				if (this.m_Method == SplineAnimate.Method.Speed)
				{
					this.m_MaxSpeed = Mathf.Max(0f, value);
					this.CalculateDuration();
				}
			}
		}

		[Obsolete("Use Easing instead.", false)]
		public SplineAnimate.EasingMode easingMode
		{
			get
			{
				return this.Easing;
			}
		}

		public SplineAnimate.EasingMode Easing
		{
			get
			{
				return this.m_EasingMode;
			}
			set
			{
				this.m_EasingMode = value;
			}
		}

		[Obsolete("Use Alignment instead.", false)]
		public SplineAnimate.AlignmentMode alignmentMode
		{
			get
			{
				return this.Alignment;
			}
		}

		public SplineAnimate.AlignmentMode Alignment
		{
			get
			{
				return this.m_AlignmentMode;
			}
			set
			{
				this.m_AlignmentMode = value;
			}
		}

		[Obsolete("Use ObjectForwardAxis instead.", false)]
		public SplineComponent.AlignAxis objectForwardAxis
		{
			get
			{
				return this.ObjectForwardAxis;
			}
		}

		public SplineComponent.AlignAxis ObjectForwardAxis
		{
			get
			{
				return this.m_ObjectForwardAxis;
			}
			set
			{
				this.m_ObjectUpAxis = this.SetObjectAlignAxis(value, ref this.m_ObjectForwardAxis, this.m_ObjectUpAxis);
			}
		}

		[Obsolete("Use ObjectUpAxis instead.", false)]
		public SplineComponent.AlignAxis objectUpAxis
		{
			get
			{
				return this.ObjectUpAxis;
			}
		}

		public SplineComponent.AlignAxis ObjectUpAxis
		{
			get
			{
				return this.m_ObjectUpAxis;
			}
			set
			{
				this.m_ObjectForwardAxis = this.SetObjectAlignAxis(value, ref this.m_ObjectUpAxis, this.m_ObjectForwardAxis);
			}
		}

		[Obsolete("Use NormalizedTime instead.", false)]
		public float normalizedTime
		{
			get
			{
				return this.NormalizedTime;
			}
		}

		public float NormalizedTime
		{
			get
			{
				return this.m_NormalizedTime;
			}
			set
			{
				this.m_NormalizedTime = value;
				if (this.m_LoopMode == SplineAnimate.LoopMode.PingPong)
				{
					int num = (int)(this.m_ElapsedTime / this.m_Duration);
					this.m_ElapsedTime = this.m_Duration * this.m_NormalizedTime + ((num % 2 == 1) ? this.m_Duration : 0f);
				}
				else
				{
					this.m_ElapsedTime = this.m_Duration * this.m_NormalizedTime;
				}
				this.UpdateTransform();
			}
		}

		[Obsolete("Use ElapsedTime instead.", false)]
		public float elapsedTime
		{
			get
			{
				return this.ElapsedTime;
			}
		}

		public float ElapsedTime
		{
			get
			{
				return this.m_ElapsedTime;
			}
			set
			{
				this.m_ElapsedTime = value;
				this.CalculateNormalizedTime(0f);
				this.UpdateTransform();
			}
		}

		public float StartOffset
		{
			get
			{
				return this.m_StartOffset;
			}
			set
			{
				if (this.m_SplineLength < 0f)
				{
					this.RebuildSplinePath();
				}
				this.m_StartOffset = Mathf.Clamp01(value);
				this.UpdateStartOffsetT();
			}
		}

		internal float StartOffsetT
		{
			get
			{
				return this.m_StartOffsetT;
			}
		}

		[Obsolete("Use IsPlaying instead.", false)]
		public bool isPlaying
		{
			get
			{
				return this.IsPlaying;
			}
		}

		public bool IsPlaying
		{
			get
			{
				return this.m_Playing;
			}
		}

		[Obsolete("Use Updated instead.", false)]
		public event Action<Vector3, Quaternion> onUpdated;

		public event Action<Vector3, Quaternion> Updated;

		public event Action Completed;

		private void Awake()
		{
			this.m_PlayOnAwakeHandledForSession = false;
			this.RecalculateAnimationParameters();
		}

		private void OnEnable()
		{
			this.RecalculateAnimationParameters();
			Spline.Changed += this.OnSplineChange;
			if (!this.m_PlayOnAwakeHandledForSession)
			{
				this.Restart(this.m_PlayOnAwake);
				this.m_PlayOnAwakeHandledForSession = true;
			}
		}

		private void OnDisable()
		{
			Spline.Changed -= this.OnSplineChange;
		}

		private void OnValidate()
		{
			this.m_Duration = Mathf.Max(0f, this.m_Duration);
			this.m_MaxSpeed = Mathf.Max(0f, this.m_MaxSpeed);
			this.RecalculateAnimationParameters();
		}

		internal void RecalculateAnimationParameters()
		{
			this.RebuildSplinePath();
			SplineAnimate.Method method = this.m_Method;
			if (method == SplineAnimate.Method.Time)
			{
				this.CalculateMaxSpeed();
				return;
			}
			if (method != SplineAnimate.Method.Speed)
			{
				Debug.Log(string.Format("{0} animation method is not supported!", this.m_Method), this);
				return;
			}
			this.CalculateDuration();
		}

		private bool IsNullOrEmptyContainer()
		{
			if (this.m_Target == null || this.m_Target.Splines.Count == 0)
			{
				if (Application.isPlaying)
				{
					Debug.LogError(SplineAnimate.k_EmptyContainerError, this);
				}
				return true;
			}
			return false;
		}

		public void Play()
		{
			if (this.IsNullOrEmptyContainer())
			{
				return;
			}
			this.m_Playing = true;
		}

		public void Pause()
		{
			this.m_Playing = false;
		}

		public void Restart(bool autoplay)
		{
			if (this.Container == null)
			{
				return;
			}
			if (this.IsNullOrEmptyContainer())
			{
				return;
			}
			this.m_Playing = false;
			this.m_ElapsedTime = 0f;
			this.NormalizedTime = 0f;
			SplineAnimate.Method method = this.m_Method;
			if (method != SplineAnimate.Method.Time)
			{
				if (method != SplineAnimate.Method.Speed)
				{
					Debug.Log(string.Format("{0} animation method is not supported!", this.m_Method), this);
				}
				else
				{
					this.CalculateDuration();
				}
			}
			else
			{
				this.CalculateMaxSpeed();
			}
			this.UpdateTransform();
			this.UpdateStartOffsetT();
			if (autoplay)
			{
				this.Play();
			}
		}

		public void Update()
		{
			if (!this.m_Playing || (this.m_LoopMode == SplineAnimate.LoopMode.Once && this.m_NormalizedTime >= 1f))
			{
				return;
			}
			float deltaTime = Time.deltaTime;
			this.CalculateNormalizedTime(deltaTime);
			this.UpdateTransform();
		}

		private void CalculateNormalizedTime(float deltaTime)
		{
			float elapsedTime = this.m_ElapsedTime;
			this.m_ElapsedTime += deltaTime;
			float num = this.m_Duration;
			float num2 = 0f;
			switch (this.m_LoopMode)
			{
			case SplineAnimate.LoopMode.Once:
				num2 = Mathf.Min(this.m_ElapsedTime, num);
				break;
			case SplineAnimate.LoopMode.Loop:
				num2 = this.m_ElapsedTime % num;
				this.UpdateEndReached(elapsedTime, num);
				break;
			case SplineAnimate.LoopMode.LoopEaseInOnce:
				if ((this.m_EasingMode == SplineAnimate.EasingMode.EaseIn || this.m_EasingMode == SplineAnimate.EasingMode.EaseInOut) && this.m_ElapsedTime >= num)
				{
					num *= 0.5f;
				}
				num2 = this.m_ElapsedTime % num;
				this.UpdateEndReached(elapsedTime, num);
				break;
			case SplineAnimate.LoopMode.PingPong:
				num2 = Mathf.PingPong(this.m_ElapsedTime, num);
				this.UpdateEndReached(elapsedTime, num);
				break;
			default:
				Debug.Log(string.Format("{0} animation loop mode is not supported!", this.m_LoopMode), this);
				break;
			}
			num2 /= num;
			if (this.m_LoopMode == SplineAnimate.LoopMode.LoopEaseInOnce)
			{
				if ((this.m_EasingMode == SplineAnimate.EasingMode.EaseIn || this.m_EasingMode == SplineAnimate.EasingMode.EaseInOut) && this.m_ElapsedTime < num)
				{
					num2 = this.EaseInQuadratic(num2);
				}
			}
			else
			{
				switch (this.m_EasingMode)
				{
				case SplineAnimate.EasingMode.EaseIn:
					num2 = this.EaseInQuadratic(num2);
					break;
				case SplineAnimate.EasingMode.EaseOut:
					num2 = this.EaseOutQuadratic(num2);
					break;
				case SplineAnimate.EasingMode.EaseInOut:
					num2 = this.EaseInOutQuadratic(num2);
					break;
				}
			}
			this.m_NormalizedTime = ((num2 == 0f) ? 0f : (Mathf.Floor(this.m_NormalizedTime) + num2));
			if (this.m_NormalizedTime >= 1f && this.m_LoopMode == SplineAnimate.LoopMode.Once)
			{
				this.m_EndReached = true;
				this.m_Playing = false;
			}
		}

		private void UpdateEndReached(float previousTime, float currentDuration)
		{
			this.m_EndReached = (Mathf.FloorToInt(previousTime / currentDuration) < Mathf.FloorToInt(this.m_ElapsedTime / currentDuration));
		}

		private void UpdateStartOffsetT()
		{
			if (this.m_SplinePath != null)
			{
				this.m_StartOffsetT = this.m_SplinePath.ConvertIndexUnit(this.m_StartOffset * this.m_SplineLength, PathIndexUnit.Distance, PathIndexUnit.Normalized);
			}
		}

		private void UpdateTransform()
		{
			if (this.m_Target == null)
			{
				return;
			}
			Vector3 vector;
			Quaternion quaternion;
			this.EvaluatePositionAndRotation(out vector, out quaternion);
			base.transform.position = vector;
			if (this.m_AlignmentMode != SplineAnimate.AlignmentMode.None)
			{
				base.transform.rotation = quaternion;
			}
			Action<Vector3, Quaternion> action = this.onUpdated;
			if (action != null)
			{
				action(vector, quaternion);
			}
			Action<Vector3, Quaternion> updated = this.Updated;
			if (updated != null)
			{
				updated(vector, quaternion);
			}
			if (this.m_EndReached)
			{
				this.m_EndReached = false;
				Action completed = this.Completed;
				if (completed == null)
				{
					return;
				}
				completed();
			}
		}

		private void EvaluatePositionAndRotation(out Vector3 position, out Quaternion rotation)
		{
			float loopInterpolation = this.GetLoopInterpolation(true);
			position = this.m_Target.EvaluatePosition<SplinePath<Spline>>(this.m_SplinePath, loopInterpolation);
			rotation = Quaternion.identity;
			float3 axis = base.GetAxis(this.m_ObjectForwardAxis);
			float3 axis2 = base.GetAxis(this.m_ObjectUpAxis);
			Quaternion rhs = Quaternion.Inverse(Quaternion.LookRotation(axis, axis2));
			if (this.m_AlignmentMode == SplineAnimate.AlignmentMode.None)
			{
				rotation = base.transform.rotation;
				return;
			}
			Vector3 vector = Vector3.forward;
			Vector3 vector2 = Vector3.up;
			SplineAnimate.AlignmentMode alignmentMode = this.m_AlignmentMode;
			if (alignmentMode != SplineAnimate.AlignmentMode.SplineElement)
			{
				if (alignmentMode != SplineAnimate.AlignmentMode.SplineObject)
				{
					Debug.Log(string.Format("{0} animation alignment mode is not supported!", this.m_AlignmentMode), this);
				}
				else
				{
					Quaternion rotation2 = this.m_Target.transform.rotation;
					vector = rotation2 * vector;
					vector2 = rotation2 * vector2;
				}
			}
			else
			{
				vector = this.m_Target.EvaluateTangent<SplinePath<Spline>>(this.m_SplinePath, loopInterpolation);
				if (Vector3.Magnitude(vector) <= Mathf.Epsilon)
				{
					if (loopInterpolation < 1f)
					{
						vector = this.m_Target.EvaluateTangent<SplinePath<Spline>>(this.m_SplinePath, Mathf.Min(1f, loopInterpolation + 0.01f));
					}
					else
					{
						vector = this.m_Target.EvaluateTangent<SplinePath<Spline>>(this.m_SplinePath, loopInterpolation - 0.01f);
					}
				}
				vector.Normalize();
				vector2 = this.m_Target.EvaluateUpVector<SplinePath<Spline>>(this.m_SplinePath, loopInterpolation);
			}
			if (math.all(math.isfinite(vector) & math.isfinite(vector2)))
			{
				rotation = Quaternion.LookRotation(vector, vector2) * rhs;
				return;
			}
			Debug.LogError("Trying to EvaluatePositionAndRotation with invalid parameters. Please check the SplineAnimate component.", this);
		}

		private void CalculateDuration()
		{
			if (this.m_SplineLength < 0f)
			{
				this.RebuildSplinePath();
			}
			if (this.m_SplineLength >= 0f)
			{
				SplineAnimate.EasingMode easingMode = this.m_EasingMode;
				if (easingMode == SplineAnimate.EasingMode.None)
				{
					this.m_Duration = this.m_SplineLength / this.m_MaxSpeed;
					return;
				}
				if (easingMode - SplineAnimate.EasingMode.EaseIn <= 2)
				{
					this.m_Duration = 2f * this.m_SplineLength / this.m_MaxSpeed;
					return;
				}
				Debug.Log(string.Format("{0} animation easing mode is not supported!", this.m_EasingMode), this);
			}
		}

		private void CalculateMaxSpeed()
		{
			if (this.m_SplineLength < 0f)
			{
				this.RebuildSplinePath();
			}
			if (this.m_SplineLength >= 0f)
			{
				SplineAnimate.EasingMode easingMode = this.m_EasingMode;
				if (easingMode == SplineAnimate.EasingMode.None)
				{
					this.m_MaxSpeed = this.m_SplineLength / this.m_Duration;
					return;
				}
				if (easingMode - SplineAnimate.EasingMode.EaseIn <= 2)
				{
					this.m_MaxSpeed = 2f * this.m_SplineLength / this.m_Duration;
					return;
				}
				Debug.Log(string.Format("{0} animation easing mode is not supported!", this.m_EasingMode), this);
			}
		}

		private void RebuildSplinePath()
		{
			if (this.m_Target != null)
			{
				this.m_SplinePath = new SplinePath<Spline>(this.m_Target.Splines);
				this.m_SplineLength = ((this.m_SplinePath != null) ? this.m_SplinePath.GetLength() : 0f);
			}
		}

		private SplineComponent.AlignAxis SetObjectAlignAxis(SplineComponent.AlignAxis newValue, ref SplineComponent.AlignAxis targetAxis, SplineComponent.AlignAxis otherAxis)
		{
			if (newValue == otherAxis)
			{
				otherAxis = targetAxis;
				targetAxis = newValue;
			}
			else if (newValue % SplineComponent.AlignAxis.NegativeXAxis != otherAxis % SplineComponent.AlignAxis.NegativeXAxis)
			{
				targetAxis = newValue;
			}
			return otherAxis;
		}

		private void OnSplineChange(Spline spline, int knotIndex, SplineModification modificationType)
		{
			this.RecalculateAnimationParameters();
		}

		internal float GetLoopInterpolation(bool offset)
		{
			float num = this.NormalizedTime + (offset ? this.m_StartOffsetT : 0f);
			float result;
			if (Mathf.Floor(num) == num)
			{
				result = Mathf.Clamp01(num);
			}
			else
			{
				result = num % 1f;
			}
			return result;
		}

		private float EaseInQuadratic(float t)
		{
			return t * t;
		}

		private float EaseOutQuadratic(float t)
		{
			return t * (2f - t);
		}

		private float EaseInOutQuadratic(float t)
		{
			float num = 2f * t * t;
			if (t > 0.5f)
			{
				num = 4f * t - num - 1f;
			}
			return num;
		}

		[SerializeField]
		[Tooltip("The target spline to follow.")]
		private SplineContainer m_Target;

		[SerializeField]
		[Tooltip("Enable to have the animation start when the GameObject first loads.")]
		private bool m_PlayOnAwake = true;

		[SerializeField]
		[Tooltip("The loop mode that the animation uses. Loop modes cause the animation to repeat after it finishes. The following loop modes are available:.\nOnce - Traverse the spline once and stop at the end.\nLoop Continuous - Traverse the spline continuously without stopping.\nEase In Then Continuous - Traverse the spline repeatedly without stopping. If Ease In easing is enabled, apply easing to the first loop only.\nPing Pong - Traverse the spline continuously without stopping and reverse direction after an end of the spline is reached.\n")]
		private SplineAnimate.LoopMode m_LoopMode = SplineAnimate.LoopMode.Loop;

		[SerializeField]
		[Tooltip("The method used to animate the GameObject along the spline.\nTime - The spline is traversed in a given amount of seconds.\nSpeed - The spline is traversed at a given maximum speed.")]
		private SplineAnimate.Method m_Method;

		[SerializeField]
		[Tooltip("The period of time that it takes for the GameObject to complete its animation along the spline.")]
		private float m_Duration = 1f;

		[SerializeField]
		[Tooltip("The speed in meters/second that the GameObject animates along the spline at.")]
		private float m_MaxSpeed = 10f;

		[SerializeField]
		[Tooltip("The easing mode used when the GameObject animates along the spline.\nNone - Apply no easing to the animation. The animation speed is linear.\nEase In Only - Apply easing to the beginning of animation.\nEase Out Only - Apply easing to the end of animation.\nEase In-Out - Apply easing to the beginning and end of animation.\n")]
		private SplineAnimate.EasingMode m_EasingMode;

		[SerializeField]
		[Tooltip("The coordinate space that the GameObject's up and forward axes align to.")]
		private SplineAnimate.AlignmentMode m_AlignmentMode = SplineAnimate.AlignmentMode.SplineElement;

		[SerializeField]
		[Tooltip("Which axis of the GameObject is treated as the forward axis.")]
		private SplineComponent.AlignAxis m_ObjectForwardAxis = SplineComponent.AlignAxis.ZAxis;

		[SerializeField]
		[Tooltip("Which axis of the GameObject is treated as the up axis.")]
		private SplineComponent.AlignAxis m_ObjectUpAxis = SplineComponent.AlignAxis.YAxis;

		[SerializeField]
		[Tooltip("Normalized distance [0;1] offset along the spline at which the GameObject should be placed when the animation begins.")]
		private float m_StartOffset;

		[NonSerialized]
		private float m_StartOffsetT;

		private bool m_PlayOnAwakeHandledForSession;

		private float m_SplineLength = -1f;

		private bool m_Playing;

		private float m_NormalizedTime;

		private float m_ElapsedTime;

		private SplinePath<Spline> m_SplinePath;

		private bool m_EndReached;

		internal static readonly string k_EmptyContainerError = "SplineAnimate does not have a valid SplineContainer set.";

		public enum Method
		{
			Time,
			Speed
		}

		public enum LoopMode
		{
			[InspectorName("Once")]
			Once,
			[InspectorName("Loop Continuous")]
			Loop,
			[InspectorName("Ease In Then Continuous")]
			LoopEaseInOnce,
			[InspectorName("Ping Pong")]
			PingPong
		}

		public enum EasingMode
		{
			[InspectorName("None")]
			None,
			[InspectorName("Ease In Only")]
			EaseIn,
			[InspectorName("Ease Out Only")]
			EaseOut,
			[InspectorName("Ease In-Out")]
			EaseInOut
		}

		public enum AlignmentMode
		{
			[InspectorName("None")]
			None,
			[InspectorName("Spline Element")]
			SplineElement,
			[InspectorName("Spline Object")]
			SplineObject,
			[InspectorName("World Space")]
			World
		}
	}
}
