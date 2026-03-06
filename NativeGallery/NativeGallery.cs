using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Liv.NativeGalleryBridge
{
	public static class NativeGallery
	{
		public static NativeGallery.Permission CheckPermission(NativeGallery.PermissionType permissionType, NativeGallery.MediaType mediaTypes)
		{
			return NativeGallery.Permission.Granted;
		}

		public static NativeGallery.Permission RequestPermission(NativeGallery.PermissionType permissionType, NativeGallery.MediaType mediaTypes)
		{
			NativeGallery.CheckPermission(permissionType, mediaTypes);
			return NativeGallery.Permission.Granted;
		}

		public static void RequestPermissionAsync(NativeGallery.PermissionCallback callback, NativeGallery.PermissionType permissionType, NativeGallery.MediaType mediaTypes)
		{
			callback(NativeGallery.Permission.Granted);
		}

		public static Task<NativeGallery.Permission> RequestPermissionAsync(NativeGallery.PermissionType permissionType, NativeGallery.MediaType mediaTypes)
		{
			TaskCompletionSource<NativeGallery.Permission> tcs = new TaskCompletionSource<NativeGallery.Permission>();
			NativeGallery.RequestPermissionAsync(delegate(NativeGallery.Permission permission)
			{
				tcs.SetResult(permission);
			}, permissionType, mediaTypes);
			return tcs.Task;
		}

		private static NativeGallery.Permission ProcessPermission(NativeGallery.Permission permission)
		{
			if (permission != (NativeGallery.Permission)3)
			{
				return permission;
			}
			return NativeGallery.Permission.Granted;
		}

		public static Task<NativeGallery.Permission> SaveVideoToGallery(byte[] mediaBytes, string album, string filename, NativeGallery.MediaSaveCallback callback = null)
		{
			NativeGallery.<SaveVideoToGallery>d__12 <SaveVideoToGallery>d__;
			<SaveVideoToGallery>d__.<>t__builder = AsyncTaskMethodBuilder<NativeGallery.Permission>.Create();
			<SaveVideoToGallery>d__.mediaBytes = mediaBytes;
			<SaveVideoToGallery>d__.album = album;
			<SaveVideoToGallery>d__.filename = filename;
			<SaveVideoToGallery>d__.callback = callback;
			<SaveVideoToGallery>d__.<>1__state = -1;
			<SaveVideoToGallery>d__.<>t__builder.Start<NativeGallery.<SaveVideoToGallery>d__12>(ref <SaveVideoToGallery>d__);
			return <SaveVideoToGallery>d__.<>t__builder.Task;
		}

		public static Task<NativeGallery.Permission> SaveVideoToGallery(string existingMediaPath, string album, string filename, NativeGallery.MediaSaveCallback callback = null)
		{
			NativeGallery.<SaveVideoToGallery>d__13 <SaveVideoToGallery>d__;
			<SaveVideoToGallery>d__.<>t__builder = AsyncTaskMethodBuilder<NativeGallery.Permission>.Create();
			<SaveVideoToGallery>d__.existingMediaPath = existingMediaPath;
			<SaveVideoToGallery>d__.album = album;
			<SaveVideoToGallery>d__.filename = filename;
			<SaveVideoToGallery>d__.callback = callback;
			<SaveVideoToGallery>d__.<>1__state = -1;
			<SaveVideoToGallery>d__.<>t__builder.Start<NativeGallery.<SaveVideoToGallery>d__13>(ref <SaveVideoToGallery>d__);
			return <SaveVideoToGallery>d__.<>t__builder.Task;
		}

		public static Task<NativeGallery.Permission> SaveImageToGallery(string existingMediaPath, string album, string filename, NativeGallery.MediaSaveCallback callback = null)
		{
			NativeGallery.<SaveImageToGallery>d__14 <SaveImageToGallery>d__;
			<SaveImageToGallery>d__.<>t__builder = AsyncTaskMethodBuilder<NativeGallery.Permission>.Create();
			<SaveImageToGallery>d__.existingMediaPath = existingMediaPath;
			<SaveImageToGallery>d__.album = album;
			<SaveImageToGallery>d__.filename = filename;
			<SaveImageToGallery>d__.callback = callback;
			<SaveImageToGallery>d__.<>1__state = -1;
			<SaveImageToGallery>d__.<>t__builder.Start<NativeGallery.<SaveImageToGallery>d__14>(ref <SaveImageToGallery>d__);
			return <SaveImageToGallery>d__.<>t__builder.Task;
		}

		public static string GetExternalStoragePublicDirectory()
		{
			return "";
		}

		private static Task<NativeGallery.Permission> SaveToGallery(byte[] mediaBytes, string album, string filename, NativeGallery.MediaType mediaType, NativeGallery.MediaSaveCallback callback)
		{
			NativeGallery.<SaveToGallery>d__16 <SaveToGallery>d__;
			<SaveToGallery>d__.<>t__builder = AsyncTaskMethodBuilder<NativeGallery.Permission>.Create();
			<SaveToGallery>d__.mediaBytes = mediaBytes;
			<SaveToGallery>d__.album = album;
			<SaveToGallery>d__.filename = filename;
			<SaveToGallery>d__.mediaType = mediaType;
			<SaveToGallery>d__.callback = callback;
			<SaveToGallery>d__.<>1__state = -1;
			<SaveToGallery>d__.<>t__builder.Start<NativeGallery.<SaveToGallery>d__16>(ref <SaveToGallery>d__);
			return <SaveToGallery>d__.<>t__builder.Task;
		}

		private static Task<NativeGallery.Permission> SaveToGallery(string existingMediaPath, string album, string filename, NativeGallery.MediaType mediaType, NativeGallery.MediaSaveCallback callback)
		{
			NativeGallery.<SaveToGallery>d__17 <SaveToGallery>d__;
			<SaveToGallery>d__.<>t__builder = AsyncTaskMethodBuilder<NativeGallery.Permission>.Create();
			<SaveToGallery>d__.existingMediaPath = existingMediaPath;
			<SaveToGallery>d__.album = album;
			<SaveToGallery>d__.filename = filename;
			<SaveToGallery>d__.mediaType = mediaType;
			<SaveToGallery>d__.callback = callback;
			<SaveToGallery>d__.<>1__state = -1;
			<SaveToGallery>d__.<>t__builder.Start<NativeGallery.<SaveToGallery>d__17>(ref <SaveToGallery>d__);
			return <SaveToGallery>d__.<>t__builder.Task;
		}

		private static void SaveToGalleryInternal(string path, string album, NativeGallery.MediaType mediaType, NativeGallery.MediaSaveCallback callback)
		{
			if (callback != null)
			{
				callback(true, null);
			}
		}

		private static string GetTemporarySavePath(string filename)
		{
			string text = Path.Combine(Application.persistentDataPath, "NGallery");
			Directory.CreateDirectory(text);
			return Path.Combine(text, filename);
		}

		private const bool PermissionFreeMode = true;

		public enum PermissionType
		{
			Read,
			Write
		}

		public enum Permission
		{
			Denied,
			Granted,
			ShouldAsk
		}

		[Flags]
		public enum MediaType
		{
			Video = 2,
			Image = 4
		}

		public delegate void PermissionCallback(NativeGallery.Permission permission);

		public delegate void MediaSaveCallback(bool success, string path);

		public delegate void MediaPickCallback(string path);
	}
}
