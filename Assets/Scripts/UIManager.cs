using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace BrickBreak
{
    public class UIManager : MonoBehaviour
    {
        public float score;
        public float highScore;
        public GameObject introScreen;
        public GameObject endScreen;
        public GameObject gameUI;
        public event Action EOnGameEnd;
        public GameController controller;
        public TextMeshProUGUI currentScore;
        public TextMeshProUGUI wave;
        public AdManager ads;

        public TextMeshProUGUI scoreText;

        public void Update()
        {
            currentScore.text = "SCORE: " + controller.score.ToString();
            wave.text = "WAVE: " + controller.wave.ToString(); ;
        }
        public void StartGame()
        {
            introScreen.SetActive(false);
            endScreen.SetActive(false);
            controller.NewGame();
            controller.EOnGameOver += DisplayGameEnd;
            gameUI.SetActive(true);
            ads.InitializeAd();
        }

        public void DisplayGameEnd()
        {
            gameUI.SetActive(false);
            endScreen.SetActive(true);
            controller.EOnGameOver -= DisplayGameEnd;
            CalculateHighScore();
            string highScoreText = "Score: " + score.ToString() + '\n';
            if(highScore == 0)
            {
                highScoreText += "Best Score: " + score.ToString();
            } else
            {
                highScoreText += "Best Score: " + highScore.ToString();
            }
            scoreText.text = highScoreText;
        }

        public void DisplayStart()
        {
            gameUI.SetActive(false);
            introScreen.SetActive(true);
            endScreen.SetActive(false);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void CalculateHighScore()
        {
            score = controller.score;
            highScore = PlayerPrefs.GetFloat("HighScore", 0);
            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetFloat("HighScore", score);
            }
            
        }

    }
}

