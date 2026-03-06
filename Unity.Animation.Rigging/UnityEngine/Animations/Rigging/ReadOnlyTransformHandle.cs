using System;

namespace UnityEngine.Animations.Rigging
{
	public struct ReadOnlyTransformHandle
	{
		public Vector3 GetLocalPosition(AnimationStream stream)
		{
			if (this.m_InStream != 1)
			{
				return this.m_SceneHandle.GetLocalPosition(stream);
			}
			return this.m_StreamHandle.GetLocalPosition(stream);
		}

		public Quaternion GetLocalRotation(AnimationStream stream)
		{
			if (this.m_InStream != 1)
			{
				return this.m_SceneHandle.GetLocalRotation(stream);
			}
			return this.m_StreamHandle.GetLocalRotation(stream);
		}

		public Vector3 GetLocalScale(AnimationStream stream)
		{
			if (this.m_InStream != 1)
			{
				return this.m_SceneHandle.GetLocalScale(stream);
			}
			return this.m_StreamHandle.GetLocalScale(stream);
		}

		public void GetLocalTRS(AnimationStream stream, out Vector3 position, out Quaternion rotation, out Vector3 scale)
		{
			if (this.m_InStream == 1)
			{
				this.m_StreamHandle.GetLocalTRS(stream, out position, out rotation, out scale);
				return;
			}
			this.m_SceneHandle.GetLocalTRS(stream, out position, out rotation, out scale);
		}

		public Matrix4x4 GetLocalToParentMatrix(AnimationStream stream)
		{
			if (this.m_InStream != 1)
			{
				return this.m_SceneHandle.GetLocalToParentMatrix(stream);
			}
			return this.m_StreamHandle.GetLocalToParentMatrix(stream);
		}

		public Vector3 GetPosition(AnimationStream stream)
		{
			if (this.m_InStream != 1)
			{
				return this.m_SceneHandle.GetPosition(stream);
			}
			return this.m_StreamHandle.GetPosition(stream);
		}

		public Quaternion GetRotation(AnimationStream stream)
		{
			if (this.m_InStream != 1)
			{
				return this.m_SceneHandle.GetRotation(stream);
			}
			return this.m_StreamHandle.GetRotation(stream);
		}

		public void GetGlobalTR(AnimationStream stream, out Vector3 position, out Quaternion rotation)
		{
			if (this.m_InStream == 1)
			{
				this.m_StreamHandle.GetGlobalTR(stream, out position, out rotation);
				return;
			}
			this.m_SceneHandle.GetGlobalTR(stream, out position, out rotation);
		}

		public Matrix4x4 GetLocalToWorldMatrix(AnimationStream stream)
		{
			if (this.m_InStream != 1)
			{
				return this.m_SceneHandle.GetLocalToWorldMatrix(stream);
			}
			return this.m_StreamHandle.GetLocalToWorldMatrix(stream);
		}

		public bool IsResolved(AnimationStream stream)
		{
			return this.m_InStream != 1 || this.m_StreamHandle.IsResolved(stream);
		}

		public bool IsValid(AnimationStream stream)
		{
			if (this.m_InStream != 1)
			{
				return this.m_SceneHandle.IsValid(stream);
			}
			return this.m_StreamHandle.IsValid(stream);
		}

		public void Resolve(AnimationStream stream)
		{
			if (this.m_InStream == 1)
			{
				this.m_StreamHandle.Resolve(stream);
			}
		}

		public static ReadOnlyTransformHandle Bind(Animator animator, Transform transform)
		{
			ReadOnlyTransformHandle readOnlyTransformHandle = default(ReadOnlyTransformHandle);
			if (transform == null)
			{
				throw new ArgumentNullException("transform");
			}
			readOnlyTransformHandle.m_InStream = (transform.IsChildOf(animator.avatarRoot) ? 1 : 0);
			if (readOnlyTransformHandle.m_InStream == 1)
			{
				readOnlyTransformHandle.m_StreamHandle = animator.BindStreamTransform(transform);
			}
			else
			{
				readOnlyTransformHandle.m_SceneHandle = animator.BindSceneTransform(transform);
			}
			return readOnlyTransformHandle;
		}

		private TransformStreamHandle m_StreamHandle;

		private TransformSceneHandle m_SceneHandle;

		private byte m_InStream;
	}
}
