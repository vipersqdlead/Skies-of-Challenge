using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public string prefabsFolderPath;
    public Transform spawnParent;
    [HideInInspector] public AssetsitosType assetType;
    public Vector3 upDirection = Vector3.up;
    [HideInInspector] public string resourcesFolder;
}
