using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos {

    /// <summary>
    /// Получить текстуру поверхности мира, указанного качества
    /// </summary>
    /// <param name="quarity"></param>
    /// <returns></returns>
    public interface IGetMainTexture
    {
        public Texture2D GetMainTexture(Size quality);
    }
}
