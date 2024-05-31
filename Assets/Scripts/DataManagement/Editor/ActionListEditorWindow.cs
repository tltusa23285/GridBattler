using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ActionListEditorWindow : JsonLibraryEditorWindow<ActionList>
{
    [MenuItem("Game/Action List Editor")]
    public static void ShowWindow()
    {
        ActionListEditorWindow window = GetWindow<ActionListEditorWindow>();
        window.Setup();
        window.Show();
    }

    [SerializeField] public ActionList CurrentList;
    protected override ActionList CurrentItem
    {
        get { return CurrentList; }
        set { CurrentList = value; }
    }

    protected override string CurrentItemClassLabel => "ActionList";
    protected override string LibraryFolder => ActionList.LIB_FOLDER_NAME;
    protected override string LibraryFile => ActionList.LIB_FILE_NAME;

    protected override string[] GetAddItems()
    {
        return new string[] { "ActionList" };
    }

    protected override void SetCurrentFromType(string type)
    {
        SetCurrent(new ActionList());
    }

    protected override void SetSerialized(ref SerializedObject obj, ref SerializedProperty prop)
    {
        obj = new SerializedObject(this);
        prop = ThisObj.FindProperty(nameof(CurrentList));
    }
}
