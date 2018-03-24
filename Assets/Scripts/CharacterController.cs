using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    //Allows use to change the speed of the character outside of the class.
    public float movementSpeed = 10.0f;
    public GameObject spawnPoint;

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
	}

    public void RespawnPlayer()
    {
        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;
    }
}
