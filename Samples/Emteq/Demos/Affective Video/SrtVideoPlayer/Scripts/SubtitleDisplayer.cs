using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EmteqLabs.Video
{
  public class SubtitleDisplayer : MonoBehaviour
  {
    public TextAsset Subtitle;
    public Text Text;
    public Text Text2;

    [Range(0, 1)] public float FadeTime;

    public IEnumerator Begin()
    {
      var currentlyDisplayingText = Text;
      var fadedOutText = Text2;

      currentlyDisplayingText.text = string.Empty;
      fadedOutText.text = string.Empty;

      currentlyDisplayingText.gameObject.SetActive(true);
      fadedOutText.gameObject.SetActive(true);

      var parser = new SRTParser(Subtitle);

      var startTime = Time.time;
      SubtitleBlock currentSubtitle = null;
      while (true)
      {
        var elapsed = Time.time - startTime;
        var subtitle = parser.GetForTime(elapsed);
        if (subtitle != null)
        {
          if (!subtitle.Equals(currentSubtitle))
          {
            currentSubtitle = subtitle;

            // Swap references around
            var temp = currentlyDisplayingText;
            currentlyDisplayingText = fadedOutText;
            fadedOutText = temp;

            // Switch subtitle text
            currentlyDisplayingText.text = currentSubtitle.Text;

            // Yield a bit for the fade out to get part-way
            yield return new WaitForSeconds(FadeTime / 3);

            // Fade in the new current
            //yield return FadeTextIn(currentlyDisplayingText);
          }

          yield return null;
        }
        else
        {
          currentlyDisplayingText.gameObject.SetActive(false);
          fadedOutText.gameObject.SetActive(false);
          yield break;
        }
      }
    }

    void OnValidate()
    {
      FadeTime = ((int) (FadeTime * 10)) / 10f;
    }
  }
}