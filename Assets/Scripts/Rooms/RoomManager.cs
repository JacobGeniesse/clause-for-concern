using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    #region Singleton Setup
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
    #endregion

    [Tooltip("List of Rooms in the scene.")]
    [SerializeField] public List<Room> rooms = new List<Room>();

    //private Dictionary<int, Room> roomMap = new Dictionary<int, Room>(); //Dictonary of the rooms and their IDs

    void Start()
    {
        CreateMap(); //Define the room map
    }

    void CreateMap()
    {
        for(int i = 0; i < rooms.Count; i++)
        {
            //roomMap.Add(i, rooms[i]);

            for(int j = 0; j < rooms[i].adjacentRoomName.Length; j++)
            {
                GrabRoom(rooms[i], rooms[i].adjacentRoomName[j]); //Assign the room names for each room.
            }
        }
    }

    //Helper func for assigning references the adjacent rooms of a room
    void GrabRoom(Room currRoom, string roomName)
    {
        bool addedRoom = false; //Has a room been added
        for(int i = 0; i < rooms.Count; i++)
        {
            //If a matching room name has been found in the rooms array add it to the currRoom's adjacentRooms list
            if(roomName.ToUpper() == rooms[i].roomName.ToUpper())
            {
                currRoom.adjacentRooms.Add(rooms[i]);
                //Debug.Log($"Added {rooms[i]} to {currRoom}'s adjacent rooms."); //Debug message telling what room has been assigned to what room
                addedRoom = true; //Indicate a room was added
            }
        }

        //Debug message for if the search for a room fails
        if(addedRoom == false)
        {
            Debug.LogWarning($"Unable to find {roomName}!");
        }
    }

    //Function for other scripts to use to search through the rooms array to find a corresponding room
    public Room FindRoom(string roomName)
    {
        Room loadedRoom = null; //No room loaded
        for(int i = 0; i < rooms.Count; i++)
        {
            if(roomName.ToUpper() == rooms[i].roomName.ToUpper())
            {
                loadedRoom = rooms[i]; //If the room was found set loadedRoom equal to it
            }
        }

        // Debug message for if the script was unable to find the desired room
        if(loadedRoom == null)
        {
            Debug.LogWarning("Unable to find requested room!");
        }

        return loadedRoom; //Return the loaded room
    }
}
