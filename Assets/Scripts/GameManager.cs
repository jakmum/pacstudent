using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
    }

    IEnumerator StartGame() {
        lifes = 3;
        uIManager.Reset();
        movementManager.pacStudentController.MoveToStart();
        audioManager.PlayStartMusic();
        yield return StartCoroutine(uIManager.CountDown(3));
        audioManager.PlayNormalMusic();
        Play();
    }

    public void AddScore(int x) {
        score += x;
    }

    public void Pause() {
        isPaused = true;
        movementManager.cherryController.StopCherrySpawn();
    }

    public void Play() {
        isStarted = true;
        isPaused = false;
        movementManager.cherryController.StartCherrySpawn();
    }
}
