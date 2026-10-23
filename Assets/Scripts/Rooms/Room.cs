using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Room
{
    public string roomName;

    public GameObject roomOrigin;

    [SerializeReference] public string[] adjacentRoomName;

    [SerializeReference, HideInInspector] public List<Room> adjacentRooms = new List<Room>();

    public override string ToString()
    {
        return roomName;
    }
}
