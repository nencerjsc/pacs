using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NencerApi.Modules.User.Model
{
    public enum PermissionTypeEnum
    {
        [Display(Name = "ROOT")]
        ROOT,
        [Display(Name = "CHILD")]
        CHILD
    }
}
