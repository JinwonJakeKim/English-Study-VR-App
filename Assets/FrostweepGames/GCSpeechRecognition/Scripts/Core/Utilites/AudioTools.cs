using UnityEngine;

public static class AudioTools
{
    public static float[] IncreaseVolume(float[] samples, float volumeMultiplier)
    {
        float dB = (Mathf.Log(volumeMultiplier) / Mathf.Log(10.0f) * 20.0f);

        float amplitude = Mathf.Pow(10, (dB / 20f));

        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] *= amplitude;
        }

        return samples;
    }
}
