using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Photon.Pun.UtilityScripts
{
	public class OnClickDestroy : MonoBehaviourPun, IPointerClickHandler, IEventSystemHandler
	{
		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (!PhotonNetwork.InRoom || (this.ModifierKey != KeyCode.None && !Input.GetKey(this.ModifierKey)) || eventData.button != this.Button)
			{
				return;
			}
			if (this.DestroyByRpc)
			{
				base.photonView.RPC("DestroyRpc", RpcTarget.AllBuffered, Array.Empty<object>());
				return;
			}
			PhotonNetwork.Destroy(base.gameObject);
		}

		[PunRPC]
		public IEnumerator DestroyRpc()
		{
			Object.Destroy(base.gameObject);
			yield return 0;
			yield break;
		}

		public PointerEventData.InputButton Button;

		public KeyCode ModifierKey;

		public bool DestroyByRpc;
	}
}
