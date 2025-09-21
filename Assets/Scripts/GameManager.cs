using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager m_Instance;
    public static GameManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindFirstObjectByType<GameManager>();
                if (m_Instance == null)
                {
                    GameObject obj = new("GameManager");
                    m_Instance = obj.AddComponent<GameManager>();
                }
            }

            DontDestroyOnLoad(m_Instance);
            return m_Instance;
        }
    }

    #endregion

    #region Game Management

    public bool m_IsGameActive = false;

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        m_IsGameActive = true;
        m_Score = 0;
        m_Lives = 3;
        m_TotalResponses = 0;
        m_TotalResponseTime = 0f;
        m_AverageResponseTime = 0f;
        m_slowestResponseTime = 0f;
        m_TotalFails = 0;
        m_CurrentConsecutiveFails = 0;
        m_MostConsecutiveFails = 0;
        m_GameTime = 0f;
    }

    public void EndGame()
    {
        m_IsGameActive = false;
        // Handle end game logic here (e.g., show score, save data, etc.)
    }

    #endregion

    #region Score & Lives Management

    public int m_Score = 0;
    public void AddScore(int score)
    {
        m_Score += score;
    }

    public int m_Lives = 3;
    public void LoseLife()
    {
        m_Lives--;
        if (m_Lives <= 0)
        {
            EndGame();
        }
    }

    #endregion

    #region Response Time Tracking

    public int m_TotalResponses = 0;
    public float m_TotalResponseTime = 0f;
    public float m_AverageResponseTime = 0f;
    public float m_slowestResponseTime = 0f;

    public void RecordResponseTime(float responseTime)
    {
        m_TotalResponses++;
        m_TotalResponseTime += responseTime;
        m_AverageResponseTime = m_TotalResponseTime / m_TotalResponses;

        if (responseTime > m_slowestResponseTime)
        {
            m_slowestResponseTime = responseTime;
        }

        m_CurrentConsecutiveFails = 0;
    }

    #endregion

    #region Fails Tracking

    public int m_TotalFails = 0;
    private int m_CurrentConsecutiveFails = 0;
    public int m_MostConsecutiveFails = 0;

    public void RecordFail()
    {
        m_TotalFails++;
        m_CurrentConsecutiveFails++;
        if (m_CurrentConsecutiveFails > m_MostConsecutiveFails)
        {
            m_MostConsecutiveFails = m_CurrentConsecutiveFails;
        }
    }

    #endregion

    #region Time Tracking

    public float m_GameTime = 0f;
    void FixedUpdate()
    {
        m_GameTime += Time.fixedDeltaTime;
    }

    #endregion
}
