using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrickBreak
{
    [System.Serializable]
    public class SaveSystem
    {
        public int wave;
        public int score;
        public int powerUpLevel;
        public string powerUpName;
        public bool powerUpAvailable;
        public SerialVector3 spawnerPosition;

        public List<KeyValuePair<int, SerialVector3>> blockData = new List<KeyValuePair<int, SerialVector3>>();
        public List<KeyValuePair<int, SerialVector3>> powerUpData = new List<KeyValuePair<int, SerialVector3>>();

    }

    [System.Serializable]
    public class SerialVector3
    {
        float x;
        float y;
        float z;

        public SerialVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 GetVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}

