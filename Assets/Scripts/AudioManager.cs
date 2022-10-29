using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField]
    private AudioSource startMusic;
    [SerializeField]
    private AudioSource normalMusic;
    [SerializeField]
    private AudioSource deadMusic;
    [SerializeField]
    private AudioSource scaredMusic;
    [SerializeField]
    private AudioSource collideClip;
    [SerializeField]
    private AudioSource dieClip;
    [SerializeField]
    private AudioSource eatClip;
    [SerializeField]
    private AudioSource moveClip;

    bool start = true;
    // Start is called before the first frame update
    public void Initialize(GameManager gm) {
        gameManager = gm;
    }
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
