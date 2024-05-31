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

[System.Serializable]
public struct ActionList : IJsonObject
{
    public const string LIB_FOLDER_NAME = "ActionLists";
    public const string LIB_FILE_NAME = "ActionListLibrary";

    public string JsonID => ListId;
    public string ListId;

    public string[] Actions;
}
