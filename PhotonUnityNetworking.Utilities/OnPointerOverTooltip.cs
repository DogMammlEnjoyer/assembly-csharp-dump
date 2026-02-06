using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Photon.Pun.UtilityScripts
{
	public class OnPointerOverTooltip : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		private void OnDestroy()
		{
			PointedAtGameObjectInfo.Instance.RemoveFocus(base.GetComponent<PhotonView>());
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			PointedAtGameObjectInfo.Instance.RemoveFocus(base.GetComponent<PhotonView>());
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			PointedAtGameObjectInfo.Instance.SetFocus(base.GetComponent<PhotonView>());
		}
	}
}
