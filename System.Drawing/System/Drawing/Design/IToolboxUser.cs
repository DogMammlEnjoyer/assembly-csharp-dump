using System;

namespace System.Drawing.Design
{
	/// <summary>Defines an interface for setting the currently selected toolbox item and indicating whether a designer supports a particular toolbox item.</summary>
	public interface IToolboxUser
	{
		/// <summary>Gets a value indicating whether the specified tool is supported by the current designer.</summary>
		/// <param name="tool">The <see cref="T:System.Drawing.Design.ToolboxItem" /> to be tested for toolbox support.</param>
		/// <returns>
		///   <see langword="true" /> if the tool is supported by the toolbox and can be enabled; <see langword="false" /> if the document designer does not know how to use the tool.</returns>
		bool GetToolSupported(ToolboxItem tool);

		/// <summary>Selects the specified tool.</summary>
		/// <param name="tool">The <see cref="T:System.Drawing.Design.ToolboxItem" /> to select.</param>
		void ToolPicked(ToolboxItem tool);
	}
}
