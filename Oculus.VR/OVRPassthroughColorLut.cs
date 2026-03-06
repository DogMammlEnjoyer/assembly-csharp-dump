using System;
using System.Runtime.InteropServices;
using Meta.XR.Util;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-customize-passthrough-color-mapping/#color-look-up-tables-luts")]
[Feature(Feature.Passthrough)]
public class OVRPassthroughColorLut : IDisposable
{
	public uint Resolution { get; private set; }

	public OVRPassthroughColorLut.ColorChannels Channels { get; private set; }

	[Obsolete("IsInitialized is deprecated. Use IsValid instead.", false)]
	public bool IsInitialized
	{
		get
		{
			return this.IsValid;
		}
	}

	public bool IsValid
	{
		get
		{
			return this._createState > OVRPassthroughColorLut.CreateState.Invalid;
		}
	}

	public OVRPassthroughColorLut(Texture2D initialLutTexture, bool flipY = true) : this(OVRPassthroughColorLut.GetTextureSize(initialLutTexture), OVRPassthroughColorLut.GetChannelsForTextureFormat(initialLutTexture.format))
	{
		this.Create(this.CreateLutDataFromTexture(initialLutTexture, flipY));
	}

	public OVRPassthroughColorLut(Color[] initialColorLut, OVRPassthroughColorLut.ColorChannels channels) : this(OVRPassthroughColorLut.GetArraySize<Color>(initialColorLut), channels)
	{
		this.Create(this.CreateLutDataFromArray(initialColorLut));
	}

	public OVRPassthroughColorLut(Color32[] initialColorLut, OVRPassthroughColorLut.ColorChannels channels) : this(OVRPassthroughColorLut.GetArraySize<Color32>(initialColorLut), channels)
	{
		this.Create(this.CreateLutDataFromArray(initialColorLut));
	}

	public OVRPassthroughColorLut(byte[] initialColorLut, OVRPassthroughColorLut.ColorChannels channels) : this(OVRPassthroughColorLut.GetTextureSizeFromByteArray(initialColorLut, channels), channels)
	{
		this.Create(this.CreateLutDataFromArray(initialColorLut));
	}

	public void UpdateFrom(Color[] colors)
	{
		if (this.IsValidLutUpdate<Color>(colors, this._channelCount))
		{
			this.WriteColorsAsBytes(colors, this._colorBytes);
			OVRPlugin.UpdatePassthroughColorLut(this._colorLutHandle, this._lutData);
		}
	}

	public void UpdateFrom(Color32[] colors)
	{
		if (this.IsValidLutUpdate<Color32>(colors, this._channelCount))
		{
			this.WriteColorsAsBytes(colors, this._colorBytes);
			OVRPlugin.UpdatePassthroughColorLut(this._colorLutHandle, this._lutData);
		}
	}

	public void UpdateFrom(byte[] colors)
	{
		if (this.IsValidLutUpdate<byte>(colors, 1))
		{
			colors.CopyTo(this._colorBytes, 0);
			OVRPlugin.UpdatePassthroughColorLut(this._colorLutHandle, this._lutData);
		}
	}

	public void UpdateFrom(Texture2D lutTexture, bool flipY = true)
	{
		if (this.IsValidUpdateResolution(OVRPassthroughColorLut.GetTextureSize(lutTexture), this._channelCount))
		{
			OVRPassthroughColorLut.ColorLutTextureConverter.TextureToColorByteMap(lutTexture, this._channelCount, this._colorBytes, flipY);
			OVRPlugin.UpdatePassthroughColorLut(this._colorLutHandle, this._lutData);
		}
	}

	public void Dispose()
	{
		if (this.IsValid)
		{
			OVRManager.OnPassthroughInitializedStateChange = (Action<bool>)Delegate.Remove(OVRManager.OnPassthroughInitializedStateChange, new Action<bool>(this.RefreshIfInitialized));
		}
		this.Destroy();
		this.FreeAllocHandle();
	}

