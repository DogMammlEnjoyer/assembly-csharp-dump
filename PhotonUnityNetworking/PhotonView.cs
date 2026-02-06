using System;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace Photon.Pun
{
	[AddComponentMenu("Photon Networking/Photon View")]
	public class PhotonView : MonoBehaviour
	{
		public int Prefix
		{
			get
			{
				if (this.prefixField == -1 && PhotonNetwork.NetworkingClient != null)
				{
					this.prefixField = (int)PhotonNetwork.currentLevelPrefix;
				}
				return this.prefixField;
			}
			set
			{
				this.prefixField = value;
			}
		}

		public object[] InstantiationData
		{
			get
			{
				return this.instantiationDataField;
			}
			protected internal set
			{
				this.instantiationDataField = value;
			}
		}

		[Obsolete("Renamed. Use IsRoomView instead")]
		public bool IsSceneView
		{
			get
			{
				return this.IsRoomView;
			}
		}

		public bool IsRoomView
		{
			get
			{
				return this.CreatorActorNr == 0;
			}
		}

		public bool IsOwnerActive
		{
			get
			{
				return this.Owner != null && !this.Owner.IsInactive;
			}
		}

		public bool IsMine { get; private set; }

		public bool AmController
		{
			get
			{
				return this.IsMine;
			}
		}

		public Player Controller { get; private set; }

		public int CreatorActorNr { get; private set; }

		public bool AmOwner { get; private set; }

		public Player Owner { get; private set; }

		public int OwnerActorNr
		{
			get
			{
				return this.ownerActorNr;
			}
			set
			{
				if (value != 0 && this.ownerActorNr == value)
				{
					return;
				}
				Player owner = this.Owner;
				this.Owner = ((PhotonNetwork.CurrentRoom == null) ? null : PhotonNetwork.CurrentRoom.GetPlayer(value, true));
				this.ownerActorNr = ((this.Owner != null) ? this.Owner.ActorNumber : value);
				this.AmOwner = (PhotonNetwork.LocalPlayer != null && this.ownerActorNr == PhotonNetwork.LocalPlayer.ActorNumber);
				this.UpdateCallbackLists();
				if (this.OnOwnerChangeCallbacks != null)
				{
					int i = 0;
					int count = this.OnOwnerChangeCallbacks.Count;
					while (i < count)
					{
						this.OnOwnerChangeCallbacks[i].OnOwnerChange(this.Owner, owner);
						i++;
					}
				}
			}
		}

		public int ControllerActorNr
		{
			get
			{
				return this.controllerActorNr;
			}
			set
			{
				Player controller = this.Controller;
				this.Controller = ((PhotonNetwork.CurrentRoom == null) ? null : PhotonNetwork.CurrentRoom.GetPlayer(value, true));
				if (this.Controller != null && this.Controller.IsInactive)
				{
					this.Controller = PhotonNetwork.MasterClient;
				}
				this.controllerActorNr = ((this.Controller != null) ? this.Controller.ActorNumber : value);
				this.IsMine = (PhotonNetwork.LocalPlayer != null && this.controllerActorNr == PhotonNetwork.LocalPlayer.ActorNumber);
				if (this.Controller != controller)
				{
					this.UpdateCallbackLists();
					if (this.OnControllerChangeCallbacks != null)
					{
						int i = 0;
						int count = this.OnControllerChangeCallbacks.Count;
						while (i < count)
						{
							this.OnControllerChangeCallbacks[i].OnControllerChange(this.Controller, controller);
							i++;
						}
					}
				}
			}
		}

		public int ViewID
		{
			get
			{
				return this.viewIdField;
			}
			set
			{
				if (value != 0 && this.viewIdField != 0)
				{
					Debug.LogWarning("Changing a ViewID while it's in use is not possible (except setting it to 0 (not being used). Current ViewID: " + this.viewIdField.ToString());
					return;
				}
				if (value == 0 && this.viewIdField != 0)
				{
					PhotonNetwork.LocalCleanPhotonView(this);
				}
				this.viewIdField = value;
				this.CreatorActorNr = value / PhotonNetwork.MAX_VIEW_IDS;
				this.OwnerActorNr = this.CreatorActorNr;
				this.ControllerActorNr = this.CreatorActorNr;
				this.RebuildControllerCache(false);
				if (value != 0)
				{
					PhotonNetwork.RegisterPhotonView(this);
				}
			}
		}

		protected internal void Awake()
		{
			if (this.ViewID != 0)
			{
				return;
			}
			if (this.sceneViewId != 0)
			{
				this.ViewID = this.sceneViewId;
			}
			this.FindObservables(false);
		}

		internal void ResetPhotonView(bool resetOwner)
		{
			this.lastOnSerializeDataSent = null;
		}

		internal void RebuildControllerCache(bool ownerHasChanged = false)
		{
			if (this.controllerActorNr == 0 || this.OwnerActorNr == 0 || this.Owner == null || this.Owner.IsInactive)
			{
				Player masterClient = PhotonNetwork.MasterClient;
				int num = (masterClient == null) ? -1 : masterClient.ActorNumber;
				this.ControllerActorNr = num;
				this.OwnerActorNr = num;
				return;
			}
			this.ControllerActorNr = this.OwnerActorNr;
		}

		public void OnPreNetDestroy(PhotonView rootView)
		{
			this.UpdateCallbackLists();
			if (this.OnPreNetDestroyCallbacks != null)
			{
				int i = 0;
				int count = this.OnPreNetDestroyCallbacks.Count;
				while (i < count)
				{
					this.OnPreNetDestroyCallbacks[i].OnPreNetDestroy(rootView);
					i++;
				}
			}
		}

		protected internal void OnDestroy()
		{
			if (!this.removedFromLocalViewList && PhotonNetwork.LocalCleanPhotonView(this) && this.InstantiationId > 0 && !ConnectionHandler.AppQuits && PhotonNetwork.LogLevel >= PunLogLevel.Informational)
			{
				Debug.Log("PUN-instantiated '" + base.gameObject.name + "' got destroyed by engine. This is OK when loading levels. Otherwise use: PhotonNetwork.Destroy().");
			}
		}

		[Obsolete("Use RequestableOwnershipGuard")]
		public void RequestOwnership()
		{
		}

		[Obsolete("Use RequestableOwnershipGuard")]
		public void TransferOwnership(Player newOwner)
		{
		}

		[Obsolete("Use RequestableOwnershipGuard")]
		public void TransferOwnership(int newOwnerId)
		{
		}

		public void FindObservables(bool force = false)
		{
			if (!force && this.observableSearch == PhotonView.ObservableSearch.Manual)
			{
				return;
			}
			if (this.ObservedComponents == null)
			{
				this.ObservedComponents = new List<Component>();
			}
			else
			{
				this.ObservedComponents.Clear();
			}
			base.transform.GetNestedComponentsInChildren(force || this.observableSearch == PhotonView.ObservableSearch.AutoFindAll, this.ObservedComponents);
		}

		public void SerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (this.ObservedComponents != null && this.ObservedComponents.Count > 0)
			{
				for (int i = 0; i < this.ObservedComponents.Count; i++)
				{
					if (this.ObservedComponents[i] != null)
					{
						this.SerializeComponent(this.ObservedComponents[i], stream, info);
					}
				}
			}
		}

		public void DeserializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (this.ObservedComponents != null && this.ObservedComponents.Count > 0)
			{
				for (int i = 0; i < this.ObservedComponents.Count; i++)
				{
					Component component = this.ObservedComponents[i];
					if (component != null)
					{
						this.DeserializeComponent(component, stream, info);
					}
				}
			}
		}

		protected internal void DeserializeComponent(Component component, PhotonStream stream, PhotonMessageInfo info)
		{
			IPunObservable punObservable = component as IPunObservable;
			if (punObservable != null)
			{
				punObservable.OnPhotonSerializeView(stream, info);
				return;
			}
			string str = "Observed scripts have to implement IPunObservable. ";
			string str2 = (component != null) ? component.ToString() : null;
			string str3 = " does not. It is Type: ";
			Type type = component.GetType();
			Debug.LogError(str + str2 + str3 + ((type != null) ? type.ToString() : null), component.gameObject);
		}

		protected internal void SerializeComponent(Component component, PhotonStream stream, PhotonMessageInfo info)
		{
			IPunObservable punObservable = component as IPunObservable;
			if (punObservable != null)
			{
				punObservable.OnPhotonSerializeView(stream, info);
				return;
			}
			string str = "Observed scripts have to implement IPunObservable. ";
			string str2 = (component != null) ? component.ToString() : null;
			string str3 = " does not. It is Type: ";
			Type type = component.GetType();
			Debug.LogError(str + str2 + str3 + ((type != null) ? type.ToString() : null), component.gameObject);
		}

		public void RefreshRpcMonoBehaviourCache()
		{
			this.RpcMonoBehaviours = base.GetComponents<MonoBehaviour>();
		}

		public void RPC(string methodName, RpcTarget target, params object[] parameters)
		{
			PhotonNetwork.RPC(this, methodName, target, false, parameters);
		}

		public void RpcSecure(string methodName, RpcTarget target, bool encrypt, params object[] parameters)
		{
			PhotonNetwork.RPC(this, methodName, target, encrypt, parameters);
		}

		public void RPC(string methodName, Player targetPlayer, params object[] parameters)
		{
			PhotonNetwork.RPC(this, methodName, targetPlayer, false, parameters);
		}

		public void RpcSecure(string methodName, Player targetPlayer, bool encrypt, params object[] parameters)
		{
			PhotonNetwork.RPC(this, methodName, targetPlayer, encrypt, parameters);
		}

		public static PhotonView Get(Component component)
		{
			return component.transform.GetParentComponent<PhotonView>();
		}

		public static PhotonView Get(GameObject gameObj)
		{
			return gameObj.transform.GetParentComponent<PhotonView>();
		}

		public static PhotonView Find(int viewID)
		{
			return PhotonNetwork.GetPhotonView(viewID);
		}

		public void AddCallbackTarget(IPhotonViewCallback obj)
		{
			this.CallbackChangeQueue.Enqueue(new PhotonView.CallbackTargetChange(obj, null, true));
		}

		public void RemoveCallbackTarget(IPhotonViewCallback obj)
		{
			this.CallbackChangeQueue.Enqueue(new PhotonView.CallbackTargetChange(obj, null, false));
		}

		public void AddCallback<T>(IPhotonViewCallback obj) where T : class, IPhotonViewCallback
		{
			this.CallbackChangeQueue.Enqueue(new PhotonView.CallbackTargetChange(obj, typeof(T), true));
		}

		public void RemoveCallback<T>(IPhotonViewCallback obj) where T : class, IPhotonViewCallback
		{
			this.CallbackChangeQueue.Enqueue(new PhotonView.CallbackTargetChange(obj, typeof(T), false));
		}

		private void UpdateCallbackLists()
		{
			while (this.CallbackChangeQueue.Count > 0)
			{
				PhotonView.CallbackTargetChange callbackTargetChange = this.CallbackChangeQueue.Dequeue();
				IPhotonViewCallback obj = callbackTargetChange.obj;
				Type type = callbackTargetChange.type;
				bool add = callbackTargetChange.add;
				if (type == null)
				{
					this.TryRegisterCallback<IOnPhotonViewPreNetDestroy>(obj, ref this.OnPreNetDestroyCallbacks, add);
					this.TryRegisterCallback<IOnPhotonViewOwnerChange>(obj, ref this.OnOwnerChangeCallbacks, add);
					this.TryRegisterCallback<IOnPhotonViewControllerChange>(obj, ref this.OnControllerChangeCallbacks, add);
				}
				else if (type == typeof(IOnPhotonViewPreNetDestroy))
				{
					this.RegisterCallback<IOnPhotonViewPreNetDestroy>(obj as IOnPhotonViewPreNetDestroy, ref this.OnPreNetDestroyCallbacks, add);
				}
				else if (type == typeof(IOnPhotonViewOwnerChange))
				{
					this.RegisterCallback<IOnPhotonViewOwnerChange>(obj as IOnPhotonViewOwnerChange, ref this.OnOwnerChangeCallbacks, add);
				}
				else if (type == typeof(IOnPhotonViewControllerChange))
				{
					this.RegisterCallback<IOnPhotonViewControllerChange>(obj as IOnPhotonViewControllerChange, ref this.OnControllerChangeCallbacks, add);
				}
			}
		}

		private void TryRegisterCallback<T>(IPhotonViewCallback obj, ref List<T> list, bool add) where T : class, IPhotonViewCallback
		{
			T t = obj as T;
			if (t != null)
			{
				this.RegisterCallback<T>(t, ref list, add);
			}
		}

		private void RegisterCallback<T>(T obj, ref List<T> list, bool add) where T : class, IPhotonViewCallback
		{
			if (list == null)
			{
				list = new List<T>();
			}
			if (add)
			{
				if (!list.Contains(obj))
				{
					list.Add(obj);
					return;
				}
			}
			else if (list.Contains(obj))
			{
				list.Remove(obj);
			}
		}

		public override string ToString()
		{
			return string.Format("View {0}{3} on {1} {2}", new object[]
			{
				this.ViewID,
				(base.gameObject != null) ? base.gameObject.name : "GO==null",
				this.IsRoomView ? "(scene)" : string.Empty,
				(this.Prefix > 0) ? ("lvl" + this.Prefix.ToString()) : ""
			});
		}

		[FormerlySerializedAs("group")]
		public byte Group;

		[FormerlySerializedAs("prefixBackup")]
		public int prefixField = -1;

		internal object[] instantiationDataField;

		protected internal List<object> lastOnSerializeDataSent;

		protected internal List<object> syncValues;

		protected internal object[] lastOnSerializeDataReceived;

		[FormerlySerializedAs("synchronization")]
		public ViewSynchronization Synchronization = ViewSynchronization.UnreliableOnChange;

		protected internal bool mixedModeIsReliable;

		[FormerlySerializedAs("ownershipTransfer")]
		public OwnershipOption OwnershipTransfer;

		public PhotonView.ObservableSearch observableSearch;

		public List<Component> ObservedComponents;

		internal MonoBehaviour[] RpcMonoBehaviours;

		[NonSerialized]
		private int ownerActorNr;

		[NonSerialized]
		private int controllerActorNr;

		[SerializeField]
		[FormerlySerializedAs("viewIdField")]
		[HideInInspector]
		public int sceneViewId;

		[NonSerialized]
		private int viewIdField;

		[FormerlySerializedAs("instantiationId")]
		public int InstantiationId;

		[SerializeField]
		[HideInInspector]
		public bool isRuntimeInstantiated;

		protected internal bool removedFromLocalViewList;

		private Queue<PhotonView.CallbackTargetChange> CallbackChangeQueue = new Queue<PhotonView.CallbackTargetChange>();

		private List<IOnPhotonViewPreNetDestroy> OnPreNetDestroyCallbacks;

		private List<IOnPhotonViewOwnerChange> OnOwnerChangeCallbacks;

		private List<IOnPhotonViewControllerChange> OnControllerChangeCallbacks;

		public enum ObservableSearch
		{
			Manual,
			AutoFindActive,
			AutoFindAll
		}

		private struct CallbackTargetChange
		{
			public CallbackTargetChange(IPhotonViewCallback obj, Type type, bool add)
			{
				this.obj = obj;
				this.type = type;
				this.add = add;
			}

			public IPhotonViewCallback obj;

			public Type type;

			public bool add;
		}
	}
}
