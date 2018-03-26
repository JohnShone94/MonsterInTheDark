using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sCharacterController : MonoBehaviour
{
    //Allows use to change the speed of the character outside of the class.
    public float movementSpeed = 10.0f;
    public GameObject spawnPoint; 
    public bool bCanSeePage;
    private GameObject page;
    public GameObject pauseMenu;
    public int pagesCollected = 0;
    public bool bCanTakeInput;
    public Text txtPages;
    private int stamina = 10;
    private bool bCoroutineExecuting;
    private bool exausted = false;
    private bool regen = false;

    public AudioSource OtherSource;
    public AudioClip wispering;
    public AudioClip heartBeat;

    public bool playing;
    public sOptionsManager om;
    sEnemyController enemy;

    void Start ()
    {
        //We need to deactivate the cursor, and lock it to the game screen.
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1.0f;
        bCanTakeInput = false;
        Invoke("CanUseInput", 1);


        om = GameObject.FindGameObjectWithTag("WorldManager").GetComponent<sOptionsManager>();
    }
	
	void Update ()
    {
        if (bCanTakeInput == true)
        {
            float Z = Input.GetAxis("Vertical") * movementSpeed;
            float X = Input.GetAxis("Horizontal") * movementSpeed;

            //in order to move at a smooth rate we need to multiply them by the time.deltatime.
            Z *= Time.deltaTime;
            X *= Time.deltaTime;

            //finally update the transform of the character.
            transform.Translate(X, 0, Z);


            //If we press Escape we need to reactivate the cursor.
            if (Input.GetKeyDown("escape"))
            {
                pauseMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0.0f;
                bCanTakeInput = false;
            }

            if (Input.GetKeyDown("e") && bCanSeePage == true)
            {
                pagesCollected++;
                page.GetComponent<sPages>().PickUpPage();
                enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<sEnemyController>();
                enemy.SetWalkingSpeed(0.5f);
                page = null;
                bCanSeePage = false;
            }

            if (Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
            {
                if(exausted != true)
                {
                    movementSpeed = 20.0f;
                    StartCoroutine(ChangeStamina(1, -1));
                    regen = false;
                    print(exausted);
                }
                else
                {
                }
            }
            if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
            {
                StartCoroutine(PlayWisper());
            }
            else
            {
                OtherSource.Stop();
            }


                if (Input.GetKeyUp(KeyCode.LeftShift) && exausted != true)
            {
                movementSpeed = 10.0f;
                regen = true;
            }

            if (stamina <= 0)
            {
                exausted = true;
                regen = true;
                movementSpeed = 5.0f;
            }

            if (regen)
            {
                if(stamina < 10)
                {
                    StartCoroutine(PlayHeartbeat());
                    StartCoroutine(ChangeStamina(0.5f, 1));
                }
                else
                {
                    exausted = false;
                    regen = false;
                    movementSpeed = 10.0f;
                }
            }

            if (pagesCollected >= 10)
            {
                om.LoadEndGame();
            }
        }

        txtPages.text = ("" + pagesCollected);
	}

    IEnumerator PlayWisper()
    {
        if (!playing)
        {
            playing = true;
            OtherSource.clip = wispering;
            OtherSource.Play();
            yield return new WaitForSeconds(7);
            playing = false;
        }
    }

    IEnumerator PlayHeartbeat()
    {
        if (!playing)
        {
            playing = true;
            OtherSource.clip = heartBeat;
            OtherSource.Play();
            yield return new WaitForSeconds(9);
            playing = false;
        }
    }

    IEnumerator ChangeStamina(float time, int change)
    {
        if (bCoroutineExecuting)
        {
            yield break;
        }
        bCoroutineExecuting = true;
        stamina += change;
        yield return new WaitForSeconds(time);
        print(stamina);
        bCoroutineExecuting = false;
    }

    public void RespawnPlayer()
    {
        transform.position = spawnPoint.transform.position;
        bCanTakeInput = true;
    }

    public void UpdateTheRot(Transform target)
    {
        transform.LookAt(target);
    }

    public void CanUseInput()
    {
        bCanTakeInput = true;
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Page")
        {
            page = col.gameObject;
            bCanSeePage = true;
        }
    }
}