	private void FreeAllocHandle()
	{
		GCHandle allocHandle = this._allocHandle;
		if (this._allocHandle.IsAllocated)
		{
			this._allocHandle.Free();
		}
	}

	public static bool IsTextureSupported(Texture2D texture, out string errorMessage)
	{
		try
		{
			OVRPassthroughColorLut.GetChannelsForTextureFormat(texture.format);
		}
		catch (ArgumentException ex)
		{
			errorMessage = ex.Message;
			return false;
		}
		int num;
		int num2;
		string text;
		if (!OVRPassthroughColorLut.ColorLutTextureConverter.TryGetTextureLayout(texture.width, texture.height, out num, out num2, out text))
		{
			errorMessage = text;
			return false;
		}
		int size = texture.width * texture.height;
		string text2;
		if (!OVRPassthroughColorLut.IsResolutionAccepted(OVRPassthroughColorLut.GetResolutionFromSize(size), size, out text2))
		{
			errorMessage = text2;
			return false;
		}
		errorMessage = string.Empty;
		return true;
	}

	private OVRPassthroughColorLut(int size, OVRPassthroughColorLut.ColorChannels channels)
	{
		this.Channels = channels;
		this.Resolution = OVRPassthroughColorLut.GetResolutionFromSize(size);
		this._channelCount = OVRPassthroughColorLut.ChannelsToCount(channels);
		string message;
		if (!OVRPassthroughColorLut.IsResolutionAccepted(this.Resolution, size, out message))
		{
			throw new ArgumentException(message);
		}
		OVRManager.PassthroughCapabilities passthroughCapabilities = OVRManager.GetPassthroughCapabilities();
		if (passthroughCapabilities != null)
		{
			if (passthroughCapabilities.MaxColorLutResolution == 0U)
			{
				throw new Exception("Passthrough Color LUTs are not supported.");
			}
			if (this.Resolution > passthroughCapabilities.MaxColorLutResolution)
			{
				throw new Exception(string.Format("Color LUT resolution {0} exceeds the maximum of {1}.", this.Resolution, passthroughCapabilities.MaxColorLutResolution));
			}
		}
		else
		{
			Debug.LogWarning("Unable to validate the maximum LUT resolution. Please instantiate OVRPassthroughColorLut after initializing the Oculus XR Plugin.");
		}
	}

	private bool IsValidUpdateResolution(int lutSize, int elementByteSize)
	{
		if (!this.IsValid)
		{
			Debug.LogError("Can not update an uninitialized lut object.");
			return false;
		}
		if (OVRPassthroughColorLut.GetResolutionFromSize(lutSize * elementByteSize / this._channelCount) != this.Resolution)
		{
			Debug.LogError(string.Format("Can only update with the same resolution of {0}.", this.Resolution));
			return false;
		}
		return true;
	}

	private bool IsValidLutUpdate<T>(T[] colorArray, int elementByteSize)
	{
		int arraySize = OVRPassthroughColorLut.GetArraySize<T>(colorArray);
		if (!this.IsValidUpdateResolution(arraySize, elementByteSize))
		{
			return false;
		}
		if (arraySize * elementByteSize != this._colorBytes.Length)
		{
			Debug.LogError("New color byte array doesn't match LUT size.");
			return false;
		}
		return true;
	}

	private static OVRPassthroughColorLut.ColorChannels GetChannelsForTextureFormat(TextureFormat format)
	{
		if (format == TextureFormat.RGB24)
		{
			return OVRPassthroughColorLut.ColorChannels.Rgb;
		}
		if (format != TextureFormat.RGBA32)
		{
			throw new ArgumentException(string.Format("Texture format {0} not supported for Color LUTs. Supported formats are RGB24 and RGBA32.", format));
		}
		return OVRPassthroughColorLut.ColorChannels.Rgba;
	}

