using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bubbles;
public class UISwapBubblesGroup : MonoBehaviour
{
    public const string SWAP_TRIGGER = "Swap";

    [SerializeField]
    private UISwapBubble currentBubble;
    [SerializeField]
    private UISwapBubble nextBubble;
    [SerializeField]
    private Animator animator;
    private BubblesSettings bubbleSettings;

    public UISwapBubble CurrentBubble
    {
        get { return currentBubble; }
    }

    public UISwapBubble NextBubble
    {
        get { return nextBubble; }
    }

    private bool afterShoot;
    private SessionController sessionController;

    public void Init()
    {
        bubbleSettings = ResourceManager.GetResource<BubblesSettings>(GameConstants.BUBBLE_SETTINGS);

        currentBubble.Init(Random.Range(1, bubbleSettings.Bubbles.Count - 1));
        nextBubble.Init(Random.Range(1, bubbleSettings.Bubbles.Count - 1));

        nextBubble.Button.onClick.AddListener(OnSwapButtonClick);

        sessionController = SessionController.Instance;
        sessionController.BubblesController.CurrentPower = CurrentBubble.Power;
        sessionController.PlayerController.BubbleShootController.OnShootEnded += OnEndShoot;
        sessionController.PlayerController.BubbleShootController.OnShootStarted += OnStartShoot;
    }

    private void OnEndShoot()
    {
        afterShoot = true;
        nextBubble.Button.onClick.RemoveListener(OnSwapButtonClick);
        animator.SetTrigger(SWAP_TRIGGER);
    }

    private void OnStartShoot()
    {
        currentBubble.gameObject.SetActive(false);
    }

    public void OnSwapButtonClick()
    {
        afterShoot = false;
        nextBubble.Button.onClick.RemoveListener(OnSwapButtonClick);
        animator.SetTrigger(SWAP_TRIGGER);
    }

    public void OnFinishSwap()
    {
        currentBubble.gameObject.SetActive(true);
        nextBubble.gameObject.SetActive(true);

        var cachedBubble = currentBubble;
        currentBubble = nextBubble;
        nextBubble = cachedBubble;

        nextBubble.transform.SetAsLastSibling();
        nextBubble.Button.onClick.AddListener(OnSwapButtonClick);
        if (afterShoot)
        {
            nextBubble.Init(Random.Range(1, bubbleSettings.Bubbles.Count - 1));
        }
        sessionController.BubblesController.CurrentPower = CurrentBubble.Power;
    }
}
