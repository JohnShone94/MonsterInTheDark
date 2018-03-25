using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sCharacterController : MonoBehaviour
{
    //Allows use to change the speed of the character outside of the class.
    public float movementSpeed = 10.0f;
    public GameObject spawnPoint;
    public bool bCanSeePage;
    private GameObject page;
    public int pagesCollected;

    void Start ()
    {
        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;
        //We need to deactivate the cursor, and lock it to the game screen.
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	void Update ()
    {
        //this allows use to get the X and Z movements from the input keyboard or controller.
        float Z = Input.GetAxis("Vertical") * movementSpeed;
        float X = Input.GetAxis("Horizontal") * movementSpeed;

        //in order to move at a smooth rate we need to multiply them by the time.deltatime.
        Z *= Time.deltaTime;
        X *= Time.deltaTime;

        //finally update the transform of the character.
        transform.Translate(X, 0, Z);

        //If we press Escape we need to reactivate the cursor.
        if(Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if(pagesCollected >= 10)
        {
            print("You have won");
        }

        if(Input.GetKeyDown("e") && bCanSeePage == true)
        {
            pagesCollected++;
            page.GetComponent<sPages>().PickUpPage();
            page = null;
            bCanSeePage = false;
        }
	}

    public void RespawnPlayer()
    {
        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;
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
