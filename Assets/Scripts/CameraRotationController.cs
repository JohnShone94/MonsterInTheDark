using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationController : MonoBehaviour
{
    //We use this to set the Sensitivity of looking around.
    public float lookSensitivity = 5.0f;
    //We use this to set how smooth looking around will be.
    public float lookSmoothness = 2.0f;
    //this keeps track of the overall movement of the rotation.
    Vector2 lookRot;
    //this allows use to smoothdown the camera rotation.
    Vector2 smoothTrans;

    //Here we are getting the character.
    GameObject character;

    void Start ()
    {
        //We need to get the character the script is connected to.
        character = this.transform.parent.gameObject;
    }
	void Update ()
    {
        //We need to create a mouseDelta variable which takes in a vector 2 of the mouse x and y.
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(lookSensitivity * lookSmoothness, lookSensitivity * lookSmoothness));
        //next we need to lerp the x and y movements for a smooth rotation.
        smoothTrans.x = Mathf.Lerp(smoothTrans.x, mouseDelta.x, 1.0f / lookSmoothness);
        smoothTrans.y = Mathf.Lerp(smoothTrans.y, mouseDelta.y, 1.0f / lookSmoothness);
        //next we add the smoothTrans to the look rotation and clamp the y axis.
        lookRot += smoothTrans;
        lookRot.y = Mathf.Clamp(lookRot.y, -90.0f, 90.0f);
        //next we set the localRotation.
        transform.localRotation = Quaternion.AngleAxis(-lookRot.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(lookRot.x, character.transform.up);
    }
}
