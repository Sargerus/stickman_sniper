using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace DWTools.Customization
{
    public abstract class BaseCustomizationActionSO : ScriptableObject, ICustomizationActionProvider
    {
        public abstract UniTask DoAction(CustomizableEntityItem item, List<string> assetsGUID);
    }

    public interface ICustomizationActionProvider
    {
        UniTask DoAction(CustomizableEntityItem item, List<string> assetsGUID);
    }

    public interface ICustomizationAction
    {
        UniTask Action(CustomizableEntityItem item, List<string> assetsGUID);
    }
}