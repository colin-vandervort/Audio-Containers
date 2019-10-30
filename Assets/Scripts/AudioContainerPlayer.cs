using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioContainerPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioContainer audioContainer;

    private uint[] playedOrder;
    private AudioClip currentClip;

    private AudioClip nextClip;
    private AudioContainer nextContainer;

    //This attaches to a gameobject and talks to the audio source on it. Just sets and triggers audio clip playback.
    //Feature requests: Fades. Blend container.

    //Starts the audio container playback
    public void Play()
    {
        switch (audioContainer.containerType)
        {
            case AudioContainer.ContainerType.Clips:
                PlayClips();
                break;
            case AudioContainer.ContainerType.Containers:
                PlayContainers();
                break;
        }

        double sampleRate = GetSampleRate(currentClip);
        audioSource.Play();//Overload is sample delay. At 44100, 44100 = 1 second delay.
    }

    //Stops the audio container playback
    public void Stop()
    {
        audioSource.Stop();
    }

    private void PlayClips()
    {
        switch (audioContainer.pattern)
        {
            case AudioContainer.Pattern.Random:
                if (audioContainer.playWholeList) //Check if we're playing whole list and queue a 'play next'
                {

                }
                break;
            case AudioContainer.Pattern.Sequential:
                if (audioContainer.playWholeList) //Check if we're playing whole list and queue a 'play next'
                {

                }
                break;
        }
    }

    private void PlayContainers()
    {
        switch (audioContainer.pattern)
        {
            case AudioContainer.Pattern.Random:
                if (audioContainer.playWholeList) //Check if we're playing whole list and queue a 'play next'
                {

                }
                break;
            case AudioContainer.Pattern.Sequential:
                if (audioContainer.playWholeList) //Check if we're playing whole list and queue a 'play next'
                {

                }
                break;
        }
    }

    private void PlayNextClip()
    {
        
    }

    private void PlayNextContainer()
    {

    }

    private double GetSampleRate(AudioClip clip)
    {
        return clip.samples / clip.length;
    }
}
