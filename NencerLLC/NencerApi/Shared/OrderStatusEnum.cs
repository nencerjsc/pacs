using System.ComponentModel.DataAnnotations;

namespace NencerApi.Shared
{
    public enum OrderStatusEnum
    {
        [Display(Name = "none")]
        NONE,

        [Display(Name = "canceled")]
        CANCELED,

        [Display(Name = "pending")]
        PENDING,

        [Display(Name = "sample_received")]

        RECEIVED_SAMPLE,

        [Display(Name = "processing")]
        PROCESSING,

        [Display(Name = "result_checked")]
        RESULT_CHECKED,

        [Display(Name = "result_returned")]
        RESULT_RETURNED
    }
}
