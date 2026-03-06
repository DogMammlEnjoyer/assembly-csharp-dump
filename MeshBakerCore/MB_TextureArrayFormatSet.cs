using System;
using DigitalOpus.MB.Core;
using UnityEngine;

[Serializable]
public class MB_TextureArrayFormatSet
{
	public bool ValidateTextureImporterFormatsExistsForTextureFormats(MB2_EditorMethodsInterface editorMethods, int idx)
	{
		if (editorMethods == null)
		{
			return true;
		}
		if (!editorMethods.TextureImporterFormatExistsForTextureFormat(this.defaultFormat))
		{
			Debug.LogError("TextureImporter format does not exist for Texture Array Output Formats: " + idx.ToString() + " Defaut Format " + this.defaultFormat.ToString());
			return false;
		}
		for (int i = 0; i < this.formatOverrides.Length; i++)
		{
			if (!editorMethods.TextureImporterFormatExistsForTextureFormat(this.formatOverrides[i].format))
			{
				Debug.LogError(string.Concat(new string[]
				{
					"TextureImporter format does not exist for Texture Array Output Formats: ",
					idx.ToString(),
					" Format Overrides: ",
					i.ToString(),
					" (",
					this.formatOverrides[i].format.ToString(),
					")"
				}));
				return false;
			}
		}
		return true;
	}

	public TextureFormat GetFormatForProperty(string propName, out MB_TextureCompressionQuality compressionQuality)
	{
		for (int i = 0; i < this.formatOverrides.Length; i++)
		{
			if (this.formatOverrides.Equals(this.formatOverrides[i].propertyName))
			{
				compressionQuality = this.formatOverrides[i].compressionQuality;
				return this.formatOverrides[i].format;
			}
		}
		compressionQuality = this.defaultCompressionQuality;
		return this.defaultFormat;
	}

	public string name;

	public TextureFormat defaultFormat;

	[Tooltip("The ammount of time Unity takes exploring different compression options to find the compressed version of a texture that most closely matches the original art.This is only used For iOS (and some Android formats)")]
	public MB_TextureCompressionQuality defaultCompressionQuality = MB_TextureCompressionQuality.normal;

	[NonReorderable]
	public MB_TextureArrayFormat[] formatOverrides;
}
