using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.AddressableAssets.ResourceLocators
{
	[Serializable]
	public class ContentCatalogData
	{
		public string BuildResultHash
		{
			get
			{
				return this.m_BuildResultHash;
			}
			set
			{
				this.m_BuildResultHash = value;
			}
		}

		public string ProviderId
		{
			get
			{
				return this.m_LocatorId;
			}
			internal set
			{
				this.m_LocatorId = value;
			}
		}

		public ObjectInitializationData InstanceProviderData
		{
			get
			{
				return this.m_InstanceProviderData;
			}
			set
			{
				this.m_InstanceProviderData = value;
			}
		}

		public ObjectInitializationData SceneProviderData
		{
			get
			{
				return this.m_SceneProviderData;
			}
			set
			{
				this.m_SceneProviderData = value;
			}
		}

		public List<ObjectInitializationData> ResourceProviderData
		{
			get
			{
				return this.m_ResourceProviderData;
			}
			set
			{
				this.m_ResourceProviderData = value;
			}
		}

		public ContentCatalogData(string id)
		{
			this.m_LocatorId = id;
		}

		public ContentCatalogData()
		{
		}

		internal void CleanData()
		{
			this.m_LocatorId = null;
			this.m_Reader = null;
		}

		internal void CopyToFile(string path)
		{
			byte[] buffer = this.m_Reader.GetBuffer();
			File.WriteAllBytes(path, buffer);
		}

		internal ContentCatalogData(BinaryStorageBuffer.Reader reader)
		{
			this.m_Reader = reader;
		}

		internal byte[] GetBytes()
		{
			return this.m_Reader.GetBuffer();
		}

		internal IResourceLocator CreateCustomLocator(string overrideId = "", string providerSuffix = null)
		{
			this.m_LocatorId = overrideId;
			return new ContentCatalogData.ResourceLocator(this.m_LocatorId, this.m_Reader, providerSuffix);
		}

		internal static ContentCatalogData LoadFromFile(string path, bool resolveInternalIds)
		{
			return new ContentCatalogData(new BinaryStorageBuffer.Reader(File.ReadAllBytes(path), 0, 0U, new BinaryStorageBuffer.ISerializationAdapter[]
			{
				resolveInternalIds ? new ContentCatalogData.Serializer() : new ContentCatalogData.Serializer().WithInternalIdResolvingDisabled()
			}));
		}

		private static int kMagic = "ContentCatalogData".GetHashCode();

		private const int kVersion = 2;

		[NonSerialized]
		public string LocalHash;

		[NonSerialized]
		internal IResourceLocation location;

		[SerializeField]
		internal string m_LocatorId;

		[SerializeField]
		internal string m_BuildResultHash;

		[SerializeField]
		private ObjectInitializationData m_InstanceProviderData;

		[SerializeField]
		private ObjectInitializationData m_SceneProviderData;

		[SerializeField]
		internal List<ObjectInitializationData> m_ResourceProviderData = new List<ObjectInitializationData>();

		private IList<ContentCatalogDataEntry> m_Entries;

		private BinaryStorageBuffer.Reader m_Reader;

		internal class Serializer : BinaryStorageBuffer.ISerializationAdapter<ContentCatalogData>, BinaryStorageBuffer.ISerializationAdapter
		{
			public IEnumerable<BinaryStorageBuffer.ISerializationAdapter> Dependencies
			{
				get
				{
					return new BinaryStorageBuffer.ISerializationAdapter[]
					{
						new ObjectInitializationData.Serializer(),
						new ContentCatalogData.AssetBundleRequestOptionsSerializationAdapter(),
						new ContentCatalogData.ResourceLocator.ResourceLocation.Serializer(this.resolveInternalIds)
					};
				}
			}

			public ContentCatalogData.Serializer WithInternalIdResolvingDisabled()
			{
				this.resolveInternalIds = false;
				return this;
			}

			public object Deserialize(BinaryStorageBuffer.Reader reader, Type t, uint offset, out uint size)
			{
				ContentCatalogData contentCatalogData = new ContentCatalogData(reader);
				uint num;
				ContentCatalogData.ResourceLocator.Header header = reader.ReadValue<ContentCatalogData.ResourceLocator.Header>(offset, out num);
				if (header.magic != ContentCatalogData.kMagic)
				{
					throw new Exception("Invalid header data!!!");
				}
				if (header.version != 2)
				{
					throw new Exception(string.Format("Expected catalog data version {0}, but file was written with version {1}.", 2, header.version));
				}
				uint num2;
				contentCatalogData.InstanceProviderData = reader.ReadObject<ObjectInitializationData>(header.instanceProvider, out num2, false);
				uint num3;
				contentCatalogData.SceneProviderData = reader.ReadObject<ObjectInitializationData>(header.sceneProvider, out num3, false);
				uint num4;
				contentCatalogData.ResourceProviderData = reader.ReadObjectArray<ObjectInitializationData>(header.initObjectsArray, out num4, false, false).ToList<ObjectInitializationData>();
				uint num5;
				contentCatalogData.BuildResultHash = reader.ReadString(header.buildResultHash, out num5, '\0', true);
				size = num + num2 + num3 + num4 + num5;
				return contentCatalogData;
			}

			public uint Serialize(BinaryStorageBuffer.Writer writer, object val)
			{
				ContentCatalogData contentCatalogData = val as ContentCatalogData;
				IList<ContentCatalogDataEntry> entries = contentCatalogData.m_Entries;
				Dictionary<object, List<int>> dictionary = new Dictionary<object, List<int>>();
				for (int k = 0; k < entries.Count; k++)
				{
					foreach (object key in entries[k].Keys)
					{
						List<int> list;
						if (!dictionary.TryGetValue(key, out list))
						{
							dictionary.Add(key, list = new List<int>());
						}
						list.Add(k);
					}
				}
				uint num = writer.Reserve<ContentCatalogData.ResourceLocator.Header>();
				uint num2 = writer.Reserve<ContentCatalogData.ResourceLocator.KeyData>((uint)dictionary.Count);
				ContentCatalogData.ResourceLocator.Header header = new ContentCatalogData.ResourceLocator.Header
				{
					magic = ContentCatalogData.kMagic,
					version = 2,
					keysOffset = num2,
					idOffset = writer.WriteString(contentCatalogData.ProviderId, '\0'),
					instanceProvider = writer.WriteObject(contentCatalogData.InstanceProviderData, false),
					sceneProvider = writer.WriteObject(contentCatalogData.SceneProviderData, false),
					initObjectsArray = writer.WriteObjects<ObjectInitializationData>(contentCatalogData.m_ResourceProviderData, false),
					buildResultHash = writer.WriteString(contentCatalogData.BuildResultHash, '\0')
				};
				writer.Write<ContentCatalogData.ResourceLocator.Header>(num, header);
				uint[] locationIds = new uint[entries.Count];
				for (int j = 0; j < entries.Count; j++)
				{
					locationIds[j] = writer.WriteObject(new ContentCatalogData.ResourceLocator.ContentCatalogDataEntrySerializationContext
					{
						entry = entries[j],
						allEntries = entries,
						keyToEntryIndices = dictionary
					}, false);
				}
				int num3 = 0;
				ContentCatalogData.ResourceLocator.KeyData[] array = new ContentCatalogData.ResourceLocator.KeyData[dictionary.Count];
				Func<int, uint> <>9__0;
				foreach (KeyValuePair<object, List<int>> keyValuePair in dictionary)
				{
					IEnumerable<int> value = keyValuePair.Value;
					Func<int, uint> selector;
					if ((selector = <>9__0) == null)
					{
						selector = (<>9__0 = ((int i) => locationIds[i]));
					}
					uint[] values = value.Select(selector).ToArray<uint>();
					array[num3++] = new ContentCatalogData.ResourceLocator.KeyData
					{
						keyNameOffset = writer.WriteObject(keyValuePair.Key, true),
						locationSetOffset = writer.Write<uint>(values, true)
					};
				}
				writer.Write<ContentCatalogData.ResourceLocator.KeyData>(num2, array, true);
				return num;
			}

			private bool resolveInternalIds = true;
		}

		internal class ResourceLocator : IResourceLocator
		{
			public string LocatorId { get; private set; }

			public IEnumerable<object> Keys
			{
				get
				{
					return this.keyData.Keys;
				}
			}

			public IEnumerable<IResourceLocation> AllLocations
			{
				get
				{
					HashSet<IResourceLocation> hashSet = new HashSet<IResourceLocation>(new ResourceLocationComparer());
					foreach (KeyValuePair<object, uint> keyValuePair in this.keyData)
					{
						IList<IResourceLocation> list;
						if (this.Locate(keyValuePair.Key, null, out list))
						{
							foreach (IResourceLocation item in list)
							{
								hashSet.Add(item);
							}
						}
					}
					return hashSet;
				}
			}

			internal ResourceLocator(string id, BinaryStorageBuffer.Reader reader, string providerSuffix)
			{
				this.LocatorId = id;
				this.providerSuffix = providerSuffix;
				this.reader = reader;
				this.keyData = new Dictionary<object, uint>();
				uint num;
				ContentCatalogData.ResourceLocator.Header header = reader.ReadValue<ContentCatalogData.ResourceLocator.Header>(0U, out num);
				ContentCatalogData.ResourceLocator.KeyData[] array = reader.ReadValueArray<ContentCatalogData.ResourceLocator.KeyData>(header.keysOffset, out num, false);
				int num2 = 0;
				foreach (ContentCatalogData.ResourceLocator.KeyData keyData in array)
				{
					object key = reader.ReadObject(keyData.keyNameOffset, out num, true);
					this.keyData.Add(key, keyData.locationSetOffset);
					num2++;
				}
				reader.ResetCache(this.keyData.Count * 3, 0U);
			}

			private static void ProcFunc(ContentCatalogData.ResourceLocator.ResourceLocation loc, ContentCatalogData.ResourceLocator.LocateProcContext context, int i, int count)
			{
				if (context.type == null || context.type == typeof(object) || context.type.IsAssignableFrom(loc.ResourceType))
				{
					if (context.locations == null)
					{
						context.locations = new List<IResourceLocation>(count);
					}
					context.locations.Add(loc);
				}
			}

			public bool Locate(object key, Type type, out IList<IResourceLocation> locations)
			{
				uint id;
				if (!this.keyData.TryGetValue(key, out id))
				{
					locations = null;
					return false;
				}
				this.sharedContext.type = type;
				uint num;
				this.reader.ProcessObjectArray<ContentCatalogData.ResourceLocator.ResourceLocation, ContentCatalogData.ResourceLocator.LocateProcContext>(id, out num, this.sharedContext, new Action<ContentCatalogData.ResourceLocator.ResourceLocation, ContentCatalogData.ResourceLocator.LocateProcContext, int, int>(ContentCatalogData.ResourceLocator.ProcFunc), true);
				locations = this.sharedContext.locations;
				this.sharedContext.locations = null;
				this.sharedContext.type = null;
				if (this.providerSuffix != null && locations != null)
				{
					foreach (IResourceLocation resourceLocation in locations)
					{
						if (!resourceLocation.ProviderId.EndsWith(this.providerSuffix))
						{
							(resourceLocation as ContentCatalogData.ResourceLocator.ResourceLocation).ProviderId = resourceLocation.ProviderId + this.providerSuffix;
						}
					}
				}
				return locations != null;
			}

			private Dictionary<object, uint> keyData;

			private BinaryStorageBuffer.Reader reader;

			private string providerSuffix;

			private ContentCatalogData.ResourceLocator.LocateProcContext sharedContext = new ContentCatalogData.ResourceLocator.LocateProcContext();

			public struct Header
			{
				public int magic;

				public int version;

				public uint keysOffset;

				public uint idOffset;

				public uint instanceProvider;

				public uint sceneProvider;

				public uint initObjectsArray;

				public uint buildResultHash;
			}

			public struct KeyData
			{
				public uint keyNameOffset;

				public uint locationSetOffset;
			}

			internal class ContentCatalogDataEntrySerializationContext
			{
				public ContentCatalogDataEntry entry;

				public Dictionary<object, List<int>> keyToEntryIndices;

				public IList<ContentCatalogDataEntry> allEntries;
			}

			internal class ResourceLocation : IResourceLocation
			{
				public ResourceLocation(BinaryStorageBuffer.Reader r, uint id, out uint size, bool resolveInternalId)
				{
					this.reader = r;
					uint num;
					ContentCatalogData.ResourceLocator.ResourceLocation.Serializer.Data data = this.reader.ReadValue<ContentCatalogData.ResourceLocator.ResourceLocation.Serializer.Data>(id, out num);
					size = num;
					uint num2;
					this.ProviderId = this.reader.ReadString(data.providerOffset, out num2, '.', true);
					size += num2;
					uint num3;
					this.PrimaryKey = this.reader.ReadString(data.primaryKeyOffset, out num3, '/', true);
					size += num3;
					uint num4;
					this.Data = this.reader.ReadObject(data.extraDataOffset, out num4, true);
					size += num4;
					if (resolveInternalId)
					{
						uint num5;
						this.InternalId = this.reader.ReadObject<ContentCatalogData.ResourceLocator.ResourceLocation.ResolvedInternalId>(data.internalIdOffset, out num5, true).InternalId;
						size += num5;
					}
					else
					{
						uint num6;
						this.InternalId = this.reader.ReadString(data.internalIdOffset, out num6, '/', true);
						size += num6;
					}
					this.dependencyDataOffset = data.dependencySetOffset;
					uint num7;
					this.ResourceType = this.reader.ReadObject<Type>(data.typeId, out num7, true);
					size += num7;
				}

				public string InternalId { get; internal set; }

				public string ProviderId { get; internal set; }

				private static void ProcDependencies(ContentCatalogData.ResourceLocator.ResourceLocation l, ContentCatalogData.ResourceLocator.ResourceLocation d, int i, int count)
				{
					if (d._deps == null)
					{
						d._deps = new List<IResourceLocation>(count);
					}
					d._deps.Add(l);
				}

				public IList<IResourceLocation> Dependencies
				{
					get
					{
						if (this._deps == null)
						{
							this._deps = new List<IResourceLocation>();
							uint num;
							this.reader.ProcessObjectArray<ContentCatalogData.ResourceLocator.ResourceLocation, ContentCatalogData.ResourceLocator.ResourceLocation>(this.dependencyDataOffset, out num, this, new Action<ContentCatalogData.ResourceLocator.ResourceLocation, ContentCatalogData.ResourceLocator.ResourceLocation, int, int>(ContentCatalogData.ResourceLocator.ResourceLocation.ProcDependencies), true);
						}
						return this._deps;
					}
				}

				public int DependencyHashCode
				{
					get
					{
						return this.dependencyDataOffset.GetHashCode();
					}
				}

				public bool HasDependencies
				{
					get
					{
						return this.dependencyDataOffset != uint.MaxValue;
					}
				}

				public object Data { get; internal set; }

				public string PrimaryKey { get; internal set; }

				public Type ResourceType { get; internal set; }

				public override string ToString()
				{
					return this.InternalId;
				}

				public int Hash(Type resultType)
				{
					return this.InternalId.GetHashCode() * 31 + this.ResourceType.GetHashCode();
				}

				private BinaryStorageBuffer.Reader reader;

				private List<IResourceLocation> _deps;

				private uint dependencyDataOffset;

				private class ResolvedInternalId
				{
					public string InternalId;
				}

				public class ResolvedInternalIdSerializer : BinaryStorageBuffer.ISerializationAdapter<ContentCatalogData.ResourceLocator.ResourceLocation.ResolvedInternalId>, BinaryStorageBuffer.ISerializationAdapter
				{
					IEnumerable<BinaryStorageBuffer.ISerializationAdapter> BinaryStorageBuffer.ISerializationAdapter.Dependencies
					{
						get
						{
							return null;
						}
					}

					object BinaryStorageBuffer.ISerializationAdapter.Deserialize(BinaryStorageBuffer.Reader reader, Type t, uint offset, out uint size)
					{
						string internalId = Addressables.ResolveInternalId(reader.ReadString(offset, out size, '/', true));
						return new ContentCatalogData.ResourceLocator.ResourceLocation.ResolvedInternalId
						{
							InternalId = internalId
						};
					}

					uint BinaryStorageBuffer.ISerializationAdapter.Serialize(BinaryStorageBuffer.Writer writer, object val)
					{
						throw new NotImplementedException();
					}
				}

				public class Serializer : BinaryStorageBuffer.ISerializationAdapter<ContentCatalogData.ResourceLocator.ResourceLocation>, BinaryStorageBuffer.ISerializationAdapter, BinaryStorageBuffer.ISerializationAdapter<ContentCatalogData.ResourceLocator.ContentCatalogDataEntrySerializationContext>
				{
					public IEnumerable<BinaryStorageBuffer.ISerializationAdapter> Dependencies
					{
						get
						{
							return new BinaryStorageBuffer.ISerializationAdapter[]
							{
								new ContentCatalogData.ResourceLocator.ResourceLocation.ResolvedInternalIdSerializer()
							};
						}
					}

					public Serializer(bool resolveInternalIds)
					{
						this.resolveInternalIds = resolveInternalIds;
					}

					public object Deserialize(BinaryStorageBuffer.Reader reader, Type t, uint offset, out uint size)
					{
						return new ContentCatalogData.ResourceLocator.ResourceLocation(reader, offset, ref size, this.resolveInternalIds);
					}

					public uint Serialize(BinaryStorageBuffer.Writer writer, object val)
					{
						ContentCatalogData.ResourceLocator.ContentCatalogDataEntrySerializationContext contentCatalogDataEntrySerializationContext = val as ContentCatalogData.ResourceLocator.ContentCatalogDataEntrySerializationContext;
						ContentCatalogDataEntry entry = contentCatalogDataEntrySerializationContext.entry;
						uint dependencySetOffset = uint.MaxValue;
						if (entry.Dependencies != null && entry.Dependencies.Count > 0)
						{
							HashSet<uint> hashSet = new HashSet<uint>();
							foreach (object key in entry.Dependencies)
							{
								foreach (int index in contentCatalogDataEntrySerializationContext.keyToEntryIndices[key])
								{
									hashSet.Add(writer.WriteObject(new ContentCatalogData.ResourceLocator.ContentCatalogDataEntrySerializationContext
									{
										entry = contentCatalogDataEntrySerializationContext.allEntries[index],
										allEntries = contentCatalogDataEntrySerializationContext.allEntries,
										keyToEntryIndices = contentCatalogDataEntrySerializationContext.keyToEntryIndices
									}, false));
								}
							}
							dependencySetOffset = writer.Write<uint>(hashSet.ToArray<uint>(), false);
						}
						ContentCatalogData.ResourceLocator.ResourceLocation.Serializer.Data val2 = new ContentCatalogData.ResourceLocator.ResourceLocation.Serializer.Data
						{
							primaryKeyOffset = writer.WriteString(entry.Keys[0] as string, '/'),
							internalIdOffset = writer.WriteString(entry.InternalId, '/'),
							providerOffset = writer.WriteString(entry.Provider, '.'),
							dependencySetOffset = dependencySetOffset,
							extraDataOffset = writer.WriteObject(entry.Data, true),
							typeId = writer.WriteObject(entry.ResourceType, false)
						};
						return writer.Write<ContentCatalogData.ResourceLocator.ResourceLocation.Serializer.Data>(val2);
					}

					private bool resolveInternalIds;

					public struct Data
					{
						public uint primaryKeyOffset;

						public uint internalIdOffset;

						public uint providerOffset;

						public uint dependencySetOffset;

						public int dependencyHashValue;

						public uint extraDataOffset;

						public uint typeId;
					}
				}
			}

			private class LocateProcContext
			{
				public IList<IResourceLocation> locations;

				public Type type;
			}
		}

		internal class AssetBundleRequestOptionsSerializationAdapter : BinaryStorageBuffer.ISerializationAdapter<AssetBundleRequestOptions>, BinaryStorageBuffer.ISerializationAdapter
		{
			public IEnumerable<BinaryStorageBuffer.ISerializationAdapter> Dependencies
			{
				get
				{
					return null;
				}
			}

			public object Deserialize(BinaryStorageBuffer.Reader reader, Type type, uint offset, out uint size)
			{
				size = 0U;
				if (type != typeof(AssetBundleRequestOptions))
				{
					return null;
				}
				uint num;
				ContentCatalogData.AssetBundleRequestOptionsSerializationAdapter.SerializedData serializedData = reader.ReadValue<ContentCatalogData.AssetBundleRequestOptionsSerializationAdapter.SerializedData>(offset, out num);
				uint num2;
				ContentCatalogData.AssetBundleRequestOptionsSerializationAdapter.SerializedData.Common common = reader.ReadValue<ContentCatalogData.AssetBundleRequestOptionsSerializationAdapter.SerializedData.Common>(serializedData.commonId, out num2);
				uint num3;
				string hash = reader.ReadValue<Hash128>(serializedData.hashId, out num3).ToString();
				uint num4;
				string bundleName = reader.ReadString(serializedData.bundleNameId, out num4, '_', true);
				AssetBundleRequestOptions assetBundleRequestOptions = new AssetBundleRequestOptions();
				assetBundleRequestOptions.Hash = hash;
				assetBundleRequestOptions.BundleName = bundleName;
				assetBundleRequestOptions.Crc = serializedData.crc;
				assetBundleRequestOptions.BundleSize = (long)((ulong)serializedData.bundleSize);
				assetBundleRequestOptions.Timeout = (int)common.timeout;
				assetBundleRequestOptions.RetryCount = (int)common.retryCount;
				assetBundleRequestOptions.RedirectLimit = (int)common.redirectLimit;
				assetBundleRequestOptions.AssetLoadMode = common.assetLoadMode;
				assetBundleRequestOptions.ChunkedTransfer = common.chunkedTransfer;
				assetBundleRequestOptions.UseUnityWebRequestForLocalBundles = common.useUnityWebRequestForLocalBundles;
				assetBundleRequestOptions.UseCrcForCachedBundle = common.useCrcForCachedBundle;
				assetBundleRequestOptions.ClearOtherCachedVersionsWhenLoaded = common.clearOtherCachedVersionsWhenLoaded;
				size = num + num2 + num3 + num4;
				return assetBundleRequestOptions;
			}

			public uint Serialize(BinaryStorageBuffer.Writer writer, object obj)
			{
				AssetBundleRequestOptions assetBundleRequestOptions = obj as AssetBundleRequestOptions;
				Hash128 val = Hash128.Parse(assetBundleRequestOptions.Hash);
				short timeout = (short)Mathf.Clamp(assetBundleRequestOptions.Timeout, 0, 32767);
				byte retryCount = (byte)Mathf.Clamp(assetBundleRequestOptions.RetryCount, 0, 128);
				byte redirectLimit = (assetBundleRequestOptions.RedirectLimit < 0) ? 32 : ((byte)Mathf.Clamp(assetBundleRequestOptions.RedirectLimit, 0, 128));
				ContentCatalogData.AssetBundleRequestOptionsSerializationAdapter.SerializedData val2 = new ContentCatalogData.AssetBundleRequestOptionsSerializationAdapter.SerializedData
				{
					hashId = writer.Write<Hash128>(val),
					bundleNameId = writer.WriteString(assetBundleRequestOptions.BundleName, '_'),
					crc = assetBundleRequestOptions.Crc,
					bundleSize = (uint)assetBundleRequestOptions.BundleSize,
					commonId = writer.Write<ContentCatalogData.AssetBundleRequestOptionsSerializationAdapter.SerializedData.Common>(new ContentCatalogData.AssetBundleRequestOptionsSerializationAdapter.SerializedData.Common
					{
						timeout = timeout,
						redirectLimit = redirectLimit,
						retryCount = retryCount,
						assetLoadMode = assetBundleRequestOptions.AssetLoadMode,
						chunkedTransfer = assetBundleRequestOptions.ChunkedTransfer,
						clearOtherCachedVersionsWhenLoaded = assetBundleRequestOptions.ClearOtherCachedVersionsWhenLoaded,
						useCrcForCachedBundle = assetBundleRequestOptions.UseCrcForCachedBundle,
						useUnityWebRequestForLocalBundles = assetBundleRequestOptions.UseUnityWebRequestForLocalBundles
					})
				};
				return writer.Write<ContentCatalogData.AssetBundleRequestOptionsSerializationAdapter.SerializedData>(val2);
			}

			private struct SerializedData
			{
				public uint hashId;

				public uint bundleNameId;

				public uint crc;

				public uint bundleSize;

				public uint commonId;

				public struct Common
				{
					public AssetLoadMode assetLoadMode
					{
						get
						{
							if ((this.flags & 1) != 1)
							{
								return AssetLoadMode.RequestedAssetAndDependencies;
							}
							return AssetLoadMode.AllPackedAssetsAndDependencies;
						}
						set
						{
							this.flags = ((this.flags & -2) | (int)value);
						}
					}

					public bool chunkedTransfer
					{
						get
						{
							return (this.flags & 2) == 2;
						}
						set
						{
							this.flags = ((this.flags & -3) | (value ? 2 : 0));
						}
					}

					public bool useCrcForCachedBundle
					{
						get
						{
							return (this.flags & 4) == 4;
						}
						set
						{
							this.flags = ((this.flags & -5) | (value ? 4 : 0));
						}
					}

					public bool useUnityWebRequestForLocalBundles
					{
						get
						{
							return (this.flags & 8) == 8;
						}
						set
						{
							this.flags = ((this.flags & -9) | (value ? 8 : 0));
						}
					}

					public bool clearOtherCachedVersionsWhenLoaded
					{
						get
						{
							return (this.flags & 16) == 16;
						}
						set
						{
							this.flags = ((this.flags & -17) | (value ? 16 : 0));
						}
					}

					public short timeout;

					public byte redirectLimit;

					public byte retryCount;

					public int flags;
				}
			}
		}
	}
}
