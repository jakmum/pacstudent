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
    private AudioSource playingMusic;

    bool start = true;
    // Start is called before the first frame update
    public void Initialize(GameManager gm) {
        gameManager = gm;
        playingMusic = startMusic;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCollisionSound() {
        collideClip.Play();
    }

    public void PlayStartMusic() {
        playingMusic.Stop();
        startMusic.Play();
        playingMusic = startMusic;
    }

    public void PlayScaredMusic() {
        playingMusic.Stop();
        scaredMusic.Play();
        playingMusic = scaredMusic;
    }

    public void PlayNormalMusic() {
        playingMusic.Stop();
        normalMusic.Play();
        playingMusic = normalMusic;
    }
    public void PlayDeadMusic() {
        playingMusic.Stop();
        deadMusic.Play();
        playingMusic = deadMusic;
    }
}
