using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Modio.Errors
{
	public class ErrorException : Error
	{
		public override string GetMessage()
		{
			return string.Format("{0}: {1}", base.GetMessage(), this.Exception);
		}

		internal ErrorException(Exception exception, ErrorCode code) : base(code)
		{
			this.Exception = exception;
		}

		internal ErrorException(Exception exception) : base(ErrorException.ErrorCodeFromException(exception))
		{
			this.Exception = exception;
		}

		private static ErrorCode ErrorCodeFromException(Exception exception)
		{
			ErrorCode result;
			if (!(exception is UnauthorizedAccessException))
			{
				if (!(exception is DirectoryNotFoundException))
				{
					if (!(exception is FileNotFoundException))
					{
						if (!(exception is HttpRequestException))
						{
							if (!(exception is TaskCanceledException))
							{
								result = ErrorCode.OPERATION_ERROR;
							}
							else
							{
								result = ErrorCode.OPERATION_CANCELLED;
							}
						}
						else
						{
							result = ErrorCode.HTTP_EXCEPTION;
						}
					}
					else
					{
						result = ErrorCode.FILE_NOT_FOUND;
					}
				}
				else
				{
					result = ErrorCode.DIRECTORY_NOT_FOUND;
				}
			}
			else
			{
				result = ErrorCode.NO_PERMISSION;
			}
			return result;
		}

		public readonly Exception Exception;
	}
}
