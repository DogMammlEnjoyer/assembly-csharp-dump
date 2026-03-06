using System;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.Samples
{
	public class MRPassthrough : MonoBehaviour
	{
		protected virtual void Reset()
		{
			this._layer = Object.FindFirstObjectByType<OVRPassthroughLayer>();
			this._camera = OVRManager.FindMainCamera();
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.ValidatePassthrough();
			}
		}

		private void ValidatePassthrough()
		{
			if (OVRManager.HasInsightPassthroughInitFailed())
			{
				this._camera.clearFlags = CameraClearFlags.Skybox;
				this._passThroughToggle.enabled = false;
				return;
			}
			if (MRPassthrough.PassThrough.IsPassThroughOn)
			{
				this.TurnPassThroughOn();
				return;
			}
			this.TurnPassThroughOff();
			if (MRPassthrough.PassThrough.IsPassThroughCompatible)
			{
				this._passThroughToggle.enabled = false;
				return;
			}
			this._passThroughToggle.enabled = true;
		}

		[Obsolete]
		public void TurnLocoMotionSceneOn()
		{
			MRPassthrough.PassThrough.IsPassThroughCompatible = true;
		}

		[Obsolete]
		public void TurnLocoMotionSceneOff()
		{
			MRPassthrough.PassThrough.IsPassThroughCompatible = false;
		}

		public void TogglePassThrough()
		{
			if (MRPassthrough.PassThrough.IsPassThroughOn)
			{
				this.TurnPassThroughOff();
				return;
			}
			this.TurnPassThroughOn();
		}

		public void CheckPassthroughToggle()
		{
			if (this._passThroughToggle.enabled && MRPassthrough.PassThrough.IsPassThroughOn)
			{
				this._passThroughToggle.isOn = true;
				this.TurnPassThroughOn();
			}
		}

		private void TurnPassThroughOn()
		{
			if (OVRManager.IsInsightPassthroughInitialized())
			{
				MRPassthrough.PassThrough.IsPassThroughOn = true;
				this._layer.textureOpacity = 1f;
				this._camera.clearFlags = CameraClearFlags.Color;
				GameObject[] objects = this._objects;
				for (int i = 0; i < objects.Length; i++)
				{
					objects[i].SetActive(false);
				}
				return;
			}
			Debug.LogError("Failed to initialize Passthrough please check the OVRManager in the Hierarchy and check if Passthrough is supported and enabled.");
		}

		private void TurnPassThroughOff()
		{
			MRPassthrough.PassThrough.IsPassThroughOn = false;
			this._layer.textureOpacity = 0f;
			this._camera.clearFlags = CameraClearFlags.Skybox;
			GameObject[] objects = this._objects;
			for (int i = 0; i < objects.Length; i++)
			{
				objects[i].SetActive(true);
			}
		}

		public void InjectAllMRPassthrough(GameObject[] objects, Toggle passThroughToggle, OVRPassthroughLayer layer, Camera camera)
		{
			this.InjectObjects(objects);
			this.InjectPassThroughToggle(passThroughToggle);
			this.InjectLayer(layer);
			this.InjectCamera(camera);
		}

		public void InjectObjects(GameObject[] objects)
		{
			this._objects = objects;
		}

		public void InjectPassThroughToggle(Toggle passThroughToggle)
		{
			this._passThroughToggle = passThroughToggle;
		}

		public void InjectLayer(OVRPassthroughLayer layer)
		{
			this._layer = layer;
		}

		public void InjectCamera(Camera camera)
		{
			this._camera = camera;
		}

		[Tooltip("Objects that shouldn't be rendered during passthrough")]
		[Header("Passthrough Objects To Remove")]
		[SerializeField]
		private GameObject[] _objects;

		[Tooltip("These are UI objects that should be toggled ON/OFF during passthrough button")]
		[SerializeField]
		private Toggle _passThroughToggle;

		[Tooltip("The OVRPassthrough Layer")]
		[SerializeField]
		private OVRPassthroughLayer _layer;

		[Tooltip("Use the CenterEyeAnchor or Center Camera")]
		[SerializeField]
		private Camera _camera;

		protected bool _started;

		public static class PassThrough
		{
			public static bool IsPassThroughOn;

			public static bool IsPassThroughCompatible;
		}
	}
}
