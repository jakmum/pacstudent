using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;
    Tweener tweener;
    float delay = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        tweener = GetComponent<Tweener>();
        InvokeRepeating("SpawnCherry2", delay, delay);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnCherry() {
        Vector3 randomLocation = new Vector3(Random.Range(0.0f,27.0f), 1.0f, 0.0f);
        GameObject newCherry = Instantiate(cherryPrefab, randomLocation, Quaternion.identity);
        MoveCherry(newCherry);
        yield return new WaitUntil(() => !tweener.TweenExists(newCherry.transform));
        Destroy(newCherry);
    }

    void SpawnCherry2() {
        StartCoroutine(SpawnCherry());
    }

    void MoveCherry(GameObject cherry) {
        tweener.AddTween(cherry.transform, 
            cherry.transform.position, 
            new Vector3(27.0f - cherry.transform.position.x, -30.0f, 0.0f),
            10.0f);
    }
}
