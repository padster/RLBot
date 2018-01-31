using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

/**
 * Script that runs during the game, pulls state from botServer, and updates the UI.
 */
public class listener : MonoBehaviour {
    // Networking constants
    private static int BUFF_LEN = 2048; // 2kb, just in case the network is backed up? :/
    private static int BYTES_10 = 8; // 2 ints (8 bytes) of boundary 1010...
    private static int EXPECTED_PACKET_BYTES = // Make sure this matches up botServer.py
        BYTES_10
        + 3 * 4 // ball x/y/z
        + 3 * 4 + 3 * 4 // your x/y/z pitch/yaw/roll
        + 3 * 4 + 3 * 4; // bot x/y/z pitch/yaw/roll
    
    // Connection, and thing to write to.
    private NetworkStream clientStreamReader;
    private byte[] buffer = new byte[BUFF_LEN];

    // Game objects to move around.
    public GameObject ball;
    public GameObject playerCar;
    public GameObject botCar;

    // Called at the start, do a blocking connect to the server.
	void Start () {
        StartCoroutine(LoadCardboardAsync());

        string serverIP = "localhost";
        int serverPort = 3451;
        if (PlayerPrefs.HasKey(launcher.KEY_IP)) {
            serverIP = PlayerPrefs.GetString(launcher.KEY_IP);
        }
        if (PlayerPrefs.HasKey(launcher.KEY_PORT)) {
            serverPort = PlayerPrefs.GetInt(launcher.KEY_PORT);
        }

        try {
            Debug.Log("RLVR Connecting to " + serverIP + ":" + serverPort + "...");
            // TODO: UDP instead?
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(serverIP, serverPort);
            clientStreamReader = tcpClient.GetStream();
            Debug.Log("RLVR Connected!");
        } catch (System.Exception e) {
            Debug.Log(e.Message);
            Debug.Log("RLVR Failed :(");
        }
	}
    
    // Load the cardboard code and turn in on when done.
    IEnumerator LoadCardboardAsync() {
        XRSettings.LoadDeviceByName("cardboard");
        yield return null;
        XRSettings.enabled = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (clientStreamReader == null) {
            return;
        }

        // Read a packet of network traffic, and find the most recent full update.
        int nRead = clientStreamReader.Read(buffer, 0, BUFF_LEN);
        int at = findLastPacketStart(buffer, nRead);
        if (at == -1) {
            Debug.Log("Whoops, couldn't find valid packet boundary :(");
        }

        // Parse the values back out of the update...
        Vector3  ballPos = readLocation(buffer, at); at += 12;
        Vector3    p0Pos = readLocation(buffer, at); at += 12;
        Quaternion p0Rot = readRotation(buffer, at); at += 12;
        Vector3    p1Pos = readLocation(buffer, at); at += 12;
        Quaternion p1Rot = readRotation(buffer, at); at += 12;

        // ...and update the world
        ball.transform.position = ballPos;
        playerCar.transform.SetPositionAndRotation(p0Pos, p0Rot);
        botCar.transform.SetPositionAndRotation(p1Pos, p1Rot);
	}

    // Read an X/Y/Z from the server, convert to Unity space.
    Vector3 readLocation(byte[] buffer, int at) {
        float x = System.BitConverter.ToSingle(buffer, at + 0);
        float y = System.BitConverter.ToSingle(buffer, at + 4);
        float z = System.BitConverter.ToSingle(buffer, at + 8);
        return new Vector3(x, z, -y) / 1000.0f;
    }

    // Read Pitch/Yaw/Roll from the server, convert to a Unity rotation.
    Quaternion readRotation(byte[] buffer, int at) {
        int pitch = System.BitConverter.ToInt32(buffer, at + 0);
        int yaw   = System.BitConverter.ToInt32(buffer, at + 4);
        int roll  = System.BitConverter.ToInt32(buffer, at + 8);
        Vector3 rotation = new Vector3(-roll, yaw, pitch) * (180.0f / 32768.0f);
        // TODO: fix - this version is wrong on the walls.
        return Quaternion.Euler(rotation);
    }

    // Given a buffer of data, walk backwards from the end until we find the boundary marker.
    int findLastPacketStart(byte[] buffer, int nRead){
        int at = 0, inARow = 0;
        for (at = nRead - (EXPECTED_PACKET_BYTES - BYTES_10) - 1; at >= 0 && inARow < BYTES_10; at--) {
            inARow = (buffer[at] == 85 ? inARow + 1 : 0);
        }
        if (at == -1 && inARow < BYTES_10) {
            Debug.Log("Whoops, couldn't find packet boundary :(");
            return -1;
        }
        at++;

        int boundaryCount = 0;
        for (int i = 0; i < BYTES_10 && at + 1 < nRead; i++, at++) {
            boundaryCount += (buffer[at] == 85 ? 1 : 0);
        }
        Debug.Assert(boundaryCount == BYTES_10, "Boundary found but not? what?");
        return at;
    }

}
