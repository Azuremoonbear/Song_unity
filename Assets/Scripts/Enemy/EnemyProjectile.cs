using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 2;
    public float speed = 8f;
    public float lifeTime = 3f;

    private Vector3 moveDir;

    public void SetDirection(Vector3 dir)
    {
        moveDir = dir.normalized;
    }
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}