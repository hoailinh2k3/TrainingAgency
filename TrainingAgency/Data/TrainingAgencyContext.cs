using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TrainingAgency.Models;

namespace TrainingAgency.Data;

public class TrainingAgencyContext : DbContext
{
    public TrainingAgencyContext()
    {
    }

    public TrainingAgencyContext(DbContextOptions<TrainingAgencyContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }

    public DbSet<Registration> Registrations { get; set; }

    public DbSet<User> Users { get; set; }
}
