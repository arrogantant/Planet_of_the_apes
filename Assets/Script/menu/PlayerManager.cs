using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public GameObject selectedCharacterPrefab;
    public GameObject spawnedPlayer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로딩되면 선택된 캐릭터를 인스턴스화합니다.
        if ((scene.name != "Main_menu") && selectedCharacterPrefab != null)
        {
            SpawnSelectedCharacter();
        }
    }

    private void SpawnSelectedCharacter()
    {
        // 기존에 스폰된 플레이어가 있으면 제거합니다.
        if (spawnedPlayer != null)
        {
            Destroy(spawnedPlayer);
        }

        // 선택된 캐릭터 프리팹을 인스턴스화합니다.
        spawnedPlayer = Instantiate(selectedCharacterPrefab, Vector3.zero, Quaternion.identity);
    }

    public void ToggleCharacterSelection(GameObject playerPrefab)
    {
        selectedCharacterPrefab = playerPrefab;
    }
}
