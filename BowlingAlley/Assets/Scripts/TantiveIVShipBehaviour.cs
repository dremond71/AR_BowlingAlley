using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TantiveIVShipBehaviour : MonoBehaviour {

    private Animator anim;
    bool performingLoop = false;

    private AudioSource exitHyperSpaceSource1;
    private AudioClip exitHyperspace;

    private void Awake () {
        anim = GetComponent<Animator> ();
        anim.speed = 0.03f; // play loop animation really slow
        exitHyperSpaceSource1 = GameObject.FindGameObjectWithTag ("exitHyperspace_Sound").GetComponent<AudioSource> ();
        exitHyperspace = exitHyperSpaceSource1.clip;

    }
    // Start is called before the first frame update
    void Start () {
        PlayExitHyperspaceSound_Immediately ();
        anim.SetBool ("shouldPerformLoop", true);
    }

    // Update is called once per frame
    void Update () {

    }

    void PlayExitHyperspaceSound_Immediately () {
        // we want a seperate object for each fire; so we can handle multi blasts in quick succession

        // Method I used to achieve multiple blasts that don't interrupt each other:
        // https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html
        exitHyperSpaceSource1.PlayOneShot (exitHyperspace, 2.0f);
    }
}