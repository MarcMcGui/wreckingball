using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrickBreak
{
    public class Ball : MonoBehaviour
    {
        public Vector3 direction;
        public bool moving;
        public float speed;
        public BallSpawner parent;
        public Rigidbody2D body;
        public int hits;
        public AudioSource sound;

        [SerializeField]
        private bool isTouchingBlock;
        private float edgeX;
        private float edgeY;
        private string lastHit;
        
        [SerializeField]
        private int durability;

        protected Vector3 previousPosition;

        protected void OnEnable()
        {
            sound = GetComponent<AudioSource>();
            lastHit = "";
            durability = 100;
            hits = 1;
            edgeY = Camera.main.orthographicSize;
            edgeX = (Camera.main.orthographicSize * Screen.width / Screen.height);
            parent = FindObjectOfType<BallSpawner>();
        }

        // Update is called once per frame
        void Update()
        {
            if(isTouchingBlock)
            {
                durability -= 1;
            }

            if(durability < 1)
            {
                BreakSelf();
            }

            if(moving)
            {
                body.velocity = (direction * (speed));
            }

            if ((transform.position.x >= edgeX && direction.x > 0))
            {
                lastHit = transform.position.ToString();
                Bounce(new Vector3(-1,0,0));
            } else if(transform.position.x <= -edgeX && direction.x < 0)
            {
                lastHit = transform.position.ToString();
                Bounce(new Vector3(1, 0, 0));
            } else if(transform.position.y >= edgeY && direction.y > 0)
            {
                lastHit = transform.position.ToString();
                Bounce(new Vector3(0, -1, 0));
            } else if(transform.position.y <= parent.transform.position.y - 0.2f)
            {
                if(parent.canMove && transform.position.x < edgeX && transform.position.x > -edgeX)
                {
                    parent.canMove = false;

                    float newPosX = transform.position.x;

                    if( newPosX < (0 - edgeX) + 0.4f)
                    {
                        newPosX = (0- edgeX) + 0.4f;
                    } else if (newPosX > (edgeX - 0.4f))
                    {
                        newPosX = edgeX - 0.4f;
                    }

                    parent.newPosition = new Vector3(newPosX, parent.transform.position.y, 0);
                    parent.CreateNewSpawnLocation();
                }
                BreakSelf();

            }
        }

        protected void Bounce(Vector3 bouncePoint)
        {
            sound.Play();
            //use skews to prevent infinite horizontal bouncing at a 90degree angle
            float xSkew;
            float ySkew; 
            if(direction.x < 0)
            {
                xSkew = -0.01f;
            } else
            {
                xSkew = 0.01f;
            }
            

            parent.controller.wreckingBallMeter += 1;

            direction = (Vector3.Reflect(direction.normalized, bouncePoint) + new Vector3(xSkew, 0, 0));

            durability -= 1;
        }

        public virtual void FixedUpdate()
        {
            //Vector3 direction = transform.position - previousPosition;
            float length = speed * Time.deltaTime;
            RaycastHit2D hit = Physics2D.Raycast(previousPosition, direction, 3f * length);
            if (hit.collider != null && hit.collider.CompareTag("block") && 
                !lastHit.Equals(hit.transform.position.ToString()))
            {
                lastHit = hit.transform.position.ToString();
                Block hitBlock = hit.transform.gameObject.GetComponent<Block>();
                Debug.DrawRay(previousPosition, direction, Color.green);
                if( hits < 20)
                {
                    Bounce(hit.normal);
                }
                hitBlock.Hit(hits);
                
            } else
            {
                Debug.DrawRay(transform.position, direction, Color.red);
            }

            this.previousPosition = transform.position;
        }

        protected void BreakSelf()
        {
            parent.controller.remainingBlocks = FindObjectsOfType<Block>();
            if ((parent.BallsInWorld.Length < 2 && hits == 1) || (parent.BallsInWorld.Length < 2 && parent.controller.remainingBlocks.Length < 1))
            {
                if(!parent.controller.generatingBlocks && !parent.isLaunching)
                {
                    parent.ChangeLocation();
                    parent.controller.GenerateNewLine();
                }
            } else if(hits > 1)
            {
                parent.ChangeLocation();
                parent.isLaunching = false;
            }

            Destroy(gameObject);
        }
    }
}

