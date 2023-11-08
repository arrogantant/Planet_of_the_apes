using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterToggleManager : MonoBehaviour
{
    public List<GameObject> selectedSubCharacters = new List<GameObject>();

    public void ToggleSubCharacterSelection(GameObject characterPrefab)
    {
        if (selectedSubCharacters.Contains(characterPrefab))
        {
            selectedSubCharacters.Remove(characterPrefab);
            CharacterSelectionManager.Instance.selectedSubCharacters.Remove(characterPrefab);
        }
        else
        {
            selectedSubCharacters.Add(characterPrefab);
            CharacterSelectionManager.Instance.selectedSubCharacters.Add(characterPrefab);
        }
    }
}
