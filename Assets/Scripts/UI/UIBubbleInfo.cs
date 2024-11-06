using Bubbles;
using Extensions;
using TMPro;
using UnityEngine;
using Utils;

namespace UI
{
    public class UIBubbleInfo : MonoBehaviour, IUIItem<Bubble>, IPoolObject
    {
        [SerializeField]protected string id;
        [SerializeField]protected TMP_Text infoText;

        public Bubble Source { get; set; }

        public string Id
        {
            get => id;
            set => id = value;
        }

        public void Init(Bubble bubble)
        {
            Source = bubble;
            if (Source == null) return;
            
            infoText.text = Source.CurrentScore.ToString();
            transform.position = new Vector3(Source.transform.position.x, Source.transform.position.y, transform.position.z);
        }

        public void FollowSource()
        {
            var sourcePos = Source.transform.position;
            var transformPos = transform.position;
        
            if (sourcePos.ApproximatelyEqual(transformPos,0.1f,true)) return;
            transform.position = new Vector3(sourcePos.x, sourcePos.y, transform.position.z);
        }

        public void Release()
        {
            infoText.text = string.Empty;
            Source = null;
        }
    }
}
