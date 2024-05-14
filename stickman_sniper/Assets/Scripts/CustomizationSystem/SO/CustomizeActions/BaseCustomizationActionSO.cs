using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DWTools.Customization
{
    public abstract class BaseCustomizationActionSO : ScriptableObject, ICustomizationActionProvider
    {
        public abstract UniTask DoAction(CustomizableEntityItem item, List<AssetReference> assetsGUID);
    }

    public interface ICustomizationActionProvider
    {
        UniTask DoAction(CustomizableEntityItem item, List<AssetReference> assetsGUID);
    }

    public interface ICustomizationAction
    {
        UniTask Action(CustomizableEntityItem item, List<AssetReference> assetsGUID);
    }
}