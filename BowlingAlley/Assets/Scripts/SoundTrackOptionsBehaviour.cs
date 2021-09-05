using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrackOptionsBehaviour : MonoBehaviour
{
    private float refreshInterval = 0.001f;
    private float refreshPauseTimer;

    // Start is called before the first frame update
    void Start()
    {

     refreshPauseTimer = refreshInterval;

    }

    // Update is called once per frame
    void FixedUpdate()
    {

    //   activateProperCheckBox(LevelManager.getPlaySoundtrackMusic(), gameObject); 
       refreshPauseTimer -= Time.deltaTime;
        if (refreshPauseTimer <= 0f)
        {
            activateProperCheckBox(LevelManager.getPlaySoundtrackMusic(), gameObject); 
            refreshPauseTimer = refreshInterval;

        }

    }

    void activateProperCheckBox(bool checkboxValue, GameObject optionsDialog)
    {

        GameObject yesCheckBox = PrefabFactory.GetChildWithName(optionsDialog, "SoundCheckBoxOn");
        GameObject noCheckBox = PrefabFactory.GetChildWithName(optionsDialog, "SoundCheckBoxOff");

        if (checkboxValue == true)
        {
            setActive_withTryCatch(yesCheckBox, true);
            setActive_withTryCatch(noCheckBox, false);
        }
        else
        {
            setActive_withTryCatch(yesCheckBox, false);
            setActive_withTryCatch(noCheckBox, true);
        }

    }
    void setActive_withTryCatch(GameObject go, bool value)
    {
        //not sure why some of the non-null objects cause an exception when I turn active on or off. weird.
        try
        {
            go.SetActive(value);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e, this);
        }
    }

}
