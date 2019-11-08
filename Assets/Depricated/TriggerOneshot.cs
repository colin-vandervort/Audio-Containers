using UnityEngine;

public class TriggerOneshot : MonoBehaviour
{
    public AudioContainerPlayer audioContainerPlayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            audioContainerPlayer.Play();
        
        if (Input.GetKeyDown(KeyCode.S))
            audioContainerPlayer.Stop();
    }
}
