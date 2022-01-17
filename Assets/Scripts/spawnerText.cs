using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BrickBreak
{
    public class spawnerText : MonoBehaviour
    {
        public BallSpawner spawner;
        public TextMeshPro text;

        public void OnEnable()
        {
            text = GetComponent<TextMeshPro>();
        }

        // Update is called once per frame
        void Update()
        {
            if (spawner.controller.wreckingBallReady)
            {
                text.alpha = 0;
            } else
            {
                text.alpha = 1;
            }
            Vector3 newPos = new Vector3(spawner.transform.position.x, transform.position.y, 0);
            transform.position = newPos;
        }
    }
}

