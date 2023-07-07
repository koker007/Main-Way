using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewBlock : MonoBehaviour
{
    [SerializeField]
    RenderTexture renderTexture;
    [SerializeField]
    Camera camera;
    [SerializeField]
    MeshRenderer meshRenderer;
    [SerializeField]
    MeshFilter meshFilter;

    [SerializeField]
    Material materialBlock;
    [SerializeField]
    Material materialVoxels;

    BlockData blockData;

    float timeLastUse = 0;
    public bool isFree { get {
            //≈сли превью не используетс€ больше 5 секунд оно свободно
            if (timeLastUse < Time.unscaledTime - 5)
                return true;

            return false;
        } }

    // Start is called before the first frame update
    void Start()
    {
        iniRenderTexture();
    }

    public RenderTexture GetRender() {
        timeLastUse = Time.unscaledTime;
        camera.Render();

        return renderTexture;
    }
    public void SetNewData(BlockData blockData)
    {
        this.blockData = blockData;
        UpdateMesh();
    }

    public void SetPosFromID(int id) {
        transform.position = new Vector3(id * 3, 0, -10);
    }

    void iniRenderTexture() {
        renderTexture = new RenderTexture(100, 100, 32);
        camera.targetTexture = renderTexture;
        camera.enabled = false;
    }

    void UpdateMesh()
    {
        if (blockData == null)
            return;

        //”дал€ем старые данные
        if (meshFilter.sharedMesh != null)
        {
            //meshFilter.sharedMesh.Clear(false);
            Destroy(meshFilter.sharedMesh);

            //”дал€ем материалы
            for (int num = 0; num < meshRenderer.materials.Length; num++)
            {
                Destroy(meshRenderer.materials[num]);
            }
        }

        TypeBlock typeBlock = blockData as TypeBlock;
        TypeVoxel typeVoxel = blockData as TypeVoxel;
        TypeLiquid typeLiquid = blockData as TypeLiquid;

        if (typeBlock != null)
            MeshBlock();
        else if (typeVoxel != null)
            MeshVoxel();
        else if (typeLiquid != null)
            MeshLiquid();


        ///////////////////////////////////////////
        ////
        void MeshBlock()
        {
            meshFilter.sharedMesh = typeBlock.GetMesh(true, false, false, true, true, false);
            meshFilter.sharedMesh.Optimize();

            //примен€ем материалы к мешу
            meshRenderer.materials = new Material[6]{
                materialBlock,
                materialBlock,
                materialBlock,
                materialBlock,
                materialBlock,
                materialBlock,
            };

            meshRenderer.materials[0].mainTexture = typeBlock.wallFace.texture;
            meshRenderer.materials[1].mainTexture = typeBlock.wallBack.texture;
            meshRenderer.materials[2].mainTexture = typeBlock.wallRight.texture;
            meshRenderer.materials[3].mainTexture = typeBlock.wallLeft.texture;
            meshRenderer.materials[4].mainTexture = typeBlock.wallUp.texture;
            meshRenderer.materials[5].mainTexture = typeBlock.wallDown.texture;
        }
        void MeshVoxel()
        {
            meshFilter.sharedMesh = typeVoxel.GetMesh();

            meshRenderer.materials = new Material[1] {
                materialVoxels
            };

            meshRenderer.materials[0].mainTexture = typeVoxel.GetTexture();
        }
        void MeshLiquid()
        {

            meshFilter.sharedMesh = typeLiquid.GetMesh(true, false, false, true, true, false);
            meshRenderer.materials = new Material[6] {
                materialBlock,
                materialBlock,
                materialBlock,
                materialBlock,
                materialBlock,
                materialBlock
            };

            long textureNum = (long)(Time.time / (1.0f / typeLiquid.data.animSpeed));

            int tickNum = (int)(textureNum % typeLiquid.data.texturesMax);

            meshRenderer.materials[0].mainTexture = typeLiquid.GetTexture(tickNum, Side.face);
            meshRenderer.materials[1].mainTexture = typeLiquid.GetTexture(tickNum, Side.back);
            meshRenderer.materials[2].mainTexture = typeLiquid.GetTexture(tickNum, Side.left);
            meshRenderer.materials[3].mainTexture = typeLiquid.GetTexture(tickNum, Side.right);
            meshRenderer.materials[4].mainTexture = typeLiquid.GetTexture(tickNum, Side.down);
            meshRenderer.materials[5].mainTexture = typeLiquid.GetTexture(tickNum, Side.up);
        }
    }
}
