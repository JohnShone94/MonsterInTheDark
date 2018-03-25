using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPages : MonoBehaviour
{
    public bool isFirstPage;
    public GameObject enemySpawnPoint;
    public GameObject enemyPrefab;
    GameObject[] pages;

    public void PickUpPage()
    {
        if(isFirstPage)
        {
            Instantiate(enemyPrefab, enemySpawnPoint.transform);

            pages = GameObject.FindGameObjectsWithTag("Page");
            for(int i = 0; i < pages.Length; i++)
            {
                pages[i].gameObject.GetComponent<sPages>().isFirstPage = false;
                pages[i].gameObject.GetComponent<sPages>().enemySpawnPoint = null;
                pages[i].gameObject.GetComponent<sPages>().enemyPrefab = null;
            }
            Destroy(transform.gameObject);
        }
        else
        {
            GameObject theEnemy;
            theEnemy = GameObject.FindGameObjectWithTag("Enemy").gameObject;
            theEnemy.GetComponent<sEnemyController>().Relocate();
            theEnemy.GetComponent<sEnemyController>().radiusMod += -0.1f;



            Destroy(transform.gameObject);
        }
    }
}
