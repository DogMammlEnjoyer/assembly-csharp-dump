using System;
using System.Linq;
using System.Reflection;

namespace Unity.XR.OpenVR
{
	public class OpenVRHelpers
	{
		public static bool IsUsingSteamVRInput()
		{
			return OpenVRHelpers.DoesTypeExist("SteamVR_Input", false);
		}

		public static bool DoesTypeExist(string className, bool fullname = false)
		{
			return OpenVRHelpers.GetType(className, fullname) != null;
		}

		public static Type GetType(string className, bool fullname = false)
		{
			Type result;
			if (fullname)
			{
				result = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
				from type in assembly.GetTypes()
				where type.FullName == className
				select type).FirstOrDefault<Type>();
			}
			else
			{
				result = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
				from type in assembly.GetTypes()
				where type.Name == className
				select type).FirstOrDefault<Type>();
			}
			return result;
		}

		public static string GetActionManifestPathFromPlugin()
		{
			return (string)OpenVRHelpers.GetType("SteamVR_Input", false).GetMethod("GetActionsFilePath").Invoke(null, new object[]
			{
				false
			});
		}

		public static string GetActionManifestNameFromPlugin()
		{
			return (string)OpenVRHelpers.GetType("SteamVR_Input", false).GetMethod("GetActionsFileName").Invoke(null, null);
		}

		public static string GetEditorAppKeyFromPlugin()
		{
			return (string)OpenVRHelpers.GetType("SteamVR_Input", false).GetMethod("GetEditorAppKey").Invoke(null, null);
		}
	}
}
