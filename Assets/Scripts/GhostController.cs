using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    MovementManager movementManager;
    [SerializeField]
    List<GameObject> ghosts;
    public bool scared = false;
    bool recovering = false;
    bool dead = false;
    float scaredTimer;
    float deadTimer;

    public void Initialize(MovementManager mm) {
        movementManager = mm;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(scared) {
            movementManager.gameManager.uIManager.SetGhostTimer(scaredTimer);
            if(scaredTimer > 0) {
                scaredTimer -= Time.deltaTime;
                if(scaredTimer <= 3.0f && !recovering) {
                    Recover();
                }
            } else {
                Normal();
            }
        }
        if(dead) {
            if(deadTimer > 0) {
                deadTimer -= Time.deltaTime;
            } else {
                Normal();
            }
        }
    }

    public void Normal() {
        scared = false;
        recovering = false;
        dead = false;
        foreach(GameObject ghost in ghosts) {
            Animator animator = ghost.GetComponent<Animator>();
            animator.Play("GhostAnimator_Left");
        }
        movementManager.gameManager.audioManager.PlayNormalMusic();
        movementManager.gameManager.uIManager.DisableGhostTimer();
    }
    public void Scared() {
        scared = true;
        scaredTimer = 10.0f;
        foreach(GameObject ghost in ghosts) {
            Animator animator = ghost.GetComponent<Animator>();
            animator.Play("GhostAnimator_Scared");
        }
        movementManager.gameManager.audioManager.PlayScaredMusic();
    }

    public void Recover() {
        recovering = true;
        foreach(GameObject ghost in ghosts) {
            Animator animator = ghost.GetComponent<Animator>();
            animator.Play("GhostAnimator_Recovering");
        }
    }

    public void Die(GameObject ghost) {
        dead = true;
        deadTimer = 5.0f;
        Animator animator = ghost.GetComponent<Animator>();
        animator.Play("GhostAnimator_Dead");
        movementManager.gameManager.audioManager.PlayDeadMusic();
    }
}
