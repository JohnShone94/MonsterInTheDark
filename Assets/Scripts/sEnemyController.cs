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
    bool bPlayLaugh;
    bool bInRadius;

    void Awake()
    {
        //Getting reference to the player.
        player = GameObject.FindGameObjectWithTag("Player");
        //Getting reference to the Character Controller.
        cc = player.transform.GetComponent<sCharacterController>();
        //Getting reference to the Enemy Nav Mesh Agent.
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //Setting the position to a random position on the NavMesh.
        transform.position = GetRandomPosition(200.0f * radiusMod);
        //Setting the walking and running speed of the enemy.
        walkingSpeed = 5.0f;
        runningSpeed = 10.0f;
        nav.speed = walkingSpeed;
        //Getting reference to the Options Menu Manager.
        om = GameObject.FindGameObjectWithTag("WorldManager").GetComponent<sOptionsManager>();
        //Getting reference to the audio source.
        OtherSource = GameObject.FindGameObjectWithTag("OtherSource").GetComponent<AudioSource>();

        bPlayLaugh = false;
        bInRadius = false;

    }

    void FixedUpdate()
    {
        //First we create a RaycastHit so we can detect what the raycast is hitting.
        RaycastHit see;
        //Next we must create a forward vector.
        Vector3 forward = player.transform.position - transform.position;
        //Here we check to see if the raycast has hit something.
        if (Physics.Raycast(transform.position, forward, out see))
        {
            //If it has hit something then we check what its hit.
            if (see.transform == player)
            {
                //If it has hit the player then we tell the enemy to start running.
                bCanSeePlayer = true;
                nav.speed = runningSpeed;
            }
            else
            {
                //if it cant find see the player then we tell it to start walking
                //and to run the courotine LostThePlayer.
                bCanSeePlayer = false;
                nav.speed = walkingSpeed;
                StartCoroutine("LostThePlayer");
            }
        }
        if(bPlayLaugh == true)
        {
            bPlayLaugh = false;
            OtherSource.clip = laughing;
            OtherSource.Play();
        }
        //Here we must tell the enemy to move towards the player.
        nav.SetDestination(player.transform.position);
        //Now we get the distance between the player and the enemy.
        float dist = Vector3.Distance(player.transform.position, transform.position);
        //If the distance is less than or equal to the stopping distance then stop.
        if (dist <= nav.stoppingDistance && bInRadius == false)
        {
            bInRadius = true;
            bPlayLaugh = true;
            //Here we set the clip to play, begin playing it and then start the 
            //coroutine CourtThePlayer.
            StartCoroutine("CourtThePlayer");
        }
    }

    public void SetWalkingSpeed(float speed)
    {
        //Update the walking speed of the enemy, used when the
        //player picks up a page
        walkingSpeed += speed;
    }

    private Vector3 GetRandomPosition(float radius)
    {
        //Here we get a random position on the navmesh.
        //First we get the random position using a UnitSphere and a radius
        Vector3 randPos = UnityEngine.Random.insideUnitSphere * radius;
        //Next we add the players position to the random position.
        randPos += player.transform.position;
        //Here we create a NavMeshHit to detect if the navmesh has been hit.
        UnityEngine.AI.NavMeshHit hit;
        //We set the target position to 0, 0, 0
        Vector3 targetPos = Vector3.zero;
        //If random position has hit the nav mesh.
        if (UnityEngine.AI.NavMesh.SamplePosition(randPos, out hit, radius, 1))
        {
            //we set the target position to equal the position hit on the navmesh.
            targetPos = hit.position;
        }
        return targetPos;
    }

    public void Relocate()
    {
        //We get a random position on the navmesh and move the enemy to it.
        transform.position = GetRandomPosition(140.0f * radiusMod);
    }

    IEnumerator CourtThePlayer()
    {
        //CourtThePlayer starts by setting the variable bCanTakeInput within
        //the CharacterController to false.
        cc.bCanTakeInput = false;
        //Then we update the rotation to look at the enemy using the
        //UpdateTheRot method within the CharacterController.
        cc.UpdateTheRot(gameObject.transform);

        //Next we delay the method for 5 seconds.
        yield return new WaitForSeconds(5.0f);
        //And finally we load the end game screen. using the 
        //LoadEndGame method from the Options Manager.
        om.LoadEndGame();
    }
    IEnumerator LostThePlayer()
    {
        //Because this is called in the update we first check if the method 
        //is running already and if so then we end the Coroutine
        if (bCoroutineExecuting)
        {
            yield break;
        }
        //If not then we set it to be running.
        bCoroutineExecuting = true;
        //and tell the method to wait 10 seconds.
        yield return new WaitForSeconds(10.0f);

        if(bCanSeePlayer == false)
        {
            //Finally relocating the enemy and telling the courtine its not
            //running anymore.
            Relocate();
            bCoroutineExecuting = false;
            bInRadius = false;
        }
        else
        {
            bCoroutineExecuting = false;
            yield break;
        }
    }

}
