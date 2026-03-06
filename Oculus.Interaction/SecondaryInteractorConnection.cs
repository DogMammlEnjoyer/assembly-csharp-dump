using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class SecondaryInteractorConnection : MonoBehaviour
	{
		public IInteractorView PrimaryInteractor { get; private set; }

		public IInteractorView SecondaryInteractor { get; private set; }

		protected virtual void Awake()
		{
			this.PrimaryInteractor = (this._primaryInteractor as IInteractorView);
			this.SecondaryInteractor = (this._secondaryInteractor as IInteractorView);
		}

		protected virtual void Start()
		{
		}

		public void InjectAllSecondaryInteractorConnection(IInteractorView primaryInteractor, IInteractorView secondaryInteractor)
		{
			this.InjectPrimaryInteractor(primaryInteractor);
			this.InjectSecondaryInteractorConnection(secondaryInteractor);
		}

		public void InjectPrimaryInteractor(IInteractorView interactorView)
		{
			this.PrimaryInteractor = interactorView;
			this._primaryInteractor = (interactorView as Object);
		}

		public void InjectSecondaryInteractorConnection(IInteractorView interactorView)
		{
			this.SecondaryInteractor = interactorView;
			this._secondaryInteractor = (interactorView as Object);
		}

		[SerializeField]
		[Interface(typeof(IInteractorView), new Type[]
		{

		})]
		private Object _primaryInteractor;

		[SerializeField]
		[Interface(typeof(IInteractorView), new Type[]
		{

		})]
		private Object _secondaryInteractor;
	}
}
