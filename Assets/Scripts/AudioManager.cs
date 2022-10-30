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
        PlayMusic();
    }

    void PlayMusic() {
        if(!gameManager.isStarted) {
            PlayStartMusic();
        } else {
            if(gameManager.movementManager.ghostController.GhostIsDead()) {
                PlayDeadMusic();
            } else if(gameManager.movementManager.ghostController.globalState == GhostController.GhostState.Scared || gameManager.movementManager.ghostController.globalState == GhostController.GhostState.Recovering) {
                PlayScaredMusic();
            } else {
                PlayNormalMusic();
            }
        }
    }

    public void PlayCollisionSound() {
        if(!collideClip.isPlaying)
            collideClip.Play();
    }

    public void PlayDieSound() {
        if(!dieClip.isPlaying)
            dieClip.Play();
    }

    public void PlayEatSound() {
        if(!eatClip.isPlaying)
            eatClip.Play();
    }

    public void PlayMoveSound() {
        if(!moveClip.isPlaying)
            moveClip.Play();
    }

    public void PlayStartMusic() {
        if(!startMusic.isPlaying) {
            playingMusic.Stop();
            startMusic.Play();
            playingMusic = startMusic;
        }
    }

    public void PlayScaredMusic() {
        if(!scaredMusic.isPlaying) {
            playingMusic.Stop();
            scaredMusic.Play();
            playingMusic = scaredMusic;
        }
    }

    public void PlayNormalMusic() {
        if(!normalMusic.isPlaying) {
            playingMusic.Stop();
            normalMusic.Play();
            playingMusic = normalMusic;
        }
    }
    public void PlayDeadMusic() {
        if(!deadMusic.isPlaying) {
            playingMusic.Stop();
            deadMusic.Play();
            playingMusic = deadMusic;
        }
    }
}
