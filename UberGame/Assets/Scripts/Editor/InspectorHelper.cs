using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;


[CustomEditor(typeof(CommonReferences))]
public class RestaurantHelper : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CommonReferences _CRSomething = (CommonReferences)target;

        if (GUILayout.Button("Spawn Client"))
        {
            _CRSomething.SpawnClient();
        }
    }
}


