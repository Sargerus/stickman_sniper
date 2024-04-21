using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DWTools.Customization
{
    [CreateAssetMenu(menuName = "[CUSTOMIZATION]Customization/CustomizationAction/ChangeMainTexture", fileName = "ChangeMainTextureCustomizationAction")]
    public class ChangeMainTextureCustomizationAction : BaseCustomizationActionSO
    {
        private ChangeMainTextureCustomizationLogic _cache;

        public override async UniTask DoAction(CustomizableEntityItem item, List<string> assetsGUID)
        {
            _cache ??= new();

            await _cache.Action(item, assetsGUID);
        }
    }
}