using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

public class GhostReactorLevelGenerator : MonoBehaviourTick
{
	public List<GhostReactorLevelGeneratorV2.TreeLevelConfig> TreeLevels
	{
		get
		{
			return this.GetTreeLevels();
		}
	}

	private void Awake()
	{
		GameObject gameObject = new GameObject("TestColliderA");
		this.testColliderA = gameObject.AddComponent<BoxCollider>();
		this.testColliderA.isTrigger = true;
		gameObject.transform.SetParent(base.transform);
		gameObject.gameObject.SetActive(false);
		GameObject gameObject2 = new GameObject("TestColliderB");
		this.testColliderB = gameObject2.AddComponent<BoxCollider>();
		this.testColliderB.isTrigger = true;
		gameObject2.transform.SetParent(base.transform);
		gameObject2.gameObject.SetActive(false);
		this.nextVisCheckNodeIndex = 0;
	}

	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	public override void Tick()
	{
		Vector3 position = VRRig.LocalRig.transform.position;
		int num = Mathf.Min(1, this.nodeList.Count);
		for (int i = 0; i < num; i++)
		{
			if (this.nextVisCheckNodeIndex >= this.nodeList.Count)
			{
				this.nextVisCheckNodeIndex = 0;
			}
			if (this.nodeList[this.nextVisCheckNodeIndex] != null)
			{
				if (this.nodeList[this.nextVisCheckNodeIndex].sectionInstance != null)
				{
					this.nodeList[this.nextVisCheckNodeIndex].sectionInstance.UpdateDisable(position);
				}
				if (this.nodeList[this.nextVisCheckNodeIndex].connectorInstance != null)
				{
					this.nodeList[this.nextVisCheckNodeIndex].connectorInstance.UpdateDisable(position);
				}
				GhostReactorLevelGenerator.Node[] children = this.nodeList[this.nextVisCheckNodeIndex].children;
				for (int j = 0; j < children.Length; j++)
				{
					if (children[j] != null)
					{
						if (children[j].sectionInstance != null)
						{
							children[j].sectionInstance.UpdateDisable(position);
						}
						if (children[j].connectorInstance != null)
						{
							children[j].connectorInstance.UpdateDisable(position);
						}
					}
				}
				this.nextVisCheckNodeIndex++;
			}
		}
	}

	private List<GhostReactorLevelGeneratorV2.TreeLevelConfig> GetTreeLevels()
	{
		if (this.depthConfigs == null || this.depthConfigs.Count == 0)
		{
			return null;
		}
		List<GhostReactorLevelGeneratorV2.TreeLevelConfig> treeLevels = this.depthConfigs[Mathf.Clamp(this.reactor.GetDepthLevel(), 0, this.depthConfigs.Count - 1)].options[this.reactor.GetDepthConfigIndex()].levelConfig.treeLevels;
		List<GhostReactorLevelGeneratorV2.TreeLevelConfig> list = new List<GhostReactorLevelGeneratorV2.TreeLevelConfig>();
		foreach (GhostReactorLevelGeneratorV2.TreeLevelConfig treeLevelConfig in treeLevels)
		{
			if (GhostReactorLevelGenerator.TreeLevelIsEnabledNow(treeLevelConfig))
			{
				list.Add(treeLevelConfig);
			}
		}
		return list;
	}

	private static bool TreeLevelIsEnabledNow(GhostReactorLevelGeneratorV2.TreeLevelConfig treeLevel)
	{
		if (string.IsNullOrEmpty(treeLevel.EnableAfterDatetime) && string.IsNullOrEmpty(treeLevel.DisableAfterDatetime))
		{
			return true;
		}
		if (!string.IsNullOrEmpty(treeLevel.EnableAfterDatetime) && !string.IsNullOrEmpty(treeLevel.DisableAfterDatetime))
		{
			throw new ArgumentException("Both enable and disable after datetime are set--this should never happen!");
		}
		DateTime t = GorillaComputer.instance.GetServerTime().ToUniversalTime();
		if (!string.IsNullOrEmpty(treeLevel.EnableAfterDatetime))
		{
			DateTime t2 = DateTime.Parse(treeLevel.EnableAfterDatetime).ToUniversalTime();
			return t > t2;
		}
		DateTime t3 = DateTime.Parse(treeLevel.DisableAfterDatetime).ToUniversalTime();
		return t < t3;
	}

