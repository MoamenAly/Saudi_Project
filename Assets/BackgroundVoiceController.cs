using UnityEngine;

public class BackgroundVoiceController : MonoBehaviour
{
    AudioSource backgroundVoice; // Assign the AudioSource in the inspector
    string playerTag = "Player"; // Tag of the player object (set this in your Unity Editor)


    private void Start()
    {
        backgroundVoice = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has entered the area (you can use the player's tag to identify the player)
        if (other.CompareTag(playerTag))
        {
            // Play the background voice
            if (!backgroundVoice.isPlaying)
            {
                backgroundVoice.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player has exited the area
        if (other.CompareTag(playerTag))
        {
            // Stop the background voice
            backgroundVoice.Stop();
        }
    }
}
