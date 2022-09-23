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

                float rotation = DecideRotation(tileNumber);
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

    float DecideRotation(int tile) {
        return 0.0f;
    }

    void CreateCopies(int tileNumber, int x, int y, float rotation) {
        int width = levelMap.GetLength(1) * 2 - 1;
        int height = levelMap.GetLength(0) * 2 - 1;

        //Top Right
        GameObject topRight = CreateTile(tileNumber, width-x, y);
        topRight.transform.SetParent(tilesTopRight.transform);

        topRight.transform.Rotate(new Vector3(0.0f, 0.0f, (rotation+180)%360));

        if (y < height/2) {
            //Bottom Left
            GameObject bottomLeft = CreateTile(tileNumber, x, height-y-1);
            bottomLeft.transform.SetParent(tilesBottomLeft.transform);

            bottomLeft.transform.Rotate(new Vector3(0.0f, 0.0f, (rotation+180)%360));
            
            //Bottom Right
            GameObject bottomRight = CreateTile(tileNumber, width-x, height-y-1);
            bottomRight.transform.SetParent(tilesBottomRight.transform);

            bottomRight.transform.Rotate(new Vector3(0.0f, 0.0f, (rotation+180)%360));
        }   
    }
}
