using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Bubbles;

public class UIBubblesInfoWindow : UIBaseWindow
{
    public const string DEFAULT_BUBBLE_INFO_ID = "BubbleInfo";
    [SerializeField]
    private UISwapBubblesGroup projectileSelectionGroup;

    [SerializeField]
    private UIBubbleInfosPool bubbleInfosPool;
    [SerializeField]
    private UINotification perfectNotification;

    private ScoreController scoreController;
    private int currentLevel;

    [Header("Level")]
    [SerializeField]
    private TMP_Text score;
    [SerializeField]
    private TMP_Text currentLevelText;
    [SerializeField]
    private Image currentLevelImage;
    [SerializeField]
    private TMP_Text nextLevelText;
    [SerializeField]
    private Image nextLevelImage;
    [SerializeField]
    private UIProgressBar levelProgressBar;
    [SerializeField]
    private Image progressBarFillImage;

    private List<UIBubbleInfo> bubbleInfos = new List<UIBubbleInfo>();
    private BubblesSettings bubblesSettings;

    public override void Init()
    {
        base.Init();
        var sessionController = SessionController.Instance;
        var bubblesController = sessionController.BubblesController;
        bubblesController.OnBubbleAdded += OnBubblesAdded;
        bubblesController.OnBubbleReleased += OnBubbleReleased;
        bubblesController.OnBubblesCleared += perfectNotification.Show;
        bubblesSettings = ResourceManager.GetResource<BubblesSettings>(GameConstants.BUBBLE_SETTINGS);

        for (int i = 0; i < bubblesController.Bubbles.Count; i++)
        {
            var bubble = bubblesController.Bubbles[i];
            OnBubblesAdded(bubble);
        }

        scoreController = sessionController.ScoreController;
        scoreController.OnScoreChanged += OnScoreChanged;
        OnScoreChanged(scoreController.Score);

        projectileSelectionGroup.Init();
    }

    private void OnBubblesAdded(Bubble bubble)
    {
        if (bubbleInfos.FindIndex(x => x.Source == bubble) == -1)
        {
            var bubbleInfo = bubbleInfosPool.GetOrInstantiate(DEFAULT_BUBBLE_INFO_ID);
            bubbleInfo.Init(bubble);
            bubbleInfos.Add(bubbleInfo);
            bubbleInfo.gameObject.SetActive(true);
        }
    }

    private void OnBubbleReleased(Bubble bubble)
    {
        var bubbleInfo = bubbleInfos.Find(x => x.Source == bubble);
        if (bubbleInfo)
        {
            bubbleInfosPool.Release(bubbleInfo);
            bubbleInfo.gameObject.SetActive(false);
            bubbleInfos.Remove(bubbleInfo);
        }
    }

    private void OnScoreChanged(int newScore)
    {
        score.text = newScore.ToString();

        var level = scoreController.LevelSettings.GetLevelByScore(newScore);
        if (currentLevel != level)
        {
            currentLevel = level;
            ChangeLevel();
        }
        levelProgressBar.SetData(newScore,
                                 scoreController.LevelSettings.GetMaxScoreByLevel(currentLevel - 1),
                                 scoreController.LevelSettings.GetMaxScoreByLevel(currentLevel));
    }

    private void ChangeLevel()
    {
        currentLevelText.text = currentLevel.ToString();
        nextLevelText.text = (currentLevel + 1).ToString();

        var index = currentLevel % bubblesSettings.Bubbles.Count;
        var color = bubblesSettings.Bubbles[index].backColor;
        currentLevelImage.color = color;
        progressBarFillImage.color = color;
        if (index == bubblesSettings.Bubbles.Count - 1)
        {
            nextLevelImage.color = bubblesSettings.Bubbles[0].backColor;
        }
        else
        {
            nextLevelImage.color = bubblesSettings.Bubbles[index + 1].backColor;
        }

        var levelUpWindow = UIMainController.Instance.GetWindow<UILevelUpWindow>(UIConstants.LEVEL_UP_WINDOW);
        levelUpWindow?.OpenWindow(currentLevel);
    }

    public void OnPauseButtonClick()
    {
        SessionController.Instance.PauseSession();
        var window = UIMainController.Instance.GetWindow<UIMainMenuWindow>(UIConstants.MAIN_MENU_WINDOW);
        window?.OpenWindow();
    }

    private void LateUpdate()
    {
        for (int i = bubbleInfos.Count - 1; i >= 0; i--)
        {
            bubbleInfos[i].FollowSource();
        }
    }
}
