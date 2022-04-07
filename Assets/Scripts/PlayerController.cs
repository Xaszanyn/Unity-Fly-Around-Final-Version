using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string levelName;

    [SerializeField] int speed;
    [SerializeField] int jumpSpeed;

    [SerializeField] int cameraAngle;

    [SerializeField] Material platform;
    [SerializeField] float colorChangeScale;

    [SerializeField] float linearInterpolation;

    [SerializeField] GameObject UI;

    [SerializeField] int maximumScore;

    [SerializeField] GameObject inventorialGrid;

    [SerializeField] AudioSource coinSound;
    [SerializeField] AudioSource powerUpSound;
    [SerializeField] AudioSource holeSound;
    [SerializeField] AudioSource wallSound;

    bool input;

    Rigidbody RB;

    Camera C;

    Vector3 pausedVelocity;

    bool isSuccessed;

    TextMeshProUGUI score;
    int itemScore;
    int heightScore;
    bool isScored;

    public List<int> inventory;

    bool processing;

    bool isScaledUp;

    private void Awake()
    {
        platform.color = Color.HSVToRGB(0, 0, 0);
    }

    void Start()
    {
        RB = GetComponent<Rigidbody>();
        transform.parent.GetComponent<Rotator>().stop = false;
        transform.parent.GetComponent<Rotator>().speed = speed;
        C = Camera.main;
        score = UI.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        Event(levelName, true);
        inventorialGrid.GetComponent<Inventory>().Arrange(inventory);
    }

    void Update()
    {
        input = Input.GetMouseButtonDown(0);

        Jump();

        LimitSpeed();
        DetermineAngle();
        MoveCamera();
        DeterminePlatformColor();
        CalculateScore();
    }

    void Jump()
    {
        if (input && (Input.mousePosition.y < Screen.height * .875F || Input.mousePosition.x > Screen.width * .25F)) RB.velocity = Vector3.up * jumpSpeed;
    }

    void LimitSpeed()
    {
        if (RB.velocity.y < -20) RB.velocity = Vector3.down * 20; ;
    }

    void DetermineAngle()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.forward * RB.velocity.y * 1.5F), linearInterpolation * .1F * Time.deltaTime * 130); // Time.deltaTime * 130 is for fixing the frames
    }

    void MoveCamera()
    {
        C.transform.localPosition = Vector3.Lerp(C.transform.localPosition, new Vector3(-2.7F, transform.localPosition.y + 1, -27), linearInterpolation * .015F * Time.deltaTime * 130);
        C.transform.localRotation = Quaternion.Lerp(C.transform.localRotation, Quaternion.Euler(0, cameraAngle, 0), linearInterpolation * .015F * Time.deltaTime * 130);
    }

    void DeterminePlatformColor()
    {
        Color color = Color.HSVToRGB((transform.position.y / colorChangeScale) % 1, .75F, 1);
        platform.color = Color.Lerp(platform.color, color, linearInterpolation * .015F * Time.deltaTime * 130);
    }

    void CalculateScore()
    {
        heightScore = heightScore < (int)transform.position.y - 10 ? (int)transform.position.y - 10 : heightScore ;

        int totalScore = itemScore + heightScore;

        if (totalScore < maximumScore)
        {
            score.text = "Score: " + totalScore;
        }
        else if (!isScored)
        {
            isScored = true;

            UI.transform.GetChild(0).GetComponent<RectTransform>().DOScale(new Vector3(1.25F, 1.25F, 1), .25F).OnComplete(() => UI.transform.GetChild(0).GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), .25F));

            score.color = new Color32(31, 195, 15, 195);
            score.text = "Score: " + maximumScore;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject collided = other.gameObject;

        if (collided.CompareTag("Platform")) Death();
        else if (!processing)
        {
            processing = true;

            if (collided.CompareTag("Coin")) Coin(collided);
            else if (collided.CompareTag("Key Red")) Key(collided, 0);
            else if (collided.CompareTag("Key Green")) Key(collided, 1);
            else if (collided.CompareTag("Key Blue")) Key(collided, 2);
            else if (collided.CompareTag("Door Red")) Door(collided, 0);
            else if (collided.CompareTag("Door Green")) Door(collided, 1);
            else if (collided.CompareTag("Door Blue")) Door(collided, 2);
            else if (collided.CompareTag("Power Up")) PowerUp(collided);
            else if (collided.CompareTag("Wall")) Wall(collided);
            else if (collided.CompareTag("Success")) Success();
            else if (collided.CompareTag("Hole")) holeSound.Play();

            StartCoroutine(FinishProcessing());
        }

        #region Triggers

        IEnumerator FinishProcessing()
        {
            yield return Wait.dotOne;
            processing = false;
        }

        void Coin(GameObject coin)
        {
            coinSound.Play();
            Destroy(coin.transform.parent.transform.parent.gameObject);
            itemScore += 2;
        }

        void Key(GameObject key, int color)
        {
            coinSound.Play();
            if (key != null) Destroy(key.transform.parent.gameObject);
            itemScore += 3;
            inventory.Add(color);
            inventorialGrid.GetComponent<Inventory>().Arrange(inventory);
        }

        void Door(GameObject door, int color)
        {
            if (inventory.Contains(color))
            {
                door.transform.DOScale(Vector3.up * 200, 1);
                Destroy(door, 1);
                inventory.Remove(color);
                inventorialGrid.GetComponent<Inventory>().Arrange(inventory);
            }
            else Death();
        }

        void PowerUp(GameObject powerUp)
        {
            powerUpSound.Play();
            Effect(powerUp.GetComponent<PowerUp>().powerUps);

            Destroy(powerUp.transform.parent.gameObject);

            itemScore += 5;

            void Effect(PowerUp.effect[] powerUps)
            {
                foreach(PowerUp.effect powerUp in powerUps)
                {
                    switch (powerUp.ToString())
                    {
                        case "zoomOut": ZoomOut(); break;
                        case "roll": Roll(); break;
                        case "fast": StartCoroutine(Fast()); break;
                        case "slow": StartCoroutine(Slow()); break;
                        case "scaleUp": ScaleUp(); break;
                        case "scaleDown": ScaleDown(); break;
                        case "weakness": StartCoroutine(Weakness()); break;
                        case "overPower": StartCoroutine(OverPower()); break;
                        case "keyRed": Key(null, 0); break;
                        case "keyGreen": Key(null, 1); break;
                        case "keyBlue": Key(null, 2); break;
                        case "permanentWeakness": Permanent(); break;
                    }
                }

                void ZoomOut()
                {
                    Event("Zoom Out");
                    DOTween.To(() => C.fieldOfView, F => C.fieldOfView = F, 150, .5F).OnComplete(() => DOTween.To(() => C.fieldOfView, F => C.fieldOfView = F, 75, 3.5F));
                }

                void Roll()
                {
                    Event("Roll");
                    C.transform.DOLocalRotate(new Vector3(C.transform.localEulerAngles.x, C.transform.localEulerAngles.y, 360), 1, RotateMode.FastBeyond360).SetEase(Ease.Linear);
                }

                IEnumerator Fast()
                {
                    Event("Faster");
                    Time.timeScale = 1.5F;
                    yield return Wait.four;
                    Time.timeScale = 1;
                }

                IEnumerator Slow()
                {
                    Event("Slower");
                    Time.timeScale = 0.5F;
                    yield return Wait.four;
                    Time.timeScale = 1;
                }

                void ScaleUp()
                {
                    Event("Bigger");

                    StartCoroutine(GoBig());
                    //transform.DOScale(new Vector3(2, 2, 2), .25F).OnComplete(() => transform.DOScale(new Vector3(1, 1, 1), 3.75F));

                    IEnumerator GoBig()
                    {
                        isScaledUp = true;
                        transform.DOScale(new Vector3(2, 2, 2), .25F);
                        yield return Wait.threeDotSeventyFive;
                        transform.DOScale(new Vector3(1, 1, 1), .25F);
                        yield return Wait.quarter;
                        isScaledUp = false;
                    }
                }

                void ScaleDown()
                {
                    Event("Smaller");

                    StartCoroutine(GoSmall());

                    IEnumerator GoSmall()
                    {
                        transform.DOScale(new Vector3(.5F, .5F, .5F), .25F);
                        yield return Wait.threeDotSeventyFive;
                        transform.DOScale(new Vector3(1, 1, 1), .25F);
                    }

                    //transform.DOScale(new Vector3(.5F, .5F, .25F), .5F).OnComplete(() => transform.DOScale(new Vector3(1, 1, 1), 3.75F));
                }

                IEnumerator Weakness()
                {
                    Event("Weakened");
                    int initialJumpSpeed = jumpSpeed;
                    jumpSpeed = jumpSpeed / 2;
                    yield return Wait.four;
                    jumpSpeed = initialJumpSpeed;
                }

                IEnumerator OverPower()
                {
                    Event("Over Powered");
                    int initialJumpSpeed = jumpSpeed;
                    jumpSpeed = jumpSpeed + jumpSpeed / 2;
                    yield return Wait.four;
                    jumpSpeed = initialJumpSpeed;
                }

                void Permanent()
                {
                    Event("Weakened");
                    int initialJumpSpeed = jumpSpeed;
                    jumpSpeed = jumpSpeed / 2 + jumpSpeed / 4;
                }
            }
        }

        void Success()
        {
            isSuccessed = true;
            Pause();

            var screens = UI.GetComponent<Screens>();

            int score = int.Parse(this.score.text.Split(' ')[1]);
            if (score == maximumScore) StartCoroutine(screens.SuccessParticles(2));
            else if (score >= maximumScore / 2F) StartCoroutine(screens.SuccessParticles(1));
            else StartCoroutine(screens.SuccessParticles(0));

            screens.OpenPanel("Success");
        }

        void Wall(GameObject triggerer)
        {
            if (isScaledUp)
            {
                wallSound.Play();
                var door = triggerer.transform.parent.gameObject;

                door.transform.GetChild(1).gameObject.SetActive(false);
                door.transform.GetChild(2).gameObject.SetActive(true);

                StartCoroutine(DeleteRest(door));

                IEnumerator DeleteRest(GameObject gameObject)
                {
                    yield return Wait.four;
                    Destroy(gameObject);
                }
            }
            else Death();
        }

        #endregion
    }

    void Event(string text, bool isTitle = false)
    {
        var eventText = UI.transform.GetChild(1).gameObject;
        eventText.SetActive(true);

        var TM = eventText.GetComponent<TextMeshProUGUI>();
        TM.text = text;

        eventText.GetComponent<RectTransform>().DOScale(new Vector3(2, 2, 1), isTitle ? 2 : .34F).OnComplete(Normalize);
        TM.DOFade(1, isTitle ? .5F : .05F).OnComplete(() => TM.DOFade(0, isTitle ? .5F : .29F));

        void Normalize()
        {
            eventText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            eventText.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform")) Death();
    }

    void Death()
    {
        if (!isSuccessed)
        {
            Time.timeScale = 1;
            Pause();

            transform.GetChild(0).gameObject.SetActive(true);

            UI.GetComponent<Screens>().OpenPanel("Fail");
        }
    }

    public void Pause()
    {
        transform.parent.GetComponent<Rotator>().stop = true;

        pausedVelocity = RB.velocity;
        RB.constraints = RigidbodyConstraints.FreezePositionY;
    }

    public void Resume()
    {
        transform.parent.GetComponent<Rotator>().stop = false;

        RB.constraints = RigidbodyConstraints.None;
        RB.velocity += pausedVelocity;
    }

    public class Wait
    {
        public static WaitForSecondsRealtime dotOne= new WaitForSecondsRealtime(.1F);
        public static WaitForSecondsRealtime quarter = new WaitForSecondsRealtime(.25F);

        public static WaitForSecondsRealtime one = new WaitForSecondsRealtime(1);
        public static WaitForSecondsRealtime two = new WaitForSecondsRealtime(2);
        public static WaitForSecondsRealtime four = new WaitForSecondsRealtime(3);


        public static WaitForSecondsRealtime threeDotSeventyFive = new WaitForSecondsRealtime(3.75F);
    }
}