﻿using GradeMaster.Common.Entities;

namespace GradeMaster.Client.Shared.Utility;

public static class EducationUtils
{
    public static decimal CalculateWeightedAverage(ICollection<Grade> grades)
    {
        if (grades == null || !grades.Any())
        {
            return 0;
        }

        decimal totalWeight = grades.Sum(g => g.Weight?.Value ?? 1); // Default weight to 1 if null
        if (totalWeight == 0)
        {
            return 0;
        }

        decimal weightedSum = grades.Sum(g => g.Value * (g.Weight?.Value ?? 1)); // Default weight to 1 if null
        return weightedSum / totalWeight;
    }

    public static decimal CalculateEducationAverage(List<Subject> subjects)
    {
        if (subjects == null || !subjects.Any())
        {
            return 0;
        }

        var subjectAverages = subjects
            .Where(s => s.Grades != null && s.Grades.Any())
            .Select(s => CalculateWeightedAverage(s.Grades));

        if (!subjectAverages.Any())
        {
            return 0;
        }

        return subjectAverages.Average();
    }
}