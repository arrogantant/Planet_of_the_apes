using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shop_statusUI : MonoBehaviour
{
    private Vector3 savedPosition;

    private void OnEnable()
    {
        // 오브젝트가 활성화될 때 현재 위치를 저장합니다.
        savedPosition = transform.position;
    }

    private void OnDisable()
    {
        // 오브젝트가 비활성화될 때 저장된 위치로 돌아갑니다.
        transform.position = savedPosition;
    }
}
