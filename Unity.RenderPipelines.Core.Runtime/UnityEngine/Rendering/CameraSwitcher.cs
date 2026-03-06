using System;

namespace UnityEngine.Rendering
{
	public class CameraSwitcher : MonoBehaviour
	{
		private void OnEnable()
		{
			this.m_OriginalCamera = base.GetComponent<Camera>();
			this.m_CurrentCamera = this.m_OriginalCamera;
			if (this.m_OriginalCamera == null)
			{
				Debug.LogError("Camera Switcher needs a Camera component attached");
				return;
			}
			this.m_CurrentCameraIndex = this.GetCameraCount() - 1;
			this.m_CameraNames = new GUIContent[this.GetCameraCount()];
			this.m_CameraIndices = new int[this.GetCameraCount()];
			for (int i = 0; i < this.m_Cameras.Length; i++)
			{
				Camera camera = this.m_Cameras[i];
				if (camera != null)
				{
					this.m_CameraNames[i] = new GUIContent(camera.name);
				}
				else
				{
					this.m_CameraNames[i] = new GUIContent("null");
				}
				this.m_CameraIndices[i] = i;
			}
			this.m_CameraNames[this.GetCameraCount() - 1] = new GUIContent("Original Camera");
			this.m_CameraIndices[this.GetCameraCount() - 1] = this.GetCameraCount() - 1;
			this.m_DebugEntry = new DebugUI.EnumField
			{
				displayName = "Camera Switcher",
				getter = (() => this.m_CurrentCameraIndex),
				setter = delegate(int value)
				{
					this.SetCameraIndex(value);
				},
				enumNames = this.m_CameraNames,
				enumValues = this.m_CameraIndices,
				getIndex = (() => this.m_DebugEntryEnumIndex),
				setIndex = delegate(int value)
				{
					this.m_DebugEntryEnumIndex = value;
				}
			};
			DebugManager.instance.GetPanel("Camera", true, 0, false).children.Add(this.m_DebugEntry);
		}

		private void OnDisable()
		{
			if (this.m_DebugEntry != null && this.m_DebugEntry.panel != null)
			{
				this.m_DebugEntry.panel.children.Remove(this.m_DebugEntry);
			}
		}

		private int GetCameraCount()
		{
			return this.m_Cameras.Length + 1;
		}

		private Camera GetNextCamera()
		{
			if (this.m_CurrentCameraIndex == this.m_Cameras.Length)
			{
				return this.m_OriginalCamera;
			}
			return this.m_Cameras[this.m_CurrentCameraIndex];
		}

		private void SetCameraIndex(int index)
		{
			if (index > 0 && index < this.GetCameraCount())
			{
				this.m_CurrentCameraIndex = index;
				if (this.m_CurrentCamera == this.m_OriginalCamera)
				{
					this.m_OriginalCameraPosition = this.m_OriginalCamera.transform.position;
					this.m_OriginalCameraRotation = this.m_OriginalCamera.transform.rotation;
				}
				this.m_CurrentCamera = this.GetNextCamera();
				if (this.m_CurrentCamera != null)
				{
					if (this.m_CurrentCamera == this.m_OriginalCamera)
					{
						this.m_OriginalCamera.transform.SetPositionAndRotation(this.m_OriginalCameraPosition, this.m_OriginalCameraRotation);
					}
					base.transform.SetPositionAndRotation(this.m_CurrentCamera.transform.position, this.m_CurrentCamera.transform.rotation);
				}
			}
		}

		public Camera[] m_Cameras;

		private int m_CurrentCameraIndex = -1;

		private Camera m_OriginalCamera;

		private Vector3 m_OriginalCameraPosition;

		private Quaternion m_OriginalCameraRotation;

		private Camera m_CurrentCamera;

		private GUIContent[] m_CameraNames;

		private int[] m_CameraIndices;

		private DebugUI.EnumField m_DebugEntry;

		private int m_DebugEntryEnumIndex;
	}
}
