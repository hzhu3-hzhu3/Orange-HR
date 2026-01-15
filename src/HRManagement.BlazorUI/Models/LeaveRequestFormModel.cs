using System.ComponentModel.DataAnnotations;
namespace HRManagement.BlazorUI.Models;
public class LeaveRequestFormModel
{
    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(7);
    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(10);
    [Required(ErrorMessage = "Reason is required")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    public string Reason { get; set; } = string.Empty;
}
