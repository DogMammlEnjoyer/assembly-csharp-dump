using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API;
using Modio.Errors;

namespace Modio.Mods.Builder
{
	public class ModBuilder
	{
		public List<ValueTuple<ChangeFlags, Error>> Results
		{
			get
			{
				Dictionary<ChangeFlags, Error> commitErrors = this._commitErrors;
				if (commitErrors == null)
				{
					return null;
				}
				return (from yeet in commitErrors
				select new ValueTuple<ChangeFlags, Error>(yeet.Key, yeet.Value)).ToList<ValueTuple<ChangeFlags, Error>>();
			}
		}

		public string Name { get; private set; }

		public string Summary { get; private set; }

		public string Description { get; private set; }

		public string LogoFilePath { get; private set; }

		public string[] GalleryFilePaths { get; private set; }

		public string[] Tags { get; private set; }

		public string MetadataBlob { get; private set; }

		public Dictionary<string, string> MetadataKvps { get; private set; }

		public List<long> Dependencies { get; private set; } = new List<long>();

		public bool Visible { get; private set; }

		public ModMaturityOptions MaturityOptions { get; private set; }

		public ModCommunityOptions CommunityOptions { get; private set; }

		public bool IsMonetized { get; private set; }

		public bool IsLimitedStock { get; private set; }

		public int Price { get; private set; }

		public int Stock { get; private set; }

		public bool IsEditMode
		{
			get
			{
				return this.EditTarget != null;
			}
		}

		public Mod EditTarget { get; private set; }

		internal ModBuilder()
		{
			this.EditTarget = null;
		}

		internal ModBuilder(Mod editTarget)
		{
			this.EditTarget = editTarget;
			this.Name = editTarget.Name;
			this.Summary = editTarget.Summary;
			this.Description = editTarget.Description;
			this.Tags = (from tag in editTarget.Tags
			select tag.ApiName).ToArray<string>();
			this.MetadataBlob = editTarget.MetadataBlob;
			this.MetadataKvps = editTarget.MetadataKvps;
			this._monetizationOptions = (editTarget.IsMonetized ? (ModBuilder.MonetizationOptions.Enabled | ModBuilder.MonetizationOptions.Live) : ModBuilder.MonetizationOptions.None);
			this.Price = (editTarget.IsMonetized ? ((int)editTarget.Price) : 0);
		}

		public ModBuilder SetName(string name)
		{
			this.Name = name;
			this._pendingChanges |= ChangeFlags.Name;
			return this;
		}

		public ModBuilder SetSummary(string summary)
		{
			this.Summary = summary;
			this._pendingChanges |= ChangeFlags.Summary;
			return this;
		}

		public ModBuilder SetDescription(string description)
		{
			this.Description = description;
			this._pendingChanges |= ChangeFlags.Description;
			return this;
		}

		public ModBuilder SetTags(ICollection<string> tags)
		{
			this.Tags = tags.ToArray<string>();
			this._pendingChanges |= ChangeFlags.Tags;
			return this;
		}

		public ModBuilder SetTags(string tag)
		{
			return this.SetTags(new string[]
			{
				tag
			});
		}

		public ModBuilder AppendTags(ICollection<string> tags)
		{
			this.Tags = this.Tags.Concat(tags).ToArray<string>();
			this._pendingChanges |= ChangeFlags.Tags;
			return this;
		}

		public ModBuilder AppendTags(string tag)
		{
			return this.AppendTags(new string[]
			{
				tag
			});
		}

		public ModBuilder SetMetadataBlob(string data)
		{
			this.MetadataBlob = data;
			this._pendingChanges |= ChangeFlags.MetadataBlob;
			return this;
		}

		public ModBuilder AppendMetadataBlob(string data)
		{
			this.MetadataBlob += data;
			this._pendingChanges |= ChangeFlags.MetadataBlob;
			return this;
		}

		public ModBuilder SetMetadataKvps(Dictionary<string, string> kvps)
		{
			foreach (KeyValuePair<string, string> keyValuePair in kvps)
			{
				string text;
				string text2;
				keyValuePair.Deconstruct(out text, out text2);
				string key = text;
				string value = text2;
				this.MetadataKvps[key] = value;
			}
			this._pendingChanges |= ChangeFlags.MetadataKvps;
			return this;
		}

		public ModBuilder SetLogo(string logoFilePath)
		{
			this.LogoFilePath = logoFilePath;
			this._pendingChanges |= ChangeFlags.Logo;
			return this;
		}

