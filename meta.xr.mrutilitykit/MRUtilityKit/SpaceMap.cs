using System;
using System.Collections;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{
	[Obsolete("SpaceMap is deprecated. Please use SpaceMapGPU instead.", false)]
	[Feature(Feature.Scene)]
	public class SpaceMap : MonoBehaviour
	{
		[Obsolete("SpaceMap is deprecated. Please use SpaceMapGPU instead.", false)]
		public Vector2 Offset
		{
			get
			{
				return new Vector2(this.MapBounds.center.x, this.MapBounds.center.z);
			}
		}

		[Obsolete("SpaceMap is deprecated. Please use SpaceMapGPU instead.", false)]
		public Vector2 Scale
		{
			get
			{
				return new Vector2(Mathf.Max(this.MapBounds.size.x, this.MapBounds.size.z) + this.MapBorder * 2f, Mathf.Max(this.MapBounds.size.x, this.MapBounds.size.z) + this.MapBorder * 2f);
			}
		}

		private void Start()
		{
			if (MRUK.Instance && this.CreateOnStart != MRUK.RoomFilter.None)
			{
				MRUK.Instance.RegisterSceneLoadedCallback(delegate
				{
					MRUK.RoomFilter createOnStart = this.CreateOnStart;
					if (createOnStart != MRUK.RoomFilter.CurrentRoomOnly)
					{
						if (createOnStart == MRUK.RoomFilter.AllRooms)
						{
							this.CalculateMap(null);
							return;
						}
					}
					else
					{
						this.CalculateMap(MRUK.Instance.GetCurrentRoom());
					}
				});
			}
		}

		[Obsolete("SpaceMap is deprecated. Please use SpaceMapGPU's 'StartSpaceMap' method instead.", false)]
		public void CalculateMap(MRUKRoom room = null)
		{
			if (this.TextureMap == null)
			{
				Debug.LogWarning("No texture specified for Space Map");
				return;
			}
			this.InitializeMapValues(room);
			base.StartCoroutine(this.CalculatePixels(room));
			Shader.SetGlobalTexture("_SpaceMap", this.TextureMap);
			Shader.SetGlobalVector("_SpaceMapParams", new Vector4(this.Scale.x, this.Scale.y, this.Offset.x, this.Offset.y));
		}

		private void InitializeMapValues(MRUKRoom room)
		{
			this.PixelDimensions = this.TextureMap.width;
			this.Pixels = new Color[this.PixelDimensions, this.PixelDimensions];
			if (room != null)
			{
				this.MapBounds = room.GetRoomBounds();
			}
			else
			{
				this.MapBounds = default(Bounds);
				foreach (MRUKRoom mrukroom in MRUK.Instance.Rooms)
				{
					this.MapBounds.Encapsulate(mrukroom.GetRoomBounds());
				}
			}
			base.transform.position = new Vector3(this.MapBounds.center.x, this.MapBounds.min.y, this.MapBounds.center.z);
			base.transform.localScale = new Vector3(this.Scale.x, this.MapBounds.size.y, this.Scale.y);
		}

		[Obsolete("SpaceMap is deprecated. Please use SpaceMapGPU instead.", false)]
		public float GetSurfaceDistance(MRUKRoom room, Vector3 worldPosition)
		{
			float num = float.PositiveInfinity;
			float num2 = 1f;
			if (room != null)
			{
				Vector3 vector;
				MRUKAnchor mrukanchor;
				num = room.TryGetClosestSurfacePosition(worldPosition, out vector, out mrukanchor, new LabelFilter(new MRUKAnchor.SceneLabels?(~(MRUKAnchor.SceneLabels.FLOOR | MRUKAnchor.SceneLabels.CEILING)), null));
				num2 = (float)(room.IsPositionInRoom(worldPosition, false) ? 1 : -1);
			}
			else
			{
				foreach (MRUKRoom mrukroom in MRUK.Instance.Rooms)
				{
					Vector3 vector2;
					MRUKAnchor mrukanchor2;
					float num3 = mrukroom.TryGetClosestSurfacePosition(worldPosition, out vector2, out mrukanchor2, new LabelFilter(new MRUKAnchor.SceneLabels?(~(MRUKAnchor.SceneLabels.FLOOR | MRUKAnchor.SceneLabels.CEILING)), null));
					if (num3 < num)
					{
						num = num3;
						num2 = (float)(mrukroom.IsPositionInRoom(worldPosition, true) ? 1 : -1);
					}
				}
			}
			return num * num2;
		}

		private IEnumerator CalculatePixels(MRUKRoom room)
		{
			float num = 0.5f / (float)this.PixelDimensions;
			float num2 = Mathf.Max(this.MapBounds.size.x, this.MapBounds.size.z) + this.MapBorder * 2f;
			for (int i = 0; i < this.PixelDimensions; i++)
			{
				for (int j = 0; j < this.PixelDimensions; j++)
				{
					float num3 = (float)i / (float)this.PixelDimensions - 0.5f + num;
					float num4 = (float)j / (float)this.PixelDimensions - 0.5f + num;
					Vector3 worldPosition = new Vector3(num3 * num2 + this.MapBounds.center.x, 0f, num4 * num2 + this.MapBounds.center.z);
					float time = Mathf.Clamp01((-this.GetSurfaceDistance(room, worldPosition) - this.InnerBorder) / (this.OuterBorder - this.InnerBorder));
					Color color = this.MapGradient.Evaluate(time);
					this.Pixels[i, j] = color;
					this.TextureMap.SetPixel(i, j, color);
				}
			}
			this.TextureMap.Apply();
			yield return null;
			yield break;
		}

		[Obsolete("SpaceMap is deprecated. Please use SpaceMapGPU instead.", false)]
		public void ResetFreespace()
		{
			for (int i = 0; i < this.PixelDimensions; i++)
			{
				for (int j = 0; j < this.PixelDimensions; j++)
				{
					this.Pixels[i, j] = Color.black;
				}
			}
		}

		[Obsolete("SpaceMap is deprecated. Please use SpaceMapGPU's 'GetColorAtPosition' method instead.", false)]
		public Color GetColorAtPosition(Vector3 worldPosition, bool getBilinear = true)
		{
			if (getBilinear)
			{
				Vector2 pixelFromWorldPosition = this.GetPixelFromWorldPosition(worldPosition, true);
				return this.TextureMap.GetPixelBilinear(pixelFromWorldPosition.x, pixelFromWorldPosition.y);
			}
			Vector2 pixelFromWorldPosition2 = this.GetPixelFromWorldPosition(worldPosition, false);
			int x = Mathf.FloorToInt(pixelFromWorldPosition2.x);
			int y = Mathf.FloorToInt(pixelFromWorldPosition2.y);
			return this.TextureMap.GetPixel(x, y);
		}

		private Vector2 GetPixelFromWorldPosition(Vector3 worldPosition, bool normalizedUV = false)
		{
			Vector3 vector = worldPosition - this.MapBounds.center;
			Vector2 vector2 = new Vector2(vector.x / this.MapBounds.size.x + 0.5f, vector.z / this.MapBounds.size.z + 0.5f);
			if (!normalizedUV)
			{
				vector2 *= (float)this.PixelDimensions;
			}
			return vector2;
		}

		[Tooltip("When the scene data is loaded, this controls what room(s) the prefabs will spawn in.")]
		[Obsolete("SpaceMap is deprecated. Please use SpaceMapGPU instead.", false)]
		public MRUK.RoomFilter CreateOnStart = MRUK.RoomFilter.CurrentRoomOnly;

		[Tooltip("Texture requirements: Read/Write enabled, RGBA 32 bit format. Texture suggestions: Wrap Mode = Clamped, size small (<128x128)")]
		[Obsolete("SpaceMap is deprecated. Please use SpaceMapGPU instead.", false)]
		public Texture2D TextureMap;

		private Bounds MapBounds;

		private Color[,] Pixels;

		private int PixelDimensions = 128;

		[Tooltip("The gradient of the generated map.")]
		[Obsolete("SpaceMap is deprecated. Please use SpaceMapGPU instead.", false)]
		public Gradient MapGradient = new Gradient();

		[Tooltip("How far inside the room the left end of the Texture Gradient should appear. 0 is at the surface, negative is inside the room.")]
		[Obsolete("SpaceMap is deprecated. Please use SpaceMapGPU instead.", false)]
		public float InnerBorder = -0.5f;

		[Tooltip("How far outside the room the right end of the Texture Gradient should appear. 0 is at the surface, positive is outside the room.")]
		[Obsolete("SpaceMap is deprecated. Please use SpaceMapGPU instead.", false)]
		public float OuterBorder;

		[Tooltip("How much the texture map should extend from the room bounds, in meters. Should ideally be greater than or equal to outerPosition.")]
		[Obsolete("SpaceMap is deprecated. Please use SpaceMapGPU instead.", false)]
		public float MapBorder;

		private const string MATERIAL_PROPERTY_NAME = "_SpaceMap";

		private const string PARAMETER_PROPERTY_NAME = "_SpaceMapParams";
	}
}
