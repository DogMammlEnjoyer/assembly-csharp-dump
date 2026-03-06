using System;
using UnityEngine;

namespace Oculus.Interaction.DistanceReticles
{
	public class ReticleIconDrawer : InteractorReticle<ReticleDataIcon>
	{
		private IDistanceInteractor DistanceInteractor { get; set; }

		public Texture DefaultIcon
		{
			get
			{
				return this._defaultIcon;
			}
			set
			{
				this._defaultIcon = value;
			}
		}

		public bool ConstantScreenSize
		{
			get
			{
				return this._constantScreenSize;
			}
			set
			{
				this._constantScreenSize = value;
			}
		}

		protected override IInteractorView Interactor { get; set; }

		protected override Component InteractableComponent
		{
			get
			{
				return this.DistanceInteractor.DistanceInteractable as Component;
			}
		}

		protected virtual void OnValidate()
		{
			if (this._renderer != null)
			{
				this._renderer.sharedMaterial.mainTexture = this._defaultIcon;
			}
		}

		protected virtual void Awake()
		{
			this.DistanceInteractor = (this._distanceInteractor as IDistanceInteractor);
			this.Interactor = this.DistanceInteractor;
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this._originalScale = base.transform.localScale;
			this.EndStart(ref this._started);
		}

		protected override void Draw(ReticleDataIcon dataIcon)
		{
			if (dataIcon != null && dataIcon.CustomIcon != null)
			{
				this._renderer.material.mainTexture = dataIcon.CustomIcon;
			}
			else
			{
				this._renderer.material.mainTexture = this._defaultIcon;
			}
			if (!this._constantScreenSize)
			{
				this._renderer.transform.localScale = this._originalScale * dataIcon.GetTargetSize().magnitude;
			}
			this._renderer.enabled = true;
		}

		protected override void Align(ReticleDataIcon data)
		{
			base.transform.position = data.ProcessHitPoint(this.DistanceInteractor.HitPoint);
			if (this._renderer.enabled)
			{
				Vector3 normalized = (this._centerEye.position - base.transform.position).normalized;
				base.transform.LookAt(base.transform.position - normalized, Vector3.up);
				if (this._constantScreenSize)
				{
					float d = Vector3.Distance(base.transform.position, this._centerEye.position);
					this._renderer.transform.localScale = this._originalScale * d;
				}
			}
		}

		protected override void Hide()
		{
			this._renderer.enabled = false;
		}

		public void InjectAllReticleIconDrawer(IDistanceInteractor distanceInteractor, Transform centerEye, MeshRenderer renderer)
		{
			this.InjectDistanceInteractor(distanceInteractor);
			this.InjectCenterEye(centerEye);
			this.InjectRenderer(renderer);
		}

		public void InjectDistanceInteractor(IDistanceInteractor distanceInteractor)
		{
			this._distanceInteractor = (distanceInteractor as Object);
			this.DistanceInteractor = distanceInteractor;
			this.Interactor = distanceInteractor;
		}

		public void InjectCenterEye(Transform centerEye)
		{
			this._centerEye = centerEye;
		}

		public void InjectRenderer(MeshRenderer renderer)
		{
			this._renderer = renderer;
		}

		[SerializeField]
		[Interface(typeof(IDistanceInteractor), new Type[]
		{

		})]
		private Object _distanceInteractor;

		[SerializeField]
		private MeshRenderer _renderer;

		[SerializeField]
		private Transform _centerEye;

		[SerializeField]
		private Texture _defaultIcon;

		[SerializeField]
		private bool _constantScreenSize;

		private Vector3 _originalScale;
	}
}
