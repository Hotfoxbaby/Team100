﻿// Authors: Liana Cruz, Luke Stansbury
// Date 4/18/2025
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private LMSContext db;
        public StudentController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query = from s in db.Students
                        join e in db.Enrolleds on s.UId equals e.UId
                        join c in db.Classes on e.ClassId equals c.ClassId
                        join co in db.Courses on c.CourseId equals co.CourseId
                        where s.UId == uid
                        select new
                        {
                            subject = co.Subject,
                            number = co.Number,
                            name = co.Name,
                            season = c.Semester!.Substring(0, c.Semester.Length - 4),
                            year = c.Semester.Substring(c.Semester.Length - 4),
                            grade = e.Grade == null ? "--" : e.Grade
                        };
            var queryList = query.ToList();
            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            string sem = season + year;
            var query = from a in db.Assignments
                        join ac in db.AssignmentCategories on a.AcId equals ac.AcId
                        join c in db.Classes on ac.ClassId equals c.ClassId
                        join co in db.Courses on c.CourseId equals co.CourseId
                        join e in db.Enrolleds on new{ unid = uid, classId = c.ClassId } equals new{ unid = e.UId, classId = e.ClassId }
                        where co.Subject == subject && co.Number == num.ToString() && c.Semester == sem
                        select new
                        {
                            aname = a.Name,
                            cname = ac.Name,
                            due = a.Due,
                            score = a.Submissions.Where(x => x.UId == uid).Select(x => x.Score).FirstOrDefault(),
                            semester = c.Semester
                        };
            var queryList = query.ToList();

            return Json(query.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents) 
        {
            var query = from co in db.Courses
                        join c in db.Classes on co.CourseId equals c.CourseId
                        join ac in db.AssignmentCategories on c.ClassId equals ac.ClassId
                        join a in db.Assignments on ac.AcId equals a.AcId
                        join s in db.Students on uid equals s.UId
                        where co.Subject == subject && co.Number == num.ToString() && c.Semester == season + year.ToString() && a.Name == asgname
                        select new{
                            a = a.AId,
                            s = s.UId
                        };
            var queryItem = query.FirstOrDefault();
            var query1 = from co in db.Courses
                         join c in db.Classes on co.CourseId equals c.CourseId
                         join ac in db.AssignmentCategories on c.ClassId equals ac.ClassId
                         join a in db.Assignments on ac.AcId equals a.AcId
                         join s in db.Students on uid equals s.UId
                         join su in db.Submissions on new{aid = a.AId, uid = s.UId} equals new{aid = su.AId, uid = su.UId}
                         where co.Subject == subject && co.Number == num.ToString() && c.Semester == season + year.ToString() && a.Name == asgname
                         select new
                         {
                             a = a.AId,
                             s = s.UId
                         };
            var queryList = query1.ToList();
            try
            {
                if(queryList.Count == 0)
                    db.Add(new Submission { AId = queryItem!.a, UId = uid, Contents = contents, Time = DateTime.Now});
                else
                    db.Update(new Submission { AId = queryItem!.a, UId = uid, Contents = contents, Time = DateTime.Now });
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {   
            var query = from co in db.Courses
                        join c in db.Classes on co.CourseId equals c.CourseId
                        where co.Subject == subject && co.Number == num.ToString() && c.Semester == season + year.ToString()
                        select c.ClassId;
            try
            {
                db.Add(new Enrolled { ClassId = query.ToArray()[0], UId = uid });
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {   
            var query = from e in db.Enrolleds
                        where e.UId == uid
                        select e.Grade;
            List<string> grades = query.ToList();
            double totalGrades = 0;
            int countedGrades = 0;
            foreach (string g in grades)
            {
                if (g is null)
                    continue;
                totalGrades += CalculateGPA(g);
                countedGrades++;
            }
            double gpa = countedGrades == 0 ? 0.0 : totalGrades / (double)countedGrades;
            return Json(new { gpa = gpa });
        }
        
        private double CalculateGPA(string grade)
        {
            if (grade == "A")
            {
                return 4.0;
            }
            else if (grade == "A-")
            {
                return 3.7;
            }
            else if (grade == "B+")
            {
                return 3.3;
            }
            else if (grade == "B")
            {
                return 3.0;
            }
            else if (grade == "B-")
            {
                return 2.7;
            }
            else if (grade == "C+")
            {
                return 2.3;
            }
            else if (grade == "C")
            {
                return 2.0;
            }
            else if (grade == "C-")
            {
                return 1.7;
            }
            else if (grade == "D+")
            {
                return 1.3;
            }
            else if (grade == "D")
            {
                return 1.0;
            }
            else if (grade == "D-")
            {
                return 0.7;
            }
            else
            {
                return 0.0;
            }
        }
        /*******End code to modify********/

    }
}

