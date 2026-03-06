using System;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	[Serializable]
	public struct SplineInfo : IEquatable<SplineInfo>, ISerializationCallbackReceiver
	{
		public Object Object
		{
			get
			{
				return this.m_Object;
			}
		}

		public ISplineContainer Container
		{
			get
			{
				return this.m_Container ?? (this.m_Object as ISplineContainer);
			}
			set
			{
				this.m_Container = value;
			}
		}

		public Transform Transform
		{
			get
			{
				Component component = this.Object as Component;
				if (component == null)
				{
					return null;
				}
				return component.transform;
			}
		}

		public Spline Spline
		{
			get
			{
				if (this.Container == null || this.Index <= -1 || this.Index >= this.Container.Splines.Count)
				{
					return null;
				}
				return this.Container.Splines[this.Index];
			}
		}

		public int Index
		{
			get
			{
				return this.m_SplineIndex;
			}
			set
			{
				this.m_SplineIndex = value;
			}
		}

		public float4x4 LocalToWorld
		{
			get
			{
				if (!(this.Transform != null))
				{
					return float4x4.identity;
				}
				return this.Transform.localToWorldMatrix;
			}
		}

		public SplineInfo(ISplineContainer container, int index)
		{
			this.m_Container = container;
			this.m_Object = (container as Object);
			this.m_SplineIndex = index;
		}

		public bool Equals(SplineInfo other)
		{
			return object.Equals(this.Container, other.Container) && this.Index == other.Index;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is SplineInfo)
			{
				SplineInfo other = (SplineInfo)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((this.Container != null) ? this.Container.GetHashCode() : 0) * 397 ^ this.Index;
		}

		public void OnBeforeSerialize()
		{
			Object @object = this.m_Container as Object;
			if (@object != null)
			{
				this.m_Object = @object;
				this.m_Container = null;
			}
		}

		public void OnAfterDeserialize()
		{
			if (this.m_Container == null)
			{
				this.m_Container = (this.m_Object as ISplineContainer);
			}
		}

		[SerializeField]
		private Object m_Object;

		[SerializeReference]
		private ISplineContainer m_Container;

		[SerializeField]
		private int m_SplineIndex;
	}
}
