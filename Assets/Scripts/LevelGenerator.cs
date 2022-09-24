using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
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
    private GameObject tilesTopLeft;
    private GameObject tilesTopRight;
    private GameObject tilesBottomLeft;
    private GameObject tilesBottomRight;
    [SerializeField]
    private Sprite[] sprites;
    private bool tOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        tilesTopLeft = gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        tilesTopRight = gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
        tilesBottomLeft = gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
        tilesBottomRight = gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;

        DeleteLevel();
        BuildLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BuildLevel() {
        for (int i = 0; i < levelMap.GetLength(0); i++) {
            for (int j = 0; j < levelMap.GetLength(1); j++) {
                int tileNumber = levelMap[i,j];

                GameObject newTile = CreateTile(tileNumber, j, i);
                newTile.transform.SetParent(tilesTopLeft.transform);

                float rotation = DecideRotation(tileNumber, j, i);
                newTile.transform.Rotate(new Vector3(0.0f, 0.0f, rotation));

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

    float DecideRotation(int tileNumber, int x, int y) {
        //start tile always in standard rotation
        if (x == 0 && y == 0)
            return 0.0f;

        //0:left; 1:top; 2:right; 3:bottom
        int[] neighbors = GetNeighbors(x, y);

        switch (tileNumber) {
            //outside corner
            case 1:
                //must have two outside walls adjacent
                if (neighbors[0] == 2) {
                    if (neighbors[3] == 2) {
                        return 270.0f;
                    } else {
                        return 180.0f;
                    }
                } else {
                    if (neighbors[1] == 2) {
                        return 90.0f;
                    } else {
                        return 0.0f;
                    }
                }
            //outside wall
            case 2:
                //must have outside corner, outside wall or t junction adjacent
                if (   neighbors[0] == 1
                    || neighbors[0] == 2
                    || neighbors[0] == 7)
                    
                {
                    if (   neighbors[2] == 1
                        || neighbors[2] == 2
                        || neighbors[2] == 7)
                    {
                        return 90.0f;
                    }
                } else if (    neighbors[1] == 1
                            || neighbors[1] == 2
                            || neighbors[1] == 7
                            || neighbors[1] == -1)
                {
                    if(    neighbors[3] == 1
                        || neighbors[3] == 2
                        || neighbors[3] == 7
                        || neighbors[3] == -1)
                    {
                        return 0.0f;
                    }
                }
                return 90.0f;
            //inside corner
            case 3:
                //inside coner or inside wall or t junction
                if (neighbors[0] == 3 || neighbors[0] == 4 || neighbors[0] == 7) {
                    if (neighbors[3] == 3 || neighbors[3] == 4 || neighbors[3] == 7) {
                        return 270.0f;
                    } else {
                        return 180.0f;
                    }
                } else {
                    if (neighbors[3] == 3 || neighbors[3] == 4 || neighbors[3] == 7) {
                        return 0.0f;
                    } else {
                        return 90.0f;
                    }
                }
            //inside wall
            case 4:
                if ((      neighbors[0] == 3
                        || neighbors[0] == 4
                        || neighbors[0] == 7
                        || neighbors[0] == -1
                    ) && ( neighbors[2] == 3
                        || neighbors[2] == 4
                        || neighbors[2] == 7
                        || neighbors[2] == -1
                        || neighbors[2] == 0))
                {
                    //rotate up/down randomly for more diverse looking layout
                    float rand = Random.Range(0,1);
                    if(rand > 0.5)
                        return 90.0f;
                    return 270.0f;
                } else {
                    //rotate right/left randomly for more diverse looking layout
                    float rand = Random.Range(0,1);
                    if(rand > 0.5)
                        return 0.0f;
                    return 180.0f;
                }
            //t junction
            default:
                return 0.0f;
        }
    }

    void CreateCopies(int tileNumber, int x, int y, float rotation) {
        int width = levelMap.GetLength(1) * 2 - 1;
        int height = levelMap.GetLength(0) * 2 - 1;

        //Top Right
        GameObject topRight = CreateTile(tileNumber, width-x, y);
        topRight.transform.SetParent(tilesTopRight.transform);

        topRight.transform.Rotate(new Vector3(0.0f, 180.0f, rotation));

        if (y < height/2) {
            //Bottom Left
            GameObject bottomLeft = CreateTile(tileNumber, x, height-y-1);
            bottomLeft.transform.SetParent(tilesBottomLeft.transform);

            bottomLeft.transform.Rotate(new Vector3(180.0f, 0.0f, rotation));
            
            //Bottom Right
            GameObject bottomRight = CreateTile(tileNumber, width-x, height-y-1);
            bottomRight.transform.SetParent(tilesBottomRight.transform);

            bottomRight.transform.Rotate(new Vector3(180.0f, 180.0f, rotation));
        }   
    }

    int[] GetNeighbors (int x, int y) {
        //0:left; 1:top; 2:right; 3:bottom
        int[] neighbors = new int[4];

        if(x > 0) {
            neighbors[0] = levelMap[y,x-1];
        } else {
            neighbors[0] = -1;
        }

        if(y > 0) {
            neighbors[1] = levelMap[y-1,x];
        } else {
            neighbors[1] = -1;
        }

        if(x < levelMap.GetLength(1)-1) {
            neighbors[2] = levelMap[y,x+1];
        } else {
            neighbors[2] = -1;
        }

        if(y < levelMap.GetLength(0)-1) {
            neighbors[3] = levelMap[y+1,x];
        } else {
            neighbors[3] = -1;
        }

        return neighbors;
    }
}
