using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace JobsApplication.Models
{
    public class LocationDAL
    {
        public int CreateLocation(SqlConnection sqlConnection, CreateLocationRequest createLocationRequest)
        {
            string query = @"INSERT INTO Location(title, city, state, country, zip)
                     OUTPUT INSERTED.Id
                     VALUES (@Title, @City, @State, @Country, @Zip)";

            using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
            {
                cmd.Parameters.AddWithValue("@Title", createLocationRequest.Title);
                cmd.Parameters.AddWithValue("@City", createLocationRequest.City);
                cmd.Parameters.AddWithValue("@State", createLocationRequest.State);
                cmd.Parameters.AddWithValue("@Country", createLocationRequest.Country);
                cmd.Parameters.AddWithValue("@Zip", createLocationRequest.Zip);

                sqlConnection.Open();
                int newJobId = (int)cmd.ExecuteScalar();
                sqlConnection.Close();

                return newJobId;
            }
        }
        public int UpdateLocation(SqlConnection sqlConnection, int locationId, UpdateLocationRequest updateLocationRequest)
        {
            string query = @"UPDATE Location SET title = @Title, city = @City, state = @State, country = @Country, zip = @Zip WHERE id = @LocationId";

            using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
            {
                cmd.Parameters.AddWithValue("@Title", updateLocationRequest.Title);
                cmd.Parameters.AddWithValue("@City", updateLocationRequest.City);
                cmd.Parameters.AddWithValue("@State", updateLocationRequest.State);
                cmd.Parameters.AddWithValue("@Country", updateLocationRequest.Country);
                cmd.Parameters.AddWithValue("@Zip", updateLocationRequest.Zip);
                cmd.Parameters.AddWithValue("@LocationId", locationId);

                sqlConnection.Open();
                int rowsUpdated = cmd.ExecuteNonQuery();
                sqlConnection.Close();

                return rowsUpdated;
            }
        }

        public LocationListResponse GetLocations(SqlConnection sqlConnection)
        {
            LocationListResponse locationListResponse = new LocationListResponse();

            string query = "SELECT Location.id, Location.title, Location.city, Location.state, Location.country, Location.zip FROM Location";
            SqlDataAdapter da = new SqlDataAdapter(query, sqlConnection);

            DataTable dt = new DataTable();

            da.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                List<Location> locationList = new List<Location>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Location location = new Location();
                    DataRow row = dt.Rows[i];
                    location.Id = Convert.ToInt32(row[0]);
                    location.Title = Convert.ToString(row[1]);
                    location.City = Convert.ToString(row[2]);
                    location.State = Convert.ToString(row[3]);
                    location.Country = Convert.ToString(row[4]);
                    location.Zip = Convert.ToInt32(row[5]);
                    locationList.Add(location);
                }
                locationListResponse.LocationList = locationList;
            }
            return locationListResponse;
        }
    }
}
