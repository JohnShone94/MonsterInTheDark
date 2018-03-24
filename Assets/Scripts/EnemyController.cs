using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Transform player;
    UnityEngine.AI.NavMeshAgent nav;
    public GameObject center;
    public bool canSeePlayer;
    public CharacterController cc;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        transform.position = GetRandomPosition(100.0f);
    }

    void FixedUpdate()
    {
        nav.SetDestination(player.position);
        float dist = Vector3.Distance(player.position, transform.position);
        if (dist < nav.stoppingDistance)
        {
            StartCoroutine("FoundPlayer");
        }

        RaycastHit see;
        Vector3 forward = player.position - transform.position;
        if (Physics.Raycast(transform.position, forward, out see))
        {
            if (see.transform == player)
            {
                canSeePlayer = true;
              //  print("Can See The Player");
            }
            else
            {
               // print("Cant See The Player");
                canSeePlayer = false;
                StartCoroutine("LookingForPlayer");
            }
        }
    }

    private Vector3 GetRandomPosition(float radius)
    {
        Vector3 randPos = UnityEngine.Random.insideUnitSphere * radius;
        randPos += center.transform.position;
        UnityEngine.AI.NavMeshHit hit;
        Vector3 targetPos = Vector3.zero;
        if (UnityEngine.AI.NavMesh.SamplePosition(randPos, out hit, radius, 1))
        {
            targetPos = hit.position;
        }
        return targetPos;
    }

    private void relocate()
    {
        transform.position = GetRandomPosition(100.0f);
    }

    IEnumerator LookingForPlayer()
    {
        yield return new WaitForSeconds(15);

        if (!canSeePlayer)
        {
            relocate();
            StopCoroutine("LookingForPlayer");
        }
    }

    IEnumerator FoundPlayer()
    {
        yield return new WaitForSeconds(2);
        print("Player Is Dead");
        cc.RespawnPlayer();
        StopCoroutine("FoundPlayer");
    }
}
