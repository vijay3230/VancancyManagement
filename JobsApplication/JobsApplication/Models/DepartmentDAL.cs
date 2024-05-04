using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace JobsApplication.Models
{
    public class DepartmentDAL
    {
        public int CreateDepartment(SqlConnection sqlConnection, CreateDepartmentRequest createDepartmentRequest)
        {
            string query = @"INSERT INTO Department(title)
                     OUTPUT INSERTED.Id
                     VALUES (@Title)";

            using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
            {
                cmd.Parameters.AddWithValue("@Title", createDepartmentRequest.Title);

                sqlConnection.Open();
                int newJobId = (int)cmd.ExecuteScalar();
                sqlConnection.Close();

                return newJobId;
            }
        }
        public int UpdateDepartment(SqlConnection sqlConnection, int departmentId, UpdateDepartmentRequest updateDepartmentRequest)
        {
            string query = @"UPDATE Department SET title = @Title WHERE id = @DepartmentId";

            using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
            {
                cmd.Parameters.AddWithValue("@Title", updateDepartmentRequest.Title);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                sqlConnection.Open();
                int rowsUpdated = cmd.ExecuteNonQuery();
                sqlConnection.Close();

                return rowsUpdated;
            }
        }

        public DepartmentListResponse GetDepartments(SqlConnection sqlConnection)
        {
            DepartmentListResponse departmentListResponse = new DepartmentListResponse();

            string query = "SELECT Department.id, Department.title FROM Department";
            SqlDataAdapter da = new SqlDataAdapter(query, sqlConnection);

            DataTable dt = new DataTable();

            da.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                List<Department> departmentList = new List<Department>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Department department = new Department();
                    DataRow row = dt.Rows[i];
                    department.Id = Convert.ToInt32(row[0]);
                    department.Title = Convert.ToString(row[1]);
                    departmentList.Add(department);
                }
                departmentListResponse.DepartmentList = departmentList;
            }
            return departmentListResponse;
        }
    }
}
