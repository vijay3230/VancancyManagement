using JobsApplication.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JobsApplication.Controllers
{
    public class LocationController : ApiController
    {
        private SqlConnection getSQLConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["JobsConnectionString"].ConnectionString);
        }

        [HttpPost]
        [Route("api/v1/locations")]
        public IHttpActionResult CreateLocation(CreateLocationRequest createLocationRequest)
        {
            try
            {
                SqlConnection sqlConnection = getSQLConnection();
                LocationDAL locationDAL = new LocationDAL();
                int newLocationId = locationDAL.CreateLocation(sqlConnection, createLocationRequest);

                string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                string resourceUrl = $"{baseUrl}/api/v1/locations/{newLocationId}";

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
        [Route("api/v1/locations/{id}")]
        public IHttpActionResult UpdateLocation(int id, UpdateLocationRequest updateLocationRequest)
        {
            try
            {
                SqlConnection sqlConnection = getSQLConnection();
                LocationDAL locationDAL = new LocationDAL();

                int updatedRows = locationDAL.UpdateLocation(sqlConnection, id, updateLocationRequest);
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
        [Route("api/v1/locations")]
        public IHttpActionResult GetLocations()
        {
            try
            {
                SqlConnection sqlConnection = getSQLConnection();
                LocationListResponse locationListResponse = new LocationListResponse();

                LocationDAL locationDAL = new LocationDAL();
                locationListResponse = locationDAL.GetLocations(sqlConnection);
                return Ok(locationListResponse);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
