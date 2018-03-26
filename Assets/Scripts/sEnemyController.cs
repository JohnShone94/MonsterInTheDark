using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sEnemyController : MonoBehaviour
{
    GameObject player;
    UnityEngine.AI.NavMeshAgent nav;
    public bool bCanSeePlayer;
    public sCharacterController cc;
    public float radiusMod = 1.0f;
    bool bCoroutineExecuting;

    public AudioSource OtherSource;
    public AudioClip laughing;

    public sOptionsManager om;

    private float runningSpeed;
    private float walkingSpeed;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cc = player.transform.GetComponent<sCharacterController>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        transform.position = GetRandomPosition(200.0f * radiusMod);
        walkingSpeed = 5.0f;
        runningSpeed = 10.0f;
        nav.speed = walkingSpeed;

        om = GameObject.FindGameObjectWithTag("WorldManager").GetComponent<sOptionsManager>();
        OtherSource = GameObject.FindGameObjectWithTag("OtherSource").GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        RaycastHit see;
        Vector3 forward = player.transform.position - transform.position;
        if (Physics.Raycast(transform.position, forward, out see))
        {
            if (see.transform == player)
            {
                bCanSeePlayer = true;
                nav.speed = runningSpeed;
            }
            else
            {
                bCanSeePlayer = false;
                nav.speed = walkingSpeed;
                StartCoroutine("LostThePlayer");
            }
        }
        nav.SetDestination(player.transform.position);
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (dist <= nav.stoppingDistance)
        {
            OtherSource.clip = laughing;
            OtherSource.Play();
            StartCoroutine("CourtThePlayer");
        }
    }

    public void SetWalkingSpeed(float speed)
    {
        walkingSpeed += speed;
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

    IEnumerator CourtThePlayer()
    {
        cc.bCanTakeInput = false;
        cc.UpdateTheRot(gameObject.transform);

        yield return new WaitForSeconds(5.0f);
        om.LoadEndGame();
    }
    IEnumerator LostThePlayer()
    {
        if (bCoroutineExecuting)
        {
            yield break;
        }
        bCoroutineExecuting = true;
        yield return new WaitForSeconds(10.0f);
        Relocate();
        bCoroutineExecuting = false;
    }

}
