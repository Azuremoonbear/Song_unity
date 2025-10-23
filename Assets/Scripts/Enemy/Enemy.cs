using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Trace, Attack, RunAway }
    public EnemyState state = EnemyState.Idle;

    [Header("�⺻����")]
    public float moveSpeed = 2f;
    public float traceRange = 15f;
    public float attackRange = 7f;
    public float attackCooldown = 1.5f;

    [Header("AI ����")]
    public float runAwayHealthPercent = 0.2f; // ü���� �� % ������ �� �������� (��: 20%)

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

        //FSM ���� ��ȯ
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
        //�÷��̾�κ��� �־����� ���� ��� (���� ������ �ݴ�)
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

        //���� ���� ��ȯ ���� �߰�
        //ü���� ���� ���� �����̰�, ���� ���� ���°� �ƴ϶��
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