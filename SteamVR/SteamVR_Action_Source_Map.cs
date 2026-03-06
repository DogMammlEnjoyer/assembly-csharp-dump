using System;

namespace Valve.VR
{
	public abstract class SteamVR_Action_Source_Map<SourceElement> : SteamVR_Action_Source_Map where SourceElement : SteamVR_Action_Source, new()
	{
		public SourceElement this[SteamVR_Input_Sources inputSource]
		{
			get
			{
				return this.GetSourceElementForIndexer(inputSource);
			}
		}

		protected virtual void OnAccessSource(SteamVR_Input_Sources inputSource)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			for (int i = 0; i < this.sources.Length; i++)
			{
				if (this.sources[i] != null)
				{
					this.sources[i].Initialize();
				}
			}
		}

		protected override void PreinitializeMap(SteamVR_Input_Sources inputSource, SteamVR_Action wrappingAction)
		{
			this.sources[(int)inputSource] = Activator.CreateInstance<SourceElement>();
			this.sources[(int)inputSource].Preinitialize(wrappingAction, inputSource);
		}

		protected virtual SourceElement GetSourceElementForIndexer(SteamVR_Input_Sources inputSource)
		{
			this.OnAccessSource(inputSource);
			return this.sources[(int)inputSource];
		}

		protected SourceElement[] sources = new SourceElement[SteamVR_Input_Source.numSources];
	}
}
