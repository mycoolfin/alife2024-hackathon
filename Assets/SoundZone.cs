using UnityEngine;
using UnityEngine.Audio;

public class SoundZone : MonoBehaviour
{
    public bool debug = false;

    private static readonly float[] pentatonicPitches = {
        1.0f,      // Root (C)
        1.122f,    // Major Second (D)
        1.260f,    // Major Third (E)
        1.498f,    // Perfect Fifth (G)
        1.682f,    // Major Sixth (A)
        2.0f,      // Octave (C)
        2.244f,    // Major Ninth (D)
        2.520f,    // Major Tenth (E)
        2.996f,    // Perfect Twelfth (G)
        3.364f     // Major Thirteenth (A)
    };

    private AudioSource audioSource;
    private AudioChorusFilter chorusFilter;

    private bool activated = false;

    private float volumeMultiplier = 1f;
    private float pitchMultiplier = 1f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
        chorusFilter = gameObject.AddComponent<AudioChorusFilter>();
        chorusFilter.depth = 0f;
        chorusFilter.rate = 0f;
    }

    void Update()
    {
        if (activated)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 1f, Time.deltaTime) * volumeMultiplier;
            if (!audioSource.isPlaying)
            {
                if (debug) GetComponent<MeshRenderer>().enabled = true;
                audioSource.Play();
                audioSource.loop = true;
            }
            activated = false;
        }
        else
        {
            if (debug) GetComponent<MeshRenderer>().enabled = false;
            audioSource.loop = false;
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime * 10f) * volumeMultiplier;
        }
    }

    public void SetMixerGroup(AudioMixerGroup mixerGroup)
    {
        audioSource.outputAudioMixerGroup = mixerGroup;
    }

    public void SetAudioClip(AudioClip clip)
    {
        audioSource.clip = clip;
    }

    public void SetVolumeMultiplier(float volumeMultiplier)
    {
        this.volumeMultiplier = volumeMultiplier;
    }

    public void SetPitchMultiplier(float pitchMultiplier)
    {
        this.pitchMultiplier = pitchMultiplier;
    }

    public void SetPitch(int pitch)
    {
        audioSource.pitch = pentatonicPitches[pitch] * pitchMultiplier;
    }

    public void SetChorusDepth(int chorusDepth)
    {
        chorusFilter.depth = chorusDepth / 10f;
    }

    public void SetChorusRate(int chorusRate)
    {
        chorusFilter.rate = chorusRate;
    }

    public void Activate()
    {
        activated = true;
    }
}
