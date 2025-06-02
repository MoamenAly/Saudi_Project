using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;

    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(WaitSound());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator WaitSound()
    {
        yield return new WaitUntil(() => !audioSource.isPlaying);
        player.GetComponent<CharacterController>().enabled = true;
    }
}
