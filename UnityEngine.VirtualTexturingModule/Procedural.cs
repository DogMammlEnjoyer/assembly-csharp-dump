using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.VirtualTexturing
{
	[StaticAccessor("VirtualTexturing::Procedural", StaticAccessorType.DoubleColon)]
	[Obsolete("Procedural Virtual Texturing is experimental, not ready for production use and Unity does not currently support it. The feature might be changed or removed in the future.", false)]
	[NativeHeader("Modules/VirtualTexturing/ScriptBindings/VirtualTexturing.bindings.h")]
	public static class Procedural
	{
		[NativeThrows]
		public static void SetDebugFlagInteger(Guid guid, long value)
		{
			System.SetDebugFlagInteger(guid, value);
		}

		[NativeThrows]
		public static void SetDebugFlagDouble(Guid guid, double value)
		{
			System.SetDebugFlagDouble(guid, value);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCPUCacheSize(int sizeInMegabytes);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCPUCacheSize();

		[NativeThrows]
		public unsafe static void SetGPUCacheSettings(GPUCacheSetting[] cacheSettings)
		{
			Span<GPUCacheSetting> span = new Span<GPUCacheSetting>(cacheSettings);
			fixed (GPUCacheSetting* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Procedural.SetGPUCacheSettings_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public static GPUCacheSetting[] GetGPUCacheSettings()
		{
			GPUCacheSetting[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Procedural.GetGPUCacheSettings_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				GPUCacheSetting[] array;
				blittableArrayWrapper.Unmarshal<GPUCacheSetting>(ref array);
				result = array;
			}
			return result;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetGPUCacheStagingAreaCapacity(uint tilesPerFrame);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetGPUCacheStagingAreaCapacity();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGPUCacheSettings_Injected(ref ManagedSpanWrapper cacheSettings);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGPUCacheSettings_Injected(out BlittableArrayWrapper ret);

		[StaticAccessor("VirtualTexturing::Procedural", StaticAccessorType.DoubleColon)]
		[NativeHeader("Modules/VirtualTexturing/ScriptBindings/VirtualTexturing.bindings.h")]
		internal static class Binding
		{
			internal static ulong Create(Procedural.CreationParameters p)
			{
				return Procedural.Binding.Create_Injected(ref p);
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void Destroy(ulong handle);

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern int PopRequests(ulong handle, IntPtr requestHandles, int length);

			[ThreadSafe]
			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void GetRequestParameters(IntPtr requestHandles, IntPtr requestParameters, int length);

			[NativeThrows]
			[ThreadSafe]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void UpdateRequestState(IntPtr requestHandles, IntPtr requestUpdates, int length);

			[NativeThrows]
			[ThreadSafe]
			internal static void UpdateRequestStateWithCommandBuffer(IntPtr requestHandles, IntPtr requestUpdates, int length, CommandBuffer fenceBuffer)
			{
				Procedural.Binding.UpdateRequestStateWithCommandBuffer_Injected(requestHandles, requestUpdates, length, (fenceBuffer == null) ? ((IntPtr)0) : CommandBuffer.BindingsMarshaller.ConvertToNative(fenceBuffer));
			}

			internal unsafe static void BindToMaterialPropertyBlock(ulong handle, [NotNull] MaterialPropertyBlock material, string name)
			{
				if (material == null)
				{
					ThrowHelper.ThrowArgumentNullException(material, "material");
				}
				try
				{
					IntPtr intPtr = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(material);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowArgumentNullException(material, "material");
					}
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = name.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					Procedural.Binding.BindToMaterialPropertyBlock_Injected(handle, intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}

			internal unsafe static void BindToMaterial(ulong handle, [NotNull] Material material, string name)
			{
				if (material == null)
				{
					ThrowHelper.ThrowArgumentNullException(material, "material");
				}
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(material);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowArgumentNullException(material, "material");
					}
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = name.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					Procedural.Binding.BindToMaterial_Injected(handle, intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}

			internal unsafe static void BindGlobally(ulong handle, string name)
			{
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = name.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					Procedural.Binding.BindGlobally_Injected(handle, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}

			[NativeThrows]
			internal static void RequestRegion(ulong handle, Rect r, int mipMap, int numMips)
			{
				Procedural.Binding.RequestRegion_Injected(handle, ref r, mipMap, numMips);
			}

			[NativeThrows]
			internal static void InvalidateRegion(ulong handle, Rect r, int mipMap, int numMips)
			{
				Procedural.Binding.InvalidateRegion_Injected(handle, ref r, mipMap, numMips);
			}

			[NativeThrows]
			public static void EvictRegion(ulong handle, Rect r, int mipMap, int numMips)
			{
				Procedural.Binding.EvictRegion_Injected(handle, ref r, mipMap, numMips);
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ulong Create_Injected([In] ref Procedural.CreationParameters p);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void UpdateRequestStateWithCommandBuffer_Injected(IntPtr requestHandles, IntPtr requestUpdates, int length, IntPtr fenceBuffer);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void BindToMaterialPropertyBlock_Injected(ulong handle, IntPtr material, ref ManagedSpanWrapper name);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void BindToMaterial_Injected(ulong handle, IntPtr material, ref ManagedSpanWrapper name);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void BindGlobally_Injected(ulong handle, ref ManagedSpanWrapper name);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void RequestRegion_Injected(ulong handle, [In] ref Rect r, int mipMap, int numMips);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void InvalidateRegion_Injected(ulong handle, [In] ref Rect r, int mipMap, int numMips);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void EvictRegion_Injected(ulong handle, [In] ref Rect r, int mipMap, int numMips);
		}

		[NativeHeader("Modules/VirtualTexturing/ScriptBindings/VirtualTexturing.bindings.h")]
		public struct CreationParameters
		{
			internal void Validate()
			{
				bool flag = this.width <= 0 || this.height <= 0 || this.tilesize <= 0;
				if (flag)
				{
					throw new ArgumentException(string.Format("Zero sized dimensions are invalid (width: {0}, height: {1}, tilesize {2}", this.width, this.height, this.tilesize));
				}
				bool flag2 = this.layers == null || this.layers.Length > 4;
				if (flag2)
				{
					throw new ArgumentException(string.Format("layers is either invalid or has too many layers (maxNumLayers: {0})", 4));
				}
				bool flag3 = this.gpuGeneration == 1 && this.filterMode != FilterMode.Bilinear;
				if (flag3)
				{
					throw new ArgumentException("Filter mode invalid for GPU PVT; only FilterMode.Bilinear is currently supported");
				}
				bool flag4 = this.gpuGeneration == 0 && this.filterMode != FilterMode.Bilinear && this.filterMode != FilterMode.Trilinear;
				if (flag4)
				{
					throw new ArgumentException("Filter mode invalid for CPU PVT; only FilterMode.Bilinear and FilterMode.Trilinear are currently supported");
				}
				GraphicsFormat[] array = new GraphicsFormat[22];
				RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.C46F0FCA118FE72C10BF852A830B34A1639A8D2308DB41CA204DB0CB19AA3E4E).FieldHandle);
				GraphicsFormat[] array2 = array;
				GraphicsFormat[] array3 = new GraphicsFormat[8];
				RuntimeHelpers.InitializeArray(array3, fieldof(<PrivateImplementationDetails>.BF8B1BB84B6FB3C983D21ED84FADE89DCC6FDB7C91DE320DE34AB9ABE24493FC).FieldHandle);
				GraphicsFormat[] array4 = array3;
				GraphicsFormatUsage usage = (this.gpuGeneration == 1) ? GraphicsFormatUsage.Render : GraphicsFormatUsage.Sample;
				for (int i = 0; i < this.layers.Length; i++)
				{
					bool flag5 = SystemInfo.GetCompatibleFormat(this.layers[i], usage) != this.layers[i];
					if (flag5)
					{
						throw new ArgumentException(string.Format("Requested format {0} on layer {1} is not supported on this platform", this.layers[i], i));
					}
					bool flag6 = false;
					GraphicsFormat[] array5 = (this.gpuGeneration == 1) ? array4 : array2;
					for (int j = 0; j < array5.Length; j++)
					{
						bool flag7 = this.layers[i] == array5[j];
						if (flag7)
						{
							flag6 = true;
							break;
						}
					}
					bool flag8 = !flag6;
					if (flag8)
					{
						string arg = (this.gpuGeneration == 1) ? "GPU" : "CPU";
						throw new ArgumentException(string.Format("{0} Procedural Virtual Texturing doesn't support GraphicsFormat {1} for stack layer {2}", arg, this.layers[i], i));
					}
				}
				bool flag9 = this.maxActiveRequests > 4095 || this.maxActiveRequests <= 0;
				if (flag9)
				{
					throw new ArgumentException(string.Format("Invalid requests per frame (maxActiveRequests: ]0, {0}])", this.maxActiveRequests));
				}
			}

			public const int MaxNumLayers = 4;

			public const int MaxRequestsPerFrameSupported = 4095;

			public int width;

			public int height;

			public int maxActiveRequests;

			public int tilesize;

			public GraphicsFormat[] layers;

			public FilterMode filterMode;

			internal int borderSize;

			internal int gpuGeneration;

			internal int flags;
		}

		[UsedByNativeCode]
		[NativeHeader("Modules/VirtualTexturing/ScriptBindings/VirtualTexturing.bindings.h")]
		internal struct RequestHandlePayload : IEquatable<Procedural.RequestHandlePayload>
		{
			public static bool operator !=(Procedural.RequestHandlePayload lhs, Procedural.RequestHandlePayload rhs)
			{
				return !(lhs == rhs);
			}

			public override bool Equals(object obj)
			{
				return obj is Procedural.RequestHandlePayload && this == (Procedural.RequestHandlePayload)obj;
			}

			public bool Equals(Procedural.RequestHandlePayload other)
			{
				return this == other;
			}

			public override int GetHashCode()
			{
				int num = -2128608763;
				num = num * -1521134295 + this.id.GetHashCode();
				num = num * -1521134295 + this.lifetime.GetHashCode();
				return num * -1521134295 + this.callback.GetHashCode();
			}

			public static bool operator ==(Procedural.RequestHandlePayload lhs, Procedural.RequestHandlePayload rhs)
			{
				return lhs.id == rhs.id && lhs.lifetime == rhs.lifetime && lhs.callback == rhs.callback;
			}

			internal int id;

			internal int lifetime;

			[NativeDisableUnsafePtrRestriction]
			internal IntPtr callback;
		}

		public struct TextureStackRequestHandle<T> : IEquatable<Procedural.TextureStackRequestHandle<T>> where T : struct
		{
			public static bool operator !=(Procedural.TextureStackRequestHandle<T> h1, Procedural.TextureStackRequestHandle<T> h2)
			{
				return !(h1 == h2);
			}

			public override bool Equals(object obj)
			{
				return obj is Procedural.TextureStackRequestHandle<T> && this == (Procedural.TextureStackRequestHandle<T>)obj;
			}

			public bool Equals(Procedural.TextureStackRequestHandle<T> other)
			{
				return this == other;
			}

			public override int GetHashCode()
			{
				return this.payload.GetHashCode();
			}

			public static bool operator ==(Procedural.TextureStackRequestHandle<T> h1, Procedural.TextureStackRequestHandle<T> h2)
			{
				return h1.payload == h2.payload;
			}

			public void CompleteRequest(Procedural.RequestStatus status)
			{
				Procedural.Binding.UpdateRequestState((IntPtr)UnsafeUtility.AddressOf<Procedural.TextureStackRequestHandle<T>>(ref this), (IntPtr)UnsafeUtility.AddressOf<Procedural.RequestStatus>(ref status), 1);
			}

			public void CompleteRequest(Procedural.RequestStatus status, CommandBuffer fenceBuffer)
			{
				Procedural.Binding.UpdateRequestStateWithCommandBuffer((IntPtr)UnsafeUtility.AddressOf<Procedural.TextureStackRequestHandle<T>>(ref this), (IntPtr)UnsafeUtility.AddressOf<Procedural.RequestStatus>(ref status), 1, fenceBuffer);
			}

			public static void CompleteRequests(NativeSlice<Procedural.TextureStackRequestHandle<T>> requestHandles, NativeSlice<Procedural.RequestStatus> status)
			{
				bool flag = !System.enabled;
				if (flag)
				{
					throw new InvalidOperationException("Virtual texturing is not enabled in the player settings.");
				}
				bool flag2 = requestHandles.Length != status.Length;
				if (flag2)
				{
					throw new ArgumentException(string.Format("Array sizes do not match ({0} handles, {1} requests)", requestHandles.Length, status.Length));
				}
				Procedural.Binding.UpdateRequestState((IntPtr)requestHandles.GetUnsafePtr<Procedural.TextureStackRequestHandle<T>>(), (IntPtr)status.GetUnsafePtr<Procedural.RequestStatus>(), requestHandles.Length);
			}

			public static void CompleteRequests(NativeSlice<Procedural.TextureStackRequestHandle<T>> requestHandles, NativeSlice<Procedural.RequestStatus> status, CommandBuffer fenceBuffer)
			{
				bool flag = !System.enabled;
				if (flag)
				{
					throw new InvalidOperationException("Virtual texturing is not enabled in the player settings.");
				}
				bool flag2 = requestHandles.Length != status.Length;
				if (flag2)
				{
					throw new ArgumentException(string.Format("Array sizes do not match ({0} handles, {1} requests)", requestHandles.Length, status.Length));
				}
				Procedural.Binding.UpdateRequestStateWithCommandBuffer((IntPtr)requestHandles.GetUnsafePtr<Procedural.TextureStackRequestHandle<T>>(), (IntPtr)status.GetUnsafePtr<Procedural.RequestStatus>(), requestHandles.Length, fenceBuffer);
			}

			public T GetRequestParameters()
			{
				T result = Activator.CreateInstance<T>();
				Procedural.Binding.GetRequestParameters((IntPtr)UnsafeUtility.AddressOf<Procedural.TextureStackRequestHandle<T>>(ref this), (IntPtr)UnsafeUtility.AddressOf<T>(ref result), 1);
				return result;
			}

			public static void GetRequestParameters(NativeSlice<Procedural.TextureStackRequestHandle<T>> handles, NativeSlice<T> requests)
			{
				bool flag = !System.enabled;
				if (flag)
				{
					throw new InvalidOperationException("Virtual texturing is not enabled in the player settings.");
				}
				bool flag2 = handles.Length != requests.Length;
				if (flag2)
				{
					throw new ArgumentException(string.Format("Array sizes do not match ({0} handles, {1} requests)", handles.Length, requests.Length));
				}
				Procedural.Binding.GetRequestParameters((IntPtr)handles.GetUnsafePtr<Procedural.TextureStackRequestHandle<T>>(), (IntPtr)requests.GetUnsafePtr<T>(), handles.Length);
			}

			internal Procedural.RequestHandlePayload payload;
		}

		[NativeHeader("Modules/VirtualTexturing/ScriptBindings/VirtualTexturing.bindings.h")]
		[UsedByNativeCode]
		public struct GPUTextureStackRequestLayerParameters
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern int GetWidth();

			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern int GetHeight();

			public int destX;

			public int destY;

			public RenderTargetIdentifier dest;
		}

		[NativeHeader("Modules/VirtualTexturing/ScriptBindings/VirtualTexturing.bindings.h")]
		[UsedByNativeCode]
		public struct CPUTextureStackRequestLayerParameters
		{
			public NativeArray<T> GetData<T>() where T : struct
			{
				return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(this.data, this.dataSize, Allocator.None);
			}

			public NativeArray<T> GetMipData<T>() where T : struct
			{
				return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(this.mipData, this.mipDataSize, Allocator.None);
			}

			public int scanlineSize
			{
				get
				{
					return this._scanlineSize;
				}
			}

			public int mipScanlineSize
			{
				get
				{
					return this._mipScanlineSize;
				}
			}

			public bool requiresCachedMip
			{
				get
				{
					return this.mipDataSize != 0;
				}
			}

			internal int _scanlineSize;

			internal int dataSize;

			[NativeDisableUnsafePtrRestriction]
			internal unsafe void* data;

			internal int _mipScanlineSize;

			internal int mipDataSize;

			[NativeDisableUnsafePtrRestriction]
			internal unsafe void* mipData;
		}

		[NativeHeader("Modules/VirtualTexturing/ScriptBindings/VirtualTexturing.bindings.h")]
		[UsedByNativeCode]
		public struct GPUTextureStackRequestParameters
		{
			public Procedural.GPUTextureStackRequestLayerParameters GetLayer(int index)
			{
				Procedural.GPUTextureStackRequestLayerParameters result;
				switch (index)
				{
				case 0:
					result = this.layer0;
					break;
				case 1:
					result = this.layer1;
					break;
				case 2:
					result = this.layer2;
					break;
				case 3:
					result = this.layer3;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
				return result;
			}

			public int level;

			public int x;

			public int y;

			public int width;

			public int height;

			public int numLayers;

			private Procedural.GPUTextureStackRequestLayerParameters layer0;

			private Procedural.GPUTextureStackRequestLayerParameters layer1;

			private Procedural.GPUTextureStackRequestLayerParameters layer2;

			private Procedural.GPUTextureStackRequestLayerParameters layer3;
		}

		[UsedByNativeCode]
		[NativeHeader("Modules/VirtualTexturing/ScriptBindings/VirtualTexturing.bindings.h")]
		public struct CPUTextureStackRequestParameters
		{
			public Procedural.CPUTextureStackRequestLayerParameters GetLayer(int index)
			{
				Procedural.CPUTextureStackRequestLayerParameters result;
				switch (index)
				{
				case 0:
					result = this.layer0;
					break;
				case 1:
					result = this.layer1;
					break;
				case 2:
					result = this.layer2;
					break;
				case 3:
					result = this.layer3;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
				return result;
			}

			public int level;

			public int x;

			public int y;

			public int width;

			public int height;

			public int numLayers;

			private Procedural.CPUTextureStackRequestLayerParameters layer0;

			private Procedural.CPUTextureStackRequestLayerParameters layer1;

			private Procedural.CPUTextureStackRequestLayerParameters layer2;

			private Procedural.CPUTextureStackRequestLayerParameters layer3;
		}

		[UsedByNativeCode]
		internal enum ProceduralTextureStackRequestStatus
		{
			StatusFree = 65535,
			StatusRequested,
			StatusProcessing,
			StatusComplete,
			StatusDropped
		}

		public enum RequestStatus
		{
			Dropped = 65539,
			Generated = 65538
		}

		public class TextureStackBase<T> : IDisposable where T : struct
		{
			public int PopRequests(NativeSlice<Procedural.TextureStackRequestHandle<T>> requestHandles)
			{
				bool flag = !this.IsValid();
				if (flag)
				{
					throw new InvalidOperationException("Invalid ProceduralTextureStack " + this.name);
				}
				return Procedural.Binding.PopRequests(this.handle, (IntPtr)requestHandles.GetUnsafePtr<Procedural.TextureStackRequestHandle<T>>(), requestHandles.Length);
			}

			public bool IsValid()
			{
				return this.handle > 0UL;
			}

			public TextureStackBase(string _name, Procedural.CreationParameters _creationParams, bool gpuGeneration)
			{
				bool flag = !System.enabled;
				if (flag)
				{
					throw new InvalidOperationException("Virtual texturing is not enabled in the player settings.");
				}
				this.name = _name;
				this.creationParams = _creationParams;
				this.creationParams.borderSize = Procedural.TextureStackBase<T>.borderSize;
				this.creationParams.gpuGeneration = (gpuGeneration ? 1 : 0);
				this.creationParams.flags = 0;
				this.creationParams.Validate();
				this.handle = Procedural.Binding.Create(this.creationParams);
			}

			public void Dispose()
			{
				bool flag = this.IsValid();
				if (flag)
				{
					Procedural.Binding.Destroy(this.handle);
					this.handle = 0UL;
				}
			}

			public void BindToMaterialPropertyBlock(MaterialPropertyBlock mpb)
			{
				bool flag = mpb == null;
				if (flag)
				{
					throw new ArgumentNullException("mbp");
				}
				bool flag2 = !this.IsValid();
				if (flag2)
				{
					throw new InvalidOperationException("Invalid ProceduralTextureStack " + this.name);
				}
				Procedural.Binding.BindToMaterialPropertyBlock(this.handle, mpb, this.name);
			}

			public void BindToMaterial(Material mat)
			{
				bool flag = mat == null;
				if (flag)
				{
					throw new ArgumentNullException("mat");
				}
				bool flag2 = !this.IsValid();
				if (flag2)
				{
					throw new InvalidOperationException("Invalid ProceduralTextureStack " + this.name);
				}
				Procedural.Binding.BindToMaterial(this.handle, mat, this.name);
			}

			public void BindGlobally()
			{
				bool flag = !this.IsValid();
				if (flag)
				{
					throw new InvalidOperationException("Invalid ProceduralTextureStack " + this.name);
				}
				Procedural.Binding.BindGlobally(this.handle, this.name);
			}

			public void RequestRegion(Rect r, int mipMap, int numMips)
			{
				bool flag = !this.IsValid();
				if (flag)
				{
					throw new InvalidOperationException("Invalid ProceduralTextureStack " + this.name);
				}
				Procedural.Binding.RequestRegion(this.handle, r, mipMap, numMips);
			}

			public void InvalidateRegion(Rect r, int mipMap, int numMips)
			{
				bool flag = !this.IsValid();
				if (flag)
				{
					throw new InvalidOperationException("Invalid ProceduralTextureStack " + this.name);
				}
				Procedural.Binding.InvalidateRegion(this.handle, r, mipMap, numMips);
			}

			public void EvictRegion(Rect r, int mipMap, int numMips)
			{
				bool flag = !this.IsValid();
				if (flag)
				{
					throw new InvalidOperationException("Invalid ProceduralTextureStack " + this.name);
				}
				Procedural.Binding.EvictRegion(this.handle, r, mipMap, numMips);
			}

			internal ulong handle;

			public static readonly int borderSize = 8;

			private string name;

			private Procedural.CreationParameters creationParams;

			public const int AllMips = 2147483647;
		}

		public sealed class GPUTextureStack : Procedural.TextureStackBase<Procedural.GPUTextureStackRequestParameters>
		{
			public GPUTextureStack(string _name, Procedural.CreationParameters creationParams) : base(_name, creationParams, true)
			{
			}
		}

		public sealed class CPUTextureStack : Procedural.TextureStackBase<Procedural.CPUTextureStackRequestParameters>
		{
			public CPUTextureStack(string _name, Procedural.CreationParameters creationParams) : base(_name, creationParams, false)
			{
			}
		}
	}
}
