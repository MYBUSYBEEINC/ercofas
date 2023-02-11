using ERCOFAS.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;

namespace ERCOFAS.Helpers
{
    /// <summary>
    /// The helper class for SMS (Short Message Service).
    /// </summary>
    public static class SMSHelpers
    {
        private static readonly DefaultDBContext _db = new DefaultDBContext();

        /// <summary>
        /// Sends one time password to target mobile number.
        /// </summary>
        /// <param name="mobileNumber">The mobile number.</param>
        /// <param name="content">The content.</param>
        public static bool SendOneTimePassword(string mobileNumber, string content)
        {
            var settings = _db.Settings.FirstOrDefault(x => x.Id == "FA85FB3A-2A1E-47F8-9B76-6536F6A95ABB");

            if (settings == null)
                return false;

            var client = new RestClient(string.Format("{0}/SendSMS", settings.OTPBaseUrl));
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", JsonConvert.SerializeObject(new OneTimePasswordModel()
            {
                SenderId = settings.OTPSenderId.Trim(),
                ApiKey = settings.OTPAPIKey,
                ClientId = settings.OTPClientId,
                Message = content,
                MobileNumbers = mobileNumber
            }), ParameterType.RequestBody);

            var restResponse = client.ExecutePost(request);

            if (!restResponse.IsSuccessful && restResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (restResponse.ErrorMessage != null)
                    throw new ArgumentException(restResponse.ErrorMessage);

                return false;
            }
            return true;
        }
    }
}