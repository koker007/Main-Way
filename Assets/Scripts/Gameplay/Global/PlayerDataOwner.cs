using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


//Серверные только с видимостью только для текущего игрока
public class PlayerDataOwner : NetworkBehaviour
{

    public readonly SyncList<SpaceObjDataNet> spaceObjList = new SyncList<SpaceObjDataNet>();
    //public readonly SyncList<ItemTest> inventory = new SyncList<ItemTest>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnStartClient()
    {
        //Регистрируем функцию обновления данных сервера
        spaceObjList.Callback += OnSpaceObjDataNetUpdate;

        // Process initial SyncList payload
        for (int index = 0; index < spaceObjList.Count; index++)
            OnSpaceObjDataNetUpdate(SyncList<SpaceObjDataNet>.Operation.OP_ADD, index, new SpaceObjDataNet(), spaceObjList[index]);
    }

    void OnSpaceObjDataNetUpdate(SyncList<SpaceObjDataNet>.Operation op, int index, SpaceObjDataNet oldItem, SpaceObjDataNet newItem)
    {
        switch (op)
        {
            case SyncList<SpaceObjDataNet>.Operation.OP_ADD:
                // index is where it was added into the list
                // newItem is the new item
                break;
            case SyncList<SpaceObjDataNet>.Operation.OP_INSERT:
                // index is where it was inserted into the list
                // newItem is the new item
                break;
            case SyncList<SpaceObjDataNet>.Operation.OP_REMOVEAT:
                // index is where it was removed from the list
                // oldItem is the item that was removed
                break;
            case SyncList<SpaceObjDataNet>.Operation.OP_SET:
                // index is of the item that was changed
                // oldItem is the previous value for the item at the index
                // newItem is the new value for the item at the index
                break;
            case SyncList<SpaceObjDataNet>.Operation.OP_CLEAR:
                // list got cleared
                break;
        }
    }
}

//Информация передаваемая по сети
public struct SpaceObjDataNet{
    
}
