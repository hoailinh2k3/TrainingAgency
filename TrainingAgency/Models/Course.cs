using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrainingAgency.Models;

public class Course
{
    [Key]
    [Column("CourseID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CourseId { get; set; }

    [StringLength(100)]
    public string CourseName { get; set; } = null!;

    [StringLength(100)]
    public string LecturerName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Fee { get; set; }

    public int MaxStudents { get; set; }

    public List<Registration>? Registrations { get; set; }
}
