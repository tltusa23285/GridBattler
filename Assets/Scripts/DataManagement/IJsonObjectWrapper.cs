using UnityEngine;

public struct IJsonObjectWrapper<T>: IJsonObject where T : IJsonObject
{
    public string JsonID => Object.JsonID;
    [SerializeReference] public T Object;
    public IJsonObjectWrapper(T obj)
    {
        Object = obj;
    }
}
