using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	internal class ProxyFlex<ControllerType, ProxyControllerType> where ControllerType : Controller, new() where ProxyControllerType : ProxyController<ControllerType>, new()
	{
		public bool Dirty { get; private set; }

		public Flex Flex
		{
			get
			{
				return this._scrollView.Flex;
			}
		}

		public int NumberOfProxies
		{
			get
			{
				return this._proxyChildren.Count;
			}
		}

		private int NumberOfControllers
		{
			get
			{
				return this.Flex.Children.Count - 2;
			}
		}

		public ProxyFlex(int numberOfInstantiatedControllers, int maximumNumberOfProxies, LayoutStyle layoutStyle, ScrollView scrollView)
		{
			this._scrollView = scrollView;
			for (int i = 0; i < numberOfInstantiatedControllers; i++)
			{
				this.Flex.Append<ControllerType>(i.ToString()).LayoutStyle = layoutStyle;
			}
			this._maximumNumberOfProxies = maximumNumberOfProxies;
			this._childrenLayoutStyle = layoutStyle;
			this._before = this.Flex.Prepend<Controller>("before");
			this._before.LayoutStyle = Style.Instantiate<LayoutStyle>("DynamicSpace");
			this._after = this.Flex.Append<Controller>("after");
			this._after.LayoutStyle = Style.Instantiate<LayoutStyle>("DynamicSpace");
		}

		public ProxyControllerType AppendProxy()
		{
			if (this.NumberOfProxies >= this._maximumNumberOfProxies)
			{
				this.RemoveProxy(this._proxyChildren[0]);
			}
			ProxyControllerType proxyControllerType = OVRObjectPool.Get<ProxyControllerType>();
			this._proxyChildren.Add(proxyControllerType);
			this.Dirty = true;
			return proxyControllerType;
		}

		public void RemoveProxy(ProxyControllerType proxy)
		{
			this._proxyChildren.Remove(proxy);
			OVRObjectPool.Return<ProxyControllerType>(proxy);
			this.Dirty = true;
		}

		public void Clear()
		{
			foreach (ProxyControllerType obj in this._proxyChildren)
			{
				OVRObjectPool.Return<ProxyControllerType>(obj);
			}
			this._proxyChildren.Clear();
			this.Dirty = true;
		}

		public void Update()
		{
			if (this.HasScrolledEnough())
			{
				this.Dirty = true;
			}
			if (this.Dirty)
			{
				this.Fill();
				this.Dirty = false;
			}
		}

		private bool HasScrolledEnough()
		{
			return Mathf.Abs(this.Flex.RectTransform.anchoredPosition.y - this._lastScroll) > 1f;
		}

		private void Fill()
		{
			this._lastScroll = this.Flex.RectTransform.anchoredPosition.y;
			float height = this.ComputeStartHeightFromProgress(this._scrollView.Progress);
			List<Controller> children = this.Flex.Children;
			int num = this.GetItemIndexAtHeight(height);
			int num2 = num + this.NumberOfControllers - 1;
			int num3 = Math.Max(0, Math.Min(num2 - this.NumberOfProxies, num));
			num -= num3;
			num2 -= num3;
			int num4 = 1;
			for (int i = num; i <= num2; i++)
			{
				if (i < this.NumberOfProxies)
				{
					ControllerType target = children[num4++] as ControllerType;
					this._proxyChildren[i].Fill(target, this._targetsDictionary);
				}
			}
			float height2 = this.ComputeHeight(0, num - 1);
			float height3 = this.ComputeHeight(num2 + 1, this.NumberOfProxies - 1);
			this._before.SetHeight(height2);
			this._after.SetHeight(height3);
		}

		private float ComputeTotalHeight()
		{
			return this.ComputeHeight(0, Math.Max(this.NumberOfProxies - 1, this.NumberOfControllers - 1));
		}

		private float ComputeTotalUsefulHeight()
		{
			return this.ComputeTotalHeight() - this.ComputeHeight(1, this.NumberOfControllers - 1) + this.Flex.LayoutStyle.spacing;
		}

		private float ComputeStartHeightFromProgress(float progress)
		{
			return (1f - progress) * this.ComputeTotalUsefulHeight();
		}

		private int GetItemIndexAtHeight(float height)
		{
			int val = (int)(height / (this._childrenLayoutStyle.size.y + this.Flex.LayoutStyle.spacing));
			return Math.Max(0, val);
		}

		private float ComputeHeight(int startIndex, int endIndex)
		{
			float num = (float)(endIndex - startIndex + 1);
			float spacing = this.Flex.LayoutStyle.spacing;
			return num * (this._childrenLayoutStyle.size.y + spacing) - spacing;
		}

		private readonly int _maximumNumberOfProxies;

		private readonly Dictionary<ControllerType, ProxyController<ControllerType>> _targetsDictionary = new Dictionary<ControllerType, ProxyController<ControllerType>>();

		private readonly ScrollView _scrollView;

		private readonly List<ProxyControllerType> _proxyChildren = new List<ProxyControllerType>();

		private readonly Controller _before;

		private readonly Controller _after;

		private readonly LayoutStyle _childrenLayoutStyle;

		private float _lastScroll;
	}
}
