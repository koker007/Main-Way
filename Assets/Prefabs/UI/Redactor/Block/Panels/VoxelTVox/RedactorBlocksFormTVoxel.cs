using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedactorBlocksFormTVoxel : MonoBehaviour
{
    static public RedactorBlocksFormTVoxel main;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        main = this;

    }

    public void acceptVoxColorArray(Vector3Int ZoneTVoxelPos, Vector3Int ZoneTVoxelSize, Color color, Vector3 plusRand)
    {

        float H = 0;
        float S = 0;
        float V = 0;
        Color.RGBToHSV(color, out H, out S, out V);

        float Hmax = H + plusRand.x;
        float Smax = S + plusRand.y;
        float Vmax = V + plusRand.z;

        if (Hmax > 1)
            Hmax = 1;
        if (Smax > 1)
            Smax = 1;
        if (Vmax > 1)
            Vmax = 1;

        for (int selectX = 0; selectX < ZoneTVoxelSize.x; selectX++)
        {
            int nowX = ZoneTVoxelPos.x + selectX;
            if (nowX >= 16)
                break;

            for (int selectY = 0; selectY < ZoneTVoxelSize.y; selectY++)
            {
                int nowY = ZoneTVoxelPos.y + selectY;
                if (nowY >= 16)
                    break;

                for (int selectZ = 0; selectZ < ZoneTVoxelSize.z; selectZ++)
                {
                    int nowZ = ZoneTVoxelPos.z + selectZ;
                    if (nowZ >= 16)
                        break;

                    //Рандомизируем цвет
                    float Hnow = Random.Range(H, Hmax);
                    float Snow = Random.Range(S, Smax);
                    float Vnow = Random.Range(V, Vmax);

                    Color colorNow = Color.HSVToRGB(Hnow, Snow, Vnow);

                    acceptVoxColor(new Vector3Int(nowX, nowY, nowZ), colorNow);
                }
            }
        }
    }
    void acceptVoxColor(Vector3Int TVoxelPos, Color color)
    {
        BlockData blockData = RedactorBlocksCTRL.blockData;

        Texture2D texture2D = blockData.TVoxels.GetTexture();

        int TexU = TVoxelPos.x + 16 * TVoxelPos.z;
        int TexV = TVoxelPos.y;

        texture2D.SetPixel(TexU, TexV, color);
        texture2D.Apply();
    }

    public void acceptVoxExistArray(Vector3Int ZoneTVoxelPos, Vector3Int ZoneTVoxelSize, int shance) {
        for (int selectX = 0; selectX < ZoneTVoxelSize.x; selectX++)
        {
            int nowX = ZoneTVoxelPos.x + selectX;
            if (nowX >= 16)
                break;

            for (int selectY = 0; selectY < ZoneTVoxelSize.y; selectY++)
            {
                int nowY = ZoneTVoxelPos.y + selectY;
                if (nowY >= 16)
                    break;

                for (int selectZ = 0; selectZ < ZoneTVoxelSize.z; selectZ++)
                {
                    int nowZ = ZoneTVoxelPos.z + selectZ;
                    if (nowZ >= 16)
                        break;

                    bool exist = false;
                    if (Random.Range(0, 100) < shance)
                        exist = true;

                    acceptVoxExist(new Vector3Int(nowX, nowY, nowZ), exist);
                }
            }
        }
    }
    public void acceptVoxExist(Vector3Int position, bool exist) {

        BlockData blockData = RedactorBlocksCTRL.blockData;
        TypeVoxel.Data data = blockData.TVoxels.GetData();

        int id = position.x + position.y * 16 + position.z * 16 * 16;

        if (exist)
            data.exist[id] = 1;
        else data.exist[id] = 0;
    }
}
