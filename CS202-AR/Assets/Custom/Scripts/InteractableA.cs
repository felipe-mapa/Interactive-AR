using System.Collections;
using UnityEngine;
    
public class InteractableA : Interactable {
    public AnimationCurve valueOverTime;
    
    private Vector3 initialScale;
    private Coroutine corResize;
    
    private void Awake() {
        initialScale = transform.localScale;
    }
    
    protected override void OnMouseDown() {
        Respond();
    }
    
    protected override void Respond() {
        if (corResize == null) {
            corResize = StartCoroutine(Resize());
        }
    }
    
    private IEnumerator Resize() {
        float t = 0;
        float duration = 0.5f;
        Vector3 growthScale = initialScale * 2;
    
        while (t < duration) {
            transform.localScale = Vector3.Lerp(initialScale, growthScale, valueOverTime.Evaluate(t / duration));
    
            t += Time.deltaTime;
            yield return null;
        }
    
        transform.localScale = initialScale;
        corResize = null;
    }
}