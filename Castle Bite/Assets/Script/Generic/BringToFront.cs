using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//public class BringToFront : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
public class BringToFront : MonoBehaviour
{

    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }

}
