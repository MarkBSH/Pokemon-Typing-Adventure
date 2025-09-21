using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class WorldRotating : MonoBehaviour
{
    #region Singleton

    private static WorldRotating m_Instance;
    public static WorldRotating Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindFirstObjectByType<WorldRotating>();
                if (m_Instance == null)
                {
                    GameObject obj = new("WorldRotating");
                    m_Instance = obj.AddComponent<WorldRotating>();
                }
            }

            DontDestroyOnLoad(m_Instance);
            return m_Instance;
        }
    }

    #endregion

    #region Rotating Management

    public float m_RotationSpeed = 10f;

    void Update()
    {
        transform.Rotate(Vector3.right, m_RotationSpeed * Time.deltaTime);
    }

    #endregion

    #region World Changer

    private Queue<IEnumerator> m_WorldChangeQueue = new();

    void Start()
    {
        StartCoroutine(CoroutineCoordinator());
    }

    private IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (m_WorldChangeQueue.Count > 0)
                yield return StartCoroutine(m_WorldChangeQueue.Dequeue());
            yield return null;
        }
    }


    public List<GameObject> m_WorldRows = new();
    private List<GameObject> m_NewWorldRows = new();
    public List<GameObject> m_WorldRowPrefabs = new();
    private int m_CurrentWorldIndex = 0;
    private int m_CurrentWorldLevel = 0;
    public int CurrentWorldLevel
    {
        get => m_CurrentWorldLevel;
        set
        {
            if (m_CurrentWorldLevel != value)
            {
                m_CurrentWorldLevel = value;
                m_WorldChangeQueue.Enqueue(ChangeWorld());
                Debug.Log($"World level set to: {m_CurrentWorldLevel}");
            }
        }
    }

    public IEnumerator ChangeWorld()
    {
        m_CurrentWorldIndex++;

        while (m_WorldRows[0].transform.rotation.x >= -100f && m_WorldRows[0].transform.rotation.x <= -200f)
        {
            Debug.Log(m_WorldRows[0].transform.rotation.x);
            yield return null;
        }
        for (int i = 0; i < m_WorldRows.Count; i++)
        {
            GameObject newRow = Instantiate(m_WorldRowPrefabs[m_CurrentWorldIndex], m_WorldRows[i].transform.position, m_WorldRows[i].transform.rotation, gameObject.transform);
            m_NewWorldRows.Add(newRow);
            Destroy(m_WorldRows[i]);

            yield return new WaitForSeconds(18 / Mathf.Abs(m_RotationSpeed));
        }

        m_WorldRows.Clear();
        m_WorldRows = new List<GameObject>(m_NewWorldRows);
        m_NewWorldRows.Clear();
    }

    #endregion
}
