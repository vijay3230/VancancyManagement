using JobsApplication.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JobsApplication.Controllers
{
    public class JobsController : ApiController
    {
        private SqlConnection getSQLConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["JobsConnectionString"].ConnectionString);
        }

        [HttpPut]
        [Route("api/v1/jobs")]
        public IHttpActionResult CreateJob(CreateJobRequest createJobRequest)
        {
            try
            {
                SqlConnection sqlConnection = getSQLConnection();
                JobDAL dal = new JobDAL();
                int newJobId = dal.CreateJob(sqlConnection, createJobRequest);

                string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                string resourceUrl = $"{baseUrl}/api/v1/jobs/{newJobId}";

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
        [Route("api/v1/jobs/{id}")]
        public IHttpActionResult UpdateJob(int id, UpdateJobRequest updateJobRequest)
        {
            try
            {
                SqlConnection sqlConnection = getSQLConnection();
                JobDAL dal = new JobDAL();

                JobDetailsResponse existingJob = dal.GetJobDetails(sqlConnection, id);
                if (existingJob == null)
                {
                    return NotFound();
                }

                int updatedRows = dal.UpdateJob(sqlConnection, id, updateJobRequest);
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


        [HttpPost]
        [Route("api/jobs/list")]
        public IHttpActionResult GetJobs(JobListRequest jobListRequest)
        {
            try
            {
                SqlConnection sqlConnection = getSQLConnection();
                JobListResponse jobListResponse = new JobListResponse();

                JobDAL dal = new JobDAL();
                jobListResponse = dal.GetJobs(sqlConnection, jobListRequest);
                return Ok(jobListResponse);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        [Route("api/v1/jobs/{id}")]
        public IHttpActionResult GetJobDetails(int id)
        {
            try
            {
                SqlConnection sqlConnection = getSQLConnection();
                JobDAL dal = new JobDAL();
                JobDetailsResponse jobDetailsResponse = dal.GetJobDetails(sqlConnection, id);

                if (jobDetailsResponse == null || jobDetailsResponse.Id == 0)
                {
                    return NotFound();
                }

                return Ok(jobDetailsResponse);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


    }
}
