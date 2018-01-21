using Clarifai.API;
using Clarifai.DTOs.Inputs;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace EcoSAN_Web.Controllers
{
    public class TrashPickupController : ApiController
    {

        [HttpGet]
        public Models.Table LeaderboardsAll()
        {
            var result = new Models.Table();
            result.Rows = new List<List<string>>();

            var dataAdapter = new SqlDataAdapter("", new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnectionString"].ConnectionString));

            dataAdapter.SelectCommand.CommandText = "Select Sum(P.Points) as Points, (Select Count(*) From Points Where DeviceID = P.DeviceID) as Count, " +
                            "(Select Top 1 DeviceName From Devices Where DeviceID = P.DeviceID) as DeviceName " +
                            "From Points as P " +
                            "Group By P.DeviceID ";


            var dt = new DataTable();

            dataAdapter.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                var newRow = new List<string>();
                newRow.Add(row["Points"].ToString());
                newRow.Add(row["Count"].ToString());
                newRow.Add(row["DeviceName"].ToString() == "" ? "" : row["DeviceName"].ToString());
                result.Rows.Add(newRow);
            }

            return result;
        }

        [HttpGet]
        public Models.Table LeaderboardsMonth()
        {
            var result = new Models.Table();
            result.Rows = new List<List<string>>();

            var dataAdapter = new SqlDataAdapter("", new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnectionString"].ConnectionString));

            dataAdapter.SelectCommand.CommandText = "Select Sum(P.Points) as Points, (Select Count(*) From Points Where DeviceID = P.DeviceID) as Count, " +
                            "(Select Top 1 DeviceName From Devices Where DeviceID = P.DeviceID) as DeviceName " +
                            "From Points as P " +
                            "Group By P.DeviceID" +
                            "Where P.TimeStamp > " + (DateTime.UtcNow.AddMonths(-1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var dt = new DataTable();

            dataAdapter.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                var newRow = new List<string>();
                newRow.Add(row["Points"].ToString());
                newRow.Add(row["Count"].ToString());
                newRow.Add(row["DeviceName"].ToString() == "" ? "" : row["DeviceName"].ToString());
                result.Rows.Add(newRow);
            }

            return result;
        }

        [HttpGet]
        public Models.Table LeaderboardsWeek()
        {
            var result = new Models.Table();
            result.Rows = new List<List<string>>();

            var dataAdapter = new SqlDataAdapter("", new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnectionString"].ConnectionString));

            dataAdapter.SelectCommand.CommandText = "Select Sum(P.Points) as Points, (Select Count(*) From Points Where DeviceID = P.DeviceID) as Count, " +
                            "(Select Top 1 DeviceName From Devices Where DeviceID = P.DeviceID) as DeviceName " +
                            "From Points as P " +
                            "Group By P.DeviceID" +
                            "Where P.TimeStamp > " + (DateTime.UtcNow.AddDays(-7).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var dt = new DataTable();

            dataAdapter.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                var newRow = new List<string>();
                newRow.Add(row["Points"].ToString());
                newRow.Add(row["Count"].ToString());
                newRow.Add(row["DeviceName"].ToString() == "" ? "" : row["DeviceName"].ToString());
                result.Rows.Add(newRow);
            }

            return result;
        }

        [HttpGet]
        public async Task<int> Points(string id)
        {
            using (var sqlCommand = new SqlCommand("", new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnectionString"].ConnectionString)))
            {
                sqlCommand.CommandText = "Select Sum(Points) From Devices Where DeviceID = @DeviceID";
                sqlCommand.Connection.Open();
                sqlCommand.Parameters.AddWithValue("@DeviceID", id);

                return (int)await sqlCommand.ExecuteScalarAsync();
            }
        }

        [HttpPost]
        public async Task<string> PickedUpTrash(Models.TrashPickupModel pickup)
        {
            if(pickup == null)
            {
                pickup = new Models.TrashPickupModel();
            }
            try
            {
                using (var sqlCommand = new SqlCommand("", new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnectionString"].ConnectionString)))
                {
                    sqlCommand.CommandText = "Select Count(*) From Devices Where DeviceID = @DeviceID";
                    sqlCommand.Connection.Open();
                    sqlCommand.Parameters.AddWithValue("@DeviceID", pickup.DeviceID);

                    var deviceCount = (int)await sqlCommand.ExecuteScalarAsync();

                    if (deviceCount == 0)
                    {
                        sqlCommand.CommandText = "Insert Into Devices (DeviceID,DeviceName) Values (@DeviceID,@DeviceName)";
                        sqlCommand.Parameters.AddWithValue("@DeviceName", pickup.DeviceName);
                    }
                    else
                    {
                        sqlCommand.CommandText = "Update Devices Set DeviceName = @DeviceName Where DeviceID = @DeviceID";
                        sqlCommand.Parameters.AddWithValue("@DeviceName", pickup.DeviceName);
                    }

                    await sqlCommand.ExecuteNonQueryAsync();

                    sqlCommand.CommandText = "Insert Into Picks (DeviceID,Latitude,Longitude,TimeStamp,Image) Values (@DeviceID,@Latitude,@Longitude,@TimeStamp,@Image)";

                    sqlCommand.Parameters.Clear();

                    sqlCommand.Parameters.AddWithValue("@DeviceID", pickup.DeviceID);
                    sqlCommand.Parameters.AddWithValue("@Latitude", pickup.Latitude);
                    sqlCommand.Parameters.AddWithValue("@Longitude", pickup.Longitude);
                    sqlCommand.Parameters.AddWithValue("@TimeStamp", pickup.TimeStamp);
                    sqlCommand.Parameters.AddWithValue("@Image", pickup.Image);

                    await sqlCommand.ExecuteScalarAsync();

                    var firebaseClient = new FirebaseClient("https://ecospan-6213f.firebaseio.com");

                    await firebaseClient.Child("Pickup").PostAsync<Models.Firebase.Pickup>(new Models.Firebase.Pickup()
                    {
                        DeviceID = pickup.DeviceID,
                        DeviceName = pickup.DeviceName,
                        TimeStamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                        Latitude = pickup.Latitude,
                        Longitude = pickup.Longitude
                    });

                    await InsertPointsSQL(new Models.Firebase.Point()
                    {
                        DeviceID = pickup.DeviceID,
                        DeviceName = pickup.DeviceName,
                        RecievedPoint = 20,
                        TimeStamp = pickup.TimeStamp
                    });

                    await InsertPoints(new Models.Firebase.Point()
                    {
                        DeviceID = pickup.DeviceID,
                        DeviceName = pickup.DeviceName,
                        RecievedPoint = 20,
                        TimeStamp = pickup.TimeStamp
                    });

                    var count = 1;
                    foreach (var phone in pickup.ConnectedDevices)
                    {
                        await InsertPoints(new Models.Firebase.Point()
                        {
                            DeviceID = phone,
                            DeviceName = pickup.DeviceName,
                            RecievedPoint = 20 / count,
                            TimeStamp = pickup.TimeStamp
                        });

                        await InsertPointsSQL(new Models.Firebase.Point()
                        {
                            DeviceID = phone,
                            DeviceName = pickup.DeviceName,
                            RecievedPoint = 20 / count,
                            TimeStamp = pickup.TimeStamp
                        });
                        count++;
                    }
                }
            }
            catch(Exception ex)
            {
                return ex.Message + ex.StackTrace + JsonConvert.SerializeObject(pickup);
            }

            return "";
        }

        public async static Task InsertPointsSQL(Models.Firebase.Point point)
        {
            using (var sqlCommand = new SqlCommand("", new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnectionString"].ConnectionString)))
            {
                sqlCommand.CommandText = "Insert Into Points (DeviceID,Points,TimeStamp) Values (@DeviceID,@Points,@TimeStamp)";
                await sqlCommand.Connection.OpenAsync();
                sqlCommand.Parameters.AddWithValue("@DeviceID", point.DeviceID);
                sqlCommand.Parameters.AddWithValue("@Points", point.RecievedPoint);
                sqlCommand.Parameters.AddWithValue("@TimeStamp", point.TimeStamp);

                await sqlCommand.ExecuteNonQueryAsync();
            }
        }

        [HttpPost]
        public async Task InsertPoints(Models.Firebase.Point point)
        {
            var firebaseClient = new FirebaseClient("https://ecospan-6213f.firebaseio.com");

            await firebaseClient.Child("Points").PostAsync<Models.Firebase.Point>(new Models.Firebase.Point()
            {
                DeviceID = point.DeviceID,
                RecievedPoint = point.RecievedPoint,
                TimeStamp = point.TimeStamp
            });
        }
    }
}
