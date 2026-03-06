using System;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	internal struct RigSyncSceneToStreamJob : IAnimationJob
	{
		public void ProcessRootMotion(AnimationStream stream)
		{
		}

		public void ProcessAnimation(AnimationStream stream)
		{
			this.transformSyncer.Sync(ref stream);
			this.propertySyncer.Sync(ref stream);
			this.rigWeightSyncer.Sync(ref stream);
			this.constraintWeightSyncer.Sync(ref stream);
			NativeArray<float> nativeArray = this.rigWeightSyncer.StreamValues(ref stream);
			NativeArray<float> buffer = this.constraintWeightSyncer.StreamValues(ref stream);
			int num = 0;
			int i = 0;
			int length = buffer.Length;
			while (i < length)
			{
				if (i >= this.rigConstraintEndIdx[num])
				{
					num++;
				}
				ref NativeArray<float> ptr = ref buffer;
				int index = i;
				ptr[index] *= nativeArray[num] * this.rigStates[num];
				i++;
			}
			AnimationStreamHandleUtility.WriteFloats(stream, this.modulatedConstraintWeights, buffer, false);
		}

		public RigSyncSceneToStreamJob.TransformSyncer transformSyncer;

		public RigSyncSceneToStreamJob.PropertySyncer propertySyncer;

		public RigSyncSceneToStreamJob.PropertySyncer rigWeightSyncer;

		public RigSyncSceneToStreamJob.PropertySyncer constraintWeightSyncer;

		public NativeArray<float> rigStates;

		public NativeArray<int> rigConstraintEndIdx;

		public NativeArray<PropertyStreamHandle> modulatedConstraintWeights;

		public struct TransformSyncer : IDisposable
		{
			public static RigSyncSceneToStreamJob.TransformSyncer Create(int size)
			{
				return new RigSyncSceneToStreamJob.TransformSyncer
				{
					sceneHandles = new NativeArray<TransformSceneHandle>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
					streamHandles = new NativeArray<TransformStreamHandle>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory)
				};
			}

			public void Dispose()
			{
				if (this.sceneHandles.IsCreated)
				{
					this.sceneHandles.Dispose();
				}
				if (this.streamHandles.IsCreated)
				{
					this.streamHandles.Dispose();
				}
			}

			public void BindAt(int index, Animator animator, Transform transform)
			{
				this.sceneHandles[index] = animator.BindSceneTransform(transform);
				this.streamHandles[index] = animator.BindStreamTransform(transform);
			}

			public void Sync(ref AnimationStream stream)
			{
				int i = 0;
				int length = this.sceneHandles.Length;
				while (i < length)
				{
					TransformSceneHandle value = this.sceneHandles[i];
					if (value.IsValid(stream))
					{
						TransformStreamHandle value2 = this.streamHandles[i];
						Vector3 position;
						Quaternion rotation;
						Vector3 scale;
						value.GetLocalTRS(stream, out position, out rotation, out scale);
						value2.SetLocalTRS(stream, position, rotation, scale, true);
						this.streamHandles[i] = value2;
						this.sceneHandles[i] = value;
					}
					i++;
				}
			}

			public NativeArray<TransformSceneHandle> sceneHandles;

			public NativeArray<TransformStreamHandle> streamHandles;
		}

		internal struct PropertySyncer : IDisposable
		{
			public static RigSyncSceneToStreamJob.PropertySyncer Create(int size)
			{
				return new RigSyncSceneToStreamJob.PropertySyncer
				{
					sceneHandles = new NativeArray<PropertySceneHandle>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
					streamHandles = new NativeArray<PropertyStreamHandle>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
					buffer = new NativeArray<float>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory)
				};
			}

			public void Dispose()
			{
				if (this.sceneHandles.IsCreated)
				{
					this.sceneHandles.Dispose();
				}
				if (this.streamHandles.IsCreated)
				{
					this.streamHandles.Dispose();
				}
				if (this.buffer.IsCreated)
				{
					this.buffer.Dispose();
				}
			}

			public void BindAt(int index, Animator animator, Component component, string property)
			{
				this.sceneHandles[index] = animator.BindSceneProperty(component.transform, component.GetType(), property);
				this.streamHandles[index] = animator.BindStreamProperty(component.transform, component.GetType(), property);
			}

			public void Sync(ref AnimationStream stream)
			{
				AnimationSceneHandleUtility.ReadFloats(stream, this.sceneHandles, this.buffer);
				AnimationStreamHandleUtility.WriteFloats(stream, this.streamHandles, this.buffer, true);
			}

			public NativeArray<float> StreamValues(ref AnimationStream stream)
			{
				AnimationStreamHandleUtility.ReadFloats(stream, this.streamHandles, this.buffer);
				return this.buffer;
			}

			public NativeArray<PropertySceneHandle> sceneHandles;

			public NativeArray<PropertyStreamHandle> streamHandles;

			public NativeArray<float> buffer;
		}
	}
}
