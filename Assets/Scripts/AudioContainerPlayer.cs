using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioContainerPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioContainer audioContainer;

    private int currentRepeatNumber;
    private uint playCount;
    private AudioClip currentClip;

    private AudioClip nextClip;
    private AudioContainer nextContainer;

    private bool clipStillPlaying;
    private bool playNextClip;

    private AudioClip[] tempClipArray;
    private float[] tempWeightingArray;

    //This attaches to a gameobject and talks to the audio source on it. Just sets and triggers audio clip playback.
    //Feature requests: Fades. Blend container.

    //External function to call to start playback.
    public void Play()
    {
        //Check for clip or container
        switch (audioContainer.containerType)
        {
            case AudioContainer.ContainerType.Clips:
                StartCoroutine(PlayClips());
                break;
            case AudioContainer.ContainerType.Containers:
                //PlayContainers(); 
                break;
        }

        //Containers

        //Check if playing whole list & if so, load list in

        //if container, recurse through whole upper script (but hang on to data(?))

        //recursing through code.
    }

    //External function to call to stop playback.
    public void Stop()
    {
        audioSource.Stop();
    }

    private IEnumerator PlayClips()
    {
        playNextClip = false;
        //Check playback pattern
        switch (audioContainer.pattern)
        {
            case AudioContainer.Pattern.Random:
                //Find weighting distribution
                float maxWeight = 0f;
                for (int i = currentRepeatNumber; i < audioContainer.audioClips.Length; i++)
                {
                    maxWeight += audioContainer.playbackWeighting[i];
                }
                
                int tempMarker = 0; //Something wrong with the way this is interacting.

                float minWeight = 0f;
                for (int i = 0; i < currentRepeatNumber; i++)
                {
                    minWeight += audioContainer.playbackWeighting[i];
                }
                
                //Select from weighting
                float playbackRoll = Random.Range(minWeight, maxWeight + minWeight);
                
                for (int i = currentRepeatNumber; i < audioContainer.audioClips.Length; i++)
                {
                    minWeight += audioContainer.playbackWeighting[i];
                    if (minWeight >= playbackRoll)
                    {
                        //Load next clip
                        currentClip = audioContainer.audioClips[i];
                        //currentRepeatNumber++;
                        tempMarker = i;
                        break;
                    }
                }
                
                //Move recently played to the front of the list for random clips & random weighting values.
                tempClipArray = audioContainer.audioClips;
                tempWeightingArray = audioContainer.playbackWeighting;

                tempClipArray[0] = audioContainer.audioClips[tempMarker];
                tempWeightingArray[0] = audioContainer.playbackWeighting[tempMarker];

                for (int i = 0; i < tempMarker; i++) //Might be tempMarker - 1 ~~Test this
                {
                    tempClipArray[i + 1] = audioContainer.audioClips[i];
                    tempWeightingArray[i + 1] = audioContainer.playbackWeighting[i];
                }

                audioContainer.audioClips = tempClipArray;
                audioContainer.playbackWeighting = tempWeightingArray;
                break;
            case AudioContainer.Pattern.Sequential:
                //Load current and next clip
                if (nextClip == null)
                    currentClip = audioContainer.audioClips[0];
                else
                    currentClip = nextClip; //Make sure we assign nextClip at some point.

                if(playCount < audioContainer.audioClips.Length)
                {
                    nextClip = audioContainer.audioClips[playCount + 1];
                    playCount++;
                }
                else
                {
                    playCount = 0;
                    nextClip = audioContainer.audioClips[0];
                }
                break;
        }

        //Check if we're just looping one clip with no delay for perfect looping.
        if (audioContainer.repeat && !audioContainer.playWholeList && Mathf.Approximately(audioContainer.maxWaitBeforePlaying,0f))
            audioSource.loop = true;
        else
            audioSource.loop = false;

        //Wait before playing
        yield return new WaitForSeconds(Random.Range(audioContainer.minWaitBeforePlaying, audioContainer.maxWaitBeforePlaying));

        //Actually play the current clip
        audioSource.clip = currentClip;
        audioSource.Play();

        if (currentRepeatNumber < audioContainer.avoidRepeatedPlaybackCount && currentRepeatNumber < audioContainer.audioClips.Length - 1)
            currentRepeatNumber++;

        if ((audioContainer.repeat || audioContainer.playWholeList) && !audioSource.isPlaying)
        {
            //End clip wait
            yield return new WaitForSeconds(Random.Range(audioContainer.minWaitAfterPlaying, audioContainer.maxWaitAfterPlaying));

            //If playing whole list, call next
            if(audioContainer.playWholeList)
            {
                //If at the end, call first if also repeating
                if (audioContainer.repeat && (currentClip.name == audioContainer.audioClips[audioContainer.audioClips.Length - 1].name))
                {
                    nextClip = audioContainer.audioClips[0];
                    playNextClip = true;
                }
                else
                {
                    //Might be able to simplify this
                    for (int i = 0; i < audioContainer.audioClips.Length; i++)
                    {
                        if(audioContainer.audioClips[i].name == currentClip.name)
                        {
                            nextClip = audioContainer.audioClips[i + 1];
                            playNextClip = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                //If just repeating, call next
                if (audioContainer.repeat)
                {
                    for (int i = 0; i < audioContainer.audioClips.Length; i++)
                    {
                        if (audioContainer.audioClips[i].name == currentClip.name)
                        {
                            nextClip = audioContainer.audioClips[i + 1];
                            playNextClip = true;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void Awake()
    {
        clipStillPlaying = false;
        playNextClip = false;
    }

    private void Update()
    {
        clipStillPlaying = audioSource.isPlaying;
        if(playNextClip)
        {
            PlayClips();
        }
    }
}
