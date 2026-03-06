using System;
using System.Threading.Tasks;

namespace Meta.WitAi.Requests
{
	internal interface IVRequestDownloadDecoder
	{
		event VRequestResponseDelegate OnFirstResponse;

		event VRequestResponseDelegate OnResponse;

		event VRequestProgressDelegate OnProgress;

		TaskCompletionSource<bool> Completion { get; }
	}
}
