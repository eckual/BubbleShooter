using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Bubbles;

public class UIBubbleInfo : MonoBehaviour, IUIItem<Bubble>, IPoolObject
{
    [SerializeField]
    protected string id;
    [SerializeField]
    protected TMP_Text infoText;

    public Bubble Source { get; set; }

    public string Id
    {
        get { return id; }
        set { id = value; }
    }

    public void Init(Bubble bubble)
    {
        Source = bubble;
        if (Source)
        {
            infoText.text =Source.CurrentNumber.ToString();
            transform.position = new Vector3(Source.transform.position.x, Source.transform.position.y, transform.position.z);
        }
    }

    public void FollowSource()
    {
        if (transform.position.y != Source.transform.position.y)
        {
            transform.position = new Vector3(Source.transform.position.x, Source.transform.position.y, transform.position.z);
        }
    }

    public void Release()
    {
        infoText.text = string.Empty;
        Source = null;
    }
}