	private static int GetTextureSizeFromByteArray(byte[] initialColorLut, OVRPassthroughColorLut.ColorChannels channels)
	{
		bool arraySize = OVRPassthroughColorLut.GetArraySize<byte>(initialColorLut) != 0;
		int num = OVRPassthroughColorLut.ChannelsToCount(channels);
		if ((arraySize ? 1 : 0) % num != 0)
		{
			throw new ArgumentException(string.Format("Invalid byte array given, {0} bytes required for each color for {1} color channels.", num, channels));
		}
		return initialColorLut.Length / num;
	}

	private static int GetTextureSize(Texture2D texture)
	{
		if (texture == null)
		{
			throw new ArgumentNullException("Lut texture is undefined.");
		}
		return texture.width * texture.height;
	}

	private static int GetArraySize<T>(T[] array)
	{
		if (array == null)
		{
			throw new ArgumentNullException("Lut " + typeof(T).Name + " array is undefined.");
		}
		return array.Length;
	}

	private static int ChannelsToCount(OVRPassthroughColorLut.ColorChannels channels)
	{
		if (channels != OVRPassthroughColorLut.ColorChannels.Rgb)
		{
			return 4;
		}
		return 3;
	}

	private static bool IsResolutionAccepted(uint resolution, int size, out string errorMessage)
	{
		if (!OVRPassthroughColorLut.IsPowerOfTwo(resolution))
		{
			errorMessage = "Color LUT texture resolution should be a power of 2.";
			return false;
		}
		if ((ulong)(resolution * resolution * resolution) != (ulong)((long)size))
		{
			errorMessage = "Unexpected LUT resolution, LUT size should be resolution in a power of 3.";
			return false;
		}
		errorMessage = string.Empty;
		return true;
	}

	private static bool IsPowerOfTwo(uint x)
	{
		return x != 0U && (x & x - 1U) == 0U;
	}

	private void Create(OVRPlugin.PassthroughColorLutData lutData)
	{
		this._lutData = lutData;
		if (OVRManager.IsInsightPassthroughInitialized())
		{
			this.InternalCreate();
		}
		else
		{
			this._createState = OVRPassthroughColorLut.CreateState.Pending;
		}
		if (this.IsValid)
		{
			OVRManager.OnPassthroughInitializedStateChange = (Action<bool>)Delegate.Combine(OVRManager.OnPassthroughInitializedStateChange, new Action<bool>(this.RefreshIfInitialized));
		}
	}

	private void RefreshIfInitialized(bool isInitialized)
	{
		if (isInitialized)
		{
			this.Recreate();
		}
	}

	private void Recreate()
	{
		this.Destroy();
		this.InternalCreate();
	}

	private void InternalCreate()
	{
		this._createState = (OVRPlugin.CreatePassthroughColorLut((OVRPlugin.PassthroughColorLutChannels)this.Channels, this.Resolution, this._lutData, out this._colorLutHandle) ? OVRPassthroughColorLut.CreateState.Created : OVRPassthroughColorLut.CreateState.Invalid);
		if (!this.IsValid)
		{
			Debug.LogError("Failed to create Passthrough Color LUT.");
		}
	}

	private static uint GetResolutionFromSize(int size)
	{
		return (uint)Mathf.Round(Mathf.Pow((float)size, 0.33333334f));
	}

	private OVRPlugin.PassthroughColorLutData CreateLutData(out byte[] colorBytes)
	{
		OVRPlugin.PassthroughColorLutData passthroughColorLutData = new OVRPlugin.PassthroughColorLutData
		{
			BufferSize = (uint)((ulong)(this.Resolution * this.Resolution * this.Resolution) * (ulong)((long)this._channelCount))
		};
		colorBytes = new byte[passthroughColorLutData.BufferSize];
		this._allocHandle = GCHandle.Alloc(colorBytes, GCHandleType.Pinned);
		passthroughColorLutData.Buffer = this._allocHandle.AddrOfPinnedObject();
		return passthroughColorLutData;
	}

