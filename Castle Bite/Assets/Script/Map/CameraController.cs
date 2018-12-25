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
    float minDistanceToTheHero = 32;
    [SerializeField]
    float minDistanceToTheCity = 1f;
    [SerializeField]
    float leftAndRightBuffer = 30f;
    MapHero followedHeroOnMap;
    MapCity followedCityOnMap;
    Transform followedObjectTransform;
    bool cameraIsFocused = false;
    Vector3 offset;
    Vector3 moveDirection;
    // float previousTime;
    Vector3 dragOrigin;
    bool doFollowMapDrag;
    // 
    int rotationsCounter;
    bool doShifMapTiles;
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
        // Vector3 cameraCenterWithBorderLimitsAdjusted = GetPositionWithScreenBordersLimits(cameraCenter);
        // get remaining distance
        // float remainingDistance = MapManager.Instance.GetToroidalDistance(GetPositionWithScreenBordersLimits(followedHeroOnMap.transform.position), cameraCenter);
        float remainingDistance = MapManager.Instance.GetToroidalDistance(GetPositionWithScreenBordersLimits(followedObjectTransform.position), cameraCenter);
        Debug.Log("Remaining distance " + remainingDistance);
        return remainingDistance;
    }

    IEnumerator WaitUntilCameraIsFocused(float minDistance)
    {
        // wait while camera center moves towards the followed object
        while (GetRemainingDistance() > minDistance)
        {
            yield return new WaitForSeconds(focusMoveDelay);
        }
        // set camera is focused
        cameraIsFocused = true;
    }

    public void SetCameraFocus(MapHero mapHero)
    {
        // Remove previous focus
        ResetFocusedObject();
        // save followed hero on map
        followedHeroOnMap = mapHero;
        // verify if mapHero is not null
        if (mapHero != null)
        {
            // reset camera is focused
            cameraIsFocused = false;
            // set followed object position
            followedObjectTransform = mapHero.transform;
            // start set focus animation
            MapManager.Instance.Queue.Run(WaitUntilCameraIsFocused(minDistanceToTheHero));
        }
    }

    void ResetFocusedObject()
    {
        followedHeroOnMap = null;
        followedCityOnMap = null;
        doFollowMapDrag = false;
    }

    public void SetCameraFocus(MapCity mapCity)
    {
        // Remove previous focus
        ResetFocusedObject();
        // save followed city link
        followedCityOnMap = mapCity;
        // verify if mapCity is not null
        if (mapCity != null)
        {
            Debug.Log("Set camera focus on " + mapCity.LCity.CityName + " city");
            // reset camera is focused flag
            cameraIsFocused = false;
            // set followed object position
            followedObjectTransform = mapCity.transform;
            // start set focus animation
            MapManager.Instance.Queue.Run(WaitUntilCameraIsFocused(minDistanceToTheCity));
        }
    }

    public void SetCameraFocus(MapManager mManager, bool doActivate)
    {
        // Remove previous focus
        ResetFocusedObject();
        // activate/deactivate follow map drag
        doFollowMapDrag = doActivate;
        // get and save drag origin mouse position
        dragOrigin = Input.mousePosition + transform.position;
        // reset map rotations counter
        rotationsCounter = 0;
        // reset has rotated flag
        doShifMapTiles = false;
        rotationLock = false;
    }

    Vector3 GetPositionWithScreenBordersLimits(Vector3 position)
    {
        float mapsize = 960f;
        //float tilesize = 16f;
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
        // Debug.Log("Destination Position " + (int)position.x);
        // float screenLeftLimit = Screen.width / 2 + buffer;
        float screenLeftLimit = transform.position.x - leftAndRightBuffer;
        // float screenRightLimit = mapsize - Screen.width / 2 - buffer;
        float screenRightLimit = transform.position.x + leftAndRightBuffer;
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
                // Debug.Log("Trigger rotation to the Left");
                doShifMapTiles = true;
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
                // Debug.Log("Trigger rotation to the Right");
                doShifMapTiles = true;
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
            if (doShifMapTiles)
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
                doShifMapTiles = false;
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
        else if (followedCityOnMap != null)
        {
            // Get destination position based on the map borders
            // Avoid that camera goes over top or bottom border
            // Get heroPosition
            Vector3 followedCityPosition = followedCityOnMap.transform.position;
            // Get camera position
            Vector3 point = Camera.main.WorldToViewportPoint(followedCityPosition);
            Vector3 cameraPostion = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            // Debug.LogWarning("Hero:Camera y " + (int)followedCityPosition.y + ":"  + (int)Camera.main.transform.position.y);
            // Get destination
            Vector3 delta = followedCityPosition - cameraPostion;
            Vector3 destination = transform.position + delta;
            Vector3 destinationBorderAligned = GetPositionWithScreenBordersLimits(destination);
            Vector3 destinationOYO = new Vector3(transform.position.x, destinationBorderAligned.y, transform.position.z);
            // verify if we need to rotate
            float tileSize = 16f;
            float mapWidth = 960f;
            if (doShifMapTiles)
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
                doShifMapTiles = false;
            }
            // verify that camera has not already focused on a hero
            if (!cameraIsFocused)
            {
                // move camera to the city
                transform.position = Vector3.SmoothDamp(transform.position, destinationOYO, ref velocity, dampTime, focusMoveSpeed);
            }
            else
            {
                // stop following
                followedCityOnMap = null;
            }
        }
        else if (doFollowMapDrag)
        {
            // Debug.Log("Camera before x:y " + (int)Camera.main.transform.position.x + ":" + (int)Camera.main.transform.position.y);
            float tileSize = 16f;
            float mapWidth = 960f;
            Vector3 newPos = dragOrigin - Input.mousePosition;
            Vector3 delta = newPos - Camera.main.transform.position - new Vector3(tileSize * rotationsCounter, 0, 0);
            // Debug.Log("delta x:y " + (int)delta.x + ":" + (int)delta.y);
            Vector3 destination = transform.position + delta;
            Vector3 destinationBorderAligned = GetPositionWithScreenBordersLimits(destination);
            // verify if we need to rotate (shift map slides)
            if (doShifMapTiles)
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
                doShifMapTiles = false;
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
