using System.Collections.Generic;
using UnityEngine;

public class Pokemon : MonoBehaviour
{
    public GameObject m_LetterTagPrefab;
    private List<GameObject> m_LetterTags = new();
    private float m_LetterSpacing = 0.9f;
    private float m_YOffset = -2.5f;

    public string m_PokemonName;
    private float m_BaseSize = 0.1f;
    private float m_Speed = 1f;
    private float m_MaxXPosition = 5f;
    private float m_MinXPosition = -5f;

    private bool m_GetNewMoveLocation = true;
    private float m_NewMoveLocation;

    public int m_CurrentLetterIndex = 0;

    void Start()
    {
        int letterCount = m_PokemonName.Length;
        float totalWidth = (letterCount - 1) * m_LetterSpacing;
        for (int i = 0; i < letterCount; i++)
        {
            float x = -totalWidth / 2f + i * m_LetterSpacing;
            GameObject letterTagObj = Instantiate(m_LetterTagPrefab, transform.position + new Vector3(x, m_YOffset, 0f), Quaternion.identity, transform);
            LetterTag letterTag = letterTagObj.GetComponent<LetterTag>();
            letterTag.SetLetter(m_PokemonName[i].ToString());
            m_LetterTags.Add(letterTagObj);
        }

        transform.localScale = new Vector3(m_BaseSize, m_BaseSize, m_BaseSize);
    }

    void Update()
    {
        transform.Translate(Vector3.down * m_Speed * Time.deltaTime);

        if (transform.position.y < -2.5f)
        {
            for (int i = 0; i < m_PokemonName.Length - m_CurrentLetterIndex; i++)
            {
                TypingManager.Instance.m_ActivePokemonNames.RemoveAt(0);
            }
            Destroy(gameObject);
            TypingManager.Instance.m_ActivePokemon.RemoveAt(0);
            GameManager.Instance.LoseLife();
        }

        float clampedX = Mathf.Clamp(transform.position.x, m_MinXPosition, m_MaxXPosition);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

        if (transform.localScale.x < 0.8f)
        {
            transform.localScale *= 1.005f;
        }

        if (m_GetNewMoveLocation)
        {
            m_NewMoveLocation = Random.Range(m_MinXPosition, m_MaxXPosition);
            m_GetNewMoveLocation = false;
        }
        else
        {
            float step = 2f * Time.deltaTime;
            transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, m_NewMoveLocation, step), transform.position.y, transform.position.z);
            if (Mathf.Abs(transform.position.x - m_NewMoveLocation) < 0.1f)
            {
                m_GetNewMoveLocation = true;
            }
        }
    }

    public void LetterCorrect()
    {
        if (m_CurrentLetterIndex < m_LetterTags.Count)
        {
            StartCoroutine(m_LetterTags[m_CurrentLetterIndex].GetComponent<LetterTag>().CorrectLerp());
            m_CurrentLetterIndex++;
            if (m_CurrentLetterIndex >= m_LetterTags.Count)
            {
                Destroy(gameObject);
                TypingManager.Instance.m_ActivePokemon.RemoveAt(0);
            }
        }
    }

    public void LetterIncorrect()
    {
        if (m_CurrentLetterIndex < m_LetterTags.Count)
        {
            StartCoroutine(m_LetterTags[m_CurrentLetterIndex].GetComponent<LetterTag>().IncorrectFlashLerp());
        }
    }
}
