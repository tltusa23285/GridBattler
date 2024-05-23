using UnityEngine;
using UnityEditor;

public abstract class JsonLibraryEditorWindow<T> : EditorWindow where T : IJsonObject
{
    protected JsonObjectLibrary<T> Library;
    protected string[] LibItems;

    protected GUIStyle LeftPane;
    protected GUIStyle RightPane;

    protected abstract T CurrentItem { get; set; }
    protected abstract string LibraryFolder { get; }
    protected abstract string LibraryFile { get; }
    protected SerializedObject ThisObj;
    protected SerializedProperty ThisProp;
    protected bool CurrentItemExists = false;

    protected bool AddingItem;
    protected string[] AddOptions;

    private void OnGUI()
    {
        LeftPane = new GUIStyle("window")
        {
            fixedWidth = position.width * 0.25f
        };
        RightPane = new GUIStyle("window")
        {
            fixedWidth = position.width * 0.75f
        };

        EditorGUILayout.BeginHorizontal();


        GUILayout.BeginVertical(LeftPane);
        DrawLeftPane();
        GUILayout.EndVertical();


        GUILayout.BeginVertical(RightPane);
        DrawRightPane();
        GUILayout.EndVertical();


        EditorGUILayout.EndHorizontal();
    }

    protected virtual void Setup()
    {
        Library = JsonObjectLibrary<T>.LoadLibrary(LibraryFolder, LibraryFile);
        LibItems = Library.GetItemIDs();
    }

    protected virtual void DrawLeftPane()
    {
        foreach (var item in LibItems)
        {
            if (GUILayout.Button(item))
            {
                SetCurrent(item);
                GUIUtility.ExitGUI();
            }
        }
        if (GUILayout.Button("Add"))
        {
            AddingItem = true;
            AddOptions = GetAddItems();
        }
    }

    protected virtual void DrawRightPane()
    {
        if (AddingItem)
        {
            foreach (var item in AddOptions)
            {
                if (GUILayout.Button(item))
                {
                    SetCurrentFromType(item);
                    AddingItem = false;
                }
            }
        }
        else if (CurrentItemExists)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                SaveItem(CurrentItem);
                Setup();
            }
            if (GUILayout.Button("Delete"))
            {
                DeleteItem(CurrentItem);
                Setup();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(ThisProp, true);
            ThisObj.ApplyModifiedProperties();
        }
        else
        {
            EditorGUILayout.LabelField("Select an item to edit");
        }
    }

    protected virtual void SaveItem(T item)
    {
        Library.SaveItem(item);
    }

    protected virtual void DeleteItem(T item)
    {
        Library.DeleteItem(item.JsonID);
    }

    protected virtual void SetCurrent(string id)
    {
        Library.GetItem(id, out T result);
        SetCurrent(result);
    }

    protected virtual void SetCurrent(T item)
    {
        if (item == null)
        {
            CurrentItemExists = false;
            CurrentItem = default(T);
            ThisObj = null;
            ThisProp = null;
        }
        else
        {
            CurrentItemExists = true;
            CurrentItem = item;
            //ThisObj = new SerializedObject(this);
            //ThisProp = this.ThisObj.FindProperty(SerializedPropertyName);
            SetSerialized(ref ThisObj, ref ThisProp);
            ThisProp.isExpanded = true;
        }
    }

    protected abstract string[] GetAddItems();
    protected abstract void SetCurrentFromType(string type);
    protected abstract void SetSerialized(ref SerializedObject obj, ref SerializedProperty prop);
}
