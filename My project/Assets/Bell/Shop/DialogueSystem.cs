using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject textObject;
    [SerializeField] float time;

    public void StartText(string dialougeText)
    {
        textObject.SetActive(true);
        StartCoroutine(PlayText(dialougeText));
    }

    IEnumerator PlayText(string dialougeText)
    {
        for (int i = 0; i < dialougeText.Length + 1; i++)
        {
            string newText = dialougeText.Substring(0, i);
            text.text = newText;
            yield return new WaitForSecondsRealtime(time);
        }
        yield return null;
    }
}
