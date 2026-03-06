using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.Examples
{
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_procedural_world.php")]
	public class ProceduralWorld : MonoBehaviour
	{
		private void Start()
		{
			this.Update();
			AstarPath.active.Scan(null);
			base.StartCoroutine(this.GenerateTiles());
		}

		private void Update()
		{
			Int2 @int = new Int2(Mathf.RoundToInt((this.target.position.x - this.tileSize * 0.5f) / this.tileSize), Mathf.RoundToInt((this.target.position.z - this.tileSize * 0.5f) / this.tileSize));
			this.range = ((this.range < 1) ? 1 : this.range);
			bool flag = true;
			while (flag)
			{
				flag = false;
				foreach (KeyValuePair<Int2, ProceduralWorld.ProceduralTile> keyValuePair in this.tiles)
				{
					if (Mathf.Abs(keyValuePair.Key.x - @int.x) > this.range || Mathf.Abs(keyValuePair.Key.y - @int.y) > this.range)
					{
						keyValuePair.Value.Destroy();
						this.tiles.Remove(keyValuePair.Key);
						flag = true;
						break;
					}
				}
			}
			for (int i = @int.x - this.range; i <= @int.x + this.range; i++)
			{
				for (int j = @int.y - this.range; j <= @int.y + this.range; j++)
				{
					if (!this.tiles.ContainsKey(new Int2(i, j)))
					{
						ProceduralWorld.ProceduralTile proceduralTile = new ProceduralWorld.ProceduralTile(this, i, j);
						IEnumerator enumerator2 = proceduralTile.Generate();
						enumerator2.MoveNext();
						this.tileGenerationQueue.Enqueue(enumerator2);
						this.tiles.Add(new Int2(i, j), proceduralTile);
					}
				}
			}
			for (int k = @int.x - this.disableAsyncLoadWithinRange; k <= @int.x + this.disableAsyncLoadWithinRange; k++)
			{
				for (int l = @int.y - this.disableAsyncLoadWithinRange; l <= @int.y + this.disableAsyncLoadWithinRange; l++)
				{
					this.tiles[new Int2(k, l)].ForceFinish();
				}
			}
		}

		private IEnumerator GenerateTiles()
		{
			for (;;)
			{
				if (this.tileGenerationQueue.Count > 0)
				{
					IEnumerator routine = this.tileGenerationQueue.Dequeue();
					yield return base.StartCoroutine(routine);
				}
				yield return null;
			}
			yield break;
		}

		public Transform target;

		public ProceduralWorld.ProceduralPrefab[] prefabs;

		public int range = 1;

		public int disableAsyncLoadWithinRange = 1;

		public float tileSize = 100f;

		public int subTiles = 20;

		public bool staticBatching;

		private Queue<IEnumerator> tileGenerationQueue = new Queue<IEnumerator>();

		private Dictionary<Int2, ProceduralWorld.ProceduralTile> tiles = new Dictionary<Int2, ProceduralWorld.ProceduralTile>();

		public enum RotationRandomness
		{
			AllAxes,
			Y
		}

		[Serializable]
		public class ProceduralPrefab
		{
			public GameObject prefab;

			public float density;

			public float perlin;

			public float perlinPower = 1f;

			public Vector2 perlinOffset = Vector2.zero;

			public float perlinScale = 1f;

			public float random = 1f;

			public ProceduralWorld.RotationRandomness randomRotation;

			public bool singleFixed;
		}

		private class ProceduralTile
		{
			public bool destroyed { get; private set; }

			public ProceduralTile(ProceduralWorld world, int x, int z)
			{
				this.x = x;
				this.z = z;
				this.world = world;
				this.rnd = new Random(x * 10007 ^ z * 36007);
			}

			public IEnumerator Generate()
			{
				this.ie = this.InternalGenerate();
				GameObject gameObject = new GameObject("Tile " + this.x.ToString() + " " + this.z.ToString());
				this.root = gameObject.transform;
				while (this.ie != null && this.root != null && this.ie.MoveNext())
				{
					yield return this.ie.Current;
				}
				this.ie = null;
				yield break;
			}

			public void ForceFinish()
			{
				while (this.ie != null && this.root != null && this.ie.MoveNext())
				{
				}
				this.ie = null;
			}

			private Vector3 RandomInside()
			{
				return new Vector3
				{
					x = ((float)this.x + (float)this.rnd.NextDouble()) * this.world.tileSize,
					z = ((float)this.z + (float)this.rnd.NextDouble()) * this.world.tileSize
				};
			}

			private Vector3 RandomInside(float px, float pz)
			{
				return new Vector3
				{
					x = (px + (float)this.rnd.NextDouble() / (float)this.world.subTiles) * this.world.tileSize,
					z = (pz + (float)this.rnd.NextDouble() / (float)this.world.subTiles) * this.world.tileSize
				};
			}

			private Quaternion RandomYRot(ProceduralWorld.ProceduralPrefab prefab)
			{
				if (prefab.randomRotation != ProceduralWorld.RotationRandomness.AllAxes)
				{
					return Quaternion.Euler(0f, 360f * (float)this.rnd.NextDouble(), 0f);
				}
				return Quaternion.Euler(360f * (float)this.rnd.NextDouble(), 360f * (float)this.rnd.NextDouble(), 360f * (float)this.rnd.NextDouble());
			}

			private IEnumerator InternalGenerate()
			{
				Debug.Log("Generating tile " + this.x.ToString() + ", " + this.z.ToString());
				int counter = 0;
				float[,] ditherMap = new float[this.world.subTiles + 2, this.world.subTiles + 2];
				int num3;
				for (int i = 0; i < this.world.prefabs.Length; i = num3 + 1)
				{
					ProceduralWorld.ProceduralPrefab pref = this.world.prefabs[i];
					if (pref.singleFixed)
					{
						Vector3 position = new Vector3(((float)this.x + 0.5f) * this.world.tileSize, 0f, ((float)this.z + 0.5f) * this.world.tileSize);
						Object.Instantiate<GameObject>(pref.prefab, position, Quaternion.identity).transform.parent = this.root;
					}
					else
					{
						float subSize = this.world.tileSize / (float)this.world.subTiles;
						for (int k = 0; k < this.world.subTiles; k++)
						{
							for (int l = 0; l < this.world.subTiles; l++)
							{
								ditherMap[k + 1, l + 1] = 0f;
							}
						}
						for (int sx = 0; sx < this.world.subTiles; sx = num3 + 1)
						{
							for (int sz = 0; sz < this.world.subTiles; sz = num3 + 1)
							{
								float px = (float)this.x + (float)sx / (float)this.world.subTiles;
								float pz = (float)this.z + (float)sz / (float)this.world.subTiles;
								float b = Mathf.Pow(Mathf.PerlinNoise((px + pref.perlinOffset.x) * pref.perlinScale, (pz + pref.perlinOffset.y) * pref.perlinScale), pref.perlinPower);
								float num = pref.density * Mathf.Lerp(1f, b, pref.perlin) * Mathf.Lerp(1f, (float)this.rnd.NextDouble(), pref.random);
								float num2 = subSize * subSize * num + ditherMap[sx + 1, sz + 1];
								int count = Mathf.RoundToInt(num2);
								ditherMap[sx + 1 + 1, sz + 1] += 0.4375f * (num2 - (float)count);
								ditherMap[sx + 1 - 1, sz + 1 + 1] += 0.1875f * (num2 - (float)count);
								ditherMap[sx + 1, sz + 1 + 1] += 0.3125f * (num2 - (float)count);
								ditherMap[sx + 1 + 1, sz + 1 + 1] += 0.0625f * (num2 - (float)count);
								for (int j = 0; j < count; j = num3 + 1)
								{
									Vector3 position2 = this.RandomInside(px, pz);
									Object.Instantiate<GameObject>(pref.prefab, position2, this.RandomYRot(pref)).transform.parent = this.root;
									num3 = counter;
									counter = num3 + 1;
									if (counter % 2 == 0)
									{
										yield return null;
									}
									num3 = j;
								}
								num3 = sz;
							}
							num3 = sx;
						}
					}
					pref = null;
					num3 = i;
				}
				ditherMap = null;
				yield return null;
				yield return null;
				if (Application.HasProLicense() && this.world.staticBatching)
				{
					StaticBatchingUtility.Combine(this.root.gameObject);
				}
				yield break;
			}

			public void Destroy()
			{
				if (this.root != null)
				{
					Debug.Log("Destroying tile " + this.x.ToString() + ", " + this.z.ToString());
					Object.Destroy(this.root.gameObject);
					this.root = null;
				}
				this.ie = null;
			}

			private int x;

			private int z;

			private Random rnd;

			private ProceduralWorld world;

			private Transform root;

			private IEnumerator ie;
		}
	}
}
