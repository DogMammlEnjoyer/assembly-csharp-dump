using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ControllerOffset : MonoBehaviour
	{
		public IController Controller { get; private set; }

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
			if (this.Controller.TryGetPose(out pose))
			{
				Pose pose2 = new Pose(this.Controller.Scale * this._offset, this._rotation);
				ref pose2.Postmultiply(pose);
				base.transform.SetPose(pose2, Space.World);
			}
		}

		public void GetOffset(ref Pose pose)
		{
			pose.position = this.Controller.Scale * this._offset;
			pose.rotation = this._rotation;
		}

		public void GetWorldPose(ref Pose pose)
		{
			pose.position = base.transform.position;
			pose.rotation = base.transform.rotation;
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

		public void InjectRotation(Quaternion rotation)
		{
			this._rotation = rotation;
		}

		public void InjectAllControllerOffset(IController controller, Vector3 offset, Quaternion rotation)
		{
			this.InjectController(controller);
			this.InjectOffset(offset);
			this.InjectRotation(rotation);
		}

		[SerializeField]
		[Interface(typeof(IController), new Type[]
		{

		})]
		private Object _controller;

		[SerializeField]
		private Vector3 _offset;

		[SerializeField]
		private Quaternion _rotation = Quaternion.identity;

		protected bool _started;
	}
}
