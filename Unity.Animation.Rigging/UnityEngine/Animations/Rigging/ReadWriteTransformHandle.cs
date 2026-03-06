using System;

namespace UnityEngine.Animations.Rigging
{
	public struct ReadWriteTransformHandle
	{
		public Vector3 GetLocalPosition(AnimationStream stream)
		{
			return this.m_Handle.GetLocalPosition(stream);
		}

		public Quaternion GetLocalRotation(AnimationStream stream)
		{
			return this.m_Handle.GetLocalRotation(stream);
		}

		public Vector3 GetLocalScale(AnimationStream stream)
		{
			return this.m_Handle.GetLocalScale(stream);
		}

		public void GetLocalTRS(AnimationStream stream, out Vector3 position, out Quaternion rotation, out Vector3 scale)
		{
			this.m_Handle.GetLocalTRS(stream, out position, out rotation, out scale);
		}

		public void SetLocalPosition(AnimationStream stream, Vector3 position)
		{
			this.m_Handle.SetLocalPosition(stream, position);
		}

		public void SetLocalRotation(AnimationStream stream, Quaternion rotation)
		{
			this.m_Handle.SetLocalRotation(stream, rotation);
		}

		public void SetLocalScale(AnimationStream stream, Vector3 scale)
		{
			this.m_Handle.SetLocalScale(stream, scale);
		}

		public void SetLocalTRS(AnimationStream stream, Vector3 position, Quaternion rotation, Vector3 scale, bool useMask = false)
		{
			this.m_Handle.SetLocalTRS(stream, position, rotation, scale, useMask);
		}

		public Vector3 GetPosition(AnimationStream stream)
		{
			return this.m_Handle.GetPosition(stream);
		}

		public Quaternion GetRotation(AnimationStream stream)
		{
			return this.m_Handle.GetRotation(stream);
		}

		public void GetGlobalTR(AnimationStream stream, out Vector3 position, out Quaternion rotation)
		{
			this.m_Handle.GetGlobalTR(stream, out position, out rotation);
		}

		public void SetPosition(AnimationStream stream, Vector3 position)
		{
			this.m_Handle.SetPosition(stream, position);
		}

		public void SetRotation(AnimationStream stream, Quaternion rotation)
		{
			this.m_Handle.SetRotation(stream, rotation);
		}

		public void SetGlobalTR(AnimationStream stream, Vector3 position, Quaternion rotation, bool useMask = false)
		{
			this.m_Handle.SetGlobalTR(stream, position, rotation, useMask);
		}

		public bool IsResolved(AnimationStream stream)
		{
			return this.m_Handle.IsResolved(stream);
		}

		public bool IsValid(AnimationStream stream)
		{
			return this.m_Handle.IsValid(stream);
		}

		public void Resolve(AnimationStream stream)
		{
			this.m_Handle.Resolve(stream);
		}

		public static ReadWriteTransformHandle Bind(Animator animator, Transform transform)
		{
			ReadWriteTransformHandle result = default(ReadWriteTransformHandle);
			if (transform == null)
			{
				throw new ArgumentNullException("transform");
			}
			if (!transform.IsChildOf(animator.avatarRoot))
			{
				throw new InvalidOperationException("Transform '" + transform.name + "' is not a child of the Animator hierarchy, and cannot be written to.");
			}
			result.m_Handle = animator.BindStreamTransform(transform);
			return result;
		}

		private TransformStreamHandle m_Handle;
	}
}
