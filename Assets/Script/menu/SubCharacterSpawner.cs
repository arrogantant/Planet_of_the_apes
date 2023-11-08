using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCharacterSpawner : MonoBehaviour
{
    public Transform playerTransform;  // 플레이어의 Transform을 참조합니다.

    void Start()
    {
        if (playerTransform == null)  // 만약 playerTransform이 Inspector에서 할당되지 않았다면
        {
            // "Player" 태그가 할당된 게임 오브젝트를 찾습니다.
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject)
            {
                playerTransform = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player object not found!");
                return;
            }
        }

        // 플레이어의 위치나 다른 위치를 기반으로 서브 캐릭터를 생성합니다.
        Vector3 spawnPosition = playerTransform.position + Vector3.right;  // 예제로 오른쪽에 생성

        foreach (GameObject subCharacterPrefab in CharacterSelectionManager.Instance.selectedSubCharacters)
        {
            Instantiate(subCharacterPrefab, spawnPosition, Quaternion.identity);
            spawnPosition += Vector3.right;  // 다음 캐릭터는 또 다시 오른쪽에 생성
        }
    }
}
