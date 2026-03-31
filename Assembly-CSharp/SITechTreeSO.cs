using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class SITechTreeSO : ScriptableObject
{
	public List<SITechTreePage> TreePages { get; private set; }

	public int TreePageCount { get; private set; }

	public int[] TreeNodeCounts { get; private set; }

	public List<GraphNode<SITechTreeNode>> AllNodes { get; private set; }

	public bool Initialized { get; private set; }

	public List<GameEntity> SpawnableEntities
	{
		get
		{
			this.EnsureInitialized();
			return this._spawnableEntities;
		}
	}

	public bool TryGetNode(SIUpgradeType upgradeType, out GraphNode<SITechTreeNode> node)
	{
		return this._nodeLookup.TryGetValue(upgradeType, out node);
	}

	public bool TryGetUpgradeTypeByEntityTypeId(int entityTypeId, out SIUpgradeType upgradeType)
	{
		return this._upgradeTypeByEntityTypeId.TryGetValue(entityTypeId, out upgradeType);
	}

	public bool IsValidPage(SITechTreePageId id)
	{
		foreach (SITechTreePage sitechTreePage in this.TreePages)
		{
			if (sitechTreePage.pageId == id && sitechTreePage.IsValid)
			{
				return true;
			}
		}
		return false;
	}

	public SITechTreePage GetTreePage(SITechTreePageId id)
	{
		SITechTreePage result;
		if (!this.TryGetTreePage(id, out result))
		{
			return null;
		}
		return result;
	}

	public bool TryGetTreePage(SITechTreePageId id, out SITechTreePage treePage)
	{
		foreach (SITechTreePage sitechTreePage in this.TreePages)
		{
			if (sitechTreePage.pageId == id && sitechTreePage.IsValid)
			{
				treePage = sitechTreePage;
				return true;
			}
		}
		treePage = null;
		return false;
	}

	public bool IsValidNode(int pageId, int nodeId)
	{
		return this.IsValidNode(SIUpgradeTypeSystem.GetUpgradeType(pageId, nodeId));
	}

	public bool IsValidNode(SIUpgradeType upgradeType)
	{
		return this._nodeLookup.ContainsKey(upgradeType);
	}

	public SITechTreeNode GetTreeNode(int pageId, int nodeId)
	{
		return this.GetTreeNode(SIUpgradeTypeSystem.GetUpgradeType(pageId, nodeId));
	}

	public SITechTreeNode GetTreeNode(SIUpgradeType upgradeType)
	{
		GraphNode<SITechTreeNode> graphNode;
		if (this._nodeLookup.TryGetValue(upgradeType, out graphNode))
		{
			return graphNode.Value;
		}
		return null;
	}

	public void EnsureInitialized()
	{
		if (!this.Initialized)
		{
			this.InitTechTree();
		}
	}

	private void InitTechTree()
	{
		Debug.Log("[SI] SITechTreeSO.InitTechTree");
		this.ClearTechTree();
		this.TreePages = new List<SITechTreePage>();
		this._spawnableEntities = new List<GameEntity>();
		int num = 0;
		foreach (SITechTreePage sitechTreePage in this.treePages)
		{
			if (sitechTreePage.IsValid)
			{
				sitechTreePage.BuildGraph();
				foreach (GraphNode<SITechTreeNode> graphNode in sitechTreePage.Roots)
				{
					foreach (GraphNode<SITechTreeNode> graphNode2 in graphNode.TraversePreOrder())
					{
						if (!this._nodeLookup.ContainsKey(graphNode2.Value.upgradeType))
						{
							this._nodeLookup.Add(graphNode2.Value.upgradeType, graphNode2);
						}
					}
				}
				foreach (SITechTreeNode sitechTreeNode in sitechTreePage.DispensableGadgets)
				{
					num++;
					this.AddSpawnableGadget(sitechTreeNode.unlockedGadgetPrefab);
				}
				if (sitechTreePage.Roots.Count > 0)
				{
					this.TreePages.Add(sitechTreePage);
				}
			}
		}
		if (this._upgradeTypeByEntityTypeId.IsCreated)
		{
			this._upgradeTypeByEntityTypeId.Clear();
		}
		else
		{
			this._upgradeTypeByEntityTypeId = new NativeHashMap<int, SIUpgradeType>(num, Allocator.Persistent);
		}
		foreach (SITechTreePage sitechTreePage2 in this.treePages)
		{
			if (sitechTreePage2.IsValid)
			{
				foreach (SITechTreeNode sitechTreeNode2 in sitechTreePage2.DispensableGadgets)
				{
					int staticHash = sitechTreeNode2.unlockedGadgetPrefab.gameObject.name.GetStaticHash();
					this._upgradeTypeByEntityTypeId.TryAdd(staticHash, sitechTreeNode2.upgradeType);
				}
			}
		}
		this.AllNodes = new List<GraphNode<SITechTreeNode>>(this._nodeLookup.Values);
		this.TreePageCount = (from v in (SIUpgradeType[])Enum.GetValues(typeof(SIUpgradeType))
		select v.GetPageId()).Max() + 1;
		this.TreeNodeCounts = new int[this.TreePageCount];
		foreach (SIUpgradeType self in (SIUpgradeType[])Enum.GetValues(typeof(SIUpgradeType)))
		{
			int pageId = self.GetPageId();
			int nodeId = self.GetNodeId();
			this.TreeNodeCounts[pageId] = Mathf.Max(this.TreeNodeCounts[pageId], nodeId + 1);
		}
		this.Initialized = true;
	}

	private void AddSpawnableGadget(GameEntity entity)
	{
		this._spawnableEntities.Add(entity);
		IPrefabRequirements component = entity.GetComponent<IPrefabRequirements>();
		if (component != null)
		{
			foreach (GameEntity item in component.RequiredPrefabs)
			{
				this._spawnableEntities.Add(item);
			}
		}
	}

	private void ClearTechTree()
	{
		SITechTreePage[] array = this.treePages;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ClearGraph();
		}
		this._nodeLookup.Clear();
		if (this._upgradeTypeByEntityTypeId.IsCreated)
		{
			this._upgradeTypeByEntityTypeId.Dispose();
		}
		this.Initialized = false;
	}

	private const string preLog = "[SITechTreeSO]  ";

	private const string preErr = "[SITechTreeSO]  ERROR!!!  ";

	private const int RESOURCE_CAP = 20;

	[SerializeField]
	private SITechTreePage[] treePages;

	private readonly Dictionary<SIUpgradeType, GraphNode<SITechTreeNode>> _nodeLookup = new Dictionary<SIUpgradeType, GraphNode<SITechTreeNode>>();

	private NativeHashMap<int, SIUpgradeType> _upgradeTypeByEntityTypeId;

	private List<GameEntity> _spawnableEntities;
}
