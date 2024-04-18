using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/LevelSettings")]
public class LevelSettings : ScriptableObject
{
    [Serializable]
    public struct LevelData
    {
        public int level;
        public int maxScore;
    }

    [SerializeField]
    private List<LevelData> levels = new List<LevelData>();

    public List<LevelData> Levels
    {
        get { return levels; }
    }

    public int GetLevelByScore(int score)
    {
        var data = levels.Find(x => x.maxScore >= score);
        return data.level;
    }

    public int GetMaxScoreByLevel(int level)
    {
        var data = levels.Find(x => x.level == level);
        return data.maxScore;
    }

    
}
