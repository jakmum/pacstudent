using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public enum GhostState {Normal, Scared, Recovering, Dead};
    MovementManager movementManager;
    Tweener tweener;
    [SerializeField]
    List<GameObject> ghosts;
    GhostState[] ghostStates = {   
        GhostState.Normal,
        GhostState.Normal,
        GhostState.Normal,
        GhostState.Normal
    };
    int[] ghostOrigin = {-1,-1,-1,-1};
    Vector3 spawnPosition = new Vector3(14.0f, -14.0f, 0.0f);
    float[] deadTimer = {0,0,0,0};
    public GhostState globalState = GhostState.Normal;
    float scaredTimer;
    float speed = 2.0f;

    public void Initialize(MovementManager mm) {
        movementManager = mm;
    }
    // Start is called before the first frame update
    void Start()
    {
        tweener = GetComponent<Tweener>();
    }

    // Update is called once per frame
    void Update()
    {
        if(movementManager.gameManager.isStarted) {
            if(globalState == GhostState.Scared || globalState == GhostState.Recovering) {
                movementManager.gameManager.uIManager.SetGhostTimer(scaredTimer);
                if(scaredTimer > 0) {
                    scaredTimer -= Time.deltaTime;
                    if(scaredTimer <= 3.0f && globalState == GhostState.Scared) {
                        Recover();
                    }
                } else {
                    Normal();
                }
            }

            for(int i = 0; i < ghosts.Count; i++) {
                if(ghostStates[i] == GhostState.Dead) {
                    if(InStartArea(ghosts[i].transform.position) || deadTimer[i] <= 0)
                        Resurrect(i);
                    else {
                        deadTimer[i] -= Time.deltaTime;
                        DeadMovement(i);
                    }
                } else if(InStartArea(ghosts[i].transform.position)) {
                    LeaveStartArea(i);
                } else if(ghostStates[i] == GhostState.Scared
                    || ghostStates[i] == GhostState.Recovering) {
                        GhostOneMovement(i);
                } else {
                    if(i == 0)
                        GhostOneMovement(i);
                    else if(i == 1)
                        GhostTwoMovement(i);
                    else if(i == 2)
                        GhostThreeMovement(i);
                    else if(i == 3)
                        GhostFourMovement(i);
                }
            }
        }
    }

    public void Normal() {
        globalState = GhostState.Normal;
        ChangeAllStates(GhostState.Normal);
        foreach(GameObject ghost in ghosts) {
            Animator animator = ghost.GetComponent<Animator>();
            animator.Play("GhostAnimator_Left");
        }
        movementManager.gameManager.uIManager.DisableGhostTimer();
    }

    public void Scared() {
        globalState = GhostState.Scared;
        ChangeAllStates(GhostState.Scared);
        scaredTimer = 10.0f;
        foreach(GameObject ghost in ghosts) {
            Animator animator = ghost.GetComponent<Animator>();
            animator.Play("GhostAnimator_Scared");
        }
    }

    public void Recover() {
        globalState = GhostState.Recovering;
        ChangeAllStates(GhostState.Recovering);
        for(int i = 0; i < ghosts.Count; i++) {
            Animator animator = ghosts[i].GetComponent<Animator>();
            if(ghostStates[i] == GhostState.Recovering)
                animator.Play("GhostAnimator_Recovering");
        }
    }

    public void Die(GameObject ghost) {
        int index = ghosts.IndexOf(ghost);
        ghostStates[index] = GhostState.Dead;
        deadTimer[index] = 5.0f;
        Animator animator = ghost.GetComponent<Animator>();
        animator.Play("GhostAnimator_Dead");
    }

    void Resurrect(int index) {
        deadTimer[index] = 0;
        ghostStates[index] = globalState;
    }

    public bool GhostIsDead() {
        foreach(GhostState state in ghostStates) {
            if(state == GhostState.Dead)
                return true;
        }
        return false;
    }

    public GhostState GetState(GameObject ghost) {
        int index = ghosts.IndexOf(ghost);
        return ghostStates[index];
    }

    bool InStartArea(Vector3 pos) {
        return (pos.x > 9
            && pos.x < 18
            && pos.y > -17
            && pos.y < -11);
    }

    public bool CanDie(GameObject ghost) {
        int index = ghosts.IndexOf(ghost);
        return (ghostStates[index] == GhostState.Scared 
            || ghostStates[index] == GhostState.Recovering);
    }

    void ChangeAllStates(GhostState newState) {
        for(int i = 0; i < ghostStates.Length; i++) {
            if(!(ghostStates[i] == GhostState.Dead && deadTimer[i] > 0))
                ghostStates[i] = newState;
        }
    }

    Vector3 GetNextPosition(Vector3 pos, int neighborID) {
        if(neighborID == 0) {
            return pos + Vector3.left;
        } else if(neighborID == 1) {
            return pos + Vector3.up;
        } else if(neighborID == 2) {
            return pos + Vector3.right;
        } else {
            return pos + Vector3.down;
        }
    }

    bool IsWalkable(int tileNumber, Vector3 pos) {
        return tileNumber != -1 
            && movementManager.IsWalkable(tileNumber)
            && !InStartArea(pos);
    }

    void MoveGhost(int index, Vector3 target, int origin) {
        ghostOrigin[index] = origin;
        tweener.AddTween(ghosts[index].transform, ghosts[index].transform.position, target, 1/speed);
    }

    void LeaveStartArea(int index) {
        // First go to center and then either up or down
        // depending on which ghost
        Vector3 currentPos = ghosts[index].transform.position;
        if(currentPos.x < 13)
            MoveGhost(index, ghosts[index].transform.position + Vector3.right, 0);
        else if(currentPos.x > 14)
            MoveGhost(index, ghosts[index].transform.position + Vector3.left, 2);
        else {
            // ghost 1 move down
            if(index == 0)
                MoveGhost(index, ghosts[index].transform.position + Vector3.down, 1);
            // ghost 2 move up
            else if(index == 1)
                MoveGhost(index, ghosts[index].transform.position + Vector3.up, 3);
            // ghost 3+4 move randomly up or down
            else {
                if(ghostOrigin[index] == 1)
                    MoveGhost(index, ghosts[index].transform.position + Vector3.down, 1);
                else if(ghostOrigin[index] == 3)
                    MoveGhost(index, ghosts[index].transform.position + Vector3.up, 3);
                else {
                    float random = Random.Range(0,1);
                    if(random > 0.5f)
                        MoveGhost(index, ghosts[index].transform.position + Vector3.up, 3);
                    else
                        MoveGhost(index, ghosts[index].transform.position + Vector3.down, 1);
                }
            }
        }

    }

    void GhostOneMovement(int index) {
        if(!tweener.TweenExists(ghosts[index].transform)) {
            //0:left; 1:top; 2:right; 3:bottom
            int[] neighbors = movementManager.gameManager.levelGenerator.GetNeighbors((int)ghosts[index].transform.position.x, -(int)ghosts[index].transform.position.y);
            int previousTile = ghostOrigin[index];
            Vector3 nextPosition;
            
            // Check if one of neighboring Tiles fulfills 
            // condition of greater distance and walk there if so
            // Start at random index and loop through all neigboring tiles
            int start = Random.Range(0,4);
            for(int i = 0; i < 4; i++) {
                int neighborIndex = (i+start)%4;
                if(neighborIndex != previousTile) {
                    int neighborTile = neighbors[neighborIndex];
                    nextPosition = GetNextPosition(ghosts[index].transform.position, neighborIndex);
                    if(IsWalkable(neighborTile, nextPosition)) {
                        Vector3 pacPosition = movementManager.pacStudentController.transform.position;
                        float distance = Vector3.Distance(ghosts[index].transform.position, pacPosition);
                        if(distance <= Vector3.Distance(nextPosition, pacPosition)) {
                            MoveGhost(index, nextPosition, (neighborIndex+2)%4);
                            return;
                        }
                    }
                }
            }

            // Check if ghost can walk anywhere else without walking back
            // else walk back
            for(int i = 0; i < 4; i++) {
                int neighborIndex = (i+previousTile+1)%4;
                int neighborTile = neighbors[neighborIndex];
                nextPosition = GetNextPosition(ghosts[index].transform.position, neighborIndex);
                if(IsWalkable(neighborTile, nextPosition)) {
                    MoveGhost(index, nextPosition, (neighborIndex+2)%4);
                    return;
                }
            }

        }
    }

    void GhostTwoMovement(int index) {
        if(!tweener.TweenExists(ghosts[index].transform)) {
            //0:left; 1:top; 2:right; 3:bottom
            int[] neighbors = movementManager.gameManager.levelGenerator.GetNeighbors((int)ghosts[index].transform.position.x, -(int)ghosts[index].transform.position.y);
            int previousTile = ghostOrigin[index];
            Vector3 nextPosition;
            
            // Check if one of neighboring Tiles fulfills 
            // condition of greater distance and walk there if so
            // Start at random index and loop through all neigboring tiles
            int start = Random.Range(0,4);
            for(int i = 0; i < 4; i++) {
                int neighborIndex = (i+start)%4;
                if(neighborIndex != previousTile) {
                    int neighborTile = neighbors[neighborIndex];
                    nextPosition = GetNextPosition(ghosts[index].transform.position, neighborIndex);
                    if(IsWalkable(neighborTile, nextPosition)) {
                        Vector3 pacPosition = movementManager.pacStudentController.transform.position;
                        float distance = Vector3.Distance(ghosts[index].transform.position, pacPosition);
                        if(distance >= Vector3.Distance(nextPosition, pacPosition)) {
                            MoveGhost(index, nextPosition, (neighborIndex+2)%4);
                            return;
                        }
                    }
                }
            }

            // Check if ghost can walk anywhere else without walking back
            // else walk back
            for(int i = 0; i < 4; i++) {
                int neighborIndex = (i+previousTile+1)%4;
                int neighborTile = neighbors[neighborIndex];
                nextPosition = GetNextPosition(ghosts[index].transform.position, neighborIndex);
                if(IsWalkable(neighborTile, nextPosition)) {
                    MoveGhost(index, nextPosition, (neighborIndex+2)%4);
                    return;
                }
            }

        }
    }

    void GhostThreeMovement(int index) {
        if(!tweener.TweenExists(ghosts[index].transform)) {
            //0:left; 1:top; 2:right; 3:bottom
            int[] neighbors = movementManager.gameManager.levelGenerator.GetNeighbors((int)ghosts[index].transform.position.x, -(int)ghosts[index].transform.position.y);
            int previousTile = ghostOrigin[index];
            Vector3 nextPosition;
            
            // Start at random index and loop through all neigboring tiles
            // Check if ghost can walk anywhere without walking back
            int start = Random.Range(0,4);
            for(int i = 0; i < 4; i++) {
                int neighborIndex = (i+start)%4;
                if(neighborIndex != previousTile) {
                    int neighborTile = neighbors[neighborIndex];
                    nextPosition = GetNextPosition(ghosts[index].transform.position, neighborIndex);
                    if(IsWalkable(neighborTile, nextPosition)) {
                        MoveGhost(index, nextPosition, (neighborIndex+2)%4);
                        return;
                    }
                }
            }

            // walk back
            nextPosition = GetNextPosition(ghosts[index].transform.position, previousTile);
            MoveGhost(index, nextPosition, (previousTile+2)%4);
        }
    }

    bool IsAtOuterWall(Vector3 pos) {
        return false;
    }

    void GhostFourMovement(int index) {
        if(!tweener.TweenExists(ghosts[index].transform)) {
            if(!IsAtOuterWall(ghosts[index].transform.position)) {

            }
        }
    }

    void DeadMovement(int index) {
        Transform gTransform = ghosts[index].transform;
        if(InStartArea(gTransform.position)) {
            Resurrect(index);
        } else {
            if(!tweener.TweenExists(gTransform)) {
                tweener.AddTween(gTransform, gTransform.position, spawnPosition, 3);
            }
        }
    }
}
