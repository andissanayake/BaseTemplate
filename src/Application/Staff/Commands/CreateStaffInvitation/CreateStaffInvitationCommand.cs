using System.ComponentModel.DataAnnotations;

namespace BaseTemplate.Application.Staff.Commands.CreateStaffInvitation;

[Authorize(Roles = Domain.Constants.Roles.StaffInvitationManager)]
public record CreateStaffInvitationCommand : IRequest<bool>
{
    [Required]
    [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
    public string StaffEmail { get; set; } = string.Empty;

    [Required]
    [MinLength(2, ErrorMessage = "Staff name must be at least 2 characters long.")]
    [MaxLength(100, ErrorMessage = "Staff name cannot exceed 100 characters.")]
    public string StaffName { get; set; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "At least one role must be selected.")]
    public List<string> Roles { get; set; } = new();
}
