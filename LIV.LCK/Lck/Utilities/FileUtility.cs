using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck.Settings;

namespace Liv.Lck.Utilities
{
	public static class FileUtility
	{
		public static bool IsFileLocked(string filePath)
		{
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			}
			catch (IOException)
			{
				return true;
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
			return false;
		}

		public static Task CopyToGallery(string sourceFilePath, string albumName, Action<bool, string> callback)
		{
			FileUtility.<CopyToGallery>d__1 <CopyToGallery>d__;
			<CopyToGallery>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CopyToGallery>d__.sourceFilePath = sourceFilePath;
			<CopyToGallery>d__.albumName = albumName;
			<CopyToGallery>d__.callback = callback;
			<CopyToGallery>d__.<>1__state = -1;
			<CopyToGallery>d__.<>t__builder.Start<FileUtility.<CopyToGallery>d__1>(ref <CopyToGallery>d__);
			return <CopyToGallery>d__.<>t__builder.Task;
		}

		public static string GenerateFilename(string extension)
		{
			string text = DateTime.Now.ToString(LckSettings.Instance.RecordingDateSuffixFormat);
			return string.Concat(new string[]
			{
				LckSettings.Instance.RecordingFilenamePrefix,
				"_",
				text,
				".",
				extension
			});
		}

		private static Task DeleteAllFilesWithSameExtensionAsync(string filePath)
		{
			FileUtility.<DeleteAllFilesWithSameExtensionAsync>d__3 <DeleteAllFilesWithSameExtensionAsync>d__;
			<DeleteAllFilesWithSameExtensionAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DeleteAllFilesWithSameExtensionAsync>d__.filePath = filePath;
			<DeleteAllFilesWithSameExtensionAsync>d__.<>1__state = -1;
			<DeleteAllFilesWithSameExtensionAsync>d__.<>t__builder.Start<FileUtility.<DeleteAllFilesWithSameExtensionAsync>d__3>(ref <DeleteAllFilesWithSameExtensionAsync>d__);
			return <DeleteAllFilesWithSameExtensionAsync>d__.<>t__builder.Task;
		}
	}
}
