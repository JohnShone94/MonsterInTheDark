using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPages : MonoBehaviour
{
    public bool bIsFirstPage;
    public GameObject enemySpawnPoint;
    public GameObject enemyPrefab;
    GameObject[] pages;

    public void PickUpPage()
    {
        //First we check if the page been picked up is the first one.
        if(bIsFirstPage)
        {
            //If it is we spawn the enemy at the enemySpawnPoint.
            Instantiate(enemyPrefab, enemySpawnPoint.transform);
            //Next we instanciate the pages array with the pages left in the game.
            pages = GameObject.FindGameObjectsWithTag("Page");
            for(int i = 0; i < pages.Length; i++)
            {
                //Next we have to go through all the pages in the game.
                //and turn off th first page boolean and set the 
                //enemy prefab and spawnpoint to equal null.
                pages[i].gameObject.GetComponent<sPages>().bIsFirstPage = false;
                pages[i].gameObject.GetComponent<sPages>().enemySpawnPoint = null;
                pages[i].gameObject.GetComponent<sPages>().enemyPrefab = null;
            }
            //finally we destroy the page.
            Destroy(transform.gameObject);
        }
        else
        {
            //if its not the first one then we get a reference to the enemy.
            GameObject theEnemy;
            theEnemy = GameObject.FindGameObjectWithTag("Enemy").gameObject;
            //then we relocate the enemy and remove -0.1 from the radius mod.
            theEnemy.GetComponent<sEnemyController>().Relocate();
            theEnemy.GetComponent<sEnemyController>().radiusMod += -0.1f;
            //finally we destroy the page.
            Destroy(transform.gameObject);
        }
    }
}
