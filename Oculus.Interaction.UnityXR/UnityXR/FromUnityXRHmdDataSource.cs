using System;
using Oculus.Interaction.Input;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

namespace Oculus.Interaction.UnityXR
{
	public class FromUnityXRHmdDataSource : DataSource<HmdDataAsset>
	{
		protected void Awake()
		{
			this.TrackingToWorldTransformer = (this._trackingToWorldTransformer as ITrackingToWorldTransformer);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.EndStart(ref this._started);
		}

		private HmdDataSourceConfig Config
		{
			get
			{
				if (this._config != null)
				{
					return this._config;
				}
				this._config = new HmdDataSourceConfig
				{
					TrackingToWorldTransformer = this.TrackingToWorldTransformer
				};
				return this._config;
			}
		}

		protected override void UpdateData()
		{
			this._hmdDataAsset.Config = this.Config;
			this._hmdDataAsset.Root = this._origin.Camera.transform.GetLocalPose();
			this._hmdDataAsset.IsTracked = XRSettings.isDeviceActive;
			this._hmdDataAsset.FrameId = Time.frameCount;
		}

		protected override HmdDataAsset DataAsset
		{
			get
			{
				return this._hmdDataAsset;
			}
		}

		public void InjectAllFromOVRHmdDataSource(DataSource<HmdDataAsset>.UpdateModeFlags updateMode, IDataSource updateAfter, bool useOvrManagerEmulatedPose, ITrackingToWorldTransformer trackingToWorldTransformer)
		{
			base.InjectAllDataSource(updateMode, updateAfter);
			this.InjectTrackingToWorldTransformer(trackingToWorldTransformer);
		}

		public void InjectTrackingToWorldTransformer(ITrackingToWorldTransformer trackingToWorldTransformer)
		{
			this._trackingToWorldTransformer = (trackingToWorldTransformer as Object);
			this.TrackingToWorldTransformer = trackingToWorldTransformer;
		}

		[Header("Shared Configuration")]
		[SerializeField]
		[Interface(typeof(ITrackingToWorldTransformer), new Type[]
		{

		})]
		private Object _trackingToWorldTransformer;

		private ITrackingToWorldTransformer TrackingToWorldTransformer;

		private HmdDataAsset _hmdDataAsset = new HmdDataAsset();

		private HmdDataSourceConfig _config;

		[SerializeField]
		private XROrigin _origin;
	}
}
