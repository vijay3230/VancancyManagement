using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace JobsApplication.Models
{
    public class JobDAL
    {
        public int CreateJob(SqlConnection sqlConnection, CreateJobRequest createJobRequest)
        {
            string query = @"INSERT INTO Job(title, description, locationId, departmentId, postedDate, closingDate)
                     OUTPUT INSERTED.Id
                     VALUES (@Title, @Description, @LocationId, @DepartmentId, GETDATE(), @ClosingDate)";

            using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
            {
                cmd.Parameters.AddWithValue("@Title", createJobRequest.Title);
                cmd.Parameters.AddWithValue("@Description", createJobRequest.Description);
                cmd.Parameters.AddWithValue("@LocationId", createJobRequest.LocationId);
                cmd.Parameters.AddWithValue("@DepartmentId", createJobRequest.DepartmentId);
                cmd.Parameters.AddWithValue("@ClosingDate", createJobRequest.ClosingDate);

                sqlConnection.Open();
                int newJobId = (int)cmd.ExecuteScalar();
                sqlConnection.Close();

                return newJobId;
            }
        }
        public int UpdateJob(SqlConnection sqlConnection, int jobId, UpdateJobRequest updateJobRequest)
        {
            string query = @"UPDATE Job SET title = @Title, description = @Description, locationId = @LocationId, departmentId = @DepartmentId, closingDate = @ClosingDate WHERE id = @JobId";

            using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
            {
                cmd.Parameters.AddWithValue("@Title", updateJobRequest.Title);
                cmd.Parameters.AddWithValue("@Description", updateJobRequest.Description);
                cmd.Parameters.AddWithValue("@LocationId", updateJobRequest.LocationId);
                cmd.Parameters.AddWithValue("@DepartmentId", updateJobRequest.DepartmentId);
                cmd.Parameters.AddWithValue("@ClosingDate", updateJobRequest.ClosingDate);
                cmd.Parameters.AddWithValue("@JobId", jobId);

                sqlConnection.Open();
                int rowsUpdated = cmd.ExecuteNonQuery();
                sqlConnection.Close();

                return rowsUpdated;
            }
        }

        public JobDetailsResponse GetJobDetails(SqlConnection sqlConnection, int id)
        {
            JobDetailsResponse jobDetailsResponse = new JobDetailsResponse();

            string query = @"SELECT Job.id, Job.code, Job.title, Job.description,
                            Location.id as locationId, Location.title as locationTitle, Location.city, Location.state, Location.country, Location.zip,
                            Department.id as DepartmentId, Department.title as departmentTitle,
                            Job.postedDate, Job.closingDate
                            FROM Job INNER JOIN Department on Job.departmentId = Department.id
                            INNER JOIN Location on Job.locationId = Location.id
                            WHERE Job.id = " + id;

            SqlDataAdapter da = new SqlDataAdapter(query, sqlConnection);

            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                jobDetailsResponse.Id = Convert.ToInt32(row[0]);
                jobDetailsResponse.Code = Convert.ToString(row[1]);
                jobDetailsResponse.Title = Convert.ToString(row[2]);
                jobDetailsResponse.Description = Convert.ToString(row[3]);

                Location location = new Location();
                location.Id = Convert.ToInt32(row[4]);
                location.Title = Convert.ToString(row[5]);
                location.City = Convert.ToString(row[6]);
                location.State = Convert.ToString(row[7]);
                location.Country = Convert.ToString(row[8]);
                location.Zip = Convert.ToInt32(row[9]);
                jobDetailsResponse.Location = location;

                Department department = new Department();
                department.Id = Convert.ToInt32(row[10]);
                department.Title = Convert.ToString(row[11]);
                jobDetailsResponse.Department = department;
                jobDetailsResponse.PostedDate = Convert.ToDateTime(row[12]);
                jobDetailsResponse.ClosingDate = Convert.ToDateTime(row[13]);
            }

            return jobDetailsResponse;
        }

    public JobListResponse GetJobs(SqlConnection sqlConnection, JobListRequest jobListRequest)
        {
            JobListResponse jobListResponse = new JobListResponse();

            string query = @"SELECT Job.id, code, Job.title, Location.title as location, Department.title as department, postedDate, closingDate
                            FROM Job INNER JOIN Department on Job.departmentId = Department.id
                            INNER JOIN Location on Job.locationId = Location.id
                            WHERE Job.title like '%" + jobListRequest.Q + @"%' AND Location.id = " + jobListRequest.LocationId + " AND Department.id = " + jobListRequest.DepartmentId
                            + @" ORDER BY Job.id OFFSET " + (jobListRequest.PageNo - 1) * jobListRequest.PageSize + " ROWS FETCH NEXT " + jobListRequest.PageSize + " ROWS ONLY";

            SqlDataAdapter da = new SqlDataAdapter(query, sqlConnection);

            DataTable dt = new DataTable();

            da.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                List<Job> jobList = new List<Job>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Job job = new Job();
                    DataRow row = dt.Rows[i];
                    job.Id = Convert.ToInt32(row[0]);
                    job.Code = Convert.ToString(row[1]);
                    job.Title = Convert.ToString(row[2]);
                    job.Location = Convert.ToString(row[3]);
                    job.Department = Convert.ToString(row[4]);
                    job.PostedDate = Convert.ToDateTime(row[5]);
                    job.ClosingDate = Convert.ToDateTime(row[6]);
                    jobList.Add(job);
                }
                jobListResponse.JobList = jobList;
            }

            jobListResponse.Total = dt.Rows.Count;
            return jobListResponse;
        }
    }
}
