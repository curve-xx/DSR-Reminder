using System;
using System.ComponentModel.DataAnnotations;

namespace EAWorkerService.Models;

public class AttendanceDetails
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Email Id is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public required string EmailId { get; set; }

    [Required(ErrorMessage = "Mobile Number is required.")]
    [StringLength(10, ErrorMessage = "Mobile Number cannot be longer than 10 characters.")]
    public required string MobileNumber { get; set; }
    
    public required string IPAddress { get; set; }

    public bool IsDeleted { get; set; }

    [Required(ErrorMessage = "Created By is required.")]
    public required string CreatedBy { get; set; }

    [Required(ErrorMessage = "Created On is required.")]
    public DateTime CreatedOn { get; set; }
}
