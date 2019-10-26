using UnityEngine;

public class TriggerOneshot : MonoBehaviour
{
    public AudioSource audioSource;

    void Update()
    {
       if( Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.Play();
        }
    }
}
