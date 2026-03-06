using System;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class Panel : InteractableController
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			Panel._hapticsClip = null;
		}

		private static OVRHapticsClip HapticsClip
		{
			get
			{
				if (OVRHaptics.Config.SampleSizeInBytes == 0)
				{
					return null;
				}
				OVRHapticsClip result;
				if ((result = Panel._hapticsClip) == null)
				{
					result = (Panel._hapticsClip = new OVRHapticsClip(new byte[]
					{
						10,
						20,
						40,
						60,
						40
					}, 5));
				}
				return result;
			}
		}

		internal float PixelsPerUnit { get; private set; }

		internal bool Initialised { get; private set; }

		public Vector3 SphericalCoordinates
		{
			get
			{
				return this._sphericalCoordinates;
			}
			set
			{
				this._sphericalCoordinates = value;
				Vector3 position = Panel.SphericalToCartesian(this._sphericalCoordinates.x, this._sphericalCoordinates.y, this._sphericalCoordinates.z);
				this.SetPosition(position);
			}
		}

		internal Interface Interface
		{
			get
			{
				return base.Owner as Interface;
			}
		}

		public ImageStyle BackgroundStyle
		{
			set
			{
				this._backgroundStyle = value;
				this.Background.Sprite = value.sprite;
				this.Background.Color = value.color;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			base.Hide();
			this._canvas = base.GameObject.AddComponent<Canvas>();
			this._canvasScaler = base.GameObject.AddComponent<CanvasScaler>();
			this.Background = base.Append<Background>("background");
			this.Background.LayoutStyle = Style.Load<LayoutStyle>("Fill");
			this.Background.RaycastTarget = true;
			this.Initialised = true;
		}

		protected void SetExpectedPixelsPerUnit(float pixelsPerUnit, float dynamicPixelsPerUnit, float referencePixelsPerUnit)
		{
			this.PixelsPerUnit = pixelsPerUnit;
			this._canvasScaler.dynamicPixelsPerUnit = dynamicPixelsPerUnit;
			this._canvasScaler.referencePixelsPerUnit = referencePixelsPerUnit;
			base.Transform.localScale = Vector3.one / this.PixelsPerUnit;
		}

		private void SetPosition(Vector3 position)
		{
			base.Transform.localPosition = position;
			base.Transform.rotation = Quaternion.LookRotation(base.Transform.position - base.Owner.Transform.position, Vector3.up);
		}

		private static Vector3 SphericalToCartesian(float radius, float theta, float phi)
		{
			theta = 1.5707964f - theta;
			phi = 1.5707964f - phi;
			float x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
			float z = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
			float y = radius * Mathf.Cos(phi);
			return new Vector3(x, y, z);
		}

		protected override void OnTransparencyChanged()
		{
			base.OnTransparencyChanged();
			this.Background.Color = (base.Transparent ? this._backgroundStyle.colorOff : this._backgroundStyle.color);
		}

		protected override void OnHoverChanged()
		{
			base.OnHoverChanged();
			if (base.Hover)
			{
				base.PlayHaptics(Panel.HapticsClip);
				this.Interface.Cursor.Attach(this);
			}
		}

		private void RefreshCanvas()
		{
			if (this._canvas.worldCamera != this.Interface.Camera)
			{
				this._canvas.worldCamera = this.Interface.Camera;
			}
		}

		private void RefreshRaycaster()
		{
			if (this._ovrRaycaster)
			{
				return;
			}
			if (!this._canvas.worldCamera)
			{
				return;
			}
			this._ovrRaycaster = base.GameObject.AddComponent<PanelRaycaster>();
			this._ovrRaycaster.pointer = this.Interface.Cursor.GameObject;
		}

		private void LateUpdate()
		{
			this.RefreshCanvas();
			this.RefreshRaycaster();
		}

		protected virtual void OnEnable()
		{
			Telemetry.OnPanelActiveStateChanged(this);
		}

		protected override void OnDisable()
		{
			Telemetry.OnPanelActiveStateChanged(this);
			base.OnDisable();
		}

		private static OVRHapticsClip _hapticsClip;

		protected Canvas _canvas;

		private CanvasScaler _canvasScaler;

		private PanelRaycaster _ovrRaycaster;

		protected Background Background;

		protected ImageStyle _backgroundStyle;

		private Vector3 _sphericalCoordinates = new Vector3(1f, 0f, 0f);
	}
}
