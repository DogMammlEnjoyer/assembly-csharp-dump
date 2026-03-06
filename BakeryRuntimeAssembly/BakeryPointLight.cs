using System;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class BakeryPointLight : MonoBehaviour
{
	public int UID;

	public Color color = Color.white;

	public float intensity = 1f;

	public float shadowSpread = 0.05f;

	public float cutoff = 10f;

	public bool realisticFalloff;

	public int samples = 8;

	public BakeryPointLight.ftLightProjectionMode projMode;

	public Texture2D cookie;

	public float angle = 30f;

	public float innerAngle;

	public Cubemap cubemap;

	public Object iesFile;

	public int bitmask = 1;

	public bool bakeToIndirect;

	public bool shadowmask;

	public float indirectIntensity = 1f;

	public float falloffMinRadius = 1f;

	public int shadowmaskGroupID;

	public BakeryPointLight.Direction directionMode;

	private const float GIZMO_MAXSIZE = 0.15f;

	private const float GIZMO_SCALE = 0.05f;

	private float screenRadius = 0.15f;

	public static int lightsChanged;

	public enum ftLightProjectionMode
	{
		Omni,
		Cookie,
		Cubemap,
		IES,
		Cone
	}

	public enum Direction
	{
		NegativeY,
		PositiveZ
	}
}
