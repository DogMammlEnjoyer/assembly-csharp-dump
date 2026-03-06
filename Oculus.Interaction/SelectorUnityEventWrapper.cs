using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction
{
	public class SelectorUnityEventWrapper : MonoBehaviour
	{
		public UnityEvent WhenSelected
		{
			get
			{
				return this._whenSelected;
			}
		}

		public UnityEvent WhenUnselected
		{
			get
			{
				return this._whenUnselected;
			}
		}

		protected virtual void Awake()
		{
			this.Selector = (this._selector as ISelector);
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
				this.Selector.WhenSelected += this.HandleSelected;
				this.Selector.WhenUnselected += this.HandleUnselected;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Selector.WhenSelected -= this.HandleSelected;
				this.Selector.WhenUnselected -= this.HandleUnselected;
			}
		}

		private void HandleSelected()
		{
			this._whenSelected.Invoke();
		}

		private void HandleUnselected()
		{
			this._whenUnselected.Invoke();
		}

		public void InjectAllSelectorUnityEventWrapper(ISelector selector)
		{
			this.InjectSelector(selector);
		}

		public void InjectSelector(ISelector selector)
		{
			this._selector = (selector as Object);
			this.Selector = selector;
		}

		[SerializeField]
		[Interface(typeof(ISelector), new Type[]
		{

		})]
		private Object _selector;

		private ISelector Selector;

		[SerializeField]
		private UnityEvent _whenSelected;

		[SerializeField]
		private UnityEvent _whenUnselected;

		protected bool _started;
	}
}
