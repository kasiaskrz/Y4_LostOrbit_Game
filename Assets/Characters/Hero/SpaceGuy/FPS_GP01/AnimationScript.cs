using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwesomeAnimationScript : MonoBehaviour
{
    public Animator anim1;
    public Animator anim2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim1.SetTrigger("Shoot");
            anim2.SetTrigger("Shoot");
        }
    }
}
