using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource startMusic;
    [SerializeField]
    private AudioSource normalMusic;
    bool start = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > startMusic.clip.length && start) {
            startMusic.Stop();
            normalMusic.Play();
            start = false;
        }
    }
}
