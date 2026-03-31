using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	public class StayOutsideOfGeo : MonoBehaviour
	{
		private void Reset()
		{
			this._target = base.transform;
		}

		private void Start()
		{
			if (this._target == null)
			{
				this._target = base.transform;
			}
			this._voxelWorld = VoxelWorld.GetFor(base.gameObject);
			if (this._voxelWorld == null)
			{
				Debug.LogError("VoxelWorld not found in the scene. Please ensure there is a VoxelWorld component present.");
				base.enabled = false;
			}
		}

		private void Update()
		{
			int3 voxelForWorldPosition = this._voxelWorld.GetVoxelForWorldPosition(this._target.position + this._targetOffset);
			if (this.TestPosition(voxelForWorldPosition, true).Item1)
			{
				this.AddPositionToHistory(voxelForWorldPosition);
				return;
			}
			if (this.ResolvePenetration(voxelForWorldPosition))
			{
				Debug.Log(string.Format("Successfully resolved penetration for {0} at position {1}", this._target.name, voxelForWorldPosition), this);
				return;
			}
			for (int i = 10; i < 100; i += 10)
			{
				for (int j = 0; j < 10; j++)
				{
					int x = voxelForWorldPosition.x + UnityEngine.Random.Range(-i, i);
					int y = voxelForWorldPosition.y + UnityEngine.Random.Range(-i, i);
					int z = voxelForWorldPosition.z + UnityEngine.Random.Range(-i, i);
					int3 @int = new int3(x, y, z);
					if (this.IsOutsideGeo(@int))
					{
						Debug.Log(string.Format("Found valid random position {0} outside geo for {1}", @int, this._target.name), this);
						this.SetPosition(this._voxelWorld.GetWorldPosition(@int));
						return;
					}
				}
			}
		}

		private bool ResolvePenetration(int3 pos)
		{
			Debug.LogWarning(string.Format("{0} inside geo in {1} [{2}={3}->{4:F2}]", new object[]
			{
				base.name,
				this._voxelWorld.GetChunkForLocalPosition(pos),
				this._target.position.RoundToInt(),
				this._voxelWorld.GetVoxelForWorldPosition(this._target.position),
				this._maxDensity
			}), this);
			for (int i = 0; i < this._positionHistory.Count; i++)
			{
				int3 @int = this.PopMostRecentPosition();
				if (!@int.Equals(int3.zero))
				{
					ValueTuple<bool, int3> valueTuple = this.TestPosition(@int, false);
					if (valueTuple.Item1)
					{
						Debug.Log(string.Format("Found valid position {0} near recent position {1} at index {2}", valueTuple.Item2, @int, i));
						this.SetPosition(this._voxelWorld.GetWorldPosition(valueTuple.Item2));
						return true;
					}
				}
			}
			return false;
		}

		private void SetPosition(Vector3 worldPosition)
		{
			Debug.Log(string.Format("Moving {0} from {1} to {2}", this._target, this._target.position, worldPosition), this);
			Debug.DrawLine(this._target.position, worldPosition, Color.red, 5f);
			if (this._disableOnMove)
			{
				this._disableOnMove.enabled = false;
			}
			this._target.position = worldPosition;
			if (this._disableOnMove)
			{
				this._disableOnMove.enabled = true;
			}
		}

		[return: TupleElementNames(new string[]
		{
			"match",
			"position"
		})]
		private ValueTuple<bool, int3> TestPosition(int3 position, bool useThreshold = true)
		{
			float num = this._voxelWorld.GetDensityAt(position, 0).ToFloat();
			this._maxDensity = Mathf.Max(num, float.MinValue);
			this._minDensity = Mathf.Min(num, float.MaxValue);
			float num2 = useThreshold ? this._threshold : 0f;
			if (this._voxelWorld.GetDensityAt(position, 0).ToFloat() < num2)
			{
				for (int i = position.x - 1; i <= position.x + 1; i++)
				{
					for (int j = position.y - 1; j <= position.y + 1; j++)
					{
						for (int k = position.z - 1; k <= position.z + 1; k++)
						{
							num = this._voxelWorld.GetDensityAt(new int3(i, j, k), 0).ToFloat();
							this._maxDensity = Mathf.Max(num, this._maxDensity);
							this._minDensity = Mathf.Min(num, this._minDensity);
							if (num < 0f)
							{
								return new ValueTuple<bool, int3>(true, new int3(i, j, k));
							}
						}
					}
				}
			}
			return new ValueTuple<bool, int3>(false, position);
		}

		private bool IsOutsideGeo(int3 position)
		{
			return this._voxelWorld.GetDensityAt(position, 0).ToFloat() < 0f;
		}

		private void AddPositionToHistory(int3 position)
		{
			if (this._positionHistory.Contains(position))
			{
				return;
			}
			if (this._positionHistory.Count >= this._maxHistorySize)
			{
				this._positionHistory[this._historyIndex] = position;
			}
			else
			{
				this._positionHistory.Add(position);
			}
			this._historyIndex = (this._historyIndex + 1) % this._maxHistorySize;
		}

		private int3 GetMostRecentPosition()
		{
			if (this._positionHistory.Count == 0)
			{
				return int3.zero;
			}
			return this._positionHistory[(this._historyIndex - 1 + this._positionHistory.Count) % this._positionHistory.Count];
		}

		private int3 PopMostRecentPosition()
		{
			if (this._positionHistory.Count == 0)
			{
				return int3.zero;
			}
			this._historyIndex = (this._historyIndex - 1 + this._maxHistorySize) % this._maxHistorySize;
			return this._positionHistory[this._historyIndex];
		}

		[SerializeField]
		private Transform _target;

		[SerializeField]
		private Vector3 _targetOffset = Vector3.zero;

		[SerializeField]
		private float _threshold = 0.4f;

		[SerializeField]
		private bool _pauseOnPenetration = true;

		[SerializeField]
		private Collider _disableOnMove;

		private VoxelWorld _voxelWorld;

		private List<int3> _positionHistory = new List<int3>();

		private int _maxHistorySize = 10;

		private int _historyIndex;

		private float _maxDensity;

		private float _minDensity;
	}
}
