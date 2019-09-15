using System;
using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public static class AudioClip2ByteConverter
    {
        public static byte[] AudioClipToByte(AudioClip clip, bool increaseVolume = false, float volumeMultiplier = 1f)
        {
            var samples = new float[clip.samples];

            clip.GetData(samples, 0);

            if (increaseVolume)
            {
                samples = AudioTools.IncreaseVolume(samples, volumeMultiplier);
            }

            return FloatToByte(samples);
        }

        public static byte[] FloatToByte(float[] samples)
        {
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

            return bytesData;
        }

        public static float[] ByteToFloat(byte[] bytesData)
        {
            int rescaleFactor = 32767;

            float[] samples = new float[bytesData.Length / 2];

            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = (float)BitConverter.ToInt16(bytesData, i * 2) / rescaleFactor;
            }

            return samples;
        }
    }
}