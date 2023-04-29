using System;
using System.Collections.Generic;
using System.Linq;
using Bubbles;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class UIBubblesInfoWindow : UIBaseWindow
    {
        private const string DEFAULT_BUBBLE_INFO_ID = "BubbleInfo";
    
        [Header("UI")]
        [SerializeField] private UISwapBubblesGroup projectileSelectionGroup;
        [SerializeField] private UIBubbleInfosPool bubbleInfosPool;
        [SerializeField] private UINotification perfectNotification;
    
        [Header("Level")]
        [SerializeField] private TMP_Text score;
        [SerializeField] private TMP_Text currentLevelText;
        [SerializeField] private Image currentLevelImage;
        [SerializeField] private TMP_Text nextLevelText;
        [SerializeField] private Image nextLevelImage;
        [SerializeField] private UIProgressBar levelProgressBar;
        [SerializeField] private Image progressBarFillImage;

        private List<UIBubbleInfo> _bubbleInfos = new List<UIBubbleInfo>();
        private BubblesSettings _bubblesSettings;
        private ScoreController _scoreController;
        private int _currentLevel;

        public override void Init()
        {
            base.Init();

            var sessionController = SessionController.Instance;
            var bubblesController = sessionController.BubblesController;
            bubblesController.OnBubbleAdded += OnBubblesAdded;
            bubblesController.OnBubbleReleased += OnBubbleReleased;
            bubblesController.OnBubblesCleared += perfectNotification.Show;
            _bubblesSettings = ResourceManager.GetResource<BubblesSettings>(GameConstants.BubbleSettings);

            for (var i = 0; i < bubblesController.Bubbles.Count; i++)
            {
                var bubble = bubblesController.Bubbles[i];
                OnBubblesAdded(bubble);
            }

            _scoreController = sessionController.ScoreController;
            _scoreController.OnScoreChanged += OnScoreChanged;
            OnScoreChanged(_scoreController.Score);

            projectileSelectionGroup.Init();
        }

        private void OnBubblesAdded(Bubble inNewBubble)
        {
            if (_bubbleInfos.FindIndex(bubble=> bubble.Source == inNewBubble) != -1) return;
            
            var bubbleInfo = bubbleInfosPool.GetOrInstantiate(DEFAULT_BUBBLE_INFO_ID);
            bubbleInfo.Init(inNewBubble);
            _bubbleInfos.Add(bubbleInfo);
            bubbleInfo.gameObject.SetActive(true);
        }

        private void OnBubbleReleased(Bubble bubble)
        {
            var bubbleInfo = _bubbleInfos.FirstOrDefault(b => b.Source == bubble);
            if (bubbleInfo == null)
            {
                Debug.Log("Released bubble should not be null");
                return;
            }
            
            bubbleInfosPool.Release(bubbleInfo);
            bubbleInfo.gameObject.SetActive(false);
            _bubbleInfos.Remove(bubbleInfo);
        }

        private void OnScoreChanged(int newScore)
        {
            score.text = newScore.ToString();

            var level = _scoreController.LevelSettings.GetLevelByScore(newScore);
            if (_currentLevel != level)
            {
                _currentLevel = level;
                ChangeLevel();
            }

            levelProgressBar.SetData(newScore, _scoreController.LevelSettings.GetMaxScoreByLevel(_currentLevel - 1),
                _scoreController.LevelSettings.GetMaxScoreByLevel(_currentLevel));
        }

        private void ChangeLevel()
        {
            currentLevelText.text = _currentLevel.ToString();
            nextLevelText.text = (_currentLevel + 1).ToString();

            var index = _currentLevel % _bubblesSettings.Bubbles.Count;
            var color = _bubblesSettings.Bubbles[index].backColor;
            currentLevelImage.color = color;
            progressBarFillImage.color = color;
            nextLevelImage.color = index == _bubblesSettings.Bubbles.Count - 1 ? _bubblesSettings.Bubbles[0].backColor : _bubblesSettings.Bubbles[index + 1].backColor;

            var levelUpWindow = UIMainController.Instance.GetWindow<UILevelUpWindow>(UIConstants.LevelUpWindow);
            
            if (levelUpWindow == null)
                return;
            
            levelUpWindow.OpenWindow(_currentLevel);
        }

        public void OnPauseButtonClick()
        {
            SessionController.Instance.PauseSession();
            var window = UIMainController.Instance.GetWindow<UIMainMenuWindow>(UIConstants.MainMenuWindow);

            if (window == null)
                throw new NullReferenceException($"{UIConstants.MainMenuWindow} is null ");
            
            window.OpenWindow();
        }

        private void LateUpdate()
        {
            for (var i = 0; i < _bubbleInfos.Count; i++) 
                _bubbleInfos[i].FollowSource();
        }
        
    }
}
