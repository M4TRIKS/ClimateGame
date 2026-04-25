using UnityEngine;

public static class OneShotAudio
{
    public static void Play(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
}