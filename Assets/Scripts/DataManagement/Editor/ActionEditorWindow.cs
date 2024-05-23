using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using WAction = IJsonObjectWrapper<ActorAction>;

public class ActionEditorWindow : JsonLibraryEditorWindow<WAction>
{
    [MenuItem("Game/Action Editor")]
    public static void ShowWindow()
    {
        ActionEditorWindow window = GetWindow<ActionEditorWindow>();
        window.Setup();
        window.Show();
    }

    [SerializeReference] public ActorAction CurrentAction;
    private WAction ActiveAction;
    protected override WAction CurrentItem
    {
        get
        {
            return ActiveAction;
        }
        set
        {
            ActiveAction = value;
            CurrentAction = ActiveAction.Object;
        }
    }

    protected override string LibraryFolder => ActorAction.LIB_FOLDER_NAME;
    protected override string LibraryFile => ActorAction.LIB_FILE_NAME;

    protected override string[] GetAddItems()
    {
        List<string> result = new List<string>();
        foreach (var i in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(ActorAction))))
        {
            result.Add(i.ToString());
        }
        result.Sort();
        return result.ToArray();
    }

    protected override void SetCurrentFromType(string type)
    {
        foreach (var i in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.ToString() == type))
        {
            SetCurrent(new WAction(Activator.CreateInstance(i) as ActorAction));
            return;
        }
    }
    protected override void SetSerialized(ref SerializedObject obj, ref SerializedProperty prop)
    {
        obj = new SerializedObject(this);
        prop = ThisObj.FindProperty(nameof(CurrentAction));
    }
}
