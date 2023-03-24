using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Storage;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Storage;

public class ObjectController : MonoBehaviour
{
    public WorldObjectManager woManager;
    public WorldObjectMemoryStorage woMemoryStorage;
    public ServerDataAdapterWrapper dataAdapter;
    public StringVariable clientName;
    public StringVariable serverObjectTag;


    public Task<IEnumerable<GameObject>> ObjectRecieve()
    {
        Debug.Log("Recieve all");
        return woManager.LoadServerContentAsync();
    }

    public async void ObjectRemoved(string name)
    {
        Debug.Log("Remove");
        await woManager.RemoveObjectAsync(name);
    }

    public async Task RemoveObjects(List<GameObject> objs)
    {
        for (int i = 0; i < objs.Count; i++)
            await DestroyObject(objs[i].name, objs[i]);
    }

    public void ObjectsClear()
    {
        Debug.Log("Clear");
        woManager.ClearLocalContent();
    }

    public void RemoveObjectFromLocal(string name)
    {
        Debug.Log("Remove local");
        GameObject o;
        bool res = woMemoryStorage.Remove(name, out o);
        if (res)
            Destroy(o);
    }

    internal async Task DestroyObject(string name, GameObject obj)
    {
        await woManager.RemoveObjectAsync(name);
        Destroy(obj);

        Debug.Log("Destroyed " + name);
    }

    internal async Task DestroyObject(string name)
    {
        await woManager.RemoveObjectAsync(name);

        Debug.Log("Destroyed " + name);
    }

    internal async Task<bool> ContainsObject(string name)
    {
        return await dataAdapter.ContainsWorldObjectAsync(name);
    }

    internal async Task AddObjectAsync(GameObject obj)
    {
        // Debug.Log("Add " + obj.name);
        await woManager.AddObjectAsync(obj);
    }

    internal async Task UpdateProperties(string name)
    {
        // Debug.Log("Update " + name);
        await woManager.UpdateObjectAsync(name);
    }

    public void AddedNewObjectAtRuntime(GameObject o)
    {
        o.tag = serverObjectTag.Value;
        var iph = o.GetComponent<InputPropertiesHandler>();
        if (iph != null)
            iph.objCont = this;
    }
}
