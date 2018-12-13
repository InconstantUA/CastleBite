using System;
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
    // 
    int rotationsCounter;
    bool doRotate;
    int rotationDirection; // - or +
    bool rotationLock;
    [SerializeField]
    float mapRotationSpeed;
    [SerializeField]
    int maxRotations;

    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;

    float GetRemainingDistance()
    {
        // get postion of the camera center
        Vector3 cameraCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
        // get destinaion with the border limits
        Vector3 cameraCenterWithBorderLimitsAdjusted = GetPositionWithScreenBordersLimits(cameraCenter);
        // get remaining distance
        // float remainingDistance = MapManager.Instance.GetToroidalDistance(GetPositionWithScreenBordersLimits(followedHeroOnMap.transform.position), cameraCenter);
        float remainingDistance = MapManager.Instance.GetToroidalDistance(GetPositionWithScreenBordersLimits(followedHeroOnMap.transform.position), cameraCenter);
        Debug.Log("Remaining distance " + remainingDistance);
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
        // reset map rotations counter
        rotationsCounter = 0;
        // reset has rotated flag
        doRotate = false;
        rotationLock = false;
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
        float tilesize = 16f;
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
        // float buffer = 16f;
        float buffer = 14f + 16f;
        // Debug.Log("Destination Position " + (int)position.x);
        // float screenLeftLimit = Screen.width / 2 + buffer;
        float screenLeftLimit = transform.position.x - buffer;
        // float screenRightLimit = mapsize - Screen.width / 2 - buffer;
        float screenRightLimit = transform.position.x + buffer;
        // float screenRightLimit = Screen.width / 2 - buffer;
        // if (position.x < screenLeftLimit - Mathf.Abs(rotationsCounter) * tilesize)
        if (position.x < screenLeftLimit)
        {
            // reset position to screen bottom limit
            position.x = screenLeftLimit;
            // verify if rotation lock is not active{
            if (!rotationLock)
            {
                // Rotate map
                rotationDirection = -1;
                Debug.Log("Trigger rotation to the Left");
                doRotate = true;
            }
        }
        // else if (position.x > screenRightLimit + Mathf.Abs(rotationsCounter) * tilesize)
        else if (position.x > screenRightLimit)
        {
            // reset position to screen top limit
            position.x = screenRightLimit;
            // verify if rotation lock is not active
            if (!rotationLock)
            {
                rotationDirection = +1;
                Debug.Log("Trigger rotation to the Right");
                doRotate = true;
            }
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
            Vector3 followedHeroPosition = followedHeroOnMap.transform.position;
            // Get camera position
            Vector3 point = Camera.main.WorldToViewportPoint(followedHeroPosition);
            Vector3 cameraPostion = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            // Debug.LogWarning("Hero:Camera y " + (int)followedHeroPosition.y + ":"  + (int)Camera.main.transform.position.y);
            // Get destination
            Vector3 delta = followedHeroPosition - cameraPostion;
            Vector3 destination = transform.position + delta;
            Vector3 destinationBorderAligned = GetPositionWithScreenBordersLimits(destination);
            Vector3 destinationOYO = new Vector3(transform.position.x, destinationBorderAligned.y, transform.position.z);
            // verify if we need to rotate
            float tileSize = 16f;
            float mapWidth = 960f;
            if (doRotate)
            {
                // get xDistance
                float xDistance = destination.x - transform.position.x;
                // get number of rotaitons required based on the distance and tile size
                int numberOfRotationsRequired = 1 + Math.Abs(Mathf.RoundToInt(xDistance / tileSize));
                // get number of rotations, which will cover whole map
                int loopRotations = Mathf.RoundToInt(mapWidth / tileSize);
                // Debug.Log("rotations reset to max when between: " + maxRotations + "-" + loopRotations);
                // make sure that number of rotaions are not higher than the number or map slices
                while (numberOfRotationsRequired >= loopRotations)
                {
                    numberOfRotationsRequired -= loopRotations;
                    // calculate them in a distance
                    rotationsCounter += loopRotations;
                }
                rotationsCounter += numberOfRotationsRequired * rotationDirection;
                // Debug.Log("Do " + numberOfRotationsRequired + " rotations, total rotaitons: " + rotationsCounter);
                if (numberOfRotationsRequired != 0)
                {
                    if (rotationDirection > 0)
                    {
                        // Rotate map
                        MapManager.Instance.RotateRight(numberOfRotationsRequired);
                    }
                    else
                    {
                        // Rotate map
                        MapManager.Instance.RotateLeft(numberOfRotationsRequired);
                    }
                }
                // reset flag
                // Debug.Log("Block rotations");
                doRotate = false;
            }
            // verify that camera has not already focused on a hero
            if (!cameraIsFocused)
            {
                // move camera to the hero
                transform.position = Vector3.SmoothDamp(transform.position, destinationOYO, ref velocity, dampTime, focusMoveSpeed);
            }
            else
            {
                // make camera follow hero on Map
                transform.position = Vector3.SmoothDamp(transform.position, destinationOYO, ref velocity, dampTime);
            }
        }
        if (doFollowMapDrag)
        {
            // Debug.Log("Camera before x:y " + (int)Camera.main.transform.position.x + ":" + (int)Camera.main.transform.position.y);
            float tileSize = 16f;
            float mapWidth = 960f;
            Vector3 newPos = dragOrigin - Input.mousePosition;
            Vector3 delta = newPos - Camera.main.transform.position - new Vector3(tileSize * rotationsCounter, 0, 0);
            // Debug.Log("delta x:y " + (int)delta.x + ":" + (int)delta.y);
            Vector3 destination = transform.position + delta;
            Vector3 destinationBorderAligned = GetPositionWithScreenBordersLimits(destination);
            // verify if we need to rotate
            if (doRotate)
            {
                // get xDistance
                float xDistance = destination.x - transform.position.x;
                // get number of rotaitons required based on the distance and tile size
                int numberOfRotationsRequired = 1 + Math.Abs(Mathf.RoundToInt(xDistance / tileSize));
                // set max number of rotations
                // int maxRotations = (int)Mathf.Floor((mapWidth - (float)Screen.width) / tileSize) - 1;
                if (maxRotations <= 0)
                    maxRotations = 1;
                // get number of rotations, which will cover whole map
                int loopRotations = Mathf.RoundToInt(mapWidth / tileSize);
                // verify if number of rotations is more than max and less than loop
                //if (numberOfRotationsRequired > maxRotations && numberOfRotationsRequired < loopRotations)
                //    numberOfRotationsRequired = maxRotations;
                // Debug.Log("rotations reset to max when between: " + maxRotations + "-" + loopRotations);
                // make sure that number of rotaions are not higher than the number or map slices
                while (numberOfRotationsRequired >= loopRotations)
                {
                    numberOfRotationsRequired -= loopRotations;
                    // calculate them in a distance
                    rotationsCounter += loopRotations;
                }
                rotationsCounter += numberOfRotationsRequired * rotationDirection;
                // Debug.Log("Do " + numberOfRotationsRequired + " rotations, total rotaitons: " + rotationsCounter);
                if (numberOfRotationsRequired != 0)
                {
                    if (rotationDirection > 0)
                    {
                        // Rotate map
                        MapManager.Instance.RotateRight(numberOfRotationsRequired);
                    }
                    else
                    {
                        // Rotate map
                        MapManager.Instance.RotateLeft(numberOfRotationsRequired);
                    }
                    // shift camera to hide rotation
                    // Debug.Log("Shift camera");
                    // transform.position -= new Vector3(tileSize * rotationDirection * numberOfRotationsRequired, 0, 0);
                }
                // reset flag
                // Debug.Log("Block rotations");
                doRotate = false;
                // Debug.Break();
                // do not move camera anymore during this frame
                // activate rotation lock, until camera has reached previous rotation target
                // rotationLock = true;
                // Debug.Log("Activate rotation lock");
            }
            // move camera to simulate drag
            Vector3 destinationOYO = new Vector3(transform.position.x, destinationBorderAligned.y, transform.position.z);
            // Vector3 destinationXYO = new Vector3(destinationBorderAligned.x, destinationBorderAligned.y, transform.position.z);
            // transform.position = Vector3.SmoothDamp(transform.position, destinationXYO, ref velocity, dampTime, mapRotationSpeed);
            // do not move camera
            transform.position = Vector3.SmoothDamp(transform.position, destinationOYO, ref velocity, dampTime);
            // get distance
            float remainingDistanceX = Mathf.Abs(transform.position.x - destinationBorderAligned.x);
            // set min distance 
            float minDistance = 0.5f;
            // verify if rotation lock can be unlocked
            if (remainingDistanceX <= minDistance && rotationLock == true)
            {
                // release rotation lock;
                rotationLock = false;
                // Debug.Log("Release rotation lock, remaining distance: " + (int)remainingDistanceX);
            }
            // Debug.Log("Camera after  x:y " + (int)Camera.main.transform.position.x + ":" + (int)Camera.main.transform.position.y);
            // Debug.Log("Rotations: " + rotationsCounter);
        }
    }
}
