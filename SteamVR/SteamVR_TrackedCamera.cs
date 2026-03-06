using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_TrackedCamera
	{
		public static SteamVR_TrackedCamera.VideoStreamTexture Distorted(int deviceIndex = 0)
		{
			if (SteamVR_TrackedCamera.distorted == null)
			{
				SteamVR_TrackedCamera.distorted = new SteamVR_TrackedCamera.VideoStreamTexture[64];
			}
			if (SteamVR_TrackedCamera.distorted[deviceIndex] == null)
			{
				SteamVR_TrackedCamera.distorted[deviceIndex] = new SteamVR_TrackedCamera.VideoStreamTexture((uint)deviceIndex, false);
			}
			return SteamVR_TrackedCamera.distorted[deviceIndex];
		}

		public static SteamVR_TrackedCamera.VideoStreamTexture Undistorted(int deviceIndex = 0)
		{
			if (SteamVR_TrackedCamera.undistorted == null)
			{
				SteamVR_TrackedCamera.undistorted = new SteamVR_TrackedCamera.VideoStreamTexture[64];
			}
			if (SteamVR_TrackedCamera.undistorted[deviceIndex] == null)
			{
				SteamVR_TrackedCamera.undistorted[deviceIndex] = new SteamVR_TrackedCamera.VideoStreamTexture((uint)deviceIndex, true);
			}
			return SteamVR_TrackedCamera.undistorted[deviceIndex];
		}

		public static SteamVR_TrackedCamera.VideoStreamTexture Source(bool undistorted, int deviceIndex = 0)
		{
			if (!undistorted)
			{
				return SteamVR_TrackedCamera.Distorted(deviceIndex);
			}
			return SteamVR_TrackedCamera.Undistorted(deviceIndex);
		}

		private static SteamVR_TrackedCamera.VideoStream Stream(uint deviceIndex)
		{
			if (SteamVR_TrackedCamera.videostreams == null)
			{
				SteamVR_TrackedCamera.videostreams = new SteamVR_TrackedCamera.VideoStream[64];
			}
			if (SteamVR_TrackedCamera.videostreams[(int)deviceIndex] == null)
			{
				SteamVR_TrackedCamera.videostreams[(int)deviceIndex] = new SteamVR_TrackedCamera.VideoStream(deviceIndex);
			}
			return SteamVR_TrackedCamera.videostreams[(int)deviceIndex];
		}

		private static SteamVR_TrackedCamera.VideoStreamTexture[] distorted;

		private static SteamVR_TrackedCamera.VideoStreamTexture[] undistorted;

		private static SteamVR_TrackedCamera.VideoStream[] videostreams;

		public class VideoStreamTexture
		{
			public VideoStreamTexture(uint deviceIndex, bool undistorted)
			{
				this.undistorted = undistorted;
				this.videostream = SteamVR_TrackedCamera.Stream(deviceIndex);
			}

			public bool undistorted { get; private set; }

			public uint deviceIndex
			{
				get
				{
					return this.videostream.deviceIndex;
				}
			}

			public bool hasCamera
			{
				get
				{
					return this.videostream.hasCamera;
				}
			}

			public bool hasTracking
			{
				get
				{
					this.Update();
					return this.header.trackedDevicePose.bPoseIsValid;
				}
			}

			public uint frameId
			{
				get
				{
					this.Update();
					return this.header.nFrameSequence;
				}
			}

			public VRTextureBounds_t frameBounds { get; private set; }

			public EVRTrackedCameraFrameType frameType
			{
				get
				{
					if (!this.undistorted)
					{
						return EVRTrackedCameraFrameType.Distorted;
					}
					return EVRTrackedCameraFrameType.Undistorted;
				}
			}

			public Texture2D texture
			{
				get
				{
					this.Update();
					return this._texture;
				}
			}

			public SteamVR_Utils.RigidTransform transform
			{
				get
				{
					this.Update();
					return new SteamVR_Utils.RigidTransform(this.header.trackedDevicePose.mDeviceToAbsoluteTracking);
				}
			}

			public Vector3 velocity
			{
				get
				{
					this.Update();
					TrackedDevicePose_t trackedDevicePose = this.header.trackedDevicePose;
					return new Vector3(trackedDevicePose.vVelocity.v0, trackedDevicePose.vVelocity.v1, -trackedDevicePose.vVelocity.v2);
				}
			}

			public Vector3 angularVelocity
			{
				get
				{
					this.Update();
					TrackedDevicePose_t trackedDevicePose = this.header.trackedDevicePose;
					return new Vector3(-trackedDevicePose.vAngularVelocity.v0, -trackedDevicePose.vAngularVelocity.v1, trackedDevicePose.vAngularVelocity.v2);
				}
			}

			public TrackedDevicePose_t GetPose()
			{
				this.Update();
				return this.header.trackedDevicePose;
			}

			public ulong Acquire()
			{
				return this.videostream.Acquire();
			}

			public ulong Release()
			{
				ulong result = this.videostream.Release();
				if (this.videostream.handle == 0UL)
				{
					Object.Destroy(this._texture);
					this._texture = null;
				}
				return result;
			}

			private void Update()
			{
				if (Time.frameCount == this.prevFrameCount)
				{
					return;
				}
				this.prevFrameCount = Time.frameCount;
				if (this.videostream.handle == 0UL)
				{
					return;
				}
				SteamVR instance = SteamVR.instance;
				if (instance == null)
				{
					return;
				}
				CVRTrackedCamera trackedCamera = OpenVR.TrackedCamera;
				if (trackedCamera == null)
				{
					return;
				}
				IntPtr nativeTex = IntPtr.Zero;
				Texture2D texture2D = (this._texture != null) ? this._texture : new Texture2D(2, 2);
				uint nFrameHeaderSize = (uint)Marshal.SizeOf(this.header.GetType());
				if (instance.textureType == ETextureType.OpenGL)
				{
					if (this.glTextureId != 0U)
					{
						trackedCamera.ReleaseVideoStreamTextureGL(this.videostream.handle, this.glTextureId);
					}
					if (trackedCamera.GetVideoStreamTextureGL(this.videostream.handle, this.frameType, ref this.glTextureId, ref this.header, nFrameHeaderSize) != EVRTrackedCameraError.None)
					{
						return;
					}
					nativeTex = (IntPtr)((long)((ulong)this.glTextureId));
				}
				else if (instance.textureType == ETextureType.DirectX && trackedCamera.GetVideoStreamTextureD3D11(this.videostream.handle, this.frameType, texture2D.GetNativeTexturePtr(), ref nativeTex, ref this.header, nFrameHeaderSize) != EVRTrackedCameraError.None)
				{
					return;
				}
				if (this._texture == null)
				{
					this._texture = Texture2D.CreateExternalTexture((int)this.header.nWidth, (int)this.header.nHeight, TextureFormat.RGBA32, false, false, nativeTex);
					uint num = 0U;
					uint num2 = 0U;
					VRTextureBounds_t vrtextureBounds_t = default(VRTextureBounds_t);
					if (trackedCamera.GetVideoStreamTextureSize(this.deviceIndex, this.frameType, ref vrtextureBounds_t, ref num, ref num2) == EVRTrackedCameraError.None)
					{
						vrtextureBounds_t.vMin = 1f - vrtextureBounds_t.vMin;
						vrtextureBounds_t.vMax = 1f - vrtextureBounds_t.vMax;
						this.frameBounds = vrtextureBounds_t;
						return;
					}
				}
				else
				{
					this._texture.UpdateExternalTexture(nativeTex);
				}
			}

			private Texture2D _texture;

			private int prevFrameCount = -1;

			private uint glTextureId;

			private SteamVR_TrackedCamera.VideoStream videostream;

			private CameraVideoStreamFrameHeader_t header;
		}

		private class VideoStream
		{
			public VideoStream(uint deviceIndex)
			{
				this.deviceIndex = deviceIndex;
				CVRTrackedCamera trackedCamera = OpenVR.TrackedCamera;
				if (trackedCamera != null)
				{
					trackedCamera.HasCamera(deviceIndex, ref this._hasCamera);
				}
			}

			public uint deviceIndex { get; private set; }

			public ulong handle
			{
				get
				{
					return this._handle;
				}
			}

			public bool hasCamera
			{
				get
				{
					return this._hasCamera;
				}
			}

			public ulong Acquire()
			{
				if (this._handle == 0UL && this.hasCamera)
				{
					CVRTrackedCamera trackedCamera = OpenVR.TrackedCamera;
					if (trackedCamera != null)
					{
						trackedCamera.AcquireVideoStreamingService(this.deviceIndex, ref this._handle);
					}
				}
				ulong result = this.refCount + 1UL;
				this.refCount = result;
				return result;
			}

			public ulong Release()
			{
				if (this.refCount > 0UL)
				{
					ulong num = this.refCount - 1UL;
					this.refCount = num;
					if (num == 0UL && this._handle != 0UL)
					{
						CVRTrackedCamera trackedCamera = OpenVR.TrackedCamera;
						if (trackedCamera != null)
						{
							trackedCamera.ReleaseVideoStreamingService(this._handle);
						}
						this._handle = 0UL;
					}
				}
				return this.refCount;
			}

			private ulong _handle;

			private bool _hasCamera;

			private ulong refCount;
		}
	}
}
