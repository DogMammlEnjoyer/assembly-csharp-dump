using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler;

internal class RenderGraphCompilationCache
{
	private static int HashEntryComparer<T>(RenderGraphCompilationCache.HashEntry<T> a, RenderGraphCompilationCache.HashEntry<T> b)
	{
		if (a.lastFrameUsed < b.lastFrameUsed)
		{
			return -1;
		}
		if (a.lastFrameUsed > b.lastFrameUsed)
		{
			return 1;
		}
		return 0;
	}

	public RenderGraphCompilationCache()
	{
		for (int i = 0; i < 20; i++)
		{
			this.m_CompiledGraphPool.Push(new RenderGraph.CompiledGraph());
			this.m_NativeCompiledGraphPool.Push(new CompilerContextData());
		}
	}

	private bool GetCompilationCache<T>(int hash, int frameIndex, out T outGraph, DynamicArray<RenderGraphCompilationCache.HashEntry<T>> hashEntries, Stack<T> pool, DynamicArray<RenderGraphCompilationCache.HashEntry<T>>.SortComparer comparer) where T : RenderGraph.ICompiledGraph
	{
		RenderGraphCompilationCache.s_Hash = hash;
		int num = hashEntries.FindIndex((RenderGraphCompilationCache.HashEntry<T> value) => value.hash == RenderGraphCompilationCache.s_Hash);
		if (num != -1)
		{
			ref RenderGraphCompilationCache.HashEntry<T> ptr = ref hashEntries[num];
			outGraph = ptr.compiledGraph;
			ptr.lastFrameUsed = frameIndex;
			return true;
		}
		if (pool.Count != 0)
		{
			RenderGraphCompilationCache.HashEntry<T> hashEntry = new RenderGraphCompilationCache.HashEntry<T>
			{
				hash = hash,
				lastFrameUsed = frameIndex,
				compiledGraph = pool.Pop()
			};
			hashEntries.Add(hashEntry);
			outGraph = hashEntry.compiledGraph;
			return false;
		}
		hashEntries.QuickSort(comparer);
		ref RenderGraphCompilationCache.HashEntry<T> ptr2 = ref hashEntries[0];
		ptr2.hash = hash;
		ptr2.lastFrameUsed = frameIndex;
		ptr2.compiledGraph.Clear();
		outGraph = ptr2.compiledGraph;
		return false;
	}

	public bool GetCompilationCache(int hash, int frameIndex, out RenderGraph.CompiledGraph outGraph)
	{
		return this.GetCompilationCache<RenderGraph.CompiledGraph>(hash, frameIndex, out outGraph, this.m_HashEntries, this.m_CompiledGraphPool, RenderGraphCompilationCache.s_EntryComparer);
	}

	public bool GetCompilationCache(int hash, int frameIndex, out CompilerContextData outGraph)
	{
		return this.GetCompilationCache<CompilerContextData>(hash, frameIndex, out outGraph, this.m_NativeHashEntries, this.m_NativeCompiledGraphPool, RenderGraphCompilationCache.s_NativeEntryComparer);
	}

	public void Cleanup()
	{
		for (int i = 0; i < this.m_HashEntries.size; i++)
		{
			this.m_HashEntries[i].compiledGraph.Clear();
		}
		this.m_HashEntries.Clear();
		RenderGraph.CompiledGraph[] array = this.m_CompiledGraphPool.ToArray();
		for (int j = 0; j < array.Length; j++)
		{
			array[j].Clear();
		}
		for (int k = 0; k < this.m_NativeHashEntries.size; k++)
		{
			this.m_NativeHashEntries[k].compiledGraph.Dispose();
		}
		this.m_NativeHashEntries.Clear();
		CompilerContextData[] array2 = this.m_NativeCompiledGraphPool.ToArray();
		for (int l = 0; l < array2.Length; l++)
		{
			array2[l].Dispose();
		}
	}

	private DynamicArray<RenderGraphCompilationCache.HashEntry<RenderGraph.CompiledGraph>> m_HashEntries = new DynamicArray<RenderGraphCompilationCache.HashEntry<RenderGraph.CompiledGraph>>();

	private DynamicArray<RenderGraphCompilationCache.HashEntry<CompilerContextData>> m_NativeHashEntries = new DynamicArray<RenderGraphCompilationCache.HashEntry<CompilerContextData>>();

	private Stack<RenderGraph.CompiledGraph> m_CompiledGraphPool = new Stack<RenderGraph.CompiledGraph>();

	private Stack<CompilerContextData> m_NativeCompiledGraphPool = new Stack<CompilerContextData>();

	private static DynamicArray<RenderGraphCompilationCache.HashEntry<RenderGraph.CompiledGraph>>.SortComparer s_EntryComparer = new DynamicArray<RenderGraphCompilationCache.HashEntry<RenderGraph.CompiledGraph>>.SortComparer(RenderGraphCompilationCache.HashEntryComparer<RenderGraph.CompiledGraph>);

	private static DynamicArray<RenderGraphCompilationCache.HashEntry<CompilerContextData>>.SortComparer s_NativeEntryComparer = new DynamicArray<RenderGraphCompilationCache.HashEntry<CompilerContextData>>.SortComparer(RenderGraphCompilationCache.HashEntryComparer<CompilerContextData>);

	private const int k_CachedGraphCount = 20;

	private static int s_Hash;

	private struct HashEntry<T>
	{
		public int hash;

		public int lastFrameUsed;

		public T compiledGraph;
	}
}
