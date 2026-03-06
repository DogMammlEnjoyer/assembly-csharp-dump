using System;

namespace Meta.WitAi.Composer.Data
{
	public abstract class BaseReservedContextPath : IContextMapReservedPathExtension
	{
		protected ComposerContextMap Map
		{
			get
			{
				return this._composer.CurrentContextMap;
			}
		}

		protected abstract string ReservedPath { get; }

		protected internal abstract void UpdateContextMap();

		public virtual void AssignTo(ComposerService composer)
		{
			if (this._composer == composer)
			{
				return;
			}
			this._composer = composer;
			ComposerContextMap.ReservedPaths.Add(this.ReservedPath);
			this.HasComposer = true;
		}

		public virtual void Clear()
		{
			ComposerContextMap map = this.Map;
			if (map == null)
			{
				return;
			}
			map.ClearData(this.ReservedPath, true);
		}

		private ComposerService _composer;

		public bool HasComposer;
	}
}
