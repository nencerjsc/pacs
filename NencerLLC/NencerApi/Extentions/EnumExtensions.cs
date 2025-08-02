using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NencerApi.Extentions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var enumMember = enumValue.GetType()
                .GetMember(enumValue.ToString())[0];

            var displayAttribute = enumMember
                .GetCustomAttribute<DisplayAttribute>();

            return displayAttribute != null ? displayAttribute.Name : enumValue.ToString();
        }
    }
}
