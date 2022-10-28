using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private PacStudentController pacStudentController;
    private LevelGenerator levGen;

    // Start is called before the first frame update
    void Start()
    {
        levGen = GameObject.FindGameObjectWithTag("tiles").GetComponent<LevelGenerator>();
        pacStudentController = GameObject.FindGameObjectWithTag("PacStudent").GetComponent<PacStudentController>();
    }

    // Update is called once per frame
    void Update()
    {
        // for (int i = 0; i < fishPositions.Length; i++) {
        //     if(fish.transform.position == fishPositions[i]) {
        //         float duration = Vector3.Distance(fishPositions[i], fishPositions[(i+1)%4]) / speed;
        //         tweener.AddTween(fish.transform, fishPositions[i], fishPositions[(i+1)%4], duration);
        //         fishAnimator.Play(fishStates[i]);
        //     }
        // }
    }

    public int GetNextTile(Vector3 position, Vector3 direction) {
        //0:left; 1:top; 2:right; 3:bottom
        int[] neighbors = levGen.GetNeighbors((int)position.x, -(int)position.y);
        if(direction.x == -1)
            return neighbors[0];
        else if(direction.x == 1)
            return neighbors[2];
        else if(direction.y == -1)
            return neighbors[3];
        else
            return neighbors[1];
    }

    public bool isWalkable(int tileNumber) {
        if(tileNumber == 0 || tileNumber == 5 || tileNumber == 6)
            return true;
        return false;
    }

}
