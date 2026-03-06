using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class ISDKSceneMenuManager : MonoBehaviour
	{
		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		public void ToggleMenu()
		{
			if (this._menuParent.activeSelf)
			{
				this._hideMenuAudio.Play();
				this._menuParent.SetActive(false);
				return;
			}
			this._showMenuAudio.Play();
			this._menuParent.transform.position = this._spawnPoint.transform.position;
			this._menuParent.transform.rotation = this._spawnPoint.transform.rotation;
			this._menuParent.SetActive(true);
		}

		public void InjectAllMenuItems(GameObject parent, AudioSource show, AudioSource hide, GameObject spawnpoint)
		{
			this.InjectMenuParent(parent);
			this.InjectShowAudio(show);
			this.InjectHideAudio(hide);
			this.InjectSpawnPoint(spawnpoint);
		}

		public void InjectMenuParent(GameObject parent)
		{
			this._menuParent = parent;
		}

		public void InjectShowAudio(AudioSource show)
		{
			this._showMenuAudio = show;
		}

		public void InjectHideAudio(AudioSource hide)
		{
			this._showMenuAudio = hide;
		}

		public void InjectSpawnPoint(GameObject spawnpoint)
		{
			this._menuParent = spawnpoint;
		}

		[Tooltip("The parent object of the menu")]
		[Header("Place the grabbable parent object here")]
		[SerializeField]
		private GameObject _menuParent;

		[Tooltip("The audio to play when showing the menu panel")]
		[Header("Place the menu open audio here")]
		[SerializeField]
		private AudioSource _showMenuAudio;

		[Tooltip("The audio to play when hiding the menu panel")]
		[Header("Place the menu hide audio here")]
		[SerializeField]
		private AudioSource _hideMenuAudio;

		[Tooltip("The location the menu should be spawning at")]
		[Header("The location the menu should be spawning at")]
		[SerializeField]
		private GameObject _spawnPoint;

		protected bool _started;
	}
}