	private bool TestForCollision(GhostReactorLevelSection section, Vector3 position, Quaternion rotation, int selfi, int selfj, int selfk)
	{
		this.testColliderA.gameObject.SetActive(true);
		this.testColliderB.gameObject.SetActive(true);
		this.testColliderA.transform.position = position + rotation * section.BoundingCollider.transform.localPosition;
		this.testColliderA.transform.rotation = rotation * section.BoundingCollider.transform.localRotation;
		this.testColliderA.transform.localScale = section.BoundingCollider.transform.localScale;
		this.testColliderA.size = section.BoundingCollider.size;
		this.testColliderA.center = section.BoundingCollider.center;
		for (int i = 0; i < this.nonOverlapZones.Count; i++)
		{
			Vector3 vector;
			float num;
			if (this.testColliderA.bounds.Intersects(this.nonOverlapZones[i].bounds) && Physics.ComputePenetration(this.testColliderA, this.testColliderA.transform.position, this.testColliderA.transform.rotation, this.nonOverlapZones[i], this.nonOverlapZones[i].transform.position, this.nonOverlapZones[i].transform.rotation, out vector, out num))
			{
				this.testColliderA.gameObject.SetActive(false);
				this.testColliderB.gameObject.SetActive(false);
				return true;
			}
		}
		for (int j = 0; j < this.nodeTree.Count; j++)
		{
			for (int k = 0; k < this.nodeTree[j].Count; k++)
			{
				if (j != selfi || k != selfj || selfk != -1)
				{
					GhostReactorLevelGenerator.Node node = this.nodeTree[j][k];
					for (int l = 0; l < node.children.Length; l++)
					{
						if (j != selfi || k != selfj || l != selfk)
						{
							GhostReactorLevelGenerator.Node node2 = node.children[l];
							if (node2 != null && node2.sectionInstance != null && node2.sectionInstance.BoundingCollider != null && (node2.type == GhostReactorLevelGenerator.NodeType.Blocker || node2.type == GhostReactorLevelGenerator.NodeType.EndCap))
							{
								GhostReactorLevelSection sectionInstance = node2.sectionInstance;
								this.testColliderB.transform.position = sectionInstance.transform.position + sectionInstance.transform.rotation * sectionInstance.BoundingCollider.transform.localPosition;
								this.testColliderB.transform.rotation = sectionInstance.transform.rotation * sectionInstance.BoundingCollider.transform.localRotation;
								this.testColliderB.transform.localScale = sectionInstance.BoundingCollider.transform.localScale;
								this.testColliderB.size = sectionInstance.BoundingCollider.size;
								this.testColliderB.center = sectionInstance.BoundingCollider.center;
								Vector3 vector2;
								float num2;
								if (this.testColliderA.bounds.Intersects(this.testColliderB.bounds) && Physics.ComputePenetration(this.testColliderA, this.testColliderA.transform.position, this.testColliderA.transform.rotation, this.testColliderB, this.testColliderB.transform.position, this.testColliderB.transform.rotation, out vector2, out num2))
								{
									this.testColliderA.gameObject.SetActive(false);
									this.testColliderB.gameObject.SetActive(false);
									return true;
								}
							}
						}
					}
					if ((j != selfi || k != selfj) && node.sectionInstance != null && node.sectionInstance.BoundingCollider != null)
					{
						GhostReactorLevelSection sectionInstance2 = node.sectionInstance;
						this.testColliderB.transform.position = sectionInstance2.transform.position + sectionInstance2.transform.rotation * sectionInstance2.BoundingCollider.transform.localPosition;
						this.testColliderB.transform.rotation = sectionInstance2.transform.rotation * sectionInstance2.BoundingCollider.transform.localRotation;
						this.testColliderB.transform.localScale = sectionInstance2.BoundingCollider.transform.localScale;
						this.testColliderB.size = sectionInstance2.BoundingCollider.size;
						this.testColliderB.center = sectionInstance2.BoundingCollider.center;
						Vector3 vector3;
						float num3;
						if (this.testColliderA.bounds.Intersects(this.testColliderB.bounds) && Physics.ComputePenetration(this.testColliderA, this.testColliderA.transform.position, this.testColliderA.transform.rotation, this.testColliderB, this.testColliderB.transform.position, this.testColliderB.transform.rotation, out vector3, out num3))
						{
							this.testColliderA.gameObject.SetActive(false);
							this.testColliderB.gameObject.SetActive(false);
							return true;
						}
					}
				}
			}
		}
		this.testColliderA.gameObject.SetActive(false);
		this.testColliderB.gameObject.SetActive(false);
		return false;
	}

