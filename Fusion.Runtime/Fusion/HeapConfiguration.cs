using System;
using UnityEngine;

namespace Fusion
{
	[Serializable]
	public class HeapConfiguration
	{
		internal Allocator.Config ToAllocatorConfig()
		{
			return new Allocator.Config(this.PageShift, this.PageCount, this.GlobalsSize);
		}

		public HeapConfiguration Init(int globalsSize)
		{
			HeapConfiguration heapConfiguration = (HeapConfiguration)base.MemberwiseClone();
			heapConfiguration.GlobalsSize = globalsSize;
			return heapConfiguration;
		}

		public override string ToString()
		{
			return string.Format("[HeapConfiguration: {0}/{1}/{2}]", this.PageShift, this.PageCount, this.GlobalsSize);
		}

		private const int PageCountMin = 16;

		private const int PageCountMax = 4096;

		[InlineHelp]
		public PageSizes PageShift = PageSizes._32Kb;

		[InlineHelp]
		[Range(16f, 4096f)]
		public int PageCount = 256;

		[InlineHelp]
		[HideInInspector]
		public int GlobalsSize;
	}
}
