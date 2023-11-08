using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{    
    public Transform target;
    public Slider healthBar;
    private Player playerScript;
    private Vector3 offset = new Vector3(0, -1, 0);

    private void Start()
    {
        if (target == null)
        {
            // 새 씬에서 플레이어 찾기
            target = FindObjectOfType<Player>().transform;
        }
        if (target != null)
        {
            playerScript = target.GetComponent<Player>();
            if (healthBar == null)
            {
                // 새 씬에서 체력바 UI 찾기
                healthBar = FindObjectOfType<Slider>();
            }
            if (playerScript != null && healthBar != null)
            {
                healthBar.maxValue = playerScript.PlayerHP;
                healthBar.value = playerScript.PlayerHP;
            }
        }
    }

    private void LateUpdate()
    {
        if (playerScript != null && healthBar != null)
        {
            healthBar.transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
            healthBar.value = playerScript.PlayerHP;
        }
    }
}