using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectEffectSelection : Interactable
{
    public Animator anim;
    
    private ParticleSystem particle;
    private AudioSource audioS;

    private Coroutine corRespond;

    private void Awake()
    {
        audioS = GetComponent<AudioSource>();
        particle = transform.GetComponentInChildren<ParticleSystem>();

        StartCoroutine(WaitingCoroutine());
    }
    
    // requires a collider
    protected override void OnMouseDown()
    {
        Respond();
    }

    protected override void Respond()
    {
        if (!audioS.isPlaying && particle.isStopped && particle.isStopped){
            RunVisualEffect();
            RunSoundEffect();
            RunAnimation();
        }
    }

    private IEnumerator WaitingCoroutine() {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }
        
    private void RunSoundEffect() {
        audioS.Play();
    }

    // Start Visual Effect
    private void RunVisualEffect() {
        particle.Play();
    }

    // Start Animation
    private void RunAnimation() {
        // anim.Play(0);
        anim.SetTrigger("Activate");
    }
}
