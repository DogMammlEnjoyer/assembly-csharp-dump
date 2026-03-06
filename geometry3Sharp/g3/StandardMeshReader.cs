using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace g3
{
	public class StandardMeshReader
	{
		public event ParsingMessagesHandler warningEvent;

		public IMeshBuilder MeshBuilder { get; set; }

		public StandardMeshReader(bool bIncludeDefaultReaders = true)
		{
			this.Readers = new List<MeshFormatReader>();
			this.MeshBuilder = new DMesh3Builder();
			if (bIncludeDefaultReaders)
			{
				this.Readers.Add(new OBJFormatReader());
				this.Readers.Add(new STLFormatReader());
				this.Readers.Add(new OFFFormatReader());
				this.Readers.Add(new BinaryG3FormatReader());
				this.Readers.Add(new DS3FormatReader());
				this.Readers.Add(new PLYFormatReader());
			}
		}

		public bool SupportsFormat(string sExtension)
		{
			foreach (MeshFormatReader meshFormatReader in this.Readers)
			{
				using (List<string>.Enumerator enumerator2 = meshFormatReader.SupportedExtensions.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.Equals(sExtension, StringComparison.OrdinalIgnoreCase))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public void AddFormatHandler(MeshFormatReader reader)
		{
			foreach (string text in reader.SupportedExtensions)
			{
				if (this.SupportsFormat(text))
				{
					throw new Exception("StandardMeshReader.AddFormatHandler: format " + text + " is already registered!");
				}
			}
			this.Readers.Add(reader);
		}

		public IOReadResult Read(string sFilename, ReadOptions options)
		{
			if (this.MeshBuilder == null)
			{
				return new IOReadResult(IOCode.GenericReaderError, "MeshBuilder is null!");
			}
			string text = Path.GetExtension(sFilename);
			if (text.Length < 2)
			{
				return new IOReadResult(IOCode.InvalidFilenameError, "filename " + sFilename + " does not contain valid extension");
			}
			text = text.Substring(1);
			MeshFormatReader meshFormatReader = null;
			foreach (MeshFormatReader meshFormatReader2 in this.Readers)
			{
				using (List<string>.Enumerator enumerator2 = meshFormatReader2.SupportedExtensions.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.Equals(text, StringComparison.OrdinalIgnoreCase))
						{
							meshFormatReader = meshFormatReader2;
						}
					}
				}
				if (meshFormatReader != null)
				{
					break;
				}
			}
			if (meshFormatReader == null)
			{
				return new IOReadResult(IOCode.UnknownFormatError, "format " + text + " is not supported");
			}
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			IOReadResult result;
			try
			{
				if (this.ReadInvariantCulture)
				{
					Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
				}
				IOReadResult ioreadResult = meshFormatReader.ReadFile(sFilename, this.MeshBuilder, options, new ParsingMessagesHandler(this.on_warning));
				if (this.ReadInvariantCulture)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
				result = ioreadResult;
			}
			catch (Exception ex)
			{
				if (this.ReadInvariantCulture)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
				result = new IOReadResult(IOCode.GenericReaderError, "Unknown error : exception : " + ex.Message);
			}
			return result;
		}

		public IOReadResult Read(Stream stream, string sExtension, ReadOptions options)
		{
			if (this.MeshBuilder == null)
			{
				return new IOReadResult(IOCode.GenericReaderError, "MeshBuilder is null!");
			}
			MeshFormatReader meshFormatReader = null;
			foreach (MeshFormatReader meshFormatReader2 in this.Readers)
			{
				using (List<string>.Enumerator enumerator2 = meshFormatReader2.SupportedExtensions.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.Equals(sExtension, StringComparison.OrdinalIgnoreCase))
						{
							meshFormatReader = meshFormatReader2;
						}
					}
				}
				if (meshFormatReader != null)
				{
					break;
				}
			}
			if (meshFormatReader == null)
			{
				return new IOReadResult(IOCode.UnknownFormatError, "format " + sExtension + " is not supported");
			}
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			IOReadResult result;
			try
			{
				if (this.ReadInvariantCulture)
				{
					Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
				}
				IOReadResult ioreadResult = meshFormatReader.ReadFile(stream, this.MeshBuilder, options, new ParsingMessagesHandler(this.on_warning));
				if (this.ReadInvariantCulture)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
				result = ioreadResult;
			}
			catch (Exception ex)
			{
				if (this.ReadInvariantCulture)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
				result = new IOReadResult(IOCode.GenericReaderError, "Unknown error : exception : " + ex.Message);
			}
			return result;
		}

		public static IOReadResult ReadFile(string sFilename, ReadOptions options, IMeshBuilder builder)
		{
			return new StandardMeshReader(true)
			{
				MeshBuilder = builder
			}.Read(sFilename, options);
		}

		public static IOReadResult ReadFile(Stream stream, string sExtension, ReadOptions options, IMeshBuilder builder)
		{
			return new StandardMeshReader(true)
			{
				MeshBuilder = builder
			}.Read(stream, sExtension, options);
		}

		public static DMesh3 ReadMesh(string sFilename)
		{
			DMesh3Builder dmesh3Builder = new DMesh3Builder();
			if (StandardMeshReader.ReadFile(sFilename, ReadOptions.Defaults, dmesh3Builder).code != IOCode.Ok)
			{
				return null;
			}
			return dmesh3Builder.Meshes[0];
		}

		public static DMesh3 ReadMesh(Stream stream, string sExtension)
		{
			DMesh3Builder dmesh3Builder = new DMesh3Builder();
			if (StandardMeshReader.ReadFile(stream, sExtension, ReadOptions.Defaults, dmesh3Builder).code != IOCode.Ok)
			{
				return null;
			}
			return dmesh3Builder.Meshes[0];
		}

		private void on_warning(string message, object extra_data)
		{
			if (this.warningEvent != null)
			{
				this.warningEvent(message, extra_data);
			}
		}

		public bool ReadInvariantCulture = true;

		private List<MeshFormatReader> Readers = new List<MeshFormatReader>();
	}
}
