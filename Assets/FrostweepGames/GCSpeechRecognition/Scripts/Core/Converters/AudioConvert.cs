using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public static class AudioConvert
    {
        public static string Convert(AudioClip clip, Enumerators.AudioEncoding encoding, bool increaseVolume = false, float volume = 1f)
        {
            byte[] audioArray;

            switch(encoding)
            {
                case Enumerators.AudioEncoding.LINEAR16:
                    {
                        if (increaseVolume)
                            clip.SetData(AudioClip2ByteConverter.ByteToFloat(
                                         AudioClip2ByteConverter.AudioClipToByte(clip, increaseVolume, volume)), 0);

                        AudioClip2PCMConverter.AudioClip2PCM(clip, out audioArray);
                    }
                    break;
                default:
                    throw new System.NotSupportedException(encoding + " doesn't supported for converting!");
            }

            return System.Convert.ToBase64String(audioArray);
        }
    }
}