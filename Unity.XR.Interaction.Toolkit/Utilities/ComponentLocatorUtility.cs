using System;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	internal static class ComponentLocatorUtility<T> where T : Component
	{
		internal static T componentCache
		{
			get
			{
				return ComponentLocatorUtility<T>.s_ComponentCache;
			}
		}

		private static bool FindWasPerformedThisFrame()
		{
			return ComponentLocatorUtility<T>.s_LastTryFindFrame == Time.frameCount;
		}

		public static T FindOrCreateComponent()
		{
			if (ComponentLocatorUtility<T>.s_ComponentCache == null)
			{
				ComponentLocatorUtility<T>.s_ComponentCache = ComponentLocatorUtility<T>.Find();
				if (ComponentLocatorUtility<T>.s_ComponentCache == null)
				{
					ComponentLocatorUtility<T>.s_ComponentCache = new GameObject(typeof(T).Name, new Type[]
					{
						typeof(T)
					}).GetComponent<T>();
				}
			}
			return ComponentLocatorUtility<T>.s_ComponentCache;
		}

		public static T FindComponent()
		{
			T result;
			ComponentLocatorUtility<T>.TryFindComponent(out result);
			return result;
		}

		public static bool TryFindComponent(out T component)
		{
			if (ComponentLocatorUtility<T>.s_ComponentCache != null)
			{
				component = ComponentLocatorUtility<T>.s_ComponentCache;
				return true;
			}
			ComponentLocatorUtility<T>.s_ComponentCache = ComponentLocatorUtility<T>.Find();
			component = ComponentLocatorUtility<T>.s_ComponentCache;
			return component != null;
		}

		internal static bool TryFindComponent(out T component, bool limitTryFindPerFrame)
		{
			if (limitTryFindPerFrame && ComponentLocatorUtility<T>.FindWasPerformedThisFrame() && ComponentLocatorUtility<T>.s_ComponentCache == null)
			{
				component = default(T);
				return false;
			}
			return ComponentLocatorUtility<T>.TryFindComponent(out component);
		}

		private static T Find()
		{
			ComponentLocatorUtility<T>.s_LastTryFindFrame = Time.frameCount;
			return Object.FindFirstObjectByType<T>();
		}

		private static T s_ComponentCache;

		private static int s_LastTryFindFrame = -1;
	}
}
