namespace APBD_Cw3_S30786.Models.Dto;

using System.ComponentModel.DataAnnotations;

public class CreateRoomDto
{
    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "Name cannot be empty")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "BuildingCode cannot be empty")]
    public string BuildingCode { get; set; } = string.Empty;

    public int Floor { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than zero")]
    public int Capacity { get; set; }

    public bool HasProjector { get; set; }

    public bool IsActive { get; set; }
}

public class UpdateRoomDto : CreateRoomDto
{
}