		public ModBuilder SetLogo(byte[] imageData, ImageFormat format)
		{
			this._logoBytes = imageData;
			this._logoBytesFormat = format;
			this._pendingChanges |= ChangeFlags.Logo;
			return this;
		}

		public ModBuilder SetGallery(ICollection<string> galleryImageFilePaths)
		{
			this.GalleryFilePaths = galleryImageFilePaths.ToArray<string>();
			this._appendingGallery = false;
			this._pendingChanges |= ChangeFlags.Gallery;
			return this;
		}

		public ModBuilder SetGallery(string galleryImageFilePath)
		{
			return this.SetGallery(new string[]
			{
				galleryImageFilePath
			});
		}

		public ModBuilder AppendGallery(ICollection<string> galleryImageFilePaths)
		{
			this.GalleryFilePaths = this.GalleryFilePaths.Concat(galleryImageFilePaths).ToArray<string>();
			this._appendingGallery = true;
			this._pendingChanges |= ChangeFlags.Gallery;
			return this;
		}

		public ModBuilder AppendGallery(string galleryImageFilePath)
		{
			return this.AppendGallery(new string[]
			{
				galleryImageFilePath
			});
		}

		public ModBuilder SetDependencies(ICollection<long> dependencies)
		{
			this.Dependencies = dependencies.ToList<long>();
			this._appendingDependencies = false;
			this._pendingChanges |= ChangeFlags.Dependencies;
			return this;
		}

		public ModBuilder SetDependencies(long dependency)
		{
			return this.SetDependencies(new long[]
			{
				dependency
			});
		}

		public ModBuilder AppendDependencies(ICollection<long> dependencies)
		{
			this.Dependencies = this.Dependencies.Concat(dependencies).ToList<long>();
			this._appendingDependencies = true;
			this._pendingChanges |= ChangeFlags.Dependencies;
			return this;
		}

		public ModBuilder AppendDependencies(long dependency)
		{
			return this.AppendDependencies(new long[]
			{
				dependency
			});
		}

		public ModfileBuilder EditModfile()
		{
			if (this._modfileBuilder == null)
			{
				this._modfileBuilder = new ModfileBuilder(this);
			}
			this._pendingChanges |= ChangeFlags.Modfile;
			return this._modfileBuilder;
		}

		public ModBuilder SetVisible(bool isVisible)
		{
			this.Visible = isVisible;
			this._pendingChanges |= ChangeFlags.Visibility;
			return this;
		}

		public ModBuilder SetMaturityOptions(ModMaturityOptions maturityOptions)
		{
			this.MaturityOptions |= maturityOptions;
			this._pendingChanges |= ChangeFlags.MaturityOptions;
			return this;
		}

		public ModBuilder OverwriteMaturityOptions(ModMaturityOptions maturityOptions)
		{
			this.MaturityOptions = maturityOptions;
			this._pendingChanges |= ChangeFlags.MaturityOptions;
			return this;
		}

		public ModBuilder SetCommunityOptions(ModCommunityOptions communityOptions)
		{
			this.CommunityOptions |= communityOptions;
			this._pendingChanges |= ChangeFlags.CommunityOptions;
			return this;
		}

		public ModBuilder OverwriteCommunityOptions(ModCommunityOptions communityOptions)
		{
			this.CommunityOptions = communityOptions;
			this._pendingChanges |= ChangeFlags.CommunityOptions;
			return this;
		}

		public ModBuilder SetMonetized(bool isMonetized)
		{
			if (isMonetized)
			{
				this._monetizationOptions |= (ModBuilder.MonetizationOptions.Enabled | ModBuilder.MonetizationOptions.Live);
			}
			else
			{
				this._monetizationOptions &= ~(ModBuilder.MonetizationOptions.Enabled | ModBuilder.MonetizationOptions.Live);
			}
			this.IsMonetized = isMonetized;
			this._pendingChanges |= ChangeFlags.MonetizationConfig;
			return this;
		}

