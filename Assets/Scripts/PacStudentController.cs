using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    MovementManager movementManager;
    private Tweener tweener;
    private new ParticleSystem particleSystem;
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
    void Start()
    {
        movementManager = GameObject.FindGameObjectWithTag("MovementManager").GetComponent<MovementManager>();
        tweener = GetComponent<Tweener>();
        fishAnimator = GetComponent<Animator>();
        particleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
        particleSystem.Stop();
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
        if(!tweener.isTweening() && lastInput != -1) {
            Vector3 direction = directions[lastInput];
            int nextTile = movementManager.GetNextTile(transform.position, direction);
            
            if(movementManager.isWalkable(nextTile)) {
                currentInput = lastInput;
                tweener.AddTween(transform, transform.position, transform.position + direction, 1/speed);
                fishAnimator.enabled = true;
                fishAnimator.Play(fishStates[currentInput]);
                particleSystem.transform.rotation = particleRotations[currentInput];
                particleSystem.Play();
            } else {
                direction = directions[currentInput];
                nextTile = movementManager.GetNextTile(transform.position, direction);
                if(movementManager.isWalkable(nextTile)) {
                    tweener.AddTween(transform, transform.position, transform.position + direction, 1/speed);
                    fishAnimator.enabled = true;
                    fishAnimator.Play(fishStates[currentInput]);
                    particleSystem.transform.rotation = particleRotations[currentInput];
                    particleSystem.Play();
                } else {
                    fishAnimator.enabled = false;
                    particleSystem.Stop();
                }
            }
        }
    }

}
