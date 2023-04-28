using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINotification : MonoBehaviour
{
    [SerializeField]
    private GameObject notificaton;

    public void Show()
    {
        notificaton.SetActive(true);
    }

    public void Hide()
    {
        notificaton.SetActive(false);
    }
}
