using System;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction.Input.Visuals
{
	[Obsolete("Use ControllerVisual instead.")]
	[Feature(Feature.Interaction)]
	public class OVRControllerVisual : MonoBehaviour
	{
		public bool ForceOffVisibility { get; set; }

		protected virtual void Awake()
		{
			this.Controller = (this._controller as IController);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			Handedness handedness = this.Controller.Handedness;
			if (handedness != Handedness.Left)
			{
				if (handedness == Handedness.Right)
				{
					this._ovrControllerHelper.m_controller = OVRInput.Controller.RTouch;
				}
			}
			else
			{
				this._ovrControllerHelper.m_controller = OVRInput.Controller.LTouch;
			}
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Controller.WhenUpdated += this.HandleUpdated;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started && this._controller != null)
			{
				this.Controller.WhenUpdated -= this.HandleUpdated;
			}
		}

		private void HandleUpdated()
		{
			Pose pose;
			if (!this.Controller.IsConnected || this.ForceOffVisibility || !this.Controller.TryGetPose(out pose))
			{
				this._ovrControllerHelper.gameObject.SetActive(false);
				return;
			}
			this._ovrControllerHelper.gameObject.SetActive(true);
			base.transform.position = pose.position;
			base.transform.rotation = pose.rotation;
			float num = (base.transform.parent != null) ? base.transform.parent.lossyScale.x : 1f;
			base.transform.localScale = this.Controller.Scale / num * Vector3.one;
		}

		public void InjectAllOVRControllerVisual(IController controller, OVRControllerHelper ovrControllerHelper)
		{
			this.InjectController(controller);
			this.InjectAllOVRControllerHelper(ovrControllerHelper);
		}

		public void InjectController(IController controller)
		{
			this._controller = (controller as Object);
			this.Controller = controller;
		}

		public void InjectAllOVRControllerHelper(OVRControllerHelper ovrControllerHelper)
		{
			this._ovrControllerHelper = ovrControllerHelper;
		}

		[SerializeField]
		[Interface(typeof(IController), new Type[]
		{

		})]
		private Object _controller;

		public IController Controller;

		[SerializeField]
		private OVRControllerHelper _ovrControllerHelper;

		protected bool _started;
	}
}
