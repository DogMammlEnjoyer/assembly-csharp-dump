using System;

namespace Valve.VR
{
	[Serializable]
	public abstract class SteamVR_Action<SourceMap, SourceElement> : SteamVR_Action, ISteamVR_Action, ISteamVR_Action_Source where SourceMap : SteamVR_Action_Source_Map<SourceElement>, new() where SourceElement : SteamVR_Action_Source, new()
	{
		public virtual SourceElement this[SteamVR_Input_Sources inputSource]
		{
			get
			{
				return this.sourceMap[inputSource];
			}
		}

		public override string fullPath
		{
			get
			{
				return this.sourceMap.fullPath;
			}
		}

		public override ulong handle
		{
			get
			{
				return this.sourceMap.handle;
			}
		}

		public override SteamVR_ActionSet actionSet
		{
			get
			{
				return this.sourceMap.actionSet;
			}
		}

		public override SteamVR_ActionDirections direction
		{
			get
			{
				return this.sourceMap.direction;
			}
		}

		public override bool active
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].active;
			}
		}

		public override bool lastActive
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastActive;
			}
		}

		public override bool activeBinding
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].activeBinding;
			}
		}

		public override bool lastActiveBinding
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastActiveBinding;
			}
		}

		public override void PreInitialize(string newActionPath)
		{
			this.actionPath = newActionPath;
			this.sourceMap = Activator.CreateInstance<SourceMap>();
			this.sourceMap.PreInitialize(this, this.actionPath, true);
			this.initialized = true;
		}

		protected override void CreateUninitialized(string newActionPath, bool caseSensitive)
		{
			this.actionPath = newActionPath;
			this.sourceMap = Activator.CreateInstance<SourceMap>();
			this.sourceMap.PreInitialize(this, this.actionPath, false);
			this.needsReinit = true;
			this.initialized = false;
		}

		protected override void CreateUninitialized(string newActionSet, SteamVR_ActionDirections direction, string newAction, bool caseSensitive)
		{
			this.actionPath = SteamVR_Input_ActionFile_Action.CreateNewName(newActionSet, direction, newAction);
			this.sourceMap = Activator.CreateInstance<SourceMap>();
			this.sourceMap.PreInitialize(this, this.actionPath, false);
			this.needsReinit = true;
			this.initialized = false;
		}

		public override string TryNeedsInitData()
		{
			if (this.needsReinit && this.actionPath != null)
			{
				SteamVR_Action steamVR_Action = SteamVR_Action.FindExistingActionForPartialPath(this.actionPath);
				if (!(steamVR_Action == null))
				{
					this.actionPath = steamVR_Action.fullPath;
					this.sourceMap = (SourceMap)((object)steamVR_Action.GetSourceMap());
					this.initialized = true;
					this.needsReinit = false;
					return this.actionPath;
				}
				this.sourceMap = default(SourceMap);
			}
			return null;
		}

		public override void Initialize(bool createNew = false, bool throwErrors = true)
		{
			if (this.needsReinit)
			{
				this.TryNeedsInitData();
			}
			if (createNew)
			{
				this.sourceMap.Initialize();
			}
			else
			{
				this.sourceMap = SteamVR_Input.GetActionDataFromPath<SourceMap>(this.actionPath, false);
				SourceMap sourceMap = this.sourceMap;
			}
			this.initialized = true;
		}

		public override SteamVR_Action_Source_Map GetSourceMap()
		{
			return this.sourceMap;
		}

		protected override void InitializeCopy(string newActionPath, SteamVR_Action_Source_Map newData)
		{
			this.actionPath = newActionPath;
			this.sourceMap = (SourceMap)((object)newData);
			this.initialized = true;
		}

		protected void InitAfterDeserialize()
		{
			if (this.sourceMap != null)
			{
				if (this.sourceMap.fullPath != this.actionPath)
				{
					this.needsReinit = true;
					this.TryNeedsInitData();
				}
				if (string.IsNullOrEmpty(this.actionPath))
				{
					this.sourceMap = default(SourceMap);
				}
			}
			if (!this.initialized)
			{
				this.Initialize(false, false);
			}
		}

		public override bool GetActive(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].active;
		}

		public override bool GetActiveBinding(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].activeBinding;
		}

		public override bool GetLastActive(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].lastActive;
		}

		public override bool GetLastActiveBinding(SteamVR_Input_Sources inputSource)
		{
			return this.sourceMap[inputSource].lastActiveBinding;
		}

		[NonSerialized]
		protected SourceMap sourceMap;

		[NonSerialized]
		protected bool initialized;
	}
}
