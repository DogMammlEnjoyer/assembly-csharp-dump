using System;

namespace Oculus.Voice.Core.Bindings.Interfaces
{
	public interface IConnection
	{
		void Connect(string version);

		void Disconnect();

		bool IsConnected { get; }
	}
}
