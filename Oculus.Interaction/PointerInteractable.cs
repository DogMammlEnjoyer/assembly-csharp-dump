using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public abstract class PointerInteractable<TInteractor, TInteractable> : Interactable<TInteractor, TInteractable>, IPointable where TInteractor : Interactor<TInteractor, TInteractable> where TInteractable : PointerInteractable<TInteractor, TInteractable>
	{
		public IPointableElement PointableElement { get; protected set; }

		public event Action<PointerEvent> WhenPointerEventRaised = delegate(PointerEvent <p0>)
		{
		};

		public void PublishPointerEvent(PointerEvent evt)
		{
			if (this.PointableElement != null)
			{
				this.PointableElement.ProcessPointerEvent(evt);
			}
			this.WhenPointerEventRaised(evt);
		}

		protected override void Awake()
		{
			base.Awake();
			this.PointableElement = (this._pointableElement as IPointableElement);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this._pointableElement != null;
			this.EndStart(ref this._started);
		}

		public void InjectOptionalPointableElement(IPointableElement pointableElement)
		{
			this.PointableElement = pointableElement;
			this._pointableElement = (pointableElement as Object);
		}

		[SerializeField]
		[Interface(typeof(IPointableElement), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.DontHide)]
		private Object _pointableElement;
	}
}
