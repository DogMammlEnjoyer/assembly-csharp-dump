using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Oculus.Interaction.Locomotion
{
	public class MenuToggler : MonoBehaviour
	{
		public Transform HeadAnchor
		{
			get
			{
				return this._headAnchor;
			}
			set
			{
				this._headAnchor = value;
			}
		}

		public Vector3 SpawnOffset
		{
			get
			{
				return this._spawnOffset;
			}
			set
			{
				this._spawnOffset = value;
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
				if (this._closeButton != null)
				{
					this._closeButton.onClick.AddListener(new UnityAction(this.HidePanel));
				}
				if (!this._panel.activeSelf)
				{
					this.HidePanel();
				}
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started && this._closeButton != null)
			{
				this._closeButton.onClick.RemoveListener(new UnityAction(this.HidePanel));
			}
		}

		public void TogglePanel()
		{
			if (this._panel.activeSelf)
			{
				this.HidePanel();
				return;
			}
			this.ShowPanel();
		}

		public void HidePanel()
		{
			this._panel.SetActive(false);
		}

		public void ShowPanel()
		{
			Quaternion quaternion = Quaternion.LookRotation(Vector3.ProjectOnPlane(this._headAnchor.forward, Vector3.up).normalized);
			Vector3 position = this._headAnchor.position + quaternion * this._spawnOffset;
			quaternion *= Quaternion.Euler(15f, 0f, 0f);
			Pose pose = new Pose(position, quaternion);
			this._panel.transform.SetPose(pose, Space.World);
			this._panel.SetActive(true);
		}

		public void InjectAllAUIToggler(GameObject panel)
		{
			this.InjectPanel(panel);
		}

		public void InjectPanel(GameObject panel)
		{
			this._panel = panel;
		}

		public void InjectOptionalCloseButton(Button closeButton)
		{
			this._closeButton = closeButton;
		}

		[SerializeField]
		private GameObject _panel;

		[SerializeField]
		[Optional]
		private Button _closeButton;

		[SerializeField]
		private Transform _headAnchor;

		[SerializeField]
		private Vector3 _spawnOffset = new Vector3(0f, -0.1f, 0.3f);

		protected bool _started;
	}
}
