using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    //[SerializeField]
    //MapManager mapManager;
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
    // float previousTime;
    Vector3 dragOrigin;
    bool doFollowMapDrag;

    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;

    float GetRemainingDistance()
    {
        // get postion of the camera center
        Vector3 cameraCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
        // get remaining distance
        // float remainingDistance = mapManager.GetToroidalDistance(followedHeroOnMap.transform.position, cameraCenter);
        float remainingDistance = MapManager.Instance.GetToroidalDistance(GetPositionWithScreenBordersLimits(followedHeroOnMap.transform.position), cameraCenter);
        // Debug.Log("Remaining distance " + remainingDistance);
        return remainingDistance;
    }

    IEnumerator SetCameraFocus()
    {
        // centers camera on the followed object
        // float deltaTime;
        // move camera center towards the followed object
        while (GetRemainingDistance() > minDistance)
        {
            // deltaTime = Time.time - previousTime;
            // This is done in Update function now, because if I use it like below, there will be a jitter.
            // transform.position = mapManager.MoveTowards(transform.position, followedHeroOnMap.transform.position, deltaTime * focusMoveSpeed, moveDirection) + offset;
            // wait until next move
            // previousTime = Time.time;
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
            CoroutineQueue mapQueue = MapManager.Instance.Queue;
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

    public void SetCameraFocus(MapManager mManager, bool doActivate)
    {
        doFollowMapDrag = doActivate;
        // get and save drag origin mouse position
        dragOrigin = Input.mousePosition + transform.position;
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

    Vector3 GetPositionWithScreenBordersLimits(Vector3 position)
    {
        float mapsize = 960f;
        // get followed hero position
        float screenBottomHeightLimit = Screen.height / 2f;
        // verify if followed hero position x will not cause camera to be off map
        float screenTopHeightLimit = mapsize - Screen.height / 2f;
        if (position.y < screenBottomHeightLimit)
        {
            // reset position to screen bottom limit
            position.y = screenBottomHeightLimit;
        }
        else if (position.y > screenTopHeightLimit)
        {
            // reset position to screen top limit
            position.y = screenTopHeightLimit;
        }
        float buffer = 16f;
        float screenLeftLimit = Screen.width / 2 + buffer;
        float screenRightLimit = mapsize - Screen.width / 2 - buffer;
        if (position.x < screenLeftLimit)
        {
            // reset position to screen bottom limit
            position.x = screenLeftLimit;
        }
        else if (position.x > screenRightLimit)
        {
            // reset position to screen top limit
            position.x = screenRightLimit;
        }
        return position;
    }

    void LateUpdate()
    {
        // verify if there is a hero which camera needs to follow
        if (followedHeroOnMap != null) {
            // Get destination position based on the map borders
            // Avoid that camera goes over top or bottom border
            // Get heroPosition
            Vector3 followedHeroPosition = GetPositionWithScreenBordersLimits(followedHeroOnMap.transform.position);
            // Get camera position
            Vector3 point = Camera.main.WorldToViewportPoint(followedHeroPosition);
            Vector3 cameraPostion = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            // Debug.LogWarning("Hero:Camera y " + (int)followedHeroPosition.y + ":"  + (int)Camera.main.transform.position.y);
            // Get destination
            Vector3 delta = followedHeroPosition - cameraPostion;
            Vector3 destination = transform.position + delta;
            // verify that camera has not already focused on a hero
            if (!cameraIsFocused)
            {
                // move camera to the hero
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime, focusMoveSpeed);
            }
            else
            {
                // make camera follow hero on Map
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
            }
        }
        if (doFollowMapDrag)
        {
            Vector3 newPos = dragOrigin - Input.mousePosition;
            Vector3 delta = newPos - Camera.main.transform.position;
            Vector3 destination = transform.position + delta;
            Vector3 destinationBorderAligned = GetPositionWithScreenBordersLimits(destination);
            // Vector3 destinationOYO = new Vector3(transform.position.x, destinationBorderAligned.y, transform.position.z);
            Vector3 destinationOYO = new Vector3(destinationBorderAligned.x, destinationBorderAligned.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, destinationOYO, ref velocity, dampTime);
        }
    }
}
