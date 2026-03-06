using System;
using UnityEngine;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_ActionSet : IEquatable<SteamVR_ActionSet>, ISteamVR_ActionSet, ISerializationCallbackReceiver
	{
		public SteamVR_Action[] allActions
		{
			get
			{
				if (!this.initialized)
				{
					this.Initialize(false, true);
				}
				return this.setData.allActions;
			}
		}

		public ISteamVR_Action_In[] nonVisualInActions
		{
			get
			{
				if (!this.initialized)
				{
					this.Initialize(false, true);
				}
				return this.setData.nonVisualInActions;
			}
		}

		public ISteamVR_Action_In[] visualActions
		{
			get
			{
				if (!this.initialized)
				{
					this.Initialize(false, true);
				}
				return this.setData.visualActions;
			}
		}

		public SteamVR_Action_Pose[] poseActions
		{
			get
			{
				if (!this.initialized)
				{
					this.Initialize(false, true);
				}
				return this.setData.poseActions;
			}
		}

		public SteamVR_Action_Skeleton[] skeletonActions
		{
			get
			{
				if (!this.initialized)
				{
					this.Initialize(false, true);
				}
				return this.setData.skeletonActions;
			}
		}

		public ISteamVR_Action_Out[] outActionArray
		{
			get
			{
				if (!this.initialized)
				{
					this.Initialize(false, true);
				}
				return this.setData.outActionArray;
			}
		}

		public string fullPath
		{
			get
			{
				if (!this.initialized)
				{
					this.Initialize(false, true);
				}
				return this.setData.fullPath;
			}
		}

		public string usage
		{
			get
			{
				if (!this.initialized)
				{
					this.Initialize(false, true);
				}
				return this.setData.usage;
			}
		}

		public ulong handle
		{
			get
			{
				if (!this.initialized)
				{
					this.Initialize(false, true);
				}
				return this.setData.handle;
			}
		}

		public static CreateType Create<CreateType>(string newSetPath) where CreateType : SteamVR_ActionSet, new()
		{
			CreateType createType = Activator.CreateInstance<CreateType>();
			createType.PreInitialize(newSetPath);
			return createType;
		}

		public static CreateType CreateFromName<CreateType>(string newSetName) where CreateType : SteamVR_ActionSet, new()
		{
			CreateType createType = Activator.CreateInstance<CreateType>();
			createType.PreInitialize(SteamVR_Input_ActionFile_ActionSet.GetPathFromName(newSetName));
			return createType;
		}

		public void PreInitialize(string newActionPath)
		{
			this.actionSetPath = newActionPath;
			this.setData = new SteamVR_ActionSet_Data();
			this.setData.fullPath = this.actionSetPath;
			this.setData.PreInitialize();
			this.initialized = true;
		}

		public virtual void FinishPreInitialize()
		{
			this.setData.FinishPreInitialize();
		}

		public virtual void Initialize(bool createNew = false, bool throwErrors = true)
		{
			if (createNew)
			{
				this.setData.Initialize();
			}
			else
			{
				this.setData = SteamVR_Input.GetActionSetDataFromPath(this.actionSetPath, false);
				SteamVR_ActionSet_Data steamVR_ActionSet_Data = this.setData;
			}
			this.initialized = true;
		}

		public string GetPath()
		{
			return this.actionSetPath;
		}

		public bool IsActive(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any)
		{
			return this.setData.IsActive(source);
		}

		public float GetTimeLastChanged(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any)
		{
			return this.setData.GetTimeLastChanged(source);
		}

		public void Activate(SteamVR_Input_Sources activateForSource = SteamVR_Input_Sources.Any, int priority = 0, bool disableAllOtherActionSets = false)
		{
			this.setData.Activate(activateForSource, priority, disableAllOtherActionSets);
		}

		public void Deactivate(SteamVR_Input_Sources forSource = SteamVR_Input_Sources.Any)
		{
			this.setData.Deactivate(forSource);
		}

		public string GetShortName()
		{
			return this.setData.GetShortName();
		}

		public bool ShowBindingHints(ISteamVR_Action_In originToHighlight = null)
		{
			if (originToHighlight == null)
			{
				return SteamVR_Input.ShowBindingHints(this);
			}
			return SteamVR_Input.ShowBindingHints(originToHighlight);
		}

		public bool ReadRawSetActive(SteamVR_Input_Sources inputSource)
		{
			return this.setData.ReadRawSetActive(inputSource);
		}

		public float ReadRawSetLastChanged(SteamVR_Input_Sources inputSource)
		{
			return this.setData.ReadRawSetLastChanged(inputSource);
		}

		public int ReadRawSetPriority(SteamVR_Input_Sources inputSource)
		{
			return this.setData.ReadRawSetPriority(inputSource);
		}

		public SteamVR_ActionSet_Data GetActionSetData()
		{
			return this.setData;
		}

		public CreateType GetCopy<CreateType>() where CreateType : SteamVR_ActionSet, new()
		{
			if (SteamVR_Input.ShouldMakeCopy())
			{
				CreateType createType = Activator.CreateInstance<CreateType>();
				createType.actionSetPath = this.actionSetPath;
				createType.setData = this.setData;
				createType.initialized = true;
				return createType;
			}
			return (CreateType)((object)this);
		}

		public bool Equals(SteamVR_ActionSet other)
		{
			return other != null && this.actionSetPath == other.actionSetPath;
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return string.IsNullOrEmpty(this.actionSetPath);
			}
			return this == other || (other is SteamVR_ActionSet && this.Equals((SteamVR_ActionSet)other));
		}

		public override int GetHashCode()
		{
			if (this.actionSetPath == null)
			{
				return 0;
			}
			return this.actionSetPath.GetHashCode();
		}

		public static bool operator !=(SteamVR_ActionSet set1, SteamVR_ActionSet set2)
		{
			return !(set1 == set2);
		}

		public static bool operator ==(SteamVR_ActionSet set1, SteamVR_ActionSet set2)
		{
			bool flag = set1 == null || string.IsNullOrEmpty(set1.actionSetPath) || set1.GetActionSetData() == null;
			bool flag2 = set2 == null || string.IsNullOrEmpty(set2.actionSetPath) || set2.GetActionSetData() == null;
			return (flag && flag2) || (flag == flag2 && set1.Equals(set2));
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (this.setData != null && this.setData.fullPath != this.actionSetPath)
			{
				this.setData = SteamVR_Input.GetActionSetDataFromPath(this.actionSetPath, false);
			}
			if (!this.initialized)
			{
				this.Initialize(false, false);
			}
		}

		[SerializeField]
		private string actionSetPath;

		[NonSerialized]
		protected SteamVR_ActionSet_Data setData;

		[NonSerialized]
		protected bool initialized;
	}
}
