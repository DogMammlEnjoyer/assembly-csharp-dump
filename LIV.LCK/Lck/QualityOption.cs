using System;
using UnityEngine.Serialization;

namespace Liv.Lck
{
	[Serializable]
	public struct QualityOption
	{
		public QualityOption(string name, bool isDefault, CameraTrackDescriptor recordingCameraTrackDescriptor, CameraTrackDescriptor streamingCameraTrackDescriptor)
		{
			this.Name = name;
			this.IsDefault = isDefault;
			this.RecordingCameraTrackDescriptor = recordingCameraTrackDescriptor;
			this.StreamingCameraTrackDescriptor = streamingCameraTrackDescriptor;
		}

		[Obsolete("Provides the RecordingCameraTrackDescriptor and only exists for backwards compability - Use RecordingCameraTrackDescriptor or StreamingCameraTrackDescriptor instead")]
		public CameraTrackDescriptor CameraTrackDescriptor
		{
			get
			{
				return this.RecordingCameraTrackDescriptor;
			}
		}

		public string Name;

		public bool IsDefault;

		[FormerlySerializedAs("CameraTrackDescriptor")]
		public CameraTrackDescriptor RecordingCameraTrackDescriptor;

		public CameraTrackDescriptor StreamingCameraTrackDescriptor;
	}
}
