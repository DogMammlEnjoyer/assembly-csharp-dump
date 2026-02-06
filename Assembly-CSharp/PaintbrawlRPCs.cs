using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

internal class PaintbrawlRPCs : RPCNetworkBase
{
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.paintbrawlManager = (GorillaPaintbrawlManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	[PunRPC]
	public void RPC_ReportSlingshotHit(Player taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RPC_ReportSlingshotHit");
		if (!NetworkSystem.Instance.IsMasterClient || taggedPlayer == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(taggedPlayer);
		PhotonMessageInfoWrapped info2 = new PhotonMessageInfoWrapped(info);
		this.paintbrawlManager.ReportSlingshotHit(player, hitLocation, projectileCount, info2);
	}

	private GameModeSerializer serializer;

	private GorillaPaintbrawlManager paintbrawlManager;
}
