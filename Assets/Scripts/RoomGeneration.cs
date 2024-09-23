using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{
    [Header("Variables")]
    public int roomsToGenerate;
    
    [Space(5)]
    [Header("DataBase")]
    public DungeonSingleRoom spawnRoom;
    [Space(5)]
    public RoomDataBase roomsDB;

    public int dungeonBoundary;
    public bool[,] occupied;

    private List<Room> rooms = new List<Room>();
    public List<GameObject> gos = new List<GameObject>();

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        StartGeneration();
        InstantiateRooms();
        CheckGeneration();
    }

    private void StartGeneration()
    {
        while(transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
        rooms.Clear();
        gos.Clear();
        dungeonBoundary = roomsToGenerate * roomsToGenerate;
        occupied = new bool[dungeonBoundary, dungeonBoundary];

        int baseX = dungeonBoundary/2, baseY = dungeonBoundary/2;
        DungeonSingleRoom currentRoom = spawnRoom.DeepCopy();
        currentRoom.aRoom.baseX = baseX;
        currentRoom.aRoom.baseY = baseY;

        Debug.Log("firstX" + baseX);
        Debug.Log("firstY" + baseY);
        rooms.Add(currentRoom.aRoom);

        for (int i = 0; i < currentRoom.aRoom.sizeX; i++)
        {
            for (int j = 0; j < currentRoom.aRoom.sizeY; j++)
            {
                occupied[currentRoom.aRoom.baseX + i, currentRoom.aRoom.baseY + j] = true;
            }
        }

        bool[] openedDoors = ExtensionMethods.ListOfBool(4, -1);

        int tester = -1;
        for (int i = 0; i < rooms.Count; i++)
        {
            if (currentRoom.aRoom == rooms[i])
            {
                tester = i;
                break;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (openedDoors[i] == true)
            {
                bool hasADoor = true;
                int doorNumber;
                int baseDoorOffset = 0;
                switch (i)
                {
                    case 0:
                        if (currentRoom.aRoom.doorT == null || currentRoom.aRoom.doorT.Length <= 0)
                            hasADoor = false;
                        else
                        {
                            doorNumber = Random.Range(0, currentRoom.aRoom.doorT.Length);
                            baseDoorOffset = currentRoom.aRoom.doorT[doorNumber].offset;
                        }
                        break;
                    case 1:
                        if (currentRoom.aRoom.doorB == null || currentRoom.aRoom.doorB.Length <= 0)
                            hasADoor = false;
                        else
                        {
                            doorNumber = Random.Range(0, currentRoom.aRoom.doorB.Length);
                            baseDoorOffset = currentRoom.aRoom.doorB[doorNumber].offset;
                        }
                        break;
                    case 2:
                        if (currentRoom.aRoom.doorL == null || currentRoom.aRoom.doorL.Length <= 0)
                            hasADoor = false;
                        else
                        {
                            doorNumber = Random.Range(0, currentRoom.aRoom.doorL.Length);
                            baseDoorOffset = currentRoom.aRoom.doorL[doorNumber].offset;
                        }
                        break;
                    case 3:
                        if (currentRoom.aRoom.doorR == null || currentRoom.aRoom.doorR.Length <= 0)
                            hasADoor = false;
                        else
                        {
                            doorNumber = Random.Range(0, currentRoom.aRoom.doorR.Length);
                            baseDoorOffset = currentRoom.aRoom.doorR[doorNumber].offset;
                        }
                        break;
                }
                if (!hasADoor)
                    continue;
                Debug.Log("Gen called by " + tester);
                //上下左右 0 1 2 3
                GenerateRoom((i != 3) ? baseX : baseX + currentRoom.aRoom.sizeX, (i != 0) ? baseY : baseY + currentRoom.aRoom.sizeY, i, baseDoorOffset);
            }
        }
    }

    //maybe return false if generate fail
    private void GenerateRoom(int baseX, int baseY, int caseNumber, int baseOffset)
    {
        int iteration = 0;
        if (rooms.Count >= roomsToGenerate)
            return;
        // caseNo 上下左右 0 1 2 3
        // = from 下上右左 1 0 3 2
        while (iteration <= 10)
        {
            iteration++;
            //Pick A Room
            DungeonSingleRoom roomToTest = PickARoom();
            //Pick A Door

            bool hasADoor = true;
            int doorNumber;
            int newDoorOffset = 0;
            switch (caseNumber)
            {
                case 0:
                    if (roomToTest.aRoom.doorB == null || roomToTest.aRoom.doorB.Length <= 0)
                        hasADoor = false;
                    else
                    {
                        doorNumber = Random.Range(0, roomToTest.aRoom.doorB.Length);
                        newDoorOffset = roomToTest.aRoom.doorB[doorNumber].offset;
                    }
                    break;
                case 1:
                    if (roomToTest.aRoom.doorT == null || roomToTest.aRoom.doorT.Length <= 0)
                        hasADoor = false;
                    else
                    {
                        doorNumber = Random.Range(0, roomToTest.aRoom.doorT.Length);
                        newDoorOffset = roomToTest.aRoom.doorT[doorNumber].offset;
                    }
                    break;
                case 2:
                    if (roomToTest.aRoom.doorR == null || roomToTest.aRoom.doorR.Length <= 0)
                        hasADoor = false;
                    else
                    {
                        doorNumber = Random.Range(0, roomToTest.aRoom.doorR.Length);
                        newDoorOffset = roomToTest.aRoom.doorR[doorNumber].offset;
                    }
                    break;
                case 3:
                    if (roomToTest.aRoom.doorL == null || roomToTest.aRoom.doorL.Length <= 0)
                        hasADoor = false;
                    else
                    {
                        doorNumber = Random.Range(0, roomToTest.aRoom.doorL.Length);
                        newDoorOffset = roomToTest.aRoom.doorL[doorNumber].offset;
                    }
                    break;
            }
            if (!hasADoor)
                return;

            //Check Valid
            if (!CheckValid(roomToTest.aRoom, baseX, baseY, caseNumber, baseOffset, newDoorOffset))
                continue;
            roomToTest.aRoom.baseX = caseNumber != 2 ? baseX : baseX - roomToTest.aRoom.sizeX;
            roomToTest.aRoom.baseY = caseNumber != 1 ? baseY : baseY - roomToTest.aRoom.sizeY;
            if (caseNumber <= 1)
                roomToTest.aRoom.baseX += baseOffset - newDoorOffset;
            else
                roomToTest.aRoom.baseY += baseOffset - newDoorOffset;

            Debug.Log(baseX + "  " + baseY);
            Debug.Log("base" + baseOffset);
            Debug.Log("new" + newDoorOffset);
            Debug.Log("case" + caseNumber);
            rooms.Add(roomToTest.aRoom);

            /// Generate Rooms for the newly generated room
            // caseNo 上下左右 0 1 2 3
            // = from 下上右左 1 0 3 2
            bool[] openedDoors = ExtensionMethods.ListOfBool(4, caseNumber%2 == 0 ? caseNumber + 1 : caseNumber - 1);

            int tester1 = -1;
            for (int i = 0; i < rooms.Count; i++)
            {
                if (roomToTest.aRoom == rooms[i])
                {
                    tester1 = i;
                    break;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (openedDoors[i] == true)
                {
                    bool hasADoor1 = true;
                    int doorNumber1;
                    int baseDoorOffset = 0;
                    switch (i)
                    {
                        case 0:
                            if (roomToTest.aRoom.doorT == null || roomToTest.aRoom.doorT.Length <= 0)
                                hasADoor1 = false;
                            else
                            {
                                doorNumber1 = Random.Range(0, roomToTest.aRoom.doorT.Length);
                                baseDoorOffset = roomToTest.aRoom.doorT[doorNumber1].offset;
                            }
                            break;
                        case 1:
                            if (roomToTest.aRoom.doorB == null || roomToTest.aRoom.doorB.Length <= 0)
                                hasADoor1 = false;
                            else
                            {
                                doorNumber1 = Random.Range(0, roomToTest.aRoom.doorB.Length);
                                baseDoorOffset = roomToTest.aRoom.doorB[doorNumber1].offset;
                            }
                            break;
                        case 2:
                            if (roomToTest.aRoom.doorL == null || roomToTest.aRoom.doorL.Length <= 0)
                                hasADoor1 = false;
                            else
                            {
                                doorNumber1 = Random.Range(0, roomToTest.aRoom.doorL.Length);
                                baseDoorOffset = roomToTest.aRoom.doorL[doorNumber1].offset;
                            }
                            break;
                        case 3:
                            if (roomToTest.aRoom.doorR == null || roomToTest.aRoom.doorR.Length <= 0)
                                hasADoor1 = false;
                            else
                            {
                                doorNumber1 = Random.Range(0, roomToTest.aRoom.doorR.Length);
                                baseDoorOffset = roomToTest.aRoom.doorR[doorNumber1].offset;
                            }
                            break;
                    }
                    if (!hasADoor1)
                        continue;
                    Debug.Log("Gen called by " + tester1);
                    //上下左右 0 1 2 3
                    GenerateRoom((i != 3) ? roomToTest.aRoom.baseX : roomToTest.aRoom.baseX + roomToTest.aRoom.sizeX, (i != 0) ? roomToTest.aRoom.baseY : roomToTest.aRoom.baseY + roomToTest.aRoom.sizeY, i, baseDoorOffset);
                }
            }

            return; //true;
        }
        return; //false;
    }

    private void InstantiateRooms()
    { 
        for(int i =0;i < rooms.Count;i++)
        {
            gos.Add(Instantiate(rooms[i].roomGO, new Vector3(rooms[i].baseX *5, 0, rooms[i].baseY *5), Quaternion.identity, transform));
        }
    }

    private DungeonSingleRoom PickARoom()
    {
        int roomType = Random.Range(0, roomsDB.data.Length);
        int roomNumber = Random.Range(0, roomsDB.data[roomType].rooms.Length);
        return roomsDB.data[roomType].rooms[roomNumber].DeepCopy();
    }

    private bool CheckValid(Room roomToCheck, int currentX, int currentY, int caseNumber, int originalOffset, int newOffset)
    {
        if (currentX < 0 || currentY < 0 || currentX > roomsToGenerate * roomsToGenerate * 9 || currentY > roomsToGenerate * roomsToGenerate * 9)
            return false;

        //上下左右 0 1 2 3
        switch (caseNumber)
        {
            case 0:
                {
                    if (currentX + roomToCheck.sizeX + originalOffset - newOffset > dungeonBoundary - 1 || currentY + roomToCheck.sizeY > dungeonBoundary - 1)
                        return false;
                    for (int i = 0; i < roomToCheck.sizeX; i++)
                    {
                        for (int j = 0; j < roomToCheck.sizeY; j++)
                        {
                            if (occupied[currentX + originalOffset - newOffset + i, currentY + j] == true)
                                return false;
                        }
                    }
                    for (int i = 0; i < roomToCheck.sizeX; i++)
                    {
                        for (int j = 0; j < roomToCheck.sizeY; j++)
                        {
                            occupied[currentX + originalOffset - newOffset + i, currentY + j] = true;
                        }
                    }
                }
                return true;
            case 1:
                {
                    if (currentX + originalOffset - newOffset + roomToCheck.sizeX > dungeonBoundary-1 || currentY - roomToCheck.sizeY < 0)
                        return false;
                    for (int i = 0; i < roomToCheck.sizeX; i++)
                    {
                        for (int j = 0; j < roomToCheck.sizeY; j++)
                        {
                            if (occupied[currentX + originalOffset - newOffset + i, currentY - roomToCheck.sizeY + j] == true)
                                return false;
                        }
                    }
                    for (int i = 0; i < roomToCheck.sizeX; i++)
                    {
                        for (int j = 0; j < roomToCheck.sizeY; j++)
                        {
                            occupied[currentX + originalOffset - newOffset + i, currentY - roomToCheck.sizeY + j] = true;
                        }
                    }
                }
                return true;
            case 2:
                {
                    if (currentX - roomToCheck.sizeX < 0 || currentY + originalOffset - newOffset + roomToCheck.sizeY > dungeonBoundary-1)
                        return false;
                    for (int i = 0; i < roomToCheck.sizeX; i++)
                    {
                        for (int j = 0; j < roomToCheck.sizeY; j++)
                        {
                            if (occupied[currentX - roomToCheck.sizeX + i, currentY + originalOffset - newOffset + j] == true)
                                return false;
                        }
                    }
                    for (int i = 0; i < roomToCheck.sizeX; i++)
                    {
                        for (int j = 0; j < roomToCheck.sizeY; j++)
                        {
                            occupied[currentX - roomToCheck.sizeX + i, currentY + originalOffset - newOffset + j] = true;
                        }
                    }
                }
                return true;
            case 3:
                {
                    if (currentX - newOffset > dungeonBoundary - 1 || currentY + originalOffset - newOffset + roomToCheck.sizeY > dungeonBoundary - 1)
                        return false;
                    for (int i = 0; i < roomToCheck.sizeX; i++)
                    {
                        for (int j = 0; j < roomToCheck.sizeY; j++)
                        {
                            if (occupied[currentX + i, currentY + originalOffset - newOffset + j] == true)
                                return false;
                        }
                    }
                    for (int i = 0; i < roomToCheck.sizeX; i++)
                    {
                        for (int j = 0; j < roomToCheck.sizeY; j++)
                        {
                            occupied[currentX + i, currentY + originalOffset - newOffset + j] = true;
                        }
                    }
                }
                return true;
            default:
                Debug.Log("FAILURE");
                return false;
        }
    }

    private void CheckGeneration()
    {
        if (rooms.Count < roomsToGenerate)
        {
            Generate();
        }
    }
}

[System.Serializable]
public class Room
{
    public int sizeX;
    public int sizeY;
    public int baseX;
    public int baseY;
    public Door[] doorL;
    public Door[] doorR;
    public Door[] doorT;
    public Door[] doorB;
    public GameObject roomGO;

    [System.Serializable]
    public class Door
    {
        public int offset;
        public GameObject doorGO;
    }

    public Room DeepCopy()
    {
        Room copy = new Room();
        copy.sizeX = sizeX;
        copy.sizeY = sizeY;
        copy.doorL = doorL;
        copy.doorR = doorR;
        copy.doorT = doorT;
        copy.doorB = doorB;
        copy.roomGO = roomGO;
        return copy;
    }
}

[System.Serializable]
public class RoomDataBase
{
    public RoomData[] data;
}

[System.Serializable]
public class RoomData
{
    public string type = "";
    public DungeonSingleRoom[] rooms;
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class RoomGeneration : MonoBehaviour
//{
//    [Header("Variables")]
//    public int roomsToGenerate;

//    [Space(5)]
//    [Header("DataBase")]
//    public Room spawnRoom;
//    [Space(5)]
//    public RoomDataBase roomsDB;

//    public int dungeonBoundary;
//    public bool[,] occupied;

//    private List<Room> rooms = new List<Room>();
//    public List<GameObject> gos = new List<GameObject>();

//    private void Start()
//    {
//        Generate();
//    }

//    public void Generate()
//    {
//        StartGeneration();
//        InstantiateRooms();
//        CheckGeneration();
//    }

//    private void StartGeneration()
//    {
//        while (transform.childCount > 0)
//            DestroyImmediate(transform.GetChild(0).gameObject);
//        rooms.Clear();
//        gos.Clear();
//        dungeonBoundary = roomsToGenerate * roomsToGenerate;
//        occupied = new bool[dungeonBoundary, dungeonBoundary];

//        int baseX = dungeonBoundary / 2, baseY = dungeonBoundary / 2;
//        Room currentRoom.aRoom = spawnRoom.DeepCopy();
//        currentRoom.aRoom.baseX = baseX;
//        currentRoom.aRoom.baseY = baseY;

//        Debug.Log("firstX" + baseX);
//        Debug.Log("firstY" + baseY);
//        rooms.Add(currentRoom.aRoom);

//        for (int i = 0; i < currentRoom.aRoom.sizeX; i++)
//        {
//            for (int j = 0; j < currentRoom.aRoom.sizeY; j++)
//            {
//                occupied[currentRoom.aRoom.baseX + i, currentRoom.aRoom.baseY + j] = true;
//            }
//        }

//        bool[] openedDoors = ExtensionMethods.ListOfBool(4, -1);

//        int tester = -1;
//        for (int i = 0; i < rooms.Count; i++)
//        {
//            if (currentRoom.aRoom == rooms[i])
//            {
//                tester = i;
//                break;
//            }
//        }

//        for (int i = 0; i < 4; i++)
//        {
//            if (openedDoors[i] == true)
//            {
//                bool hasADoor = true;
//                int doorNumber;
//                int baseDoorOffset = 0;
//                switch (i)
//                {
//                    case 0:
//                        if (currentRoom.aRoom.doorT == null || currentRoom.aRoom.doorT.Length <= 0)
//                            hasADoor = false;
//                        else
//                        {
//                            doorNumber = Random.Range(0, currentRoom.aRoom.doorT.Length);
//                            baseDoorOffset = currentRoom.aRoom.doorT[doorNumber].offset;
//                        }
//                        break;
//                    case 1:
//                        if (currentRoom.aRoom.doorB == null || currentRoom.aRoom.doorB.Length <= 0)
//                            hasADoor = false;
//                        else
//                        {
//                            doorNumber = Random.Range(0, currentRoom.aRoom.doorB.Length);
//                            baseDoorOffset = currentRoom.aRoom.doorB[doorNumber].offset;
//                        }
//                        break;
//                    case 2:
//                        if (currentRoom.aRoom.doorL == null || currentRoom.aRoom.doorL.Length <= 0)
//                            hasADoor = false;
//                        else
//                        {
//                            doorNumber = Random.Range(0, currentRoom.aRoom.doorL.Length);
//                            baseDoorOffset = currentRoom.aRoom.doorL[doorNumber].offset;
//                        }
//                        break;
//                    case 3:
//                        if (currentRoom.aRoom.doorR == null || currentRoom.aRoom.doorR.Length <= 0)
//                            hasADoor = false;
//                        else
//                        {
//                            doorNumber = Random.Range(0, currentRoom.aRoom.doorR.Length);
//                            baseDoorOffset = currentRoom.aRoom.doorR[doorNumber].offset;
//                        }
//                        break;
//                }
//                if (!hasADoor)
//                    continue;
//                Debug.Log("Gen called by " + tester);
//                //上下左右 0 1 2 3
//                GenerateRoom((i != 3) ? baseX : baseX + currentRoom.aRoom.sizeX, (i != 0) ? baseY : baseY + currentRoom.aRoom.sizeY, i, baseDoorOffset);
//            }
//        }
//    }

//    //maybe return false if generate fail
//    private void GenerateRoom(int baseX, int baseY, int caseNumber, int baseOffset)
//    {
//        int iteration = 0;
//        if (rooms.Count >= roomsToGenerate)
//            return;
//        // caseNo 上下左右 0 1 2 3
//        // = from 下上右左 1 0 3 2
//        while (iteration <= 10)
//        {
//            iteration++;
//            //Pick A Room
//            Room roomToTest.aRoom = PickARoom();
//            //Pick A Door

//            bool hasADoor = true;
//            int doorNumber;
//            int newDoorOffset = 0;
//            switch (caseNumber)
//            {
//                case 0:
//                    if (roomToTest.aRoom.doorB == null || roomToTest.aRoom.doorB.Length <= 0)
//                        hasADoor = false;
//                    else
//                    {
//                        doorNumber = Random.Range(0, roomToTest.aRoom.doorB.Length);
//                        newDoorOffset = roomToTest.aRoom.doorB[doorNumber].offset;
//                    }
//                    break;
//                case 1:
//                    if (roomToTest.aRoom.doorT == null || roomToTest.aRoom.doorT.Length <= 0)
//                        hasADoor = false;
//                    else
//                    {
//                        doorNumber = Random.Range(0, roomToTest.aRoom.doorT.Length);
//                        newDoorOffset = roomToTest.aRoom.doorT[doorNumber].offset;
//                    }
//                    break;
//                case 2:
//                    if (roomToTest.aRoom.doorR == null || roomToTest.aRoom.doorR.Length <= 0)
//                        hasADoor = false;
//                    else
//                    {
//                        doorNumber = Random.Range(0, roomToTest.aRoom.doorR.Length);
//                        newDoorOffset = roomToTest.aRoom.doorR[doorNumber].offset;
//                    }
//                    break;
//                case 3:
//                    if (roomToTest.aRoom.doorL == null || roomToTest.aRoom.doorL.Length <= 0)
//                        hasADoor = false;
//                    else
//                    {
//                        doorNumber = Random.Range(0, roomToTest.aRoom.doorL.Length);
//                        newDoorOffset = roomToTest.aRoom.doorL[doorNumber].offset;
//                    }
//                    break;
//            }
//            if (!hasADoor)
//                return;

//            //Check Valid
//            if (!CheckValid(roomToTest.aRoom, baseX, baseY, caseNumber, baseOffset, newDoorOffset))
//                continue;
//            roomToTest.aRoom.baseX = caseNumber != 2 ? baseX : baseX - roomToTest.aRoom.sizeX;
//            roomToTest.aRoom.baseY = caseNumber != 1 ? baseY : baseY - roomToTest.aRoom.sizeY;
//            if (caseNumber <= 1)
//                roomToTest.aRoom.baseX += baseOffset - newDoorOffset;
//            else
//                roomToTest.aRoom.baseY += baseOffset - newDoorOffset;

//            Debug.Log(baseX + "  " + baseY);
//            Debug.Log("base" + baseOffset);
//            Debug.Log("new" + newDoorOffset);
//            Debug.Log("case" + caseNumber);
//            rooms.Add(roomToTest.aRoom);

//            /// Generate Rooms for the newly generated room
//            // caseNo 上下左右 0 1 2 3
//            // = from 下上右左 1 0 3 2
//            bool[] openedDoors = ExtensionMethods.ListOfBool(4, caseNumber % 2 == 0 ? caseNumber + 1 : caseNumber - 1);

//            int tester1 = -1;
//            for (int i = 0; i < rooms.Count; i++)
//            {
//                if (roomToTest.aRoom == rooms[i])
//                {
//                    tester1 = i;
//                    break;
//                }
//            }

//            for (int i = 0; i < 4; i++)
//            {
//                if (openedDoors[i] == true)
//                {
//                    bool hasADoor1 = true;
//                    int doorNumber1;
//                    int baseDoorOffset = 0;
//                    switch (i)
//                    {
//                        case 0:
//                            if (roomToTest.aRoom.doorT == null || roomToTest.aRoom.doorT.Length <= 0)
//                                hasADoor1 = false;
//                            else
//                            {
//                                doorNumber1 = Random.Range(0, roomToTest.aRoom.doorT.Length);
//                                baseDoorOffset = roomToTest.aRoom.doorT[doorNumber1].offset;
//                            }
//                            break;
//                        case 1:
//                            if (roomToTest.aRoom.doorB == null || roomToTest.aRoom.doorB.Length <= 0)
//                                hasADoor1 = false;
//                            else
//                            {
//                                doorNumber1 = Random.Range(0, roomToTest.aRoom.doorB.Length);
//                                baseDoorOffset = roomToTest.aRoom.doorB[doorNumber1].offset;
//                            }
//                            break;
//                        case 2:
//                            if (roomToTest.aRoom.doorL == null || roomToTest.aRoom.doorL.Length <= 0)
//                                hasADoor1 = false;
//                            else
//                            {
//                                doorNumber1 = Random.Range(0, roomToTest.aRoom.doorL.Length);
//                                baseDoorOffset = roomToTest.aRoom.doorL[doorNumber1].offset;
//                            }
//                            break;
//                        case 3:
//                            if (roomToTest.aRoom.doorR == null || roomToTest.aRoom.doorR.Length <= 0)
//                                hasADoor1 = false;
//                            else
//                            {
//                                doorNumber1 = Random.Range(0, roomToTest.aRoom.doorR.Length);
//                                baseDoorOffset = roomToTest.aRoom.doorR[doorNumber1].offset;
//                            }
//                            break;
//                    }
//                    if (!hasADoor1)
//                        continue;
//                    Debug.Log("Gen called by " + tester1);
//                    //上下左右 0 1 2 3
//                    GenerateRoom((i != 3) ? roomToTest.aRoom.baseX : roomToTest.aRoom.baseX + roomToTest.aRoom.sizeX, (i != 0) ? roomToTest.aRoom.baseY : roomToTest.aRoom.baseY + roomToTest.aRoom.sizeY, i, baseDoorOffset);
//                }
//            }

//            return; //true;
//        }
//        return; //false;
//    }

//    private void InstantiateRooms()
//    {
//        for (int i = 0; i < rooms.Count; i++)
//        {
//            gos.Add(Instantiate(rooms[i].roomGO, new Vector3(rooms[i].baseX * 5, 0, rooms[i].baseY * 5), Quaternion.identity, transform));
//        }
//    }

//    private Room PickARoom()
//    {
//        int roomType = Random.Range(0, roomsDB.data.Length);
//        int roomNumber = Random.Range(0, roomsDB.data[roomType].rooms.Length);
//        return roomsDB.data[roomType].rooms[roomNumber].DeepCopy();
//    }

//    private bool CheckValid(Room roomToCheck, int currentX, int currentY, int caseNumber, int originalOffset, int newOffset)
//    {
//        if (currentX < 0 || currentY < 0 || currentX > roomsToGenerate * roomsToGenerate * 9 || currentY > roomsToGenerate * roomsToGenerate * 9)
//            return false;

//        //上下左右 0 1 2 3
//        switch (caseNumber)
//        {
//            case 0:
//                {
//                    if (currentX + roomToCheck.sizeX + originalOffset - newOffset > dungeonBoundary - 1 || currentY + roomToCheck.sizeY > dungeonBoundary - 1)
//                        return false;
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            if (occupied[currentX + originalOffset - newOffset + i, currentY + j] == true)
//                                return false;
//                        }
//                    }
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            occupied[currentX + originalOffset - newOffset + i, currentY + j] = true;
//                        }
//                    }
//                }
//                return true;
//            case 1:
//                {
//                    if (currentX + originalOffset - newOffset + roomToCheck.sizeX > dungeonBoundary - 1 || currentY - roomToCheck.sizeY < 0)
//                        return false;
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            if (occupied[currentX + originalOffset - newOffset + i, currentY - roomToCheck.sizeY + j] == true)
//                                return false;
//                        }
//                    }
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            occupied[currentX + originalOffset - newOffset + i, currentY - roomToCheck.sizeY + j] = true;
//                        }
//                    }
//                }
//                return true;
//            case 2:
//                {
//                    if (currentX - roomToCheck.sizeX < 0 || currentY + originalOffset - newOffset + roomToCheck.sizeY > dungeonBoundary - 1)
//                        return false;
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            if (occupied[currentX - roomToCheck.sizeX + i, currentY + originalOffset - newOffset + j] == true)
//                                return false;
//                        }
//                    }
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            occupied[currentX - roomToCheck.sizeX + i, currentY + originalOffset - newOffset + j] = true;
//                        }
//                    }
//                }
//                return true;
//            case 3:
//                {
//                    if (currentX - newOffset > dungeonBoundary - 1 || currentY + originalOffset - newOffset + roomToCheck.sizeY > dungeonBoundary - 1)
//                        return false;
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            if (occupied[currentX + i, currentY + originalOffset - newOffset + j] == true)
//                                return false;
//                        }
//                    }
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            occupied[currentX + i, currentY + originalOffset - newOffset + j] = true;
//                        }
//                    }
//                }
//                return true;
//            default:
//                Debug.Log("FAILURE");
//                return false;
//        }
//    }

//    private void CheckGeneration()
//    {
//        if (rooms.Count < roomsToGenerate)
//        {
//            Generate();
//        }
//    }
//}

//[System.Serializable]
//public class Room
//{
//    public int sizeX;
//    public int sizeY;
//    public int baseX;
//    public int baseY;
//    public Door[] doorL;
//    public Door[] doorR;
//    public Door[] doorT;
//    public Door[] doorB;
//    public GameObject roomGO;

//    [System.Serializable]
//    public class Door
//    {
//        public int offset;
//        public GameObject doorGO;
//    }

//    public Room DeepCopy()
//    {
//        Room copy = new Room();
//        copy.sizeX = sizeX;
//        copy.sizeY = sizeY;
//        copy.doorL = doorL;
//        copy.doorR = doorR;
//        copy.doorT = doorT;
//        copy.doorB = doorB;
//        copy.roomGO = roomGO;
//        return copy;
//    }
//}

//[System.Serializable]
//public class RoomDataBase
//{
//    public RoomData[] data;
//}

//[System.Serializable]
//public class RoomData
//{
//    public string type = "";
//    public Room[] rooms;
//}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class RoomGeneration : MonoBehaviour
//{
//    [Header("Variables")]
//    public int roomsToGenerate;

//    [Space(5)]
//    [Header("DataBase")]
//    public Room spawnRoom;
//    [Space(5)]
//    public RoomDataBase roomsDB;

//    public int dungeonBoundary;
//    public bool[,] occupied;

//    private List<Room> rooms = new List<Room>();
//    public List<GameObject> gos = new List<GameObject>();

//    private void Start()
//    {
//        Generate();
//    }

//    public void Generate()
//    {
//        StartGeneration();
//        InstantiateRooms();
//    }

//    private void StartGeneration()
//    {
//        while (transform.childCount > 0)
//        {
//            Destroy(transform.GetChild(0));
//        }
//        dungeonBoundary = roomsToGenerate * roomsToGenerate;
//        occupied = new bool[dungeonBoundary, dungeonBoundary];

//        int baseX = 50, baseY = 50;
//        Room currentRoom.aRoom = spawnRoom.DeepCopy();
//        currentRoom.aRoom.baseX = baseX;
//        currentRoom.aRoom.baseY = baseY;
//        rooms.Add(currentRoom.aRoom);

//        for (int i = 0; i < currentRoom.aRoom.sizeX; i++)
//        {
//            for (int j = 0; j < currentRoom.aRoom.sizeY; j++)
//            {
//                occupied[currentRoom.aRoom.baseX + i, currentRoom.aRoom.baseY + j] = true;
//            }
//        }

//        bool[] openedDoors = ExtensionMethods.ListOfBool(4, -1);

//        int tester = -1;
//        for (int i = 0; i < rooms.Count; i++)
//        {
//            if (currentRoom.aRoom == rooms[i])
//            {
//                tester = i;
//                break;
//            }
//        }

//        for (int i = 0; i < 4; i++)
//        {
//            if (openedDoors[i] == true)
//            {
//                Debug.Log("Gen called by " + tester);
//                //上下左右 0 1 2 3
//                GenerateRoom((i != 3) ? baseX : baseX + currentRoom.aRoom.sizeX, (i != 0) ? baseY : baseY + currentRoom.aRoom.sizeY, i);
//            }
//        }
//    }

//    //maybe return false if generate fail
//    private void GenerateRoom(int baseX, int baseY, int caseNumber)
//    {
//        int iteration = 0;
//        if (rooms.Count >= roomsToGenerate)
//            return;
//        //上下左右 0 1 2 3
//        while (iteration <= 10)
//        {
//            iteration++;
//            Room roomToTest.aRoom = PickARoom();
//            if (!CheckValid(roomToTest.aRoom, baseX, baseY, caseNumber))
//                continue;
//            roomToTest.aRoom.baseX = caseNumber != 2 ? baseX : baseX - roomToTest.aRoom.sizeX;
//            roomToTest.aRoom.baseY = caseNumber != 1 ? baseY : baseY - roomToTest.aRoom.sizeY;
//            //roomToTest.aRoom.baseX = caseNumber < 2 ? baseX : caseNumber==2 ? baseX - roomToTest.aRoom.sizeX: baseX + roomToTest.aRoom.sizeX;
//            //roomToTest.aRoom.baseY = caseNumber > 1 ? baseY : caseNumber==0 ? baseY - roomToTest.aRoom.sizeY: baseY + roomToTest.aRoom.sizeY;
//            Debug.Log(baseX + "  " + baseY);
//            Debug.Log("X" + roomToTest.aRoom.baseX);
//            Debug.Log("Y" + roomToTest.aRoom.baseY);
//            rooms.Add(roomToTest.aRoom);

//            /// Generate Rooms for the newly generated room
//            // caseNo 上下左右 0 1 2 3
//            // = from 下上右左 1 0 3 2
//            bool[] openedDoors = ExtensionMethods.ListOfBool(4, caseNumber % 2 == 0 ? caseNumber + 1 : caseNumber - 1);

//            int tester1 = -1;
//            for (int i = 0; i < rooms.Count; i++)
//            {
//                if (roomToTest.aRoom == rooms[i])
//                {
//                    tester1 = i;
//                    break;
//                }
//            }

//            for (int i = 0; i < 4; i++)
//            {
//                if (openedDoors[i] == true)
//                {
//                    Debug.Log("Gen called by " + tester1);
//                    GenerateRoom((i != 3) ? roomToTest.aRoom.baseX : roomToTest.aRoom.baseX + roomToTest.aRoom.sizeX, (i != 0) ? roomToTest.aRoom.baseY : roomToTest.aRoom.baseY + roomToTest.aRoom.sizeY, i);
//                }
//            }

//            return; //true;
//        }
//        return; //false;
//    }

//    private void InstantiateRooms()
//    {
//        for (int i = 0; i < rooms.Count; i++)
//        {
//            gos.Add(Instantiate(rooms[i].roomGO, new Vector3(rooms[i].baseX * 5, 0, rooms[i].baseY * 5), Quaternion.identity, transform));
//        }
//    }

//    private Room PickARoom()
//    {
//        int roomType = Random.Range(0, roomsDB.data.Length);
//        int roomNumber = Random.Range(0, roomsDB.data[roomType].rooms.Length);
//        return roomsDB.data[roomType].rooms[roomNumber].DeepCopy();
//    }

//    private bool CheckValid(Room roomToCheck, int currentX, int currentY, int caseNumber)
//    {
//        Debug.Log("CHECK");
//        Debug.Log(currentX + currentY + caseNumber);

//        if (currentX < 0 || currentY < 0 || currentX > roomsToGenerate * roomsToGenerate * 9 || currentY > roomsToGenerate * roomsToGenerate * 9)
//            return false;

//        //上下左右 0 1 2 3
//        switch (caseNumber)
//        {
//            case 0:
//            case 3:
//                {
//                    if (currentX + roomToCheck.sizeX > dungeonBoundary - 1 || currentY + roomToCheck.sizeY > dungeonBoundary - 1)
//                        return false;
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            if (occupied[currentX + i, currentY + j] == true)
//                                return false;
//                        }
//                    }
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            occupied[currentX + i, currentY + j] = true;
//                        }
//                    }
//                }
//                return true;
//            case 1:
//                {
//                    if (currentX + roomToCheck.sizeX > dungeonBoundary - 1 || currentY - roomToCheck.sizeY < 0)
//                        return false;
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            if (occupied[currentX + i, currentY - roomToCheck.sizeY + j] == true)
//                                return false;
//                        }
//                    }
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            occupied[currentX + i, currentY - roomToCheck.sizeY + j] = true;
//                        }
//                    }
//                }
//                return true;
//            case 2:
//                {
//                    if (currentX - roomToCheck.sizeX < 0 || currentY + roomToCheck.sizeY > dungeonBoundary - 1)
//                        return false;
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            if (occupied[currentX - roomToCheck.sizeX + i, currentY + j] == true)
//                                return false;
//                        }
//                    }
//                    for (int i = 0; i < roomToCheck.sizeX; i++)
//                    {
//                        for (int j = 0; j < roomToCheck.sizeY; j++)
//                        {
//                            occupied[currentX - roomToCheck.sizeX + i, currentY + j] = true;
//                        }
//                    }
//                }
//                return true;
//            default:
//                Debug.Log("FAILURE");
//                return false;
//        }
//    }

//    [System.Serializable]
//    public class Room
//    {
//        public int sizeX;
//        public int sizeY;
//        public int baseX;
//        public int baseY;
//        public Door[] doors;
//        public GameObject roomGO;

//        [System.Serializable]
//        public class Door
//        {
//            public int locationX;
//            public int locationY;
//        }

//        public Room DeepCopy()
//        {
//            Room copy = new Room();
//            copy.sizeX = sizeX;
//            copy.sizeY = sizeY;
//            copy.doors = doors;
//            copy.roomGO = roomGO;
//            return copy;
//        }
//    }

//    [System.Serializable]
//    public class RoomDataBase
//    {
//        public RoomData[] data;
//    }

//    [System.Serializable]
//    public class RoomData
//    {
//        public string type = "";
//        public Room[] rooms;
//    }
//}

