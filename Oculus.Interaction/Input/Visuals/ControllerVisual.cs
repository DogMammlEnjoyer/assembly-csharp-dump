using System;
using UnityEngine;

namespace Oculus.Interaction.Input.Visuals
{
	public class ControllerVisual : MonoBehaviour
	{
		public IController Controller { get; private set; }

		public bool ForceOffVisibility { get; set; }

		protected virtual void Awake()
		{
			if (this.Controller == null)
			{
				this.Controller = (this._controller as IController);
			}
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
				this._root.SetActive(false);
				return;
			}
			this._root.SetActive(true);
			this._root.transform.position = pose.position;
			this._root.transform.rotation = pose.rotation;
			float num = (this._root.transform.parent != null) ? this._root.transform.parent.lossyScale.x : 1f;
			this._root.transform.localScale = this.Controller.Scale / num * Vector3.one;
		}

		public void InjectAllOVRControllerVisual(IController controller, GameObject root)
		{
			this.InjectController(controller);
			this.InjectRoot(root);
		}

		public void InjectController(IController controller)
		{
			this._controller = (controller as Object);
			this.Controller = controller;
		}

		public void InjectRoot(GameObject root)
		{
			this._root = root;
		}

		[SerializeField]
		[Interface(typeof(IController), new Type[]
		{

		})]
		private Object _controller;

		[SerializeField]
		private GameObject _root;

		private bool _started;
	}
}
