using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
    const string VERSION = "v1";
    const string ROOM_NAME = "mainRoom"; //eventaully lobby
	// Use this for initialization
	void Start () {
        //PhotonNetwork.ConnectUsingSettings(VERSION); //assume this joins lobby?
	}

    void OnJoinedLobby()
    {
        //RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 4 };
        //PhotonNetwork.JoinOrCreateRoom(ROOM_NAME, roomOptions, TypedLobby.Default);
    }

    void OnJoinedRoomed()
    {


    }
	
}
