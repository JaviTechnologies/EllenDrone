using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EllenDrone
{
    public class ScoreUI : MonoBehaviour
    {
        private Text scoreText;

        public GameObject scoreUIPrefab;

        IEnumerator Start()
        {
            yield return null;

            // instantiate and allocate score hud in the canvas
            GameObject scoreUIGO = Instantiate(scoreUIPrefab, this.transform);
            RectTransform scoreUIRect = scoreUIGO.transform as RectTransform;
            scoreUIRect.anchorMin = Vector2.one;
            scoreUIRect.anchorMax = Vector2.one;
            scoreUIRect.pivot = Vector2.one;
            scoreUIRect.anchoredPosition = Vector2.zero;

            // keep local ref to the text
            scoreText = scoreUIGO.GetComponent<Text>();

            // init score text
            scoreText.text = "Score: 0";
        }

        public void UpdateScoreUI(Scorer scorer)
        {
            scoreText.text = string.Format("Score: {0}", scorer.CurrentScore);
        }
    }
}

