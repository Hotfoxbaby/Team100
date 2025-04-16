using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCategories = new HashSet<AssignmentCategory>();
            Enrolleds = new HashSet<Enrolled>();
        }
        public Class(int courseId, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            CourseId = courseId;
            Semester = season + year.ToString();
            StartTime = start;
            EndTime = end;
            Location = location;
            PId = instructor;
            
            AssignmentCategories = new HashSet<AssignmentCategory>();
            Enrolleds = new HashSet<Enrolled>();
        }
        public string? Location { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Semester { get; set; }
        public int? CourseId { get; set; }
        public int ClassId { get; set; }
        public string? PId { get; set; }

        public virtual Course? Course { get; set; }
        public virtual Professor? PIdNavigation { get; set; }
        public virtual ICollection<AssignmentCategory> AssignmentCategories { get; set; }
        public virtual ICollection<Enrolled> Enrolleds { get; set; }
    }
}
