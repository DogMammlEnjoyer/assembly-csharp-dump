using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
	public class PunTurnManager : MonoBehaviourPunCallbacks, IOnEventCallback
	{
		public int Turn
		{
			get
			{
				return PhotonNetwork.CurrentRoom.GetTurn();
			}
			private set
			{
				this._isOverCallProcessed = false;
				PhotonNetwork.CurrentRoom.SetTurn(value, true);
			}
		}

		public float ElapsedTimeInTurn
		{
			get
			{
				return (float)(PhotonNetwork.ServerTimestamp - PhotonNetwork.CurrentRoom.GetTurnStart()) / 1000f;
			}
		}

		public float RemainingSecondsInTurn
		{
			get
			{
				return Mathf.Max(0f, this.TurnDuration - this.ElapsedTimeInTurn);
			}
		}

		public bool IsCompletedByAll
		{
			get
			{
				return PhotonNetwork.CurrentRoom != null && this.Turn > 0 && this.finishedPlayers.Count == (int)PhotonNetwork.CurrentRoom.PlayerCount;
			}
		}

		public bool IsFinishedByMe
		{
			get
			{
				return this.finishedPlayers.Contains(PhotonNetwork.LocalPlayer);
			}
		}

		public bool IsOver
		{
			get
			{
				return this.RemainingSecondsInTurn <= 0f;
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (this.Turn > 0 && this.IsOver && !this._isOverCallProcessed)
			{
				this._isOverCallProcessed = true;
				this.TurnManagerListener.OnTurnTimeEnds(this.Turn);
			}
		}

		public void BeginTurn()
		{
			this.Turn++;
		}

		public void SendMove(object move, bool finished)
		{
			if (this.IsFinishedByMe)
			{
				Debug.LogWarning("Can't SendMove. Turn is finished by this player.");
				return;
			}
			Hashtable hashtable = new Hashtable();
			hashtable.Add("turn", this.Turn);
			hashtable.Add("move", move);
			byte eventCode = finished ? 2 : 1;
			PhotonNetwork.RaiseEvent(eventCode, hashtable, new RaiseEventOptions
			{
				CachingOption = EventCaching.AddToRoomCache
			}, SendOptions.SendReliable);
			if (finished)
			{
				PhotonNetwork.LocalPlayer.SetFinishedTurn(this.Turn);
			}
			this.ProcessOnEvent(eventCode, hashtable, PhotonNetwork.LocalPlayer.ActorNumber);
		}

		public bool GetPlayerFinishedTurn(Player player)
		{
			return player != null && this.finishedPlayers != null && this.finishedPlayers.Contains(player);
		}

		private void ProcessOnEvent(byte eventCode, object content, int senderId)
		{
			if (senderId == -1)
			{
				return;
			}
			this.sender = PhotonNetwork.CurrentRoom.GetPlayer(senderId, false);
			if (eventCode == 1)
			{
				Hashtable hashtable = content as Hashtable;
				int turn = (int)hashtable["turn"];
				object move = hashtable["move"];
				this.TurnManagerListener.OnPlayerMove(this.sender, turn, move);
				return;
			}
			if (eventCode != 2)
			{
				return;
			}
			Hashtable hashtable2 = content as Hashtable;
			int num = (int)hashtable2["turn"];
			object move2 = hashtable2["move"];
			if (num == this.Turn)
			{
				this.finishedPlayers.Add(this.sender);
				this.TurnManagerListener.OnPlayerFinished(this.sender, num, move2);
			}
			if (this.IsCompletedByAll)
			{
				this.TurnManagerListener.OnTurnCompleted(this.Turn);
			}
		}

		public void OnEvent(EventData photonEvent)
		{
			this.ProcessOnEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender);
		}

		public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			if (propertiesThatChanged.ContainsKey("Turn"))
			{
				this._isOverCallProcessed = false;
				this.finishedPlayers.Clear();
				this.TurnManagerListener.OnTurnBegins(this.Turn);
			}
		}

		private Player sender;

		public float TurnDuration = 20f;

		public IPunTurnManagerCallbacks TurnManagerListener;

		private readonly HashSet<Player> finishedPlayers = new HashSet<Player>();

		public const byte TurnManagerEventOffset = 0;

		public const byte EvMove = 1;

		public const byte EvFinalMove = 2;

		private bool _isOverCallProcessed;
	}
}
