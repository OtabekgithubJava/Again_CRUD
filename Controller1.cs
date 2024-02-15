using Microsoft.AspNetCore.Mvc;
using Dapper;
using Npgsql;


using System;
using System.Collections.Generic;

namespace Just
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class Writing : ControllerBase
    {
        public string connectionString = "Server=localhost;Port=5432;Database=WritingDB;username=postgres;Password=2712;";


        [HttpGet]
        public List<student> GetStudentsInfo()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM student;";
                using NpgsqlCommand cmd = new NpgsqlCommand(query, connection);

                var outcome = cmd.ExecuteReader();

                List<student> student_list = new List<student>();

                while (outcome.Read())
                {
                    student_list.Add(new student
                    {
                        student_id = (int)outcome[0],
                        first_name = (string)outcome[1],
                        last_name = (string)outcome[2],
                        student_level = (string)outcome[3]
                    });
                }

                return student_list;
            }
        }

        [HttpGet]
        public List<course> GetCourseInfo()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM course;";
                using NpgsqlCommand cmd = new NpgsqlCommand(query, connection);

                var outcome = cmd.ExecuteReader();

                List<course> course_list = new List<course>();

                while (outcome.Read())
                {
                    course_list.Add(new course
                    {
                        course_id = (int)outcome[0],
                        course_name = (string)outcome[1],
                        descriptions = (string)outcome[2]
                    });
                }

                return course_list;
            }
        }
        
        [HttpGet]
        public List<course_enrolled> GetEnrolledCourseInfo()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM course_enrolled";

                using NpgsqlCommand cmd = new NpgsqlCommand(query, connection);

                var outcome = cmd.ExecuteReader();

                List<course_enrolled> enrolledCourseList = new List<course_enrolled>();

                while (outcome.Read())
                {
                    enrolledCourseList.Add(new course_enrolled
                    {
                        course_id = (int)outcome[0],
                        enrollment_id = (int)outcome[1],
                        student_id = (int)outcome[2]
                    });
                }

                return enrolledCourseList;
            }
        }

        
        [HttpPut]
        public student UpdateStudentInfo(student updatedStudent)
        {
            string query = "UPDATE student SET first_name = @first_name, last_name = @last_name, student_level = @student_level WHERE student_id = @student_id";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Execute(query, new student
                {
                    student_id = updatedStudent.student_id,
                    first_name = updatedStudent.first_name,
                    last_name = updatedStudent.last_name,
                    student_level = updatedStudent.student_level
                });

                return updatedStudent;
            }
        }

        [HttpPut]
        public course UpdateCourseInfo(course updatedCourse)
        {
            string query = "UPDATE course SET course_name = @course_name, descriptions = @descriptions WHERE course_id = @course_id";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Execute(query, new course
                {
                    course_id = updatedCourse.course_id,
                    course_name = updatedCourse.course_name,
                    descriptions = updatedCourse.descriptions
                });

                return updatedCourse;
            }
        }
        
        [HttpDelete]
        public int DeleteStudentInfo(int id)
        {
            string deleteEnrollmentsSql = "DELETE FROM course_enrolled WHERE student_id = @student_id";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Execute(deleteEnrollmentsSql, new { student_id = id });
            }
            
            string deleteStudentSql = "DELETE FROM student WHERE student_id = @student_id";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                return connection.Execute(deleteStudentSql, new { student_id = id });
            }
        }


        [HttpDelete]
        public int DeleteCourseInfo(int id)
        {
            string deleteEnrollmentsSql = "DELETE FROM course_enrolled WHERE course_id = @course_id";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Execute(deleteEnrollmentsSql, new { course_id = id });
            }
            
            string deleteCourseSql = "DELETE FROM course WHERE course_id = @course_id";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                return connection.Execute(deleteCourseSql, new { course_id = id });
            }
        }


        
        
        [HttpPost]
        public student CreateNewStudent(student viewModel)
        {
            string sql = "INSERT INTO student (first_name, last_name, student_level) VALUES (@first_name, @last_name, @student_level) RETURNING student_id;";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                int newStudentId = connection.QuerySingle<int>(sql, new
                {
                    first_name = viewModel.first_name,
                    last_name = viewModel.last_name,
                    student_level = viewModel.student_level
                });

                viewModel.student_id = newStudentId;
                return viewModel;
            }
        }

        [HttpPost]
        public course CreateNewCourse(course viewModel)
        {
            string query = "INSERT INTO course(course_name, descriptions) VALUES (@course_name, @descriptions) RETURNING course_id";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                int newCourseId = connection.QuerySingle<int>(query, new
                {
                    course_name = viewModel.course_name,
                    descriptions = viewModel.descriptions
                });

                viewModel.course_id = newCourseId;
                return viewModel;
            }
        }

        
    }
}