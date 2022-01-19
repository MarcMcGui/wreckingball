using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
        public bool SaveGameEnabled;
        public bool enableAds;
        public bool enableAnalytics;

        private float angle;
        private float timePlayed;
        private float timeSinceLastAction;
        private string currentPowerUpName;
        private Vector3 spawnerStartLocation;
        private bool loadFound;

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
        /// Function to set all values back to zero. clear all objects currently on the screen and create a new line of blocks
        /// </summary>
        public void NewGame()
        {
            timePlayed = 0;
            wave = 0;
            score = 0;
            wreckingBallMeter = 0;
            wreckingBallReady = false;
            ClearScreen();

            if (File.Exists(Application.persistentDataPath + "/gamesave.save") && !gameLost && SaveGameEnabled)
            {
                Load();
            }
            else
            {
                GenerateNewLine();
                spawner.transform.position = spawnerStartLocation;
            }

            if(enableAds)
            {
                ads.LoadAd();
            }
            gameLost = false;
        }

        public void RestartGame()
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            generatingBlocks = false;
            gameLost = true;
            NewGame();
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

        public void Menu()
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            generatingBlocks = false;
            Save();
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
            if(enableAnalytics)
            {
                Analytics.CustomEvent("Launch Action made, time since last action: " + timeSinceLastAction);
            }

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
            if(enableAnalytics)
            {
                Analytics.CustomEvent("Game over after: " + timePlayed);
                Analytics.CustomEvent("Game Ended on Wave: " + wave);
                Analytics.CustomEvent("Game Ended with Score: " + score);
            }

            ClearScreen();
            gameLost = true;
            if(enableAds)
            {
                ads.ShowAd();
            }
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

        public SaveSystem CreateSave()
        {
            PowerUp[] powerUps = FindObjectsOfType<PowerUp>();
            SaveSystem save = new SaveSystem();
            save.blockData = new List<KeyValuePair<int, SerialVector3>>();
            save.powerUpData = new List<KeyValuePair<int, SerialVector3>>();
            save.wave = this.wave;
            save.score = (int)this.score;
            save.powerUpLevel = spawner.wreckingBallAvailable;
            save.powerUpName = currentPowerUpName;
            save.powerUpAvailable = wreckingBallReady;
            save.spawnerPosition = new SerialVector3(spawner.transform.position);

            foreach (Block block in remainingBlocks)
            {
                Vector3 blockPos;
                if (block.movingDown)
                {
                    blockPos = block.nextPos;
                } else
                {
                    blockPos = block.transform.position;
                }
                KeyValuePair<int, SerialVector3> blockdat = new KeyValuePair<int, SerialVector3>((int)block.hits, new SerialVector3(blockPos));
                save.blockData.Add(blockdat);
            }

            foreach (PowerUp powerup in powerUps)
            {
                Vector3 pos;
                if (powerup.movingDown)
                {
                    pos = powerup.nextPos;
                } else
                {
                    pos = powerup.transform.position;
                }
                KeyValuePair<int, SerialVector3> powerdat = new KeyValuePair<int, SerialVector3>(powerup.wreckingBallValue, new SerialVector3(pos));
                save.powerUpData.Add(powerdat);
            } 

            return save;
        }

        public void Save()
        {
            SaveSystem save = CreateSave();

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
                bf.Serialize(file, save);
                file.Close();
            } catch (Exception e)
            {
                Debug.LogError("SaveFailed with message: " + e.Message);
            }
            
        }

        public void Load()
        {
            SaveSystem save = new SaveSystem();
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
                save = (SaveSystem)bf.Deserialize(file);
                file.Close();
            } catch(Exception e)
            {
                loadFound = false;
                Debug.LogError("Load Failed with reason: " + e.Message);
                return;
            }
            

            foreach(KeyValuePair<int, SerialVector3> blockdat in save.blockData)
            {
                Block newBlock = Instantiate(blockObjects[0], blockdat.Value.GetVector3(), Quaternion.identity);
                newBlock.hits = blockdat.Key;
                newBlock.SetUpBlock();
            }

            foreach (KeyValuePair<int, SerialVector3> powerdat in save.powerUpData)
            {
                PowerUp newPower = Instantiate(PowerUps[powerdat.Key], powerdat.Value.GetVector3(), Quaternion.identity);
            }

            score = save.score;
            wave = save.wave;

            spawner.transform.position = save.spawnerPosition.GetVector3();

            if(save.powerUpAvailable)
            {
                ChangeWreckingBallLevel(save.powerUpLevel, save.powerUpName);
            }

            loadFound = true;
        }
    }
}

