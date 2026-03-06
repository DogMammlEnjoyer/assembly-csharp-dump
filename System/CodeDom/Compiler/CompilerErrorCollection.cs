using System;
using System.Collections;

namespace System.CodeDom.Compiler
{
	/// <summary>Represents a collection of <see cref="T:System.CodeDom.Compiler.CompilerError" /> objects.</summary>
	[Serializable]
	public class CompilerErrorCollection : CollectionBase
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.Compiler.CompilerErrorCollection" /> class.</summary>
		public CompilerErrorCollection()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.Compiler.CompilerErrorCollection" /> class that contains the contents of the specified <see cref="T:System.CodeDom.Compiler.CompilerErrorCollection" />.</summary>
		/// <param name="value">A <see cref="T:System.CodeDom.Compiler.CompilerErrorCollection" /> object with which to initialize the collection.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		public CompilerErrorCollection(CompilerErrorCollection value)
		{
			this.AddRange(value);
		}

		/// <summary>Initializes a new instance of <see cref="T:System.CodeDom.Compiler.CompilerErrorCollection" /> that contains the specified array of <see cref="T:System.CodeDom.Compiler.CompilerError" /> objects.</summary>
		/// <param name="value">An array of <see cref="T:System.CodeDom.Compiler.CompilerError" /> objects to initialize the collection with.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		public CompilerErrorCollection(CompilerError[] value)
		{
			this.AddRange(value);
		}

		/// <summary>Gets or sets the <see cref="T:System.CodeDom.Compiler.CompilerError" /> at the specified index.</summary>
		/// <param name="index">The zero-based index of the entry to locate in the collection.</param>
		/// <returns>A <see cref="T:System.CodeDom.Compiler.CompilerError" /> at each valid index.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index value indicated by the <paramref name="index" /> parameter is outside the valid range of indexes for the collection.</exception>
		public CompilerError this[int index]
		{
			get
			{
				return (CompilerError)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		/// <summary>Adds the specified <see cref="T:System.CodeDom.Compiler.CompilerError" /> object to the error collection.</summary>
		/// <param name="value">The <see cref="T:System.CodeDom.Compiler.CompilerError" /> object to add.</param>
		/// <returns>The index at which the new element was inserted.</returns>
		public int Add(CompilerError value)
		{
			return base.List.Add(value);
		}

		/// <summary>Copies the elements of an array to the end of the error collection.</summary>
		/// <param name="value">An array of type <see cref="T:System.CodeDom.Compiler.CompilerError" /> that contains the objects to add to the collection.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		public void AddRange(CompilerError[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			for (int i = 0; i < value.Length; i++)
			{
				this.Add(value[i]);
			}
		}

		/// <summary>Adds the contents of the specified compiler error collection to the end of the error collection.</summary>
		/// <param name="value">A <see cref="T:System.CodeDom.Compiler.CompilerErrorCollection" /> object that contains the objects to add to the collection.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		public void AddRange(CompilerErrorCollection value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int count = value.Count;
			for (int i = 0; i < count; i++)
			{
				this.Add(value[i]);
			}
		}

		/// <summary>Gets a value that indicates whether the collection contains the specified <see cref="T:System.CodeDom.Compiler.CompilerError" /> object.</summary>
		/// <param name="value">The <see cref="T:System.CodeDom.Compiler.CompilerError" /> to locate.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.CodeDom.Compiler.CompilerError" /> is contained in the collection; otherwise, <see langword="false" />.</returns>
		public bool Contains(CompilerError value)
		{
			return base.List.Contains(value);
		}

		/// <summary>Copies the collection values to a one-dimensional <see cref="T:System.Array" /> instance at the specified index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the values copied from <see cref="T:System.CodeDom.Compiler.CompilerErrorCollection" />.</param>
		/// <param name="index">The index in the array at which to start copying.</param>
		/// <exception cref="T:System.ArgumentException">The array indicated by the <paramref name="array" /> parameter is multidimensional.  
		///  -or-  
		///  The number of elements in the <see cref="T:System.CodeDom.Compiler.CompilerErrorCollection" /> is greater than the available space between the index value of the <paramref name="arrayIndex" /> parameter in the array indicated by the <paramref name="array" /> parameter and the end of the array indicated by the <paramref name="array" /> parameter.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="array" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="index" /> parameter is less than the lowbound of the array indicated by the <paramref name="array" /> parameter.</exception>
		public void CopyTo(CompilerError[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		/// <summary>Gets a value that indicates whether the collection contains errors.</summary>
		/// <returns>
		///   <see langword="true" /> if the collection contains errors; otherwise, <see langword="false" />.</returns>
		public bool HasErrors
		{
			get
			{
				if (base.Count > 0)
				{
					using (IEnumerator enumerator = base.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (!((CompilerError)enumerator.Current).IsWarning)
							{
								return true;
							}
						}
					}
					return false;
				}
				return false;
			}
		}

		/// <summary>Gets a value that indicates whether the collection contains warnings.</summary>
		/// <returns>
		///   <see langword="true" /> if the collection contains warnings; otherwise, <see langword="false" />.</returns>
		public bool HasWarnings
		{
			get
			{
				if (base.Count > 0)
				{
					using (IEnumerator enumerator = base.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (((CompilerError)enumerator.Current).IsWarning)
							{
								return true;
							}
						}
					}
					return false;
				}
				return false;
			}
		}

		/// <summary>Gets the index of the specified <see cref="T:System.CodeDom.Compiler.CompilerError" /> object in the collection, if it exists in the collection.</summary>
		/// <param name="value">The <see cref="T:System.CodeDom.Compiler.CompilerError" /> to locate.</param>
		/// <returns>The index of the specified <see cref="T:System.CodeDom.Compiler.CompilerError" /> in the <see cref="T:System.CodeDom.Compiler.CompilerErrorCollection" />, if found; otherwise, -1.</returns>
		public int IndexOf(CompilerError value)
		{
			return base.List.IndexOf(value);
		}

		/// <summary>Inserts the specified <see cref="T:System.CodeDom.Compiler.CompilerError" /> into the collection at the specified index.</summary>
		/// <param name="index">The zero-based index where the compiler error should be inserted.</param>
		/// <param name="value">The <see cref="T:System.CodeDom.Compiler.CompilerError" /> to insert.</param>
		public void Insert(int index, CompilerError value)
		{
			base.List.Insert(index, value);
		}

		/// <summary>Removes a specific <see cref="T:System.CodeDom.Compiler.CompilerError" /> from the collection.</summary>
		/// <param name="value">The <see cref="T:System.CodeDom.Compiler.CompilerError" /> to remove from the <see cref="T:System.CodeDom.Compiler.CompilerErrorCollection" />.</param>
		/// <exception cref="T:System.ArgumentException">The specified object is not found in the collection.</exception>
		public void Remove(CompilerError value)
		{
			base.List.Remove(value);
		}
	}
}
