using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rotator : MonoBehaviour
{
    public int speed { get { return Speed; } set { Speed = value; } }
    int Speed;

    public bool stop { get { return Stop; } set { Stop = value; } }
    bool Stop = true;

    private void Update()
    {
        if (!Stop) transform.Rotate(Vector3.down * Time.deltaTime * 1.3F * Speed);
    }

    //public void Stop()
    //{
    //    sequence.Kill();
    //}

    //public void Rotate(int period)
    //{
    //    sequence = DOTween.Sequence();

    //    sequence.Append(transform.DORotate(new Vector3(0, -360, 0), period, RotateMode.FastBeyond360).SetEase(Ease.Linear));
    //    sequence.SetLoops(-1);

    //    sequence.Play();
    //}
}
