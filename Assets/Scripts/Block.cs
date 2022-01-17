using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BrickBreak
{
    public class Block : MonoBehaviour
    {
        public float hits;
        public float maxHits;
        public GameObject textObject;
        public TextMeshPro hitText;
        public GameController controller;
        public ParticleSystem shakeParticles;
        public GameObject breakParticles;
        public ParticleSystem hitParticles;
        public bool movingDown;
        public Vector3 nextPos;
        public AudioSource sound;
        public Color startColor;
        public Color endColor;

        private SpriteRenderer image;
        private bool isBreaking;
       

        private void OnEnable()
        {
            sound = GetComponent<AudioSource>();
            controller = FindObjectOfType<GameController>();
            image = GetComponent<SpriteRenderer>();
            hitText = textObject.GetComponent<TextMeshPro>();
        }

        public void MoveDown(Vector3 newPos)
        {
            if(transform.position == newPos)
            {
                movingDown = false;
            }
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 2);
        }

        public void Update()
        {
            if (movingDown)
            {
                MoveDown(nextPos);
            }
        }

        public void SetUpBlock()
        {
            hitText.text = "" + hits;
            SetColor();
        }

        public void Hit(float hits)
        {
            SetColor();
            if (hits > 1)
            {
                StartCoroutine(Shake());
            }
            controller.score += Mathf.Min(this.hits, hits);
            this.hits -= hits;
            hitText.text = "" + this.hits;
            if (this.hits <= 0) Break();
            hitParticles.startColor = image.color;
            hitParticles.Emit(1);
            hitParticles.Play();
            
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.CompareTag("wreckingBall"))
            {
                Ball hittingBall = collision.gameObject.GetComponent<Ball>();
                hits -= hittingBall.hits;
                hitText.text = "" + this.hits;
                controller.score += Mathf.Min(this.hits, hits);
                if (this.hits <= 0)
                {
                    Break();
                }
            }
        }

        private void SetColor()
        {
            float redValue = 1 - (hits / 40);
            float blueValue = 0 + (hits / 40);
            image.color = new Color(redValue, 0.7f, blueValue);
        }

        public void Break()
        {
            controller.blockSound.Play();
            Instantiate(breakParticles, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        IEnumerator Shake()
        {
            shakeParticles.Emit(1);
            shakeParticles.Play();
            Vector3 currentPosition = transform.position;
            int shakeTime = 20;
            while (shakeTime > 0)
            {
                Vector3 newPos = new Vector3(transform.position.x + (Mathf.Sin(shakeTime)/2), currentPosition.y, 0);
                transform.position = newPos;
                shakeTime -= 1;
                yield return new WaitForSeconds(0);
            }

            transform.position = currentPosition;
            
        }
    }
}

