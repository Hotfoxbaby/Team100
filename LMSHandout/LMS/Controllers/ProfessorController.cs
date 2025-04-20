// Authors: Liana Cruz, Luke Stansbury
// Date 4/18/2025
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var query = from co in db.Courses
                        join c in db.Classes on co.CourseId equals c.CourseId
                        join e in db.Enrolleds on c.ClassId equals e.ClassId
                        join s in db.Students on e.UId equals s.UId
                        where co.Subject == subject && co.Number == num.ToString() && c.Semester == season + year
                        select new
                        {

                            fname = s.Fname,
                            lname = s.Lname,
                            uid = s.UId,
                            dob = s.Dob,
                            grade = e.Grade == null ? "--" : e.Grade
                        };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var query = from co in db.Courses
                        join c in db.Classes on co.CourseId equals c.CourseId
                        join ac in db.AssignmentCategories on c.ClassId equals ac.ClassId
                        join a in db.Assignments on ac.AcId equals a.AcId
                        where co.Subject == subject && co.Number == num.ToString() && c.Semester == season + year && ac.Name == category
                        select new
                        {
                            aname = a.Name,
                            cname = ac.Name,
                            due = a.Due,
                            submissions = a.Submissions.Count()
                        };
            if(category == null)
            {
                query = from co in db.Courses
                        join c in db.Classes on co.CourseId equals c.CourseId
                        join ac in db.AssignmentCategories on c.ClassId equals ac.ClassId
                        join a in db.Assignments on ac.AcId equals a.AcId
                        where co.Subject == subject && co.Number == num.ToString() && c.Semester == season + year
                        select new
                        {
                            aname = a.Name,
                            cname = ac.Name,
                            due = a.Due,
                            submissions = a.Submissions.Count()
                        };
            }
            return Json(query.ToArray());
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query = from co in db.Courses
                        join c in db.Classes on co.CourseId equals c.CourseId
                        join ac in db.AssignmentCategories on c.ClassId equals ac.ClassId
                        where co.Subject == subject && co.Number == num.ToString() && c.Semester == season + year
                        select new
                        {
                            name = ac.Name,
                            weight = ac.Weight
                        };
            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            var query = from co in db.Courses
                        join c in db.Classes on co.CourseId equals c.CourseId 
                        where co.Subject == subject && co.Number == num.ToString() && c.Semester == season + year
                        select c.ClassId;
            try
            {
                int classId = query.ToArray()[0];
                db.AssignmentCategories.Add(new AssignmentCategory
                { ClassId = classId, Name = category, Weight = ((uint)catweight)});
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            var query = from c in db.Classes
                        join co in db.Courses on c.CourseId equals co.CourseId
                        join ac in db.AssignmentCategories on c.ClassId equals ac.ClassId
                        where co.Subject == subject && co.Number == num.ToString() && c.Semester == season + year && ac.Name == category
                        select ac.AcId;
            try
            {
                db.Add(new Assignment { Name = asgname, Points = ((uint)asgpoints), Due = asgdue, Contents = asgcontents, AcId = query.ToArray()[0] });
                var query2 = from co in db.Courses 
                             join c in db.Classes on co.CourseId equals c.CourseId
                             join e in db.Enrolleds on c.ClassId equals e.ClassId
                             where co.Subject == subject && co.Number == num.ToString() && c.Semester == season + year
                             select e.UId;
                db.SaveChanges();
                foreach (string uid in query2.ToArray())
                {
                    UpdateGrades(subject, num, uid, season, year);
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var query1 = from a in db.Assignments
                         join s in db.Submissions on a.AId equals s.AId
                         into asa
                         from s in asa.DefaultIfEmpty()
                         select new
                         {
                             s = s,
                             a = a
                         };
            var query = from co in db.Courses
                        join c in db.Classes on co.CourseId equals c.CourseId
                        join ac in db.AssignmentCategories on c.ClassId equals ac.ClassId
                        join asa in query1 on ac.AcId equals asa.a.AcId
                        join st in db.Students on asa.s.UId equals st.UId
                        where co.Subject == subject && co.Number == num.ToString() && c.Semester == season + year.ToString() && ac.Name == category && asa.a.Name == asgname
                        select new
                        {
                            fname = st.Fname,
                            lname = st.Lname,
                            uid = asa.s.UId,
                            time = asa.s.Time,
                            score = asa.s.Score == null ? "--" : asa.s.Score.ToString()
                        };
            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            var query = from co in db.Courses
                        join c in db.Classes on co.CourseId equals c.CourseId
                        join ac in db.AssignmentCategories on c.ClassId equals ac.ClassId
                        join a in db.Assignments on ac.AcId equals a.AcId
                        join s in db.Submissions on a.AId equals s.AId
                        where co.Subject == subject && co.Number == num.ToString() && c.Semester == season + year && ac.Name == category && a.Name == asgname && s.UId == uid
                        select s;
            try
            {
                query.ToArray()[0].Score = (uint)score;
                db.Update(query.ToArray()[0]);
                db.SaveChanges();
                UpdateGrades(subject, num, uid, season, year);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });

            }
        }

        /// <summary>
        /// Updates the grades for all students when a new assignment is released or
        /// When an assignment is graded.
        /// </summary>
        /// <returns></returns>
        private void UpdateGrades(string subject, int num, string uid, string season, int year)
        {
            var query1 = from a in db.Assignments
                         join s in db.Submissions on a.AId equals s.AId
                         into asa
                         from joined in asa.DefaultIfEmpty()
                         select new
                         {
                             uid = joined.UId,
                             sScore = joined.Score,
                             a = joined.AId,
                             ac = a.AcId,
                             aPoints =a.Points
                         };
            var query = from co in db.Courses
                        join c in db.Classes on co.CourseId equals c.CourseId
                        join ac in db.AssignmentCategories on c.ClassId equals ac.ClassId
                        join asa in query1 on ac.AcId equals asa.ac
                        join e in db.Enrolleds on asa.uid equals e.UId
                        where co.Subject == subject && co.Number == num.ToString() && asa.uid == uid && c.Semester == season + year
                        select new
                        {
                            aScore = asa.sScore == null ? 0 : asa.sScore,
                            aPoints = asa.aPoints,
                            ac = ac,
                            Aid = asa.a
                        };
            
            Dictionary<int, int> acPercentages = new Dictionary<int, int>();
            Dictionary<int, double> sScore = new Dictionary<int, double>();
            Dictionary<int, int> sPoints = new Dictionary<int, int>();
            List<int> aIDs = new List<int>();
            var queryList = query.ToList();
            var classID = queryList[0].ac.ClassId;
            foreach (var q in queryList)
            {
                if (!sScore.ContainsKey(q.ac.AcId))
                {
                    acPercentages.Add(q.ac.AcId, (int)q.ac.Weight!);
                    sScore.Add(q.ac.AcId, (int)q.aScore!);
                    sPoints.Add(q.ac.AcId, (int)q.aPoints!);
                    aIDs.Add(q.Aid);
                }
                else if(!aIDs.Contains(q.Aid))
                {
                    sScore[q.ac.AcId] += (double)q.aScore!;
                    sPoints[q.ac.AcId] += (int)q.aPoints!;
                    aIDs.Add(q.Aid);
                }
            }
            double scale = 0;
            foreach (var q in sScore.Keys)
            {
                scale += acPercentages[q];
                sScore[q] = (sScore[q] / sPoints[q]) * acPercentages[q];
            }
            scale = 100/scale;
            double grade = 0;
            foreach (var q in sScore.Keys)
            {
                grade += sScore[q] * scale;
            }
            if(grade >= 93)
            {
                db.Update(new Enrolled { UId = uid, Grade = "A", ClassId = (int)classID! });
            }
            else if (grade >= 90)
            {
                db.Update(new Enrolled { UId = uid, Grade = "A-", ClassId = (int)classID! });
            }
            else if (grade >= 87)
            {
                db.Update(new Enrolled { UId = uid, Grade = "B+", ClassId = (int)classID! });
            }
            else if (grade >= 83)
            {
                db.Update(new Enrolled { UId = uid, Grade = "B", ClassId = (int)classID! });
            }
            else if (grade >= 80)
            {
                db.Update(new Enrolled { UId = uid, Grade = "B-", ClassId = (int)classID! });
            }
            else if (grade >= 77)
            {
                db.Update(new Enrolled { UId = uid, Grade = "C+", ClassId = (int)classID! });
            }
            else if (grade >= 73)
            {
                db.Update(new Enrolled { UId = uid, Grade = "C", ClassId = (int)classID! });
            }
            else if (grade >= 70)
            {
                db.Update(new Enrolled { UId = uid, Grade = "C-", ClassId = (int)classID! });
            }
            else if (grade >= 67)
            {
                db.Update(new Enrolled { UId = uid, Grade = "D+", ClassId = (int)classID! });
            }
            else if (grade >= 63)
            {
                db.Update(new Enrolled { UId = uid, Grade = "D", ClassId = (int)classID! });
            }
            else if (grade >= 60)
            {
                db.Update(new Enrolled { UId = uid, Grade = "D-", ClassId = (int)classID! });
            }
            else 
            {
                db.Update(new Enrolled { UId = uid, Grade = "E", ClassId = (int)classID! });
            }
            db.SaveChanges();
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query = from p in db.Professors
                        join c in db.Classes on p.UId equals c.PId
                        join co in db.Courses on c.CourseId equals co.CourseId
                        where p.UId == uid
                        select new
                        {
                            subject = co.Subject,
                            number = co.Number,
                            name = co.Name,
                            season = c.Semester!.Substring(0, c.Semester.Length - 4),
                            year = c.Semester.Substring(c.Semester.Length - 4)
                        };
            return Json(query.ToArray());
        }


        
        /*******End code to modify********/
    }
}

