using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oculus.Interaction
{
	public class SequentialSlotsProvider : MonoBehaviour, ISnapPoseDelegate
	{
		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._slotInteractors = new int[this._slots.Count];
			this.EndStart(ref this._started);
		}

		public void TrackElement(int id, Pose pose)
		{
			int num = this.FindBestSlotIndex(pose.position, false);
			if (this.TryOccupySlot(num))
			{
				this._slotInteractors[num] = id;
			}
		}

		public void UntrackElement(int id)
		{
			int num;
			if (this.TryFindIndexForInteractor(id, out num))
			{
				this._slotInteractors[num] = 0;
			}
		}

		public void SnapElement(int id, Pose pose)
		{
		}

		public void UnsnapElement(int id)
		{
		}

		public void MoveTrackedElement(int id, Pose pose)
		{
			int num = this.FindBestSlotIndex(pose.position, false);
			int num2;
			if (this.TryFindIndexForInteractor(id, out num2))
			{
				if (num != num2)
				{
					this._slotInteractors[num2] = 0;
					if (this.TryOccupySlot(num))
					{
						this._slotInteractors[num] = id;
						return;
					}
				}
			}
			else if (this.TryOccupySlot(num))
			{
				this._slotInteractors[num] = id;
			}
		}

		private bool TryFindIndexForInteractor(int id, out int index)
		{
			index = Array.FindIndex<int>(this._slotInteractors, (int i) => i == id);
			return index >= 0;
		}

		public bool SnapPoseForElement(int id, Pose pose, out Pose result)
		{
			int index;
			if (this.TryFindIndexForInteractor(id, out index))
			{
				result = this._slots[index].GetPose(Space.World);
				return true;
			}
			int num = this.FindBestSlotIndex(pose.position, true);
			if (num >= 0)
			{
				result = this._slots[num].GetPose(Space.World);
				return true;
			}
			result = Pose.identity;
			return false;
		}

		private bool TryOccupySlot(int index)
		{
			if (this.IsSlotFree(index))
			{
				return true;
			}
			Vector3 position = this._slots[index].position;
			int num = this.FindBestSlotIndex(position, true);
			if (num < 0)
			{
				return false;
			}
			this.PushSlots(index, num);
			return true;
		}

		private bool IsSlotFree(int index)
		{
			return this._slotInteractors[index] == 0;
		}

		private int FindBestSlotIndex(in Vector3 target, bool freeOnly = false)
		{
			int result = -1;
			float num = float.PositiveInfinity;
			for (int i = 0; i < this._slots.Count; i++)
			{
				if (!freeOnly || this.IsSlotFree(i))
				{
					float sqrMagnitude = (target - this._slots[i].position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						result = i;
					}
				}
			}
			return result;
		}

		private void PushSlots(int index, int freeSlot)
		{
			SequentialSlotsProvider.<>c__DisplayClass14_0 CS$<>8__locals1;
			CS$<>8__locals1.forwardDirection = (index > freeSlot);
			for (int num = freeSlot; num != index; num = SequentialSlotsProvider.<PushSlots>g__Next|14_0(num, ref CS$<>8__locals1))
			{
				int freeSlot2 = SequentialSlotsProvider.<PushSlots>g__Next|14_0(num, ref CS$<>8__locals1);
				this.SwapSlot(num, freeSlot2);
			}
		}

		private void SwapSlot(int index, int freeSlot)
		{
			ref int ptr = ref this._slotInteractors[index];
			int[] slotInteractors = this._slotInteractors;
			int num = this._slotInteractors[freeSlot];
			int num2 = this._slotInteractors[index];
			ptr = num;
			slotInteractors[freeSlot] = num2;
		}

		public void InjectAllSequentialSlotsProvider(List<Transform> slots)
		{
			this.InjectSlots(slots);
		}

		public void InjectSlots(List<Transform> slots)
		{
			this._slots = slots;
		}

		[CompilerGenerated]
		internal static int <PushSlots>g__Next|14_0(int value, ref SequentialSlotsProvider.<>c__DisplayClass14_0 A_1)
		{
			return value + (A_1.forwardDirection ? 1 : -1);
		}

		[SerializeField]
		private List<Transform> _slots;

		private int[] _slotInteractors;

		protected bool _started;
	}
}
