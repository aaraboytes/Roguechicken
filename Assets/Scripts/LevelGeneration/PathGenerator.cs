using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { top,down,right,left};
public class PathGenerator : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField] int maxRooms;
    int currentRooms = 0;
    [SerializeField]List<Vector2> generatedPositions = new List<Vector2>();
    List<GameObject> rooms = new List<GameObject>();
    List<Vector2> corners = new List<Vector2>();
    Direction pastDir;
    Direction nextDir = Direction.top;
    int index = 0;
    Vector3 originPos;
    LineRenderer debugLine;
    GameObject map;

    //Indicators
    [SerializeField] GameObject boss;
    [SerializeField] GameObject chest;
    [SerializeField] GameObject shop;

    //Templates
    public GameObject startRoom;
    public GameObject[] TDRooms;
    public GameObject[] TRRooms;
    public GameObject[] TLRooms;
    public GameObject[] BLRooms;
    public GameObject[] BRRooms;
    public GameObject[] RLRooms;

    //Dead ends
    public GameObject TRoom;
    public GameObject BRoom;
    public GameObject LRoom;
    public GameObject RRoom;

    [SerializeField]int tWeight, dWeight, rWeight, lWeight;
    Direction currentBranch = Direction.top;
    bool endGeneration = false;

    private void Start()
    {
        map = new GameObject("Map");
        originPos = transform.position;
        CalculateWeights();

        GenerateTrack();
        debugLine = GetComponent<LineRenderer>();
        debugLine.positionCount = maxRooms;
    }
    void CalculateWeights()
    {
        int balanced = maxRooms / 4;
        tWeight = dWeight = rWeight = lWeight = balanced;
        int remover = Random.Range(1,balanced);
        int stored = remover;
        tWeight -= remover;
        remover = Random.Range(0, balanced);
        stored += remover;
        dWeight -= remover;
        remover = Random.Range(0, balanced);
        stored += remover;
        rWeight -= remover;
        remover = Random.Range(0, balanced);
        stored += remover;
        lWeight -= remover;
        print("Stored rooms " + stored);
        int restore = Random.Range(0, stored);
        stored -= restore;
        tWeight += restore;
        restore = Random.Range(0, stored);
        stored -= restore;
        dWeight += restore;
        restore = Random.Range(0, stored);
        stored -= restore;
        rWeight += restore;
        lWeight += stored;
        print("Stored rooms " + stored);
    }
    void GenerateTrack()
    {
        GameObject start = Instantiate(startRoom, map.transform);
        start.transform.position = transform.position;
        generatedPositions.Add(transform.position);
        InstantiateRoomStarter();
        StartCoroutine(IEGeneratePath());
    }
    IEnumerator IEGeneratePath()
    {
        while (!endGeneration)
        {
            print("Generating branch " + currentBranch.ToString() + " : " + SpacesLeftInBranch() + " left");
            pastDir = nextDir;
            Vector2 newPos = GetNewPos(transform.position, pastDir);
            transform.position = newPos;
            
            bool emptySlot = false;
            bool branchCorrect = true;
            int mistakes = 0;
            while (!emptySlot)
            {
                List<Direction> possibilities = new List<Direction>();
                Direction currentDir = Direction.top;
                for(int i = 0; i < 4; i++)
                {
                    if (!generatedPositions.Contains(GetNewPos(transform.position, currentDir)))
                        possibilities.Add(currentDir);
                    currentDir++;
                }
                print(possibilities);
                //Select one of the possibilites
                if (possibilities.Count > 0)
                {
                    nextDir = possibilities[Random.Range(0, possibilities.Count)];
                    emptySlot = true;
                }
                else
                {
                    branchCorrect = false;
                    break;
                }
                yield return null;
            }
            if (branchCorrect)
            {
                InstantiateRoom();
            }
            else
            {
                InstantiateCorner();
            }
            generatedPositions.Add(newPos);
            currentRooms++;
            yield return null;
        }
        SetObjectives();
        SetLine();
    }

    #region Instantiates
    void InstantiateRoomStarter()
    {
        GameObject newRoom = null;
        nextDir = currentBranch;
        pastDir = nextDir;
        transform.position = GetNewPos(transform.position, pastDir);
        if (!generatedPositions.Contains(transform.position)) {
            InstantiateRoom();
            generatedPositions.Add(transform.position);
            DecreaseInCurrentBranch();
        }
        else
        {
            if (NextBranch())
            {
                transform.position = originPos;
                nextDir = currentBranch;
                pastDir = OppositeDir(nextDir);
                InstantiateRoomStarter();
            }
            else
            {
                endGeneration = true;
            }
        }
    }
    void InstantiateCorner()
    {
        if (generatedPositions.Contains(transform.position))
            return;
        GameObject newRoom = GetCorner(pastDir);
        newRoom = Instantiate(newRoom,map.transform);
        newRoom.transform.position = transform.position;
        rooms.Add(newRoom);
        corners.Add(transform.position);
        if (NextBranch())
        {
            transform.position = originPos;
            nextDir = currentBranch;
            pastDir = OppositeDir(nextDir);
            InstantiateRoomStarter();
        }
        else
        {
            endGeneration = true;
        }
    }
    void InstantiateRoom()
    {
        GameObject newRoom = GetRoom();
        newRoom = Instantiate(newRoom,map.transform);
        newRoom.transform.position = transform.position;
        rooms.Add(newRoom);
        DecreaseInCurrentBranch();
        if (SpacesLeftInBranch() <= 0)
        {
            if (NextBranch())
            {
                transform.position = originPos;
                nextDir = currentBranch;
                pastDir = OppositeDir(nextDir);
                InstantiateRoomStarter();
            }
            else
            {
                endGeneration = true;
            }
        }
    }
    #endregion
    #region Get room
    GameObject GetCorner(Direction dir)
    {
        GameObject corner = null;
        dir = OppositeDir(dir);
        if (dir == Direction.top)
            corner = TRoom;
        else if (dir == Direction.down)
            corner = BRoom;
        else if (dir == Direction.right)
            corner = RRoom;
        else if (dir == Direction.left)
            corner = LRoom;
        return corner;
    }
    GameObject GetRoom()
    {
        GameObject newRoom = null;
        if (SpacesLeftInBranch() > 1)
        {
            if (pastDir == Direction.top && nextDir == Direction.top || pastDir == Direction.down && nextDir == Direction.down)
                newRoom = TDRooms[Random.Range(0, TDRooms.Length)];
            else if (pastDir == Direction.top && nextDir == Direction.right || pastDir == Direction.left && nextDir == Direction.down)
                newRoom = BRRooms[Random.Range(0, BRRooms.Length)];
            else if (pastDir == Direction.top && nextDir == Direction.left || pastDir == Direction.right && nextDir == Direction.down)
                newRoom = BLRooms[Random.Range(0, BLRooms.Length)];
            else if (pastDir == Direction.down && nextDir == Direction.right || pastDir == Direction.left && nextDir == Direction.top)
                newRoom = TRRooms[Random.Range(0, TRRooms.Length)];
            else if (pastDir == Direction.down && nextDir == Direction.left || pastDir == Direction.right && nextDir == Direction.top)
                newRoom = TLRooms[Random.Range(0, TLRooms.Length)];
            else if (pastDir == Direction.right && nextDir == Direction.right || pastDir == Direction.left && nextDir == Direction.left)
                newRoom = RLRooms[Random.Range(0, RLRooms.Length)];
        }
        else
        {
            newRoom = GetCorner(pastDir);
            corners.Add(transform.position);
        }
        print("Room selected " + pastDir.ToString() + " + " + nextDir.ToString() + " = " + newRoom);
        return newRoom;
    }
    #endregion
    #region Directions
    Direction GetRandomDir()
    {
        int rand = Random.Range(0, 5);
        if (rand == 0)
            return Direction.top;
        else if (rand == 1)
            return Direction.down;
        else if (rand == 2)
            return Direction.right;
        else
            return Direction.left;
    }
    Direction OppositeDir(Direction dir)
    {
        if (dir == Direction.top)
            return Direction.down;
        else if (dir == Direction.down)
            return Direction.top;
        else if (dir == Direction.right)
            return Direction.left;
        else
            return Direction.right;
    }
    #endregion
    Vector2 GetNewPos(Vector2 pos, Direction nextPos)
    {
        if (nextPos == Direction.top)
            return pos + Vector2.up * distance;
        else if (nextPos == Direction.down)
            return pos + Vector2.down * distance;
        else if (nextPos == Direction.right)
            return pos + Vector2.right * distance;
        else
            return pos + Vector2.left * distance;
    }
    #region Branch
    void DecreaseInCurrentBranch()
    {
        switch (currentBranch)
        {
            case Direction.top:
                tWeight--;
                break;
            case Direction.down:
                dWeight--;
                break;
            case Direction.right:
                rWeight--;
                break;
            case Direction.left:
                lWeight--;
                break;
        }
    }
    int SpacesLeftInBranch()
    {
        switch (currentBranch)
        {
            case Direction.top:
                return tWeight;
                break;
            case Direction.down:
                return dWeight;
                break;
            case Direction.right:
                return rWeight;
                break;
            case Direction.left:
                return lWeight;
                break;
        }
        return 0;
    }
    bool NextBranch()
    {
        BalanceWeights();
        if (currentBranch == Direction.left)
            return false;
        else
        {
            currentBranch++;
            return true;
        }
    }
    #endregion
    void SetLine()
    {
        for(int i = 0; i < generatedPositions.Count; i++)
        {
            debugLine.SetPosition(i, generatedPositions[i]);
        }
    }
    void SetObjectives()
    {
        StartCoroutine(IESelectObjectives());
    }
    IEnumerator IESelectObjectives()
    {
        Vector2 bossPos;
        Vector2 chestPos = Vector2.zero;
        Vector2 shopPos = Vector2.zero;
        bossPos = corners[Random.Range(0, corners.Count)];
        chestPos = bossPos;
        if (corners.Count > 0)
        {
            while (bossPos == chestPos)
            {
                chestPos = corners[Random.Range(0, corners.Count)];
                yield return null;
            }
            if (corners.Count > 1)
            {
                shopPos = chestPos;
                if (corners.Count > 2)
                {
                    while (shopPos == bossPos || shopPos == chestPos)
                    {
                        shopPos = corners[Random.Range(0, corners.Count)];
                        yield return null;
                    }
                    Instantiate(shop, shopPos, Quaternion.identity);
                }
                Instantiate(chest, chestPos, Quaternion.identity);
            }
            Instantiate(boss, bossPos, Quaternion.identity);
        }
        FindObjectOfType<AstarPath>().Scan();
    }
    void BalanceWeights()
    {
        switch (currentBranch)
        {
            case Direction.top:
                dWeight += tWeight;
                tWeight = 0;
                break;
            case Direction.down:
                rWeight += dWeight;
                dWeight = 0;
                break;
            case Direction.right:
                lWeight += rWeight;
                rWeight = 0;
                break;
        }
    }
}
