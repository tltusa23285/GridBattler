using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using WEnemy = IJsonObjectWrapper<EnemyActorController>;

public class EnemyEditorWindow : JsonLibraryEditorWindow<WEnemy>
{
    [MenuItem("Game/Enemy Editor")]
    public static void ShowWindow()
    {
        EnemyEditorWindow window = GetWindow<EnemyEditorWindow>();
        window.Setup();
        window.Show();
    }

    [SerializeReference] public EnemyActorController CurrentEnemy;
    private WEnemy ActiveEnemy;
    protected override WEnemy CurrentItem
    {
        get { return ActiveEnemy; }
        set
        {
            ActiveEnemy = value;
            CurrentEnemy = ActiveEnemy.Object;
        }
    }
    protected override string LibraryFolder => EnemyActorController.LIB_FOLDER_NAME;
    protected override string LibraryFile => EnemyActorController.LIB_FILE_NAME;

    protected override string[] GetAddItems()
    {
        List<string> result = new List<string>();
        foreach (var i in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(EnemyActorController))))
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
            SetCurrent(new WEnemy(Activator.CreateInstance(i) as EnemyActorController));
            return;
        }
    }
    protected override void SetSerialized(ref SerializedObject obj, ref SerializedProperty prop)
    {
        obj = new SerializedObject(this);
        prop = ThisObj.FindProperty(nameof(CurrentEnemy));
    }
}
