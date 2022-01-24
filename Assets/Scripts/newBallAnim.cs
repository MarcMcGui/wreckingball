using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrickBreak
{
    public class newBallAnim : MonoBehaviour
    {
        public BallSpawner spawner;
        public float baseSpeed;

        private float acceleration;
        // Start is called before the first frame update
        void Start()
        {
            spawner = FindObjectOfType<BallSpawner>();
            baseSpeed = 10;
            acceleration = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if(transform.position.y > spawner.transform.position.y)
            {
                acceleration += 0.2f;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, -100, 0), (15 + acceleration) * Time.deltaTime);
            } else
            {
                transform.position = Vector3.MoveTowards(transform.position, spawner.transform.position, 10 * Time.deltaTime);
            }

        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("spawner"))
            {
                Destroy(gameObject);
            }
        }
    }
}

