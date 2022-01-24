using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrickBreak
{
    public class PowerUp : Block
    {
        public int wreckingBallValue;
        public Sprite wreckingBallSprite;
        public ParticleSystem pickupAnim;
        public string powerUpName;
        public GameObject newBall;

        public void OnEnable()
        {
            controller = FindObjectOfType<GameController>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("ball") && !powerUpName.Equals("additionalBalls"))
            {
                controller.ChangeWreckingBallLevel(wreckingBallValue, powerUpName);
                collected();
            } else if(powerUpName.Equals("additionalBalls"))
            {
                controller.spawner.maxBalls += 1;
                Instantiate(newBall, transform.position, Quaternion.identity);
                collected();
            }
        }

        private void collected()
        {
            CheckForEmptyScreen();
            Destroy(gameObject);
        }
    }
}

