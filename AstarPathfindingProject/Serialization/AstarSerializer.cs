using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Pathfinding.Ionic.Zip;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding.Serialization
{
	public class AstarSerializer
	{
		private static StringBuilder GetStringBuilder()
		{
			AstarSerializer._stringBuilder.Length = 0;
			return AstarSerializer._stringBuilder;
		}

		public AstarSerializer(AstarData data, GameObject contextRoot) : this(data, SerializeSettings.Settings, contextRoot)
		{
		}

		public AstarSerializer(AstarData data, SerializeSettings settings, GameObject contextRoot)
		{
			this.data = data;
			this.contextRoot = contextRoot;
			this.settings = settings;
		}

		public void SetGraphIndexOffset(int offset)
		{
			this.graphIndexOffset = offset;
		}

		private void AddChecksum(byte[] bytes)
		{
			this.checksum = Checksum.GetChecksum(bytes, this.checksum);
		}

		private void AddEntry(string name, byte[] bytes)
		{
			this.zip.AddEntry(name, bytes);
		}

		public uint GetChecksum()
		{
			return this.checksum;
		}

		public void OpenSerialize()
		{
			this.zipStream = new MemoryStream();
			this.zip = new ZipFile();
			this.zip.AlternateEncoding = Encoding.UTF8;
			this.zip.AlternateEncodingUsage = ZipOption.Always;
			this.zip.ParallelDeflateThreshold = -1L;
			this.meta = new GraphMeta();
		}

		public byte[] CloseSerialize()
		{
			byte[] array = this.SerializeMeta();
			this.AddChecksum(array);
			this.AddEntry("meta.json", array);
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			foreach (ZipEntry zipEntry in this.zip.Entries)
			{
				zipEntry.AccessedTime = dateTime;
				zipEntry.CreationTime = dateTime;
				zipEntry.LastModified = dateTime;
				zipEntry.ModifiedTime = dateTime;
			}
			this.zip.Save(this.zipStream);
			this.zip.Dispose();
			array = this.zipStream.ToArray();
			this.zip = null;
			this.zipStream = null;
			return array;
		}

		public void SerializeGraphs(NavGraph[] _graphs)
		{
			if (this.graphs != null)
			{
				throw new InvalidOperationException("Cannot serialize graphs multiple times.");
			}
			this.graphs = _graphs;
			if (this.zip == null)
			{
				throw new NullReferenceException("You must not call CloseSerialize before a call to this function");
			}
			if (this.graphs == null)
			{
				this.graphs = new NavGraph[0];
			}
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					byte[] bytes = this.Serialize(this.graphs[i]);
					this.AddChecksum(bytes);
					this.AddEntry("graph" + i.ToString() + ".json", bytes);
				}
			}
		}

		private byte[] SerializeMeta()
		{
			if (this.graphs == null)
			{
				throw new Exception("No call to SerializeGraphs has been done");
			}
			this.meta.version = AstarPath.Version;
			this.meta.graphs = this.graphs.Length;
			this.meta.guids = new List<string>();
			this.meta.typeNames = new List<string>();
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					this.meta.guids.Add(this.graphs[i].guid.ToString());
					this.meta.typeNames.Add(this.graphs[i].GetType().FullName);
				}
				else
				{
					this.meta.guids.Add(null);
					this.meta.typeNames.Add(null);
				}
			}
			StringBuilder stringBuilder = AstarSerializer.GetStringBuilder();
			TinyJsonSerializer.Serialize(this.meta, stringBuilder);
			return this.encoding.GetBytes(stringBuilder.ToString());
		}

		public byte[] Serialize(NavGraph graph)
		{
			StringBuilder stringBuilder = AstarSerializer.GetStringBuilder();
			TinyJsonSerializer.Serialize(graph, stringBuilder);
			return this.encoding.GetBytes(stringBuilder.ToString());
		}

		[Obsolete("Not used anymore. You can safely remove the call to this function.")]
		public void SerializeNodes()
		{
		}

		private static int GetMaxNodeIndexInAllGraphs(NavGraph[] graphs)
		{
			int maxIndex = 0;
			Action<GraphNode> <>9__0;
			for (int i = 0; i < graphs.Length; i++)
			{
				if (graphs[i] != null)
				{
					NavGraph navGraph = graphs[i];
					Action<GraphNode> action;
					if ((action = <>9__0) == null)
					{
						action = (<>9__0 = delegate(GraphNode node)
						{
							maxIndex = Math.Max(node.NodeIndex, maxIndex);
							if (node.NodeIndex == -1)
							{
								Debug.LogError("Graph contains destroyed nodes. This is a bug.");
							}
						});
					}
					navGraph.GetNodes(action);
				}
			}
			return maxIndex;
		}

		private static byte[] SerializeNodeIndices(NavGraph[] graphs)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(memoryStream);
			int maxNodeIndexInAllGraphs = AstarSerializer.GetMaxNodeIndexInAllGraphs(graphs);
			writer.Write(maxNodeIndexInAllGraphs);
			int maxNodeIndex2 = 0;
			Action<GraphNode> <>9__0;
			for (int i = 0; i < graphs.Length; i++)
			{
				if (graphs[i] != null)
				{
					NavGraph navGraph = graphs[i];
					Action<GraphNode> action;
					if ((action = <>9__0) == null)
					{
						action = (<>9__0 = delegate(GraphNode node)
						{
							maxNodeIndex2 = Math.Max(node.NodeIndex, maxNodeIndex2);
							writer.Write(node.NodeIndex);
						});
					}
					navGraph.GetNodes(action);
				}
			}
			if (maxNodeIndex2 != maxNodeIndexInAllGraphs)
			{
				throw new Exception("Some graphs are not consistent in their GetNodes calls, sequential calls give different results.");
			}
			byte[] result = memoryStream.ToArray();
			writer.Close();
			return result;
		}

		private static byte[] SerializeGraphExtraInfo(NavGraph graph)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			GraphSerializationContext ctx = new GraphSerializationContext(binaryWriter);
			((IGraphInternals)graph).SerializeExtraInfo(ctx);
			byte[] result = memoryStream.ToArray();
			binaryWriter.Close();
			return result;
		}

		private static byte[] SerializeGraphNodeReferences(NavGraph graph)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			GraphSerializationContext ctx = new GraphSerializationContext(binaryWriter);
			graph.GetNodes(delegate(GraphNode node)
			{
				node.SerializeReferences(ctx);
			});
			binaryWriter.Close();
			return memoryStream.ToArray();
		}

		public void SerializeExtraInfo()
		{
			if (!this.settings.nodes)
			{
				return;
			}
			if (this.graphs == null)
			{
				throw new InvalidOperationException("Cannot serialize extra info with no serialized graphs (call SerializeGraphs first)");
			}
			byte[] bytes = AstarSerializer.SerializeNodeIndices(this.graphs);
			this.AddChecksum(bytes);
			this.AddEntry("graph_references.binary", bytes);
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					bytes = AstarSerializer.SerializeGraphExtraInfo(this.graphs[i]);
					this.AddChecksum(bytes);
					this.AddEntry("graph" + i.ToString() + "_extra.binary", bytes);
					bytes = AstarSerializer.SerializeGraphNodeReferences(this.graphs[i]);
					this.AddChecksum(bytes);
					this.AddEntry("graph" + i.ToString() + "_references.binary", bytes);
				}
			}
			bytes = this.SerializeNodeLinks();
			this.AddChecksum(bytes);
			this.AddEntry("node_link2.binary", bytes);
		}

		private byte[] SerializeNodeLinks()
		{
			MemoryStream memoryStream = new MemoryStream();
			NodeLink2.SerializeReferences(new GraphSerializationContext(new BinaryWriter(memoryStream)));
			return memoryStream.ToArray();
		}

		private ZipEntry GetEntry(string name)
		{
			return this.zip[name];
		}

		private bool ContainsEntry(string name)
		{
			return this.GetEntry(name) != null;
		}

		public bool OpenDeserialize(byte[] bytes)
		{
			this.zipStream = new MemoryStream();
			this.zipStream.Write(bytes, 0, bytes.Length);
			this.zipStream.Position = 0L;
			try
			{
				this.zip = ZipFile.Read(this.zipStream);
				this.zip.ParallelDeflateThreshold = -1L;
			}
			catch (Exception ex)
			{
				string str = "Caught exception when loading from zip\n";
				Exception ex2 = ex;
				Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
				this.zipStream.Dispose();
				return false;
			}
			if (this.ContainsEntry("meta.json"))
			{
				this.meta = this.DeserializeMeta(this.GetEntry("meta.json"));
			}
			else
			{
				if (!this.ContainsEntry("meta.binary"))
				{
					throw new Exception("No metadata found in serialized data.");
				}
				this.meta = this.DeserializeBinaryMeta(this.GetEntry("meta.binary"));
			}
			if (AstarSerializer.FullyDefinedVersion(this.meta.version) > AstarSerializer.FullyDefinedVersion(AstarPath.Version))
			{
				string[] array = new string[5];
				array[0] = "Trying to load data from a newer version of the A* Pathfinding Project\nCurrent version: ";
				int num = 1;
				Version version = AstarPath.Version;
				array[num] = ((version != null) ? version.ToString() : null);
				array[2] = " Data version: ";
				int num2 = 3;
				Version version2 = this.meta.version;
				array[num2] = ((version2 != null) ? version2.ToString() : null);
				array[4] = "\nThis is usually fine as the stored data is usually backwards and forwards compatible.\nHowever node data (not settings) can get corrupted between versions (even though I try my best to keep compatibility), so it is recommended to recalculate any caches (those for faster startup) and resave any files. Even if it seems to load fine, it might cause subtle bugs.\n";
				Debug.LogWarning(string.Concat(array));
			}
			return true;
		}

		private static Version FullyDefinedVersion(Version v)
		{
			return new Version(Mathf.Max(v.Major, 0), Mathf.Max(v.Minor, 0), Mathf.Max(v.Build, 0), Mathf.Max(v.Revision, 0));
		}

		public void CloseDeserialize()
		{
			this.zipStream.Dispose();
			this.zip.Dispose();
			this.zip = null;
			this.zipStream = null;
		}

		private NavGraph DeserializeGraph(int zipIndex, int graphIndex, Type[] availableGraphTypes)
		{
			Type graphType = this.meta.GetGraphType(zipIndex, availableGraphTypes);
			if (object.Equals(graphType, null))
			{
				return null;
			}
			NavGraph navGraph = this.data.CreateGraph(graphType);
			navGraph.graphIndex = (uint)graphIndex;
			string name = "graph" + zipIndex.ToString() + ".json";
			string name2 = "graph" + zipIndex.ToString() + ".binary";
			if (this.ContainsEntry(name))
			{
				TinyJsonDeserializer.Deserialize(AstarSerializer.GetString(this.GetEntry(name)), graphType, navGraph, this.contextRoot);
			}
			else
			{
				if (!this.ContainsEntry(name2))
				{
					throw new FileNotFoundException(string.Concat(new string[]
					{
						"Could not find data for graph ",
						zipIndex.ToString(),
						" in zip. Entry 'graph",
						zipIndex.ToString(),
						".json' does not exist"
					}));
				}
				GraphSerializationContext ctx = new GraphSerializationContext(AstarSerializer.GetBinaryReader(this.GetEntry(name2)), null, navGraph.graphIndex, this.meta);
				((IGraphInternals)navGraph).DeserializeSettingsCompatibility(ctx);
			}
			if (navGraph.guid.ToString() != this.meta.guids[zipIndex])
			{
				string str = "Guid in graph file not equal to guid defined in meta file. Have you edited the data manually?\n";
				Guid guid = navGraph.guid;
				throw new Exception(str + guid.ToString() + " != " + this.meta.guids[zipIndex]);
			}
			return navGraph;
		}

		public NavGraph[] DeserializeGraphs(Type[] availableGraphTypes)
		{
			List<NavGraph> list = new List<NavGraph>();
			this.graphIndexInZip = new Dictionary<NavGraph, int>();
			for (int i = 0; i < this.meta.graphs; i++)
			{
				int graphIndex = list.Count + this.graphIndexOffset;
				NavGraph navGraph = this.DeserializeGraph(i, graphIndex, availableGraphTypes);
				if (navGraph != null)
				{
					list.Add(navGraph);
					this.graphIndexInZip[navGraph] = i;
				}
			}
			this.graphs = list.ToArray();
			return this.graphs;
		}

		private bool DeserializeExtraInfo(NavGraph graph)
		{
			ZipEntry entry = this.GetEntry("graph" + this.graphIndexInZip[graph].ToString() + "_extra.binary");
			if (entry == null)
			{
				return false;
			}
			GraphSerializationContext ctx = new GraphSerializationContext(AstarSerializer.GetBinaryReader(entry), null, graph.graphIndex, this.meta);
			((IGraphInternals)graph).DeserializeExtraInfo(ctx);
			return true;
		}

		private bool AnyDestroyedNodesInGraphs()
		{
			bool result = false;
			Action<GraphNode> <>9__0;
			for (int i = 0; i < this.graphs.Length; i++)
			{
				NavGraph navGraph = this.graphs[i];
				Action<GraphNode> action;
				if ((action = <>9__0) == null)
				{
					action = (<>9__0 = delegate(GraphNode node)
					{
						if (node.Destroyed)
						{
							result = true;
						}
					});
				}
				navGraph.GetNodes(action);
			}
			return result;
		}

		private GraphNode[] DeserializeNodeReferenceMap()
		{
			ZipEntry entry = this.GetEntry("graph_references.binary");
			if (entry == null)
			{
				throw new Exception("Node references not found in the data. Was this loaded from an older version of the A* Pathfinding Project?");
			}
			BinaryReader reader = AstarSerializer.GetBinaryReader(entry);
			int num = reader.ReadInt32();
			GraphNode[] int2Node = new GraphNode[num + 1];
			try
			{
				Action<GraphNode> <>9__0;
				for (int i = 0; i < this.graphs.Length; i++)
				{
					NavGraph navGraph = this.graphs[i];
					Action<GraphNode> action;
					if ((action = <>9__0) == null)
					{
						action = (<>9__0 = delegate(GraphNode node)
						{
							int num2 = reader.ReadInt32();
							int2Node[num2] = node;
						});
					}
					navGraph.GetNodes(action);
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Some graph(s) has thrown an exception during GetNodes, or some graph(s) have deserialized more or fewer nodes than were serialized", innerException);
			}
			if (reader.BaseStream.Position != reader.BaseStream.Length)
			{
				throw new Exception((reader.BaseStream.Length / 4L).ToString() + " nodes were serialized, but only data for " + (reader.BaseStream.Position / 4L).ToString() + " nodes was found. The data looks corrupt.");
			}
			reader.Close();
			return int2Node;
		}

		private void DeserializeNodeReferences(NavGraph graph, GraphNode[] int2Node)
		{
			int num = this.graphIndexInZip[graph];
			ZipEntry entry = this.GetEntry("graph" + num.ToString() + "_references.binary");
			if (entry == null)
			{
				throw new Exception("Node references for graph " + num.ToString() + " not found in the data. Was this loaded from an older version of the A* Pathfinding Project?");
			}
			BinaryReader binaryReader = AstarSerializer.GetBinaryReader(entry);
			GraphSerializationContext ctx = new GraphSerializationContext(binaryReader, int2Node, graph.graphIndex, this.meta);
			graph.GetNodes(delegate(GraphNode node)
			{
				node.DeserializeReferences(ctx);
			});
		}

		public void DeserializeExtraInfo()
		{
			bool flag = false;
			for (int i = 0; i < this.graphs.Length; i++)
			{
				flag |= this.DeserializeExtraInfo(this.graphs[i]);
			}
			if (!flag)
			{
				return;
			}
			if (this.AnyDestroyedNodesInGraphs())
			{
				Debug.LogError("Graph contains destroyed nodes. This is a bug.");
			}
			GraphNode[] int2Node = this.DeserializeNodeReferenceMap();
			for (int j = 0; j < this.graphs.Length; j++)
			{
				this.DeserializeNodeReferences(this.graphs[j], int2Node);
			}
			this.DeserializeNodeLinks(int2Node);
		}

		private void DeserializeNodeLinks(GraphNode[] int2Node)
		{
			ZipEntry entry = this.GetEntry("node_link2.binary");
			if (entry == null)
			{
				return;
			}
			NodeLink2.DeserializeReferences(new GraphSerializationContext(AstarSerializer.GetBinaryReader(entry), int2Node, 0U, this.meta));
		}

		public void PostDeserialization()
		{
			for (int i = 0; i < this.graphs.Length; i++)
			{
				GraphSerializationContext ctx = new GraphSerializationContext(null, null, 0U, this.meta);
				((IGraphInternals)this.graphs[i]).PostDeserialization(ctx);
			}
		}

		public void DeserializeEditorSettingsCompatibility()
		{
			for (int i = 0; i < this.graphs.Length; i++)
			{
				ZipEntry entry = this.GetEntry("graph" + this.graphIndexInZip[this.graphs[i]].ToString() + "_editor.json");
				if (entry != null)
				{
					((IGraphInternals)this.graphs[i]).SerializedEditorSettings = AstarSerializer.GetString(entry);
				}
			}
		}

		private static BinaryReader GetBinaryReader(ZipEntry entry)
		{
			MemoryStream memoryStream = new MemoryStream();
			entry.Extract(memoryStream);
			memoryStream.Position = 0L;
			return new BinaryReader(memoryStream);
		}

		private static string GetString(ZipEntry entry)
		{
			MemoryStream memoryStream = new MemoryStream();
			entry.Extract(memoryStream);
			memoryStream.Position = 0L;
			StreamReader streamReader = new StreamReader(memoryStream);
			string result = streamReader.ReadToEnd();
			streamReader.Dispose();
			return result;
		}

		private GraphMeta DeserializeMeta(ZipEntry entry)
		{
			return TinyJsonDeserializer.Deserialize(AstarSerializer.GetString(entry), typeof(GraphMeta), null, null) as GraphMeta;
		}

		private GraphMeta DeserializeBinaryMeta(ZipEntry entry)
		{
			GraphMeta graphMeta = new GraphMeta();
			BinaryReader binaryReader = AstarSerializer.GetBinaryReader(entry);
			if (binaryReader.ReadString() != "A*")
			{
				throw new Exception("Invalid magic number in saved data");
			}
			int num = binaryReader.ReadInt32();
			int num2 = binaryReader.ReadInt32();
			int num3 = binaryReader.ReadInt32();
			int num4 = binaryReader.ReadInt32();
			if (num < 0)
			{
				graphMeta.version = new Version(0, 0);
			}
			else if (num2 < 0)
			{
				graphMeta.version = new Version(num, 0);
			}
			else if (num3 < 0)
			{
				graphMeta.version = new Version(num, num2);
			}
			else if (num4 < 0)
			{
				graphMeta.version = new Version(num, num2, num3);
			}
			else
			{
				graphMeta.version = new Version(num, num2, num3, num4);
			}
			graphMeta.graphs = binaryReader.ReadInt32();
			graphMeta.guids = new List<string>();
			int num5 = binaryReader.ReadInt32();
			for (int i = 0; i < num5; i++)
			{
				graphMeta.guids.Add(binaryReader.ReadString());
			}
			graphMeta.typeNames = new List<string>();
			num5 = binaryReader.ReadInt32();
			for (int j = 0; j < num5; j++)
			{
				graphMeta.typeNames.Add(binaryReader.ReadString());
			}
			binaryReader.Close();
			return graphMeta;
		}

		public static void SaveToFile(string path, byte[] data)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Create))
			{
				fileStream.Write(data, 0, data.Length);
			}
		}

		public static byte[] LoadFromFile(string path)
		{
			byte[] result;
			using (FileStream fileStream = new FileStream(path, FileMode.Open))
			{
				byte[] array = new byte[(int)fileStream.Length];
				fileStream.Read(array, 0, (int)fileStream.Length);
				result = array;
			}
			return result;
		}

		private AstarData data;

		private ZipFile zip;

		private MemoryStream zipStream;

		private GraphMeta meta;

		private SerializeSettings settings;

		private GameObject contextRoot;

		private NavGraph[] graphs;

		private Dictionary<NavGraph, int> graphIndexInZip;

		private int graphIndexOffset;

		private const string binaryExt = ".binary";

		private const string jsonExt = ".json";

		private uint checksum = uint.MaxValue;

		private UTF8Encoding encoding = new UTF8Encoding();

		private static StringBuilder _stringBuilder = new StringBuilder();

		public static readonly Version V3_8_3 = new Version(3, 8, 3);

		public static readonly Version V3_9_0 = new Version(3, 9, 0);

		public static readonly Version V4_1_0 = new Version(4, 1, 0);
	}
}
