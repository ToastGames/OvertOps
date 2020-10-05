﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    /////////////////////////
    // NEEDS COMMENTS PASS //
    /////////////////////////

    public GameObject testObject;
    public GameObject testObject2;

    public int RoomCount;

    public List<GameObject> RoomPrefabs;
    public GameObject doorObject;
    public GameObject elevatorDoorObject;
    public GameObject doorPlugObject;
    public GameObject levelStartObject;
    public GameObject levelEndObject;

    private GameObject roomParent;

    private List<BoxCollider> allRoomColliders = new List<BoxCollider>();

    /////////////////////////////////////////////// YES, currently all child indexes are SUPER hard coded. I should probably fix this and do it a different way at some point to the code isn't so damn precarious

   

    void Awake()
    {
        bool isCollision = false;

        Vector3 newPos = gameObject.transform.position;
        Quaternion newRot = gameObject.transform.rotation;

        GameObject prevRoom = null;
        GameObject prevDoor = null;

        List<float> testPoints = new List<float>();   // bounds to test          +X , -X , +Z , -Z
        List<float> testPoints2 = new List<float>();  // being tested against

        for (int i = 0; i < 4; i++) { testPoints.Add(0.0f); testPoints2.Add(0.0f); } // putting all this on one line to save space because it's not important


        // Assign nodes to variables by tag so it doesn't matter if the prefab heirarchy gets rearranged

        roomParent = GameObject.FindGameObjectWithTag("Room_Parent");

        for (int i = 0; i < RoomCount; i++)
        {
            if (i == 0)
                Instantiate(levelStartObject, newPos, newRot);      // start by spawning the "start" room at the location of LevelGenerator Object (this is in addition to the other room spawned this iteration)

            int roomChoice = Random.Range(0, RoomPrefabs.Count);                                            // pick and instantiate random room
            GameObject newGameObject = Instantiate(RoomPrefabs[roomChoice], newPos, newRot) as GameObject;  //
            newGameObject.transform.SetParent(roomParent.transform);
            BoxCollider currentCollider = newGameObject.transform.GetChild(3).GetComponent<BoxCollider>();  // This 100% relies on the room prefab being set up "properly", with child 4 having a box collider on it

            newGameObject.transform.Translate(newGameObject.transform.GetChild(0).transform.localPosition * -1);    // Offset by position of "IN" object (always the first child of the prefab) <-- will fail if it isn't

            GameObject styleListObject = newGameObject.transform.Find("StyleList").gameObject;
            GameObject panelParent = newGameObject.transform.Find("Panels").gameObject; // <-----------------------------------------------// not used?
            GameObject floorsParent = newGameObject.transform.Find("Panels").gameObject.transform.Find("Floors").gameObject;
            GameObject roofsParent = newGameObject.transform.Find("Panels").gameObject.transform.Find("Roofs").gameObject;
            GameObject wallsParent = newGameObject.transform.Find("Panels").gameObject.transform.Find("Walls").gameObject;


            ////////////////////////////////////////// assign material type per room //////////////// ALL THIS NUMBER SHIT has now been replaced by variables
            // "style list" object is child 12
            // "panels" object is child 4
            // children of panels --> 0, 1, 2 are Floors, Roofs, Walls in that order, and for all of them, child 0 is "regular"

            int newRoomStyleType = Random.Range(0, styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs.Count);

            // floors
            for (int f = 0; f < floorsParent.transform.childCount; f++)
                floorsParent.transform.GetChild(0).transform.GetChild(f).GetComponent<MeshRenderer>().material = styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().floorMaterial;

            // roofs
            for (int r = 0; r < roofsParent.transform.childCount; r++)
                roofsParent.transform.GetChild(0).transform.GetChild(r).GetComponent<MeshRenderer>().material = styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().roofMaterial;

            // walls
            for (int w = 0; w < wallsParent.transform.childCount; w++)
                wallsParent.transform.GetChild(0).transform.GetChild(w).GetComponent<MeshRenderer>().material = styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().wallMaterial;

            ///////

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Intersection detection of room generation is STILL BUGGY AS FUCK. Need to come back to this later with fresh eyes
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            testPoints[0] = currentCollider.transform.position.x + currentCollider.bounds.extents.x;
            testPoints[1] = currentCollider.transform.position.x - currentCollider.bounds.extents.x;
            testPoints[2] = currentCollider.transform.position.z + currentCollider.bounds.extents.z;
            testPoints[3] = currentCollider.transform.position.z - currentCollider.bounds.extents.z;

            bool keepChecking = true;

            for (int j = 0; j < allRoomColliders.Count; j++)
            {
                if ((i > 0) && (keepChecking))
                {
                    testPoints2[0] = allRoomColliders[j].transform.position.x + allRoomColliders[j].bounds.extents.x;
                    testPoints2[1] = allRoomColliders[j].transform.position.x - allRoomColliders[j].bounds.extents.x;
                    testPoints2[2] = allRoomColliders[j].transform.position.z + allRoomColliders[j].bounds.extents.z;
                    testPoints2[3] = allRoomColliders[j].transform.position.z - allRoomColliders[j].bounds.extents.z;

                    if ((((testPoints[1] > testPoints2[1]) && (testPoints[1] < testPoints2[0])) ||
                         ((testPoints[0] < testPoints2[0]) && (testPoints[0] > testPoints2[1])))
                        &&
                        (((testPoints[3] > testPoints2[3]) && (testPoints[3] < testPoints2[2])) ||
                         ((testPoints[2] < testPoints2[2]) && (testPoints[2] > testPoints2[3]))))
                    {
                        isCollision = true;
                        keepChecking = false;
                    }
                    else
                        isCollision = false;
                }
            }
            allRoomColliders.Add(currentCollider);  // <-- do I still need this shit? maybe strip it out later if other method ends up working
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (isCollision)
            {
                Destroy(newGameObject);
            }
            else
            {
                int chosenExit = Random.Range(0, newGameObject.transform.GetChild(1).childCount);            // get number of "OUT" locations, "OUT" has to be the second child, and has to have a non zero amount of children

                newPos = newGameObject.transform.GetChild(1).GetChild(chosenExit).transform.position;        // new Pos and Rot for NEXT room determined by randomly chosen out location
                newRot = newGameObject.transform.GetChild(1).GetChild(chosenExit).transform.rotation;        //

                GameObject newDoorObject = null;
                if (i == RoomCount - 1)
                    newDoorObject = Instantiate(elevatorDoorObject, newPos, newRot) as GameObject;           //create ELEVATOR door at out location
                else
                    newDoorObject = Instantiate(doorObject, newPos, newRot) as GameObject;                   // create door at out location
                newDoorObject.transform.SetParent(newGameObject.transform.GetChild(7).transform);            // parent all doors to "DOOR" child of prefab


                if (i > 0)
                {
                    GameObject prevStyleListObject = prevRoom.transform.Find("StyleList").gameObject;

                    // set doorway textures for _previous_ doorway (if there is one)
                    prevDoor.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material.SetTexture("_OverlayTexture", prevStyleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().DoorFloorTexture);
                    prevDoor.transform.GetChild(0).transform.GetChild(1).GetComponent<MeshRenderer>().material.SetTexture("_OverlayTexture", prevStyleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().DoorWallTexture);
                    prevDoor.transform.GetChild(0).transform.GetChild(2).GetComponent<MeshRenderer>().material.SetTexture("_OverlayTexture", prevStyleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().DoorWallTexture);
                    prevDoor.transform.GetChild(0).transform.GetChild(3).GetComponent<MeshRenderer>().material.SetTexture("_OverlayTexture", prevStyleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().DoorRoofTexture);
                }

                // set doorway textures for _next_ doorway
                newDoorObject.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material.SetTexture("_MainTexture", styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().DoorFloorTexture);
                newDoorObject.transform.GetChild(0).transform.GetChild(1).GetComponent<MeshRenderer>().material.SetTexture("_MainTexture", styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().DoorWallTexture);
                newDoorObject.transform.GetChild(0).transform.GetChild(2).GetComponent<MeshRenderer>().material.SetTexture("_MainTexture", styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().DoorWallTexture);
                newDoorObject.transform.GetChild(0).transform.GetChild(3).GetComponent<MeshRenderer>().material.SetTexture("_MainTexture", styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().DoorRoofTexture);
                newDoorObject.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material.SetTexture("_TrimTexture", styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().DoorTrimTexture);
                newDoorObject.transform.GetChild(0).transform.GetChild(1).GetComponent<MeshRenderer>().material.SetTexture("_TrimTexture", styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().DoorTrimTexture);
                newDoorObject.transform.GetChild(0).transform.GetChild(2).GetComponent<MeshRenderer>().material.SetTexture("_TrimTexture", styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().DoorTrimTexture);
                newDoorObject.transform.GetChild(0).transform.GetChild(3).GetComponent<MeshRenderer>().material.SetTexture("_TrimTexture", styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().DoorTrimTexture);


                for (int j = 0; j < newGameObject.transform.GetChild(1).childCount; j++)                     // creat a PLUG object at ALL OUT locations
                {
                    GameObject newPlugObject = Instantiate(doorPlugObject, newGameObject.transform.GetChild(1).transform.GetChild(j).transform.position, newGameObject.transform.GetChild(1).transform.GetChild(j).transform.rotation) as GameObject;
                    newPlugObject.transform.SetParent(newGameObject.transform.GetChild(6).transform);        // parent all plugs to "PLUG" child of prefab

                    // apply wall style material to every plug in the room. this only works on the mesh renderer of the 0th child
                    newPlugObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().wallMaterial;

                    if (j == chosenExit)            // delete plug object if it's at the same location as the door
                        Destroy(newPlugObject);     // this makes NO sense <-- it works if I check after creating then delete, but not if I check BEFORE creating
                }

                prevRoom = newGameObject;
                prevDoor = newDoorObject;
            }

            if (i == RoomCount - 1)
               Instantiate(levelEndObject, newPos, newRot); // close off the level with an elevator (LEVEL-END prefab)
        }
    }


}