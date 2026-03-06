using System;
using UnityEngine;

public class BufferedAudioStream
{
	public BufferedAudioStream(AudioSource audio)
	{
		this.audioBuffer = new float[12000];
		this.audio = audio;
		audio.loop = true;
		audio.clip = AudioClip.Create("", 12000, 1, 48000, false);
		this.Stop();
	}

	public void Update()
	{
		if (this.remainingBufferTime > 0f)
		{
			if (!this.audio.isPlaying && this.remainingBufferTime > 0.05f)
			{
				this.playbackDelayRemaining -= Time.deltaTime;
				if (this.playbackDelayRemaining <= 0f)
				{
					this.audio.Play();
				}
			}
			if (this.audio.isPlaying)
			{
				this.remainingBufferTime -= Time.deltaTime;
				if (this.remainingBufferTime < 0f)
				{
					this.remainingBufferTime = 0f;
				}
			}
		}
		if (this.remainingBufferTime <= 0f)
		{
			if (this.audio.isPlaying)
			{
				Debug.Log("Buffer empty, stopping " + DateTime.Now.ToString());
				this.Stop();
				return;
			}
			if (this.writePos != 0)
			{
				Debug.LogError("writePos non zero while not playing, how did this happen?");
			}
		}
	}

	private void Stop()
	{
		this.audio.Stop();
		this.audio.time = 0f;
		this.writePos = 0;
		this.playbackDelayRemaining = 0.05f;
	}

	public void AddData(float[] samples)
	{
		int num = samples.Length;
		if (this.writePos > this.audioBuffer.Length)
		{
			throw new Exception();
		}
		for (;;)
		{
			int num2 = num;
			int num3 = this.audioBuffer.Length - this.writePos;
			if (num2 > num3)
			{
				num2 = num3;
			}
			Array.Copy(samples, 0, this.audioBuffer, this.writePos, num2);
			num -= num2;
			this.writePos += num2;
			if (this.writePos > this.audioBuffer.Length)
			{
				break;
			}
			if (this.writePos == this.audioBuffer.Length)
			{
				this.writePos = 0;
			}
			if (num <= 0)
			{
				goto Block_5;
			}
		}
		throw new Exception();
		Block_5:
		this.remainingBufferTime += (float)samples.Length / 48000f;
		this.audio.clip.SetData(this.audioBuffer, 0);
	}

	private const bool VerboseLogging = false;

	private AudioSource audio;

	private float[] audioBuffer;

	private int writePos;

	private const float bufferLengthSeconds = 0.25f;

	private const int sampleRate = 48000;

	private const int bufferSize = 12000;

	private const float playbackDelayTimeSeconds = 0.05f;

	private float playbackDelayRemaining;

	private float remainingBufferTime;
}
