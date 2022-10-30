using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public MovementManager movementManager;
    [SerializeField]
    public AudioManager audioManager;
    [SerializeField]
    public LevelGenerator levelGenerator;
    [SerializeField]
    public UIManager uIManager;
    private int score = 0;
    public int lifes = 3;
    public bool isStarted = false;
    public bool isPaused = true;
    // Start is called before the first frame update
    void Awake() {
        movementManager.Initialize(this);
        audioManager.Initialize(this);
        levelGenerator.Initialize(this);
        uIManager.Initialize(this);
    }
    void Start()
    {
        StartCoroutine(StartGame());
    }

    // Update is called once per frame
    void Update()
    {
        if(levelGenerator.pelletCounter <= 0)
            Win();
    }

    IEnumerator StartGame() {
        lifes = 3;
        uIManager.Reset();
        audioManager.PlayStartMusic();
        // Wait two more seconds so start music has time to finish
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(uIManager.CountDown(3));
        audioManager.PlayNormalMusic();
        uIManager.StartTimer();
        Play();
    }

    public void AddScore(int x) {
        score += x;
        uIManager.SetScore(score);
    }

    public void Pause() {
        isPaused = true;
        movementManager.cherryController.StopCherrySpawn();
    }

    public void Stop() {
        Pause();
        isStarted = false;
    }

    public void Play() {
        isStarted = true;
        isPaused = false;
        movementManager.cherryController.StartCherrySpawn();
    }

    public void GameOver() {
        Stop();
        movementManager.pacStudentController.fishAnimator.enabled = false;
        SaveHighScore();
        uIManager.ShowGameOver();
        Invoke("ShowStartScene", 3);
    }

    public void Win() {
        Stop();
        movementManager.pacStudentController.fishAnimator.enabled = false;
        SaveHighScore();
        uIManager.ShowWin();
        Invoke("ShowStartScene", 3);
    }

    public void SaveHighScore() {
        int currentHighScore = PlayerPrefs.GetInt("highscore", 0);
        float currentTime = PlayerPrefs.GetFloat("highscoreTime", 0.0f);

        if(score > currentHighScore) {
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.SetFloat("highscoreTime", uIManager.GetTimePlayed());
        } else if(score == currentHighScore) {
            if(currentTime > uIManager.GetTimePlayed()) {
                PlayerPrefs.SetInt("highscore", score);
                PlayerPrefs.SetFloat("highscoreTime", uIManager.GetTimePlayed());
            }
        }
    }

    public void ShowStartScene() {
        SceneManager.LoadScene(0);
    }
}
