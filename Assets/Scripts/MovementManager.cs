using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public GameManager gameManager;
    [SerializeField]
    public PacStudentController pacStudentController;
    [SerializeField]
    public CherryController cherryController;
    [SerializeField]
    public GhostController ghostController;

    // Start is called before the first frame update
    public void Initialize(GameManager gm) {
        gameManager = gm;
        pacStudentController.Initialize(this);
        cherryController.Initialize(this);
        ghostController.Initialize(this);
    }

    void Start()
    {
        pacStudentController = GameObject.FindGameObjectWithTag("PacStudent").GetComponent<PacStudentController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetNextTile(Vector3 position, Vector3 direction) {
        //0:left; 1:top; 2:right; 3:bottom
        int[] neighbors = gameManager.levelGenerator.GetNeighbors((int)position.x, -(int)position.y);
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
        if(tileNumber == 0 || tileNumber == 5 || tileNumber == 6 || tileNumber == -1)
            return true;
        return false;
    }

}
