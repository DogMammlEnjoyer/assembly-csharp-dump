using System;
using System.Globalization;
using UnityEngine;

namespace Valve.VR
{
	public abstract class SteamVR_Action_Source_Map
	{
		public string fullPath { get; protected set; }

		public ulong handle { get; protected set; }

		public SteamVR_ActionSet actionSet { get; protected set; }

		public SteamVR_ActionDirections direction { get; protected set; }

		public virtual void PreInitialize(SteamVR_Action wrappingAction, string actionPath, bool throwErrors = true)
		{
			this.fullPath = actionPath;
			this.action = wrappingAction;
			this.actionSet = SteamVR_Input.GetActionSetFromPath(this.GetActionSetPath(), false);
			this.direction = this.GetActionDirection();
			SteamVR_Input_Sources[] allSources = SteamVR_Input_Source.GetAllSources();
			for (int i = 0; i < allSources.Length; i++)
			{
				this.PreinitializeMap(allSources[i], wrappingAction);
			}
		}

		protected abstract void PreinitializeMap(SteamVR_Input_Sources inputSource, SteamVR_Action wrappingAction);

		public virtual void Initialize()
		{
			ulong handle = 0UL;
			EVRInputError actionHandle = OpenVR.Input.GetActionHandle(this.fullPath.ToLowerInvariant(), ref handle);
			this.handle = handle;
			if (actionHandle != EVRInputError.None)
			{
				Debug.LogError("<b>[SteamVR]</b> GetActionHandle (" + this.fullPath.ToLowerInvariant() + ") error: " + actionHandle.ToString());
			}
		}

		private string GetActionSetPath()
		{
			int startIndex = this.fullPath.IndexOf('/', 1) + 1;
			int length = this.fullPath.IndexOf('/', startIndex);
			return this.fullPath.Substring(0, length);
		}

		private SteamVR_ActionDirections GetActionDirection()
		{
			int startIndex = this.fullPath.IndexOf('/', 1) + 1;
			int num = this.fullPath.IndexOf('/', startIndex);
			int length = this.fullPath.IndexOf('/', num + 1) - num - 1;
			string text = this.fullPath.Substring(num + 1, length);
			if (text == SteamVR_Action_Source_Map.inLowered)
			{
				return SteamVR_ActionDirections.In;
			}
			if (text == SteamVR_Action_Source_Map.outLowered)
			{
				return SteamVR_ActionDirections.Out;
			}
			Debug.LogError("Could not find match for direction: " + text);
			return SteamVR_ActionDirections.In;
		}

		public SteamVR_Action action;

		private static string inLowered = "IN".ToLower(CultureInfo.CurrentCulture);

		private static string outLowered = "OUT".ToLower(CultureInfo.CurrentCulture);
	}
}
