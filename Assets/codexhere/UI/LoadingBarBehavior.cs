using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace codexhere.UI {
    [ExecuteInEditMode]
    [SelectionBase]
    public class LoadingBarBehavior : MonoBehaviour {
        [SerializeField] RectTransform progressBackgroundTransform;
        [SerializeField] RectTransform progressTransform;
        [SerializeField] TMP_Text progressText;

        [Range(0f, 100f)] public float Percentage;

        private void Update() {
            float newWidth = progressBackgroundTransform.rect.width * (Percentage / 100);
            progressTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            progressText.text = "Loaded: " + string.Format("{0:F2}", Percentage) + "%";
        }

        public async void FadeTo(float fadeValue, float fadeAmount = 1) {
            CanvasGroup cg = GetComponent<CanvasGroup>();

            while (cg.alpha != fadeValue) {
                int direction = fadeValue > cg.alpha ? 1 : -1;

                cg.alpha += direction * Time.deltaTime * fadeAmount;
                await Task.Yield();
            }
        }
    }
}
