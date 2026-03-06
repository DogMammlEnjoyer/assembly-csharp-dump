using System;
using UnityEngine;

namespace Pathfinding
{
	public abstract class VersionedMonoBehaviour : MonoBehaviour, ISerializationCallbackReceiver, IVersionedMonoBehaviourInternal
	{
		protected virtual void Awake()
		{
			if (Application.isPlaying)
			{
				this.version = this.OnUpgradeSerializedData(int.MaxValue, true);
			}
		}

		protected virtual void Reset()
		{
			this.version = this.OnUpgradeSerializedData(int.MaxValue, true);
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			int num = this.OnUpgradeSerializedData(this.version, false);
			if (num >= 0)
			{
				this.version = num;
			}
		}

		protected virtual int OnUpgradeSerializedData(int version, bool unityThread)
		{
			return 1;
		}

		void IVersionedMonoBehaviourInternal.UpgradeFromUnityThread()
		{
			int num = this.OnUpgradeSerializedData(this.version, true);
			if (num < 0)
			{
				throw new Exception();
			}
			this.version = num;
		}

		[SerializeField]
		[HideInInspector]
		private int version;
	}
}
