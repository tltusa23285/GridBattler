using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GBGame.Actors
{
    public class ActorCanvas : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI HpText;
        [SerializeField] private Image HpBar;

        public void UpdateHealth(in int c, in int m)
        {
            HpText.text = c.ToString();
            HpBar.fillAmount = Mathf.InverseLerp(0, m, c);
        }
    } 
}