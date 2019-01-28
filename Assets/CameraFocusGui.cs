using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocusGui : MonoBehaviour
{
    public Camera MainCamera;
    public GameObject White;
    public GameObject Red;
    public GameObject Green;
    public GameObject Blue;

    private bool val0 = true;
    private bool val1 = true;
    private bool val2 = false;
    private bool val3 = false;

    private void OnGUI()
    {
        val0 = GUI.Toggle(new Rect(Screen.width - 100, 10, 90, 20), val0, "White");
        val1 = GUI.Toggle(new Rect(Screen.width - 100, 30, 90, 20), val1, "Red");
        val2 = GUI.Toggle(new Rect(Screen.width - 100, 50, 90, 20), val2, "Blue");
        val3 = GUI.Toggle(new Rect(Screen.width - 100, 70, 90, 20), val3, "Green");
        var btn = GUI.Button(new Rect(Screen.width - 100, 90, 90, 20), "Focus");
        if (btn)
        {
            var targets = new List<GameObject>();
            if (val0) targets.Add(White);
            if (val1) targets.Add(Red);
            if (val2) targets.Add(Blue);
            if (val3) targets.Add(Green);
            if (targets.Count == 0) return;

            this.GetComponent<CameraFocus>().Focus(MainCamera, targets);
        }
    }
}
