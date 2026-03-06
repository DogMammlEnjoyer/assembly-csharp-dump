using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace g3
{
	public class StandardMeshWriter : IDisposable
	{
		public void Dispose()
		{
		}

		public static IOWriteResult WriteMeshes(string sFilename, List<DMesh3> vMeshes, WriteOptions options)
		{
			List<WriteMesh> list = new List<WriteMesh>();
			foreach (DMesh3 mesh in vMeshes)
			{
				list.Add(new WriteMesh(mesh, ""));
			}
			return new StandardMeshWriter().Write(sFilename, list, options);
		}

		public static IOWriteResult WriteFile(string sFilename, List<WriteMesh> vMeshes, WriteOptions options)
		{
			return new StandardMeshWriter().Write(sFilename, vMeshes, options);
		}

		public static IOWriteResult WriteMesh(string sFilename, IMesh mesh, WriteOptions options)
		{
			return new StandardMeshWriter().Write(sFilename, new List<WriteMesh>
			{
				new WriteMesh(mesh, "")
			}, options);
		}

		public IOWriteResult Write(string sFilename, List<WriteMesh> vMeshes, WriteOptions options)
		{
			Func<string, List<WriteMesh>, WriteOptions, IOWriteResult> func = null;
			string extension = Path.GetExtension(sFilename);
			if (extension.Equals(".obj", StringComparison.OrdinalIgnoreCase))
			{
				func = new Func<string, List<WriteMesh>, WriteOptions, IOWriteResult>(this.Write_OBJ);
			}
			else if (extension.Equals(".stl", StringComparison.OrdinalIgnoreCase))
			{
				func = new Func<string, List<WriteMesh>, WriteOptions, IOWriteResult>(this.Write_STL);
			}
			else if (extension.Equals(".off", StringComparison.OrdinalIgnoreCase))
			{
				func = new Func<string, List<WriteMesh>, WriteOptions, IOWriteResult>(this.Write_OFF);
			}
			else if (extension.Equals(".g3mesh", StringComparison.OrdinalIgnoreCase))
			{
				func = new Func<string, List<WriteMesh>, WriteOptions, IOWriteResult>(this.Write_G3Mesh);
			}
			if (func == null)
			{
				return new IOWriteResult(IOCode.UnknownFormatError, "format " + extension + " is not supported");
			}
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			IOWriteResult result;
			try
			{
				if (this.WriteInvariantCulture)
				{
					Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
				}
				IOWriteResult iowriteResult = func(sFilename, vMeshes, options);
				if (this.WriteInvariantCulture)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
				result = iowriteResult;
			}
			catch (Exception ex)
			{
				if (this.WriteInvariantCulture)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
				result = new IOWriteResult(IOCode.WriterError, "Unknown error : exception : " + ex.Message);
			}
			return result;
		}

		private IOWriteResult Write_OBJ(string sFilename, List<WriteMesh> vMeshes, WriteOptions options)
		{
			Stream stream = this.OpenStreamF(sFilename);
			if (stream == null)
			{
				return new IOWriteResult(IOCode.FileAccessError, "Could not open file " + sFilename + " for writing");
			}
			IOWriteResult result;
			try
			{
				StreamWriter streamWriter = new StreamWriter(stream);
				IOWriteResult iowriteResult = new OBJWriter
				{
					OpenStreamF = this.OpenStreamF,
					CloseStreamF = this.CloseStreamF
				}.Write(streamWriter, vMeshes, options);
				streamWriter.Flush();
				result = iowriteResult;
			}
			finally
			{
				this.CloseStreamF(stream);
			}
			return result;
		}

		private IOWriteResult Write_OFF(string sFilename, List<WriteMesh> vMeshes, WriteOptions options)
		{
			Stream stream = this.OpenStreamF(sFilename);
			if (stream == null)
			{
				return new IOWriteResult(IOCode.FileAccessError, "Could not open file " + sFilename + " for writing");
			}
			IOWriteResult result;
			try
			{
				StreamWriter streamWriter = new StreamWriter(stream);
				IOWriteResult iowriteResult = new OFFWriter().Write(streamWriter, vMeshes, options);
				streamWriter.Flush();
				result = iowriteResult;
			}
			finally
			{
				this.CloseStreamF(stream);
			}
			return result;
		}

		private IOWriteResult Write_STL(string sFilename, List<WriteMesh> vMeshes, WriteOptions options)
		{
			Stream stream = this.OpenStreamF(sFilename);
			if (stream == null)
			{
				return new IOWriteResult(IOCode.FileAccessError, "Could not open file " + sFilename + " for writing");
			}
			IOWriteResult result;
			try
			{
				if (options.bWriteBinary)
				{
					BinaryWriter binaryWriter = new BinaryWriter(stream);
					IOWriteResult iowriteResult = new STLWriter().Write(binaryWriter, vMeshes, options);
					binaryWriter.Flush();
					result = iowriteResult;
				}
				else
				{
					StreamWriter streamWriter = new StreamWriter(stream);
					IOWriteResult iowriteResult2 = new STLWriter().Write(streamWriter, vMeshes, options);
					streamWriter.Flush();
					result = iowriteResult2;
				}
			}
			finally
			{
				this.CloseStreamF(stream);
			}
			return result;
		}

		private IOWriteResult Write_G3Mesh(string sFilename, List<WriteMesh> vMeshes, WriteOptions options)
		{
			Stream stream = this.OpenStreamF(sFilename);
			if (stream == null)
			{
				return new IOWriteResult(IOCode.FileAccessError, "Could not open file " + sFilename + " for writing");
			}
			IOWriteResult result;
			try
			{
				BinaryWriter binaryWriter = new BinaryWriter(stream);
				IOWriteResult iowriteResult = new BinaryG3Writer().Write(binaryWriter, vMeshes, options);
				binaryWriter.Flush();
				result = iowriteResult;
			}
			finally
			{
				this.CloseStreamF(stream);
			}
			return result;
		}

		public bool WriteInvariantCulture = true;

		public Func<string, Stream> OpenStreamF = (string sFilename) => File.Open(sFilename, FileMode.Create);

		public Action<Stream> CloseStreamF = delegate(Stream stream)
		{
			stream.Close();
			stream.Dispose();
		};
	}
}
