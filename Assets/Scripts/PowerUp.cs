using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum effect { zoomOut, roll, fast, slow, scaleUp, scaleDown, weakness, overPower, keyRed, keyGreen, keyBlue, permanentWeakness };

    public effect[] powerUps { get { return PowerUps; }}
    [SerializeField] effect[] PowerUps;
}
