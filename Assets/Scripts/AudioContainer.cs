using UnityEngine;

[System.Serializable]
public class AudioContainer : ScriptableObject
{
    public enum ContainerType
    {
        Clips, Containers
    };

    public enum Pattern
    {
        Random, Sequential
    };

    public string containerName;             //The name of this container.
    public ContainerType containerType;      //Toggle between holding a sequence of containers and a sequence of clips

    public AudioClip[] audioClips;           //Hold individual audio clips to place in the list

    public AudioContainer[] audioContainers; //Hold a list of other containers

    public Pattern pattern;                  //Select the playback pattern of the list
    public bool playWholeList;               //If true, play through the whole list, rather than a single selection
    public bool repeat;                      //If true, repeat the playing of a clip selection or list sequence
    public int avoidRepeatedPlaybackCount;   //The number of audio clips that need to be played from the list before any given clip can be played again

    public float[] playbackWeighting;        //The relative weighting of playback for each option.

    public float minWaitBeforePlaying;       //The fewest number of seconds to wait before a sound or container is played
    public float maxWaitBeforePlaying;       //The greatest number of seconds to wait before a sound or container is played
    public float minWaitAfterPlaying;        //The fewest number of seconds to wait after a sound or container is played
    public float maxWaitAfterPlaying;        //The greatest number of seconds to wait after a sound or container is played
}
