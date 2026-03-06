using System;
using System.IO;
using System.Security.Principal;
using System.Text;
using WebSocketSharp.Net;

namespace WebSocketSharp.Server
{
	public class HttpRequestEventArgs : EventArgs
	{
		internal HttpRequestEventArgs(HttpListenerContext context, string documentRootPath)
		{
			this._context = context;
			this._docRootPath = documentRootPath;
		}

		public HttpListenerRequest Request
		{
			get
			{
				return this._context.Request;
			}
		}

		public HttpListenerResponse Response
		{
			get
			{
				return this._context.Response;
			}
		}

		public IPrincipal User
		{
			get
			{
				return this._context.User;
			}
		}

		private string createFilePath(string childPath)
		{
			childPath = childPath.TrimStart(new char[]
			{
				'/',
				'\\'
			});
			return new StringBuilder(this._docRootPath, 32).AppendFormat("/{0}", childPath).ToString().Replace('\\', '/');
		}

		private static bool tryReadFile(string path, out byte[] contents)
		{
			contents = null;
			bool flag = !File.Exists(path);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				try
				{
					contents = File.ReadAllBytes(path);
				}
				catch
				{
					return false;
				}
				result = true;
			}
			return result;
		}

		public byte[] ReadFile(string path)
		{
			bool flag = path == null;
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			bool flag2 = path.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("An empty string.", "path");
			}
			bool flag3 = path.IndexOf("..") > -1;
			if (flag3)
			{
				throw new ArgumentException("It contains '..'.", "path");
			}
			path = this.createFilePath(path);
			byte[] result;
			HttpRequestEventArgs.tryReadFile(path, out result);
			return result;
		}

		public bool TryReadFile(string path, out byte[] contents)
		{
			bool flag = path == null;
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			bool flag2 = path.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("An empty string.", "path");
			}
			bool flag3 = path.IndexOf("..") > -1;
			if (flag3)
			{
				throw new ArgumentException("It contains '..'.", "path");
			}
			path = this.createFilePath(path);
			return HttpRequestEventArgs.tryReadFile(path, out contents);
		}

		private HttpListenerContext _context;

		private string _docRootPath;
	}
}
