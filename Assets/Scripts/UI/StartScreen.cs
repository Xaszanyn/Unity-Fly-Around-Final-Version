using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class StartScreen : MonoBehaviour
{
    [SerializeField] GameObject levelScreen;

    public void Play()
    {
        Camera.main.transform.DOMoveX(40, 1);
        GetComponent<RectTransform>().DOAnchorPosX(-1080, 1);
        levelScreen.GetComponent<RectTransform>().DOAnchorPosX(0, 1);
    }

    public void L1(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
