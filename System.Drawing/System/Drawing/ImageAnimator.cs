using System;
using System.Collections;
using System.Drawing.Imaging;
using System.Threading;

namespace System.Drawing
{
	/// <summary>Animates an image that has time-based frames.</summary>
	public sealed class ImageAnimator
	{
		private ImageAnimator()
		{
		}

		/// <summary>Displays a multiple-frame image as an animation.</summary>
		/// <param name="image">The <see cref="T:System.Drawing.Image" /> object to animate.</param>
		/// <param name="onFrameChangedHandler">An <see langword="EventHandler" /> object that specifies the method that is called when the animation frame changes.</param>
		public static void Animate(Image image, EventHandler onFrameChangedHandler)
		{
			if (!ImageAnimator.CanAnimate(image))
			{
				return;
			}
			if (ImageAnimator.ht.ContainsKey(image))
			{
				return;
			}
			byte[] value = image.GetPropertyItem(20736).Value;
			int[] array = new int[value.Length >> 2];
			int i = 0;
			int num = 0;
			while (i < value.Length)
			{
				int num2 = BitConverter.ToInt32(value, i) * 10;
				array[num] = ((num2 < 100) ? 100 : num2);
				i += 4;
				num++;
			}
			AnimateEventArgs animateEventArgs = new AnimateEventArgs(image);
			Thread thread = new Thread(new ThreadStart(new WorkerThread(onFrameChangedHandler, animateEventArgs, array).LoopHandler));
			thread.IsBackground = true;
			animateEventArgs.RunThread = thread;
			ImageAnimator.ht.Add(image, animateEventArgs);
			thread.Start();
		}

		/// <summary>Returns a Boolean value indicating whether the specified image contains time-based frames.</summary>
		/// <param name="image">The <see cref="T:System.Drawing.Image" /> object to test.</param>
		/// <returns>This method returns <see langword="true" /> if the specified image contains time-based frames; otherwise, <see langword="false" />.</returns>
		public static bool CanAnimate(Image image)
		{
			if (image == null)
			{
				return false;
			}
			int num = image.FrameDimensionsList.Length;
			if (num < 1)
			{
				return false;
			}
			for (int i = 0; i < num; i++)
			{
				if (image.FrameDimensionsList[i].Equals(FrameDimension.Time.Guid))
				{
					return image.GetFrameCount(FrameDimension.Time) > 1;
				}
			}
			return false;
		}

		/// <summary>Terminates a running animation.</summary>
		/// <param name="image">The <see cref="T:System.Drawing.Image" /> object to stop animating.</param>
		/// <param name="onFrameChangedHandler">An <see langword="EventHandler" /> object that specifies the method that is called when the animation frame changes.</param>
		public static void StopAnimate(Image image, EventHandler onFrameChangedHandler)
		{
			if (image == null)
			{
				return;
			}
			if (ImageAnimator.ht.ContainsKey(image))
			{
				((AnimateEventArgs)ImageAnimator.ht[image]).RunThread.Abort();
				ImageAnimator.ht.Remove(image);
			}
		}

		/// <summary>Advances the frame in all images currently being animated. The new frame is drawn the next time the image is rendered.</summary>
		public static void UpdateFrames()
		{
			foreach (object obj in ImageAnimator.ht.Keys)
			{
				ImageAnimator.UpdateImageFrame((Image)obj);
			}
		}

		/// <summary>Advances the frame in the specified image. The new frame is drawn the next time the image is rendered. This method applies only to images with time-based frames.</summary>
		/// <param name="image">The <see cref="T:System.Drawing.Image" /> object for which to update frames.</param>
		public static void UpdateFrames(Image image)
		{
			if (image == null)
			{
				return;
			}
			if (ImageAnimator.ht.ContainsKey(image))
			{
				ImageAnimator.UpdateImageFrame(image);
			}
		}

		private static void UpdateImageFrame(Image image)
		{
			AnimateEventArgs animateEventArgs = (AnimateEventArgs)ImageAnimator.ht[image];
			image.SelectActiveFrame(FrameDimension.Time, animateEventArgs.GetNextFrame());
		}

		private static Hashtable ht = Hashtable.Synchronized(new Hashtable());
	}
}
