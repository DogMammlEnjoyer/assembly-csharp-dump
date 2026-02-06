using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

internal class OwnershipGuardHandler : IPunOwnershipCallbacks
{
	static OwnershipGuardHandler()
	{
		PhotonNetwork.AddCallbackTarget(OwnershipGuardHandler.callbackInstance);
	}

	internal static void RegisterView(PhotonView view)
	{
		if (view == null || OwnershipGuardHandler.guardedViews.Contains(view))
		{
			return;
		}
		OwnershipGuardHandler.guardedViews.Add(view);
	}

	internal static void RegisterViews(PhotonView[] photonViews)
	{
		for (int i = 0; i < photonViews.Length; i++)
		{
			OwnershipGuardHandler.RegisterView(photonViews[i]);
		}
	}

	internal static void RemoveView(PhotonView view)
	{
		if (view == null)
		{
			return;
		}
		OwnershipGuardHandler.guardedViews.Remove(view);
	}

	internal static void RemoveViews(PhotonView[] photonViews)
	{
		for (int i = 0; i < photonViews.Length; i++)
		{
			OwnershipGuardHandler.RemoveView(photonViews[i]);
		}
	}

	void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		if (!OwnershipGuardHandler.guardedViews.Contains(targetView))
		{
			return;
		}
		if (targetView.IsRoomView)
		{
			if (targetView.Owner != PhotonNetwork.MasterClient)
			{
				targetView.OwnerActorNr = 0;
				targetView.ControllerActorNr = 0;
				return;
			}
		}
		else if (targetView.OwnerActorNr != targetView.CreatorActorNr || targetView.ControllerActorNr != targetView.CreatorActorNr)
		{
			targetView.OwnerActorNr = targetView.CreatorActorNr;
			targetView.ControllerActorNr = targetView.CreatorActorNr;
		}
	}

	void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	private static HashSet<PhotonView> guardedViews = new HashSet<PhotonView>();

	private static readonly OwnershipGuardHandler callbackInstance = new OwnershipGuardHandler();
}