		public ModBuilder SetPrice(int price)
		{
			if (!this._monetizationOptions.HasFlag(ModBuilder.MonetizationOptions.Enabled | ModBuilder.MonetizationOptions.Live))
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log("Mod is not set for Monetization! Use SetMonetized(bool isMonetized) before setting a price.");
				}
				return this;
			}
			this.Price = price;
			this._pendingChanges |= ChangeFlags.MonetizationConfig;
			return this;
		}

		public ModBuilder SetLimitedStock(bool isLimitedStock)
		{
			if (isLimitedStock)
			{
				this._monetizationOptions |= ModBuilder.MonetizationOptions.LimitedStock;
			}
			else
			{
				this._monetizationOptions &= ModBuilder.MonetizationOptions.LimitedStock;
			}
			this.IsLimitedStock = isLimitedStock;
			this._pendingChanges |= ChangeFlags.MonetizationConfig;
			return this;
		}

		public ModBuilder SetStockAmount(int stockAmount)
		{
			if (!this._monetizationOptions.HasFlag(ModBuilder.MonetizationOptions.Enabled | ModBuilder.MonetizationOptions.Live | ModBuilder.MonetizationOptions.LimitedStock))
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log("Mod is not set for Monetization or Limited Stock! Use SetMonetized(bool isMonetized) & SetLimtedStock(bool isLimitedStock) before setting a stock value.");
				}
				return this;
			}
			this.Stock = stockAmount;
			this._pendingChanges |= ChangeFlags.MonetizationConfig;
			return this;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"mod"
		})]
		public Task<ValueTuple<Error, Mod>> Publish()
		{
			ModBuilder.<Publish>d__112 <Publish>d__;
			<Publish>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Mod>>.Create();
			<Publish>d__.<>4__this = this;
			<Publish>d__.<>1__state = -1;
			<Publish>d__.<>t__builder.Start<ModBuilder.<Publish>d__112>(ref <Publish>d__);
			return <Publish>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"mod"
		})]
		private Task<ValueTuple<Error, Mod>> PublishNewMod()
		{
			ModBuilder.<PublishNewMod>d__113 <PublishNewMod>d__;
			<PublishNewMod>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Mod>>.Create();
			<PublishNewMod>d__.<>4__this = this;
			<PublishNewMod>d__.<>1__state = -1;
			<PublishNewMod>d__.<>t__builder.Start<ModBuilder.<PublishNewMod>d__113>(ref <PublishNewMod>d__);
			return <PublishNewMod>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"logo"
		})]
		private ValueTuple<Error, ModioAPIFileParameter> TryGetLogoFileParameter()
		{
			ModioAPIFileParameter item = ModioAPIFileParameter.None;
			Error error = Error.None;
			if (!string.IsNullOrEmpty(this.LogoFilePath))
			{
				ValueTuple<Error, ModioAPIFileParameter> valueTuple = ModBuilder.LogoFromFilePath(this.LogoFilePath);
				error = valueTuple.Item1;
				item = valueTuple.Item2;
				if (error)
				{
					ModioLog error2 = ModioLog.Error;
					if (error2 != null)
					{
						error2.Log("Couldn't create Logo file from file path " + this.LogoFilePath + ", cannot publish edits");
					}
				}
			}
			else if (this._logoBytes.Length != 0)
			{
				item = this.LogoFromByteArray();
			}
			else
			{
				ModioLog error3 = ModioLog.Error;
				if (error3 != null)
				{
					error3.Log("Couldn't create Logo file from either source! Cannot publish edits");
				}
				error = new Error(ErrorCode.BAD_PARAMETER);
			}
			return new ValueTuple<Error, ModioAPIFileParameter>(error, item);
		}

		private Task PublishRemainingChanges()
		{
			ModBuilder.<PublishRemainingChanges>d__115 <PublishRemainingChanges>d__;
			<PublishRemainingChanges>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<PublishRemainingChanges>d__.<>4__this = this;
			<PublishRemainingChanges>d__.<>1__state = -1;
			<PublishRemainingChanges>d__.<>t__builder.Start<ModBuilder.<PublishRemainingChanges>d__115>(ref <PublishRemainingChanges>d__);
			return <PublishRemainingChanges>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"mod"
		})]
		private Task<ValueTuple<Error, Mod>> PublishEdits()
		{
			ModBuilder.<PublishEdits>d__116 <PublishEdits>d__;
			<PublishEdits>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Mod>>.Create();
			<PublishEdits>d__.<>4__this = this;
			<PublishEdits>d__.<>1__state = -1;
			<PublishEdits>d__.<>t__builder.Start<ModBuilder.<PublishEdits>d__116>(ref <PublishEdits>d__);
			return <PublishEdits>d__.<>t__builder.Task;
		}

		private Task<Error> PublishGallery()
		{
			ModBuilder.<PublishGallery>d__117 <PublishGallery>d__;
			<PublishGallery>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<PublishGallery>d__.<>4__this = this;
			<PublishGallery>d__.<>1__state = -1;
			<PublishGallery>d__.<>t__builder.Start<ModBuilder.<PublishGallery>d__117>(ref <PublishGallery>d__);
			return <PublishGallery>d__.<>t__builder.Task;
		}

		private Task<Error> PublishMetadataKvps()
		{
			ModBuilder.<PublishMetadataKvps>d__118 <PublishMetadataKvps>d__;
			<PublishMetadataKvps>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<PublishMetadataKvps>d__.<>4__this = this;
			<PublishMetadataKvps>d__.<>1__state = -1;
			<PublishMetadataKvps>d__.<>t__builder.Start<ModBuilder.<PublishMetadataKvps>d__118>(ref <PublishMetadataKvps>d__);
			return <PublishMetadataKvps>d__.<>t__builder.Task;
		}

		private Task<Error> PublishDependencies()
		{
			ModBuilder.<PublishDependencies>d__119 <PublishDependencies>d__;
			<PublishDependencies>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<PublishDependencies>d__.<>4__this = this;
			<PublishDependencies>d__.<>1__state = -1;
			<PublishDependencies>d__.<>t__builder.Start<ModBuilder.<PublishDependencies>d__119>(ref <PublishDependencies>d__);
			return <PublishDependencies>d__.<>t__builder.Task;
		}

		private Task<Error> PublishModfile()
		{
			return this._modfileBuilder.PublishModfile();
		}

		private Task<Error> PublishMonetization()
		{
			ModBuilder.<PublishMonetization>d__121 <PublishMonetization>d__;
			<PublishMonetization>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<PublishMonetization>d__.<>4__this = this;
			<PublishMonetization>d__.<>1__state = -1;
			<PublishMonetization>d__.<>t__builder.Start<ModBuilder.<PublishMonetization>d__121>(ref <PublishMonetization>d__);
			return <PublishMonetization>d__.<>t__builder.Task;
		}

		private Task<Error> GetChangeSpecificPublishTask(ChangeFlags flag)
		{
			if (flag <= ChangeFlags.Visibility)
			{
				if (flag <= ChangeFlags.Tags)
				{
					switch (flag)
					{
					case ChangeFlags.None:
						throw new ArgumentException("None changes?");
					case ChangeFlags.Name:
						throw new ArgumentException(string.Format("{0} should be changed through the Mods endpoint", flag));
					case ChangeFlags.Summary:
						throw new ArgumentException(string.Format("{0} should be changed through the Mods endpoint", flag));
					case ChangeFlags.Name | ChangeFlags.Summary:
					case ChangeFlags.Name | ChangeFlags.Description:
					case ChangeFlags.Summary | ChangeFlags.Description:
					case ChangeFlags.Name | ChangeFlags.Summary | ChangeFlags.Description:
						break;
					case ChangeFlags.Description:
						throw new ArgumentException(string.Format("{0} should be changed through the Mods endpoint", flag));
					case ChangeFlags.Logo:
						throw new ArgumentException(string.Format("{0} should be changed through the Mods endpoint", flag));
					default:
						if (flag == ChangeFlags.Gallery)
						{
							return this.PublishGallery();
						}
						if (flag == ChangeFlags.Tags)
						{
							throw new ArgumentException(string.Format("{0} should be changed through the Mods endpoint", flag));
						}
						break;
					}
				}
				else
				{
					if (flag == ChangeFlags.MetadataBlob)
					{
						throw new ArgumentException(string.Format("{0} should be changed through the Mods endpoint", flag));
					}
					if (flag == ChangeFlags.MetadataKvps)
					{
						return this.PublishMetadataKvps();
					}
					if (flag == ChangeFlags.Visibility)
					{
						throw new ArgumentException(string.Format("{0} should be changed through the Mods endpoint", flag));
					}
				}
			}
			else if (flag <= ChangeFlags.AddFlags)
			{
				if (flag == ChangeFlags.MaturityOptions)
				{
					throw new ArgumentException(string.Format("{0} should be changed through the Mods endpoint", flag));
				}
				if (flag == ChangeFlags.CommunityOptions)
				{
					throw new ArgumentException(string.Format("{0} should be changed through the Mods endpoint", flag));
				}
				if (flag == ChangeFlags.AddFlags)
				{
					throw new ArgumentException(string.Format("{0} should not be gotten from the {1} function! This could result in erroneous data being uploaded!", flag, "GetChangeSpecificPublishTask"));
				}
			}
			else if (flag <= ChangeFlags.MonetizationConfig)
			{
				if (flag == ChangeFlags.Modfile)
				{
					return this.PublishModfile();
				}
				if (flag == ChangeFlags.MonetizationConfig)
				{
					return this.PublishMonetization();
				}
			}
			else
			{
				if (flag == ChangeFlags.EditFlags)
				{
					throw new ArgumentException(string.Format("{0} should not be gotten from the {1} function! This could result in erroneous data being uploaded!", flag, "GetChangeSpecificPublishTask"));
				}
				if (flag == ChangeFlags.Dependencies)
				{
					return this.PublishDependencies();
				}
			}
			throw new ArgumentException(string.Format("Change flag {0} doesn't exist!", flag));
		}

		private static bool ValidateImageFilePath(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log("Image file path " + filePath + " cannot be null or empty!");
				}
				return false;
			}
			string text = Path.GetExtension(filePath).ToLowerInvariant();
			string text2 = Path.GetFileName(filePath).ToLowerInvariant();
			if (string.IsNullOrEmpty(text2))
			{
				ModioLog error2 = ModioLog.Error;
				if (error2 != null)
				{
					error2.Log(string.Concat(new string[]
					{
						"Image file name ",
						text2,
						" from path ",
						filePath,
						" cannot be null or empty!"
					}));
				}
				return false;
			}
			if (text != ".png" && text != ".jpeg" && text != ".jpg")
			{
				ModioLog error3 = ModioLog.Error;
				if (error3 != null)
				{
					error3.Log("Invalid file extension: " + text + ". It must be either a .png, .jpg or .jpeg.");
				}
				return false;
			}
			if (!File.Exists(filePath))
			{
				ModioLog error4 = ModioLog.Error;
				if (error4 != null)
				{
					error4.Log("Image " + filePath + " not found on file system.");
				}
				return false;
			}
			return true;
		}

		private ModioAPIFileParameter LogoFromByteArray()
		{
			return new ModioAPIFileParameter(new MemoryStream(this._logoBytes)
			{
				Position = 0L
			}, string.Format("logo.{0}", this._logoBytesFormat), string.Format("image/{0}", this._logoBytesFormat));
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"file"
		})]
		private static ValueTuple<Error, ModioAPIFileParameter> LogoFromFilePath(string filePath)
		{
			if (!ModBuilder.ValidateImageFilePath(filePath))
			{
				return new ValueTuple<Error, ModioAPIFileParameter>(new Error(ErrorCode.BAD_PARAMETER), default(ModioAPIFileParameter));
			}
			string text = Path.GetExtension(filePath).ToLowerInvariant();
			if (string.IsNullOrEmpty(text))
			{
				return new ValueTuple<Error, ModioAPIFileParameter>(new Error(ErrorCode.BAD_PARAMETER), default(ModioAPIFileParameter));
			}
			Error none = Error.None;
			string name = "logo" + text;
			string str = "image/";
			string text2 = text;
			return new ValueTuple<Error, ModioAPIFileParameter>(none, new ModioAPIFileParameter(name, str + text2.Substring(1, text2.Length - 1), filePath));
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"file"
		})]
		private static Task<ValueTuple<Error, ModioAPIFileParameter>> GalleryZipFromFilePaths(ICollection<string> imageFilePaths)
		{
			ModBuilder.<GalleryZipFromFilePaths>d__126 <GalleryZipFromFilePaths>d__;
			<GalleryZipFromFilePaths>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModioAPIFileParameter>>.Create();
			<GalleryZipFromFilePaths>d__.imageFilePaths = imageFilePaths;
			<GalleryZipFromFilePaths>d__.<>1__state = -1;
			<GalleryZipFromFilePaths>d__.<>t__builder.Start<ModBuilder.<GalleryZipFromFilePaths>d__126>(ref <GalleryZipFromFilePaths>d__);
			return <GalleryZipFromFilePaths>d__.<>t__builder.Task;
		}

		private static string GetExtensionFromFormat(ImageFormat format)
		{
			string result;
			switch (format)
			{
			case ImageFormat.Jpg:
				result = "jpg";
				break;
			case ImageFormat.Jpeg:
				result = "jpeg";
				break;
			case ImageFormat.Png:
				result = "png";
				break;
			default:
				throw new ArgumentException(string.Format("Image format {0} not supported!", format));
			}
			return result;
		}

		private Dictionary<ChangeFlags, Error> _commitErrors;

		private ChangeFlags _pendingChanges;

		private byte[] _logoBytes;

		private ImageFormat _logoBytesFormat;

		private bool _appendingGallery;

		private ModfileBuilder _modfileBuilder;

		private bool _appendingDependencies;

		private ModBuilder.MonetizationOptions _monetizationOptions;

		[Flags]
		private enum MonetizationOptions
		{
			None = 0,
			Enabled = 1,
			Live = 2,
			LimitedStock = 8
		}
	}
}
