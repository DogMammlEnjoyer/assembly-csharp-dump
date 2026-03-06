using System;
using Fusion;
using Meta.XR.MultiplayerBlocks.Shared;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Fusion
{
	[NetworkBehaviourWeaved(904)]
	public class AvatarBehaviourFusion : NetworkBehaviour, IAvatarBehaviour
	{
		[Networked]
		[OnChangedRender("OnAvatarIdChanged")]
		[NetworkedWeaved(0, 2)]
		public unsafe ulong OculusId
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing AvatarBehaviourFusion.OculusId. Networked properties can only be accessed when Spawned() has been called.");
				}
				return (ulong)(*(long*)(this.Ptr + 0));
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing AvatarBehaviourFusion.OculusId. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(long*)(this.Ptr + 0) = (long)value;
			}
		}

		[Networked]
		[OnChangedRender("OnAvatarIdChanged")]
		[NetworkedWeaved(2, 1)]
		public unsafe int LocalAvatarIndex
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing AvatarBehaviourFusion.LocalAvatarIndex. Networked properties can only be accessed when Spawned() has been called.");
				}
				return this.Ptr[2];
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing AvatarBehaviourFusion.LocalAvatarIndex. Networked properties can only be accessed when Spawned() has been called.");
				}
				this.Ptr[2] = value;
			}
		}

		[Networked]
		[NetworkedWeaved(3, 1)]
		private unsafe int AvatarDataStreamLength
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing AvatarBehaviourFusion.AvatarDataStreamLength. Networked properties can only be accessed when Spawned() has been called.");
				}
				return this.Ptr[3];
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing AvatarBehaviourFusion.AvatarDataStreamLength. Networked properties can only be accessed when Spawned() has been called.");
				}
				this.Ptr[3] = value;
			}
		}

		[Networked]
		[Capacity(900)]
		[OnChangedRender("OnAvatarDataStreamChanged")]
		[NetworkedWeaved(4, 900)]
		[NetworkedWeavedArray(900, 1, typeof(ElementReaderWriterByte))]
		private unsafe NetworkArray<byte> AvatarDataStream
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing AvatarBehaviourFusion.AvatarDataStream. Networked properties can only be accessed when Spawned() has been called.");
				}
				return new NetworkArray<byte>((byte*)(this.Ptr + 4), 900, ElementReaderWriterByte.GetInstance());
			}
		}

		public override void Spawned()
		{
			if (OVRManager.instance)
			{
				this._cameraRig = OVRManager.instance.GetComponentInChildren<OVRCameraRig>().transform;
			}
		}

		private void OnAvatarIdChanged()
		{
		}

		private void OnAvatarDataStreamChanged()
		{
		}

		public override void FixedUpdateNetwork()
		{
			if (!base.Object.HasStateAuthority)
			{
				return;
			}
			if (this._cameraRig == null)
			{
				return;
			}
			Transform transform = base.transform;
			base.transform.position = Vector3.Lerp(transform.position, this._cameraRig.position, 0.5f);
			base.transform.rotation = Quaternion.Lerp(transform.rotation, this._cameraRig.rotation, 0.5f);
		}

		public void ReceiveStreamData(byte[] bytes)
		{
			if (bytes.Length > 900)
			{
				Debug.LogError(string.Format("[{0}] Cannot send a stream data of length {1} greater than the max capacity of {2}", "AvatarBehaviourFusion", bytes.Length, 900));
				return;
			}
			this.AvatarDataStreamLength = bytes.Length;
			this.AvatarDataStream.CopyFrom(bytes, 0, bytes.Length);
		}

		bool IAvatarBehaviour.get_HasInputAuthority()
		{
			return base.HasInputAuthority;
		}

		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			this.OculusId = this._OculusId;
			this.LocalAvatarIndex = this._LocalAvatarIndex;
			this.AvatarDataStreamLength = this._AvatarDataStreamLength;
			NetworkBehaviourUtils.InitializeNetworkArray<byte>(this.AvatarDataStream, this._AvatarDataStream, "AvatarDataStream");
		}

		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			this._OculusId = this.OculusId;
			this._LocalAvatarIndex = this.LocalAvatarIndex;
			this._AvatarDataStreamLength = this.AvatarDataStreamLength;
			NetworkBehaviourUtils.CopyFromNetworkArray<byte>(this.AvatarDataStream, ref this._AvatarDataStream);
		}

		private const float LERP_TIME = 0.5f;

		private const int AvatarDataStreamMaxCapacity = 900;

		private Transform _cameraRig;

		private byte[] _buffer;

		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("OculusId", 0, 2)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private ulong _OculusId;

		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("LocalAvatarIndex", 2, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private int _LocalAvatarIndex;

		[WeaverGenerated]
		[DefaultForProperty("AvatarDataStreamLength", 3, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private int _AvatarDataStreamLength;

		[WeaverGenerated]
		[DefaultForProperty("AvatarDataStream", 4, 900)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private byte[] _AvatarDataStream;
	}
}
