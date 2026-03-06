using System;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.Input
{
	[Feature(Feature.Interaction)]
	public class AnimatedHandOVR : MonoBehaviour, IDeltaTimeConsumer
	{
		public AnimatedHandOVR.AllowThumbUp AllowThumbUpMode
		{
			get
			{
				return this._allowThumbUp;
			}
			set
			{
				this._allowThumbUp = value;
			}
		}

		public float AnimFlexGain
		{
			get
			{
				return this._animFlexGain;
			}
			set
			{
				this._animFlexGain = value;
			}
		}

		public float AnimPinchGain
		{
			get
			{
				return this._animPinchGain;
			}
			set
			{
				this._animPinchGain = value;
			}
		}

		public float AnimPointAndThumbsUpGain
		{
			get
			{
				return this._animPointAndThumbsUpGain;
			}
			set
			{
				this._animPointAndThumbsUpGain = value;
			}
		}

		public Func<float> DeltaTimeProvider { get; set; } = () => Time.deltaTime;

		public void SetDeltaTimeProvider(Func<float> deltaTimeProvider)
		{
			this._deltaTimeProvider = deltaTimeProvider;
		}

		protected virtual void Start()
		{
			this._animLayerIndexPoint = this._animator.GetLayerIndex("Point Layer");
			this._animLayerIndexThumb = this._animator.GetLayerIndex("Thumb Layer");
		}

		protected virtual void Update()
		{
			this.UpdateCapTouchStates();
			this._pointBlend = Mathf.Lerp(this._pointBlend, this._pointTarget, this._animPointAndThumbsUpGain * this._deltaTimeProvider());
			this._slideBlend = Mathf.Lerp(this._slideBlend, this._slideTarget, this._animPointAndThumbsUpGain * this._deltaTimeProvider());
			this._thumbsUpBlend = Mathf.Lerp(this._thumbsUpBlend, (float)(this._isGivingThumbsUp ? 1 : 0), this._animPointAndThumbsUpGain * this._deltaTimeProvider());
			this.UpdateAnimStates();
		}

		private void UpdateCapTouchStates()
		{
			float indexCurl = OVRControllerUtility.GetIndexCurl(this._controller);
			float indexSlide = OVRControllerUtility.GetIndexSlide(this._controller);
			this._pointTarget = 1f - indexCurl;
			this._slideTarget = indexSlide;
			bool flag = this._allowThumbUp == AnimatedHandOVR.AllowThumbUp.Always || (this._allowThumbUp == AnimatedHandOVR.AllowThumbUp.GripRequired && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this._controller) >= 0.95f) || (this._allowThumbUp == AnimatedHandOVR.AllowThumbUp.TriggerAndGripRequired && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this._controller) >= 0.95f && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this._controller) >= 0.95f);
			this._isGivingThumbsUp = (flag && !OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, this._controller));
		}

		private void UpdateAnimStates()
		{
			float b = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this._controller);
			this._animFlex = Mathf.Lerp(this._animFlex, b, this._animFlexGain * this.DeltaTimeProvider());
			this._animator.SetFloat(this._animParamIndexFlex, this._animFlex);
			float b2 = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this._controller);
			this._animPinch = Mathf.Lerp(this._animPinch, b2, this._animPinchGain * this.DeltaTimeProvider());
			this._animator.SetFloat(this._animParamPinch, this._animPinch);
			this._animator.SetLayerWeight(this._animLayerIndexPoint, this._pointBlend);
			this._animator.SetFloat(this._animParamIndexSlide, this._slideBlend);
			this._animator.SetLayerWeight(this._animLayerIndexThumb, this._thumbsUpBlend);
		}

		public void InjectAllAnimatedHandOVR(OVRInput.Controller controller, Animator animator)
		{
			this.InjectController(controller);
			this.InjectAnimator(animator);
		}

		public void InjectController(OVRInput.Controller controller)
		{
			this._controller = controller;
		}

		public void InjectAnimator(Animator animator)
		{
			this._animator = animator;
		}

		[SerializeField]
		private OVRInput.Controller _controller;

		[SerializeField]
		private Animator _animator;

		[SerializeField]
		[Tooltip("Indicates the input needed in order to perform a thumbs-up when the fist is closed")]
		private AnimatedHandOVR.AllowThumbUp _allowThumbUp = AnimatedHandOVR.AllowThumbUp.TriggerAndGripRequired;

		[Header("Animation Speed")]
		[SerializeField]
		[FormerlySerializedAs("_animFlexhGain")]
		[Tooltip("Speed of the index flex animation")]
		private float _animFlexGain = 35f;

		[SerializeField]
		[Tooltip("Speed of the pinch animation")]
		private float _animPinchGain = 35f;

		[SerializeField]
		[Tooltip("Speed of the point, slide and thumbs up animation")]
		private float _animPointAndThumbsUpGain = 20f;

		private const string ANIM_LAYER_NAME_POINT = "Point Layer";

		private const string ANIM_LAYER_NAME_THUMB = "Thumb Layer";

		private const string ANIM_PARAM_NAME_FLEX = "Flex";

		private const string ANIM_PARAM_NAME_PINCH = "Pinch";

		private const string ANIM_PARAM_NAME_INDEX_SLIDE = "IndexSlide";

		private const float TRIGGER_MAX = 0.95f;

		private int _animLayerIndexThumb = -1;

		private int _animLayerIndexPoint = -1;

		private int _animParamIndexFlex = Animator.StringToHash("Flex");

		private int _animParamPinch = Animator.StringToHash("Pinch");

		private int _animParamIndexSlide = Animator.StringToHash("IndexSlide");

		private bool _isGivingThumbsUp;

		private float _pointBlend;

		private float _slideBlend;

		private float _thumbsUpBlend;

		private float _pointTarget;

		private float _slideTarget;

		private float _animFlex;

		private float _animPinch;

		private Func<float> _deltaTimeProvider = () => Time.deltaTime;

		public enum AllowThumbUp
		{
			Always,
			GripRequired,
			TriggerAndGripRequired
		}
	}
}
