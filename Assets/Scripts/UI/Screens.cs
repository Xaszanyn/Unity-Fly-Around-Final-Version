using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class Screens : MonoBehaviour
{
    [SerializeField] GameObject player;

    [SerializeField] GameObject playScreen;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject failScreen;
    [SerializeField] GameObject successScreen;

    [SerializeField] GameObject[] successScreenParticles;

    [SerializeField] AudioSource music;
    [SerializeField] AudioSource fail;
    [SerializeField] AudioSource success;

    GameObject tap;

    //public bool isPaused { get { return IsPaused; } set { IsPaused = value; } }
    bool isPaused;

    void Start()
    {
        tap = playScreen.transform.GetChild(0).gameObject;

        ScaleTextUp(tap.GetComponentInChildren<RectTransform>());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPaused && (Input.mousePosition.y < Screen.height * .875F || Input.mousePosition.x > Screen.width * .25F)) Tap();
    }

    void ScaleTextUp(RectTransform text)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(text.DOScale(new Vector3(1.25F, 1.25F, 1), .5F));
        sequence.Append(text.DOScale(new Vector3(1, 1, 1), .5F));

        sequence.SetLoops(-1);

        sequence.Play();
    }

    void Tap()
    {
        tap.GetComponent<RectTransform>().DOScale(new Vector3(0, 0, 1), .5F).OnComplete(Complete);

        void Complete()
        {
            playScreen.SetActive(false);

            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<Rigidbody>().useGravity = true;

            player.transform.parent.GetComponent<Rotator>().enabled = true;

            pauseScreen.transform.GetChild(1).gameObject.SetActive(true);
            pauseScreen.transform.GetChild(1).GetComponent<Image>().DOFade(.7647F, .5F);

            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(.7647F, .5F);
        }
    }

    public void OpenPanel(string screen)
    {
        if (screen == "Pause")
        {
            player.GetComponent<PlayerController>().Pause();

            isPaused = true;

            var black = pauseScreen.transform.GetChild(0).gameObject;
            var pauseButton = pauseScreen.transform.GetChild(1).gameObject;
            var resumeButton = pauseScreen.transform.GetChild(2).gameObject;
            var panel = pauseScreen.transform.GetChild(3).gameObject;

            black.SetActive(true);
            black.GetComponent<Image>().DOFade(.5F, .5F);

            pauseButton.SetActive(false);
            resumeButton.SetActive(true);

            panel.SetActive(true);
            panel.GetComponent<RectTransform>().DOAnchorPosY(-120, .5F);
        }
        else if (screen == "Fail")
        {
            if (music != null) music.DOFade(0, 1).OnComplete(() => music.Stop());
            fail.Play();

            music.DOFade(0, 1);

            var black = failScreen.transform.GetChild(0).gameObject;
            var restartButton = failScreen.transform.GetChild(1).gameObject;
            var pauseButton = pauseScreen.transform.GetChild(1).gameObject;
            var resumeButton = pauseScreen.transform.GetChild(2).gameObject;
            var panel = failScreen.transform.GetChild(2).gameObject;

            restartButton.SetActive(true);
            pauseButton.SetActive(false);
            resumeButton.SetActive(false);

            black.SetActive(true);
            black.GetComponent<Image>().DOFade(.5F, .5F);

            panel.SetActive(true);
            panel.GetComponent<RectTransform>().DOAnchorPosY(-120, .5F);
        }
        else if (screen == "Success")
        {
            if (music != null) music.DOFade(0, 1).OnComplete(() => music.Stop());
            success.Play();

            var black = successScreen.transform.GetChild(0).gameObject;
            var pauseButton = pauseScreen.transform.GetChild(1).gameObject;
            var resumeButton = pauseScreen.transform.GetChild(2).gameObject;
            var panel = successScreen.transform.GetChild(1).gameObject;

            pauseButton.SetActive(false);
            resumeButton.SetActive(false);

            black.SetActive(true);
            black.GetComponent<Image>().DOFade(.5F, .5F);

            panel.SetActive(true);
            panel.GetComponent<RectTransform>().DOAnchorPosY(-120, .5F);
        }
    }

    public IEnumerator SuccessParticles(int scoreRatio)
    {
        yield return new WaitForSeconds(.5F);
        foreach (GameObject particle in successScreenParticles)
        {
            if (particle.name == "Confetti Shower")
            {
                if (scoreRatio > 0) particle.SetActive(true);
            }
            else if (particle.name == "Star")
            {
                if (scoreRatio == 2) particle.SetActive(true);
            }
            else particle.SetActive(true);
        }
    }

    public void ClosePanel(string screen)
    {
        if (screen == "Pause")
        {
            player.GetComponent<PlayerController>().Resume();

            isPaused = false;

            var black = pauseScreen.transform.GetChild(0).gameObject;
            var pauseButton = pauseScreen.transform.GetChild(1).gameObject;
            var resumeButton = pauseScreen.transform.GetChild(2).gameObject;
            var panel = pauseScreen.transform.GetChild(3).gameObject;

            black.GetComponent<Image>().DOFade(0, .5F).OnComplete(() => black.SetActive(false));

            pauseButton.SetActive(true);
            resumeButton.SetActive(false);

            panel.GetComponent<RectTransform>().DOAnchorPosY(-1600, .5F).OnComplete(() => panel.SetActive(false));
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Continue()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        if (index != 9) SceneManager.LoadScene(index + 1);
        else Menu();
    }
}
