using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectEffectSelection : Interactable
{
    private ParticleSystem particle;
    private AudioSource audioS;
    private Animator anim;

    private Coroutine corRespond;

    private void Awake()
    {
        audioS = GetComponent<AudioSource>();
        particle = transform.GetComponentInChildren<ParticleSystem>();
        anim = GetComponent<Animator>();

        StartCoroutine(WaitingCoroutine());
    }
    
    // requires a collider
    protected override void OnMouseDown()
    {
        Respond();
    }

    protected override void Respond()
    {
        RunVisualEffect();
        RunSoundEffect();
        RunAnimation();
    }

    private IEnumerator WaitingCoroutine() {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }
        
    private void RunSoundEffect() {
        if(!audioS.isPlaying){ 
            audioS.Play();
        }
    }

    // Start Visual Effect
    private void RunVisualEffect() {

        if(particle.isStopped){ 
            particle.Play();
        }
    }

    // Start Animation
    private void RunAnimation() {
        anim.Play(0);
    }
}
