using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class Revealer : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _radius;

    void Update()
    {
        if (_player != null)
        {
            Shader.SetGlobalVector("_Revealer_Position", _player.position);
            Shader.SetGlobalFloat("_Radius", _radius);
        }
    }
}
