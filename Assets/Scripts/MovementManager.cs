using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    [SerializeField]
    public GameObject fish;
    private Tweener tweener;
    private Animator fishAnimator;
    private Vector3[] fishPositions = {
        new Vector3(1.0f, -1.0f, 0.0f),
        new Vector3(6.0f, -1.0f, 0.0f),
        new Vector3(6.0f, -5.0f, 0.0f),
        new Vector3(1.0f, -5.0f, 0.0f)
    };
    private string[] fishStates = {
        "FishRight",
        "FishDown",
        "FishLeft",
        "FishUp"
    };
    private int speed = 2;

    // Start is called before the first frame update
    void Start()
    {
        tweener = GetComponent<Tweener>();
        fishAnimator = fish.GetComponent<Animator>();

        //Reset fish to start position
        fish.transform.position = fishPositions[0];
    }


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < fishPositions.Length; i++) {
            if(fish.transform.position == fishPositions[i]) {
                float duration = Vector3.Distance(fishPositions[i], fishPositions[(i+1)%4]) / speed;
                tweener.AddTween(fish.transform, fishPositions[i], fishPositions[(i+1)%4], duration);
                fishAnimator.Play(fishStates[i]);
            }
        }
    }

}
