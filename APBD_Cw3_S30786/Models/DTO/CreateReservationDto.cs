using System;
using System.Collections.Generic;

namespace APBD_Cw3_S30786.Models.Dto;

using System.ComponentModel.DataAnnotations;

public class CreateReservationDto : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "RoomId must be greater than zero")]
    public int RoomId { get; set; }

    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "OrganizerName cannot be empty")]
    public string OrganizerName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "Topic cannot be empty")]
    public string Topic { get; set; } = string.Empty;

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "Status cannot be empty")]
    public string Status { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult(
                "EndTime must be later than StartTime",
                new[] { nameof(EndTime), nameof(StartTime) });
        }
    }
}

public class UpdateReservationDto : CreateReservationDto
{
}