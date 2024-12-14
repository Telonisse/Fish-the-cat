using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor.Rendering;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject textObject;
    [SerializeField] float time;

    private bool textDone = true;

    private IEnumerator enumerator;

    public void StartText(string dialougeText)
    {
        text.text = null;
        textDone = false;
        textObject.SetActive(true);
        enumerator = PlayText(dialougeText);
        StartCoroutine(enumerator);
    }

    public void StopText()
    {
        StopCoroutine(enumerator);
        text.text = null;
        textObject.SetActive(false);
    }

    IEnumerator PlayText(string dialougeText)
    {
        for (int i = 0; i < dialougeText.Length + 1; i++)
        {
            string newText = dialougeText.Substring(0, i);
            text.text = newText;
            yield return new WaitForSecondsRealtime(time);
        }
        textDone = true;
        yield return null;
    }

    public bool IsTextDone()
    {
        return textDone;
    }
}
