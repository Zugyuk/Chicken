using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassAnimScript : MonoBehaviour
{
    private Animation anim;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play(0, -1, Random.value);
    }

}
