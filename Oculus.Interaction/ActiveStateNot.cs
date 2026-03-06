using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oculus.Interaction.PoseDetection.Debug;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ActiveStateNot : MonoBehaviour, IActiveState
	{
		protected virtual void Awake()
		{
			this.ActiveState = (this._activeState as IActiveState);
		}

		protected virtual void Start()
		{
		}

		public bool Active
		{
			get
			{
				return !this.ActiveState.Active;
			}
		}

		public void InjectAllActiveStateNot(IActiveState activeState)
		{
			this.InjectActiveState(activeState);
		}

		public void InjectActiveState(IActiveState activeState)
		{
			this._activeState = (activeState as Object);
			this.ActiveState = activeState;
		}

		[Tooltip("The IActiveState that the NOT operation will be applied to.")]
		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _activeState;

		private IActiveState ActiveState;

		private class DebugModel : ActiveStateModel<ActiveStateNot>
		{
			protected override Task<IEnumerable<IActiveState>> GetChildrenAsync(ActiveStateNot activeState)
			{
				return Task.FromResult<IEnumerable<IActiveState>>(new IActiveState[]
				{
					activeState.ActiveState
				});
			}
		}
	}
}
