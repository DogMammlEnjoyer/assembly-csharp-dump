using System;
using UnityEngine;
using UnityEngine.Audio;
using Voxels;

public class Voxel_Pickaxe : MonoBehaviour
{
	public bool Held { get; set; }

	private void Reset()
	{
		this._layerMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
	}

	private void Awake()
	{
		this._layerMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		if (this.sound.transform == base.transform)
		{
			Debug.LogError("Audio source for " + base.name + " must be on a separate gameobject!", this);
		}
	}

	private void OnEnable()
	{
		this.ResetVelocity();
	}

	private void OnDisable()
	{
	}

	private void FixedUpdate()
	{
		if (!this.Held)
		{
			return;
		}
		for (int i = 0; i < this.points.Length; i++)
		{
			this.UpdateInteractionPoint(ref this.points[i]);
		}
	}

	private void StartGrabbing()
	{
		this.Held = true;
		this.ResetVelocity();
	}

	private void StopGrabbing()
	{
		this.Held = false;
	}

	private void ResetVelocity()
	{
		for (int i = 0; i < this.points.Length; i++)
		{
			this.points[i].position = (this.points[i].previousPosition = this.points[i].transform.position);
		}
	}

	private void UpdateInteractionPoint(ref Voxel_Pickaxe.InteractionPoint point)
	{
		point.previousPosition = point.position;
		point.position = point.transform.position;
		if (Time.time < this._nextHitTime)
		{
			return;
		}
		Vector3 vector = (point.position - point.previousPosition) / Time.fixedDeltaTime;
		float magnitude = vector.magnitude;
		if (magnitude < this.minHitSpeed)
		{
			return;
		}
		bool flag = Vector3.Dot(vector.normalized, point.transform.forward) >= this.alignThreshold;
		RaycastHit hit;
		if (Physics.Linecast(point.previousPosition, point.position, out hit, this._layerMask, QueryTriggerInteraction.Ignore))
		{
			ChunkComponent component = hit.collider.GetComponent<ChunkComponent>();
			if (component && flag && magnitude >= this.minMineSpeed)
			{
				this.Play(this.goodHit, hit.point);
				component.World.Mine(hit, this.mine);
			}
			else
			{
				this.Play(this.badHit, hit.point);
			}
			this._nextHitTime = Time.time + this.hitCooldown;
		}
	}

	private void Play(AudioResource resource, Vector3 position)
	{
		if (!resource)
		{
			return;
		}
		this.sound.Stop();
		this.sound.resource = resource;
		this.sound.transform.position = position;
		this.sound.Play();
	}

	public void OnEntityInit()
	{
		this.sound.transform.parent = null;
	}

	public void OnEntityDestroy()
	{
		this.sound.transform.parent = base.transform;
	}

	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	private void OnDrawGizmosSelected()
	{
		if (this.points == null)
		{
			return;
		}
		Gizmos.color = Color.green;
		foreach (Voxel_Pickaxe.InteractionPoint interactionPoint in this.points)
		{
			Gizmos.DrawWireSphere(interactionPoint.transform.position, 0.02f);
			Gizmos.DrawLine(interactionPoint.transform.position, interactionPoint.transform.position + interactionPoint.transform.forward * 0.5f);
		}
	}

	public VoxelAction mine = new VoxelAction
	{
		strength = 1f,
		radius = 0.5f,
		operation = OperationType.Subtract
	};

	public Voxel_Pickaxe.InteractionPoint[] points;

	public AudioResource goodHit;

	public AudioResource badHit;

	public AudioSource sound;

	public float hitCooldown = 0.5f;

	public float minHitSpeed = 1f;

	public float minMineSpeed = 5f;

	public float alignThreshold = 0.7f;

	private int _layerMask;

	private float _nextHitTime;

	[Serializable]
	public struct InteractionPoint
	{
		public Transform transform;

		public Vector3 previousPosition;

		public Vector3 position;
	}
}
