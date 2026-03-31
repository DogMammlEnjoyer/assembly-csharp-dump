using System;

[Serializable]
public struct VoxelAction
{
	public float strength;

	public float radius;

	public OperationType operation;

	public byte material;
}
