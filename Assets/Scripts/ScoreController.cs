using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bubbles;

public class ScoreController : MonoBehaviour
{
    private LevelSettings levelSettings;
    private int score;
    private int currentLevel;

    public event Action<int> OnScoreChanged;

    public int Score
    {
        get { return score; }
        set
        {
            if(score != value)
            {
                score = value;
                OnScoreChanged?.Invoke(score);
            }
        }
    }

    public LevelSettings LevelSettings
    {
        get
        {
            if (!levelSettings)
            {
                levelSettings = ResourceManager.GetResource<LevelSettings>(GameConstants.LEVEL_SETTINGS);
            }
            return levelSettings;
        }
    }

    public void Init()
    {
        SessionController.Instance.BubblesController.OnMerge += OnMerged;
    }

    private void OnMerged(MergeInfo mergeInfo)
    {
        Score += Bubble.GetNumber(mergeInfo.power);
    }
}