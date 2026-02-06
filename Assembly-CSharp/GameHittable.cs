using System;
using System.Collections.Generic;
using UnityEngine;

public class GameHittable : MonoBehaviour
{
	private void Awake()
	{
		this.components = new List<IGameHittable>(1);
		base.GetComponentsInChildren<IGameHittable>(this.components);
		for (int i = 0; i < this.hittablePoints.Count; i++)
		{
			this.hittablePoints[i].damageFlash.Setup();
		}
	}

	private void OnEnable()
	{
		if (this.gameEntity != null)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.OnUpdate));
		}
	}

	private void OnDisable()
	{
		if (this.gameEntity != null)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnTick = (Action)Delegate.Remove(gameEntity.OnTick, new Action(this.OnUpdate));
		}
	}

	public void OnUpdate()
	{
		for (int i = 0; i < this.hittablePoints.Count; i++)
		{
			this.hittablePoints[i].damageFlash.Update();
		}
	}

	public void RequestHit(GameHitData hitData)
	{
		hitData.hitEntityId = this.gameEntity.id;
		this.gameEntity.manager.RequestHit(hitData);
	}

	public void ApplyHit(GameHitData hitData)
	{
		for (int i = 0; i < this.components.Count; i++)
		{
			this.components[i].OnHit(hitData);
		}
		GameHitter component = this.gameEntity.manager.GetGameEntity(hitData.hitByEntityId).GetComponent<GameHitter>();
		if (component != null)
		{
			component.ApplyHit(hitData);
		}
		GameHittable.HittablePoint hittablePoint = this.GetHittablePoint(hitData.hittablePoint);
		if (hittablePoint != null)
		{
			hittablePoint.damageFlash.Play();
		}
	}

	private GameHittable.HittablePoint GetHittablePoint(int hittablePoint)
	{
		if (hittablePoint < 0 || hittablePoint >= this.hittablePoints.Count)
		{
			return null;
		}
		return this.hittablePoints[hittablePoint];
	}

	public bool IsHitValid(GameHitData hitData)
	{
		for (int i = 0; i < this.components.Count; i++)
		{
			if (!this.components[i].IsHitValid(hitData))
			{
				return false;
			}
		}
		return this.hittablePoints.Count <= 0 || (hitData.hittablePoint >= 0 && hitData.hittablePoint < this.hittablePoints.Count);
	}

	public int FindHittablePoint(Collider collider)
	{
		if (this.hittablePoints == null || this.hittablePoints.Count == 0)
		{
			return 0;
		}
		for (int i = 0; i < this.hittablePoints.Count; i++)
		{
			if (this.hittablePoints[i].colliders.Contains(collider))
			{
				return i;
			}
		}
		return 0;
	}

	public bool IsColliderValid(Collider collider)
	{
		if (this.hittablePoints == null || this.hittablePoints.Count == 0)
		{
			return true;
		}
		for (int i = 0; i < this.hittablePoints.Count; i++)
		{
			if (this.hittablePoints[i].colliders.Contains(collider))
			{
				return true;
			}
		}
		return false;
	}

	public GameEntity gameEntity;

	public List<GameHittable.HittablePoint> hittablePoints;

	private List<IGameHittable> components;

	[Serializable]
	public class HittablePoint
	{
		public List<Collider> colliders;

		public GRDamageFlash damageFlash;
	}
}
