using System;
using Bubbles;
using Utils;

namespace Controllers
{
    public class ScoreController : ControllerBase
    {
        public event Action<int> OnScoreChanged;
        
        private LevelSettings _levelSettings;
        private int _currentLevel;
        private int _score;
       
        public LevelSettings LevelSettings
        {
            get
            {
                if (!_levelSettings) _levelSettings = ResourceManager.GetResource<LevelSettings>(GameConstants.LevelSettings);
                return _levelSettings;
            }
        }

        public int Score
        {
            get => _score;
            private set
            {
                if (_score == value) return;
                _score = value;
                OnScoreChanged?.Invoke(_score);
            }
        }

        public override void Init()
        {
            SessionController.Instance.BubblesController.OnMerge += OnMerged;
        }
        
        private void OnMerged(MergeInfo mergeInfo)
        {
            Score += Bubble.GetNumber(mergeInfo.power);
        }
    
    }
}
