using UnityEngine;
using System;

// See  https://docs.unity3d.com/ScriptReference/Resources.Load.html
//      https://medium.com/@thescottcabs/instantiating-monobehaviours-in-unity-d2d092f29f55 
public static class PrefabFactory
{
    
  public static GameObject getPrefab(string prefabName) {
     GameObject prefab = null;

     try {
      
        // e.g. Load an AudioClip (Assets/Resources/Audio/audioClip01.mp3)
        //       var audioClip = Resources.Load<AudioClip>("Audio/audioClip01");
      
        prefab = Resources.Load<GameObject>("Prefabs/" + prefabName);

     }
     catch(Exception e){
         Debug.Log(e);
     }
     

     return prefab;
  }


   public static GameObject GetChildWithName(GameObject obj, string name) {
     Transform trans = obj.transform;
     Transform childTrans = trans.Find(name);
     if (childTrans != null) {
         return childTrans.gameObject;
     } else {
         return null;
     }
 }

}