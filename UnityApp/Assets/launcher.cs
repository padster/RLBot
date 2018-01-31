using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class launcher : MonoBehaviour {
    public static readonly string KEY_IP = "serverIP";
    public static readonly string KEY_PORT = "serverPort";

    public InputField ipInput;
    public InputField portInput;

	// Ignore
    void Update() {}

    // Called at start, load last value from prefs
    void Start() {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        if (PlayerPrefs.HasKey(KEY_IP)) {
            ipInput.text = PlayerPrefs.GetString(KEY_IP);
        }
        if (PlayerPrefs.HasKey(KEY_PORT)) {
            portInput.text = PlayerPrefs.GetInt(KEY_PORT).ToString();
        }
    }

    // Called on button click
    public void OnLaunch() {
        PlayerPrefs.SetString(KEY_IP, ipInput.text);
        PlayerPrefs.SetInt(KEY_PORT, int.Parse(portInput.text));
        SceneManager.LoadScene("GameScene");
    }
}
