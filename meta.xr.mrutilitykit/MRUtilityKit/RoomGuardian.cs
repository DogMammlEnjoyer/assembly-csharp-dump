using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{
	[HelpURL("https://developers.meta.com/horizon/reference/mruk/latest/class_meta_x_r_m_r_utility_kit_room_guardian")]
	[Feature(Feature.Scene)]
	public class RoomGuardian : MonoBehaviour
	{
		private void Start()
		{
			OVRPlugin.eyeFovPremultipliedAlphaModeEnabled = false;
			OVRTelemetry.Start(651901100, 0, -1L).Send();
		}

		private void Update()
		{
			if (this.GuardianMaterial == null)
			{
				return;
			}
			MRUK instance = MRUK.Instance;
			MRUKRoom mrukroom = (instance != null) ? instance.GetCurrentRoom() : null;
			if (!mrukroom)
			{
				return;
			}
			bool flag = mrukroom.IsPositionInRoom(Camera.main.transform.position, true);
			Vector3 vector = new Vector3(Camera.main.transform.position.x, 0.2f, Camera.main.transform.position.z);
			Vector3 end;
			MRUKAnchor mrukanchor;
			float num = mrukroom.TryGetClosestSurfacePosition(vector, out end, out mrukanchor, new LabelFilter(new MRUKAnchor.SceneLabels?(~(MRUKAnchor.SceneLabels.FLOOR | MRUKAnchor.SceneLabels.CEILING)), null));
			bool flag2 = !mrukroom.IsPositionInSceneVolume(vector, 0f);
			float value = (flag && flag2) ? Mathf.Clamp01(1f - num / this.GuardianDistance) : 1f;
			this.GuardianMaterial.SetFloat("_GuardianFade", value);
			Color color = flag ? Color.green : Color.red;
			Debug.DrawLine(vector, end, color);
		}

		[Tooltip("Material to use for the Guardian effect")]
		public Material GuardianMaterial;

		[Tooltip("This is how far, in meters, the player must be form a surface for the Guardian to become visible (in other words, it blends `_GuardianFade` from 0 to 1). The position of the user is calculated as a point 0.2m above the ground. This is to catch tripping hazards, as well as walls.")]
		public float GuardianDistance = 1f;
	}
}
