using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemRotator : MonoBehaviour
{
    Vector3 direction;
    int time;

    void Start()
    {
        if (CompareTag("Coin"))
        {
            direction = Vector3.forward * 360;
            time = 1;
        }
        else if (CompareTag("Key Red") || CompareTag("Key Green") || CompareTag("Key Blue"))
        {
            direction = Vector3.up * 360;
            time = 1;
        }
        else if (CompareTag("Power Up"))
        {
            direction = new Vector3(22.5F, 360, 22.5F);
            time = 1;
        }
        else if (CompareTag("Start Level"))
        {
            direction = Vector3.down * 360;
            time = 8;
        }

        transform.DOLocalRotate(direction, time, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
    }
}
