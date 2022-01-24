using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace BrickBreak
{
    public class scoreBonus : MonoBehaviour
    {
        public TextMeshPro text;
        public int scoreValue;
        private float scaler;
        private float alpha;

        // Start is called before the first frame update
        void Start()
        {
            scaler = 0;
            alpha = 1;
            text = GetComponent<TextMeshPro>();
        }

        public void updateScore(int score)
        {
            text.text = "+" + score;
        }

        // Update is called once per frame
        void Update()
        {
            scaler += 0.0001f;
            alpha -= 0.003f;
            if(text.fontSize > 0)
            {
                text.fontSize -= scaler;
            }

            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            if(alpha <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}

