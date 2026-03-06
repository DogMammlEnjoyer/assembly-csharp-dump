using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.Jobs
{
	public static class IJobParallelForTransformExtensions
	{
		public static void EarlyJobInit<T>() where T : struct, IJobParallelForTransform
		{
			IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.Initialize();
		}

		private unsafe static IntPtr GetReflectionData<T>() where T : struct, IJobParallelForTransform
		{
			IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.Initialize();
			return *IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.jobReflectionData.Data;
		}

		public static JobHandle Schedule<T>(this T jobData, TransformAccessArray transforms, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForTransform
		{
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), IJobParallelForTransformExtensions.GetReflectionData<T>(), dependsOn, ScheduleMode.Batched);
			return JobsUtility.ScheduleParallelForTransform(ref jobScheduleParameters, transforms.GetTransformAccessArrayForSchedule());
		}

		public static JobHandle ScheduleReadOnly<T>(this T jobData, TransformAccessArray transforms, int batchSize, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForTransform
		{
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), IJobParallelForTransformExtensions.GetReflectionData<T>(), dependsOn, ScheduleMode.Batched);
			return JobsUtility.ScheduleParallelForTransformReadOnly(ref jobScheduleParameters, transforms.GetTransformAccessArrayForSchedule(), batchSize);
		}

		public static void RunReadOnly<T>(this T jobData, TransformAccessArray transforms) where T : struct, IJobParallelForTransform
		{
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), IJobParallelForTransformExtensions.GetReflectionData<T>(), default(JobHandle), ScheduleMode.Run);
			JobsUtility.ScheduleParallelForTransformReadOnly(ref jobScheduleParameters, transforms.GetTransformAccessArrayForSchedule(), transforms.length);
		}

		public static JobHandle ScheduleByRef<T>(this T jobData, TransformAccessArray transforms, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForTransform
		{
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), IJobParallelForTransformExtensions.GetReflectionData<T>(), dependsOn, ScheduleMode.Batched);
			return JobsUtility.ScheduleParallelForTransform(ref jobScheduleParameters, transforms.GetTransformAccessArrayForSchedule());
		}

		public static JobHandle ScheduleReadOnlyByRef<T>(this T jobData, TransformAccessArray transforms, int batchSize, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForTransform
		{
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), IJobParallelForTransformExtensions.GetReflectionData<T>(), dependsOn, ScheduleMode.Batched);
			return JobsUtility.ScheduleParallelForTransformReadOnly(ref jobScheduleParameters, transforms.GetTransformAccessArrayForSchedule(), batchSize);
		}

		public static void RunReadOnlyByRef<T>(this T jobData, TransformAccessArray transforms) where T : struct, IJobParallelForTransform
		{
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), IJobParallelForTransformExtensions.GetReflectionData<T>(), default(JobHandle), ScheduleMode.Run);
			JobsUtility.ScheduleParallelForTransformReadOnly(ref jobScheduleParameters, transforms.GetTransformAccessArrayForSchedule(), transforms.length);
		}

		internal struct TransformParallelForLoopStruct<T> where T : struct, IJobParallelForTransform
		{
			[BurstDiscard]
			internal unsafe static void Initialize()
			{
				bool flag = *IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.jobReflectionData.Data == IntPtr.Zero;
				if (flag)
				{
					*IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(T), new IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.ExecuteJobFunction(IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.Execute), null, null);
				}
			}

			public unsafe static void Execute(ref T jobData, IntPtr jobData2, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex)
			{
				IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.TransformJobData transformJobData;
				UnsafeUtility.CopyPtrToStructure<IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.TransformJobData>((void*)jobData2, out transformJobData);
				int* ptr = (int*)((void*)TransformAccessArray.GetSortedToUserIndex(transformJobData.TransformAccessArray));
				TransformAccess* ptr2 = (TransformAccess*)((void*)TransformAccessArray.GetSortedTransformAccess(transformJobData.TransformAccessArray));
				bool flag = transformJobData.IsReadOnly == 1;
				if (flag)
				{
					for (;;)
					{
						int num;
						int num2;
						bool flag2 = !JobsUtility.GetWorkStealingRange(ref ranges, jobIndex, out num, out num2);
						if (flag2)
						{
							break;
						}
						int num3 = num2;
						for (int i = num; i < num3; i++)
						{
							int num4 = i;
							int index = ptr[num4];
							TransformAccess transform = ptr2[num4];
							jobData.Execute(index, transform);
						}
					}
				}
				else
				{
					int num5;
					int num6;
					JobsUtility.GetJobRange(ref ranges, jobIndex, out num5, out num6);
					for (int j = num5; j < num6; j++)
					{
						int num7 = j;
						int index2 = ptr[num7];
						TransformAccess transform2 = ptr2[num7];
						jobData.Execute(index2, transform2);
					}
				}
			}

			internal static readonly BurstLike.SharedStatic<IntPtr> jobReflectionData = BurstLike.SharedStatic<IntPtr>.GetOrCreate<IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>>(0U);

			private struct TransformJobData
			{
				public IntPtr TransformAccessArray;

				public int IsReadOnly;
			}

			public delegate void ExecuteJobFunction(ref T jobData, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);
		}
	}
}
