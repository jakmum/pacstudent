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
    GameObject[] lifeIndicators;
    [SerializeField]
    GameObject timer;
    [SerializeField]
    GameObject ghostTimer;
    [SerializeField]
    GameObject countDown;
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

    public IEnumerator CountDown(int t) {
        countDown.SetActive(true);
        countDown.GetComponent<Animator>().Play("CountDown");
        float expired = Time.time + t + 1;
        while(Time.time < expired) {
            float timeLeft = expired - Time.time;
            Debug.Log(timeLeft);
            if(timeLeft > 1.0f) {
                countDown.GetComponent<TextMeshProUGUI>().SetText(Mathf.CeilToInt(timeLeft-1).ToString());
            } else {
                countDown.GetComponent<TextMeshProUGUI>().SetText("GO!");
            }
            yield return null;
        }
        countDown.SetActive(false);
    }

    public void Reset() {
        SetScore(0);
        DisableGhostTimer();
        ResetLifes();
    }

    public void SetScore(int x) {
        score.GetComponent<TextMeshProUGUI>().SetText(x.ToString());
    }

    public void SetGhostTimer(float value) {
        ghostTimer.SetActive(true);
        ghostTimer.GetComponent<TextMeshProUGUI>().SetText(Mathf.CeilToInt(value).ToString());
    }

    public void DisableGhostTimer() {
        ghostTimer.SetActive(false);
    }

    public void LoseLife() {
        int lifes = --gameManager.lifes;
        if(lifes < 1) {
            lifeIndicators[0].SetActive(false);
        } else if(lifes < 2) {
            lifeIndicators[1].SetActive(false);
        } else if(lifes < 3) {
            lifeIndicators[2].SetActive(false);
        }
    }

    public void ResetLifes() {
        foreach(GameObject lifeIndicator in lifeIndicators) {
            lifeIndicator.SetActive(true);
        }
    }
}
