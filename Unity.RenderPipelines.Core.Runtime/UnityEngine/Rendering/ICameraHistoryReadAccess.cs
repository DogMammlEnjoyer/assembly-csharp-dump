using System;

namespace UnityEngine.Rendering
{
	public interface ICameraHistoryReadAccess
	{
		Type GetHistoryForRead<Type>() where Type : ContextItem;

		event ICameraHistoryReadAccess.HistoryRequestDelegate OnGatherHistoryRequests;

		public delegate void HistoryRequestDelegate(IPerFrameHistoryAccessTracker historyAccess);
	}
}
