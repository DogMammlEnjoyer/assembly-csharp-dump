using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Oculus.Interaction.Input.UnityXR
{
	public class FromUnityXRHandDataSource : FromOpenXRHandDataSource
	{
		private InputActionMap MetaAimHandBindings
		{
			get
			{
				if (this._handedness != Handedness.Left)
				{
					return this._metaAimHandBindingsRight;
				}
				return this._metaAimHandBindingsLeft;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.TrackingToWorldTransformer = (this._trackingToWorldTransformer as ITrackingToWorldTransformer);
			this.UpdateConfig();
		}

		protected override void Start()
		{
			base.Start();
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.UpdateConfig();
			InputActionMap metaAimHandBindings = this.MetaAimHandBindings;
			this._metaAimFlags = metaAimHandBindings["aimFlags"];
			this._pinchStrengthIndex = metaAimHandBindings["pinchStrengthIndex"];
			this._pinchStrengthMiddle = metaAimHandBindings["pinchStrengthMiddle"];
			this._pinchStrengthRing = metaAimHandBindings["pinchStrengthRing"];
			this._pinchStrengthLittle = metaAimHandBindings["pinchStrengthLittle"];
			this._devicePosition = metaAimHandBindings["devicePosition"];
			this._deviceRotation = metaAimHandBindings["deviceRotation"];
			this.EndStart(ref this._started);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.MetaAimHandBindings.Enable();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.MetaAimHandBindings.Disable();
		}

		private HandDataSourceConfig Config
		{
			get
			{
				if (this._config != null)
				{
					return this._config;
				}
				this._config = new HandDataSourceConfig
				{
					Handedness = this._handedness
				};
				return this._config;
			}
		}

		private void UpdateConfig()
		{
			this.Config.TrackingToWorldTransformer = this.TrackingToWorldTransformer;
			this.Config.HandSkeleton = ((this._handedness == Handedness.Left) ? HandSkeleton.DefaultLeftSkeleton : HandSkeleton.DefaultRightSkeleton);
			this._dataAsset.Config = this.Config;
		}

		public void InjectTrackingToWorldTransformer(ITrackingToWorldTransformer trackingToWorldTransformer)
		{
			this._trackingToWorldTransformer = (trackingToWorldTransformer as Object);
			this.TrackingToWorldTransformer = trackingToWorldTransformer;
			this.UpdateConfig();
		}

		[Header("Shared Configuration")]
		[SerializeField]
		private Handedness _handedness;

		[SerializeField]
		[Interface(typeof(ITrackingToWorldTransformer), new Type[]
		{

		})]
		private Object _trackingToWorldTransformer;

		private ITrackingToWorldTransformer TrackingToWorldTransformer;

		private static string _metaAimHandActionMap = "{\n            \"maps\": [\n                {\n                    \"name\": \"MetaAimHand\",\n                    \"actions\": [\n                        {\n                            \"name\": \"aimFlags\",\n                            \"expectedControlLayout\": \"Integer\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<MetaAimHand>{LeftHand}/aimFlags\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"pinchStrengthIndex\",\n                            \"expectedControlLayout\": \"Axis\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<MetaAimHand>{LeftHand}/pinchStrengthIndex\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"pinchStrengthMiddle\",\n                            \"expectedControlLayout\": \"Axis\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<MetaAimHand>{LeftHand}/pinchStrengthMiddle\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"pinchStrengthRing\",\n                            \"expectedControlLayout\": \"Axis\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<MetaAimHand>{LeftHand}/pinchStrengthRing\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"pinchStrengthLittle\",\n                            \"expectedControlLayout\": \"Axis\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<MetaAimHand>{LeftHand}/pinchStrengthLittle\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"devicePosition\",\n                            \"expectedControlLayout\": \"Vector3\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<MetaAimHand>{LeftHand}/devicePosition\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"deviceRotation\",\n                            \"expectedControlLayout\": \"Quaternion\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<MetaAimHand>{LeftHand}/deviceRotation\"\n                                }\n                            ]\n                        }\n                    ]\n                }\n            ]}";

		[SerializeField]
		private InputActionMap _metaAimHandBindingsLeft = InputActionMap.FromJson(FromUnityXRHandDataSource._metaAimHandActionMap).FirstOrDefault<InputActionMap>();

		[SerializeField]
		private InputActionMap _metaAimHandBindingsRight = InputActionMap.FromJson(FromUnityXRHandDataSource._metaAimHandActionMap.Replace("{LeftHand}", "{RightHand}")).FirstOrDefault<InputActionMap>();

		private HandDataSourceConfig _config;

		private InputAction _metaAimFlags;

		private InputAction _pinchStrengthIndex;

		private InputAction _pinchStrengthMiddle;

		private InputAction _pinchStrengthRing;

		private InputAction _pinchStrengthLittle;

		private InputAction _devicePosition;

		private InputAction _deviceRotation;
	}
}