	private void DebugGenerate()
	{
		this.Generate(this.seed);
	}

	public void Generate(int inputSeed)
	{
		this.ClearLevelSections();
		if (!Application.isPlaying)
		{
			return;
		}
		this.seed = inputSeed;
		this.randomGenerator = new SRand(this.seed);
		if (this.TreeLevels.Count < 1)
		{
			return;
		}
		this.spawnedHubHashSet.Clear();
		for (int i = 0; i < this.TreeLevels.Count; i++)
		{
			this.nodeTree.Add(new List<GhostReactorLevelGenerator.Node>());
			GameObject gameObject = new GameObject(string.Format("Tree Level {0}", i));
			gameObject.transform.parent = base.transform;
			gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			this.treeParents.Add(gameObject.transform);
		}
		GhostReactorLevelGenerator.Node node = new GhostReactorLevelGenerator.Node();
		node.type = GhostReactorLevelGenerator.NodeType.Hub;
		node.configIndex = -1;
		node.attachAnchorIndex = -1;
		node.parentAnchorIndex = -1;
		node.children = new GhostReactorLevelGenerator.Node[this.mainHub.Anchors.Count];
		node.sectionInstance = this.mainHub;
		node.anchorCount = this.mainHub.Anchors.Count;
		node.anchorOrder = new List<int>();
		this.RandomizeIndices(ref node.anchorOrder, node.anchorCount);
		this.nodeTree[0].Add(node);
		this.nodeList.Add(node);
		for (int j = 0; j < this.TreeLevels.Count; j++)
		{
			List<GhostReactorLevelSection> hubs = this.TreeLevels[j].hubs;
			List<GhostReactorLevelSectionConnector> connectors = this.TreeLevels[j].connectors;
			if (hubs.Count >= 1 && connectors.Count >= 1)
			{
				this.RandomizeIndices(ref this.hubOrder, hubs.Count);
				this.RandomizeIndices(ref this.connectorOrder, connectors.Count);
				int num = 0;
				int num2 = 0;
				int num3 = Mathf.Max(this.TreeLevels[j].maxHubs - this.TreeLevels[j].minHubs, 0);
				int num4 = Mathf.Max(this.TreeLevels[j].minHubs, 0) + this.randomGenerator.NextInt(num3 + 1);
				for (int k = 0; k < num4; k++)
				{
					if (j <= 0 || this.nodeTree[j].Count >= 1)
					{
						int num5 = this.hubOrder[num % this.hubOrder.Count];
						num++;
						int index = this.connectorOrder[num2 % this.connectorOrder.Count];
						num2++;
						int num6 = (j == 0) ? -1 : (k % this.nodeTree[j].Count);
						GhostReactorLevelGenerator.Node node2 = (num6 != -1) ? this.nodeTree[j][num6] : node;
						for (int l = 0; l < node2.anchorOrder.Count; l++)
						{
							int num7 = node2.anchorOrder[l];
							bool flag = this.spawnedHubHashSet.Contains(hubs[num5].gameObject.name);
							if (node2.children[num7] == null && node2.attachAnchorIndex != num7 && !flag)
							{
								Quaternion rhs = node2.sectionInstance.Anchors[num7].rotation * this.flip180;
								Vector3 position = node2.sectionInstance.Anchors[num7].position;
								GhostReactorLevelSectionConnector ghostReactorLevelSectionConnector = connectors[index];
								Quaternion quaternion = Quaternion.Inverse(ghostReactorLevelSectionConnector.hubAnchor.localRotation) * rhs;
								Vector3 vector = quaternion * -ghostReactorLevelSectionConnector.hubAnchor.localPosition + position;
								Vector3 b = quaternion * ghostReactorLevelSectionConnector.sectionAnchor.localPosition + vector;
								Quaternion rhs2 = quaternion * ghostReactorLevelSectionConnector.sectionAnchor.localRotation;
								GhostReactorLevelSection ghostReactorLevelSection = hubs[num5];
								bool flag2 = false;
								if (ghostReactorLevelSection.Anchors.Count > 0)
								{
									this.RandomizeIndices(ref this.entryAnchorOrder, ghostReactorLevelSection.Anchors.Count);
									for (int m = 0; m < this.entryAnchorOrder.Count; m++)
									{
										int num8 = this.entryAnchorOrder[m];
										Transform transform = ghostReactorLevelSection.Anchors[num8];
										Quaternion rotation = Quaternion.Inverse(transform.localRotation) * rhs2;
										Vector3 position2 = rotation * -transform.localPosition + b;
										if (!this.TestForCollision(ghostReactorLevelSection, position2, rotation, j, k, num7))
										{
											GhostReactorLevelGenerator.Node node3 = new GhostReactorLevelGenerator.Node();
											node3.type = GhostReactorLevelGenerator.NodeType.Hub;
											node3.configIndex = num5;
											node3.children = new GhostReactorLevelGenerator.Node[ghostReactorLevelSection.Anchors.Count];
											node3.parentAnchorIndex = num7;
											node3.attachAnchorIndex = num8;
											node3.anchorCount = ghostReactorLevelSection.Anchors.Count;
											node3.anchorOrder = new List<int>();
											this.RandomizeIndices(ref node3.anchorOrder, node3.anchorCount);
											GhostReactorLevelSectionConnector component = Object.Instantiate<GameObject>(ghostReactorLevelSectionConnector.gameObject, vector, quaternion, this.treeParents[j]).GetComponent<GhostReactorLevelSectionConnector>();
											node3.connectorInstance = component;
											GhostReactorLevelSection component2 = Object.Instantiate<GameObject>(ghostReactorLevelSection.gameObject, position2, rotation, this.treeParents[j]).GetComponent<GhostReactorLevelSection>();
											node3.sectionInstance = component2;
											node2.children[node3.parentAnchorIndex] = node3;
											this.nodeTree[j + 1].Add(node3);
											this.nodeList.Add(node3);
											this.spawnedHubHashSet.Add(ghostReactorLevelSection.gameObject.name);
											flag2 = true;
											break;
										}
									}
								}
								if (flag2)
								{
									break;
								}
							}
						}
					}
				}
			}
		}
		for (int n = 0; n < this.nodeTree.Count; n++)
		{
			List<GhostReactorLevelSection> endCaps = this.TreeLevels[n].endCaps;
			List<GhostReactorLevelSection> blockers = this.TreeLevels[n].blockers;
			this.RandomizeIndices(ref this.blockerOrder, blockers.Count);
			this.RandomizeIndices(ref this.endCapOrder, endCaps.Count);
			int num9 = 0;
			int num10 = 0;
			for (int num11 = 0; num11 < this.nodeTree[n].Count; num11++)
			{
				GhostReactorLevelGenerator.Node node4 = this.nodeTree[n][num11];
				int num12 = Mathf.Max(this.TreeLevels[n].maxCaps - this.TreeLevels[n].minCaps, 0);
				int num13 = Mathf.Max(this.TreeLevels[n].minCaps, 0) + this.randomGenerator.NextInt(num12 + 1);
				for (int num14 = 0; num14 < node4.children.Length; num14++)
				{
					if (node4.children[num14] == null && node4.attachAnchorIndex != num14)
					{
						bool flag3 = false;
						if (num13 > 0 && this.endCapOrder.Count > 0)
						{
							int num15 = this.endCapOrder[num10 % this.endCapOrder.Count];
							num10++;
							num13--;
							Quaternion rhs3 = node4.sectionInstance.Anchors[num14].rotation * this.flip180;
							Vector3 position3 = node4.sectionInstance.Anchors[num14].position;
							GhostReactorLevelSection ghostReactorLevelSection2 = endCaps[num15];
							Quaternion rotation2 = Quaternion.Inverse(ghostReactorLevelSection2.Anchor.localRotation) * rhs3;
							Vector3 position4 = rotation2 * -ghostReactorLevelSection2.Anchor.localPosition + position3;
							if (!this.TestForCollision(ghostReactorLevelSection2, position4, rotation2, n, num11, num14))
							{
								GhostReactorLevelGenerator.Node node5 = new GhostReactorLevelGenerator.Node();
								node5.type = GhostReactorLevelGenerator.NodeType.EndCap;
								node5.configIndex = num15;
								node5.parentAnchorIndex = num14;
								GhostReactorLevelSection component3 = Object.Instantiate<GameObject>(ghostReactorLevelSection2.gameObject, position4, rotation2, this.treeParents[n]).GetComponent<GhostReactorLevelSection>();
								node5.sectionInstance = component3;
								node4.children[num14] = node5;
								flag3 = true;
							}
						}
						if (!flag3 && this.blockerOrder.Count > 0)
						{
							int configIndex = this.blockerOrder[num9 % this.blockerOrder.Count];
							num9++;
							GhostReactorLevelGenerator.Node node6 = new GhostReactorLevelGenerator.Node();
							node6.type = GhostReactorLevelGenerator.NodeType.Blocker;
							node6.configIndex = configIndex;
							node6.parentAnchorIndex = num14;
							Quaternion rhs4 = node4.sectionInstance.Anchors[num14].rotation * this.flip180;
							Vector3 position5 = node4.sectionInstance.Anchors[num14].position;
							GhostReactorLevelSection ghostReactorLevelSection3 = blockers[node6.configIndex];
							Quaternion rotation3 = Quaternion.Inverse(ghostReactorLevelSection3.Anchor.localRotation) * rhs4;
							Vector3 position6 = rotation3 * -ghostReactorLevelSection3.Anchor.localPosition + position5;
							GhostReactorLevelSection component4 = Object.Instantiate<GameObject>(ghostReactorLevelSection3.gameObject, position6, rotation3, this.treeParents[n]).GetComponent<GhostReactorLevelSection>();
							node6.sectionInstance = component4;
							node4.children[num14] = node6;
						}
					}
				}
			}
		}
		for (int num16 = 0; num16 < this.nodeList.Count; num16++)
		{
			if (this.nodeList[num16].connectorInstance != null)
			{
				this.nodeList[num16].connectorInstance.Init(this.reactor.grManager);
			}
			this.nodeList[num16].sectionInstance.InitLevelSection(num16, this.reactor);
		}
	}

