using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public static class AudioClip2PCMConverter
    {
        private const int HEADER_SIZE = 44;

        public static void AudioClip2PCM(AudioClip clip, out byte[] bytes)
        {
            MemoryStream stream = new MemoryStream();
            using (var fileStream = CreateEmpty(stream))
            {

                ConvertAndWrite(fileStream, clip);

                WriteHeader(fileStream, clip);

                var data = fileStream.ToArray();
                bytes = data;
            }
        }

        private static AudioClip TrimSilence(AudioClip clip, float min)
        {
            var samples = new float[clip.samples];

            clip.GetData(samples, 0);

            return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);
        }

        private static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz)
        {
            return TrimSilence(samples, min, channels, hz, false);
        }

        private static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool stream)
        {
            int i;

            for (i = 0; i < samples.Count; i++)
            {
                if (Mathf.Abs(samples[i]) > min)
                {
                    break;
                }
            }

            samples.RemoveRange(0, i);

            for (i = samples.Count - 1; i > 0; i--)
            {
                if (Mathf.Abs(samples[i]) > min)
                {
                    break;
                }
            }

            samples.RemoveRange(i, samples.Count - i);

            var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, stream);

            clip.SetData(samples.ToArray(), 0);

            return clip;
        }

        private static MemoryStream CreateEmpty(MemoryStream mstream)
        {
            byte emptyByte = new byte();

            for (int i = 0; i < HEADER_SIZE; i++)
            {
                mstream.WriteByte(emptyByte);
            }

            return mstream;
        }

        private static void ConvertAndWrite(MemoryStream mstream, AudioClip clip)
        {
            var samples = new float[clip.samples];

            clip.GetData(samples, 0);

            Int16[] intData = new Int16[samples.Length];

            Byte[] bytesData = new Byte[samples.Length * 2];

            int rescaleFactor = 32767;

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(samples[i] * rescaleFactor);
                Byte[] byteArr = new Byte[2];
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            mstream.Write(bytesData, 0, bytesData.Length);
        }

        private static void WriteHeader(MemoryStream mstream, AudioClip clip)
        {
            var hz = clip.frequency;
            var channels = clip.channels;
            var samples = clip.samples;

            mstream.Seek(0, SeekOrigin.Begin);

            Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            mstream.Write(riff, 0, 4);

            Byte[] chunkSize = BitConverter.GetBytes(mstream.Length - 8);
            mstream.Write(chunkSize, 0, 4);

            Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            mstream.Write(wave, 0, 4);

            Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            mstream.Write(fmt, 0, 4);

            Byte[] subChunk1 = BitConverter.GetBytes(16);
            mstream.Write(subChunk1, 0, 4);

            UInt16 one = 1;

            Byte[] audioFormat = BitConverter.GetBytes(one);
            mstream.Write(audioFormat, 0, 2);

            Byte[] numChannels = BitConverter.GetBytes(channels);
            mstream.Write(numChannels, 0, 2);

            Byte[] sampleRate = BitConverter.GetBytes(hz);
            mstream.Write(sampleRate, 0, 4);

            Byte[] byteRate = BitConverter.GetBytes(hz * 2);
            mstream.Write(byteRate, 0, 4);

            UInt16 blockAlign = (ushort)(2);
            mstream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

            UInt16 bps = 16;
            Byte[] bitsPerSample = BitConverter.GetBytes(bps);
            mstream.Write(bitsPerSample, 0, 2);

            Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
            mstream.Write(datastring, 0, 4);

            Byte[] subChunk2 = BitConverter.GetBytes(samples * 2);
            mstream.Write(subChunk2, 0, 4);
        }
    }
}