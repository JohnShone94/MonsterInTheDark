using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sEnemyController : MonoBehaviour
{
    GameObject player;
    UnityEngine.AI.NavMeshAgent nav;
    public bool bCanSeePlayer;
    public sCharacterController cc;
    public float radiusMod = 1.0f;
    bool bCoroutineExecuting;
    bool atPlayer;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cc = player.transform.GetComponent<sCharacterController>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        transform.position = GetRandomPosition(200.0f * radiusMod);
    }

    void FixedUpdate()
    {
        nav.SetDestination(player.transform.position);
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (dist <= nav.stoppingDistance && !atPlayer)
        {
            atPlayer = true;

            StartCoroutine(DelayAndRun(2.0f, () =>
            {
                cc.RespawnPlayer();
                Relocate();
                atPlayer = false;
            }));
        }

        RaycastHit see;
        Vector3 forward = player.transform.position - transform.position;
        if (Physics.Raycast(transform.position, forward, out see))
        {
            if (see.transform == player)
            {
                bCanSeePlayer = true;
            }
            else
            {
                bCanSeePlayer = false;

                StartCoroutine(DelayAndRun(10.0f, () =>
                {
                    if (!bCanSeePlayer)
                    {
                        Relocate();
                    }
                }));
            }
        }
    }

    private Vector3 GetRandomPosition(float radius)
    {
        Vector3 randPos = UnityEngine.Random.insideUnitSphere * radius;
        randPos += player.transform.position;
        UnityEngine.AI.NavMeshHit hit;
        Vector3 targetPos = Vector3.zero;
        if (UnityEngine.AI.NavMesh.SamplePosition(randPos, out hit, radius, 1))
        {
            targetPos = hit.position;
        }
        return targetPos;
    }

    public void Relocate()
    {
        transform.position = GetRandomPosition(140.0f * radiusMod);
    }

    IEnumerator DelayAndRun(float time, Action function)
    {
        if (bCoroutineExecuting)
        {
            yield break;
        }

        bCoroutineExecuting = true;

        yield return new WaitForSeconds(time);

        function();

        bCoroutineExecuting = false;
    }
}
