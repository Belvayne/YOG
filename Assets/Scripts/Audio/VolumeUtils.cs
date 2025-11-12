// VolumeUtils.cs
using UnityEngine;

public static class VolumeUtils
{
    // value in [0..1] -> dB for AudioMixer
    public static float LinearToDecibels(float value, float muteFloorDb = -80f)
    {
        if (value <= 0.0001f) return muteFloorDb;
        return Mathf.Log10(value) * 20f;
    }

    // dB -> [0..1] (useful if you ever read back from mixer)
    public static float DecibelsToLinear(float dB)
    {
        return Mathf.Pow(10f, dB / 20f);
    }
}
