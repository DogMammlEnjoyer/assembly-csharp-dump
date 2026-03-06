using System;
using Oculus.Interaction.Surfaces;
using UnityEngine;

namespace Oculus.Interaction
{
	public class RayInteractable : PointerInteractable<RayInteractor, RayInteractable>
	{
		public ISurface Surface { get; private set; }

		private IMovementProvider MovementProvider { get; set; }

		public int TiebreakerScore
		{
			get
			{
				return this._tiebreakerScore;
			}
			set
			{
				this._tiebreakerScore = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.Surface = (this._surface as ISurface);
			this.SelectSurface = (this._selectSurface as ISurface);
			this.MovementProvider = (this._movementProvider as IMovementProvider);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			if (!(this._selectSurface != null))
			{
				this.SelectSurface = this.Surface;
				this._selectSurface = (this.SelectSurface as MonoBehaviour);
			}
			this.EndStart(ref this._started);
		}

		public bool Raycast(Ray ray, out SurfaceHit hit, in float maxDistance, bool selectSurface)
		{
			return (selectSurface ? this.SelectSurface : this.Surface).Raycast(ray, out hit, maxDistance);
		}

		public IMovement GenerateMovement(in Pose to, in Pose source)
		{
			if (this.MovementProvider == null)
			{
				return null;
			}
			IMovement movement = this.MovementProvider.CreateMovement();
			movement.StopAndSetPose(source);
			movement.MoveTo(to);
			return movement;
		}

		public void InjectAllRayInteractable(ISurface surface)
		{
			this.InjectSurface(surface);
		}

		public void InjectSurface(ISurface surface)
		{
			this.Surface = surface;
			this._surface = (surface as Object);
		}

		public void InjectOptionalSelectSurface(ISurface surface)
		{
			this.SelectSurface = surface;
			this._selectSurface = (surface as Object);
		}

		public void InjectOptionalMovementProvider(IMovementProvider provider)
		{
			this._movementProvider = (provider as Object);
			this.MovementProvider = provider;
		}

		[Tooltip("The mesh used as the interactive surface for the ray.")]
		[SerializeField]
		[Interface(typeof(ISurface), new Type[]
		{

		})]
		private Object _surface;

		[Tooltip("Defines the boundaries of the raycast. All RayInteractables must be inside this surface for the raycast to reach them.")]
		[SerializeField]
		[Optional]
		[Interface(typeof(ISurface), new Type[]
		{

		})]
		private Object _selectSurface;

		private ISurface SelectSurface;

		[Tooltip("An IMovementProvider that determines how the interactable moves when selected.")]
		[SerializeField]
		[Optional]
		[Interface(typeof(IMovementProvider), new Type[]
		{

		})]
		private Object _movementProvider;

		[Tooltip("The score used when comparing two interactables to determine which one should be selected. Each interactable has its own score, and the highest scoring interactable will be selected.")]
		[SerializeField]
		[Optional]
		private int _tiebreakerScore;
	}
}
