using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class LocomotionGate : MonoBehaviour
	{
		public IHand Hand { get; private set; }

		private IActiveState EnableShape { get; set; }

		private IActiveState DisableShape { get; set; }

		public LocomotionGate.LocomotionMode ActiveMode
		{
			get
			{
				return this._activeMode;
			}
			private set
			{
				if (this._activeMode != value)
				{
					LocomotionGate.LocomotionMode activeMode = this._activeMode;
					this._activeMode = value;
					this._teleportState.Active = (this._activeMode == LocomotionGate.LocomotionMode.Teleport);
					this._turningState.Active = (this._activeMode == LocomotionGate.LocomotionMode.Turn);
					this._whenActiveModeChanged(new LocomotionGate.LocomotionModeEventArgs(activeMode, this._activeMode));
				}
			}
		}

		public float CurrentAngle { get; private set; }

		public Vector3 WristDirection { get; private set; }

		public Pose StabilizationPose { get; private set; } = Pose.identity;

		public event Action<LocomotionGate.LocomotionModeEventArgs> WhenActiveModeChanged
		{
			add
			{
				this._whenActiveModeChanged = (Action<LocomotionGate.LocomotionModeEventArgs>)Delegate.Combine(this._whenActiveModeChanged, value);
			}
			remove
			{
				this._whenActiveModeChanged = (Action<LocomotionGate.LocomotionModeEventArgs>)Delegate.Remove(this._whenActiveModeChanged, value);
			}
		}

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
			this.EnableShape = (this._enableShape as IActiveState);
			this.DisableShape = (this._disableShape as IActiveState);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated += this.HandleHandupdated;
				this.Disable();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated -= this.HandleHandupdated;
				this.Disable();
			}
		}

		public void Disable()
		{
			this.ActiveMode = LocomotionGate.LocomotionMode.None;
			this._currentGateIndex = -1;
			this._cancelled = false;
		}

		public void Cancel()
		{
			this.ActiveMode = LocomotionGate.LocomotionMode.None;
			if (this._currentGateIndex >= 0)
			{
				this._cancelled = true;
			}
		}

		private void HandleHandupdated()
		{
			Pose pose;
			if (!this.Hand.GetJointPose(HandJointId.HandWristRoot, out pose))
			{
				this.Disable();
				return;
			}
			bool flag = this.Hand.Handedness == Handedness.Right;
			Vector3 up = Vector3.up;
			Vector3 normalized = (pose.position - this._shoulder.position).normalized;
			Vector3 vector = Vector3.Cross(up, normalized).normalized;
			vector = (flag ? vector : (-vector));
			Vector3 vector2 = pose.rotation * (flag ? Constants.RightThumbSide : Constants.LeftThumbSide);
			bool flag2 = (double)Vector3.Dot(pose.rotation * (flag ? Constants.RightDistal : Constants.LeftDistal), Vector3.ProjectOnPlane(normalized, up).normalized) * 0.5 + 0.5 > 0.5;
			vector2 = Vector3.ProjectOnPlane(vector2, normalized).normalized;
			float num = Vector3.SignedAngle(vector2, vector, normalized);
			num = ((this.Hand.Handedness == Handedness.Right) ? (-num) : num);
			if (num < -70f)
			{
				num += 360f;
			}
			this.CurrentAngle = num;
			this.StabilizationPose = new Pose(this._shoulder.position, Quaternion.LookRotation(normalized));
			this.WristDirection = vector2;
			bool flag3 = false;
			if (this.EnableShape.Active && !this._previousShapeEnabled)
			{
				flag3 = true;
			}
			this._previousShapeEnabled = this.EnableShape.Active;
			if (this._currentGateIndex < 0 && flag3 && flag2)
			{
				LocomotionGate.GateSection bestGateSection = this.GetBestGateSection(this.CurrentAngle, out this._currentGateIndex);
				if (bestGateSection.canEnterDirectly)
				{
					this.ActiveMode = bestGateSection.locomotionMode;
					return;
				}
				this._currentGateIndex = -1;
				return;
			}
			else
			{
				if (this._currentGateIndex >= 0 && this.DisableShape.Active)
				{
					this.Disable();
					return;
				}
				if (this._currentGateIndex < 0 || this._cancelled)
				{
					return;
				}
				LocomotionGate.GateSection gateSection = this._gateSections[this._currentGateIndex];
				if (this.CurrentAngle < gateSection.minAngle)
				{
					this._currentGateIndex = Mathf.Max(0, this._currentGateIndex - 1);
					this.ActiveMode = this._gateSections[this._currentGateIndex].locomotionMode;
					return;
				}
				if (this.CurrentAngle > gateSection.maxAngle)
				{
					this._currentGateIndex = Mathf.Min(this._gateSections.Length - 1, this._currentGateIndex + 1);
					this.ActiveMode = this._gateSections[this._currentGateIndex].locomotionMode;
				}
				return;
			}
		}

		private LocomotionGate.GateSection GetBestGateSection(float angle, out int index)
		{
			float num = float.PositiveInfinity;
			index = -1;
			for (int i = 0; i < this._gateSections.Length; i++)
			{
				float num2 = this._gateSections[i].ScoreToAngle(angle);
				if (num2 < num)
				{
					num = num2;
					index = i;
				}
			}
			if (index == -1)
			{
				return LocomotionGate.DefaultSection;
			}
			return this._gateSections[index];
		}

		public void InjectAllLocomotionGate(IHand hand, Transform shoulder, IActiveState enableShape, IActiveState disableShape, VirtualActiveState turningState, VirtualActiveState teleportState, LocomotionGate.GateSection[] gateSections)
		{
			this.InjectHand(hand);
			this.InjectShoulder(shoulder);
			this.InjectEnableShape(enableShape);
			this.InjectDisableShape(disableShape);
			this.InjectTurningState(turningState);
			this.InjectTeleportState(teleportState);
			this.InjectGateSections(gateSections);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectShoulder(Transform shoulder)
		{
			this._shoulder = shoulder;
		}

		public void InjectEnableShape(IActiveState enableShape)
		{
			this._enableShape = (enableShape as Object);
			this.EnableShape = enableShape;
		}

		public void InjectDisableShape(IActiveState disableShape)
		{
			this._disableShape = (disableShape as Object);
			this.DisableShape = disableShape;
		}

		public void InjectTurningState(VirtualActiveState turningState)
		{
			this._turningState = turningState;
		}

		public void InjectTeleportState(VirtualActiveState teleportState)
		{
			this._teleportState = teleportState;
		}

		public void InjectGateSections(LocomotionGate.GateSection[] gateSections)
		{
			this._gateSections = gateSections;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		private Transform _shoulder;

		[SerializeField]
		private LocomotionGate.GateSection[] _gateSections = new LocomotionGate.GateSection[]
		{
			new LocomotionGate.GateSection
			{
				locomotionMode = LocomotionGate.LocomotionMode.Teleport,
				maxAngle = 95f
			},
			new LocomotionGate.GateSection
			{
				locomotionMode = LocomotionGate.LocomotionMode.Turn,
				minAngle = 40f,
				maxAngle = 165f
			},
			new LocomotionGate.GateSection
			{
				locomotionMode = LocomotionGate.LocomotionMode.Teleport,
				minAngle = 120f,
				canEnterDirectly = false
			}
		};

		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _enableShape;

		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _disableShape;

		[SerializeField]
		private VirtualActiveState _turningState;

		[SerializeField]
		private VirtualActiveState _teleportState;

		protected bool _started;

		private bool _previousShapeEnabled;

		private int _currentGateIndex = -1;

		private LocomotionGate.LocomotionMode _activeMode;

		private Action<LocomotionGate.LocomotionModeEventArgs> _whenActiveModeChanged = delegate(LocomotionGate.LocomotionModeEventArgs <p0>)
		{
		};

		private static readonly LocomotionGate.GateSection DefaultSection = new LocomotionGate.GateSection
		{
			locomotionMode = LocomotionGate.LocomotionMode.None
		};

		private const float _enterPoseThreshold = 0.5f;

		private const float _wristLimit = -70f;

		private bool _cancelled;

		public enum LocomotionMode
		{
			None,
			Teleport,
			Turn
		}

		[Serializable]
		public class GateSection
		{
			public float ScoreToAngle(float angle)
			{
				float num = Mathf.Repeat(angle - this.minAngle, 360f);
				float num2 = Mathf.Repeat(this.maxAngle - this.minAngle, 360f);
				if (num > num2)
				{
					return float.PositiveInfinity;
				}
				float target = (this.minAngle + this.maxAngle) / 2f;
				return Mathf.Repeat(Mathf.DeltaAngle(angle, target), 360f);
			}

			public float minAngle = -70f;

			public float maxAngle = 290f;

			public bool canEnterDirectly = true;

			public LocomotionGate.LocomotionMode locomotionMode;
		}

		public struct LocomotionModeEventArgs
		{
			public readonly LocomotionGate.LocomotionMode PreviousMode { get; }

			public readonly LocomotionGate.LocomotionMode NewMode { get; }

			public LocomotionModeEventArgs(LocomotionGate.LocomotionMode previousMode, LocomotionGate.LocomotionMode newMode)
			{
				this.PreviousMode = previousMode;
				this.NewMode = newMode;
			}
		}
	}
}
