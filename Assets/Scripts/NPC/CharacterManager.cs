using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    public List<CharacterLogic> characters;
    public GameObject iceTrapPrefab;
    public float freezeDuration = 5f;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // Get all characters in the scene
        characters = GameObject.FindObjectsByType<CharacterLogic>(FindObjectsSortMode.None).ToList();
    }

    public IEnumerable<CharacterLogic> GetCharactersOnCell(EnvironmentGridCell cell, float gridSize)
    {
        foreach (var character in characters)
        {
            Vector2 charPos = new Vector2(character.transform.position.x, character.transform.position.z);
            Vector2 cellPos = new Vector2(cell.worldPosition.x, cell.worldPosition.z);
            float distance = Vector2.Distance(charPos, cellPos);

            if (distance < gridSize)
            {
                yield return character;
            }
        }
    }
}