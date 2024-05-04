using JobsApplication.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JobsApplication.Controllers
{
    public class DepartmentController : ApiController
    {
        private SqlConnection getSQLConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["JobsConnectionString"].ConnectionString);
        }

        [HttpPost]
        [Route("api/v1/departments")]
        public IHttpActionResult CreateDepartment(CreateDepartmentRequest createDepartmentRequest)
        {
            try
            {
                SqlConnection sqlConnection = getSQLConnection();
                DepartmentDAL departmentDAL = new DepartmentDAL();
                int newDepartmentId = departmentDAL.CreateDepartment(sqlConnection, createDepartmentRequest);

                string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                string resourceUrl = $"{baseUrl}/api/v1/departments/{newDepartmentId}";

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                response.Headers.Location = new Uri(resourceUrl);

                return ResponseMessage(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/v1/departments/{id}")]
        public IHttpActionResult UpdateDepartment(int id, UpdateDepartmentRequest updateDepartmentRequest)
        {
            try
            {
                SqlConnection sqlConnection = getSQLConnection();
                DepartmentDAL departmentDAL = new DepartmentDAL();

                int updatedRows = departmentDAL.UpdateDepartment(sqlConnection, id, updateDepartmentRequest);
                if (updatedRows == 0)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        [Route("api/v1/departments")]
        public IHttpActionResult GetDepartments()
        {
            try
            {
                SqlConnection sqlConnection = getSQLConnection();
                DepartmentListResponse departmentListResponse = new DepartmentListResponse();

                DepartmentDAL departmentDAL = new DepartmentDAL();
                departmentListResponse = departmentDAL.GetDepartments(sqlConnection);
                return Ok(departmentListResponse);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
