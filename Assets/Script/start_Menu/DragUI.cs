using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Vector3 initialPosition;
    public float maxYPercentage = 0.5f;
    private float minY;
    private float maxY;

    private float flingSpeed = 0f;
    private float currentVelocity = 0f; 
    private const float dragSpeedModifier = 1.2f; 
    private const float smoothTime = 1f;
    private float lastDeltaY = 0f; // 이전 프레임에서의 deltaY 값을 저장

    private void Start()
    {
        initialPosition = transform.position;
        minY = initialPosition.y;

        if(Screen.height <= 800)
        {
            maxYPercentage = 0.75f;
        }

        maxY = initialPosition.y + Screen.height * maxYPercentage;
    }
    public void UpdateInitialPosition()
    {
        initialPosition = transform.position;
    }
    
    public void ResetToInitialPosition()
    {
        transform.position = initialPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float newY = transform.position.y + (eventData.delta.y / dragSpeedModifier);
        transform.position = new Vector3(initialPosition.x, Mathf.Clamp(newY, minY, maxY), initialPosition.z);

        // 순간적인 드래그 강도를 계산
        float dragIntensity = Mathf.Abs(eventData.delta.y - lastDeltaY);
        flingSpeed = eventData.delta.y * (2.0f + dragIntensity); // 드래그 강도를 반영하여 flingSpeed 갱신

        lastDeltaY = eventData.delta.y; // 현재 프레임의 deltaY 값을 저장
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StartCoroutine(FlingEffect());
    }

    IEnumerator FlingEffect()
    {
        while (Mathf.Abs(flingSpeed) > 0.1f)
        {
            float newY = transform.position.y + flingSpeed * Time.deltaTime;
            transform.position = new Vector3(initialPosition.x, Mathf.Clamp(newY, minY, maxY), initialPosition.z);

            flingSpeed = Mathf.SmoothDamp(flingSpeed, 0, ref currentVelocity, smoothTime);

            yield return null;
        }
    }
}