	private OVRPlugin.PassthroughColorLutData CreateLutDataFromTexture(Texture2D lut, bool flipY)
	{
		OVRPlugin.PassthroughColorLutData result = this.CreateLutData(out this._colorBytes);
		OVRPassthroughColorLut.ColorLutTextureConverter.TextureToColorByteMap(lut, this._channelCount, this._colorBytes, flipY);
		return result;
	}

	private OVRPlugin.PassthroughColorLutData CreateLutDataFromArray(Color[] colors)
	{
		OVRPlugin.PassthroughColorLutData result = this.CreateLutData(out this._colorBytes);
		this.WriteColorsAsBytes(colors, this._colorBytes);
		return result;
	}

	private OVRPlugin.PassthroughColorLutData CreateLutDataFromArray(Color32[] colors)
	{
		OVRPlugin.PassthroughColorLutData result = this.CreateLutData(out this._colorBytes);
		this.WriteColorsAsBytes(colors, this._colorBytes);
		return result;
	}

	private OVRPlugin.PassthroughColorLutData CreateLutDataFromArray(byte[] colors)
	{
		OVRPlugin.PassthroughColorLutData result = this.CreateLutData(out this._colorBytes);
		colors.CopyTo(this._colorBytes, 0);
		return result;
	}

	private void WriteColorsAsBytes(Color[] colors, byte[] target)
	{
		using (NativeArray<Color> source = new NativeArray<Color>(colors, Allocator.TempJob))
		{
			using (NativeArray<byte> target2 = new NativeArray<byte>(target, Allocator.TempJob))
			{
				new OVRPassthroughColorLut.WriteColorsAsBytesJob
				{
					source = source,
					target = target2,
					channelCount = this._channelCount
				}.Schedule(source.Length, 128, default(JobHandle)).Complete();
				target2.CopyTo(target);
			}
		}
	}

	private void WriteColorsAsBytes(Color32[] colors, byte[] target)
	{
		for (int i = 0; i < colors.Length; i++)
		{
			for (int j = 0; j < this._channelCount; j++)
			{
				target[i * this._channelCount + j] = colors[i][j];
			}
		}
	}

	~OVRPassthroughColorLut()
	{
		this.Dispose();
	}

	private void Destroy()
	{
		if (this._createState == OVRPassthroughColorLut.CreateState.Created)
		{
			object locker = this._locker;
			lock (locker)
			{
				OVRPlugin.DestroyPassthroughColorLut(this._colorLutHandle);
			}
		}
		this._createState = OVRPassthroughColorLut.CreateState.Invalid;
	}

	private const int RecomendedBatchSize = 128;

	internal ulong _colorLutHandle;

	private GCHandle _allocHandle;

	private OVRPlugin.PassthroughColorLutData _lutData;

	private int _channelCount;

	private byte[] _colorBytes;

	private object _locker = new object();

	private OVRPassthroughColorLut.CreateState _createState;

	public enum ColorChannels
	{
		Rgb = 1,
		Rgba
	}

	private struct WriteColorsAsBytesJob : IJobParallelFor
	{
		public void Execute(int index)
		{
			for (int i = 0; i < this.channelCount; i++)
			{
				this.target[index * this.channelCount + i] = (byte)Mathf.Min(this.source[index][i] * 255f, 255f);
			}
		}

		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public NativeArray<byte> target;

		[NativeDisableParallelForRestriction]
		[ReadOnly]
		public NativeArray<Color> source;

		public int channelCount;
	}

	private static class ColorLutTextureConverter
	{
		public static void TextureToColorByteMap(Texture2D lut, int channelCount, byte[] target, bool flipY)
		{
			OVRPassthroughColorLut.ColorLutTextureConverter.MapColorValues(OVRPassthroughColorLut.ColorLutTextureConverter.GetTextureSettings(lut, channelCount, flipY), lut.GetPixelData<byte>(0), target);
		}

