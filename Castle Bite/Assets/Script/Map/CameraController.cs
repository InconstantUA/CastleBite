using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    MapManager mapManager;
    [SerializeField]
    float focusMoveSpeed;
    [SerializeField]
    float focusMoveDelay;
    [SerializeField]
    float minDistance;
    MapHero followedHeroOnMap;
    bool cameraIsFocused = false;
    Vector3 offset;
    Vector3 moveDirection;
    float previousTime;

    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;

    float GetRemainingDistance()
    {
        // get postion of the camera center
        Vector3 cameraCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
        // get remaining distance
        float remainingDistance = mapManager.GetToroidalDistance(followedHeroOnMap.transform.position, cameraCenter);
        // Debug.Log("Remaining distance " + remainingDistance);
        return remainingDistance;
    }

    IEnumerator SetCameraFocus()
    {
        // centers camera on the followed object
        float deltaTime;
        // move camera center towards the followed object
        while (GetRemainingDistance() > minDistance)
        {
            deltaTime = Time.time - previousTime;
            // This is done in Update function now
            // transform.position = mapManager.MoveTowards(transform.position, followedHeroOnMap.transform.position, deltaTime * focusMoveSpeed, moveDirection) + offset;
            // wait until next move
            previousTime = Time.time;
            yield return new WaitForSeconds(focusMoveDelay);
        }
        // set camera is focused
        cameraIsFocused = true;
    }

    public void SetCameraFocus(MapHero mapHero)
    {
        // save followed hero on map
        followedHeroOnMap = mapHero;
        // verify if mapHero is not null
        if (mapHero != null)
        {
            // get map queue
            CoroutineQueue mapQueue = mapManager.Queue;
            //// get postion of the camera center
            //Vector3 cameraCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
            //Debug.Log("Camera center " + cameraCenter.x + ":" + cameraCenter.y + ":" + cameraCenter.z);
            //Debug.Log("Camera position " + transform.position.x + ":" + transform.position.y + ":" + transform.position.z);
            //// get offset between camera and its center
            //offset = transform.position - cameraCenter;
            //Debug.Log("Offset " + offset.x + ":" + offset.y + ":" + offset.z);
            //Debug.Log("Hero position " + followedHeroOnMap.transform.position.x + ":" + followedHeroOnMap.transform.position.y + ":" + followedHeroOnMap.transform.position.z);
            //// Get move direction
            //moveDirection = mapManager.GetDirection(followedHeroOnMap.transform.position, cameraCenter);
            //Debug.Log("Direction " + moveDirection.x + ":" + moveDirection.y + ":" + moveDirection.z);
            //// set time
            //previousTime = Time.time;
            // reset camera is focused
            cameraIsFocused = false;
            // start set focus animation
            mapQueue.Run(SetCameraFocus());
        }
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (followedHeroOnMap != null && !cameraIsFocused)
    //    {
    //        Vector3 point = Camera.main.WorldToViewportPoint(followedHeroOnMap.transform.position);
    //        Vector3 delta = followedHeroOnMap.transform.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
    //        Vector3 destination = transform.position + delta;
    //        transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime, focusMoveSpeed);
    //    }
    //}

    void LateUpdate()
    {
        // move camera to the hero
        if (followedHeroOnMap != null && !cameraIsFocused)
        {
            Vector3 point = Camera.main.WorldToViewportPoint(followedHeroOnMap.transform.position);
            Vector3 delta = followedHeroOnMap.transform.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime, focusMoveSpeed);
        }
        // follow hero
        if (followedHeroOnMap != null && cameraIsFocused)
        {
            // make camera follow hero on Map
            // transform.position = followedHeroOnMap.transform.position + offset;
            Vector3 point = Camera.main.WorldToViewportPoint(followedHeroOnMap.transform.position);
            Vector3 delta = followedHeroOnMap.transform.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime, focusMoveSpeed);
        }
    }
}
