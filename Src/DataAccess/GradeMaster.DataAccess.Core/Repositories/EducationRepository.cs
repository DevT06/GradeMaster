﻿using SQLitePCL;
using GradeMaster.Shared.Core.Entities;
using GradeMaster.Shared.Core.Interfaces;

namespace GradeMaster.DataAccess.Core.Repositories;

public class EducationRepository : IEducationRepository
{
    private readonly GradeMasterDbContext _context;

    public EducationRepository(GradeMasterDbContext context)
    {
        _context = context;
    }

    public Education? GetById(int id)
    {
        return _context.Educations.Find(id);
    }

    public List<Education> GetAll()
    { 
        return _context.Educations.ToList();
    } 

    public Education Add(Education education)
    { 
        _context.Subjects.AttachRange(education.Subjects);

        _context.Educations.Add(education);

        _context.SaveChanges();

        return education;
    }

    public void Update(int id, Education education)
    {
        // handle if education doesnt exist in db
        _context.Subjects.AttachRange(education.Subjects);

        _context.Educations.Update(education);

        _context.SaveChanges();
    }

    public void DeleteById(int id)
    {
        var existingBlog = GetById(id);

        if (existingBlog != null)
        {
            _context.Remove(existingBlog);
            _context.SaveChanges();
        }
    }

    // Lambda expression
    //public void DeleteById(int id) => throw new NotImplementedException();
}