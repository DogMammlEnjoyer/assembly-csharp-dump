using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Photon.Pun.UtilityScripts
{
	public class OnClickInstantiate : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (!PhotonNetwork.InRoom || (this.ModifierKey != KeyCode.None && !Input.GetKey(this.ModifierKey)) || eventData.button != this.Button)
			{
				return;
			}
			OnClickInstantiate.InstantiateOption instantiateType = this.InstantiateType;
			if (instantiateType == OnClickInstantiate.InstantiateOption.Mine)
			{
				PhotonNetwork.Instantiate(this.Prefab.name, eventData.pointerCurrentRaycast.worldPosition + new Vector3(0f, 0.5f, 0f), Quaternion.identity, 0, null);
				return;
			}
			if (instantiateType != OnClickInstantiate.InstantiateOption.Scene)
			{
				return;
			}
			PhotonNetwork.InstantiateRoomObject(this.Prefab.name, eventData.pointerCurrentRaycast.worldPosition + new Vector3(0f, 0.5f, 0f), Quaternion.identity, 0, null);
		}

		public PointerEventData.InputButton Button;

		public KeyCode ModifierKey;

		public GameObject Prefab;

		[SerializeField]
		private OnClickInstantiate.InstantiateOption InstantiateType;

		public enum InstantiateOption
		{
			Mine,
			Scene
		}
	}
}
