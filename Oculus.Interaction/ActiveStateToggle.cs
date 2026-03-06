using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oculus.Interaction.PoseDetection.Debug;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ActiveStateToggle : MonoBehaviour, IActiveState
	{
		public ActiveStateToggle.StatePrecedence Precedence
		{
			get
			{
				return this._precedence;
			}
			set
			{
				this._precedence = value;
			}
		}

		protected virtual void Awake()
		{
			this.On = (this._on as IActiveState);
			this.Off = (this._off as IActiveState);
		}

		protected virtual void Start()
		{
		}

		public bool Active
		{
			get
			{
				if (this.Precedence == ActiveStateToggle.StatePrecedence.Off)
				{
					if (this.Off.Active)
					{
						this._internalActive = false;
					}
					else if (this.On.Active)
					{
						this._internalActive = true;
					}
				}
				else if (this.Precedence == ActiveStateToggle.StatePrecedence.On)
				{
					if (this.On.Active)
					{
						this._internalActive = true;
					}
					else if (this.Off.Active)
					{
						this._internalActive = false;
					}
				}
				return this._internalActive && base.isActiveAndEnabled;
			}
		}

		public void InjectAllActiveStateToggle(IActiveState on, IActiveState off)
		{
			this.InjectOn(on);
			this.InjectOff(off);
		}

		public void InjectOn(IActiveState activeState)
		{
			this._on = (activeState as Object);
			this.On = activeState;
		}

		public void InjectOff(IActiveState activeState)
		{
			this._off = (activeState as Object);
			this.Off = activeState;
		}

		[Tooltip("When this ActiveState is Active, the ActiveStateToggle will be Active.")]
		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _on;

		private IActiveState On;

		[Tooltip("When this ActiveState is Inactive, the ActiveStateToggle will be Inactive.")]
		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _off;

		private IActiveState Off;

		[Tooltip("If both On and Off conditions are Active simultaneously, this condition will take precedence and dictate the output state.")]
		[SerializeField]
		private ActiveStateToggle.StatePrecedence _precedence;

		private bool _internalActive;

		public enum StatePrecedence
		{
			On,
			Off
		}

		private class DebugModel : ActiveStateModel<ActiveStateToggle>
		{
			protected override Task<IEnumerable<IActiveState>> GetChildrenAsync(ActiveStateToggle activeState)
			{
				return Task.FromResult<IEnumerable<IActiveState>>(new IActiveState[]
				{
					activeState.On,
					activeState.Off
				});
			}
		}
	}
}
