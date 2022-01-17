using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using TMPro;

namespace BrickBreak
{
    public class GameController : MonoBehaviour
    {
        public float score;
        public float wreckingBallMeter;
        public int wave;
        public Button left;
        public Button right;
        public Button launch;
        public bool gameLost;
        public event Action EOnGameOver;
        public BallSpawner spawner;
        public Block[] blockObjects;
        public Block[] remainingBlocks;
        public float[] wreckingBallNumbers;
        public bool wreckingBallReady;
        public Text currentPowerUp;
        public GameObject pauseMenu;
        public PowerUp[] PowerUps;
        public bool generatingBlocks;
        public AdManager ads;
        public AudioSource blockSound;
        public AudioSource clusterBallSound; 

        private float angle;
        private float timePlayed;
        private float timeSinceLastAction;
        private string currentPowerUpName;
        private Vector3 spawnerStartLocation;

        private void OnEnable()
        { 
            timeSinceLastAction = 0;
            timePlayed = 0;
            angle = 0;
            spawner = FindObjectOfType<BallSpawner>();
            spawnerStartLocation = spawner.transform.position;
            NewGame();
        }

        /// <summary>
        /// Funcrion to set all values back to zero. clear all objects currently on the screen and create a new line of blocks
        /// </summary>
        public void NewGame()
        {
           // ads.LoadAd();
            timePlayed = 0;
            wave = 0;
            score = 0;
            wreckingBallMeter = 0;
            gameLost = false;
            wreckingBallReady = false;
            ClearScreen();
            GenerateNewLine();
            spawner.transform.position = spawnerStartLocation;
        }

        public void Pause()
        {
            timeSinceLastAction = 0;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }

        public void Resume()
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }

        /// <summary>
        /// Removes all blocks and balls currently on the screen 
        /// </summary>
        private void ClearScreen()
        {
            StopAllCoroutines();
            spawner.ResetPosition();

            remainingBlocks = FindObjectsOfType<Block>();

            if (remainingBlocks.Length > 0)
            {
                foreach(Block block in remainingBlocks)
                {
                    Destroy(block.gameObject);
                    
                }
                
            }

            if(spawner.BallsInWorld.Length > 0)
            {
                foreach (Ball ball in spawner.BallsInWorld)
                {
                    Destroy(ball.gameObject);
                }
            }
            remainingBlocks = FindObjectsOfType<Block>();
        }

        public void Update()
        {
            if(wreckingBallReady)
            {
                currentPowerUp.text = currentPowerUpName;
            } else
            {
                currentPowerUp.text = "";
            }

            if (angle != 0)
            {
                //update the angle of the spawner if we have recieved a new angle
                RotateSpawner(angle);
            }
            timePlayed += Time.deltaTime;
            timeSinceLastAction += Time.deltaTime;
        }

        /// <summary>
        /// Function for spawning a new line of blocks
        /// </summary>
        public void GenerateNewLine()
        {
            generatingBlocks = true;

            var ghostLocations = GameObject.FindGameObjectsWithTag("newPos");

            foreach(var ghost in ghostLocations)
            {
                Destroy(ghost.gameObject);
            }

            remainingBlocks = FindObjectsOfType<Block>();
            if (remainingBlocks.Length > 0)
            {
                foreach(Block block in remainingBlocks)
                {
                    if (block.transform.position.y < spawner.transform.position.y + 2f)
                    {
                        GameOver();
                        break;
                    }
                    block.nextPos = new Vector3(block.transform.position.x, block.transform.position.y - 0.925f, 0);
                    block.movingDown = true;
                }
            }

            //update the "wave" count
            wave += 1;
            int maxHitValue;
            int maxPowerUpLevel;
            //max hit value determines how many hits the blocks can possibly be assigned
            //scales with wave level with a the highest possible value being 40
            if (wave < 20)
            {
                maxPowerUpLevel = wave / 6;
                maxHitValue = wave * 2;
            }
            else {
                maxHitValue = 40;
                maxPowerUpLevel = 3;
            }


            float halfHeight = Camera.main.orthographicSize;
            float halfWidth = Camera.main.aspect * halfHeight;
            float edgeX = (Camera.main.orthographicSize * Screen.width / Screen.height);
            float resolutionOffSet = (edgeX - 0.95f) / 2f; 
            for (int i = 0; i < (halfWidth); i++)
            {

                int createBlockChance = UnityEngine.Random.Range(0, 100);
                int powerUpToSpawn = UnityEngine.Random.Range(0, maxPowerUpLevel);
                int powerUpChance = UnityEngine.Random.Range(0, 100);
                if (createBlockChance < 70)
                {
                    Vector3 location = new Vector3((-(edgeX - 1.05f) + (i * (1.85f))), 6f, 0);
                    Block newBlock = Instantiate(blockObjects[0], transform);
                    newBlock.hits = UnityEngine.Random.Range(1, maxHitValue);
                    newBlock.maxHits = newBlock.hits;
                    newBlock.SetUpBlock();
                    newBlock.transform.position = location;
                    location.y = 3.9f;
                    newBlock.movingDown = true;
                    newBlock.nextPos = location;
                } else if (powerUpChance < 10)
                {
                    Vector3 location = new Vector3(-(edgeX - 0.95f) + (i * 1.85f), 6f, 0);
                    PowerUp newPowerUp = Instantiate(PowerUps[powerUpToSpawn], transform);
                    newPowerUp.transform.position = location;
                    location.y = 3.9f;
                    newPowerUp.movingDown = true;
                    newPowerUp.nextPos = location;
                }
                
            }

            StartCoroutine("WaitForBlocks");

            remainingBlocks = FindObjectsOfType<Block>();
        }

        IEnumerator WaitForBlocks()
        {
            yield return new WaitForSeconds(1.5f);
            generatingBlocks = false;
        }

        public void RotateSpawner(float direction)
        {
            Analytics.CustomEvent("Roatate Action made, time since last action: " + timeSinceLastAction);
            timeSinceLastAction = 0;
            if(direction == 0)
            {
                angle = 0;
                return;
            }
            
            angle += direction * Time.deltaTime;

            float rotateTo = spawner.transform.eulerAngles.z + direction;
            
            
            if (rotateTo > 10 && rotateTo < 170)
            {
                spawner.transform.Rotate(0, 0, direction);
            }
        }

        public void Launch()
        {
            ads.LoadAd();
            Analytics.CustomEvent("Launch Action made, time since last action: " + timeSinceLastAction);
            if (spawner.BallsInWorld.Length < 1 && !generatingBlocks && !wreckingBallReady)
            {
                spawner.LaunchBalls();
                spawner.isLaunching = true;
            } else if (wreckingBallReady)
            {
                spawner.LaunchWreckingBall();
                spawner.isLaunching = true;
            } else if(spawner.BallsInWorld.Length > 0)
            {
                Debug.Log("there are still " + spawner.BallsInWorld.Length + " balls in the world!");
            }
        }

        public void GameOver()
        {
            Analytics.CustomEvent("Game over after: " + timePlayed);
            Analytics.CustomEvent("Game Ended on Wave: " + wave);
            Analytics.CustomEvent("Game Ended with Score: " + score);
            ClearScreen();
            gameLost = true;
            ads.ShowAd();
            if(EOnGameOver != null)
            {
                EOnGameOver();
            }
        }

        public void ChangeWreckingBallLevel(int level, string powerUpName)
        {
            currentPowerUpName = powerUpName;
            wreckingBallReady = true;
            spawner.wreckingBallAvailable = level;
        }
    }
}

