using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTrackableEventHandler : DefaultTrackableEventHandler
{

    protected override void OnTrackingFound(){
        base.OnTrackingFound();
        
    }
    protected override void OnTrackingLost(){
        base.OnTrackingLost();
        
        foreach (Transform t in transform) {
            if (t.gameObject.tag == "Forklift/Body")
            {
                t.gameObject.AddComponent<Forklift>();
            }
        }
    }
}
