using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Android
{
	[StaticAccessor("GameStateHelper::Get()", StaticAccessorType.Dot)]
	[NativeHeader("Modules/AndroidJNI/Public/GameStateHelper.h")]
	public static class AndroidGame
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StartLoading(int label);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StopLoading(int label);

		private static AndroidJavaObject GetUnityGameManager()
		{
			bool flag = AndroidGame.m_UnityGameManager != null;
			AndroidJavaObject unityGameManager;
			if (flag)
			{
				unityGameManager = AndroidGame.m_UnityGameManager;
			}
			else
			{
				AndroidGame.m_UnityGameManager = new AndroidJavaClass("com.unity3d.player.UnityGameManager");
				unityGameManager = AndroidGame.m_UnityGameManager;
			}
			return unityGameManager;
		}

		private static AndroidJavaObject GetUnityGameState()
		{
			bool flag = AndroidGame.m_UnityGameState != null;
			AndroidJavaObject unityGameState;
			if (flag)
			{
				unityGameState = AndroidGame.m_UnityGameState;
			}
			else
			{
				AndroidGame.m_UnityGameState = new AndroidJavaClass("com.unity3d.player.UnityGameState");
				unityGameState = AndroidGame.m_UnityGameState;
			}
			return unityGameState;
		}

		public static AndroidGameMode GameMode
		{
			get
			{
				return AndroidGameMode.Unsupported;
			}
		}

		public static void SetGameState(bool isLoading, AndroidGameState gameState)
		{
		}

		public static void SetGameState(bool isLoading, AndroidGameState gameState, int label, int quality)
		{
		}

		private static AndroidJavaObject m_UnityGameManager;

		private static AndroidJavaObject m_UnityGameState;

		[StaticAccessor("GameStateHelper::Get()", StaticAccessorType.Dot)]
		public static class Automatic
		{
			[NativeMethod("SetGameStateMode")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetGameState(AndroidGameState mode);
		}
	}
}
