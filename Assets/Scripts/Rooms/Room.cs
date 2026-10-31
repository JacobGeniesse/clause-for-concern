using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Room
{
    [Tooltip("Name of the Room.")]
    public string roomName;

    [Tooltip("Game Object group this Room is tied to.")]
    public GameObject roomOrigin;

    [Tooltip("Names of the Rooms adjacent to this one.")]
    [SerializeReference] public string[] adjacentRoomName;

    //Actual reference to the neighboring rooms to this one
    [SerializeReference, HideInInspector] public List<Room> adjacentRooms = new List<Room>();

    //Override to make a room printed to string be its room name
    public override string ToString()
    {
        return roomName;
    }
}
