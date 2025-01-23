﻿using System.ComponentModel.DataAnnotations;

namespace GradeMaster.Shared.Core.Entities;

public class Education
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [MaxLength(2500, ErrorMessage = "Description may not exceed 2500 characters.")]
    public string? Description { get; set; }

    // length of Education in Semesters
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Semesters must be at least 1.")]
    public int Semesters { get; set; }

    [Required]
    public bool Completed { get; set; }

    [Required]
    public DateOnly StartDate { get; set; } // start date of education

    [Required]
    public DateOnly EndDate { get; set; } // end date of education
    // public string Duration { get; set; } //start date and end date? optional

    // maybe implement duration / length?
    // implement variable properties like average

    public string? Institution { get; set; } // optional

    public List<Subject> Subjects { get; set; }

    public Education()
    {
        Subjects = new List<Subject>();
    }
}