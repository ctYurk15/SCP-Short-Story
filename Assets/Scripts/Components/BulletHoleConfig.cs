using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleConfig : MonoBehaviour
{
    [SerializeField] private string element_material;

    public string ElementMaterial
    {
        get => element_material;
    }
}
