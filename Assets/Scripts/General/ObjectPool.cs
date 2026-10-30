using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    #region Singleton Setup
    public static ObjectPool instance; //Singleton Instance

    void Awake()
    {
        if (instance == null)
        {
            instance = this; //Set this as the static instance
        }
        else
        {
            //If multiple object pools are in the same scene delete the duplicate
            Debug.LogWarning("Multiple ObjectPools in the scene, deleting duplicate.");
            Destroy(this);
        }
    }
    #endregion
    [HideInInspector] public List<GameObject> standardList = new List<GameObject>(); //Default Pooled Objects list
    private IEnumerator timeKiller = null;

    //Function for creating an object from the object pool
    public GameObject Create(GameObject newbie, Vector3 position, Quaternion rotation)
    {
        //Determine if the object exists in the object pool
        if (standardList.Find(x => x.name == newbie.name))
        {
            int selectedPooledObject = standardList.FindIndex(x => x.name == newbie.name); //Find the index of the chosen object in the list
            GameObject createdObject = standardList[selectedPooledObject]; //Get reference to the chosen object
            createdObject.transform.position = position; //Set the object's position to the desired location
            createdObject.SetActive(true); //Set the object to be active
            standardList.RemoveAt(selectedPooledObject); //Remove the object from the object pool
            return createdObject; //Return refernce to the object
        }
        else
        {
            //If the object doesn't exist instantiate it and rename if to match the original name
            GameObject createdObject = Instantiate(newbie, position, rotation);
            createdObject.name = newbie.name;
            return createdObject; //Return reference to the object
        }
    }

    //Function for creating an object from the object pool
    public GameObject Create(GameObject newbie, Vector3 position, Quaternion rotation, List<GameObject> pool)
    {
        //Determine if the object exists in the object pool
        if(pool.Find(x => x.name == newbie.name))
        {
            int selectedPooledObject = pool.FindIndex(x => x.name == newbie.name); //Find the index of the chosen object in the list
            GameObject createdObject = pool[selectedPooledObject]; //Get reference to the chosen object
            createdObject.transform.position = position; //Set the object's position to the desired location
            createdObject.SetActive(true); //Set the object to be active
            pool.RemoveAt(selectedPooledObject); //Remove the object from the object pool
            return createdObject; //Return refernce to the object
        }
        else
        {
            //If the object doesn't exist instantiate it and rename if to match the original name
            GameObject createdObject = Instantiate(newbie, position, rotation);
            createdObject.name = newbie.name;
            return createdObject; //Return reference to the object
        }
    }

    //Function for returning an object to the object pool
    public GameObject Destroy(GameObject gameObject) 
    {
        //If the game object still exists at this point
        if (gameObject != null)
        {
            standardList.Add(gameObject); //Add the game object to the object pool
            gameObject.SetActive(false); //Make the game object inactive
        }
        return gameObject;

    }

    //Overload for destroying a game object but adding to a different list than the standard object pool list
    public GameObject Destroy(GameObject gameObject, List<GameObject> pool)
    {
        //If the game object still exists at this point
        if(gameObject != null)
        {
            pool.Add(gameObject); //Add the game object to the object pool
            gameObject.SetActive(false); //Make the game object inactive
        }
        return gameObject;
    }

    //Overload for destroying a game object but doing so after an amount of time has passed
    public GameObject Destroy(GameObject gameObject, List<GameObject> pool, float time)
    {
        //Start the time killer coroutine
        if(timeKiller == null)
        {
            timeKiller = DestroyTime(pool, gameObject, time);
            StartCoroutine(timeKiller);
        }
        //If the game object still exists at this point
        return gameObject;
    }

    //IEnumerator for returning an object to the object pool after a set amount of time
    private IEnumerator DestroyTime(List<GameObject> pool, GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);
        if (gameObject != null)
        {
            standardList.Add(gameObject); //Add the game object to the object pool
            timeKiller = null; //Clear the timeKiller holder for a new one later on
            gameObject.SetActive(false); //Make the game object inactive
        }
    }
}
