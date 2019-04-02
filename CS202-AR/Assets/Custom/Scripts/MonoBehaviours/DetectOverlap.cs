using UnityEngine;
using System.Collections.Generic;
    
public class DetectOverlap : MonoBehaviour {
    public static List<Collider> overlaps = new List<Collider>();
    
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Interactive") {
            overlaps.Add(other);
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Interactive") {
            overlaps.Remove(other);
        }
    }
}