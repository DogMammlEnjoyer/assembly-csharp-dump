using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;

namespace GorillaTag
{
	public static class MonkeAgentCleanup
	{
		static MonkeAgentCleanup()
		{
			MonkeAgentCleanup.k_destroyTimer.callback = new Action(MonkeAgentCleanup.CheckDestroyQueue);
			RoomSystem.LeftRoomEvent += new Action(MonkeAgentCleanup.OnLeftRoom);
		}

		public static void RegisterForDestroy(PhotonView target)
		{
			if (MonkeAgentCleanup.k_destroyTargets.Contains(target))
			{
				return;
			}
			if (target.gameObject.activeSelf)
			{
				target.gameObject.Disable();
			}
			if (MonkeAgentCleanup.k_destroyTargets.Add(target))
			{
				MonkeAgentCleanup.k_destroyQueue.Enqueue(target);
			}
			if (!MonkeAgentCleanup.k_destroyTimer.Running && MonkeAgentCleanup.k_destroyQueue.Count > 0)
			{
				MonkeAgentCleanup.k_destroyTimer.Start();
			}
		}

		private static void OnLeftRoom()
		{
			MonkeAgentCleanup.k_destroyQueue.Clear();
			MonkeAgentCleanup.k_destroyTargets.Clear();
			MonkeAgentCleanup.k_destroyTimer.Stop();
		}

		private static void CheckDestroyQueue()
		{
			if (!RoomSystem.JoinedRoom)
			{
				return;
			}
			bool flag = RoomSystem.GetLowestActorNumberPlayer() == NetworkSystem.Instance.LocalPlayer;
			int num = 0;
			while (MonkeAgentCleanup.k_destroyQueue.Count > 0 && num < 10)
			{
				PhotonView photonView = MonkeAgentCleanup.k_destroyQueue.Dequeue();
				if (MonkeAgentCleanup.k_destroyTargets.Remove(photonView) && !photonView.IsNull())
				{
					if ((photonView.IsRoomView && flag) || photonView.IsMine)
					{
						MonkeAgentCleanup.k_cacheInfo[MonkeAgentCleanup.k_viewIdKey] = photonView.InstantiationId;
						PhotonNetwork.NetworkingClient.OpRaiseEvent(202, MonkeAgentCleanup.k_cacheInfo, MonkeAgentCleanup.k_raiseEventOptions, SendOptions.SendReliable);
					}
					PhotonNetwork.RemoveInstantiatedGO(photonView.gameObject, true);
					num++;
				}
			}
			if (MonkeAgentCleanup.k_destroyTargets.Count == 0)
			{
				MonkeAgentCleanup.k_destroyTimer.Stop();
			}
		}

		private static readonly Queue<PhotonView> k_destroyQueue = new Queue<PhotonView>();

		private static readonly HashSet<PhotonView> k_destroyTargets = new HashSet<PhotonView>();

		private static readonly TickSystemTimer k_destroyTimer = new TickSystemTimer(1f);

		private static readonly Hashtable k_cacheInfo = new Hashtable(1);

		private static readonly RaiseEventOptions k_raiseEventOptions = new RaiseEventOptions
		{
			CachingOption = EventCaching.RemoveFromRoomCache
		};

		private static readonly object k_viewIdKey = 7;
	}
}
