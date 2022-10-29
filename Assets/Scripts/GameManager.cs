using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public MovementManager movementManager;
    [SerializeField]
    public AudioManager audioManager;
    [SerializeField]
    public LevelGenerator levelGenerator;
    // Start is called before the first frame update
    void Awake() {
        movementManager.Initialize(this);
        audioManager.Initialize(this);
        levelGenerator.Initialize(this);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
