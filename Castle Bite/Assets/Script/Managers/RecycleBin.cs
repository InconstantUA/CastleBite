using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecycleBin : Singleton<RecycleBin>
{
    // use it only for single objects destroy
    // don't use it in foreach loop
    // because: https://forum.unity.com/threads/foreach-and-transform-parent.120336/
    public static void Recycle(GameObject gameObjectToDestroy)
    {
        // move game object to this recycle bin
        // move is required, because actual destroy is being done at the end of the frame,
        // but we have a logic which assumes that object has already been destroyed
        gameObjectToDestroy.transform.SetParent(Instance.transform);
        // destroy object
        Destroy(gameObjectToDestroy);
    }

    public static void RecycleChildrenOf(GameObject parentGameObject)
    {
        // get all children 1 level below the parent
        Transform[] children = new Transform[parentGameObject.transform.childCount];
        int i = 0;
        foreach (Transform tempTransform in parentGameObject.transform)
        {
            children[i] = tempTransform;
            i++;
        }
        // loop through all objects
        for (int j = 0; j < children.Length; j++)
        {
            // recycle the object
            Recycle(children[j].gameObject);
        }
    }

}

