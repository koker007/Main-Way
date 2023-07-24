using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    public class ShadowCTRL : MonoBehaviour
    {
        [SerializeField]
        Material shadowMat;

        public void updateShadow(PlanetData planetData)
        {
            shadowMat.mainTexture = planetData.TextureShadow;
        }
    }
}
