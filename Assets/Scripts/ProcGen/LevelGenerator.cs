using System.Collections;
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
    public GameObject terminalObject;
    public GameObject levelStartObject;
    public GameObject levelEndObject;

    public Material hackermanMaterial;
    public Material hackermanMaterialSpecial;
    public Material hackermanMaterialHiddenWall;
    public Material hackermanMaterialInternal;
    public Material hackermanMaterialDoor;

    private GameObject roomParent;
    private GameObject roomParentHackerman;
    private GameObject terminalParent;

    private List<BoxCollider> allRoomColliders = new List<BoxCollider>();
    private List<GameObject> hackermanPickups = new List<GameObject>();

    public GameObject lineObject;
    public float linePercentChance;

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
        roomParentHackerman = GameObject.FindGameObjectWithTag("Room_Parent_Hackerman");
        terminalParent = GameObject.FindGameObjectWithTag("Terminal_Parent");

        for (int i = 0; i < RoomCount; i++)
        {
            if (i == 0)
                Instantiate(levelStartObject, newPos, newRot);      // start by spawning the "start" room at the location of LevelGenerator Object (this is in addition to the other room spawned this iteration)

            int roomChoice = Random.Range(0, RoomPrefabs.Count);                                            // pick and instantiate random room (STUPIDLY called NEWGAMEOBJECT instead of NEW ROOM)
            GameObject newGameObject = Instantiate(RoomPrefabs[roomChoice], newPos, newRot) as GameObject;  //

            //////////////////////////////////////////////////////////////////////////////////////////////////

            newGameObject.transform.SetParent(roomParent.transform);
            BoxCollider currentCollider = newGameObject.transform.GetChild(3).GetComponent<BoxCollider>();  // This 100% relies on the room prefab being set up "properly", with child 4 having a box collider on it

            newGameObject.transform.Translate(newGameObject.transform.GetChild(0).transform.localPosition * -1);    // Offset by position of "IN" object (always the first child of the prefab) <-- will fail if it isn't


            GameObject newHackermanRoom = Instantiate(RoomPrefabs[roomChoice], newGameObject.transform.position + Vector3.up * 4, newRot) as GameObject;  // create a duplicate of every room above the original to be used as CYBERSPACE (Hackerman)
            newHackermanRoom.transform.SetParent(roomParentHackerman.transform);


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

            //
            // REGULAR
            //
            // floors
            for (int f = 0; f < floorsParent.transform.GetChild(0).transform.childCount; f++)
                floorsParent.transform.GetChild(0).transform.GetChild(f).GetComponent<MeshRenderer>().material = styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().floorMaterial;

            // roofs
            for (int r = 0; r < roofsParent.transform.GetChild(0).transform.childCount; r++)
                roofsParent.transform.GetChild(0).transform.GetChild(r).GetComponent<MeshRenderer>().material = styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().roofMaterial;

            // walls
            for (int w = 0; w < wallsParent.transform.GetChild(0).transform.childCount; w++)
                wallsParent.transform.GetChild(0).transform.GetChild(w).GetComponent<MeshRenderer>().material = styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().wallMaterial;
            //
            // OUTER
            //
            // floors
            for (int f = 0; f < floorsParent.transform.GetChild(1).transform.childCount; f++)
                floorsParent.transform.GetChild(1).transform.GetChild(f).GetComponent<MeshRenderer>().material = styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().outerFloorMaterial;

            // roofs
            for (int r = 0; r < roofsParent.transform.GetChild(1).transform.childCount; r++)
                roofsParent.transform.GetChild(1).transform.GetChild(r).GetComponent<MeshRenderer>().material = styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().outerRoofMaterial;

            // walls
            for (int w = 0; w < wallsParent.transform.GetChild(1).transform.childCount; w++)
                wallsParent.transform.GetChild(1).transform.GetChild(w).GetComponent<MeshRenderer>().material = styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().outerWallMaterial;

            //
            // MID <-- these are the walls that the player can't walk through, but separate the "play space" from the "sky tiles"
            //
            // walls
            for (int w = 0; w < wallsParent.transform.GetChild(2).transform.childCount; w++)
                wallsParent.transform.GetChild(2).transform.GetChild(w).GetComponent<MeshRenderer>().material = styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().midWallMaterial;


            ///////////////////////////////////////////////////////////// After changing texture on all room panels, change material on all Hackerman panels to "Hackerman"

            floorsParent = newHackermanRoom.transform.Find("Panels").gameObject.transform.Find("Floors").gameObject;                        // These 3 variables are recycled from above
            roofsParent = newHackermanRoom.transform.Find("Panels").gameObject.transform.Find("Roofs").gameObject;                          //
            wallsParent = newHackermanRoom.transform.Find("Panels").gameObject.transform.Find("Walls").gameObject;                          //
            GameObject internalWallsParent = newHackermanRoom.transform.Find("Panels").gameObject.transform.Find("Internal").gameObject;    // This one isn't
            GameObject featureWallsParent = newHackermanRoom.transform.Find("Panels").gameObject.transform.Find("Features").gameObject;     // This one isn't
            GameObject breakAwayWallsParent = newHackermanRoom.transform.Find("BREAKAWAYS").gameObject;                                     // This one isn't either

            for (int f = 0; f < floorsParent.transform.GetChild(0).transform.childCount; f++)
                floorsParent.transform.GetChild(0).transform.GetChild(f).GetComponent<MeshRenderer>().material = hackermanMaterial;
            for (int r = 0; r < roofsParent.transform.GetChild(0).transform.childCount; r++)
                roofsParent.transform.GetChild(0).transform.GetChild(r).GetComponent<MeshRenderer>().material = hackermanMaterial;
            for (int w = 0; w < wallsParent.transform.GetChild(0).transform.childCount; w++)
                wallsParent.transform.GetChild(0).transform.GetChild(w).GetComponent<MeshRenderer>().material = hackermanMaterial;
            for (int f = 0; f < floorsParent.transform.GetChild(1).transform.childCount; f++)
                floorsParent.transform.GetChild(1).transform.GetChild(f).GetComponent<MeshRenderer>().material = hackermanMaterial;
            for (int r = 0; r < roofsParent.transform.GetChild(1).transform.childCount; r++)
                roofsParent.transform.GetChild(1).transform.GetChild(r).GetComponent<MeshRenderer>().material = hackermanMaterial;
            for (int w = 0; w < wallsParent.transform.GetChild(1).transform.childCount; w++)
                wallsParent.transform.GetChild(1).transform.GetChild(w).GetComponent<MeshRenderer>().material = hackermanMaterial;
            for (int w = 0; w < wallsParent.transform.GetChild(2).transform.childCount; w++)
                wallsParent.transform.GetChild(2).transform.GetChild(w).GetComponent<MeshRenderer>().material = hackermanMaterial;                  // up to here is repeats of above, regular room texture replacement stuff

            for (int iw = 0; iw < internalWallsParent.transform.GetChild(0).transform.childCount; iw++)
                internalWallsParent.transform.GetChild(0).transform.GetChild(iw).GetComponent<MeshRenderer>().material = hackermanMaterialInternal;
            for (int iw = 0; iw < internalWallsParent.transform.GetChild(1).transform.childCount; iw++)
                internalWallsParent.transform.GetChild(1).transform.GetChild(iw).GetComponent<MeshRenderer>().material = hackermanMaterialInternal; // up to here is internal (transparent) walls

            for (int f = 0; f < featureWallsParent.transform.childCount; f++)
                featureWallsParent.transform.GetChild(f).GetComponent<MeshRenderer>().material = hackermanMaterialSpecial;                         // up to here is "feture wall, floors and roofs"

            for (int ba = 0; ba < breakAwayWallsParent.transform.childCount; ba++)
            {
                breakAwayWallsParent.transform.GetChild(ba).tag = "HackermanHiddenWall";
                for (int baChild = 0; baChild < breakAwayWallsParent.transform.GetChild(ba).transform.childCount; baChild++)
                    breakAwayWallsParent.transform.GetChild(ba).transform.GetChild(baChild).GetComponent<MeshRenderer>().material = hackermanMaterialHiddenWall;    // up to here are BREAKAWAY walls
            }

            // DOORS (are a bit of a fuck around to change to hackerman materials)

            GameObject doorsParent = newHackermanRoom.transform.Find("DOORS").gameObject.transform.GetChild(0).gameObject;      // Find Door Parent Object (which is actually it's first child called "Internal"

            for (int d = 0; d < doorsParent.transform.childCount; d++)
            {
                doorsParent.transform.GetChild(d).transform.GetChild(1).transform.GetChild(0).GetComponent<MeshRenderer>().material = hackermanMaterialDoor;
                doorsParent.transform.GetChild(d).transform.GetChild(1).transform.GetChild(1).GetComponent<MeshRenderer>().material = hackermanMaterialDoor;
                doorsParent.transform.GetChild(d).transform.GetChild(1).transform.GetChild(2).GetComponent<MeshRenderer>().material = hackermanMaterialHiddenWall;
            }

            // HIDE SHIT WE DON'T NEED FOR NOW <-- Enemies, Pickups and Props

            newHackermanRoom.transform.Find("ITEMS").gameObject.SetActive(false);
            newHackermanRoom.transform.Find("MOBS").gameObject.SetActive(false);
            newHackermanRoom.transform.Find("PROPS").gameObject.SetActive(false);

            newHackermanRoom.transform.Find("HACKERMAN").gameObject.SetActive(true);
            newGameObject.transform.Find("HACKERMAN").gameObject.SetActive(false);      // make sure hackerman objects are only on in hackman space

            for (int h = 0; h < newHackermanRoom.transform.Find("HACKERMAN").transform.GetChild(0).childCount; h++)        // for every hackerman "Group", if it's child "Group Container" is unhidden, then add all it's child prefabs to the list of pickups
            {
                if (newHackermanRoom.transform.Find("HACKERMAN").transform.GetChild(0).transform.GetChild(h).gameObject.activeSelf)
                {
                    for (int g = 0; g < newHackermanRoom.transform.Find("HACKERMAN").transform.GetChild(0).transform.GetChild(h).childCount; g++)
                    {
                        for (int b = 0; b < newHackermanRoom.transform.Find("HACKERMAN").transform.GetChild(0).transform.GetChild(h).transform.GetChild(g).childCount; b++)
                        {
                            hackermanPickups.Add(newHackermanRoom.transform.Find("HACKERMAN").transform.GetChild(0).transform.GetChild(h).transform.GetChild(g).transform.GetChild(b).gameObject);
                        }
                    }
                }
            }

            newHackermanRoom.transform.Find("HACKERMAN").transform.SetParent(GameObject.FindGameObjectWithTag("Object_Parent_Hackerman").transform);


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////


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
                int chosenExit = Random.Range(0, newGameObject.transform.GetChild(1).childCount);                // get number of "OUT" locations, "OUT" has to be the second child, and has to have a non zero amount of children
                int terminalLocation = -1;

                if ((newGameObject.transform.GetChild(1).childCount) > 1)
                {
                    //Random.InitState((int)Mathf.Floor(Time.time));                                             // don't actually need to reseed random, but going to leave it here for now anyway
                    terminalLocation = Random.Range(0, newGameObject.transform.GetChild(1).childCount);          // FOR NOW this guarantees there is one termainal per room (if the room has more than 1 exit). THIS MAY CHANGE LATER
                    if (terminalLocation == chosenExit)                                                          // make sure the terminal and door don't choose the same location
                    {
                        if (chosenExit == 0)
                            terminalLocation = 1;                                                                // if they do, and the door location is the first possibility, make the terminal the next
                        else
                            terminalLocation = chosenExit - 1;                                                   // otherwise, make the terminal the previous location to the door
                    }
                }


                newPos = newGameObject.transform.GetChild(1).GetChild(chosenExit).transform.position;        // new Pos and Rot for NEXT room determined by randomly chosen out location
                newRot = newGameObject.transform.GetChild(1).GetChild(chosenExit).transform.rotation;        //

                Vector3 newTerminalPos = newGameObject.transform.GetChild(1).GetChild(terminalLocation).transform.position;        // new Pos and Rot for NEXT room determined by randomly chosen out location
                Quaternion newTerminalRot = newGameObject.transform.GetChild(1).GetChild(terminalLocation).transform.rotation;        //


                GameObject newDoorObject = null;
                GameObject newTerminalObject = null;

                if (i == RoomCount - 1)
                    newDoorObject = Instantiate(elevatorDoorObject, newPos, newRot) as GameObject;           //create ELEVATOR door at out location
                else
                    newDoorObject = Instantiate(doorObject, newPos, newRot) as GameObject;                   // create door at out location
                newDoorObject.transform.SetParent(newGameObject.transform.GetChild(7).transform);            // parent all doors to "DOOR" child of prefab


                if (terminalLocation != -1)                                                                  // here, -1 is just shorthand for "there is no terminal"
                {
                    newTerminalObject = Instantiate(terminalObject, newTerminalPos, newTerminalRot) as GameObject;
                    newTerminalObject.transform.SetParent(terminalParent.transform);
                }


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
                    GameObject newHackermanPlugObject = Instantiate(doorPlugObject, newHackermanRoom.transform.GetChild(1).transform.GetChild(j).transform.position, newGameObject.transform.GetChild(1).transform.GetChild(j).transform.rotation) as GameObject;
                    newPlugObject.transform.SetParent(newGameObject.transform.GetChild(6).transform);                    // parent all plugs to "PLUG" child of prefab
                    newHackermanPlugObject.transform.SetParent(newHackermanRoom.transform.GetChild(6).transform);        // parent all plugs to "PLUG" child of prefab

                    // apply wall style material to every plug in the room. this only works on the mesh renderer of the 0th child
                    newPlugObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = styleListObject.GetComponent<RoomStyleTypes>().roomStyleDefs[newRoomStyleType].GetComponent<RoomStyleDef>().wallMaterial;

                    // apply hackerman material to hackerman plugs
                    newHackermanPlugObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = hackermanMaterial;


                    if (j == chosenExit)                // delete plug object if it's at the same location as the door
                    {
                        Destroy(newPlugObject);         // this makes NO sense <-- it works if I check after creating then delete, but not if I check BEFORE creating
                        Destroy(newHackermanPlugObject);
                    }
                    if (terminalLocation != -1)         // here, -1 is just shorthand for "there is no terminal"
                        if (j == terminalLocation)
                            Destroy(newPlugObject);     // also destroy the plug at the terminal location
                }

                prevRoom = newGameObject;
                prevDoor = newDoorObject;

                //////////////////////////////////////////////// here seems like as good a place as any to spawn the hackerman doors

                GameObject newHackermanDoorObject = null;
                newHackermanDoorObject = Instantiate(doorObject, newPos + Vector3.up * 4, newRot) as GameObject;                   // create door at out location
                newHackermanDoorObject.transform.SetParent(newHackermanRoom.transform.GetChild(7).transform);            // parent all doors to "DOOR" child of prefab

                // DOORS (are a bit of a fuck around to change to hackerman materials)

                doorsParent = newHackermanDoorObject.gameObject;      // Find Door Parent Object (which is actually it's first child called "Internal"

                for (int d = 0; d < doorsParent.transform.GetChild(0).transform.childCount; d++)
                    doorsParent.transform.GetChild(0).transform.GetChild(d).GetComponent<MeshRenderer>().material = hackermanMaterial;

                doorsParent.transform.GetChild(1).transform.GetChild(1).transform.GetChild(0).GetComponent<MeshRenderer>().material = hackermanMaterialDoor;
                doorsParent.transform.GetChild(1).transform.GetChild(1).transform.GetChild(1).GetComponent<MeshRenderer>().material = hackermanMaterialDoor;
                doorsParent.transform.GetChild(1).transform.GetChild(1).transform.GetChild(2).GetComponent<MeshRenderer>().material = hackermanMaterialHiddenWall;


            }   // END OF ROOM FOR LOOP

            if (i == RoomCount - 1)
               Instantiate(levelEndObject, newPos, newRot); // close off the level with an elevator (LEVEL-END prefab)
        }

        // after all the rooms are generated, create line connections between random hackerman pickups

        for (int i = 0; i < hackermanPickups.Count; i++)
        {
            if (Random.Range(0, 100) > (100.0f-linePercentChance))
            {
                int newConnection = Random.Range(0, hackermanPickups.Count);
                if (newConnection != i)
                {
                    GameObject newLine = Instantiate(lineObject, hackermanPickups[i].transform.position, Quaternion.identity) as GameObject;
                    newLine.transform.SetParent(hackermanPickups[i].transform);
                    newLine.GetComponent<LineRenderer>().SetPosition(0, hackermanPickups[i].transform.position);
                    newLine.GetComponent<LineRenderer>().SetPosition(1, hackermanPickups[newConnection].transform.position);
                }
            }
        }


        // then hide the whole hackerman parent object

        GameObject.FindGameObjectWithTag("Object_Parent_Hackerman").gameObject.SetActive(false);



        // this is just to check the correct number of objects are in the hackerman pickup list
        /*
        for (int i = 0; i < hackermanPickups.Count; i++)
        {
            Debug.Log(hackermanPickups[i]);
        }
        Debug.Log(hackermanPickups.Count);
        */

    }   // end of Awake() function
}
