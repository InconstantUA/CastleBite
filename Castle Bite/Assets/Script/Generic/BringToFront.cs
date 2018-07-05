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

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    Debug.Log("OnPointerEnter");
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    Debug.Log("OnPointerExit");
    //}

}
