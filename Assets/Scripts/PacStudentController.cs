using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    MovementManager movementManager;
    private Tweener tweener;
    private ParticleSystem trailParticles;
    private ParticleSystem bumpParticles;
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
    private Animator fishAnimator;
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
        trailParticles.Stop();
        bumpParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("current:" + currentInput);
        // Debug.Log("last:" + lastInput);
        if(Input.GetKeyDown(KeyCode.W))
            lastInput = 0;
        if(Input.GetKeyDown(KeyCode.A))
            lastInput = 1;
        if(Input.GetKeyDown(KeyCode.S))
            lastInput = 2;
        if(Input.GetKeyDown(KeyCode.D))
            lastInput = 3;

        Move();
    }

    void Move() {
        if(!tweener.TweenExists(transform) && lastInput != -1) {            
            Vector3 direction = directions[lastInput];
            int nextTile = movementManager.GetNextTile(transform.position, direction);

            if(nextTile == -1)
                Teleport(direction);
            else {   
                if(movementManager.isWalkable(nextTile)) {
                    currentInput = lastInput;
                    tweener.AddTween(transform, transform.position, transform.position + direction, 1/speed);
                    fishAnimator.enabled = true;
                    fishAnimator.Play(fishStates[currentInput]);
                    trailParticles.transform.rotation = particleRotations[currentInput];
                    trailParticles.Play();
                    bumped = false;
                } else {
                    direction = directions[currentInput];
                    nextTile = movementManager.GetNextTile(transform.position, direction);
                    if(movementManager.isWalkable(nextTile)) {
                        tweener.AddTween(transform, transform.position, transform.position + direction, 1/speed);
                        fishAnimator.enabled = true;
                        fishAnimator.Play(fishStates[currentInput]);
                        trailParticles.transform.rotation = particleRotations[currentInput];
                        trailParticles.Play();
                        bumped = false;
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

    void OnTriggerEnter2D(Collider2D other) {
        string tag = other.gameObject.tag;
        if(tag == "Pellet") {
            Eat(other.gameObject);
            AddScore(10);
        } else if(tag == "Cherry") {
            movementManager.cherryController.RemoveCherry(other.transform);
            Eat(other.gameObject);
            AddScore(100);
        } else if(tag == "PowerPellet") {
            Eat(other.gameObject);
            movementManager.ghostController.Scared();
        }
    }

    void AddScore(int x) {
        movementManager.gameManager.AddScore(x);
    }
}
