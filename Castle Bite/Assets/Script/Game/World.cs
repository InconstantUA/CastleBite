using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;

    void Awake()
    {
        Instance = this;
        // Remove all current chapers in the World
        foreach (Chapter chapter in GetComponentsInChildren<Chapter>(true))
        {
            Debug.LogWarning("Removing " + chapter.ChapterData.chapterName + " chapter.");
            Destroy(chapter.gameObject);
        }
    }
}
