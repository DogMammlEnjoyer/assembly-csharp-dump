using System;
using UnityEngine;

public class InlineLinkAttribute : PropertyAttribute
{
	public InlineLinkAttribute(string documentationURL)
	{
		this.DocumentationURL = documentationURL;
	}

	public string DocumentationURL;
}
