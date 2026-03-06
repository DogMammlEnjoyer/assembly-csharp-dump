using System;
using System.Collections.Generic;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding
{
	public abstract class NavmeshClipper : VersionedMonoBehaviour
	{
		public static void AddEnableCallback(Action<NavmeshClipper> onEnable, Action<NavmeshClipper> onDisable)
		{
			NavmeshClipper.OnEnableCallback = (Action<NavmeshClipper>)Delegate.Combine(NavmeshClipper.OnEnableCallback, onEnable);
			NavmeshClipper.OnDisableCallback = (Action<NavmeshClipper>)Delegate.Combine(NavmeshClipper.OnDisableCallback, onDisable);
		}

		public static void RemoveEnableCallback(Action<NavmeshClipper> onEnable, Action<NavmeshClipper> onDisable)
		{
			NavmeshClipper.OnEnableCallback = (Action<NavmeshClipper>)Delegate.Remove(NavmeshClipper.OnEnableCallback, onEnable);
			NavmeshClipper.OnDisableCallback = (Action<NavmeshClipper>)Delegate.Remove(NavmeshClipper.OnDisableCallback, onDisable);
		}

		public static List<NavmeshClipper> allEnabled
		{
			get
			{
				return NavmeshClipper.all;
			}
		}

		protected virtual void OnEnable()
		{
			if (NavmeshClipper.OnEnableCallback != null)
			{
				NavmeshClipper.OnEnableCallback(this);
			}
			this.listIndex = NavmeshClipper.all.Count;
			NavmeshClipper.all.Add(this);
		}

		protected virtual void OnDisable()
		{
			NavmeshClipper.all[this.listIndex] = NavmeshClipper.all[NavmeshClipper.all.Count - 1];
			NavmeshClipper.all[this.listIndex].listIndex = this.listIndex;
			NavmeshClipper.all.RemoveAt(NavmeshClipper.all.Count - 1);
			this.listIndex = -1;
			if (NavmeshClipper.OnDisableCallback != null)
			{
				NavmeshClipper.OnDisableCallback(this);
			}
		}

		internal abstract void NotifyUpdated();

		public abstract Rect GetBounds(GraphTransform transform);

		public abstract bool RequiresUpdate();

		public abstract void ForceUpdate();

		private static Action<NavmeshClipper> OnEnableCallback;

		private static Action<NavmeshClipper> OnDisableCallback;

		private static readonly List<NavmeshClipper> all = new List<NavmeshClipper>();

		private int listIndex = -1;

		public GraphMask graphMask = GraphMask.everything;
	}
}
