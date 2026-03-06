using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class RayInteractorDebugGizmos : MonoBehaviour
	{
		public float RayWidth
		{
			get
			{
				return this._rayWidth;
			}
			set
			{
				this._rayWidth = value;
			}
		}

		public Color NormalColor
		{
			get
			{
				return this._normalColor;
			}
			set
			{
				this._normalColor = value;
			}
		}

		public Color HoverColor
		{
			get
			{
				return this._hoverColor;
			}
			set
			{
				this._hoverColor = value;
			}
		}

		public Color SelectColor
		{
			get
			{
				return this._selectColor;
			}
			set
			{
				this._selectColor = value;
			}
		}

		protected virtual void Start()
		{
		}

		private void LateUpdate()
		{
			if (this._rayInteractor.State == InteractorState.Disabled)
			{
				return;
			}
			switch (this._rayInteractor.State)
			{
			case InteractorState.Normal:
				DebugGizmos.Color = this._normalColor;
				break;
			case InteractorState.Hover:
				DebugGizmos.Color = this._hoverColor;
				break;
			case InteractorState.Select:
				DebugGizmos.Color = this._selectColor;
				break;
			case InteractorState.Disabled:
				return;
			}
			DebugGizmos.LineWidth = this._rayWidth;
			DebugGizmos.DrawLine(this._rayInteractor.Origin, this._rayInteractor.End, null);
		}

		public void InjectAllRayInteractorDebugGizmos(RayInteractor rayInteractor)
		{
			this.InjectRayInteractor(rayInteractor);
		}

		public void InjectRayInteractor(RayInteractor rayInteractor)
		{
			this._rayInteractor = rayInteractor;
		}

		[SerializeField]
		private RayInteractor _rayInteractor;

		[SerializeField]
		private float _rayWidth = 0.01f;

		[SerializeField]
		private Color _normalColor = Color.red;

		[SerializeField]
		private Color _hoverColor = Color.blue;

		[SerializeField]
		private Color _selectColor = Color.green;
	}
}
