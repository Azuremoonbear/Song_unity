using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Trace, Attack, RunAway }
    public EnemyState state = EnemyState.Idle;

    [Header("기본설정")]
    public float moveSpeed = 2f;
    public float traceRange = 15f;
    public float attackRange = 7f;
    public float attackCooldown = 1.5f;

    [Header("AI 설정")]
    public float runAwayHealthPercent = 0.2f; // 체력이 몇 % 이하일 때 도망갈지 (예: 20%)

    public GameObject projectilePrefabs;
    public Transform firePoint;

    private Transform player;
    private float lastAttackTime;
    public int maxHP = 5;
    private int currentHP;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastAttackTime = -attackCooldown;
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        //FSM 상태 전환
        switch (state)
        {
            case EnemyState.Idle:
                if (dist < traceRange)
                    state = EnemyState.Trace;
                break;

            case EnemyState.Trace:
                if (dist < attackRange)
                    state = EnemyState.Attack;
                else if (dist > traceRange)
                    state = EnemyState.Idle;
                else
                    TracePlayer();
                break;

            case EnemyState.Attack:
                if (dist > attackRange)
                    state = EnemyState.Trace;
                else
                    AttackPlayer();
                break;

            case EnemyState.RunAway:
                if (dist < attackRange)
                    state = EnemyState.Idle;
                else
                    RunAway();
                break;
        }

    }

    void TracePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.LookAt(player.position);
    }

    void RunAway()
    {
        //플레이어로부터 멀어지는 방향 계산 (추적 방향의 반대)
        Vector3 awayDir = (transform.position - player.position).normalized;
        transform.position += awayDir * moveSpeed * Time.deltaTime;
        transform.LookAt(transform.position + awayDir);
    }

    void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefabs != null && firePoint != null)
        {
            transform.LookAt(player.position);
            GameObject proj = Instantiate(projectilePrefabs, firePoint.position, firePoint.rotation);
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            if (ep != null)
            {
                Vector3 dir = (player.position - firePoint.position).normalized;
                ep.SetDirection(dir);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        //도망 상태 전환 로직 추가
        //체력이 일정 비율 이하이고, 아직 도망 상태가 아니라면
        if (currentHP <= maxHP * runAwayHealthPercent && state != EnemyState.RunAway)
        {
            state = EnemyState.RunAway;
        }


        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}