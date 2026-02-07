using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffectTMP : MonoBehaviour
{
    [Header("Настройки скорости")]
    [SerializeField] private float delayBetweenChars = 0.05f;//задержка между символами
    [SerializeField] private float delayAfterPunctuation = 0.5f; //доп.задержка после знаков препинания

    [Header("Эффекты")]
    [SerializeField] private bool useSound = false;
    [SerializeField] private AudioClip typeSound;
    [SerializeField] private bool showCursor = true;
    [SerializeField] private string cursorChar = "|";

    private TMP_Text tmpText;
    private string fullText;
    private AudioSource audioSource;

    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null && useSound)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        fullText = tmpText.text;
        tmpText.text = "";
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            tmpText.text += fullText[i];

            //звук можно навесить)))
            if (useSound && typeSound != null && !char.IsWhiteSpace(fullText[i]))
            {
                audioSource.PlayOneShot(typeSound);
            }

            float delay = delayBetweenChars;

            if (i < fullText.Length - 1)
            {
                char nextChar = fullText[i + 1];
                if (nextChar == '.' || nextChar == '!' || nextChar == '?' || nextChar == ',')
                {
                    delay = delayAfterPunctuation;
                }
            }

            if (showCursor)
            {
                tmpText.text += cursorChar;
                yield return new WaitForSeconds(delay);
                tmpText.text = tmpText.text.Remove(tmpText.text.Length - 1);
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }
        }

        if (showCursor)
        {
            tmpText.text += cursorChar;
        }
    }

    public void StartTyping(string newText = "")
    {
        if (!string.IsNullOrEmpty(newText))
        {
            fullText = newText;
        }

        StopAllCoroutines();
        tmpText.text = "";
        StartCoroutine(TypeText());
    }
}