using System;

namespace Meta.WitAi.Interfaces
{
	public interface ITranscriptionEventProvider
	{
		ITranscriptionEvent TranscriptionEvents { get; }
	}
}
