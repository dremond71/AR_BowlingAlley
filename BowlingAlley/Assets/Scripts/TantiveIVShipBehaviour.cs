using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TantiveIVShipBehaviour : MonoBehaviour
{

    private Animator anim;
    bool performingLoop = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.speed = 0.03f;
    }
    // Start is called before the first frame update
    void Start()
    {
        anim.SetBool("shouldPerformLoop", true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}