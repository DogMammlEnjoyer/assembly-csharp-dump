using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Jobs;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	[NativeHeader("Runtime/Graphics/GraphicsStateCollection.h")]
	public sealed class GraphicsStateCollection : Object
	{
		public bool BeginTrace()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.BeginTrace_Injected(intPtr);
		}

		public void EndTrace()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsStateCollection.EndTrace_Injected(intPtr);
		}

		public bool isTracing
		{
			[NativeName("IsTracing")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_isTracing_Injected(intPtr);
			}
		}

		public int version
		{
			[NativeName("GetVersion")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_version_Injected(intPtr);
			}
			[NativeName("SetVersion")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GraphicsStateCollection.set_version_Injected(intPtr, value);
			}
		}

		public GraphicsDeviceType graphicsDeviceType
		{
			[NativeName("GetDeviceRenderer")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_graphicsDeviceType_Injected(intPtr);
			}
			[NativeName("SetDeviceRenderer")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GraphicsStateCollection.set_graphicsDeviceType_Injected(intPtr, value);
			}
		}

		public RuntimePlatform runtimePlatform
		{
			[NativeName("GetRuntimePlatform")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_runtimePlatform_Injected(intPtr);
			}
			[NativeName("SetRuntimePlatform")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GraphicsStateCollection.set_runtimePlatform_Injected(intPtr, value);
			}
		}

		public unsafe string qualityLevelName
		{
			[NativeName("GetQualityLevelName")]
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpan;
					GraphicsStateCollection.get_qualityLevelName_Injected(intPtr, out managedSpan);
				}
				finally
				{
					ManagedSpanWrapper managedSpan;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
				}
				return stringAndDispose;
			}
			[NativeName("SetQualityLevelName")]
			set
			{
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					GraphicsStateCollection.set_qualityLevelName_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		public unsafe bool LoadFromFile(string filePath)
		{
			bool result;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = GraphicsStateCollection.LoadFromFile_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		public unsafe bool SaveToFile(string filePath)
		{
			bool result;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = GraphicsStateCollection.SaveToFile_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		public unsafe bool SendToEditor(string fileName)
		{
			bool result;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(fileName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = fileName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = GraphicsStateCollection.SendToEditor_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		[NativeName("Warmup")]
		public JobHandle WarmUp(JobHandle dependency = default(JobHandle))
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			JobHandle result;
			GraphicsStateCollection.WarmUp_Injected(intPtr, ref dependency, out result);
			return result;
		}

		[NativeName("WarmupProgressively")]
		public JobHandle WarmUpProgressively(int count, JobHandle dependency = default(JobHandle))
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			JobHandle result;
			GraphicsStateCollection.WarmUpProgressively_Injected(intPtr, count, ref dependency, out result);
			return result;
		}

		public int totalGraphicsStateCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_totalGraphicsStateCount_Injected(intPtr);
			}
		}

		public int completedWarmupCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_completedWarmupCount_Injected(intPtr);
			}
		}

		public bool isWarmedUp
		{
			[NativeName("IsWarmedUp")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_isWarmedUp_Injected(intPtr);
			}
		}

		private void GetVariants([Out] GraphicsStateCollection.ShaderVariant[] results)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsStateCollection.GetVariants_Injected(intPtr, results);
		}

		public void GetVariants(List<GraphicsStateCollection.ShaderVariant> results)
		{
			bool flag = results == null;
			if (flag)
			{
				throw new ArgumentNullException("The result shader variant list cannot be null.");
			}
			results.Clear();
			NoAllocHelpers.EnsureListElemCount<GraphicsStateCollection.ShaderVariant>(results, this.variantCount);
			this.GetVariants(NoAllocHelpers.ExtractArrayFromList<GraphicsStateCollection.ShaderVariant>(results));
		}

		private void GetGraphicsStatesForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords, [Out] GraphicsStateCollection.GraphicsState[] results)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsStateCollection.GetGraphicsStatesForVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords, results);
		}

		public void GetGraphicsStatesForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords, List<GraphicsStateCollection.GraphicsState> results)
		{
			bool flag = results == null;
			if (flag)
			{
				throw new ArgumentNullException("The result graphics state list cannot be null.");
			}
			results.Clear();
			NoAllocHelpers.EnsureListElemCount<GraphicsStateCollection.GraphicsState>(results, this.GetGraphicsStateCountForVariant(shader, passId, keywords));
			this.GetGraphicsStatesForVariant(shader, passId, keywords, NoAllocHelpers.ExtractArrayFromList<GraphicsStateCollection.GraphicsState>(results));
		}

		public int variantCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_variantCount_Injected(intPtr);
			}
		}

		public int GetGraphicsStateCountForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.GetGraphicsStateCountForVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords);
		}

		public bool AddVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.AddVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords);
		}

		public bool RemoveVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.RemoveVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords);
		}

		public bool ContainsVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.ContainsVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords);
		}

		public void ClearVariants()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsStateCollection.ClearVariants_Injected(intPtr);
		}

		public bool AddGraphicsStateForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords, GraphicsStateCollection.GraphicsState setup)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.AddGraphicsStateForVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords, ref setup);
		}

		public bool RemoveGraphicsStatesForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.RemoveGraphicsStatesForVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords);
		}

		[NativeName("CreateFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] GraphicsStateCollection gsc);

		public GraphicsStateCollection()
		{
			GraphicsStateCollection.Internal_Create(this);
		}

		public GraphicsStateCollection(string filePath)
		{
			GraphicsStateCollection.Internal_Create(this);
			this.LoadFromFile(filePath);
		}

		public void GetGraphicsStatesForVariant(GraphicsStateCollection.ShaderVariant variant, List<GraphicsStateCollection.GraphicsState> results)
		{
			this.GetGraphicsStatesForVariant(variant.shader, variant.passId, variant.keywords, results);
		}

		public int GetGraphicsStateCountForVariant(GraphicsStateCollection.ShaderVariant variant)
		{
			return this.GetGraphicsStateCountForVariant(variant.shader, variant.passId, variant.keywords);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool BeginTrace_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EndTrace_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isTracing_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_version_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_version_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsDeviceType get_graphicsDeviceType_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_graphicsDeviceType_Injected(IntPtr _unity_self, GraphicsDeviceType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RuntimePlatform get_runtimePlatform_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_runtimePlatform_Injected(IntPtr _unity_self, RuntimePlatform value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_qualityLevelName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_qualityLevelName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool LoadFromFile_Injected(IntPtr _unity_self, ref ManagedSpanWrapper filePath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SaveToFile_Injected(IntPtr _unity_self, ref ManagedSpanWrapper filePath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SendToEditor_Injected(IntPtr _unity_self, ref ManagedSpanWrapper fileName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WarmUp_Injected(IntPtr _unity_self, [In] ref JobHandle dependency, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WarmUpProgressively_Injected(IntPtr _unity_self, int count, [In] ref JobHandle dependency, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_totalGraphicsStateCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_completedWarmupCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isWarmedUp_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVariants_Injected(IntPtr _unity_self, [Out] GraphicsStateCollection.ShaderVariant[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGraphicsStatesForVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords, [Out] GraphicsStateCollection.GraphicsState[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_variantCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetGraphicsStateCountForVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContainsVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearVariants_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddGraphicsStateForVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords, [In] ref GraphicsStateCollection.GraphicsState setup);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveGraphicsStatesForVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

		public struct GraphicsState
		{
			public VertexAttributeDescriptor[] vertexAttributes;

			public AttachmentDescriptor[] attachments;

			public SubPassDescriptor[] subPasses;

			public RenderStateBlock renderState;

			public MeshTopology topology;

			public CullMode forceCullMode;

			public ShadingRateCombiner shadingRateCombinerPrimitive;

			public ShadingRateCombiner shadingRateCombinerFragment;

			public ShadingRateFragmentSize baseShadingRate;

			public float depthBias;

			public float slopeDepthBias;

			public int depthAttachmentIndex;

			public int subPassIndex;

			public int shadingRateIndex;

			public int multiviewCount;

			public int sampleCount;

			public bool wireframe;

			public bool invertCulling;

			public bool negativeScale;

			public bool invertProjection;
		}

		public struct ShaderVariant
		{
			public Shader shader;

			public PassIdentifier passId;

			public LocalKeyword[] keywords;
		}
	}
}