	private void DebugClear()
	{
		this.ClearLevelSections();
	}

	public void ClearLevelSections()
	{
		for (int i = 0; i < this.nodeList.Count; i++)
		{
			if (!(this.nodeList[i].sectionInstance == this.mainHub))
			{
				if (this.nodeList[i].connectorInstance != null)
				{
					Object.Destroy(this.nodeList[i].connectorInstance.gameObject);
				}
				Object.Destroy(this.nodeList[i].sectionInstance.gameObject);
			}
		}
		this.nodeList.Clear();
		for (int j = 0; j < this.nodeTree.Count; j++)
		{
			this.nodeTree[j].Clear();
		}
		this.nodeTree.Clear();
		for (int k = 0; k < this.treeParents.Count; k++)
		{
			Object.Destroy(this.treeParents[k].gameObject);
		}
		this.treeParents.Clear();
	}

	public void SpawnEntitiesInEachSection(float respawnCount)
	{
		for (int i = 0; i < this.nodeTree.Count; i++)
		{
			List<GhostReactorSpawnConfig> spawnConfigs = (i < 1) ? this.mainHubSpawnConfigs : this.TreeLevels[i - 1].sectionSpawnConfigs;
			List<GhostReactorSpawnConfig> endCapSpawnConfigs = this.TreeLevels[i].endCapSpawnConfigs;
			for (int j = 0; j < this.nodeTree[i].Count; j++)
			{
				GhostReactorLevelGenerator.Node node = this.nodeTree[i][j];
				if (node != null && node.sectionInstance != null && node.type == GhostReactorLevelGenerator.NodeType.Hub)
				{
					node.sectionInstance.SpawnSectionEntities(ref this.randomGenerator, this.reactor.grManager.gameEntityManager, this.reactor, spawnConfigs, respawnCount);
				}
				for (int k = 0; k < node.children.Length; k++)
				{
					GhostReactorLevelGenerator.Node node2 = node.children[k];
					if (node2 != null && node2.sectionInstance != null && node2.type == GhostReactorLevelGenerator.NodeType.EndCap)
					{
						node2.sectionInstance.SpawnSectionEntities(ref this.randomGenerator, this.reactor.grManager.gameEntityManager, this.reactor, endCapSpawnConfigs, respawnCount);
					}
				}
			}
		}
		if (GhostReactorLevelSection.tempCreateEntitiesList.Count > 0)
		{
			this.reactor.grManager.gameEntityManager.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
			GhostReactorLevelSection.tempCreateEntitiesList.Clear();
		}
	}

