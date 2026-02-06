using System;
using System.Runtime.CompilerServices;
using GorillaTagScripts;
using UnityEngine;

namespace GorillaNetworking
{
	public class GorillaNetworkLeaveRoomTrigger : GorillaTriggerBox
	{
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (NetworkSystem.Instance.InRoom && (!this.excludePrivateRooms || !NetworkSystem.Instance.SessionIsPrivate))
			{
				if (FriendshipGroupDetection.Instance.IsInParty)
				{
					FriendshipGroupDetection.Instance.LeaveParty();
					this.DisconnectAfterDelay(1f);
					return;
				}
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
		}

		private void DisconnectAfterDelay(float seconds)
		{
			GorillaNetworkLeaveRoomTrigger.<DisconnectAfterDelay>d__2 <DisconnectAfterDelay>d__;
			<DisconnectAfterDelay>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DisconnectAfterDelay>d__.seconds = seconds;
			<DisconnectAfterDelay>d__.<>1__state = -1;
			<DisconnectAfterDelay>d__.<>t__builder.Start<GorillaNetworkLeaveRoomTrigger.<DisconnectAfterDelay>d__2>(ref <DisconnectAfterDelay>d__);
		}

		[SerializeField]
		private bool excludePrivateRooms;
	}
}
