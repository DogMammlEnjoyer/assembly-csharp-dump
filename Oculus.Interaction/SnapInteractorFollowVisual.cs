using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class SnapInteractorFollowVisual : MonoBehaviour
	{
		public float HoverOffset
		{
			get
			{
				return this._hoverOffset;
			}
			set
			{
				this._hoverOffset = value;
			}
		}

		public ProgressCurve EaseCurve
		{
			get
			{
				return this._easeCurve;
			}
			set
			{
				this._easeCurve = value;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			if (this._transform == null)
			{
				this._transform = base.transform;
			}
			this._from = (this._to = this.ComputeTargetPose());
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._snapInteractor.WhenStateChanged += this.HandleStateChanged;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._snapInteractor.WhenStateChanged -= this.HandleStateChanged;
			}
		}

		private void HandleStateChanged(InteractorStateChangeArgs args)
		{
			this._from = base.transform.GetPose(Space.World);
			this._to = this.ComputeTargetPose();
			this._easeCurve.Start();
		}

		protected virtual Pose ComputeTargetPose()
		{
			Pose result;
			if (this._snapInteractor.HasInteractable && this._snapInteractor.Interactable.PoseForInteractor(this._snapInteractor, out result))
			{
				if (this._snapInteractor.State == InteractorState.Hover)
				{
					result.position += this._hoverOffset * result.forward;
				}
				return result;
			}
			return this._snapInteractor.transform.GetPose(Space.World);
		}

		protected virtual void Update()
		{
			this._to = this.ComputeTargetPose();
			float t = this._easeCurve.Progress();
			Pose from = this._from;
			ref from.Lerp(this._to, t);
			this._transform.position = from.position;
			this._transform.rotation = from.rotation;
		}

		public void InjectAllSnapInteractorFollowVisual(SnapInteractor snapInteractor)
		{
			this._snapInteractor = snapInteractor;
		}

		public void InjectOptionalTransform(Transform transform)
		{
			this._transform = transform;
		}

		[SerializeField]
		private SnapInteractor _snapInteractor;

		[SerializeField]
		private float _hoverOffset;

		[SerializeField]
		private ProgressCurve _easeCurve = new ProgressCurve(AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 0.1f);

		[SerializeField]
		[Optional]
		private Transform _transform;

		protected bool _started;

		private Pose _from;

		private Pose _to;
	}
}
