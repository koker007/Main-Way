using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//’ранит данные о позици€х в которых сейчас кто-то находитс€ говорит серверу в каких област€х надо генерировать детально
public class GenPositions : MonoBehaviour
{
    public static GenPositions main;

    /*—писок позиций которые требуют обработки*/
    List<GenPos> genPosList = new List<GenPos>();

    class GenPos {
        /*ѕозици€ в галактике*/
        Vector3 galaxy;
        /*ѕозици€ мира в звездной €чейке котора€ требует детальной генерации*/
        string worldNum;
        /*ѕозици€ внутри мира*/
        Vector3 worldPos;


        public void SetData(Vector3 galaxyPos, string worldNumStr, Vector3 worldPos) {
            galaxy = galaxyPos;
            worldNum = worldNumStr;
            this.worldPos = worldPos;
        }

        /*ѕроверить позицию на необходимость генерации*/
        public void Generate() {
            //провер€ем текушую позицию на необходимость генерации

        }
    }

    [Server]
    void TestServerGenPosList() {
        //¬ыходим если не сервер
        if (!NetworkCTRL.main.isServer) 
            return;

        List<GenPos> genPosListNew = new List<GenPos>();
        foreach (GenPos genPos in genPosList) {
            if (genPos == null) 
                continue;

            genPos.Generate();

            genPosListNew.Add(genPos);
        }

        genPosList = genPosListNew;
    }

    private void FixedUpdate()
    {
        TestServerGenPosList();
    }

    private void Start()
    {
        main = this;
    }

}
