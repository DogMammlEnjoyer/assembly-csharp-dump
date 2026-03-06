using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ControllerPointerPose : MonoBehaviour, IActiveState
	{
		public IController Controller { get; private set; }

		public bool Active { get; private set; }

		protected virtual void Awake()
		{
			this.Controller = (this._controller as IController);
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
				this.Controller.WhenUpdated += this.HandleUpdated;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Controller.WhenUpdated -= this.HandleUpdated;
			}
		}

		private void HandleUpdated()
		{
			Pose pose;
			if (this.Controller.TryGetPointerPose(out pose))
			{
				pose.position += pose.rotation * (this.Controller.Scale * this._offset);
				base.transform.SetPose(pose, Space.World);
				this.Active = true;
				return;
			}
			this.Active = false;
		}

		public void InjectController(IController controller)
		{
			this._controller = (controller as Object);
			this.Controller = controller;
		}

		public void InjectOffset(Vector3 offset)
		{
			this._offset = offset;
		}

		public void InjectAllControllerPointerPose(IController controller, Vector3 offset)
		{
			this.InjectController(controller);
			this.InjectOffset(offset);
		}

		[Tooltip("A controller ray interactor.")]
		[SerializeField]
		[Interface(typeof(IController), new Type[]
		{

		})]
		private Object _controller;

		[Tooltip("How much the ray origin is offset relative to the controller.")]
		[SerializeField]
		private Vector3 _offset;

		protected bool _started;
	}
}
