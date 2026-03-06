using System;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_ActionFile_Action
	{
		[JsonIgnore]
		public static string[] requirementValues
		{
			get
			{
				if (SteamVR_Input_ActionFile_Action._requirementValues == null)
				{
					SteamVR_Input_ActionFile_Action._requirementValues = Enum.GetNames(typeof(SteamVR_Input_ActionFile_Action_Requirements));
				}
				return SteamVR_Input_ActionFile_Action._requirementValues;
			}
		}

		public SteamVR_Input_ActionFile_Action GetCopy()
		{
			return new SteamVR_Input_ActionFile_Action
			{
				name = this.name,
				type = this.type,
				scope = this.scope,
				skeleton = this.skeleton,
				requirement = this.requirement
			};
		}

		[JsonIgnore]
		public SteamVR_Input_ActionFile_Action_Requirements requirementEnum
		{
			get
			{
				for (int i = 0; i < SteamVR_Input_ActionFile_Action.requirementValues.Length; i++)
				{
					if (string.Equals(SteamVR_Input_ActionFile_Action.requirementValues[i], this.requirement, StringComparison.CurrentCultureIgnoreCase))
					{
						return (SteamVR_Input_ActionFile_Action_Requirements)i;
					}
				}
				return SteamVR_Input_ActionFile_Action_Requirements.suggested;
			}
			set
			{
				this.requirement = value.ToString();
			}
		}

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
				return SteamVR_Input_ActionFile.GetShortName(this.name);
			}
		}

		[JsonIgnore]
		public string path
		{
			get
			{
				int num = this.name.LastIndexOf('/');
				if (num != -1 && num + 1 < this.name.Length)
				{
					return this.name.Substring(0, num + 1);
				}
				return this.name;
			}
		}

		public static string CreateNewName(string actionSet, string direction)
		{
			return string.Format("/actions/{0}/{1}/{2}", actionSet, direction, "NewAction");
		}

		public static string CreateNewName(string actionSet, SteamVR_ActionDirections direction, string actionName)
		{
			return string.Format("/actions/{0}/{1}/{2}", actionSet, direction.ToString().ToLower(), actionName);
		}

		public static SteamVR_Input_ActionFile_Action CreateNew(string actionSet, SteamVR_ActionDirections direction, string actionType)
		{
			return new SteamVR_Input_ActionFile_Action
			{
				name = SteamVR_Input_ActionFile_Action.CreateNewName(actionSet, direction.ToString().ToLower()),
				type = actionType
			};
		}

		[JsonIgnore]
		public SteamVR_ActionDirections direction
		{
			get
			{
				if (this.type.ToLower() == SteamVR_Input_ActionFile_ActionTypes.vibration)
				{
					return SteamVR_ActionDirections.Out;
				}
				return SteamVR_ActionDirections.In;
			}
		}

		[JsonIgnore]
		public string actionSet
		{
			get
			{
				int num = this.name.IndexOf('/', "/actions/".Length);
				if (num == -1)
				{
					return string.Empty;
				}
				return this.name.Substring(0, num);
			}
		}

		public void SetNewActionSet(string newSetName)
		{
			this.name = string.Format("/actions/{0}/{1}/{2}", newSetName, this.direction.ToString().ToLower(), this.shortName);
		}

		public override string ToString()
		{
			return this.shortName;
		}

		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_ActionFile_Action)
			{
				SteamVR_Input_ActionFile_Action steamVR_Input_ActionFile_Action = (SteamVR_Input_ActionFile_Action)obj;
				return this == obj || (this.name == steamVR_Input_ActionFile_Action.name && this.type == steamVR_Input_ActionFile_Action.type && this.skeleton == steamVR_Input_ActionFile_Action.skeleton && this.requirement == steamVR_Input_ActionFile_Action.requirement);
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		[JsonIgnore]
		private static string[] _requirementValues;

		public string name;

		public string type;

		public string scope;

		public string skeleton;

		public string requirement;

		private const string nameTemplate = "/actions/{0}/{1}/{2}";

		protected const string prefix = "/actions/";
	}
}
