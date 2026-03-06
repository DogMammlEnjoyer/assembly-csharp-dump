using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	/// <summary>Represents a set of <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> objects which will be added or removed from the container in a single transactional composition.</summary>
	public class CompositionBatch
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionBatch" /> class.</summary>
		public CompositionBatch() : this(null, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionBatch" /> class with the specified parts for addition and removal.</summary>
		/// <param name="partsToAdd">A collection of <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> objects to add.</param>
		/// <param name="partsToRemove">A collection of <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> objects to remove.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="partsToAdd" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="partsToRemove" /> is <see langword="null" />.</exception>
		public CompositionBatch(IEnumerable<ComposablePart> partsToAdd, IEnumerable<ComposablePart> partsToRemove)
		{
			this._partsToAdd = new List<ComposablePart>();
			if (partsToAdd != null)
			{
				foreach (ComposablePart composablePart in partsToAdd)
				{
					if (composablePart == null)
					{
						throw ExceptionBuilder.CreateContainsNullElement("partsToAdd");
					}
					this._partsToAdd.Add(composablePart);
				}
			}
			this._readOnlyPartsToAdd = this._partsToAdd.AsReadOnly();
			this._partsToRemove = new List<ComposablePart>();
			if (partsToRemove != null)
			{
				foreach (ComposablePart composablePart2 in partsToRemove)
				{
					if (composablePart2 == null)
					{
						throw ExceptionBuilder.CreateContainsNullElement("partsToRemove");
					}
					this._partsToRemove.Add(composablePart2);
				}
			}
			this._readOnlyPartsToRemove = this._partsToRemove.AsReadOnly();
		}

		/// <summary>Gets the collection of <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> objects to be added.</summary>
		/// <returns>A collection of parts to be added.</returns>
		public ReadOnlyCollection<ComposablePart> PartsToAdd
		{
			get
			{
				object @lock = this._lock;
				ReadOnlyCollection<ComposablePart> readOnlyPartsToAdd;
				lock (@lock)
				{
					this._copyNeededForAdd = true;
					readOnlyPartsToAdd = this._readOnlyPartsToAdd;
				}
				return readOnlyPartsToAdd;
			}
		}

		/// <summary>Gets the collection of <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> objects to be removed.</summary>
		/// <returns>A collection of parts to be removed.</returns>
		public ReadOnlyCollection<ComposablePart> PartsToRemove
		{
			get
			{
				object @lock = this._lock;
				ReadOnlyCollection<ComposablePart> readOnlyPartsToRemove;
				lock (@lock)
				{
					this._copyNeededForRemove = true;
					readOnlyPartsToRemove = this._readOnlyPartsToRemove;
				}
				return readOnlyPartsToRemove;
			}
		}

		/// <summary>Adds the specified part to the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionBatch" /> object.</summary>
		/// <param name="part">The part to add.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="part" /> is <see langword="null" />.</exception>
		public void AddPart(ComposablePart part)
		{
			Requires.NotNull<ComposablePart>(part, "part");
			object @lock = this._lock;
			lock (@lock)
			{
				if (this._copyNeededForAdd)
				{
					this._partsToAdd = new List<ComposablePart>(this._partsToAdd);
					this._readOnlyPartsToAdd = this._partsToAdd.AsReadOnly();
					this._copyNeededForAdd = false;
				}
				this._partsToAdd.Add(part);
			}
		}

		/// <summary>Puts the specified part on the list of parts to remove.</summary>
		/// <param name="part">The part to be removed.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="part" /> is <see langword="null" />.</exception>
		public void RemovePart(ComposablePart part)
		{
			Requires.NotNull<ComposablePart>(part, "part");
			object @lock = this._lock;
			lock (@lock)
			{
				if (this._copyNeededForRemove)
				{
					this._partsToRemove = new List<ComposablePart>(this._partsToRemove);
					this._readOnlyPartsToRemove = this._partsToRemove.AsReadOnly();
					this._copyNeededForRemove = false;
				}
				this._partsToRemove.Add(part);
			}
		}

		/// <summary>Adds the specified export to the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionBatch" /> object.</summary>
		/// <param name="export">The export to add to the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionBatch" /> object.</param>
		/// <returns>The part added.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="export" /> is <see langword="null" />.</exception>
		public ComposablePart AddExport(Export export)
		{
			Requires.NotNull<Export>(export, "export");
			ComposablePart composablePart = new CompositionBatch.SingleExportComposablePart(export);
			this.AddPart(composablePart);
			return composablePart;
		}

		private object _lock = new object();

		private bool _copyNeededForAdd;

		private bool _copyNeededForRemove;

		private List<ComposablePart> _partsToAdd;

		private ReadOnlyCollection<ComposablePart> _readOnlyPartsToAdd;

		private List<ComposablePart> _partsToRemove;

		private ReadOnlyCollection<ComposablePart> _readOnlyPartsToRemove;

		private class SingleExportComposablePart : ComposablePart
		{
			public SingleExportComposablePart(Export export)
			{
				Assumes.NotNull<Export>(export);
				this._export = export;
			}

			public override IDictionary<string, object> Metadata
			{
				get
				{
					return MetadataServices.EmptyMetadata;
				}
			}

			public override IEnumerable<ExportDefinition> ExportDefinitions
			{
				get
				{
					return new ExportDefinition[]
					{
						this._export.Definition
					};
				}
			}

			public override IEnumerable<ImportDefinition> ImportDefinitions
			{
				get
				{
					return Enumerable.Empty<ImportDefinition>();
				}
			}

			public override object GetExportedValue(ExportDefinition definition)
			{
				Requires.NotNull<ExportDefinition>(definition, "definition");
				if (definition != this._export.Definition)
				{
					throw ExceptionBuilder.CreateExportDefinitionNotOnThisComposablePart("definition");
				}
				return this._export.Value;
			}

			public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
			{
				Requires.NotNull<ImportDefinition>(definition, "definition");
				Requires.NotNullOrNullElements<Export>(exports, "exports");
				throw ExceptionBuilder.CreateImportDefinitionNotOnThisComposablePart("definition");
			}

			private readonly Export _export;
		}
	}
}
