using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BrickBreak
{
    public class BallSpawner : MonoBehaviour
    {
        public GameObject ballTemplate;
        public GameObject[] WreckingBalls;
        public int wreckingBallAvailable;
        public float speedOfBalls;
        public int maxBalls;
        public int numberOfBalls;
        public Ball[] BallsInWorld;
        public bool canMove;
        public GameObject aimLocation;
        public GameController controller;
        public Vector3 newPosition;
        public GameObject arrow;
        public bool isLaunching;
        public Sprite[] images;
        public Color[] wreckingBallColors;
        public GameObject ballsLeftDisplay;
        public GameObject newSpawnerLocation;
        

        private Vector3 direction;
        private GameObject newSpawn;
        private LineRenderer aimLine;
        private SpriteRenderer sprite;
        private Sprite defaultSprite;
        private Coroutine delayedLaunch = null;

        private void OnEnable()
        {
            controller = FindObjectOfType<GameController>();
            sprite = GetComponent<SpriteRenderer>();
            defaultSprite = sprite.sprite;
            maxBalls = 25;
            ballsLeftDisplay.GetComponent<TextMeshPro>().text = "x" + maxBalls;
            wreckingBallAvailable = 0;
            aimLine = GetComponent<LineRenderer>();
            canMove = false;
            numberOfBalls = maxBalls;
            aimLine.positionCount = 2;
            
            
        }

        private void Update()
        {
            if(controller.wreckingBallReady)
            {
                sprite.sprite = images[wreckingBallAvailable];
                sprite.color = wreckingBallColors[wreckingBallAvailable];
            } else
            {
                sprite.sprite = defaultSprite;
                sprite.color = Color.white;
            }

            BallsInWorld = FindObjectsOfType<Ball>();

            aimLine.SetPosition(0, transform.position);
            aimLine.SetPosition(1, aimLocation.transform.position);

            if (Input.GetMouseButton(0) && Camera.main.ScreenToWorldPoint(Input.mousePosition).y > transform.position.y + 0.5f)
            {
                var pos = Camera.main.WorldToScreenPoint(transform.position);
                var dir = Input.mousePosition - pos;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
           
            if(!isLaunching)
            {
                arrow.SetActive(true);
                aimLine.startColor = Color.white;
            } else
            {
                arrow.SetActive(false);
                aimLine.startColor = new Color(1, 1, 1, 0f);
            }
            
            aimLine.endColor = new Color(1, 1, 1, 0f);

            if (numberOfBalls < 1)
            {
                numberOfBalls = maxBalls;
            }

            if (BallsInWorld.Length < 1 && numberOfBalls == maxBalls)
            {
                ballsLeftDisplay.GetComponent<TextMeshPro>().text = "x" + maxBalls;
            }

        }

        public IEnumerator SpawnBallsDelayed()
        {
            while(numberOfBalls > 0)
            {
                Ball ball = Instantiate(ballTemplate, transform.position, Quaternion.identity).GetComponent<Ball>();
                ball.direction = direction;
                ball.speed = speedOfBalls;
                ball.moving = true;
                
                yield return new WaitForSeconds(0.1f);
                numberOfBalls -= 1;
                ballsLeftDisplay.GetComponent<TextMeshPro>().text = "x" + numberOfBalls;
            }
            isLaunching = false;
        }

        public void LaunchBalls()
        {
            canMove = true;
            isLaunching = true;
            direction = (aimLocation.transform.position - transform.position).normalized;
            StartCoroutine(SpawnBallsDelayed());
        }

        public void ResetPosition() {
            transform.position = new Vector3(0, transform.position.y, 0);
        }

        public void LaunchWreckingBall()
        {
            if(BallsInWorld.Length < 1)
            {
                canMove = true;
                Ball ball = Instantiate(WreckingBalls[wreckingBallAvailable], transform.position, Quaternion.identity).GetComponent<Ball>();
                direction = (aimLocation.transform.position - ball.transform.position).normalized;
                ball.hits = (int)(Mathf.Pow(2, wreckingBallAvailable) * 10);
                ball.direction = direction;
                ball.speed = speedOfBalls / (1.5f + wreckingBallAvailable);
                ball.moving = true;
                controller.wreckingBallMeter = 0;
                controller.wreckingBallReady = false;
                wreckingBallAvailable = 0;
            } 

        }

        public void CreateNewSpawnLocation()
        {
            newSpawn = Instantiate(newSpawnerLocation);
            newSpawn.transform.position = newPosition;
        }

        public void ChangeLocation()
        {
            this.transform.position = newPosition;
            Destroy(newSpawn);
        }
    }
}

