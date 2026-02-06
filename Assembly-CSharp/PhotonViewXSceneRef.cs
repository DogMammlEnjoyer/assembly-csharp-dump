using System;
using Photon.Pun;
using UnityEngine;

public class PhotonViewXSceneRef : MonoBehaviour
{
	public PhotonView photonView
	{
		get
		{
			PhotonView result;
			if (this.reference.TryResolve<PhotonView>(out result))
			{
				return result;
			}
			return null;
		}
	}

	[SerializeField]
	private XSceneRef reference;
}
