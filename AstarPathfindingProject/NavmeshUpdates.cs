using System;
using System.Collections.Generic;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding
{
	[Serializable]
	public class NavmeshUpdates
	{
		internal void OnEnable()
		{
			NavmeshClipper.AddEnableCallback(new Action<NavmeshClipper>(this.HandleOnEnableCallback), new Action<NavmeshClipper>(this.HandleOnDisableCallback));
		}

		internal void OnDisable()
		{
			NavmeshClipper.RemoveEnableCallback(new Action<NavmeshClipper>(this.HandleOnEnableCallback), new Action<NavmeshClipper>(this.HandleOnDisableCallback));
		}

		public void DiscardPending()
		{
			for (int i = 0; i < NavmeshClipper.allEnabled.Count; i++)
			{
				NavmeshClipper.allEnabled[i].NotifyUpdated();
			}
			NavGraph[] graphs = AstarPath.active.graphs;
			for (int j = 0; j < graphs.Length; j++)
			{
				NavmeshBase navmeshBase = graphs[j] as NavmeshBase;
				if (navmeshBase != null)
				{
					navmeshBase.navmeshUpdateData.forcedReloadRects.Clear();
				}
			}
		}

		private void HandleOnEnableCallback(NavmeshClipper obj)
		{
			NavGraph[] graphs = AstarPath.active.graphs;
			for (int i = 0; i < graphs.Length; i++)
			{
				NavmeshBase navmeshBase = graphs[i] as NavmeshBase;
				if (navmeshBase != null)
				{
					navmeshBase.navmeshUpdateData.AddClipper(obj);
				}
			}
			obj.ForceUpdate();
		}

		private void HandleOnDisableCallback(NavmeshClipper obj)
		{
			NavGraph[] graphs = AstarPath.active.graphs;
			for (int i = 0; i < graphs.Length; i++)
			{
				NavmeshBase navmeshBase = graphs[i] as NavmeshBase;
				if (navmeshBase != null)
				{
					navmeshBase.navmeshUpdateData.RemoveClipper(obj);
				}
			}
			this.lastUpdateTime = float.NegativeInfinity;
		}

		internal void Update()
		{
			if (AstarPath.active.isScanning)
			{
				return;
			}
			bool flag = false;
			NavGraph[] graphs = AstarPath.active.graphs;
			for (int i = 0; i < graphs.Length; i++)
			{
				NavmeshBase navmeshBase = graphs[i] as NavmeshBase;
				if (navmeshBase != null)
				{
					navmeshBase.navmeshUpdateData.Refresh(false);
					flag = (navmeshBase.navmeshUpdateData.forcedReloadRects.Count > 0);
				}
			}
			if ((this.updateInterval >= 0f && Time.realtimeSinceStartup - this.lastUpdateTime > this.updateInterval) || flag)
			{
				this.ForceUpdate();
			}
		}

		public void ForceUpdate()
		{
			this.lastUpdateTime = Time.realtimeSinceStartup;
			List<NavmeshClipper> list = null;
			NavGraph[] graphs = AstarPath.active.graphs;
			for (int i = 0; i < graphs.Length; i++)
			{
				NavmeshBase navmeshBase = graphs[i] as NavmeshBase;
				if (navmeshBase != null)
				{
					navmeshBase.navmeshUpdateData.Refresh(false);
					TileHandler handler = navmeshBase.navmeshUpdateData.handler;
					if (handler != null)
					{
						List<IntRect> forcedReloadRects = navmeshBase.navmeshUpdateData.forcedReloadRects;
						GridLookup<NavmeshClipper>.Root allItems = handler.cuts.AllItems;
						if (forcedReloadRects.Count == 0)
						{
							bool flag = false;
							for (GridLookup<NavmeshClipper>.Root root = allItems; root != null; root = root.next)
							{
								if (root.obj.RequiresUpdate())
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								goto IL_16A;
							}
						}
						handler.StartBatchLoad();
						for (int j = 0; j < forcedReloadRects.Count; j++)
						{
							handler.ReloadInBounds(forcedReloadRects[j]);
						}
						forcedReloadRects.ClearFast<IntRect>();
						if (list == null)
						{
							list = ListPool<NavmeshClipper>.Claim();
						}
						for (GridLookup<NavmeshClipper>.Root root2 = allItems; root2 != null; root2 = root2.next)
						{
							if (root2.obj.RequiresUpdate())
							{
								handler.ReloadInBounds(root2.previousBounds);
								Rect bounds = root2.obj.GetBounds(handler.graph.transform);
								IntRect touchingTilesInGraphSpace = handler.graph.GetTouchingTilesInGraphSpace(bounds);
								handler.cuts.Move(root2.obj, touchingTilesInGraphSpace);
								handler.ReloadInBounds(touchingTilesInGraphSpace);
								list.Add(root2.obj);
							}
						}
						handler.EndBatchLoad();
					}
				}
				IL_16A:;
			}
			if (list != null)
			{
				for (int k = 0; k < list.Count; k++)
				{
					list[k].NotifyUpdated();
				}
				ListPool<NavmeshClipper>.Release(ref list);
			}
		}

		public float updateInterval;

		private float lastUpdateTime = float.NegativeInfinity;

		internal class NavmeshUpdateSettings
		{
			public NavmeshUpdateSettings(NavmeshBase graph)
			{
				this.graph = graph;
			}

			public void Refresh(bool forceCreate = false)
			{
				if (!this.graph.enableNavmeshCutting)
				{
					if (this.handler != null)
					{
						this.handler.cuts.Clear();
						this.handler.ReloadInBounds(new IntRect(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue));
						AstarPath.active.FlushGraphUpdates();
						AstarPath.active.FlushWorkItems();
						this.forcedReloadRects.ClearFast<IntRect>();
						this.handler = null;
						return;
					}
				}
				else if ((this.handler == null && (forceCreate || NavmeshClipper.allEnabled.Count > 0)) || (this.handler != null && !this.handler.isValid))
				{
					this.handler = new TileHandler(this.graph);
					for (int i = 0; i < NavmeshClipper.allEnabled.Count; i++)
					{
						this.AddClipper(NavmeshClipper.allEnabled[i]);
					}
					this.handler.CreateTileTypesFromGraph();
					this.forcedReloadRects.Add(new IntRect(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue));
				}
			}

			public void OnRecalculatedTiles(NavmeshTile[] tiles)
			{
				this.Refresh(false);
				if (this.handler != null)
				{
					this.handler.OnRecalculatedTiles(tiles);
				}
			}

			public void AddClipper(NavmeshClipper obj)
			{
				if (!obj.graphMask.Contains((int)this.graph.graphIndex))
				{
					return;
				}
				this.Refresh(true);
				if (this.handler == null)
				{
					return;
				}
				Rect bounds = obj.GetBounds(this.handler.graph.transform);
				IntRect touchingTilesInGraphSpace = this.handler.graph.GetTouchingTilesInGraphSpace(bounds);
				this.handler.cuts.Add(obj, touchingTilesInGraphSpace);
			}

			public void RemoveClipper(NavmeshClipper obj)
			{
				this.Refresh(false);
				if (this.handler == null)
				{
					return;
				}
				GridLookup<NavmeshClipper>.Root root = this.handler.cuts.GetRoot(obj);
				if (root != null)
				{
					this.forcedReloadRects.Add(root.previousBounds);
					this.handler.cuts.Remove(obj);
				}
			}

			public TileHandler handler;

			public readonly List<IntRect> forcedReloadRects = new List<IntRect>();

			private readonly NavmeshBase graph;
		}
	}
}
