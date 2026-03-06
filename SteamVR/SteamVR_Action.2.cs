using System;
using UnityEngine;

namespace Valve.VR
{
	[Serializable]
	public abstract class SteamVR_Action : IEquatable<SteamVR_Action>, ISteamVR_Action, ISteamVR_Action_Source
	{
		public SteamVR_Action()
		{
		}

		public static CreateType Create<CreateType>(string newActionPath) where CreateType : SteamVR_Action, new()
		{
			CreateType createType = Activator.CreateInstance<CreateType>();
			createType.PreInitialize(newActionPath);
			return createType;
		}

		public static CreateType CreateUninitialized<CreateType>(string setName, SteamVR_ActionDirections direction, string newActionName, bool caseSensitive) where CreateType : SteamVR_Action, new()
		{
			CreateType createType = Activator.CreateInstance<CreateType>();
			createType.CreateUninitialized(setName, direction, newActionName, caseSensitive);
			return createType;
		}

		public static CreateType CreateUninitialized<CreateType>(string actionPath, bool caseSensitive) where CreateType : SteamVR_Action, new()
		{
			CreateType createType = Activator.CreateInstance<CreateType>();
			createType.CreateUninitialized(actionPath, caseSensitive);
			return createType;
		}

		public CreateType GetCopy<CreateType>() where CreateType : SteamVR_Action, new()
		{
			if (SteamVR_Input.ShouldMakeCopy())
			{
				CreateType createType = Activator.CreateInstance<CreateType>();
				createType.InitializeCopy(this.actionPath, this.GetSourceMap());
				return createType;
			}
			return (CreateType)((object)this);
		}

		public abstract string TryNeedsInitData();

		protected abstract void InitializeCopy(string newActionPath, SteamVR_Action_Source_Map newData);

		public abstract string fullPath { get; }

		public abstract ulong handle { get; }

		public abstract SteamVR_ActionSet actionSet { get; }

		public abstract SteamVR_ActionDirections direction { get; }

		public bool setActive
		{
			get
			{
				return this.actionSet.IsActive(SteamVR_Input_Sources.Any);
			}
		}

		public abstract bool active { get; }

		public abstract bool activeBinding { get; }

		public abstract bool lastActive { get; }

		public abstract bool lastActiveBinding { get; }

		public abstract void PreInitialize(string newActionPath);

		protected abstract void CreateUninitialized(string newActionPath, bool caseSensitive);

		protected abstract void CreateUninitialized(string newActionSet, SteamVR_ActionDirections direction, string newAction, bool caseSensitive);

		public abstract void Initialize(bool createNew = false, bool throwNotSetError = true);

		public abstract float GetTimeLastChanged(SteamVR_Input_Sources inputSource);

		public abstract SteamVR_Action_Source_Map GetSourceMap();

		public abstract bool GetActive(SteamVR_Input_Sources inputSource);

		public bool GetSetActive(SteamVR_Input_Sources inputSource)
		{
			return this.actionSet.IsActive(inputSource);
		}

		public abstract bool GetActiveBinding(SteamVR_Input_Sources inputSource);

		public abstract bool GetLastActive(SteamVR_Input_Sources inputSource);

		public abstract bool GetLastActiveBinding(SteamVR_Input_Sources inputSource);

		public string GetPath()
		{
			return this.actionPath;
		}

		public abstract bool IsUpdating(SteamVR_Input_Sources inputSource);

		public override int GetHashCode()
		{
			if (this.actionPath == null)
			{
				return 0;
			}
			return this.actionPath.GetHashCode();
		}

		public bool Equals(SteamVR_Action other)
		{
			return other != null && this.actionPath == other.actionPath;
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return string.IsNullOrEmpty(this.actionPath) || this.GetSourceMap() == null;
			}
			return this == other || (other is SteamVR_Action && this.Equals((SteamVR_Action)other));
		}

		public static bool operator !=(SteamVR_Action action1, SteamVR_Action action2)
		{
			return !(action1 == action2);
		}

		public static bool operator ==(SteamVR_Action action1, SteamVR_Action action2)
		{
			bool flag = action1 == null || string.IsNullOrEmpty(action1.actionPath) || action1.GetSourceMap() == null;
			bool flag2 = action2 == null || string.IsNullOrEmpty(action2.actionPath) || action2.GetSourceMap() == null;
			return (flag && flag2) || (flag == flag2 && action1.Equals(action2));
		}

		public static SteamVR_Action FindExistingActionForPartialPath(string path)
		{
			if (string.IsNullOrEmpty(path) || path.IndexOf('/') == -1)
			{
				return null;
			}
			string[] array = path.Split('/', StringSplitOptions.None);
			SteamVR_Action result;
			if (array.Length >= 5 && string.IsNullOrEmpty(array[2]))
			{
				string actionSetName = array[2];
				string actionName = array[4];
				result = SteamVR_Input.GetBaseAction(actionSetName, actionName, false);
			}
			else
			{
				result = SteamVR_Input.GetBaseActionFromPath(path, false);
			}
			return result;
		}

		public string GetShortName()
		{
			if (this.cachedShortName == null)
			{
				this.cachedShortName = SteamVR_Input_ActionFile.GetShortName(this.fullPath);
			}
			return this.cachedShortName;
		}

		public void ShowOrigins()
		{
			OpenVR.Input.ShowActionOrigins(this.actionSet.handle, this.handle);
		}

		public void HideOrigins()
		{
			OpenVR.Input.ShowActionOrigins(0UL, 0UL);
		}

		[SerializeField]
		protected string actionPath;

		[SerializeField]
		protected bool needsReinit;

		public static bool startUpdatingSourceOnAccess = true;

		[NonSerialized]
		private string cachedShortName;
	}
}
