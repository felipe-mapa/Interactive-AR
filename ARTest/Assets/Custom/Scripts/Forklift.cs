using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forklift : MonoBehaviour{
    private Animator animator;

    void Awake(){
        animator = GetComponent<Animator>();
    }

    void OnMouseDown(){
        StartCoroutine(Respond());
    }

    private IEnumerator Respond(){
        animator.SetTrigger("Activate");

        yield return null;
    }
}
