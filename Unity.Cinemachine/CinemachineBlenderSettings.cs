using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineBlending.html")]
	[Serializable]
	public sealed class CinemachineBlenderSettings : ScriptableObject
	{
		public CinemachineBlendDefinition GetBlendForVirtualCameras(string fromCameraName, string toCameraName, CinemachineBlendDefinition defaultBlend)
		{
			bool flag = false;
			bool flag2 = false;
			CinemachineBlendDefinition result = defaultBlend;
			CinemachineBlendDefinition result2 = defaultBlend;
			if (this.CustomBlends != null)
			{
				for (int i = 0; i < this.CustomBlends.Length; i++)
				{
					CinemachineBlenderSettings.CustomBlend customBlend = this.CustomBlends[i];
					if (customBlend.From == fromCameraName && customBlend.To == toCameraName)
					{
						return customBlend.Blend;
					}
					if (customBlend.From == "**ANY CAMERA**")
					{
						if (!string.IsNullOrEmpty(toCameraName) && customBlend.To == toCameraName)
						{
							if (!flag)
							{
								result = customBlend.Blend;
							}
							flag = true;
						}
						else if (customBlend.To == "**ANY CAMERA**")
						{
							defaultBlend = customBlend.Blend;
						}
					}
					else if (customBlend.To == "**ANY CAMERA**" && !string.IsNullOrEmpty(fromCameraName) && customBlend.From == fromCameraName)
					{
						if (!flag2)
						{
							result2 = customBlend.Blend;
						}
						flag2 = true;
					}
				}
			}
			if (flag)
			{
				return result;
			}
			if (flag2)
			{
				return result2;
			}
			return defaultBlend;
		}

		public static CinemachineBlendDefinition LookupBlend(ICinemachineCamera outgoing, ICinemachineCamera incoming, CinemachineBlendDefinition defaultBlend, CinemachineBlenderSettings customBlends, Object owner)
		{
			CinemachineBlendDefinition cinemachineBlendDefinition = defaultBlend;
			if (customBlends != null)
			{
				string fromCameraName = (outgoing != null) ? outgoing.Name : string.Empty;
				string toCameraName = (incoming != null) ? incoming.Name : string.Empty;
				cinemachineBlendDefinition = customBlends.GetBlendForVirtualCameras(fromCameraName, toCameraName, cinemachineBlendDefinition);
			}
			if (CinemachineCore.GetBlendOverride != null)
			{
				cinemachineBlendDefinition = CinemachineCore.GetBlendOverride(outgoing, incoming, cinemachineBlendDefinition, owner);
			}
			return cinemachineBlendDefinition;
		}

		[Tooltip("The array containing explicitly defined blends between two Virtual Cameras")]
		[FormerlySerializedAs("m_CustomBlends")]
		public CinemachineBlenderSettings.CustomBlend[] CustomBlends;

		internal const string kBlendFromAnyCameraLabel = "**ANY CAMERA**";

		[Serializable]
		public struct CustomBlend
		{
			[Tooltip("When blending from a camera with this name")]
			[FormerlySerializedAs("m_From")]
			public string From;

			[Tooltip("When blending to a camera with this name")]
			[FormerlySerializedAs("m_To")]
			public string To;

			[Tooltip("Blend curve definition")]
			[FormerlySerializedAs("m_Blend")]
			public CinemachineBlendDefinition Blend;
		}
	}
}
