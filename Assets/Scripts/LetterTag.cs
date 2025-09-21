using System.Collections;
using TMPro;
using UnityEngine;

public class LetterTag : MonoBehaviour
{
    private TextMeshPro m_LetterText;
    private SpriteRenderer m_MiddleRenderer;
    private SpriteRenderer m_BackRenderer;

    void Awake()
    {
        m_LetterText = GetComponentInChildren<TextMeshPro>();
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        m_MiddleRenderer = renderers[0];
        m_BackRenderer = renderers[1];
    }

    public string m_Letter;

    public Color m_DefaultMiddleColor;
    public Color m_DefaultBackColor;
    public Color m_CorrectMiddleColor;
    public Color m_CorrectBackColor;
    public Color m_IncorrectMiddleColor;
    public Color m_IncorrectBackColor;

    void Start()
    {
        m_DefaultMiddleColor = m_MiddleRenderer.color;
        m_DefaultBackColor = m_BackRenderer.color;
    }

    public void SetLetter(string letter)
    {
        m_Letter = letter;
        Debug.Log("Setting letter: " + m_Letter);
        Debug.Log("Setting letter text: " + m_Letter.ToUpper());
        Debug.Log("Setting letter text: " + m_LetterText);
        m_LetterText.text = m_Letter.ToUpper();
    }

    public IEnumerator CorrectLerp()
    {
        StopCoroutine(IncorrectFlashLerp());

        float flashDuration = 0.2f;

        float t = 0f;

        while (t < flashDuration)
        {
            float lerpT = t / flashDuration;
            m_MiddleRenderer.color = Color.Lerp(m_DefaultMiddleColor, m_CorrectMiddleColor, lerpT);
            m_BackRenderer.color = Color.Lerp(m_DefaultBackColor, m_CorrectBackColor, lerpT);
            t += Time.deltaTime;
            yield return null;
        }

        m_MiddleRenderer.color = m_CorrectMiddleColor;
        m_BackRenderer.color = m_CorrectBackColor;
    }

    public IEnumerator IncorrectFlashLerp()
    {
        float flashDuration = 0.2f;

        float halfDuration = flashDuration / 2f;
        float t = 0f;

        while (t < halfDuration)
        {
            float lerpT = t / halfDuration;
            m_MiddleRenderer.color = Color.Lerp(m_DefaultMiddleColor, m_IncorrectMiddleColor, lerpT);
            m_BackRenderer.color = Color.Lerp(m_DefaultBackColor, m_IncorrectBackColor, lerpT);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        while (t < halfDuration)
        {
            float lerpT = t / halfDuration;
            m_MiddleRenderer.color = Color.Lerp(m_IncorrectMiddleColor, m_DefaultMiddleColor, lerpT);
            m_BackRenderer.color = Color.Lerp(m_IncorrectBackColor, m_DefaultBackColor, lerpT);
            t += Time.deltaTime;
            yield return null;
        }

        m_MiddleRenderer.color = m_DefaultMiddleColor;
        m_BackRenderer.color = m_DefaultBackColor;
    }
}