	public void RespawnEntity(int entityId, long entityCreateData, GameEntityId createdByEntityId)
	{
		int sectionIndex = GhostReactor.EnemyEntityCreateData.Unpack(entityCreateData).sectionIndex;
		if (sectionIndex >= 0 && sectionIndex < this.nodeList.Count)
		{
			this.nodeList[sectionIndex].sectionInstance.RespawnEntity(ref this.randomGenerator, this.reactor.grManager.gameEntityManager, entityId, entityCreateData, createdByEntityId);
		}
	}

	public GRPatrolPath GetPatrolPath(long createData)
	{
		GhostReactor.EnemyEntityCreateData enemyEntityCreateData = GhostReactor.EnemyEntityCreateData.Unpack(createData);
		int sectionIndex = enemyEntityCreateData.sectionIndex;
		int patrolIndex = enemyEntityCreateData.patrolIndex;
		if (sectionIndex < 0 || sectionIndex >= this.nodeList.Count)
		{
			return null;
		}
		return this.nodeList[sectionIndex].sectionInstance.GetPatrolPath(patrolIndex);
	}

	private void RandomizeIndices(ref List<int> list, int count)
	{
		list.Clear();
		for (int i = 0; i < count; i++)
		{
			list.Add(i);
		}
		this.randomGenerator.Shuffle<int>(list);
	}

