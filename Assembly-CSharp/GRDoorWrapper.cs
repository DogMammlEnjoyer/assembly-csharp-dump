using System;
using UnityEngine;

public class GRDoorWrapper : MonoBehaviour
{
	public void ToggleDoor(bool value)
	{
		this.grDoor.SetDoorState(value ? GRDoor.DoorState.Open : GRDoor.DoorState.Closed);
	}

	[SerializeField]
	private GRDoor grDoor;
}
