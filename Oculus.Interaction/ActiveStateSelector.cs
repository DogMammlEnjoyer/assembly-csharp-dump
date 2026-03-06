using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ActiveStateSelector : MonoBehaviour, ISelector
	{
		private protected IActiveState ActiveState { protected get; private set; }

		public event Action WhenSelected = delegate()
		{
		};

		public event Action WhenUnselected = delegate()
		{
		};

		protected virtual void Awake()
		{
			this.ActiveState = (this._activeState as IActiveState);
		}

		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
			if (this._selecting != this.ActiveState.Active)
			{
				this._selecting = this.ActiveState.Active;
				if (this._selecting)
				{
					this.WhenSelected();
					return;
				}
				this.WhenUnselected();
			}
		}

		public void InjectAllActiveStateSelector(IActiveState activeState)
		{
			this.InjectActiveState(activeState);
		}

		public void InjectActiveState(IActiveState activeState)
		{
			this._activeState = (activeState as Object);
			this.ActiveState = activeState;
		}

		[Tooltip("ISelector events will be raised based on state changes of this IActiveState.")]
		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _activeState;

		private bool _selecting;
	}
}
