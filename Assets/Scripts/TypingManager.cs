using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TypingManager : MonoBehaviour
{
    #region Singleton

    private static TypingManager m_Instance;
    public static TypingManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindFirstObjectByType<TypingManager>();
                if (m_Instance == null)
                {
                    GameObject obj = new("TypingManager");
                    m_Instance = obj.AddComponent<TypingManager>();
                }
            }

            DontDestroyOnLoad(m_Instance);
            return m_Instance;
        }
    }

    #endregion

    #region Spawn Management

    public List<PokemonNames> m_PokemonList = new();
    private List<PokemonNames> m_EasyPokemon = new();
    private List<PokemonNames> m_MediumPokemon = new();
    private List<PokemonNames> m_HardPokemon = new();
    private int m_EasyDifficultyChance = 60;
    private int m_MediumDifficultyChance = 30;
    private int m_HardDifficultyChance = 10;

    private void Start()
    {
        PokemonNames[] allPokemon = Resources.LoadAll<PokemonNames>("PokemonNames");
        m_PokemonList.AddRange(allPokemon);

        foreach (var pokemon in allPokemon)
        {
            switch (pokemon.spellRarity)
            {
                case PokemonNames.PokemonDifficulty.Easy:
                    m_EasyPokemon.Add(pokemon);
                    break;
                case PokemonNames.PokemonDifficulty.Medium:
                    m_MediumPokemon.Add(pokemon);
                    break;
                case PokemonNames.PokemonDifficulty.Hard:
                    m_HardPokemon.Add(pokemon);
                    break;
            }
        }

        SpawnPokemon();
    }

    public float m_SpawnInterval = 4f;
    private float m_TimeSinceLastSpawn = 0f;

    private void Update()
    {
        if (GameManager.Instance.m_IsGameActive)
        {
            m_TimeSinceLastSpawn += Time.deltaTime;

            if (m_TimeSinceLastSpawn >= m_SpawnInterval)
            {
                SpawnPokemon();
                m_TimeSinceLastSpawn = 0f;
            }
        }
    }

    private void SpawnPokemon()
    {
        switch (GameManager.Instance.m_Score)
        {
            case >= 300:
                m_SpawnInterval = 3f;
                m_EasyDifficultyChance = 25;
                m_MediumDifficultyChance = 35;
                m_HardDifficultyChance = 40;
                WorldRotating.Instance.CurrentWorldLevel = 4;
                break;
            case >= 175:
                m_SpawnInterval = 4f;
                m_EasyDifficultyChance = 35;
                m_MediumDifficultyChance = 45;
                m_HardDifficultyChance = 20;
                WorldRotating.Instance.CurrentWorldLevel = 3;
                break;
            case >= 100:
                m_SpawnInterval = 5f;
                m_EasyDifficultyChance = 50;
                m_MediumDifficultyChance = 35;
                m_HardDifficultyChance = 15;
                WorldRotating.Instance.CurrentWorldLevel = 2;
                break;
            case >= 50:
                m_SpawnInterval = 6f;
                m_EasyDifficultyChance = 60;
                m_MediumDifficultyChance = 30;
                m_HardDifficultyChance = 10;
                WorldRotating.Instance.CurrentWorldLevel = 1;
                break;
            default:
                m_SpawnInterval = 8f;
                m_EasyDifficultyChance = 70;
                m_MediumDifficultyChance = 25;
                m_HardDifficultyChance = 5;
                WorldRotating.Instance.CurrentWorldLevel = 0;
                break;
        }

        if (m_PokemonList.Count > 0)
        {
            PokemonNames selectedPokemon;

            int roll = Random.Range(1, 101);
            if (roll <= m_EasyDifficultyChance)
            {
                selectedPokemon = m_EasyPokemon[Random.Range(0, m_EasyPokemon.Count)];
            }
            else if (roll <= m_EasyDifficultyChance + m_MediumDifficultyChance)
            {
                selectedPokemon = m_MediumPokemon[Random.Range(0, m_MediumPokemon.Count)];
            }
            else
            {
                selectedPokemon = m_HardPokemon[Random.Range(0, m_HardPokemon.Count)];
            }

            Vector3 spawnPosition = new(Random.Range(-5f, 5f), 7f, -20f);
            GameObject spawnedPokemon = Instantiate(selectedPokemon.PokemonPrefab, spawnPosition, Quaternion.identity);
            spawnedPokemon.GetComponent<Pokemon>().m_PokemonName = selectedPokemon.Name;
            m_ActivePokemon.Add(spawnedPokemon);

            foreach (char letter in selectedPokemon.Name.ToCharArray())
            {
                m_ActivePokemonNames.Add(letter.ToString());
            }
        }
    }

    #endregion

    #region Input Management

    public List<string> m_ActivePokemonNames = new();
    public List<GameObject> m_ActivePokemon = new();

    public void ProcessInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (context.control.name.Length == 1)
            {
                CheckInput(context.control.name);
            }
        }
    }

    private void CheckInput(string input)
    {
        if (m_ActivePokemonNames[0] == input)
        {
            m_ActivePokemonNames.RemoveAt(0);
            m_ActivePokemon[0].GetComponent<Pokemon>().LetterCorrect();
            GameManager.Instance.AddScore(1);
            GameManager.Instance.RecordResponseTime(Time.timeSinceLevelLoad);
            Debug.Log($"Correct! Score: {GameManager.Instance.m_Score}");
        }
        else
        {
            m_ActivePokemon[0].GetComponent<Pokemon>().LetterIncorrect();
            GameManager.Instance.AddScore(-2);
            GameManager.Instance.RecordFail();
        }
    }

    #endregion
}
