using System;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	/// <summary>Represents a node or an attribute in an XML tree.</summary>
	public abstract class XObject : IXmlLineInfo
	{
		internal XObject()
		{
		}

		/// <summary>Gets the base URI for this <see cref="T:System.Xml.Linq.XObject" />.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the base URI for this <see cref="T:System.Xml.Linq.XObject" />.</returns>
		public string BaseUri
		{
			get
			{
				XObject xobject = this;
				BaseUriAnnotation baseUriAnnotation;
				for (;;)
				{
					if (xobject == null || xobject.annotations != null)
					{
						if (xobject == null)
						{
							goto IL_33;
						}
						baseUriAnnotation = xobject.Annotation<BaseUriAnnotation>();
						if (baseUriAnnotation != null)
						{
							break;
						}
						xobject = xobject.parent;
					}
					else
					{
						xobject = xobject.parent;
					}
				}
				return baseUriAnnotation.baseUri;
				IL_33:
				return string.Empty;
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.Linq.XDocument" /> for this <see cref="T:System.Xml.Linq.XObject" />.</summary>
		/// <returns>The <see cref="T:System.Xml.Linq.XDocument" /> for this <see cref="T:System.Xml.Linq.XObject" />.</returns>
		public XDocument Document
		{
			get
			{
				XObject xobject = this;
				while (xobject.parent != null)
				{
					xobject = xobject.parent;
				}
				return xobject as XDocument;
			}
		}

		/// <summary>Gets the node type for this <see cref="T:System.Xml.Linq.XObject" />.</summary>
		/// <returns>The node type for this <see cref="T:System.Xml.Linq.XObject" />.</returns>
		public abstract XmlNodeType NodeType { get; }

		/// <summary>Gets the parent <see cref="T:System.Xml.Linq.XElement" /> of this <see cref="T:System.Xml.Linq.XObject" />.</summary>
		/// <returns>The parent <see cref="T:System.Xml.Linq.XElement" /> of this <see cref="T:System.Xml.Linq.XObject" />.</returns>
		public XElement Parent
		{
			get
			{
				return this.parent as XElement;
			}
		}

		/// <summary>Adds an object to the annotation list of this <see cref="T:System.Xml.Linq.XObject" />.</summary>
		/// <param name="annotation">An object that contains the annotation to add.</param>
		public void AddAnnotation(object annotation)
		{
			if (annotation == null)
			{
				throw new ArgumentNullException("annotation");
			}
			if (this.annotations == null)
			{
				object obj;
				if (!(annotation is object[]))
				{
					obj = annotation;
				}
				else
				{
					(obj = new object[1])[0] = annotation;
				}
				this.annotations = obj;
				return;
			}
			object[] array = this.annotations as object[];
			if (array == null)
			{
				this.annotations = new object[]
				{
					this.annotations,
					annotation
				};
				return;
			}
			int num = 0;
			while (num < array.Length && array[num] != null)
			{
				num++;
			}
			if (num == array.Length)
			{
				Array.Resize<object>(ref array, num * 2);
				this.annotations = array;
			}
			array[num] = annotation;
		}

		/// <summary>Gets the first annotation object of the specified type from this <see cref="T:System.Xml.Linq.XObject" />.</summary>
		/// <param name="type">The type of the annotation to retrieve.</param>
		/// <returns>The <see cref="T:System.Object" /> that contains the first annotation object that matches the specified type, or <see langword="null" /> if no annotation is of the specified type.</returns>
		public object Annotation(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (this.annotations != null)
			{
				object[] array = this.annotations as object[];
				if (array == null)
				{
					if (XHelper.IsInstanceOfType(this.annotations, type))
					{
						return this.annotations;
					}
				}
				else
				{
					foreach (object obj in array)
					{
						if (obj == null)
						{
							break;
						}
						if (XHelper.IsInstanceOfType(obj, type))
						{
							return obj;
						}
					}
				}
			}
			return null;
		}

		private object AnnotationForSealedType(Type type)
		{
			if (this.annotations != null)
			{
				object[] array = this.annotations as object[];
				if (array == null)
				{
					if (this.annotations.GetType() == type)
					{
						return this.annotations;
					}
				}
				else
				{
					foreach (object obj in array)
					{
						if (obj == null)
						{
							break;
						}
						if (obj.GetType() == type)
						{
							return obj;
						}
					}
				}
			}
			return null;
		}

		/// <summary>Gets the first annotation object of the specified type from this <see cref="T:System.Xml.Linq.XObject" />.</summary>
		/// <typeparam name="T">The type of the annotation to retrieve.</typeparam>
		/// <returns>The first annotation object that matches the specified type, or <see langword="null" /> if no annotation is of the specified type.</returns>
		public T Annotation<T>() where T : class
		{
			if (this.annotations != null)
			{
				object[] array = this.annotations as object[];
				if (array == null)
				{
					return this.annotations as T;
				}
				foreach (object obj in array)
				{
					if (obj == null)
					{
						break;
					}
					T t = obj as T;
					if (t != null)
					{
						return t;
					}
				}
			}
			return default(T);
		}

		/// <summary>Gets a collection of annotations of the specified type for this <see cref="T:System.Xml.Linq.XObject" />.</summary>
		/// <param name="type">The type of the annotations to retrieve.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Object" /> that contains the annotations that match the specified type for this <see cref="T:System.Xml.Linq.XObject" />.</returns>
		public IEnumerable<object> Annotations(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return this.AnnotationsIterator(type);
		}

		private IEnumerable<object> AnnotationsIterator(Type type)
		{
			if (this.annotations != null)
			{
				object[] a = this.annotations as object[];
				if (a == null)
				{
					if (XHelper.IsInstanceOfType(this.annotations, type))
					{
						yield return this.annotations;
					}
				}
				else
				{
					int num;
					for (int i = 0; i < a.Length; i = num + 1)
					{
						object obj = a[i];
						if (obj == null)
						{
							break;
						}
						if (XHelper.IsInstanceOfType(obj, type))
						{
							yield return obj;
						}
						num = i;
					}
				}
				a = null;
			}
			yield break;
		}

		/// <summary>Gets a collection of annotations of the specified type for this <see cref="T:System.Xml.Linq.XObject" />.</summary>
		/// <typeparam name="T">The type of the annotations to retrieve.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the annotations for this <see cref="T:System.Xml.Linq.XObject" />.</returns>
		public IEnumerable<T> Annotations<T>() where T : class
		{
			if (this.annotations != null)
			{
				object[] a = this.annotations as object[];
				if (a == null)
				{
					T t = this.annotations as T;
					if (t != null)
					{
						yield return t;
					}
				}
				else
				{
					int num;
					for (int i = 0; i < a.Length; i = num + 1)
					{
						object obj = a[i];
						if (obj == null)
						{
							break;
						}
						T t2 = obj as T;
						if (t2 != null)
						{
							yield return t2;
						}
						num = i;
					}
				}
				a = null;
			}
			yield break;
		}

		/// <summary>Removes the annotations of the specified type from this <see cref="T:System.Xml.Linq.XObject" />.</summary>
		/// <param name="type">The type of annotations to remove.</param>
		public void RemoveAnnotations(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (this.annotations != null)
			{
				object[] array = this.annotations as object[];
				if (array == null)
				{
					if (XHelper.IsInstanceOfType(this.annotations, type))
					{
						this.annotations = null;
						return;
					}
				}
				else
				{
					int i = 0;
					int j = 0;
					while (i < array.Length)
					{
						object obj = array[i];
						if (obj == null)
						{
							break;
						}
						if (!XHelper.IsInstanceOfType(obj, type))
						{
							array[j++] = obj;
						}
						i++;
					}
					if (j == 0)
					{
						this.annotations = null;
						return;
					}
					while (j < i)
					{
						array[j++] = null;
					}
				}
			}
		}

		/// <summary>Removes the annotations of the specified type from this <see cref="T:System.Xml.Linq.XObject" />.</summary>
		/// <typeparam name="T">The type of annotations to remove.</typeparam>
		public void RemoveAnnotations<T>() where T : class
		{
			if (this.annotations != null)
			{
				object[] array = this.annotations as object[];
				if (array == null)
				{
					if (this.annotations is T)
					{
						this.annotations = null;
						return;
					}
				}
				else
				{
					int i = 0;
					int j = 0;
					while (i < array.Length)
					{
						object obj = array[i];
						if (obj == null)
						{
							break;
						}
						if (!(obj is T))
						{
							array[j++] = obj;
						}
						i++;
					}
					if (j == 0)
					{
						this.annotations = null;
						return;
					}
					while (j < i)
					{
						array[j++] = null;
					}
				}
			}
		}

		/// <summary>Raised when this <see cref="T:System.Xml.Linq.XObject" /> or any of its descendants have changed.</summary>
		public event EventHandler<XObjectChangeEventArgs> Changed
		{
			add
			{
				if (value == null)
				{
					return;
				}
				XObjectChangeAnnotation xobjectChangeAnnotation = this.Annotation<XObjectChangeAnnotation>();
				if (xobjectChangeAnnotation == null)
				{
					xobjectChangeAnnotation = new XObjectChangeAnnotation();
					this.AddAnnotation(xobjectChangeAnnotation);
				}
				XObjectChangeAnnotation xobjectChangeAnnotation2 = xobjectChangeAnnotation;
				xobjectChangeAnnotation2.changed = (EventHandler<XObjectChangeEventArgs>)Delegate.Combine(xobjectChangeAnnotation2.changed, value);
			}
			remove
			{
				if (value == null)
				{
					return;
				}
				XObjectChangeAnnotation xobjectChangeAnnotation = this.Annotation<XObjectChangeAnnotation>();
				if (xobjectChangeAnnotation == null)
				{
					return;
				}
				XObjectChangeAnnotation xobjectChangeAnnotation2 = xobjectChangeAnnotation;
				xobjectChangeAnnotation2.changed = (EventHandler<XObjectChangeEventArgs>)Delegate.Remove(xobjectChangeAnnotation2.changed, value);
				if (xobjectChangeAnnotation.changing == null && xobjectChangeAnnotation.changed == null)
				{
					this.RemoveAnnotations<XObjectChangeAnnotation>();
				}
			}
		}

		/// <summary>Raised when this <see cref="T:System.Xml.Linq.XObject" /> or any of its descendants are about to change.</summary>
		public event EventHandler<XObjectChangeEventArgs> Changing
		{
			add
			{
				if (value == null)
				{
					return;
				}
				XObjectChangeAnnotation xobjectChangeAnnotation = this.Annotation<XObjectChangeAnnotation>();
				if (xobjectChangeAnnotation == null)
				{
					xobjectChangeAnnotation = new XObjectChangeAnnotation();
					this.AddAnnotation(xobjectChangeAnnotation);
				}
				XObjectChangeAnnotation xobjectChangeAnnotation2 = xobjectChangeAnnotation;
				xobjectChangeAnnotation2.changing = (EventHandler<XObjectChangeEventArgs>)Delegate.Combine(xobjectChangeAnnotation2.changing, value);
			}
			remove
			{
				if (value == null)
				{
					return;
				}
				XObjectChangeAnnotation xobjectChangeAnnotation = this.Annotation<XObjectChangeAnnotation>();
				if (xobjectChangeAnnotation == null)
				{
					return;
				}
				XObjectChangeAnnotation xobjectChangeAnnotation2 = xobjectChangeAnnotation;
				xobjectChangeAnnotation2.changing = (EventHandler<XObjectChangeEventArgs>)Delegate.Remove(xobjectChangeAnnotation2.changing, value);
				if (xobjectChangeAnnotation.changing == null && xobjectChangeAnnotation.changed == null)
				{
					this.RemoveAnnotations<XObjectChangeAnnotation>();
				}
			}
		}

		/// <summary>Gets a value indicating whether or not this <see cref="T:System.Xml.Linq.XObject" /> has line information.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Xml.Linq.XObject" /> has line information; otherwise, <see langword="false" />.</returns>
		bool IXmlLineInfo.HasLineInfo()
		{
			return this.Annotation<LineInfoAnnotation>() != null;
		}

		/// <summary>Gets the line number that the underlying <see cref="T:System.Xml.XmlReader" /> reported for this <see cref="T:System.Xml.Linq.XObject" />.</summary>
		/// <returns>An <see cref="T:System.Int32" /> that contains the line number reported by the <see cref="T:System.Xml.XmlReader" /> for this <see cref="T:System.Xml.Linq.XObject" />.</returns>
		int IXmlLineInfo.LineNumber
		{
			get
			{
				LineInfoAnnotation lineInfoAnnotation = this.Annotation<LineInfoAnnotation>();
				if (lineInfoAnnotation != null)
				{
					return lineInfoAnnotation.lineNumber;
				}
				return 0;
			}
		}

		/// <summary>Gets the line position that the underlying <see cref="T:System.Xml.XmlReader" /> reported for this <see cref="T:System.Xml.Linq.XObject" />.</summary>
		/// <returns>An <see cref="T:System.Int32" /> that contains the line position reported by the <see cref="T:System.Xml.XmlReader" /> for this <see cref="T:System.Xml.Linq.XObject" />.</returns>
		int IXmlLineInfo.LinePosition
		{
			get
			{
				LineInfoAnnotation lineInfoAnnotation = this.Annotation<LineInfoAnnotation>();
				if (lineInfoAnnotation != null)
				{
					return lineInfoAnnotation.linePosition;
				}
				return 0;
			}
		}

		internal bool HasBaseUri
		{
			get
			{
				return this.Annotation<BaseUriAnnotation>() != null;
			}
		}

		internal bool NotifyChanged(object sender, XObjectChangeEventArgs e)
		{
			bool result = false;
			XObject xobject = this;
			for (;;)
			{
				if (xobject == null || xobject.annotations != null)
				{
					if (xobject == null)
					{
						break;
					}
					XObjectChangeAnnotation xobjectChangeAnnotation = xobject.Annotation<XObjectChangeAnnotation>();
					if (xobjectChangeAnnotation != null)
					{
						result = true;
						if (xobjectChangeAnnotation.changed != null)
						{
							xobjectChangeAnnotation.changed(sender, e);
						}
					}
					xobject = xobject.parent;
				}
				else
				{
					xobject = xobject.parent;
				}
			}
			return result;
		}

		internal bool NotifyChanging(object sender, XObjectChangeEventArgs e)
		{
			bool result = false;
			XObject xobject = this;
			for (;;)
			{
				if (xobject == null || xobject.annotations != null)
				{
					if (xobject == null)
					{
						break;
					}
					XObjectChangeAnnotation xobjectChangeAnnotation = xobject.Annotation<XObjectChangeAnnotation>();
					if (xobjectChangeAnnotation != null)
					{
						result = true;
						if (xobjectChangeAnnotation.changing != null)
						{
							xobjectChangeAnnotation.changing(sender, e);
						}
					}
					xobject = xobject.parent;
				}
				else
				{
					xobject = xobject.parent;
				}
			}
			return result;
		}

		internal void SetBaseUri(string baseUri)
		{
			this.AddAnnotation(new BaseUriAnnotation(baseUri));
		}

		internal void SetLineInfo(int lineNumber, int linePosition)
		{
			this.AddAnnotation(new LineInfoAnnotation(lineNumber, linePosition));
		}

		internal bool SkipNotify()
		{
			XObject xobject = this;
			for (;;)
			{
				if (xobject == null || xobject.annotations != null)
				{
					if (xobject == null)
					{
						break;
					}
					if (xobject.Annotation<XObjectChangeAnnotation>() != null)
					{
						return false;
					}
					xobject = xobject.parent;
				}
				else
				{
					xobject = xobject.parent;
				}
			}
			return true;
		}

		internal SaveOptions GetSaveOptionsFromAnnotations()
		{
			XObject xobject = this;
			object obj;
			for (;;)
			{
				if (xobject == null || xobject.annotations != null)
				{
					if (xobject == null)
					{
						break;
					}
					obj = xobject.AnnotationForSealedType(typeof(SaveOptions));
					if (obj != null)
					{
						goto Block_3;
					}
					xobject = xobject.parent;
				}
				else
				{
					xobject = xobject.parent;
				}
			}
			return SaveOptions.None;
			Block_3:
			return (SaveOptions)obj;
		}

		internal XContainer parent;

		internal object annotations;
	}
}
