using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeAsStruct]
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class TreePrototype
	{
		public GameObject prefab
		{
			get
			{
				return this.m_Prefab;
			}
			set
			{
				this.m_Prefab = value;
			}
		}

		public float bendFactor
		{
			get
			{
				return this.m_BendFactor;
			}
			set
			{
				this.m_BendFactor = value;
			}
		}

		public int navMeshLod
		{
			get
			{
				return this.m_NavMeshLod;
			}
			set
			{
				this.m_NavMeshLod = value;
			}
		}

		public TreePrototype()
		{
		}

		public TreePrototype(TreePrototype other)
		{
			this.prefab = other.prefab;
			this.bendFactor = other.bendFactor;
			this.navMeshLod = other.navMeshLod;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as TreePrototype);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private bool Equals(TreePrototype other)
		{
			bool flag = other == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = other == this;
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = base.GetType() != other.GetType();
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = this.prefab == other.prefab && this.bendFactor == other.bendFactor && this.navMeshLod == other.navMeshLod;
						result = flag4;
					}
				}
			}
			return result;
		}

		internal bool Validate(out string errorMessage)
		{
			return TreePrototype.ValidateTreePrototype(this, out errorMessage);
		}

		[FreeFunction("TerrainDataScriptingInterface::ValidateTreePrototype")]
		internal static bool ValidateTreePrototype([NotNull] TreePrototype prototype, out string errorMessage)
		{
			if (prototype == null)
			{
				ThrowHelper.ThrowArgumentNullException(prototype, "prototype");
			}
			bool result;
			try
			{
				ManagedSpanWrapper managedSpan;
				result = TreePrototype.ValidateTreePrototype_Injected(prototype, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				errorMessage = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ValidateTreePrototype_Injected(TreePrototype prototype, out ManagedSpanWrapper errorMessage);

		[NativeName("prefab")]
		internal GameObject m_Prefab;

		[NativeName("bendFactor")]
		internal float m_BendFactor;

		[NativeName("navMeshLod")]
		internal int m_NavMeshLod;
	}
}
