using System;

namespace Oculus.Interaction.Input
{
	public interface IDataSource<TData> : IDataSource
	{
		TData GetData();
	}
}
