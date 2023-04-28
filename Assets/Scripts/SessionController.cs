using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bubbles;

public class SessionController : MonoSingleton<SessionController>
{
    [SerializeField]
    private BubblesController bubblesController;
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private ScoreController scoreController;
    [SerializeField]
    private VFXController vfxController;

    public BubblesController BubblesController
    {
        get { return bubblesController; }
    }

    public PlayerController PlayerController
    {
        get { return playerController; }
    }

    public ScoreController ScoreController
    {
        get { return scoreController; }
    }

    public VFXController VFXController
    {
        get { return vfxController; }
    }

    public bool IsRunning { get; set; }

    public override void Init()
    {
        scoreController.Init();
        bubblesController.Init();
        playerController.Init();
        vfxController.Init();
    }

    public void StartSession()
    {
        IsRunning = true;
    }

    public void PauseSession()
    {
        IsRunning = false;
    }
}
