using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    public float speed;
    private Vector2 inputVec;
    Rigidbody2D rigid;
    public int PlayerHP;
    private bool isTakingDamage = false; // 플레이어가 데미지를 입고 있는지 확인하기 위한 플래그
    private bool canMove = true;
    private Animator animator;
    public Collider2D attackCollider;
    private Coroutine damageCoroutine;
    public VirtualJoystick virtualJoystick;
    private bool joystickInitialized = false;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
    }
    void Start()
    {
        // 1초마다 공격 콜라이더를 활성화하는 ToggleAttack 메서드를 호출합니다.
        InvokeRepeating("ToggleAttack", 0, 1f);
    }
    private void Update()
    {
        // VirtualJoystick이 아직 찾아지지 않았다면 찾습니다.
        if (!joystickInitialized)
        {
            virtualJoystick = FindObjectOfType<VirtualJoystick>();

            if (virtualJoystick != null)
            {
                joystickInitialized = true; // 찾았으니 더 이상 찾지 않도록 플래그를 설정합니다.
            }
        }
    }
    
    void FixedUpdate() 
    {
        if (canMove && virtualJoystick != null)
        {
            Vector2 nextVec = virtualJoystick.InputDirection * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);

            if (nextVec != Vector2.zero)
            {
                animator.SetTrigger("Walk");
                Vector3 originalScale = transform.localScale;
                if(nextVec.x > 0)
                {
                    transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                }
                else if(nextVec.x < 0)
                {
                    transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                }
            }
            else
            {
                animator.SetTrigger("Idle");
            }
        }
        
    }
    void ToggleAttack()
    {
        attackCollider.enabled = true; // 콜라이더를 활성화합니다.
        animator.SetBool("Attack", true);
        Invoke("DisableAttack", 0.4f); // 0.4초 후 콜라이더를 비활성화하는 메서드를 호출합니다.
    }

    // 공격 콜라이더를 비활성화하는 메서드
    void DisableAttack()
    {
        attackCollider.enabled = false; // 콜라이더를 비활성화합니다.
        animator.SetBool("Attack", false); // 공격 상태를 false로 설정합니다.
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy") && !isTakingDamage)
        {
            isTakingDamage = true;
            // DamageOverTime 코루틴을 시작하고 참조를 저장합니다.
            damageCoroutine = StartCoroutine(DamageOverTime());
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            if (isTakingDamage)
            {
                isTakingDamage = false;
                // DamageOverTime 코루틴을 중지합니다.
                if (damageCoroutine != null)
                {
                    StopCoroutine(damageCoroutine);
                }
            }
        }
    }
    IEnumerator DamageOverTime()
    {
        while (isTakingDamage)
        {
            PlayerHP -= 1;
            if (PlayerHP <= 0)
            {
                // 플레이어가 죽었을 때의 로직
                GameManager.instance.GameOver(); // 게임 매니저의 GameOver 메서드를 호출합니다.
                break; // 체력이 0 이하가 되면 루프를 탈출합니다.
            }
            yield return new WaitForSeconds(0.1f); // 0.1초 동안 대기
        }

        // 코루틴이 종료되었을 때 필요한 추가 로직 (예를 들면, 상태 초기화)
        isTakingDamage = false;
    }
    void OnMove(InputValue value)
    {
        if (canMove) // 추가된 부분: canMove가 true일 때만 입력을 받음
        {
            inputVec = value.Get<Vector2>();
        }
    }
    public void SetCanMove(bool move)
    {
        canMove = move;
    }
        private void OnEnable()
    {
        Monster.OnMonsterDisabled += HandleMonsterDisabled;
    }

    private void OnDisable()
    {
        Monster.OnMonsterDisabled -= HandleMonsterDisabled;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject joystickObject = GameObject.FindGameObjectWithTag("VirtualJoystick");
        if (joystickObject != null)
        {
            // 찾은 게임 오브젝트에서 VirtualJoystick 컴포넌트를 가져옵니다.
            virtualJoystick = joystickObject.GetComponent<VirtualJoystick>();
        }
    }
    private void HandleMonsterDisabled(Monster monster)
    {
        // 여기서 Monster의 Collider와 현재 Player가 충돌 상태인지 확인하고
        // 충돌 상태라면 DamageOverTime 코루틴을 중지합니다.
        if (isTakingDamage)
        {
            // 이 부분은 게임의 다른 로직에 따라 달라질 수 있으며,
            // 실제로 충돌 상태인지를 여기서 체크할 수 있어야 합니다.
            // 예를 들어, Monster 게임 오브젝트의 ID나 참조를 Player가 들고 있을 경우 이를 사용할 수 있습니다.
            
            isTakingDamage = false;
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
            }
        }
    }
    public void TakeDamage(int damage)
    {
        PlayerHP -= damage;

        // 체력이 0 이하인지 확인
        if (PlayerHP <= 0)
        {
            // 게임 오버 처리
            GameOver();
        }
        if (!isTakingDamage)
        {
            PlayerHP -= damage;
            isTakingDamage = true;

            // 데미지를 받은 후 무적 시간 설정
            StartCoroutine(InvulnerabilityAfterDamage());
        }
    }
    private IEnumerator InvulnerabilityAfterDamage()
    {
        yield return new WaitForSeconds(1f); // 예시로 1초 동안 무적
        isTakingDamage = false;
    }
    private void GameOver()
    {
        // 게임 오버 로직
        // 예: 게임 매니저의 GameOver 메서드 호출
        GameManager.instance.GameOver();
    }
}
