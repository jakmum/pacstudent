using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    GameManager gameManager;
    int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,0},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };
    int[,] completeLvlMap;
    private GameObject tilesTopLeft;
    private GameObject tilesTopRight;
    private GameObject tilesBottomLeft;
    private GameObject tilesBottomRight;
    [SerializeField]
    private Sprite[] sprites;
    private bool tOpen = false;

    // Start is called before the first frame update
    public void Initialize(GameManager gm) {
        gameManager = gm;
    }
    void Start()
    {
        tilesTopLeft = gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        tilesTopRight = gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
        tilesBottomLeft = gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
        tilesBottomRight = gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;

        CompleteLvlMap();
        DeleteLevel();
        BuildLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CompleteLvlMap() {
        completeLvlMap = new int[levelMap.GetLength(0)*2-1,levelMap.GetLength(1)*2];
        for (int i = 0; i < completeLvlMap.GetLength(0); i++) {
            for (int j = 0; j < completeLvlMap.GetLength(1); j++) {
                int x = i;
                int y = j;
                if(i >= levelMap.GetLength(0))
                    x = 2*levelMap.GetLength(0)-i-2;
                if(j >= levelMap.GetLength(1))
                    y = 2*levelMap.GetLength(1)-j-1;
                completeLvlMap[i,j] = levelMap[x,y];
            }
        }
    }

    void BuildLevel() {
        for (int i = 0; i < levelMap.GetLength(0); i++) {
            for (int j = 0; j < levelMap.GetLength(1); j++) {
                int tileNumber = levelMap[i,j];

                GameObject newTile = CreateTile(tileNumber, j, i);
                newTile.transform.SetParent(tilesTopLeft.transform);

                Vector3 rotation = DecideRotation(tileNumber, j, i);
                newTile.transform.Rotate(rotation);

                CreateCopies(tileNumber, j, i, rotation);
            }
        }
    }

    void DeleteLevel() {
        foreach (Transform tile in tilesTopLeft.transform) {
            GameObject.Destroy(tile.gameObject);
        }
        foreach (Transform tile in tilesTopRight.transform) {
            GameObject.Destroy(tile.gameObject);
        }
        foreach (Transform tile in tilesBottomLeft.transform) {
            GameObject.Destroy(tile.gameObject);
        }
        foreach (Transform tile in tilesBottomRight.transform) {
            GameObject.Destroy(tile.gameObject);
        }
    }

    GameObject CreateTile(int tileNumber, int x, int y) {
        GameObject newTile = new GameObject("tile"+x+y);
        SpriteRenderer renderer = newTile.AddComponent<SpriteRenderer>();
        if (tileNumber > 0) {
            renderer.sprite = sprites[tileNumber-1];
        }
        newTile.transform.position = new Vector3((float) x, (float) -y, 0.0f);

        return newTile;
    }

    Vector3 DecideRotation(int tileNumber, int x, int y) {
        //start tile always in standard rotation
        if (x == 0 && y == 0)
            return new Vector3(0.0f, 0.0f, 0.0f);

        //0:left; 1:top; 2:right; 3:bottom
        int[] neighbors = GetNeighbors(x, y);

        switch (tileNumber) {
            //outside corner
            case 1:
                //must have outside corner, outside wall or t junction adjacent
                if (   neighbors[0] == 1
                    || neighbors[0] == 2
                    || neighbors[0] == 7) 
                {
                    if (   neighbors[3] == 1
                        || neighbors[3] == 2
                        || neighbors[3] == 7)
                    {
                        return new Vector3(0.0f, 0.0f, 270.0f);
                    } else {
                        return new Vector3(0.0f, 0.0f, 180.0f);
                    }
                } else {
                    if (   neighbors[3] == 1
                        || neighbors[3] == 2
                        || neighbors[3] == 7)
                    {
                        return new Vector3(0.0f, 0.0f, 0.0f);
                    } else {
                        return new Vector3(0.0f, 0.0f, 90.0f);
                    }
                }
            //outside wall
            case 2:
                //must have outside corner, outside wall or t junction adjacent
                if ((  neighbors[0] == 1
                    || neighbors[0] == 2
                    || neighbors[0] == 7)
                    &&
                    (  neighbors[2] == 1
                    || neighbors[2] == 2
                    || neighbors[2] == 7))
                {
                    return new Vector3(0.0f, 0.0f, 90.0f);
                } else if ((   neighbors[1] == 1
                            || neighbors[1] == 2
                            || neighbors[1] == 7
                            || neighbors[1] == -1)
                        &&
                        (  neighbors[3] == 1
                        || neighbors[3] == 2
                        || neighbors[3] == 7
                        || neighbors[3] == -1))
                {
                    return new Vector3(0.0f, 0.0f, 0.0f);
                } else {
                    return new Vector3(0.0f, 0.0f, 90.0f);
                }
            //inside corner
            case 3:
                //must have inside corner, inside wall or t junction adjacent
                if (   neighbors[0] == 3
                    || neighbors[0] == 4
                    || neighbors[0] == 7) 
                {
                    if (   neighbors[3] == 3
                        || neighbors[3] == 4
                        || neighbors[3] == 7) 
                    {
                        return new Vector3(0.0f, 0.0f, 270.0f);
                    } else {
                        return new Vector3(0.0f, 0.0f, 180.0f);
                    }
                } else {
                    if (   neighbors[3] == 3
                        || neighbors[3] == 4
                        || neighbors[3] == 7) 
                    {
                        return new Vector3(0.0f, 0.0f, 0.0f);
                    } else {
                        return new Vector3(0.0f, 0.0f, 90.0f);
                    }
                }
            //inside wall
            case 4:
                float rand = Random.Range(0.0f,1.0f);
                //must have inside corner, inside wall or t junction adjacent
                if ((  neighbors[0] == 3
                    || neighbors[0] == 4
                    || neighbors[0] == 7)
                    && 
                    (  neighbors[2] == 3
                    || neighbors[2] == 4
                    || neighbors[2] == 7))
                {
                    //rotate up/down randomly for more diverse looking layout
                    if(rand > 0.5f)
                        return new Vector3(0.0f, 0.0f, 90.0f);
                    return new Vector3(0.0f, 0.0f, 270.0f);
                } else if (((  neighbors[1] == 3
                            || neighbors[1] == 4
                            || neighbors[1] == 7)
                            &&
                        (  neighbors[3] == 3
                        || neighbors[3] == 4
                        || neighbors[3] == 7
                        || neighbors[3] == 0
                        || neighbors[3] == -1))
                        ||
                        (( neighbors[1] == 3
                        || neighbors[1] == 4
                        || neighbors[1] == 7
                        || neighbors[3] == 0
                        || neighbors[3] == -1)
                        &&
                        (  neighbors[3] == 3
                        || neighbors[3] == 4
                        || neighbors[3] == 7)))
                {
                    //rotate left/right randomly for more diverse looking layout
                    if(rand > 0.5f)
                        return new Vector3(0.0f, 0.0f, 0.0f);
                    return new Vector3(0.0f, 0.0f, 180.0f);
                } else 
                {
                    //rotate up/down randomly for more diverse looking layout
                    if(rand > 0.5f)
                        return new Vector3(0.0f, 0.0f, 90.0f);
                    return new Vector3(0.0f, 0.0f, 270.0f);
                }
            //t junction
            case 7:
                if ((  neighbors[0] == 1
                    || neighbors[0] == 2
                    || neighbors[0] == 7)
                    &&
                    (  neighbors[2] == 1
                    || neighbors[2] == 2
                    || neighbors[2] == 7))
                {
                    if (   neighbors[1] == 3
                        || neighbors[1] == 4
                        || neighbors[1] == 7)
                    {
                        if (tOpen) {
                            tOpen = false;
                            return new Vector3(0.0f, 0.0f, 180.0f);
                        } else {
                            tOpen = true;
                            return new Vector3(180.0f, 0.0f, 0.0f);
                        }
                    } else {
                        if (tOpen) {
                            tOpen = false;
                            return new Vector3(0.0f, 180.0f, 0.0f);
                        } else {
                            tOpen = true;
                            return new Vector3(0.0f, 0.0f, 0.0f);
                        }
                    }
                } else if ((   neighbors[1] == 1
                            || neighbors[1] == 2
                            || neighbors[1] == 7)
                            &&
                            (  neighbors[3] == 1
                            || neighbors[3] == 2
                            || neighbors[3] == 7))
                {
                    if (   neighbors[0] == 3
                        || neighbors[0] == 4
                        || neighbors[0] == 7)
                    {
                        if (tOpen) {
                            tOpen = false;
                            return new Vector3(180.0f, 0.0f, 270.0f);
                        } else {
                            tOpen = true;
                            return new Vector3(0.0f, 0.0f, 270.0f);
                        }
                    } else {
                        if (tOpen) {
                            tOpen = false;
                            return new Vector3(0.0f, 0.0f, 90.0f);
                        } else {
                            tOpen = true;
                            return new Vector3(180.0f, 0.0f, 90.0f);
                        }
                    }
                } else {
                    return new Vector3(0.0f, 0.0f, 0.0f);
                }
            //no rotation needed for pellets
            default:
                return new Vector3(0.0f, 0.0f, 0.0f);
        }    
    }

    void CreateCopies(int tileNumber, int x, int y, Vector3 rotation) {
        int width = levelMap.GetLength(1) * 2 - 1;
        int height = levelMap.GetLength(0) * 2 - 1;

        //Top Right
        GameObject topRight = CreateTile(tileNumber, width-x, y);
        topRight.transform.SetParent(tilesTopRight.transform);

        topRight.transform.Rotate(new Vector3(rotation.x, rotation.y + 180.0f, rotation.z));

        if (y < height/2) {
            //Bottom Left
            GameObject bottomLeft = CreateTile(tileNumber, x, height-y-1);
            bottomLeft.transform.SetParent(tilesBottomLeft.transform);

            bottomLeft.transform.Rotate(new Vector3(rotation.x + 180.0f, rotation.y, rotation.z));
            
            //Bottom Right
            GameObject bottomRight = CreateTile(tileNumber, width-x, height-y-1);
            bottomRight.transform.SetParent(tilesBottomRight.transform);

            bottomRight.transform.Rotate(new Vector3(rotation.x + 180.0f, rotation.y + 180.0f, rotation.z));
        }
    }

    public int[] GetNeighbors (int x, int y) {
        //0:left; 1:top; 2:right; 3:bottom
        int[] neighbors = new int[4];

        if(x > 0) {
            neighbors[0] = completeLvlMap[y,x-1];
        } else {
            neighbors[0] = -1;
        }

        if(y > 0) {
            neighbors[1] = completeLvlMap[y-1,x];
        } else {
            neighbors[1] = -1;
        }

        if(x < completeLvlMap.GetLength(1)-1) {
            neighbors[2] = completeLvlMap[y,x+1];
        } else {
            neighbors[2] = -1;
        }

        if(y < completeLvlMap.GetLength(0)-1) {
            neighbors[3] = completeLvlMap[y+1,x];
        } else {
            neighbors[3] = -1;
        }

        return neighbors;
    }
}
