using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
	internal interface IAsyncOperation
	{
		object GetResultAsObject();

		Type ResultType { get; }

		int Version { get; }

		string DebugName { get; }

		void DecrementReferenceCount();

		void IncrementReferenceCount();

		int ReferenceCount { get; }

		float PercentComplete { get; }

		DownloadStatus GetDownloadStatus(HashSet<object> visited);

		AsyncOperationStatus Status { get; }

		Exception OperationException { get; }

		bool IsDone { get; }

		Action<IAsyncOperation> OnDestroy { set; }

		void GetDependencies(List<AsyncOperationHandle> deps);

		bool IsRunning { get; }

		event Action<AsyncOperationHandle> CompletedTypeless;

		event Action<AsyncOperationHandle> Destroyed;

		void InvokeCompletionEvent();

		Task<object> Task { get; }

		void Start(ResourceManager rm, AsyncOperationHandle dependency, DelegateList<float> updateCallbacks);

		AsyncOperationHandle Handle { get; }

		void WaitForCompletion();
	}
}
