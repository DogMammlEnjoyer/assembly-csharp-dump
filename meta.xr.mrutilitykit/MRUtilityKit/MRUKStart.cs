using System;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.MRUtilityKit
{
	[Obsolete("This class is now obsolete, please register events directly with the MRUK class", true)]
	[Feature(Feature.Scene)]
	public class MRUKStart : MonoBehaviour
	{
		private void Start()
		{
			if (!MRUK.Instance)
			{
				Debug.LogWarning("Couldn't find instance of MRUK");
				return;
			}
			MRUK.Instance.RegisterSceneLoadedCallback(delegate
			{
				UnityEvent unityEvent = this.sceneLoadedEvent;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke();
			});
			MRUK.Instance.RegisterRoomCreatedCallback(delegate(MRUKRoom room)
			{
				UnityEvent<MRUKRoom> unityEvent = this.roomCreatedEvent;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(room);
			});
			MRUK.Instance.RegisterRoomRemovedCallback(delegate(MRUKRoom room)
			{
				UnityEvent<MRUKRoom> unityEvent = this.roomRemovedEvent;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(room);
			});
			MRUK.Instance.RegisterRoomUpdatedCallback(delegate(MRUKRoom room)
			{
				UnityEvent<MRUKRoom> unityEvent = this.roomUpdatedEvent;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(room);
			});
		}

		public UnityEvent sceneLoadedEvent = new UnityEvent();

		public UnityEvent<MRUKRoom> roomCreatedEvent = new UnityEvent<MRUKRoom>();

		public UnityEvent<MRUKRoom> roomUpdatedEvent = new UnityEvent<MRUKRoom>();

		public UnityEvent<MRUKRoom> roomRemovedEvent = new UnityEvent<MRUKRoom>();
	}
}
