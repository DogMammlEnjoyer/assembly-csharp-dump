using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit
{
	internal static class SortingHelpers
	{
		public static void Sort<T>(IList<T> hits, IComparer<T> comparer) where T : struct
		{
			SortingHelpers.Sort<T>(hits, comparer, hits.Count);
		}

		public static void Sort<T>(IList<T> hits, IComparer<T> comparer, int count) where T : struct
		{
			if (count <= 1)
			{
				return;
			}
			for (int i = count - 1; i > 0; i--)
			{
				bool flag = false;
				for (int j = 1; j <= i; j++)
				{
					if (comparer.Compare(hits[j - 1], hits[j]) > 0)
					{
						int index = j - 1;
						int index2 = j;
						T value = hits[j];
						T value2 = hits[j - 1];
						hits[index] = value;
						hits[index2] = value2;
						flag = true;
					}
				}
				if (!flag)
				{
					break;
				}
			}
		}

		public static void SortByDistanceToInteractor(IXRInteractor interactor, List<IXRInteractable> unsortedTargets, List<IXRInteractable> results)
		{
			SortingHelpers.SortByDistanceToInteractor(interactor, unsortedTargets, results, SortingHelpers.interactableBasedEvaluator);
		}

		public static void SortByDistanceToInteractor(IXRInteractor interactor, List<IXRInteractable> unsortedTargets, List<IXRInteractable> results, IInteractorDistanceEvaluator distanceEvaluator)
		{
			results.Clear();
			if (unsortedTargets.Count == 0)
			{
				return;
			}
			if (unsortedTargets.Count == 1)
			{
				results.Add(unsortedTargets[0]);
				return;
			}
			results.AddRange(unsortedTargets);
			SortingHelpers.s_InteractableDistanceSqrMap.Clear();
			foreach (IXRInteractable ixrinteractable in unsortedTargets)
			{
				SortingHelpers.s_InteractableDistanceSqrMap[ixrinteractable] = distanceEvaluator.EvaluateDistance(interactor, ixrinteractable);
			}
			results.Sort(SortingHelpers.s_InteractableDistanceComparison);
		}

		public static void SortByDistanceToInteractor(IXRInteractor interactor, List<IXRInteractable> interactablesToSort)
		{
			SortingHelpers.SortByDistanceToInteractor(interactor, interactablesToSort, SortingHelpers.interactableBasedEvaluator);
		}

		public static void SortByDistanceToInteractor(IXRInteractor interactor, List<IXRInteractable> interactablesToSort, IInteractorDistanceEvaluator distanceEvaluator)
		{
			if (interactablesToSort.Count <= 1)
			{
				return;
			}
			SortingHelpers.s_InteractableDistanceSqrMap.Clear();
			foreach (IXRInteractable ixrinteractable in interactablesToSort)
			{
				SortingHelpers.s_InteractableDistanceSqrMap[ixrinteractable] = distanceEvaluator.EvaluateDistance(interactor, ixrinteractable);
			}
			interactablesToSort.Sort(SortingHelpers.s_InteractableDistanceComparison);
		}

		private static int InteractableDistanceComparison(IXRInteractable x, IXRInteractable y)
		{
			float num = SortingHelpers.s_InteractableDistanceSqrMap[x];
			float value = SortingHelpers.s_InteractableDistanceSqrMap[y];
			return num.CompareTo(value);
		}

		private static readonly Dictionary<IXRInteractable, float> s_InteractableDistanceSqrMap = new Dictionary<IXRInteractable, float>();

		private static readonly Comparison<IXRInteractable> s_InteractableDistanceComparison = new Comparison<IXRInteractable>(SortingHelpers.InteractableDistanceComparison);

		public static readonly IInteractorDistanceEvaluator squareDistanceAttachPointEvaluator = new SortingHelpers.SquareDistanceAttachPointEvaluator();

		public static readonly IInteractorDistanceEvaluator interactableBasedEvaluator = new SortingHelpers.InteractableBasedEvaluator();

		public static readonly IInteractorDistanceEvaluator closestPointOnColliderEvaluator = new SortingHelpers.ClosestPointOnColliderEvaluator();

		private class InteractableBasedEvaluator : IInteractorDistanceEvaluator
		{
			public float EvaluateDistance(IXRInteractor interactor, IXRInteractable interactable)
			{
				return interactable.GetDistanceSqrToInteractor(interactor);
			}
		}

		private class ClosestPointOnColliderEvaluator : IInteractorDistanceEvaluator
		{
			public float EvaluateDistance(IXRInteractor interactor, IXRInteractable interactable)
			{
				float3 v = interactor.GetAttachTransform(interactable).position;
				DistanceInfo distanceInfo;
				XRInteractableUtility.TryGetClosestPointOnCollider(interactable, v, out distanceInfo);
				return distanceInfo.distanceSqr;
			}
		}

		[BurstCompile]
		private class SquareDistanceAttachPointEvaluator : IInteractorDistanceEvaluator
		{
			public float EvaluateDistance(IXRInteractor interactor, IXRInteractable interactable)
			{
				float3 @float = interactor.GetAttachTransform(interactable).position;
				float3 float2 = interactable.GetAttachTransform(interactor).position;
				return SortingHelpers.SquareDistanceAttachPointEvaluator.SqDistanceToInteractable(@float, float2);
			}

			[BurstCompile]
			[MonoPInvokeCallback(typeof(SortingHelpers.SquareDistanceAttachPointEvaluator.SqDistanceToInteractable_000017C3$PostfixBurstDelegate))]
			private static float SqDistanceToInteractable(in float3 attachPosition, in float3 interactablePosition)
			{
				return SortingHelpers.SquareDistanceAttachPointEvaluator.SqDistanceToInteractable_000017C3$BurstDirectCall.Invoke(attachPosition, interactablePosition);
			}

			[BurstCompile]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal static float SqDistanceToInteractable$BurstManaged(in float3 attachPosition, in float3 interactablePosition)
			{
				return math.lengthsq(attachPosition - interactablePosition);
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate float SqDistanceToInteractable_000017C3$PostfixBurstDelegate(in float3 attachPosition, in float3 interactablePosition);

			internal static class SqDistanceToInteractable_000017C3$BurstDirectCall
			{
				[BurstDiscard]
				private static void GetFunctionPointerDiscard(ref IntPtr A_0)
				{
					if (SortingHelpers.SquareDistanceAttachPointEvaluator.SqDistanceToInteractable_000017C3$BurstDirectCall.Pointer == 0)
					{
						SortingHelpers.SquareDistanceAttachPointEvaluator.SqDistanceToInteractable_000017C3$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<SortingHelpers.SquareDistanceAttachPointEvaluator.SqDistanceToInteractable_000017C3$PostfixBurstDelegate>(new SortingHelpers.SquareDistanceAttachPointEvaluator.SqDistanceToInteractable_000017C3$PostfixBurstDelegate(SortingHelpers.SquareDistanceAttachPointEvaluator.SqDistanceToInteractable)).Value;
					}
					A_0 = SortingHelpers.SquareDistanceAttachPointEvaluator.SqDistanceToInteractable_000017C3$BurstDirectCall.Pointer;
				}

				private static IntPtr GetFunctionPointer()
				{
					IntPtr result = (IntPtr)0;
					SortingHelpers.SquareDistanceAttachPointEvaluator.SqDistanceToInteractable_000017C3$BurstDirectCall.GetFunctionPointerDiscard(ref result);
					return result;
				}

				public static float Invoke(in float3 attachPosition, in float3 interactablePosition)
				{
					if (BurstCompiler.IsEnabled)
					{
						IntPtr functionPointer = SortingHelpers.SquareDistanceAttachPointEvaluator.SqDistanceToInteractable_000017C3$BurstDirectCall.GetFunctionPointer();
						if (functionPointer != 0)
						{
							return calli(System.Single(Unity.Mathematics.float3&,Unity.Mathematics.float3&), ref attachPosition, ref interactablePosition, functionPointer);
						}
					}
					return SortingHelpers.SquareDistanceAttachPointEvaluator.SqDistanceToInteractable$BurstManaged(attachPosition, interactablePosition);
				}

				private static IntPtr Pointer;
			}
		}
	}
}
