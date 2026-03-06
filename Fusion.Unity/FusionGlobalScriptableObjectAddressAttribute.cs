using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Scripting;

namespace Fusion
{
	[Preserve]
	public class FusionGlobalScriptableObjectAddressAttribute : FusionGlobalScriptableObjectSourceAttribute
	{
		public FusionGlobalScriptableObjectAddressAttribute(Type objectType, string address) : base(objectType)
		{
			this.Address = address;
		}

		public string Address { get; }

		public override FusionGlobalScriptableObjectLoadResult Load(Type type)
		{
			AsyncOperationHandle<FusionGlobalScriptableObject> op = Addressables.LoadAssetAsync<FusionGlobalScriptableObject>(this.Address);
			FusionGlobalScriptableObject obj = op.WaitForCompletion();
			if (op.Status == AsyncOperationStatus.Succeeded)
			{
				return new FusionGlobalScriptableObjectLoadResult(obj, delegate(FusionGlobalScriptableObject x)
				{
					Addressables.Release<FusionGlobalScriptableObject>(op);
				});
			}
			return default(FusionGlobalScriptableObjectLoadResult);
		}
	}
}
