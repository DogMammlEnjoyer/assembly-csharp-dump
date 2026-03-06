using System;

namespace System.ComponentModel.Design.Serialization
{
	/// <summary>Indicates the base serializer to use for a root designer object. This class cannot be inherited.</summary>
	[Obsolete("This attribute has been deprecated. Use DesignerSerializerAttribute instead.  For example, to specify a root designer for CodeDom, use DesignerSerializerAttribute(...,typeof(TypeCodeDomSerializer)).  https://go.microsoft.com/fwlink/?linkid=14202")]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	public sealed class RootDesignerSerializerAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.Serialization.RootDesignerSerializerAttribute" /> class using the specified attributes.</summary>
		/// <param name="serializerType">The data type of the serializer.</param>
		/// <param name="baseSerializerType">The base type of the serializer. A class can include multiple serializers as they all have different base types.</param>
		/// <param name="reloadable">
		///   <see langword="true" /> if this serializer supports dynamic reloading of the document; otherwise, <see langword="false" />.</param>
		public RootDesignerSerializerAttribute(Type serializerType, Type baseSerializerType, bool reloadable)
		{
			this.SerializerTypeName = serializerType.AssemblyQualifiedName;
			this.SerializerBaseTypeName = baseSerializerType.AssemblyQualifiedName;
			this.Reloadable = reloadable;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.Serialization.RootDesignerSerializerAttribute" /> class using the specified attributes.</summary>
		/// <param name="serializerTypeName">The fully qualified name of the data type of the serializer.</param>
		/// <param name="baseSerializerType">The name of the base type of the serializer. A class can include multiple serializers, as they all have different base types.</param>
		/// <param name="reloadable">
		///   <see langword="true" /> if this serializer supports dynamic reloading of the document; otherwise, <see langword="false" />.</param>
		public RootDesignerSerializerAttribute(string serializerTypeName, Type baseSerializerType, bool reloadable)
		{
			this.SerializerTypeName = serializerTypeName;
			this.SerializerBaseTypeName = baseSerializerType.AssemblyQualifiedName;
			this.Reloadable = reloadable;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.Serialization.RootDesignerSerializerAttribute" /> class using the specified attributes.</summary>
		/// <param name="serializerTypeName">The fully qualified name of the data type of the serializer.</param>
		/// <param name="baseSerializerTypeName">The name of the base type of the serializer. A class can include multiple serializers as they all have different base types.</param>
		/// <param name="reloadable">
		///   <see langword="true" /> if this serializer supports dynamic reloading of the document; otherwise, <see langword="false" />.</param>
		public RootDesignerSerializerAttribute(string serializerTypeName, string baseSerializerTypeName, bool reloadable)
		{
			this.SerializerTypeName = serializerTypeName;
			this.SerializerBaseTypeName = baseSerializerTypeName;
			this.Reloadable = reloadable;
		}

		/// <summary>Gets a value indicating whether the root serializer supports reloading of the design document without first disposing the designer host.</summary>
		/// <returns>
		///   <see langword="true" /> if the root serializer supports reloading; otherwise, <see langword="false" />.</returns>
		public bool Reloadable { get; }

		/// <summary>Gets the fully qualified type name of the serializer.</summary>
		/// <returns>The name of the type of the serializer.</returns>
		public string SerializerTypeName { get; }

		/// <summary>Gets the fully qualified type name of the base type of the serializer.</summary>
		/// <returns>The name of the base type of the serializer.</returns>
		public string SerializerBaseTypeName { get; }

		/// <summary>Gets a unique ID for this attribute type.</summary>
		/// <returns>An object containing a unique ID for this attribute type.</returns>
		public override object TypeId
		{
			get
			{
				if (this._typeId == null)
				{
					string text = this.SerializerBaseTypeName;
					int num = text.IndexOf(',');
					if (num != -1)
					{
						text = text.Substring(0, num);
					}
					this._typeId = base.GetType().FullName + text;
				}
				return this._typeId;
			}
		}

		private string _typeId;
	}
}
