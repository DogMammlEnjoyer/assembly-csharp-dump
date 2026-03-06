using System;
using Meta.WitAi.Composer.Integrations;

namespace Oculus.Voice.Composer
{
	public class AppComposerExperience : WitComposerService
	{
		public AppVoiceExperience AppVoiceExperience
		{
			get
			{
				return (AppVoiceExperience)base.VoiceService;
			}
		}
	}
}
