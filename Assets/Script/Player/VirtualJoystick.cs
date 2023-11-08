using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private RectTransform baseRect; // 조이스틱 배경의 RectTransform
    private RectTransform handleRect; // 조이스틱 핸들의 RectTransform
    public Vector2 InputDirection { get; private set; } // 조이스틱의 입력 방향

    void Start()
    {
        baseRect = GetComponent<RectTransform>();
        handleRect = transform.GetChild(0).GetComponent<RectTransform>();
        InputDirection = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - (Vector2)baseRect.position;
        InputDirection = (direction.magnitude > baseRect.sizeDelta.x / 2f) ? direction.normalized : direction / (baseRect.sizeDelta.x / 2f);
        handleRect.anchoredPosition = (InputDirection * baseRect.sizeDelta.x / 2f) * 0.4f; // 조이스틱 핸들의 움직임 범위를 조절합니다.
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputDirection = Vector2.zero;
        handleRect.anchoredPosition = Vector2.zero;
    }
}