		private static void MapColorValues(OVRPassthroughColorLut.ColorLutTextureConverter.TextureSettings settings, NativeArray<byte> source, byte[] target)
		{
			using (NativeArray<byte> target2 = new NativeArray<byte>(target, Allocator.TempJob))
			{
				new OVRPassthroughColorLut.ColorLutTextureConverter.MapColorValuesJob
				{
					settings = settings,
					source = source,
					target = target2
				}.Schedule(settings.Resolution * settings.Resolution, settings.Resolution, default(JobHandle)).Complete();
				target2.CopyTo(target);
			}
		}

		private static OVRPassthroughColorLut.ColorLutTextureConverter.TextureSettings GetTextureSettings(Texture2D lut, int channelCount, bool flipY)
		{
			int resolution;
			int slicesPerRow;
			string message;
			if (OVRPassthroughColorLut.ColorLutTextureConverter.TryGetTextureLayout(lut.width, lut.height, out resolution, out slicesPerRow, out message))
			{
				return new OVRPassthroughColorLut.ColorLutTextureConverter.TextureSettings(lut.width, lut.height, resolution, slicesPerRow, channelCount, flipY);
			}
			throw new Exception(message);
		}

		internal static bool TryGetTextureLayout(int width, int height, out int resolution, out int slicesPerRow, out string errorMessage)
		{
			resolution = -1;
			slicesPerRow = -1;
			if (width == height)
			{
				float num = Mathf.Pow((float)width, 0.6666667f);
				if ((double)Mathf.Abs(num - Mathf.Round(num)) > 0.001)
				{
					errorMessage = "Texture layout is not compatible for color LUTs: the dimensions don't result in a power-of-two resolution for the LUT. Acceptable image sizes are e.g. 64 (for a LUT resolution of 16) or 512 (for a LUT resolution of 64).";
					return false;
				}
				resolution = (int)Mathf.Round(num);
				slicesPerRow = (int)Mathf.Sqrt((float)resolution);
			}
			else
			{
				if (width != height * height)
				{
					errorMessage = "Texture layout is not compatible for color LUTs: for horizontal layouts, the Width is expected to be equal to Height * Height.";
					return false;
				}
				resolution = height;
				slicesPerRow = resolution;
			}
			errorMessage = string.Empty;
			return true;
		}

		private struct MapColorValuesJob : IJobParallelFor
		{
			public void Execute(int index)
			{
				int num = index / this.settings.Resolution;
				int num2 = index % this.settings.Resolution;
				int num3 = num % this.settings.SlicesPerRow;
				int num4 = (int)Mathf.Floor((float)(num / this.settings.SlicesPerRow));
				int num5 = num2 + num4 * this.settings.Resolution;
				int num6 = this.settings.FlipY ? (this.settings.Height - num5 - 1) : num5;
				int num7 = (num3 * this.settings.Resolution + num6 * this.settings.Width) * this.settings.ChannelCount;
				int num8 = (num * this.settings.Resolution * this.settings.Resolution + num2 * this.settings.Resolution) * this.settings.ChannelCount;
				for (int i = 0; i < this.settings.Resolution * this.settings.ChannelCount; i++)
				{
					this.target[num8 + i] = this.source[num7 + i];
				}
			}

			public OVRPassthroughColorLut.ColorLutTextureConverter.TextureSettings settings;

			[NativeDisableParallelForRestriction]
			[WriteOnly]
			public NativeArray<byte> target;

			[NativeDisableParallelForRestriction]
			[ReadOnly]
			public NativeArray<byte> source;
		}

		private struct TextureSettings
		{
			public readonly int Width { get; }

			public readonly int Height { get; }

			public readonly int Resolution { get; }

			public readonly int SlicesPerRow { get; }

			public readonly int ChannelCount { get; }

			public readonly bool FlipY { get; }

			public TextureSettings(int width, int height, int resolution, int slicesPerRow, int channelCount, bool flipY)
			{
				this.Width = width;
				this.Height = height;
				this.Resolution = resolution;
				this.SlicesPerRow = slicesPerRow;
				this.ChannelCount = channelCount;
				this.FlipY = flipY;
			}
		}
	}

	private enum CreateState
	{
		Invalid,
		Pending,
		Created
	}
}
