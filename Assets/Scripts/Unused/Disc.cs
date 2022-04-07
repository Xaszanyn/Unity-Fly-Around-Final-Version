using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disc : MonoBehaviour
{
    public GameObject[] platforms;

    Vector3 pivot = new Vector3(0, 0, 0);

    void Start()
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            var platform = Instantiate(platforms[i], transform);
            platform.transform.localRotation = Quaternion.Euler(Vector3.up * 45 * (7 - i));
        }
    }
}
