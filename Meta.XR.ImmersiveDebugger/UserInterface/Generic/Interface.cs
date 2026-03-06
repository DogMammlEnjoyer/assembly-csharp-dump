using System;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class Interface : Controller
	{
		internal Cursor Cursor { get; private set; }

		internal Camera Camera
		{
			get
			{
				return this._proxyCameraRig.Camera;
			}
		}

		protected virtual bool FollowOverride { get; set; }

		protected virtual bool RotateOverride { get; set; }

		internal virtual void Awake()
		{
			this.Setup(null);
			GameObject gameObject = new GameObject("cursor");
			gameObject.transform.SetParent(base.Transform);
			this.Cursor = gameObject.AddComponent<Cursor>();
			this._proxyCameraRig = new ProxyCameraRig();
			this._proxyInputModule = new ProxyInputModule(base.GameObject, this.Cursor);
		}

		private void UpdateTransform(bool updatePosition, bool updateRotation)
		{
			if (updatePosition)
			{
				Vector3 position = this._proxyCameraRig.CameraTransform.position;
				if (position != Vector3.zero)
				{
					this._positionHasBeenInitialized = true;
				}
				base.Transform.position = position;
			}
			if (updateRotation)
			{
				Vector3 eulerAngles = this._proxyCameraRig.CameraTransform.eulerAngles;
				eulerAngles.x = 0f;
				eulerAngles.z = 0f;
				base.Transform.rotation = Quaternion.Euler(eulerAngles);
			}
		}

		protected override void OnVisibilityChanged()
		{
			base.OnVisibilityChanged();
			if (base.Visibility && this._proxyCameraRig.Refresh())
			{
				this.UpdateTransform(true, true);
			}
		}

		private void UpdateCulling()
		{
			RuntimeSettings instance = RuntimeSettings.Instance;
			if (!instance.AutomaticLayerCullingUpdate)
			{
				return;
			}
			int cullingMask = this.Camera.cullingMask;
			int num = Interface.SetBits(cullingMask, instance.PanelLayer, instance.MeshRendererLayer, !RuntimeSettings.Instance.ShouldUseOverlay);
			if (num != cullingMask)
			{
				this.Camera.cullingMask = num;
			}
		}

		private static int SetBits(int cullingMask, int bitPosition1, int bitPosition2, bool state)
		{
			if (state)
			{
				cullingMask |= 1 << bitPosition1;
				cullingMask |= 1 << bitPosition2;
			}
			else
			{
				cullingMask &= ~(1 << bitPosition1);
				cullingMask &= ~(1 << bitPosition2);
			}
			return cullingMask;
		}

		internal virtual void LateUpdate()
		{
			base.UpdateRefreshLayout(false);
			if (!this._proxyCameraRig.Refresh())
			{
				return;
			}
			this.UpdateTransform(this.FollowOverride || !this._positionHasBeenInitialized, this.RotateOverride || !this._positionHasBeenInitialized);
			this.UpdateCulling();
			this._proxyInputModule.Refresh();
		}

		protected override void RefreshLayoutPreChildren()
		{
		}

		private ProxyInputModule _proxyInputModule;

		private ProxyCameraRig _proxyCameraRig;

		private bool _positionHasBeenInitialized;
	}
}
