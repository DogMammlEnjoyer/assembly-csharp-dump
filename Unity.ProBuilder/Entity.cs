using System;
using UnityEngine.Serialization;

namespace UnityEngine.ProBuilder
{
	[DisallowMultipleComponent]
	[AddComponentMenu("")]
	internal sealed class Entity : MonoBehaviour
	{
		public EntityType entityType
		{
			get
			{
				return this.m_EntityType;
			}
		}

		public void Awake()
		{
			MeshRenderer component = base.GetComponent<MeshRenderer>();
			if (!component)
			{
				return;
			}
			switch (this.entityType)
			{
			case EntityType.Detail:
			case EntityType.Occluder:
				break;
			case EntityType.Trigger:
				component.enabled = false;
				return;
			case EntityType.Collider:
				component.enabled = false;
				break;
			default:
				return;
			}
		}

		public void SetEntity(EntityType t)
		{
			this.m_EntityType = t;
		}

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("_entityType")]
		private EntityType m_EntityType;
	}
}
