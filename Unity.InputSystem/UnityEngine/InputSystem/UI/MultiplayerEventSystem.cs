using System;
using UnityEngine.EventSystems;

namespace UnityEngine.InputSystem.UI
{
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/UISupport.html#multiplayer-uis")]
	public class MultiplayerEventSystem : EventSystem
	{
		public GameObject playerRoot
		{
			get
			{
				return this.m_PlayerRoot;
			}
			set
			{
				this.m_PlayerRoot = value;
				this.InitializePlayerRoot();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.InitializePlayerRoot();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		private void InitializePlayerRoot()
		{
			if (this.m_PlayerRoot == null)
			{
				return;
			}
			InputSystemUIInputModule component = base.GetComponent<InputSystemUIInputModule>();
			if (component != null)
			{
				component.localMultiPlayerRoot = this.m_PlayerRoot;
			}
		}

		protected override void Update()
		{
			EventSystem current = EventSystem.current;
			EventSystem.current = this;
			try
			{
				base.Update();
			}
			finally
			{
				EventSystem.current = current;
			}
		}

		[Tooltip("If set, only process mouse and navigation events for any game objects which are children of this game object.")]
		[SerializeField]
		private GameObject m_PlayerRoot;
	}
}
