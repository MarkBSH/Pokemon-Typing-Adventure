using UnityEngine;

[CreateAssetMenu(fileName = "PokemonNames", menuName = "Scriptable Objects/PokemonNames")]
public class PokemonNames : ScriptableObject
{
    public string Name;

    public GameObject PokemonPrefab;

    public enum PokemonDifficulty
    {
        Easy,
        Medium,
        Hard
    };
    public PokemonDifficulty spellRarity;
}
