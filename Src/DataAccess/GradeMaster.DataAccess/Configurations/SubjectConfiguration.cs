﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GradeMaster.Common.Entities;

namespace GradeMaster.DataAccess.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("Subjects");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("Id").ValueGeneratedOnAdd();

        builder.Property(s => s.Name).HasColumnName("Name").IsUnicode().HasMaxLength(255).IsRequired().UseCollation("NOCASE");
        builder.Property(s => s.Description).HasColumnName("Description").IsUnicode().HasMaxLength(2500).IsRequired(false).UseCollation("NOCASE");
        builder.Property(s => s.Semester).HasColumnName("Semester").IsRequired();
        builder.Property(s => s.Completed).HasColumnName("Completed").IsRequired();

        builder.HasOne(s => s.Education).WithMany(e => e.Subjects).HasForeignKey(s => s.EducationId).OnDelete(DeleteBehavior.Cascade).IsRequired();

        builder.HasMany(s => s.Grades).WithOne(g => g.Subject).HasForeignKey(g => g.SubjectId).OnDelete(DeleteBehavior.Cascade).IsRequired();
    }
}