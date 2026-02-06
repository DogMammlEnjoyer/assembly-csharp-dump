using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio.Mods;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.UI.ModIO
{
	public class CustomMapsRoomMapDisplay : MonoBehaviour
	{
		public void Start()
		{
			this.roomMapNameText.text = this.noRoomMapString;
			this.roomMapStatusText.text = this.notLoadedStatusString;
			this.roomMapLabelText.gameObject.SetActive(true);
			this.roomMapNameText.gameObject.SetActive(true);
			this.roomMapStatusLabelText.gameObject.SetActive(false);
			this.roomMapStatusText.gameObject.SetActive(false);
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnectedFromRoom;
			CustomMapManager.OnRoomMapChanged.AddListener(new UnityAction<ModId>(this.OnRoomMapChanged));
			CustomMapManager.OnMapLoadStatusChanged.AddListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
			CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnMapLoadComplete));
		}

		public void OnDestroy()
		{
			NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnectedFromRoom;
			CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		}

		private void OnJoinedRoom()
		{
			this.UpdateRoomMap();
		}

		private void OnDisconnectedFromRoom()
		{
			this.UpdateRoomMap();
		}

		private void OnRoomMapChanged(ModId roomMapModId)
		{
			this.UpdateRoomMap();
		}

		private Task UpdateRoomMap()
		{
			CustomMapsRoomMapDisplay.<UpdateRoomMap>d__18 <UpdateRoomMap>d__;
			<UpdateRoomMap>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UpdateRoomMap>d__.<>4__this = this;
			<UpdateRoomMap>d__.<>1__state = -1;
			<UpdateRoomMap>d__.<>t__builder.Start<CustomMapsRoomMapDisplay.<UpdateRoomMap>d__18>(ref <UpdateRoomMap>d__);
			return <UpdateRoomMap>d__.<>t__builder.Task;
		}

		private void OnMapLoadComplete(bool success)
		{
			if (success)
			{
				this.roomMapStatusText.text = this.readyToPlayStatusString;
				this.roomMapStatusText.color = this.readyToPlayStatusStringColor;
				return;
			}
			this.roomMapStatusText.text = this.loadFailedStatusString;
			this.roomMapStatusText.color = this.loadFailedStatusStringColor;
		}

		private void OnMapLoadProgress(MapLoadStatus status, int progress, string message)
		{
			if (status - MapLoadStatus.Downloading <= 1)
			{
				this.roomMapStatusText.text = this.loadingStatusString;
				this.roomMapStatusText.color = this.loadingStatusStringColor;
			}
		}

		[SerializeField]
		private TMP_Text roomMapLabelText;

		[SerializeField]
		private TMP_Text roomMapNameText;

		[SerializeField]
		private TMP_Text roomMapStatusLabelText;

		[SerializeField]
		private TMP_Text roomMapStatusText;

		[SerializeField]
		private string noRoomMapString = "NONE";

		[SerializeField]
		private string notLoadedStatusString = "NOT LOADED";

		[SerializeField]
		private string loadingStatusString = "LOADING...";

		[SerializeField]
		private string readyToPlayStatusString = "READY!";

		[SerializeField]
		private string loadFailedStatusString = "LOAD FAILED";

		[SerializeField]
		private Color notLoadedStatusStringColor = Color.red;

		[SerializeField]
		private Color loadingStatusStringColor = Color.yellow;

		[SerializeField]
		private Color readyToPlayStatusStringColor = Color.green;

		[SerializeField]
		private Color loadFailedStatusStringColor = Color.red;
	}
}
