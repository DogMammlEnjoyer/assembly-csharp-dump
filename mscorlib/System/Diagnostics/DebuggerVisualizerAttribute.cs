using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	/// <summary>Specifies that the type has a visualizer. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	[ComVisible(true)]
	public sealed class DebuggerVisualizerAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DebuggerVisualizerAttribute" /> class, specifying the type name of the visualizer.</summary>
		/// <param name="visualizerTypeName">The fully qualified type name of the visualizer.</param>
		public DebuggerVisualizerAttribute(string visualizerTypeName)
		{
			this.visualizerName = visualizerTypeName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DebuggerVisualizerAttribute" /> class, specifying the type name of the visualizer and the type name of the visualizer object source.</summary>
		/// <param name="visualizerTypeName">The fully qualified type name of the visualizer.</param>
		/// <param name="visualizerObjectSourceTypeName">The fully qualified type name of the visualizer object source.</param>
		public DebuggerVisualizerAttribute(string visualizerTypeName, string visualizerObjectSourceTypeName)
		{
			this.visualizerName = visualizerTypeName;
			this.visualizerObjectSourceName = visualizerObjectSourceTypeName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DebuggerVisualizerAttribute" /> class, specifying the type name of the visualizer and the type of the visualizer object source.</summary>
		/// <param name="visualizerTypeName">The fully qualified type name of the visualizer.</param>
		/// <param name="visualizerObjectSource">The type of the visualizer object source.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="visualizerObjectSource" /> is <see langword="null" />.</exception>
		public DebuggerVisualizerAttribute(string visualizerTypeName, Type visualizerObjectSource)
		{
			if (visualizerObjectSource == null)
			{
				throw new ArgumentNullException("visualizerObjectSource");
			}
			this.visualizerName = visualizerTypeName;
			this.visualizerObjectSourceName = visualizerObjectSource.AssemblyQualifiedName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DebuggerVisualizerAttribute" /> class, specifying the type of the visualizer.</summary>
		/// <param name="visualizer">The type of the visualizer.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="visualizer" /> is <see langword="null" />.</exception>
		public DebuggerVisualizerAttribute(Type visualizer)
		{
			if (visualizer == null)
			{
				throw new ArgumentNullException("visualizer");
			}
			this.visualizerName = visualizer.AssemblyQualifiedName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DebuggerVisualizerAttribute" /> class, specifying the type of the visualizer and the type of the visualizer object source.</summary>
		/// <param name="visualizer">The type of the visualizer.</param>
		/// <param name="visualizerObjectSource">The type of the visualizer object source.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="visualizerObjectSource" /> is <see langword="null" />.</exception>
		public DebuggerVisualizerAttribute(Type visualizer, Type visualizerObjectSource)
		{
			if (visualizer == null)
			{
				throw new ArgumentNullException("visualizer");
			}
			if (visualizerObjectSource == null)
			{
				throw new ArgumentNullException("visualizerObjectSource");
			}
			this.visualizerName = visualizer.AssemblyQualifiedName;
			this.visualizerObjectSourceName = visualizerObjectSource.AssemblyQualifiedName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DebuggerVisualizerAttribute" /> class, specifying the type of the visualizer and the type name of the visualizer object source.</summary>
		/// <param name="visualizer">The type of the visualizer.</param>
		/// <param name="visualizerObjectSourceTypeName">The fully qualified type name of the visualizer object source.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="visualizer" /> is <see langword="null" />.</exception>
		public DebuggerVisualizerAttribute(Type visualizer, string visualizerObjectSourceTypeName)
		{
			if (visualizer == null)
			{
				throw new ArgumentNullException("visualizer");
			}
			this.visualizerName = visualizer.AssemblyQualifiedName;
			this.visualizerObjectSourceName = visualizerObjectSourceTypeName;
		}

		/// <summary>Gets the fully qualified type name of the visualizer object source.</summary>
		/// <returns>The fully qualified type name of the visualizer object source.</returns>
		public string VisualizerObjectSourceTypeName
		{
			get
			{
				return this.visualizerObjectSourceName;
			}
		}

		/// <summary>Gets the fully qualified type name of the visualizer.</summary>
		/// <returns>The fully qualified visualizer type name.</returns>
		public string VisualizerTypeName
		{
			get
			{
				return this.visualizerName;
			}
		}

		/// <summary>Gets or sets the description of the visualizer.</summary>
		/// <returns>The description of the visualizer.</returns>
		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		/// <summary>Gets or sets the target type when the attribute is applied at the assembly level.</summary>
		/// <returns>The type that is the target of the visualizer.</returns>
		/// <exception cref="T:System.ArgumentNullException">The value cannot be set because it is <see langword="null" />.</exception>
		public Type Target
		{
			get
			{
				return this.target;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.targetName = value.AssemblyQualifiedName;
				this.target = value;
			}
		}

		/// <summary>Gets or sets the fully qualified type name when the attribute is applied at the assembly level.</summary>
		/// <returns>The fully qualified type name of the target type.</returns>
		public string TargetTypeName
		{
			get
			{
				return this.targetName;
			}
			set
			{
				this.targetName = value;
			}
		}

		private string visualizerObjectSourceName;

		private string visualizerName;

		private string description;

		private string targetName;

		private Type target;
	}
}
