using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    //private Tween activeTween;
    private List<Tween> activeTweens = new List<Tween>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = activeTweens.Count - 1; i >= 0; i--) {
            float distance = Vector3.Distance(activeTweens[i].Target.position, activeTweens[i].EndPos);
            if (distance > 0.1f) {
                float t = (Time.time - activeTweens[i].StartTime) / activeTweens[i].Duration;
                activeTweens[i].Target.position = Vector3.Lerp(activeTweens[i].StartPos, activeTweens[i].EndPos, t);
            } else {
                activeTweens[i].Target.position = activeTweens[i].EndPos;
                activeTweens.RemoveAt(i);
            }
        }
    }

    public bool AddTween (Transform targetObject, Vector3 startPos, Vector3 endPos, float duration) {
        if (!TweenExists(targetObject)) {
            activeTweens.Add(new Tween(targetObject, startPos, endPos, Time.time, duration));
            return true;
        }
        return false;
    }

    public bool TweenExists (Transform target) {
        foreach (Tween tween in activeTweens) {
            if (tween.Target.Equals(target))
                return true;
        }
        return false;
    }
}
