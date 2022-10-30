using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    MovementManager movementManager;
    private Tweener tweener;
    private ParticleSystem trailParticles;
    private ParticleSystem bumpParticles;
    private ParticleSystem deadParticles;
    private bool bumped = false;
    private float speed = 2;
    private int lastInput = -1;
    private int currentInput;
    private string[] fishStates = {
        "FishUp",
        "FishLeft",
        "FishDown",
        "FishRight"
    };
    private Vector3[] directions = {
        Vector3.up,
        Vector3.left,
        Vector3.down,
        Vector3.right
    };
    private Quaternion[] particleRotations = {
        Quaternion.Euler(0.0f,0.0f,260.0f),
        Quaternion.Euler(0.0f,0.0f,350.0f),
        Quaternion.Euler(0.0f,0.0f,80.0f),
        Quaternion.Euler(0.0f,0.0f,170.0f)
    };
    public Animator fishAnimator;
    private Vector3 spawnPosition = new Vector3(1,-1,0);
    // Start is called before the first frame update
    public void Initialize(MovementManager mm) {
        movementManager = mm;
    }
    void Start()
    {
        tweener = GetComponent<Tweener>();
        fishAnimator = GetComponent<Animator>();
        trailParticles = GameObject.Find("TrailParticles").GetComponent<ParticleSystem>();
        bumpParticles = GameObject.Find("BumpParticles").GetComponent<ParticleSystem>();
        deadParticles = GameObject.Find("DeadParticles").GetComponent<ParticleSystem>();
        trailParticles.Stop();
        bumpParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if(movementManager.gameManager.isStarted) {
            if(Input.GetKeyDown(KeyCode.W)) {
                lastInput = 0;
                if(movementManager.gameManager.isPaused)
                    movementManager.gameManager.Play();
            }
            if(Input.GetKeyDown(KeyCode.A)) {
                lastInput = 1;
                if(movementManager.gameManager.isPaused)
                    movementManager.gameManager.Play();
            }
            if(Input.GetKeyDown(KeyCode.S)) {
                lastInput = 2;
                if(movementManager.gameManager.isPaused)
                    movementManager.gameManager.Play();
            }
            if(Input.GetKeyDown(KeyCode.D)) {
                lastInput = 3;
                if(movementManager.gameManager.isPaused)
                    movementManager.gameManager.Play();
            }
            
            if(!movementManager.gameManager.isPaused)
                Move();
        }
    }

    void Move() {
        if(!tweener.TweenExists(transform) && lastInput != -1) {            
            Vector3 direction = directions[lastInput];
            int nextTile = movementManager.GetNextTile(transform.position, direction);

            if(nextTile == -1)
                Teleport(direction);
            else {   
                if(movementManager.IsWalkable(nextTile)) {
                    currentInput = lastInput;
                    tweener.AddTween(transform, transform.position, transform.position + direction, 1/speed);
                    fishAnimator.enabled = true;
                    fishAnimator.Play(fishStates[currentInput]);
                    trailParticles.transform.rotation = particleRotations[currentInput];
                    trailParticles.Play();
                    bumped = false;
                    movementManager.gameManager.audioManager.PlayMoveSound();
                } else {
                    direction = directions[currentInput];
                    nextTile = movementManager.GetNextTile(transform.position, direction);
                    if(movementManager.IsWalkable(nextTile)) {
                        tweener.AddTween(transform, transform.position, transform.position + direction, 1/speed);
                        fishAnimator.enabled = true;
                        fishAnimator.Play(fishStates[currentInput]);
                        trailParticles.transform.rotation = particleRotations[currentInput];
                        trailParticles.Play();
                        bumped = false;
                        movementManager.gameManager.audioManager.PlayMoveSound();
                    } else {
                        fishAnimator.enabled = false;
                        trailParticles.Stop();
                        if(!bumped) {
                            bumpParticles.Play();
                            bumped = true;
                            movementManager.gameManager.audioManager.PlayCollisionSound();
                        }
                    }
                }
            }
        }
    }

    void Teleport(Vector3 direction) {
        float newX = Mathf.Abs(transform.position.x - movementManager.gameManager.levelGenerator.width + 1);
        transform.position = new Vector3(newX, transform.position.y, 0.0f);
    }

    void Eat(GameObject pellet) {
        Destroy(pellet);
    }

    void CollectPellet() {
        movementManager.gameManager.levelGenerator.pelletCounter--;
    }

    void OnTriggerEnter2D(Collider2D other) {
        string tag = other.gameObject.tag;
        if(tag == "Pellet") {
            movementManager.gameManager.audioManager.PlayEatSound();
            Eat(other.gameObject);
            CollectPellet();
            AddScore(10);
        } else if(tag == "Cherry") {
            movementManager.cherryController.RemoveCherry(other.transform);
            Eat(other.gameObject);
            AddScore(100);
        } else if(tag == "PowerPellet") {
            Eat(other.gameObject);
            movementManager.ghostController.Scared();
        } else if(tag == "Ghost") {
            if(movementManager.ghostController.CanDie(other.gameObject)) {
                movementManager.ghostController.Die(other.gameObject);
                AddScore(300);
            } else if(movementManager.ghostController.GetState(other.gameObject) != GhostController.GhostState.Dead) {
                Die();
            }
        }
    }

    void AddScore(int x) {
        movementManager.gameManager.AddScore(x);
    }
    
    void Die() {
        movementManager.gameManager.Pause();
        tweener.RemoveTween(transform);
        fishAnimator.Play("FishDead");
        movementManager.gameManager.audioManager.PlayDieSound();
        movementManager.gameManager.uIManager.LoseLife();
        if(movementManager.gameManager.lifes > 0) {
            movementManager.gameManager.isStarted = false;
            deadParticles.Play();
            Invoke("MoveToStart", 1.0f);
        } else {
            movementManager.gameManager.GameOver();
        }
    }

    public void MoveToStart() {
        fishAnimator.Play("FishRight");
        trailParticles.Stop();
        lastInput = -1;
        transform.position = spawnPosition;
        movementManager.gameManager.isStarted = true;
    }
}
