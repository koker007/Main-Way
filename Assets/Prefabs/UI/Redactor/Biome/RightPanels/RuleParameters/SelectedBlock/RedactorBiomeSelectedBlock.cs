using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        ButtonCTRL ButtonChooseBlock;

        BlockData SelectBlock;

        // Start is called before the first frame update
        void Start()
        {
            RedactorBiomeCTRL.changeSelectBlock += ReSelectBlock;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            ReSelectBlock();
        }

        //Взять данные о блоке и обновить все данные о нем
        void ReSelectBlock() {
            if(RedactorBiomeCTRL.main != null)
                SelectBlock = RedactorBiomeCTRL.main.GetSelectBlock();


            if (SelectBlock == null) {
                TextMod.text = Language.GetTextFromKey(Language.keys.redactorBiomeNoBlockMod, "----");
                TextName.text = Language.GetTextFromKey(Language.keys.redactorBiomeNoBlockName, "Void");

                return;
            }

            TextMod.text = SelectBlock.mod;
            TextName.text = SelectBlock.name;

        }
    }
}
