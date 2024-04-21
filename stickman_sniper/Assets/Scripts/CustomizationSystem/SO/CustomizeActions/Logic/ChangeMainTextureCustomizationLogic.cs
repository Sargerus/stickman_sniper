using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DWTools.Customization
{
    public class ChangeMainTextureCustomizationLogic : ICustomizationAction
    {
        public async UniTask Action(CustomizableEntityItem item, List<string> assetsGUID)
        {
            AsyncOperationHandle<Texture2D> handler = default;

            try
            {
                if (item.MainMaterial == null)
                    return;

                if (assetsGUID is not { Count: > 0 })
                    return;

                handler = Addressables.LoadAssetAsync<Texture2D>(assetsGUID[0]);
                await handler.Task;

                if (handler.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"Error customizing item {item}, loading resource {assetsGUID[0]} in action {nameof(ChangeMainTextureCustomizationLogic)}");
                }

                item.MainMaterial.SetTexture("_BaseMap", handler.Result);
            }
            catch { }
            finally
            {
                if (handler.IsValid())
                    Addressables.Release(handler);
            }
        }
    }
}