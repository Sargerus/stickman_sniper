using System.Collections.Generic;
using UnityEngine;

namespace DWTools.Customization
{
    [CreateAssetMenu(menuName = "[CUSTOMIZATION]Customization/CustomizationDataContainer", fileName = "CustomizationDataContainer")]
    public class CustomiationDataContainerSO : ScriptableObject
    {
        public List<CustomizationDataSO> CustomizationData;
    }
}