	public bool GetExitFromCurrentSection(Vector3 pos, out Vector3 exitPos, out Quaternion exitRot, List<Vector3> connectorCorners)
	{
		exitPos = Vector3.zero;
		exitRot = Quaternion.identity;
		GhostReactorLevelGenerator.Node currentNode = this.GetCurrentNode(pos);
		if (currentNode == null || currentNode.parentAnchorIndex < 0)
		{
			return false;
		}
		Transform anchor = currentNode.sectionInstance.GetAnchor(currentNode.attachAnchorIndex);
		exitPos = anchor.transform.position;
		exitRot = anchor.transform.rotation;
		GRLevelAnchor component = anchor.GetComponent<GRLevelAnchor>();
		if (component != null && component.navigablePoint != null)
		{
			exitPos = component.navigablePoint.position;
			exitRot = component.navigablePoint.rotation;
		}
		connectorCorners.Clear();
		if (currentNode.connectorInstance != null)
		{
			for (int i = 0; i < currentNode.connectorInstance.pathNodes.Count; i++)
			{
				connectorCorners.Add(currentNode.connectorInstance.pathNodes[i].position);
			}
		}
		return true;
	}

	private GhostReactorLevelGenerator.Node GetCurrentNode(Vector3 pos)
	{
		float num = float.MaxValue;
		GhostReactorLevelGenerator.Node result = null;
		for (int i = 0; i < this.nodeTree.Count; i++)
		{
			List<GhostReactorLevelGenerator.Node> list = this.nodeTree[i];
			for (int j = 0; j < list.Count; j++)
			{
				GhostReactorLevelGenerator.Node node = list[j];
				if (!(node.sectionInstance == null))
				{
					float distSq = node.sectionInstance.GetDistSq(pos);
					if (distSq < num)
					{
						num = distSq;
						result = node;
					}
				}
			}
		}
		return result;
	}

	public List<GhostReactorLevelDepthConfig> depthConfigs;

	[SerializeField]
	private GhostReactorLevelSection mainHub = new GhostReactorLevelSection();

	[SerializeField]
	private List<GhostReactorSpawnConfig> mainHubSpawnConfigs;

	[SerializeField]
	private List<Collider> nonOverlapZones = new List<Collider>();

	public int seed = 2343;

	private List<List<GhostReactorLevelGenerator.Node>> nodeTree = new List<List<GhostReactorLevelGenerator.Node>>();

	private List<GhostReactorLevelGenerator.Node> nodeList = new List<GhostReactorLevelGenerator.Node>();

	private HashSet<string> spawnedHubHashSet = new HashSet<string>();

	private List<int> hubOrder = new List<int>();

	private List<int> connectorOrder = new List<int>();

	private List<int> endCapOrder = new List<int>();

	private List<int> blockerOrder = new List<int>();

	private List<int> entryAnchorOrder = new List<int>();

	private List<Transform> treeParents = new List<Transform>();

	private string generationOutput = "";

	private SRand randomGenerator;

	private BoxCollider testColliderA;

	private BoxCollider testColliderB;

	private GhostReactor reactor;

	[NonSerialized]
	public int depthConfigIndex;

	private Quaternion flip180 = Quaternion.AngleAxis(180f, Vector3.up);

	private const int MAX_VIS_CHECKS_PER_FRAME = 1;

	public int nextVisCheckNodeIndex;

	public enum NodeType
	{
		Hub,
		EndCap,
		Blocker
	}

	public class Node
	{
		public GhostReactorLevelGenerator.NodeType type;

		public int configIndex;

		public int parentAnchorIndex;

		public int attachAnchorIndex;

		public int anchorCount;

		public List<int> anchorOrder;

		public GhostReactorLevelSection sectionInstance;

		public GhostReactorLevelSectionConnector connectorInstance;

		public GhostReactorLevelGenerator.Node[] children;
	}
}
