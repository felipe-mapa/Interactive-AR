using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fovAssistant : MonoBehaviour
{
    private void OnEnable() {
        Events.OnTrackingFound += OnTrackingFound;
    }

    private void OnDisable() {
        Events.OnTrackingFound -= OnTrackingFound;
    }

    void OnTrackingFound()
    {
        GetComponent<Camera>().fieldOfView = Camera.main.fieldOfView;
    }
}
