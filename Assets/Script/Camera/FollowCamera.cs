using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;

    private void Start()
    {
        // Player 태그가 붙은 게임 오브젝트의 Transform을 찾아 target에 할당
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject)
            {
                target = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player 객체를 찾을 수 없습니다.");
                return;
            }
        }

        offset = transform.position - target.position; 
    }

    private void LateUpdate()
    {
        if(target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = desiredPosition;
    }
}
