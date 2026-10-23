using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple RoomManager instances active!");
            Destroy(this);
        }
    }

    [SerializeField] public List<Room> rooms = new List<Room>();

    private Dictionary<int, Room> roomMap = new Dictionary<int, Room>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateMap()
    {
        for(int i = 0; i < rooms.Count; i++)
        {
            roomMap.Add(i, rooms[i]);

            for(int j = 0; j < rooms[i].adjacentRoomName.Length; j++)
            {
                GrabRoom(rooms[i], rooms[i].adjacentRoomName[j]);
            }
        }
    }

    void GrabRoom(Room currRoom, string roomName)
    {
        bool addedRoom = false;
        //Take name of room
        //Search through list of rooms referencing room name
        for(int i = 0; i < rooms.Count; i++)
        {
            if(roomName.ToUpper() == rooms[i].roomName.ToUpper())
            {
                currRoom.adjacentRooms.Add(rooms[i]);
                //Debug.Log($"Added {rooms[i]} to {currRoom}'s adjacent rooms.");
                addedRoom = true;
            }
        }

        if(addedRoom == false)
        {
            Debug.LogWarning($"Unable to find {roomName}!");
        }
        //If the two match add the room class to the hidden room class array
    }

    public Room FindRoom(string roomName)
    {
        Room loadedRoom = null;
        for(int i = 0; i < rooms.Count; i++)
        {
            if(roomName.ToUpper() == rooms[i].roomName.ToUpper())
            {
                loadedRoom = rooms[i];
            }
        }

        if(loadedRoom == null)
        {
            Debug.LogWarning("Unable to find requested room!");
        }

        return loadedRoom;
    }
}
