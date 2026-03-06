using System;
using UnityEngine;

namespace Liv.Lck.Tablet
{
	public class LckSplitNotification : LckBaseNotification
	{
		public GameObject AndroidUI { get; private set; }

		public GameObject DesktopUI { get; private set; }

		private void Start()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				this.AndroidUI.SetActive(true);
				this.DesktopUI.SetActive(false);
				return;
			}
			this.DesktopUI.SetActive(true);
			this.AndroidUI.SetActive(false);
		}
	}
}
