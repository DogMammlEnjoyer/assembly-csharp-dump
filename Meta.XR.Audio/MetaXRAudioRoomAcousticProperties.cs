using System;
using UnityEngine;

public sealed class MetaXRAudioRoomAcousticProperties : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod]
	private static void CheckSceneHasRoom()
	{
		MetaXRAudioRoomAcousticProperties[] array = Object.FindObjectsOfType<MetaXRAudioRoomAcousticProperties>();
		if (array.Length == 0)
		{
			Debug.Log("No Meta XR Audio Room found, setting default room");
			GameObject gameObject = new GameObject("Temporary Room");
			gameObject.AddComponent<MetaXRAudioRoomAcousticProperties>().Update();
			Object.DestroyImmediate(gameObject);
		}
		if (array.Length > 1)
		{
			Debug.LogError("Multiple Meta XR Audio Rooms found, only one is allowed!");
		}
	}

	private void Update()
	{
		this.SetWallMaterialPreset(0, this.rightMaterial);
		this.SetWallMaterialPreset(1, this.leftMaterial);
		this.SetWallMaterialPreset(2, this.ceilingMaterial);
		this.SetWallMaterialPreset(3, this.floorMaterial);
		this.SetWallMaterialPreset(4, this.frontMaterial);
		this.SetWallMaterialPreset(5, this.backMaterial);
		MetaXRAudioNativeInterface.Interface.SetAdvancedBoxRoomParameters(this.width, this.height, this.depth, this.lockPositionToListener, base.transform.position, this.wallMaterials);
		float num = this.clutterFactor;
		for (int i = 3; i >= 0; i--)
		{
			this.clutterFactorBands[i] = num;
			num *= 0.5f;
		}
		MetaXRAudioNativeInterface.Interface.SetRoomClutterFactor(this.clutterFactorBands);
	}

	private void SetWallMaterialPreset(int wallIndex, MetaXRAudioRoomAcousticProperties.MaterialPreset materialPreset)
	{
		switch (materialPreset)
		{
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.AcousticTile:
			this.SetWallMaterialProperties(wallIndex, 0.48816842f, 0.36147523f, 0.33959538f, 0.49894625f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Brick:
			this.SetWallMaterialProperties(wallIndex, 0.9754688f, 0.9720645f, 0.9491802f, 0.9301054f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.BrickPainted:
			this.SetWallMaterialProperties(wallIndex, 0.9757106f, 0.98332417f, 0.9781167f, 0.9700527f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Cardboard:
			this.SetWallMaterialProperties(wallIndex, 0.59f, 0.435728f, 0.25165f, 0.208f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Carpet:
			this.SetWallMaterialProperties(wallIndex, 0.9876337f, 0.90548664f, 0.5831106f, 0.35105383f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.CarpetHeavy:
			this.SetWallMaterialProperties(wallIndex, 0.9776337f, 0.8590829f, 0.5264796f, 0.37079042f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.CarpetHeavyPadded:
			this.SetWallMaterialProperties(wallIndex, 0.91053474f, 0.5304332f, 0.29405582f, 0.27010542f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.CeramicTile:
			this.SetWallMaterialProperties(wallIndex, 0.99f, 0.99f, 0.98275393f, 0.98f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Concrete:
			this.SetWallMaterialProperties(wallIndex, 0.99f, 0.98332417f, 0.98f, 0.98f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.ConcreteRough:
			this.SetWallMaterialProperties(wallIndex, 0.98940843f, 0.96449465f, 0.922127f, 0.90010536f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.ConcreteBlock:
			this.SetWallMaterialProperties(wallIndex, 0.6352674f, 0.6522307f, 0.67105347f, 0.7890516f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.ConcreteBlockPainted:
			this.SetWallMaterialProperties(wallIndex, 0.9029579f, 0.9402359f, 0.91758406f, 0.9199473f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Curtain:
			this.SetWallMaterialProperties(wallIndex, 0.68649423f, 0.54586f, 0.31007856f, 0.39947313f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Foliage:
			this.SetWallMaterialProperties(wallIndex, 0.51825935f, 0.5035683f, 0.5786888f, 0.6902108f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Glass:
			this.SetWallMaterialProperties(wallIndex, 0.6559158f, 0.8006318f, 0.9188397f, 0.92348814f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.GlassHeavy:
			this.SetWallMaterialProperties(wallIndex, 0.82709897f, 0.95022273f, 0.9746041f, 0.98f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Grass:
			this.SetWallMaterialProperties(wallIndex, 0.8811263f, 0.5071708f, 0.1318931f, 0.010368884f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Gravel:
			this.SetWallMaterialProperties(wallIndex, 0.7292947f, 0.37312245f, 0.25531745f, 0.20026344f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.GypsumBoard:
			this.SetWallMaterialProperties(wallIndex, 0.72124004f, 0.92769015f, 0.9343023f, 0.9101054f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Marble:
			this.SetWallMaterialProperties(wallIndex, 0.99f, 0.99f, 0.982754f, 0.98f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Mud:
			this.SetWallMaterialProperties(wallIndex, 0.844084f, 0.726577f, 0.794683f, 0.849737f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.PlasterOnBrick:
			this.SetWallMaterialProperties(wallIndex, 0.9756965f, 0.979106f, 0.9610635f, 0.9500527f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.PlasterOnConcreteBlock:
			this.SetWallMaterialProperties(wallIndex, 0.8817747f, 0.92477393f, 0.95149755f, 0.9599473f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Rubber:
			this.SetWallMaterialProperties(wallIndex, 0.95f, 0.916621f, 0.93623f, 0.95f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Soil:
			this.SetWallMaterialProperties(wallIndex, 0.8440842f, 0.63462424f, 0.41666287f, 0.40000004f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.SoundProof:
			this.SetWallMaterialProperties(wallIndex, 0f, 0f, 0f, 0f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Snow:
			this.SetWallMaterialProperties(wallIndex, 0.53225267f, 0.15453577f, 0.050964415f, 0.050000012f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Steel:
			this.SetWallMaterialProperties(wallIndex, 0.7931117f, 0.8401404f, 0.92559177f, 0.97973657f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Stone:
			this.SetWallMaterialProperties(wallIndex, 0.98f, 0.97874f, 0.955701f, 0.95f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Vent:
			this.SetWallMaterialProperties(wallIndex, 0.847042f, 0.62045f, 0.70217f, 0.799473f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.Water:
			this.SetWallMaterialProperties(wallIndex, 0.97058827f, 0.9717535f, 0.9783096f, 0.9700527f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.WoodThin:
			this.SetWallMaterialProperties(wallIndex, 0.59242314f, 0.8582733f, 0.9172423f, 0.94f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.WoodThick:
			this.SetWallMaterialProperties(wallIndex, 0.8129579f, 0.8953296f, 0.9413047f, 0.9499473f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.WoodFloor:
			this.SetWallMaterialProperties(wallIndex, 0.8523663f, 0.8989921f, 0.9347841f, 0.9300527f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.WoodOnConcrete:
			this.SetWallMaterialProperties(wallIndex, 0.96f, 0.94123226f, 0.9379238f, 0.9300527f);
			return;
		case MetaXRAudioRoomAcousticProperties.MaterialPreset.MetaDefault:
			this.SetWallMaterialProperties(wallIndex, 0.9f, 0.9f, 0.9f, 0.9f);
			return;
		default:
			return;
		}
	}

	private void SetWallMaterialProperties(int wallIndex, float band0, float band1, float band2, float band3)
	{
		this.wallMaterials[wallIndex * 4] = band0;
		this.wallMaterials[wallIndex * 4 + 1] = band1;
		this.wallMaterials[wallIndex * 4 + 2] = band2;
		this.wallMaterials[wallIndex * 4 + 3] = band3;
	}

	[Tooltip("Center the room model on the listener. When disabled, center the room model on the GameObject this script is attached to.")]
	public bool lockPositionToListener = true;

	[Tooltip("Width of the room model in meters")]
	public float width = 8f;

	[Tooltip("Height of the room model in meters")]
	public float height = 3f;

	[Tooltip("Depth of the room model in meters")]
	public float depth = 5f;

	[Tooltip("Material of the left wall of the room model")]
	public MetaXRAudioRoomAcousticProperties.MaterialPreset leftMaterial = MetaXRAudioRoomAcousticProperties.MaterialPreset.GypsumBoard;

	[Tooltip("Material of the right wall of the room model")]
	public MetaXRAudioRoomAcousticProperties.MaterialPreset rightMaterial = MetaXRAudioRoomAcousticProperties.MaterialPreset.GypsumBoard;

	[Tooltip("Material of the ceiling of the room model")]
	public MetaXRAudioRoomAcousticProperties.MaterialPreset ceilingMaterial;

	[Tooltip("Material of the floor of the room model")]
	public MetaXRAudioRoomAcousticProperties.MaterialPreset floorMaterial = MetaXRAudioRoomAcousticProperties.MaterialPreset.Carpet;

	[Tooltip("Material of the front wall of the room model")]
	public MetaXRAudioRoomAcousticProperties.MaterialPreset frontMaterial = MetaXRAudioRoomAcousticProperties.MaterialPreset.GypsumBoard;

	[Tooltip("Material of the back wall of the room model")]
	public MetaXRAudioRoomAcousticProperties.MaterialPreset backMaterial = MetaXRAudioRoomAcousticProperties.MaterialPreset.GypsumBoard;

	[Tooltip("Diffuses the reflections and reverberation to simulate objects inside the room. Zero represents a completely empty room.")]
	[Range(0f, 1f)]
	public float clutterFactor = 0.5f;

	private const int kAudioBandCount = 4;

	private float[] clutterFactorBands = new float[4];

	private float[] wallMaterials = new float[24];

	public enum MaterialPreset
	{
		AcousticTile,
		Brick,
		BrickPainted,
		Cardboard,
		Carpet,
		CarpetHeavy,
		CarpetHeavyPadded,
		CeramicTile,
		Concrete,
		ConcreteRough,
		ConcreteBlock,
		ConcreteBlockPainted,
		Curtain,
		Foliage,
		Glass,
		GlassHeavy,
		Grass,
		Gravel,
		GypsumBoard,
		Marble,
		Mud,
		PlasterOnBrick,
		PlasterOnConcreteBlock,
		Rubber,
		Soil,
		SoundProof,
		Snow,
		Steel,
		Stone,
		Vent,
		Water,
		WoodThin,
		WoodThick,
		WoodFloor,
		WoodOnConcrete,
		MetaDefault
	}
}
