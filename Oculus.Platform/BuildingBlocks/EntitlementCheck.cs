using System;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform.BuildingBlocks
{
	public class EntitlementCheck : MonoBehaviour
	{
		public event Action UserFailedEntitlementCheck;

		public event Action UserPassedEntitlementCheck;

		private void Start()
		{
			if (this.quitAppOnNotEntitled)
			{
				this.UserFailedEntitlementCheck += this.QuitAppOnFailure;
			}
			this.PerformUserEntitlementCheck();
		}

		public void PerformUserEntitlementCheck()
		{
			if (!Core.IsInitialized())
			{
				try
				{
					Core.AsyncInitialize(null).OnComplete(new Message<PlatformInitialize>.Callback(this.PlatformInitializeCallback));
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception occured during OvrPlatform init - " + ex.Message);
				}
			}
		}

		public void PlatformInitializeCallback(Message<PlatformInitialize> msg)
		{
			PlatformInitialize data = msg.Data;
			if (data != null && data.Result <= PlatformInitializeResult.Success)
			{
				try
				{
					Entitlements.IsUserEntitledToApplication().OnComplete(new Message.Callback(this.EntitlementCheckCallback));
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception occured during Entitlement Check - " + ex.Message);
				}
				return;
			}
			Debug.LogError(string.Format("OvrPlatform init resulted in failure. - {0}\n{1}", msg.Data.Result, msg.GetError().Message));
			Action userFailedEntitlementCheck = this.UserFailedEntitlementCheck;
			if (userFailedEntitlementCheck == null)
			{
				return;
			}
			userFailedEntitlementCheck();
		}

		private void EntitlementCheckCallback(Message msg)
		{
			if (!msg.IsError)
			{
				Debug.Log("You are entitled to use this app.");
				Action userPassedEntitlementCheck = this.UserPassedEntitlementCheck;
				if (userPassedEntitlementCheck == null)
				{
					return;
				}
				userPassedEntitlementCheck();
				return;
			}
			else
			{
				Debug.LogError("You are NOT entitled to use this app.");
				Action userFailedEntitlementCheck = this.UserFailedEntitlementCheck;
				if (userFailedEntitlementCheck == null)
				{
					return;
				}
				userFailedEntitlementCheck();
				return;
			}
		}

		private void QuitAppOnFailure()
		{
			Debug.LogError("Oculus user entitlement check failed. Exiting now...");
			Application.Quit();
		}

		public bool quitAppOnNotEntitled;
	}
}
