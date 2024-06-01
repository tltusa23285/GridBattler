using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Text = TMPro.TextMeshProUGUI;
using GBGame.Data;

namespace GBGame.Actors
{
    public class ActionCard : MonoBehaviour
    {
        [SerializeField] bool IsMiniCard = false;

        [Header("Refs")]
        [SerializeField] private Image Image;
        [SerializeField] private Text NameText;
        [SerializeField] private Text DescriptionText;
        [SerializeField] private Text DamageText;

        [Header("Debug")]
        public ActionInfoData ActionInfoDebug;
        [ContextMenu("Load Debug")]
        private void TestCard()
        {
            FillCard(ActionInfoDebug);
        }

        public void FillCard(in ActionInfoData data)
        {
            if (IsMiniCard) FillMini(data);
            else FillFull(data);
        }

        private void FillFull(in ActionInfoData data)
        {
            Sprite spr = Addressables.LoadAssetAsync<Sprite>(data.Image).WaitForCompletion();
            if (spr == null) Debug.LogWarning($"Given image was null {data.Image}");
            Image.sprite = spr;
            NameText.text = data.Name;
            DescriptionText.text = data.Description;
            DamageText.text = data.Damage.ToString();
        }

        private void FillMini(in ActionInfoData data)
        {
            Sprite spr = Addressables.LoadAssetAsync<Sprite>(data.Thumbnail).WaitForCompletion();
            if (spr == null) Debug.LogWarning($"Given image was null {data.Thumbnail}");
            Image.sprite = spr;
        }
    } 
}
