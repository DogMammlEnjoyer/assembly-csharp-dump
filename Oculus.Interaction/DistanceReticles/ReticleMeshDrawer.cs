using System;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.DistanceReticles
{
	public class ReticleMeshDrawer : InteractorReticle<ReticleDataMesh>
	{
		private IHandGrabInteractor HandGrabInteractor { get; set; }

		public PoseTravelData TravelData
		{
			get
			{
				return this._travelData;
			}
			set
			{
				this._travelData = value;
			}
		}

		protected override IInteractorView Interactor { get; set; }

		protected override Component InteractableComponent
		{
			get
			{
				return this.HandGrabInteractor.TargetInteractable as Component;
			}
		}

		protected virtual void Reset()
		{
			this._filter = base.GetComponent<MeshFilter>();
			this._renderer = base.GetComponent<MeshRenderer>();
		}

		protected virtual void Awake()
		{
			this.HandGrabInteractor = (this._handGrabInteractor as IHandGrabInteractor);
			this.Interactor = (this._handGrabInteractor as IInteractorView);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.EndStart(ref this._started);
		}

		protected override void Draw(ReticleDataMesh dataMesh)
		{
			this._filter.sharedMesh = dataMesh.Filter.sharedMesh;
			this._filter.transform.localScale = dataMesh.Filter.transform.lossyScale;
			this._renderer.enabled = true;
			Pose pose = this.DestinationPose(dataMesh, this.HandGrabInteractor.GetTargetGrabPose());
			Pose pose2 = dataMesh.Target.GetPose(Space.World);
			this._tween = this._travelData.CreateTween(pose2, pose);
		}

		protected override void Hide()
		{
			this._tween = null;
			this._renderer.enabled = false;
		}

		protected override void Align(ReticleDataMesh data)
		{
			Pose target = this.DestinationPose(data, this.HandGrabInteractor.GetTargetGrabPose());
			this._tween.UpdateTarget(target);
			this._tween.Tick();
			Transform transform = this._filter.transform;
			Pose pose = this._tween.Pose;
			transform.SetPose(pose, Space.World);
		}

		private Pose DestinationPose(ReticleDataMesh data, Pose worldSnapPose)
		{
			Pose pose = data.Target.GetPose(Space.World);
			Pose pose2 = PoseUtils.Delta(worldSnapPose, pose);
			Pose result;
			this.HandGrabInteractor.HandGrabApi.Hand.GetRootPose(out result);
			pose = this.HandGrabInteractor.WristToGrabPoseOffset;
			ref result.Premultiply(pose);
			ref result.Premultiply(pose2);
			return result;
		}

		public void InjectAllReticleMeshDrawer(IHandGrabInteractor handGrabInteractor, MeshFilter filter, MeshRenderer renderer)
		{
			this.InjectHandGrabInteractor(handGrabInteractor);
			this.InjectFilter(filter);
			this.InjectRenderer(renderer);
		}

		public void InjectHandGrabInteractor(IHandGrabInteractor handGrabInteractor)
		{
			this._handGrabInteractor = (handGrabInteractor as Object);
			this.HandGrabInteractor = handGrabInteractor;
			this.Interactor = (handGrabInteractor as IInteractorView);
		}

		public void InjectFilter(MeshFilter filter)
		{
			this._filter = filter;
		}

		public void InjectRenderer(MeshRenderer renderer)
		{
			this._renderer = renderer;
		}

		[Tooltip("The hand grab interactor that uses the reticle.")]
		[FormerlySerializedAs("_handGrabber")]
		[SerializeField]
		[Interface(typeof(IHandGrabInteractor), new Type[]
		{
			typeof(IInteractorView)
		})]
		private Object _handGrabInteractor;

		[Tooltip("The ReticleMesh prefab's mesh filter.")]
		[SerializeField]
		private MeshFilter _filter;

		[Tooltip("The ReticleMesh prefab's mesh renderer.")]
		[SerializeField]
		private MeshRenderer _renderer;

		[SerializeField]
		private PoseTravelData _travelData = PoseTravelData.FAST;

		private Tween _tween;
	}
}
