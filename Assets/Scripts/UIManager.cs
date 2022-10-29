using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField]
    GameObject score;
    [SerializeField]
    GameObject lifeIndicator;
    [SerializeField]
    GameObject timer;
    [SerializeField]
    GameObject ghostTimer;
    // Start is called before the first frame update
    
    public void Initialize(GameManager gm) {
        gameManager = gm;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGhostTimer(float value) {
        ghostTimer.SetActive(true);
        ghostTimer.GetComponent<TextMeshProUGUI>().SetText(Mathf.CeilToInt(value).ToString());
    }

    public void DisableGhostTimer() {
        ghostTimer.SetActive(false);
    }
}
