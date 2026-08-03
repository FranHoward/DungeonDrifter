using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudioEmitter : MonoBehaviour
{
    private AudioSource audioSource;
    private double releaseTime = double.PositiveInfinity;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip clip, float pitch)
    {
        if (clip == null)
            return;

        audioSource.pitch = Mathf.Clamp(pitch, -3f, 3f);
        audioSource.PlayOneShot(clip);
    }

    public void PlayDeathAndRelease(AudioClip clip, float pitch)
    {
        if (clip == null)
            return;

        transform.SetParent(null, true);
        PlayOneShot(clip, pitch);

        float playbackSpeed = Mathf.Max(0.01f, Mathf.Abs(audioSource.pitch));
        releaseTime = AudioSettings.dspTime + clip.length / playbackSpeed + 0.1f;
    }

    private void Update()
    {
        if (AudioSettings.dspTime >= releaseTime)
            Destroy(gameObject);
    }
}
