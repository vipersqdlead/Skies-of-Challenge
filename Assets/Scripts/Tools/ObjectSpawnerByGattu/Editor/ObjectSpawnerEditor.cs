using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectSpawner))]
public class ObjectSpawnerEditor : Editor
{
    private GameObject visualReference;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ObjectSpawner t = target as ObjectSpawner;

        // Mostrar campo para la carpeta de prefabs
        EditorGUILayout.LabelField("Prefab Folder Path", t.prefabsFolderPath);
        if (GUILayout.Button("Select Prefab Folder"))
        {
            string folderPath = EditorUtility.OpenFolderPanel("Select Prefab Folder", "", "");
            if (!string.IsNullOrEmpty(folderPath))
            {
                // Convertir la ruta a relativa al directorio de Assets
                if (folderPath.StartsWith(Application.dataPath))
                {
                    t.prefabsFolderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);
                    GenerateEnum(t.prefabsFolderPath);
                }
                else
                {
                    Debug.LogError("Folder must be inside the Assets folder.");
                }
            }
        }

        //if (GUILayout.Button("Refresh Enums"))
        //{
        //    GenerateEnum(t.prefabsFolderPath);
        //}

        // Detectar cambios en el enum AssetsitosType
        EditorGUI.BeginChangeCheck();
        AssetsitosType newAssetType = (AssetsitosType)EditorGUILayout.EnumPopup("Asset Type", t.assetType);
        if (EditorGUI.EndChangeCheck())
        {
            t.assetType = newAssetType;
            UpdateVisualReference(t);
        }
    }

    void GenerateEnum(string folderPath)
    {
        string[] prefabPaths = Directory.GetFiles(folderPath, "*.prefab");
        List<string> prefabNames = new List<string>();

        foreach (string prefabPath in prefabPaths)
        {
            string prefabName = Path.GetFileNameWithoutExtension(prefabPath);
            prefabNames.Add(prefabName);
        }

        EnumGen enumGen = new EnumGen();
        enumGen.CreateEnum(prefabNames, "AssetsitosType", "Scripts/Tools/ObjectSpawnerByGattu", false);
        
        AssetDatabase.Refresh();
        
        ObjectSpawner t = target as ObjectSpawner;
        t.prefabsFolderPath = folderPath;
    }


    void OnSceneGUI()
    {
        ObjectSpawner t = target as ObjectSpawner;

        // Obtener la posición del mouse y lanzar un rayo
        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Mover visualReference a la posición del hit
            MoveVisualReference(hit.point, hit.normal, t.upDirection);

            // Manejar el evento de clic del mouse
            if (e.type == EventType.MouseDown && e.button == 0) // Click izquierdo del mouse
            {
                SpawnObjectAtPoint(t, hit.point, hit.normal, t.upDirection);
                e.Use(); // Prevenir que otros elementos en la escena respondan al evento de clic
            }
        }

        // Repaint la vista de la escena para asegurar que se actualice el dibujo del visualReference
        SceneView.RepaintAll();
    }

    private void UpdateVisualReference(ObjectSpawner spawner)
    {
        // Destruir el visualReference anterior si existe
        if (visualReference != null)
        {
            DestroyImmediate(visualReference);
        }

        // Obtener la ruta completa del prefab
        string prefabPath = Path.Combine(spawner.prefabsFolderPath, spawner.assetType.ToString() + ".prefab");

        // Cargar el prefab desde la ruta especificada
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            visualReference = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            visualReference.name = "VisualReference";
            visualReference.hideFlags = HideFlags.HideAndDontSave;
        }
        else
        {
            Debug.LogError("Prefab not found at path: " + prefabPath);
        }
    }

    private void MoveVisualReference(Vector3 position, Vector3 normal, Vector3 upDirection)
    {
        if (visualReference != null)
        {
            visualReference.transform.position = position;

            // Verificar si la normal no es un vector cero antes de ajustar la rotación
            if (normal != Vector3.zero)
            {
                visualReference.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(upDirection, normal), normal);
            }
        }
    }

    private void SpawnObjectAtPoint(ObjectSpawner spawner, Vector3 point, Vector3 normal, Vector3 upDirection)
    {
        // Obtener la ruta completa del prefab
        string prefabPath = Path.Combine(spawner.prefabsFolderPath, spawner.assetType.ToString() + ".prefab");

        // Cargar el prefab desde la ruta especificada
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            obj.transform.position = point;

            // Verificar si la normal no es un vector cero antes de ajustar la rotación
            if (normal != Vector3.zero)
            {
                obj.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(upDirection, normal), normal);
            }

            obj.transform.SetParent(spawner.spawnParent);

            Undo.RegisterCreatedObjectUndo(obj, "Spawn Object");
            Selection.activeGameObject = obj;
        }
        else
        {
            Debug.LogError("Prefab not found at path: " + prefabPath);
        }
    }
}
