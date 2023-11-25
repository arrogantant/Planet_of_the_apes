using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveImage : MonoBehaviour
{
    public float speed = 2.0f;
    public float changeDirectionTime = 2.0f;
    public Vector2 boundary; // 화면 경계를 설정하는 변수

    private Vector3 moveDirection;
    private Vector3 startPosition;
    private float timer;

    void Start()
    {
        startPosition = transform.localPosition;
        ChangeDirection();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > changeDirectionTime)
        {
            ChangeDirection();
            timer = 0;
        }

        Vector3 newPosition = transform.localPosition + moveDirection * speed * Time.deltaTime;
        newPosition = RestrictWithinBoundary(newPosition);
        transform.localPosition = newPosition;
    }

    void ChangeDirection()
    {
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        moveDirection = new Vector3(randomX, randomY, 0).normalized;
    }

    Vector3 RestrictWithinBoundary(Vector3 position)
    {
        // X와 Y 좌표를 경계 내로 제한합니다.
        position.x = Mathf.Clamp(position.x, -boundary.x + startPosition.x, boundary.x + startPosition.x);
        position.y = Mathf.Clamp(position.y, -boundary.y + startPosition.y, boundary.y + startPosition.y);
        return position;
    }
}
