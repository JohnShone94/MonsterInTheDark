using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSpawnPoint : MonoBehaviour
{
    public GameObject player;
	void Start ()
    {
        Instantiate(player, gameObject.transform);
    }
}
