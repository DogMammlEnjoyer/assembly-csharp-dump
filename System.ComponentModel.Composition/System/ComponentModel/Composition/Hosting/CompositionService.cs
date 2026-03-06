using System;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal;
using Unity;

namespace System.ComponentModel.Composition.Hosting
{
	/// <summary>Provides methods to satisfy imports on an existing part instance.</summary>
	public class CompositionService : ICompositionService, IDisposable
	{
		internal CompositionService(ComposablePartCatalog composablePartCatalog)
		{
			Assumes.NotNull<ComposablePartCatalog>(composablePartCatalog);
			this._notifyCatalog = (composablePartCatalog as INotifyComposablePartCatalogChanged);
			try
			{
				if (this._notifyCatalog != null)
				{
					this._notifyCatalog.Changing += this.OnCatalogChanging;
				}
				CompositionOptions compositionOptions = CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe | CompositionOptions.ExportCompositionService;
				CompositionContainer compositionContainer = new CompositionContainer(composablePartCatalog, compositionOptions, Array.Empty<ExportProvider>());
				this._compositionContainer = compositionContainer;
			}
			catch
			{
				if (this._notifyCatalog != null)
				{
					this._notifyCatalog.Changing -= this.OnCatalogChanging;
				}
				throw;
			}
		}

		/// <summary>Composes the specified part, with recomposition and validation disabled.</summary>
		/// <param name="part">The part to compose.</param>
		public void SatisfyImportsOnce(ComposablePart part)
		{
			Requires.NotNull<ComposablePart>(part, "part");
			Assumes.NotNull<CompositionContainer>(this._compositionContainer);
			this._compositionContainer.SatisfyImportsOnce(part);
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> class.</summary>
		public void Dispose()
		{
			Assumes.NotNull<CompositionContainer>(this._compositionContainer);
			if (this._notifyCatalog != null)
			{
				this._notifyCatalog.Changing -= this.OnCatalogChanging;
			}
			this._compositionContainer.Dispose();
		}

		private void OnCatalogChanging(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			throw new ChangeRejectedException(Strings.NotSupportedCatalogChanges);
		}

		internal CompositionService()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private CompositionContainer _compositionContainer;

		private INotifyComposablePartCatalogChanged _notifyCatalog;
	}
}
