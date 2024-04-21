using Cysharp.Threading.Tasks;
using System.Linq;

namespace DWTools.Customization
{
    public static class CustomizationDataSOExtensions
    {
        public static async UniTask Customize(this ICustomizationDataProvider customizeData, ICustomizableEntityProvider entityProvider)
        {
            foreach (var item in customizeData.CustomizeItems)
            {
                CustomizableEntityItem entity = entityProvider.Items.FirstOrDefault(g => g.PointKey.Equals(item.PointKey));
                if(entity == null) 
                {
                    continue;
                }

                await item.CustomizationAction.DoAction(entity, item.AssetsGUID);
            }
        }
    }
}