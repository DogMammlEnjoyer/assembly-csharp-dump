using System;
using System.Collections.Generic;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_ActionFile_ActionSet
	{
		[JsonIgnore]
		public string codeFriendlyName
		{
			get
			{
				return SteamVR_Input_ActionFile.GetCodeFriendlyName(this.name);
			}
		}

		[JsonIgnore]
		public string shortName
		{
			get
			{
				if (this.name.LastIndexOf('/') == this.name.Length - 1)
				{
					return string.Empty;
				}
				return SteamVR_Input_ActionFile.GetShortName(this.name);
			}
		}

		public void SetNewShortName(string newShortName)
		{
			this.name = SteamVR_Input_ActionFile_ActionSet.GetPathFromName(newShortName);
		}

		public static string CreateNewName()
		{
			return SteamVR_Input_ActionFile_ActionSet.GetPathFromName("NewSet");
		}

		public static string GetPathFromName(string name)
		{
			return string.Format("/actions/{0}", name);
		}

		public static SteamVR_Input_ActionFile_ActionSet CreateNew()
		{
			return new SteamVR_Input_ActionFile_ActionSet
			{
				name = SteamVR_Input_ActionFile_ActionSet.CreateNewName()
			};
		}

		public SteamVR_Input_ActionFile_ActionSet GetCopy()
		{
			return new SteamVR_Input_ActionFile_ActionSet
			{
				name = this.name,
				usage = this.usage
			};
		}

		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_ActionFile_ActionSet)
			{
				SteamVR_Input_ActionFile_ActionSet steamVR_Input_ActionFile_ActionSet = (SteamVR_Input_ActionFile_ActionSet)obj;
				return steamVR_Input_ActionFile_ActionSet == this || steamVR_Input_ActionFile_ActionSet.name == this.name;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		[JsonIgnore]
		private const string actionSetInstancePrefix = "instance_";

		public string name;

		public string usage;

		private const string nameTemplate = "/actions/{0}";

		[JsonIgnore]
		public List<SteamVR_Input_ActionFile_Action> actionsInList = new List<SteamVR_Input_ActionFile_Action>();

		[JsonIgnore]
		public List<SteamVR_Input_ActionFile_Action> actionsOutList = new List<SteamVR_Input_ActionFile_Action>();

		[JsonIgnore]
		public List<SteamVR_Input_ActionFile_Action> actionsList = new List<SteamVR_Input_ActionFile_Action>();
	}
}
