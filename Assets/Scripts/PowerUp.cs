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

        public void OnEnable()
        {
            controller = FindObjectOfType<GameController>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("ball"))
            {
                controller.ChangeWreckingBallLevel(wreckingBallValue, powerUpName);
                collected();
            }
        }

        private void collected()
        {
            Destroy(gameObject);
        }
    }
}

