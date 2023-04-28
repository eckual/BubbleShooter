using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bubbles
{
    [Serializable]
    public struct BubbleData
    {
        public int number;
        public Color32 backColor;
        public Color32 borderColor;
    }

    [CreateAssetMenu(menuName ="ScriptableObjects/Bubbles/Settings")]
    public class BubblesSettings : ScriptableObject
    {
        [SerializeField]
        private int width = 6;
        [SerializeField]
        private int height = 5;
        [SerializeField]
        private float fallingSpeed = 0;
        [SerializeField]
        private float moveSpeed = 1;

        [SerializeField]
        private int initialRowsCount = 4;
        [SerializeField]
        private int initialColumnsCount = 6;
        [SerializeField]
        private int bubblesVisibleCount = 30;

        [SerializeField]
        private int generationMinPower = 1;
        [SerializeField]
        private int generationMaxPower = 11;

        [SerializeField]
        private List<BubbleData> bubbles = new List<BubbleData>();

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int InitialRowsCount
        {
            get { return initialRowsCount; }
        }

        public int InitialColumnsCount
        {
            get { return initialColumnsCount; }
        }

        public float FallingSpeed
        {
            get { return fallingSpeed; }
        }

        public float MoveSpeed
        {
            get { return moveSpeed; }
        }

        public int BubblesVisibleCount
        {
            get { return bubblesVisibleCount; }
        }

        public int GenerationMinPower
        {
            get { return generationMinPower; }
        }

        public int GenerationMaxPower
        {
            get { return generationMaxPower; }
        }

        public List<BubbleData> Bubbles
        {
            get { return bubbles; }
        }
    }
}