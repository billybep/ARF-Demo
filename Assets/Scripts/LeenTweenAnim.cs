using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeenTweenAnim : MonoBehaviour
{
    [SerializeField] GameObject Title, BtnStart;
    [SerializeField] LeanTweenType easeType;

    // Start is called before the first frame update
    void Start()
    {
        LeanTween
            .scale(Title, new Vector3(2f, 2f, 2f), 2f)
            .setEase(easeType)
            .setOnComplete(ButtonStartAnimation);
    }

    void ButtonStartAnimation()
    {
        LeanTween
            .scale(BtnStart, new Vector3(0.6f, 0.6f, 0.6f), 1f)
            .setEase(LeanTweenType.easeOutElastic);
    }
}
