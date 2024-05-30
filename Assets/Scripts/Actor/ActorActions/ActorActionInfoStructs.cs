using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Display information related to an action, such as UI elements
/// </summary>
[System.Serializable]
public struct ActionInfoData
{
    public string Name;
    public string Image;
    public string Thumbnail;
    public int Damage;
    [TextArea(2,4)]public string Description;
}
