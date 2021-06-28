using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace CoWin.Telemetry
{
    public static class PersistenceFunction
    {
        [FunctionName("storage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                log.LogInformation($"Received Request for Persisting Data. Details: {requestBody}");
                var telemetryModel = JsonConvert.DeserializeObject<TelemetryModel>(requestBody);

                using (var connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    var command = new SqlCommand
                    {
                        CommandText = "dbo.Usp_AddTelemetryData",
                        CommandType = CommandType.StoredProcedure,
                        Connection = connection
                    };
                    command.Parameters.AddWithValue("@UniqueId", telemetryModel.UniqueId);
                    command.Parameters.AddWithValue("@AppVersion", telemetryModel.AppVersion);
                    command.Parameters.AddWithValue("@Source", telemetryModel.Source);
                    command.Parameters.AddWithValue("@BookedOn", telemetryModel.BookedOn);
                    command.Parameters.AddWithValue("@TimeTakenToBookInSeconds", telemetryModel.TimeTakenToBookInSeconds);
                    command.Parameters.AddWithValue("@CaptchaMode", telemetryModel.CaptchaMode);
                    command.Parameters.AddWithValue("@Latitude", telemetryModel.Latitude);
                    command.Parameters.AddWithValue("@Longitude", telemetryModel.Longitude);
                    command.Parameters.AddWithValue("@PinCode", telemetryModel.PINCode);
                    command.Parameters.AddWithValue("@District", telemetryModel.District);
                    command.Parameters.AddWithValue("@State", telemetryModel.State);
                    command.Parameters.AddWithValue("@BeneficiaryCount", telemetryModel.BeneficiaryCount);
                    command.Parameters.AddWithValue("@MinimumAge", telemetryModel.MinimumAge);
                    command.Parameters.AddWithValue("@MaximumAge", telemetryModel.MaximumAge);
                    command.ExecuteNonQuery();
                    log.LogInformation("Data Successfully Persisted");

                }
            }
            catch (Exception e)
            {
                log.LogError($"Error While Persisting Data. Details: {e}");
                return new BadRequestResult();
            }

            return new OkResult();
        }
    }
}
