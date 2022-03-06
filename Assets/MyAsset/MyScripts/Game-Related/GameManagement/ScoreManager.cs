using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int score, hiScore;
    public TMP_Text scoreTxt, hiScoreTxt, gameOverScroeTxt;
    public float countdownTimer = 5f;
    public float timeRemaining = 0;
    public bool isGameOver = false;
    public bool gameHasStarted = false;
    public DestructionManager destructionManager;
    public GameObject UIHolder;
    public TMP_Text timerText;

    public float m_camHeight = 0f;
    public Transform cam;

    public bool canUseOldInputM;
    public Joystick joy;
    bool button2Hist;
    private void Awake()
    {
        instance = this;

        if (PlayerPrefs.HasKey("HighScore"))//check if high score value exists
        {
            hiScore = PlayerPrefs.GetInt("HighScore");
            if(hiScoreTxt) hiScoreTxt.text = ToString();
        }

        isGameOver = false;
        gameHasStarted = false;
        UIHolder.SetActive(false);

        joy = Joystick.current;

    }
    // Start is called before the first frame update
    void Start()
    {
        if(!gameHasStarted)
        {
            destructionManager.gameObject.SetActive(false);
        }

       
    }

    // Update is called once per frame
    void Update()
    {
        if (gameHasStarted)
        {
            if (!isGameOver)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                }
                else
                {
                    GameOver();
                }

            }
        }

        if(timerText)
        {
            timerText.text = timeRemaining.ToString();
        }

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            recenterPlease();
        }

    }

    public void recenterPlease()
    {
        Debug.LogWarning("Recenter Please");
        List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances<XRInputSubsystem>(subsystems);
        for (int i = 0; i < subsystems.Count; i++)
        {
            subsystems[i].TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);
            subsystems[i].TryRecenter();
        }

        cam.localPosition = new Vector3(cam.localPosition.x, m_camHeight, cam.localPosition.z);
    }

    void LateUpdate()
    {

        bool button2 = false;

        if (canUseOldInputM)
        {
            button2 = Input.GetButtonDown("Start");
            
        } else
        {
            button2 = joy.allControls[3].IsPressed();
        }



        if (button2Hist != button2)
        {
            if (button2)
            {
                if (!gameHasStarted)
                {
                    destructionManager.gameObject.SetActive(true);
                    UIHolder.SetActive(false);
                    destructionManager.DestroyAllTargets();
                    Player.TurretController.instance.ResetBars();
                    timeRemaining = countdownTimer;
                    gameHasStarted = true;
                    isGameOver = false;
                }
                else
                {
                    GameOver();
                }
            }


            button2Hist = button2;
        }

     


     }


    public void StartGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!gameHasStarted)
            {
                destructionManager.gameObject.SetActive(true);
                UIHolder.SetActive(false);
                destructionManager.DestroyAllTargets();
                Player.TurretController.instance.ResetBars();
                timeRemaining = countdownTimer;
                gameHasStarted = true;
                isGameOver = false;
            } else
            {
                GameOver();
            }
        }
    }


    public void GameOver()
    {
        isGameOver = true;
        gameHasStarted = false;
        destructionManager.gameObject.SetActive(false);

        float overallScore = 0f;
        Player.TurretController player = Player.TurretController.instance;
        

        // Calculate Remaining Health
        float healthScore = (player.health * 100);
        overallScore += healthScore;

        // Calculate Remaining Time
        float timeScore = (timeRemaining * 1);
        if (timeScore > 0)
        {
            overallScore -= timeScore;
        }

        // Calculate Shoot Score
        overallScore += score;

        Debug.Log("Health_Score: " + healthScore + "Time_Score: " + timeScore + "DMG_Score: " + score + "OVERALL_Score: " + overallScore);


        //UI
        UIHolder.SetActive(true);
        if(scoreTxt) scoreTxt.text = overallScore.ToString();

    }

    public void AddScore()
    {
        score++;
        UpdateHighScore();
    }

    public void UpdateHighScore()
    {
        if (score > hiScore)
        {
            hiScore = score;
            if(hiScoreTxt) hiScoreTxt.text = ToString();

            PlayerPrefs.SetInt("HighScore", hiScore);//Store high score on users device
        }

    }

    

    public void ResetScore()
    {
        score = 0;
        scoreTxt.text = score.ToString();
        gameOverScroeTxt.text = score.ToString();
    }


    public void ClearHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        hiScore = 0;
        hiScoreTxt.text = hiScore.ToString();
    }
}