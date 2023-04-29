using UnityEngine;

namespace UI
{
    public class UINotification : MonoBehaviour
    {
        [SerializeField] private GameObject notification;

        public void Show() => notification.SetActive(true);

        public void Hide() => notification.SetActive(false);
    }
}
