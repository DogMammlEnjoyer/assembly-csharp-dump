using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class LocomotionActiveState : MonoBehaviour, IActiveState, ITimeConsumer
	{
		private ILocomotionEventBroadcaster LocomotionBroadcaster { get; set; }

		public float IdleTime
		{
			get
			{
				return this._idleTime;
			}
			set
			{
				this._idleTime = value;
			}
		}

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		public bool Active { get; private set; }

		protected void Awake()
		{
			if (this.LocomotionBroadcaster == null)
			{
				this.LocomotionBroadcaster = (this._locomotionBroadcaster as ILocomotionEventBroadcaster);
			}
		}

		protected void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected void OnEnable()
		{
			if (this._started)
			{
				this.LocomotionBroadcaster.WhenLocomotionPerformed += this.HandleLocomotionPerformed;
			}
		}

		protected void OnDisable()
		{
			if (this._started)
			{
				this.Active = false;
				this.LocomotionBroadcaster.WhenLocomotionPerformed -= this.HandleLocomotionPerformed;
			}
		}

		protected void Update()
		{
			if (this.Active && this._timeProvider() - this._lastEventTime > this._idleTime)
			{
				this.Active = false;
			}
		}

		private void HandleLocomotionPerformed(LocomotionEvent obj)
		{
			if (obj.Translation != LocomotionEvent.TranslationType.None || obj.Rotation != LocomotionEvent.RotationType.None)
			{
				this._lastEventTime = this._timeProvider();
				this.Active = true;
			}
		}

		[SerializeField]
		[Interface(typeof(ILocomotionEventBroadcaster), new Type[]
		{

		})]
		private Object _locomotionBroadcaster;

		[SerializeField]
		private float _idleTime = 0.1f;

		private Func<float> _timeProvider = () => Time.time;

		private float _lastEventTime;

		protected bool _started;
	}
}
