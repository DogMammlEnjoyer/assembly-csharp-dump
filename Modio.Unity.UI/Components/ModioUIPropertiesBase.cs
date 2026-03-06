using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Components
{
	public abstract class ModioUIPropertiesBase<TOwner, TProperty> : MonoBehaviour where TOwner : Component, IModioUIPropertiesOwner
	{
		protected abstract TProperty[] Properties { get; }

		protected virtual void Awake()
		{
			this.Owner = base.GetComponentInParent<TOwner>();
			if (this.Owner != null)
			{
				this.Owner.AddUpdatePropertiesListener(new UnityAction(this.UpdateProperties));
				this._monoBehaviourEvents = (this.Properties.Any((TProperty property) => property is IPropertyMonoBehaviourEvents) ? this.Properties.OfType<IPropertyMonoBehaviourEvents>().ToArray<IPropertyMonoBehaviourEvents>() : Array.Empty<IPropertyMonoBehaviourEvents>());
				return;
			}
			Debug.LogWarning(base.GetType().Name + " " + base.gameObject.name + " could not find a TOwner, disabling.", this);
			base.enabled = false;
		}

		protected virtual void Start()
		{
			IPropertyMonoBehaviourEvents[] monoBehaviourEvents = this._monoBehaviourEvents;
			for (int i = 0; i < monoBehaviourEvents.Length; i++)
			{
				monoBehaviourEvents[i].Start();
			}
			this.UpdateProperties();
		}

		protected void OnDestroy()
		{
			if (this.Owner)
			{
				this.Owner.RemoveUpdatePropertiesListener(new UnityAction(this.UpdateProperties));
			}
			IPropertyMonoBehaviourEvents[] monoBehaviourEvents = this._monoBehaviourEvents;
			for (int i = 0; i < monoBehaviourEvents.Length; i++)
			{
				monoBehaviourEvents[i].OnDestroy();
			}
		}

		private void OnEnable()
		{
			IPropertyMonoBehaviourEvents[] monoBehaviourEvents = this._monoBehaviourEvents;
			for (int i = 0; i < monoBehaviourEvents.Length; i++)
			{
				monoBehaviourEvents[i].OnEnable();
			}
		}

		private void OnDisable()
		{
			IPropertyMonoBehaviourEvents[] monoBehaviourEvents = this._monoBehaviourEvents;
			for (int i = 0; i < monoBehaviourEvents.Length; i++)
			{
				monoBehaviourEvents[i].OnDisable();
			}
		}

		protected abstract void UpdateProperties();

		protected TOwner Owner;

		private IPropertyMonoBehaviourEvents[] _monoBehaviourEvents;
	}
}
