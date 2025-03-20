using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.ui
{
    public class SubPanelSingleSelector : SingleSelectGroup<CharacterMenuSubPanel>
    {
        [SerializeField] private List<CharacterMenuSubPanel> subPanels;

        protected override List<CharacterMenuSubPanel> GetSelectables()
        {
            return subPanels;
        }
    }
}
