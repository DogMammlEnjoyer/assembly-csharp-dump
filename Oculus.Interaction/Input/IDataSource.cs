using System;

namespace Oculus.Interaction.Input
{
	public interface IDataSource
	{
		int CurrentDataVersion { get; }

		void MarkInputDataRequiresUpdate();

		event Action InputDataAvailable;
	}
}
