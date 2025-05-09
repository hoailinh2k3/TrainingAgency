using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAgency.Models;

public class Registration
{
    [Key]
    [Column("RegistrationID")]
    public int RegistrationId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("CourseID")]
    public int CourseId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime RegistrationDate { get; set; }

    public bool PaymentStatus { get; set; }

    [ForeignKey("CourseId")]
    public Course Course { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
