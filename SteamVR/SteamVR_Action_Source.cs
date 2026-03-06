using System;

namespace Valve.VR
{
	public abstract class SteamVR_Action_Source : ISteamVR_Action_Source
	{
		public string fullPath
		{
			get
			{
				return this.action.fullPath;
			}
		}

		public ulong handle
		{
			get
			{
				return this.action.handle;
			}
		}

		public SteamVR_ActionSet actionSet
		{
			get
			{
				return this.action.actionSet;
			}
		}

		public SteamVR_ActionDirections direction
		{
			get
			{
				return this.action.direction;
			}
		}

		public SteamVR_Input_Sources inputSource { get; protected set; }

		public bool setActive
		{
			get
			{
				return this.actionSet.IsActive(this.inputSource);
			}
		}

		public abstract bool active { get; }

		public abstract bool activeBinding { get; }

		public abstract bool lastActive { get; protected set; }

		public abstract bool lastActiveBinding { get; }

		public virtual void Preinitialize(SteamVR_Action wrappingAction, SteamVR_Input_Sources forInputSource)
		{
			this.action = wrappingAction;
			this.inputSource = forInputSource;
		}

		public SteamVR_Action_Source()
		{
		}

		public virtual void Initialize()
		{
			this.inputSourceHandle = SteamVR_Input_Source.GetHandle(this.inputSource);
		}

		protected ulong inputSourceHandle;

		protected SteamVR_Action action;
	}
}
