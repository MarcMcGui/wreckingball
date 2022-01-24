using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrickBreak
{
    public class ClusterBall : Ball
    {
        public GameObject ballTemplate;

        private void Split()
        {
            sound.Play();
            //adjusted starting point of the circle so that balls do not launch straight down or to the sides as to prevent infinite bouncing
            for (float i = 0.5f; i < 8.5f; i++)
            {
                //https://answers.unity.com/questions/1068513/place-8-objects-around-a-target-gameobject.html
                float angle = i * Mathf.PI * 2f / 8;
                Vector3 newPos = new Vector3(transform.position.x + (Mathf.Cos(angle)), transform.position.y + (Mathf.Sin(angle)),  0);
                Ball spawnedBall = Instantiate(ballTemplate, newPos, Quaternion.identity).GetComponent<Ball>();
                spawnedBall.speed = parent.speedOfBalls;
                spawnedBall.direction = (newPos - transform.position).normalized;
                spawnedBall.hits = 5;
                spawnedBall.moving = true;
            }

            //Destroy main ball after the "cluster balls have been fired"
            Destroy(gameObject);
        }

        public override void FixedUpdate()
        {
            float length = speed * Time.deltaTime;
            RaycastHit2D hit = Physics2D.Raycast(previousPosition, direction, 2 * length);

            if (hit.collider != null && hit.collider.CompareTag("block") &&
                transform.position.y > (parent.transform.position.y + 0.5f))
            {
                Split();
            }
            else
            {
                Debug.DrawRay(transform.position, direction, Color.red);
            }

            this.previousPosition = transform.position;
        }

    }
}

