using System;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	internal class ProxyInputModule
	{
		public PanelInputModule InputModule { get; private set; }

		public ProxyInputModule(GameObject owner, OVRCursor cursor)
		{
			this._cursor = cursor;
			this._owner = owner;
		}

		public bool Refresh()
		{
			if (this.InputModule != null && this.InputModule.isActiveAndEnabled)
			{
				return true;
			}
			this.SearchForEventSystem();
			return this.InputModule;
		}

		private void SearchForEventSystem()
		{
			EventSystem eventSystem = Object.FindAnyObjectByType<EventSystem>();
			if (!eventSystem && RuntimeSettings.Instance.CreateEventSystem)
			{
				eventSystem = this._owner.AddComponent<EventSystem>();
			}
			this.SetupEventSystem(eventSystem);
		}

		private void SetupEventSystem(EventSystem eventSystem)
		{
			this._eventSystem = eventSystem;
			if (!this._eventSystem)
			{
				return;
			}
			PanelInputModule inputModule = this._eventSystem.gameObject.AddComponent<PanelInputModule>();
			this._eventSystem.UpdateModules();
			this.SetupInputModule(inputModule);
		}

		private void SetupInputModule(PanelInputModule inputModule)
		{
			this.InputModule = inputModule;
			if (!this.InputModule)
			{
				return;
			}
			this.InputModule.SetDebugInterface(this._owner.GetComponent<Interface>());
			PanelInputModule inputModule2 = this.InputModule;
			if (inputModule2.m_Cursor == null)
			{
				inputModule2.m_Cursor = this._cursor;
			}
		}

		private readonly GameObject _owner;

		private readonly OVRCursor _cursor;

		private EventSystem _eventSystem;
	}
}
