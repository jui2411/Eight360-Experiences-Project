using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int score, hiScore;
    public TMP_Text scoreTxt, hiScoreTxt, gameOverScroeTxt;
    private void Awake()
    {
        instance = this;

        if (PlayerPrefs.HasKey("HighScore"))//check if high score value exists
        {
            hiScore = PlayerPrefs.GetInt("HighScore");
            hiScoreTxt.text = ToString();
        }

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddScore()
    {
        score++;
        UpdateHighScore();
        scoreTxt.text = score.ToString();
        gameOverScroeTxt.text = score.ToString();//UI for game over score 
    }

    public void UpdateHighScore()
    {
        if (score > hiScore)
        {
            hiScore = score;
            hiScoreTxt.text = ToString();

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