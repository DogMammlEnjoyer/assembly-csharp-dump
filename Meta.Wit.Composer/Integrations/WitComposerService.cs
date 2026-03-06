using System;
using Meta.WitAi.Composer.Interfaces;

namespace Meta.WitAi.Composer.Integrations
{
	public class WitComposerService : ComposerService
	{
		protected override IComposerRequestHandler GetRequestHandler()
		{
			return this._requestHandler;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this._requestHandler == null)
			{
				this._requestHandler = new WitComposerRequestHandler(base.VoiceService.WitConfiguration);
			}
		}

		private WitComposerRequestHandler _requestHandler;
	}
}
