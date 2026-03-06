using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace XNode
{
	public static class NodeDataCache
	{
		private static bool Initialized
		{
			get
			{
				return NodeDataCache.portDataCache != null;
			}
		}

		public static string GetTypeQualifiedName(Type type)
		{
			if (NodeDataCache.typeQualifiedNameCache == null)
			{
				NodeDataCache.typeQualifiedNameCache = new Dictionary<Type, string>();
			}
			string assemblyQualifiedName;
			if (!NodeDataCache.typeQualifiedNameCache.TryGetValue(type, out assemblyQualifiedName))
			{
				assemblyQualifiedName = type.AssemblyQualifiedName;
				NodeDataCache.typeQualifiedNameCache.Add(type, assemblyQualifiedName);
			}
			return assemblyQualifiedName;
		}

		public static void UpdatePorts(Node node, Dictionary<string, NodePort> ports)
		{
			if (!NodeDataCache.Initialized)
			{
				NodeDataCache.BuildCache();
			}
			Dictionary<string, List<NodePort>> dictionary = new Dictionary<string, List<NodePort>>();
			Type type = node.GetType();
			Dictionary<string, string> dictionary2 = null;
			if (NodeDataCache.formerlySerializedAsCache != null)
			{
				NodeDataCache.formerlySerializedAsCache.TryGetValue(type, out dictionary2);
			}
			List<NodePort> list = new List<NodePort>();
			Dictionary<string, NodePort> dictionary3;
			if (!NodeDataCache.portDataCache.TryGetValue(type, out dictionary3))
			{
				dictionary3 = new Dictionary<string, NodePort>();
			}
			foreach (NodePort nodePort in ports.Values.ToArray<NodePort>())
			{
				NodePort nodePort2;
				if (dictionary3.TryGetValue(nodePort.fieldName, out nodePort2))
				{
					if (nodePort.IsDynamic || nodePort.direction != nodePort2.direction || nodePort.connectionType != nodePort2.connectionType || nodePort.typeConstraint != nodePort2.typeConstraint)
					{
						if (!nodePort.IsDynamic && nodePort.direction == nodePort2.direction)
						{
							dictionary.Add(nodePort.fieldName, nodePort.GetConnections());
						}
						nodePort.ClearConnections();
						ports.Remove(nodePort.fieldName);
					}
					else
					{
						nodePort.ValueType = nodePort2.ValueType;
					}
				}
				else if (nodePort.IsStatic)
				{
					string key = null;
					if (dictionary2 != null && dictionary2.TryGetValue(nodePort.fieldName, out key))
					{
						dictionary.Add(key, nodePort.GetConnections());
					}
					nodePort.ClearConnections();
					ports.Remove(nodePort.fieldName);
				}
				else if (NodeDataCache.IsDynamicListPort(nodePort))
				{
					list.Add(nodePort);
				}
			}
			foreach (NodePort nodePort3 in dictionary3.Values)
			{
				if (!ports.ContainsKey(nodePort3.fieldName))
				{
					NodePort nodePort4 = new NodePort(nodePort3, node);
					List<NodePort> list2;
					if (dictionary.TryGetValue(nodePort3.fieldName, out list2))
					{
						for (int j = 0; j < list2.Count; j++)
						{
							NodePort nodePort5 = list2[j];
							if (nodePort5 != null && nodePort4.CanConnectTo(nodePort5))
							{
								nodePort4.Connect(nodePort5);
							}
						}
					}
					ports.Add(nodePort3.fieldName, nodePort4);
				}
			}
			foreach (NodePort nodePort6 in list)
			{
				string key2 = nodePort6.fieldName.Substring(0, nodePort6.fieldName.IndexOf(' '));
				NodePort nodePort7 = dictionary3[key2];
				nodePort6.ValueType = NodeDataCache.GetBackingValueType(nodePort7.ValueType);
				nodePort6.direction = nodePort7.direction;
				nodePort6.connectionType = nodePort7.connectionType;
				nodePort6.typeConstraint = nodePort7.typeConstraint;
			}
		}

		private static Type GetBackingValueType(Type portValType)
		{
			if (portValType.HasElementType)
			{
				return portValType.GetElementType();
			}
			if (portValType.IsGenericType && portValType.GetGenericTypeDefinition() == typeof(List<>))
			{
				return portValType.GetGenericArguments()[0];
			}
			return portValType;
		}

		private static bool IsDynamicListPort(NodePort port)
		{
			string[] array = port.fieldName.Split(' ', StringSplitOptions.None);
			if (array.Length != 2)
			{
				return false;
			}
			FieldInfo field = port.node.GetType().GetField(array[0]);
			if (field == null)
			{
				return false;
			}
			return field.GetCustomAttributes(true).Any(delegate(object x)
			{
				Node.InputAttribute inputAttribute = x as Node.InputAttribute;
				Node.OutputAttribute outputAttribute = x as Node.OutputAttribute;
				return (inputAttribute != null && inputAttribute.dynamicPortList) || (outputAttribute != null && outputAttribute.dynamicPortList);
			});
		}

		private static void BuildCache()
		{
			NodeDataCache.portDataCache = new NodeDataCache.PortDataCache();
			Type baseType = typeof(Node);
			List<Type> list = new List<Type>();
			Func<Type, bool> <>9__0;
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				string text = assembly.GetName().Name;
				int num = text.IndexOf('.');
				if (num != -1)
				{
					text = text.Substring(0, num);
				}
				if (!(text == "UnityEditor") && !(text == "UnityEngine") && !(text == "Unity") && !(text == "System") && !(text == "mscorlib") && !(text == "Microsoft"))
				{
					List<Type> list2 = list;
					IEnumerable<Type> types = assembly.GetTypes();
					Func<Type, bool> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = ((Type t) => !t.IsAbstract && baseType.IsAssignableFrom(t)));
					}
					list2.AddRange(types.Where(predicate).ToArray<Type>());
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				NodeDataCache.CachePorts(list[j]);
			}
		}

		public static List<FieldInfo> GetNodeFields(Type nodeType)
		{
			List<FieldInfo> list = new List<FieldInfo>(nodeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			Type type = nodeType;
			while ((type = type.BaseType) != typeof(Node))
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
				for (int i = 0; i < fields.Length; i++)
				{
					FieldInfo parentField = fields[i];
					if (list.TrueForAll((FieldInfo x) => x.Name != parentField.Name))
					{
						list.Add(parentField);
					}
				}
			}
			return list;
		}

		private static void CachePorts(Type nodeType)
		{
			List<FieldInfo> nodeFields = NodeDataCache.GetNodeFields(nodeType);
			for (int i = 0; i < nodeFields.Count; i++)
			{
				object[] customAttributes = nodeFields[i].GetCustomAttributes(true);
				Node.InputAttribute inputAttribute = customAttributes.FirstOrDefault((object x) => x is Node.InputAttribute) as Node.InputAttribute;
				Node.OutputAttribute outputAttribute = customAttributes.FirstOrDefault((object x) => x is Node.OutputAttribute) as Node.OutputAttribute;
				FormerlySerializedAsAttribute formerlySerializedAsAttribute = customAttributes.FirstOrDefault((object x) => x is FormerlySerializedAsAttribute) as FormerlySerializedAsAttribute;
				if (inputAttribute != null || outputAttribute != null)
				{
					if (inputAttribute != null && outputAttribute != null)
					{
						Debug.LogError(string.Concat(new string[]
						{
							"Field ",
							nodeFields[i].Name,
							" of type ",
							nodeType.FullName,
							" cannot be both input and output."
						}));
					}
					else
					{
						if (!NodeDataCache.portDataCache.ContainsKey(nodeType))
						{
							NodeDataCache.portDataCache.Add(nodeType, new Dictionary<string, NodePort>());
						}
						NodePort nodePort = new NodePort(nodeFields[i]);
						NodeDataCache.portDataCache[nodeType].Add(nodePort.fieldName, nodePort);
					}
					if (formerlySerializedAsAttribute != null)
					{
						if (NodeDataCache.formerlySerializedAsCache == null)
						{
							NodeDataCache.formerlySerializedAsCache = new Dictionary<Type, Dictionary<string, string>>();
						}
						if (!NodeDataCache.formerlySerializedAsCache.ContainsKey(nodeType))
						{
							NodeDataCache.formerlySerializedAsCache.Add(nodeType, new Dictionary<string, string>());
						}
						if (NodeDataCache.formerlySerializedAsCache[nodeType].ContainsKey(formerlySerializedAsAttribute.oldName))
						{
							Debug.LogError("Another FormerlySerializedAs with value '" + formerlySerializedAsAttribute.oldName + "' already exist on this node.");
						}
						else
						{
							NodeDataCache.formerlySerializedAsCache[nodeType].Add(formerlySerializedAsAttribute.oldName, nodeFields[i].Name);
						}
					}
				}
			}
		}

		private static NodeDataCache.PortDataCache portDataCache;

		private static Dictionary<Type, Dictionary<string, string>> formerlySerializedAsCache;

		private static Dictionary<Type, string> typeQualifiedNameCache;

		[Serializable]
		private class PortDataCache : Dictionary<Type, Dictionary<string, NodePort>>
		{
		}
	}
}
