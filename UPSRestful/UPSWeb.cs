using Newtonsoft.Json;
using RestSharp;
using ITLHealthWeb.UPSRestful.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace ITLHealthWeb.UPSRestful
{
   /// <summary>
   /// Accesses and exposes UPS RESTful Web API functionality for tracking.
   /// Based on SIDITL/UPSRestful/UPSWeb.cs
   /// </summary>
   public class UPSWeb
   {
      internal DBAccess _DB;
      internal NetEncrypt.Encrypter _En;
      internal UPSCred _Cred;

      public string AccountNo { get; set; } = "";

      /// <summary>
      /// UPS RESTful Web API constructor.
      /// </summary>
      /// <param name="iBusID">Business ID</param>
      /// <param name="iSiteID">Site ID</param>
      public UPSWeb(int iBusID, int iSiteID)
      {
         try
         {
            DataTable dt = new DataTable();
            int iDelMode = 4; // UPS
            string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
            _En = new NetEncrypt.Encrypter(encryptionKey);
            _DB = new DBAccess(encryptionKey);

            using (SqlConnection conn = _DB.GetConnection())
            {
               using (SqlCommand cmd = new SqlCommand("[dbo].[GetCredRestByBusIDDelModeSite]", conn)
               {
                  CommandType = CommandType.StoredProcedure,
                  CommandTimeout = _DB.CommandTimeout
               })
               {
                  cmd.Parameters.Add(new SqlParameter() { ParameterName = "@BusID", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = iBusID });
                  cmd.Parameters.Add(new SqlParameter() { ParameterName = "@DelModeID", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = iDelMode });
                  cmd.Parameters.Add(new SqlParameter() { ParameterName = "@SiteID", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = iSiteID });
                  
                  using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                     da.Fill(dt);
               }

               if (dt.Rows.Count > 0)
               {
                  _Cred = new UPSCred(
                     dt.Rows[0]["Url"].ToString(),
                     dt.Rows[0]["RedirectUri"].ToString(),
                     dt.Rows[0]["AccountNo"].ToString(),
                     _En.Decrypt(dt.Rows[0]["client_id"].ToString()),
                     _En.Decrypt(dt.Rows[0]["client_secret"].ToString()),
                     dt.Rows[0]["refresh_token"].ToString(),
                     Convert.ToDateTime(dt.Rows[0]["TokenTimeout"]),
                     (Guid)dt.Rows[0]["ID"]
                  );

                  AccountNo = _Cred.AccountID;
               }
               else
               {
                  throw new Exception($"No UPS credentials found for BusID={iBusID}, SiteID={iSiteID}");
               }
            }
         }
         catch (Exception ex)
         {
            Debug.WriteLine($"Error in UPSWeb Constructor: {ex.Message}");
            throw;
         }
      }

      /// <summary>
      /// Get OAuth access token from UPS
      /// </summary>
      internal object GetAccessToken()
      {
         try
         {
            string token = "";

            RestClient client = new RestClient(_Cred.BaseUri);
            RestRequest request = new RestRequest("security/v1/oauth/token", Method.Post)
            {
               Timeout = new TimeSpan(0, 0, 10)
            };
            
            request.AddHeader("x-merchant-id", _Cred.AccountID);
            string base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_Cred.ClientKey}:{_Cred.ClientSecret}"));
            request.AddHeader("Authorization", $"Basic {base64String}");
            request.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);

            RestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
               Debug.WriteLine(response.Content);
               UPSAccessTokenResponseModel result = JsonConvert.DeserializeObject<UPSAccessTokenResponseModel>(response.Content);
               if (result != null && result.status.Equals("approved"))
               {
                  token = result.access_token;
                  _Cred.AccessToken = token;
               }
               else
               {
                  return new Exception("UPS access token not approved");
               }
            }
            else
            {
               Debug.WriteLine($"Error: {response.ErrorMessage ?? ""}");
               return new Exception($"Failed to get UPS access token: {response.ErrorMessage}");
            }
            
            return token;
         }
         catch (Exception ex)
         {
            return ex;
         }
      }

      /// <summary>
      /// Get UPS tracking information
      /// Based on SIDITL/UPSRestful/UPSWeb.cs lines 198-243
      /// </summary>
      public object UPSGetTrackingInfo(string TransRef, string TrackingNo)
      {
         try
         {
            object auth = GetAccessToken();
            if (auth is string token)
            {
               string resource = $"api/track/v1/details/{TrackingNo}?locale=en_US&returnSignature=false&returnMilestones=true&returnPOD=false";
               RestClient client = new RestClient(_Cred.BaseUri);
               RestRequest request = new RestRequest(resource, Method.Get)
               {
                  Timeout = new TimeSpan(0, 0, 10)
               };

               // Add headers
               request.AddHeader("transId", TransRef);
               request.AddHeader("transactionSrc", "ITLHealthWeb application");
               request.AddHeader("Authorization", $"Bearer {token}");

               // Execute the request
               RestResponse response = client.Execute(request);
               if (response.StatusCode == HttpStatusCode.OK)
               {
                  Debug.WriteLine(response.Content);
                  return JsonConvert.DeserializeObject<UPSTrackingInfoResponseModel>(response.Content);
               }
               else
               {
                  Debug.WriteLine(response.Content);
                  return response.Content;
               }
            }
            else if (auth is Exception authErr)
            {
               return authErr;
            }
            else
            {
               return new Exception("Could not generate access token for UPS account");
            }
         }
         catch (Exception ex)
         {
            return ex;
         }
      }
   }

   /// <summary>
   /// UPS Credentials class
   /// </summary>
   internal class UPSCred
   {
      public string BaseUri { get; set; }
      public string RedirectUri { get; set; }
      public string AccountID { get; set; }
      public string ClientKey { get; set; }
      public string ClientSecret { get; set; }
      public string RefreshToken { get; set; }
      public DateTime ExpireTime { get; set; }
      public Guid DBID { get; set; }
      public string AccessToken { get; set; }

      public UPSCred(string baseUri, string redirectUri, string accountId, string clientKey, 
                     string clientSecret, string refreshToken, DateTime expireTime, Guid dbId)
      {
         BaseUri = baseUri;
         RedirectUri = redirectUri;
         AccountID = accountId;
         ClientKey = clientKey;
         ClientSecret = clientSecret;
         RefreshToken = refreshToken;
         ExpireTime = expireTime;
         DBID = dbId;
      }
   }

   /// <summary>
   /// UPS Access Token Response Model
   /// </summary>
   public class UPSAccessTokenResponseModel
   {
      public string access_token { get; set; }
      public string token_type { get; set; }
      public string issued_at { get; set; }
      public string client_id { get; set; }
      public string expires_in { get; set; }
      public string status { get; set; }
   }
}
