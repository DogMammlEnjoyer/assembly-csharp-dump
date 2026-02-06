using System;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.UtilityScripts
{
	public class PointedAtGameObjectInfo : MonoBehaviour
	{
		private void Start()
		{
			if (PointedAtGameObjectInfo.Instance != null)
			{
				Debug.LogWarning("PointedAtGameObjectInfo is already featured in the scene, gameobject is destroyed");
				Object.Destroy(base.gameObject);
			}
			PointedAtGameObjectInfo.Instance = this;
		}

		public void SetFocus(PhotonView pv)
		{
			this.focus = ((pv != null) ? pv.transform : null);
			if (pv != null)
			{
				this.text.text = string.Format("id {0} own: {1} {2}{3}", new object[]
				{
					pv.ViewID,
					pv.OwnerActorNr,
					pv.IsRoomView ? "scn" : "",
					pv.IsMine ? " mine" : ""
				});
				return;
			}
			this.text.text = string.Empty;
		}

		public void RemoveFocus(PhotonView pv)
		{
			if (pv == null)
			{
				this.text.text = string.Empty;
				return;
			}
			if (pv.transform == this.focus)
			{
				this.text.text = string.Empty;
				return;
			}
		}

		private void LateUpdate()
		{
			if (this.focus != null)
			{
				base.transform.position = Camera.main.WorldToScreenPoint(this.focus.position);
			}
		}

		public static PointedAtGameObjectInfo Instance;

		public Text text;

		private Transform focus;
	}
}
