using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Redactor
{
    public class RedactorBiomeSelectedBlock : MonoBehaviour
    {
        [SerializeField]
        TranslaterComp TitleMod;
        [SerializeField]
        TextMeshProUGUI TextMod;

        [SerializeField]
        TranslaterComp TitleName;
        [SerializeField]
        TextMeshProUGUI TextName;

        [SerializeField]
        RawImage imagePreview;
        PreviewBlock previewBlock;

        BlockData SelectBlock;

        // Start is called before the first frame update
        void Start()
        {
            RedactorBiomeCTRL.changeBiome += ReSelectBlock;
        }

        // Update is called once per frame
        void Update()
        {
            updateImagePreview();
        }

        private void OnEnable()
        {
            ReSelectBlock();
        }

        public void ClickButtonSelectBlock()
        {
            WindowMenuCTRL.ClickRedactorBiomeSelectBlock();
        }

        //Взять данные о блоке и обновить все данные о нем
        void ReSelectBlock() {
            if(RedactorBiomeCTRL.main != null)
                SelectBlock = RedactorBiomeCTRL.main.GetSelectBlock();


            if (SelectBlock == null) {
                TextMod.text = Language.GetTextFromKey(Language.keys.redactorBiomeNoBlockMod, "----");
                TextName.text = Language.GetTextFromKey(Language.keys.redactorBiomeNoBlockName, "Void");
                previewBlock = null;

                return;
            }

            TextMod.text = SelectBlock.mod;
            TextName.text = SelectBlock.name;

            previewBlock = PreviewBlocksCTRL.GetPreview(SelectBlock);
        }

        void updateImagePreview() {
            if (previewBlock == null)
            {
                if(imagePreview.color.a > 0.5f)
                    imagePreview.color = new Color(1,1,1,0);
                return;
            }

            if(imagePreview.color.a < 0.05f)
                imagePreview.color = new Color(1,1,1,1);

            imagePreview.texture = previewBlock.GetRender();

        }
    }
}
