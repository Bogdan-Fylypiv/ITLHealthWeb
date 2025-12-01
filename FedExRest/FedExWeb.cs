using Newtonsoft.Json;
using RestSharp;
using ITLHealthWeb.FedExRest.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace ITLHealthWeb.FedExRest
{
   /// <summary>
   /// Accesses and exposes FedEx RESTful Web API functionality for tracking.
   /// Based on SIDITL/FedExRest/FedExWeb.cs
   /// </summary>
   public class FedExWeb
   {
      internal string _FedExUrlTest = "https://apis-sandbox.fedex.com";
      internal string _FedExtUrlProduction = "https://apis.fedex.com";
      internal string _FedExtUrl = "";
      internal int _BusID = 0;
      internal bool IsProduction = false;
      internal FedExAuthCredRequest _Cred = null;
      internal DBAccess _DB = null;
      internal NetEncrypt.Encrypter _En = null;
      
      // Token caching fields
      internal Guid _CredRestID = Guid.Empty;
      internal string _CachedAccessTokenTrack = "";
      internal DateTime _TokenTimeoutTrack = DateTime.MinValue;

      public string AccountNo { get; set; } = "";

      /// <summary>
      /// FedEx RESTful Web API constructor.
      /// </summary>
      /// <param name="BusID">Business ID</param>
      /// <param name="SiteID">Site ID</param>
      public FedExWeb(int BusID, int SiteID)
      {
         try
         {
            DataTable dt = new DataTable();
            _BusID = BusID;
            string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
            _En = new NetEncrypt.Encrypter(encryptionKey);
            _DB = new DBAccess(encryptionKey);
            
            using (SqlConnection conn = _DB.GetConnection())
            {
               using (SqlCommand cmd = new SqlCommand("dbo.GetCredRestByBusSiteDelmode", conn)
               {
                  CommandType = CommandType.StoredProcedure,
                  CommandTimeout = _DB.CommandTimeout
               })
               {
                  cmd.Parameters.Add(new SqlParameter() { ParameterName = "@BusID", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = _BusID });
                  cmd.Parameters.Add(new SqlParameter() { ParameterName = "@SiteID", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = SiteID });
                  cmd.Parameters.Add(new SqlParameter() { ParameterName = "@DelModeID", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = 8 }); // 8 is FedEx
                  
                  using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                     ad.Fill(dt);
                  
                  if (dt.Rows.Count == 0)
                  {
                     throw new Exception($"There are no FedEx Rest credentials in the database for Business {_BusID} at site ID {SiteID}.");
                  }
               }
               
               DataRow dr = dt.Rows[0];
               IsProduction = !(bool)dr["IsSandbox"];
               _Cred = new FedExAuthCredRequest()
               {
                  GrantType = dr["Region"].ToString(),
                  ClientID = _En.Decrypt(dr["client_id"].ToString()),
                  ClientSecret = _En.Decrypt(dr["client_secret"].ToString()),
                  ClientIDTrack = _En.Decrypt(dr["client_id_track"].ToString()),
                  ClientSecretTrack = _En.Decrypt(dr["client_secret_track"].ToString()),
                  url = dr["Url"].ToString()
               };
               AccountNo = dr["AccountNo"].ToString();
               _FedExtUrl = dr["Url"].ToString();
               
               // Load cached token values (tokens are stored encrypted in database)
               _CredRestID = (Guid)dr["ID"];
               if (dr["refresh_token_track"] != DBNull.Value && !string.IsNullOrEmpty(dr["refresh_token_track"].ToString()))
               {
                  // Store the encrypted token as-is (will decrypt when using)
                  _CachedAccessTokenTrack = dr["refresh_token_track"].ToString();
               }
               if (dr["TokenTimeoutTrack"] != DBNull.Value)
               {
                  _TokenTimeoutTrack = Convert.ToDateTime(dr["TokenTimeoutTrack"]);
               }
            }
         }
         catch (Exception ex)
         {
            Debug.WriteLine($"FedExWeb Constructor Error: {ex.Message}");
            throw;
         }
      }

      /// <summary>
      /// Get OAuth access token for tracking operations
      /// Based on SIDITL/FedExRest/FedExWeb.cs lines 341-420
      /// Includes token caching and database updates
      /// </summary>
      internal object GetAccessTokenTrack()
      {
         try
         {
            string AccessToken = "";
            
            // Check if we have a cached token that's still valid
            // Token timeout check: if expired or not set, get new token
            if (_TokenTimeoutTrack <= DateTime.Now)
            {
               // Need to get a new token
               if (_Cred == null)
               {
                  throw new Exception("FedEx credentials must first be populated");
               }

               string cred = $"grant_type={_Cred.GrantType}&client_id={_Cred.ClientIDTrack}&client_secret={_Cred.ClientSecretTrack}";

               RestClient client = new RestClient(_FedExtUrl);
               RestRequest request = new RestRequest("oauth/token", Method.Post)
               {
                  Timeout = new TimeSpan(0, 0, 10)
               };

               request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
               request.AddParameter("application/x-www-form-urlencoded", cred, ParameterType.RequestBody);

               RestResponse response = client.Execute(request);

               if (response != null && response.StatusCode == HttpStatusCode.OK)
               {
                  FedExAuthCredResponse result = JsonConvert.DeserializeObject<FedExAuthCredResponse>(response.Content);
                  AccessToken = result.AccessToken;
                  
                  // Set token timeout to 55 minutes from now (5 minutes before actual expiration)
                  int timeOut = result.ExpiresIn / 60 - 5;
                  _TokenTimeoutTrack = DateTime.Now.AddMinutes(timeOut);
                  
                  // Update database with new token (encrypted)
                  using (SqlConnection conn = _DB.GetConnection())
                  {
                     using (SqlCommand cmd = new SqlCommand("dbo.UpdCredRestAccessTokenTrack", conn)
                     {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = _DB.CommandTimeout
                     })
                     {
                        cmd.Parameters.Add(new SqlParameter() 
                        { 
                           ParameterName = "@CredRestID", 
                           SqlDbType = SqlDbType.UniqueIdentifier, 
                           Direction = ParameterDirection.Input, 
                           Value = _CredRestID 
                        });
                        cmd.Parameters.Add(new SqlParameter() 
                        { 
                           ParameterName = "@AccessToken", 
                           SqlDbType = SqlDbType.VarChar, 
                           Direction = ParameterDirection.Input, 
                           Value = _En.Encrypt(AccessToken) 
                        });
                        cmd.Parameters.Add(new SqlParameter() 
                        { 
                           ParameterName = "@TokenTimeout", 
                           SqlDbType = SqlDbType.DateTime, 
                           Direction = ParameterDirection.Input, 
                           Value = _TokenTimeoutTrack 
                        });
                        cmd.ExecuteNonQuery();
                     }
                  }
                  
                  // Cache the encrypted token (matching WinForms line 396)
                  _CachedAccessTokenTrack = _En.Encrypt(AccessToken);
               }
               else
               {
                  string errorMsg = $"FedEx OAuth Error - Status: {response.StatusCode}";
                  if (!string.IsNullOrEmpty(response.Content))
                  {
                     errorMsg += $"\nResponse: {response.Content}";
                  }
                  if (response.ErrorException != null)
                  {
                     errorMsg += $"\nException: {response.ErrorException.Message}";
                  }
                  Debug.WriteLine(errorMsg);
                  return new Exception(errorMsg);
               }
            }
            else
            {
               // Use cached token - decrypt it (matching WinForms line 411)
               AccessToken = _En.Decrypt(_CachedAccessTokenTrack);
            }

            return AccessToken;
         }
         catch (Exception ex)
         {
            return ex;
         }
      }

      /// <summary>
      /// Get FedEx tracking information
      /// Based on SIDITL/FedExRest/FedExWeb.cs lines 815-882
      /// </summary>
      public object GetTrackingByTrackingNumberRequest(string TrackingNumber, string CarrierCode, DateTime StartDate, string CustTransID = "")
      {
         try
         {
            object Auth = GetAccessTokenTrack();
            if (Auth is string token)
            {
               FedExTrackingByTrackingNumberRequest req = new FedExTrackingByTrackingNumberRequest()
               {
                  IncludeDetailedScans = true,
                  TrackingInfo = new List<TrackingInfo>()
                  {
                     new TrackingInfo()
                     {
                        ShipDateBegin = StartDate.ToString("yyyy-MM-dd"),
                        ShipDateEnd = DateTime.Today.ToString("yyyy-MM-dd"),
                        TrackingNumberInfo = new TrackingNumberInfo()
                        {
                           CarrierCode = CarrierCode,
                           TrackingNumber = TrackingNumber
                        }
                     }
                  }
               };

               RestClient client = new RestClient(_FedExtUrl);
               RestRequest request = new RestRequest("track/v1/trackingnumbers", Method.Post)
               {
                  Timeout = new TimeSpan(0, 0, 10)
               };
               
               request.AddHeader("Authorization", $"Bearer {token}");
               request.AddHeader("X-locale", "en_US");
               request.AddHeader("Content-Type", "application/json");
               
               if (!string.IsNullOrEmpty(CustTransID))
               {
                  request.AddHeader("x-customer-transaction-id", CustTransID);
               }

               request.AddParameter("application/json", JsonConvert.SerializeObject(req), ParameterType.RequestBody);
               
               RestResponse response = client.Execute(request);
               
               if (response.StatusCode == HttpStatusCode.OK)
               {
                  if (response.Content.Contains("\"errors\": ["))
                  {
                     return new Exception(response.Content.ToString());
                  }
                  else
                  {
                     FedExTrackingResponse result = JsonConvert.DeserializeObject<FedExTrackingResponse>(response.Content);
                     return result;
                  }
               }
               else if (response.StatusCode == HttpStatusCode.Unauthorized)
               {
                  // Token is invalid - force refresh by resetting timeout
                  _TokenTimeoutTrack = DateTime.MinValue;
                  _CachedAccessTokenTrack = "";
                  string errorMsg = $"FedEx API returned 401 Unauthorized. Token has been invalidated and will refresh on next request.\nResponse: {response.Content}";
                  Debug.WriteLine(errorMsg);
                  return new Exception(errorMsg);
               }
               else if (response.ErrorException != null)
               {
                  string errorMsg = $"FedEx Tracking Error - Status: {response.StatusCode}\nException: {response.ErrorException.Message}";
                  Debug.WriteLine(errorMsg);
                  return new Exception(errorMsg);
               }
               else
               {
                  string errorMsg = $"FedEx Tracking Error - Status: {response.StatusCode}\nResponse: {response.Content}";
                  Debug.WriteLine(errorMsg);
                  return new Exception(errorMsg);
               }
            }
            else
            {
               return new Exception("Could not retrieve an access token for the FedEx tracking API call.");
            }
         }
         catch (Exception ex)
         {
            return ex;
         }
      }
   }

   /// <summary>
   /// FedEx Authentication Credentials Request
   /// </summary>
   internal class FedExAuthCredRequest
   {
      public string GrantType { get; set; }
      public string ClientID { get; set; }
      public string ClientSecret { get; set; }
      public string ClientIDTrack { get; set; }
      public string ClientSecretTrack { get; set; }
      public string url { get; set; }
   }

   /// <summary>
   /// FedEx Authentication Response
   /// </summary>
   public class FedExAuthCredResponse
   {
      [JsonProperty("access_token")]
      public string AccessToken { get; set; }

      [JsonProperty("token_type")]
      public string TokenType { get; set; }

      [JsonProperty("expires_in")]
      public int ExpiresIn { get; set; }

      [JsonProperty("scope")]
      public string Scope { get; set; }
   }

   /// <summary>
   /// FedEx Tracking Request Model
   /// </summary>
   public class FedExTrackingByTrackingNumberRequest
   {
      [JsonProperty("includeDetailedScans")]
      public bool IncludeDetailedScans { get; set; }

      [JsonProperty("trackingInfo")]
      public List<TrackingInfo> TrackingInfo { get; set; }
   }

   public class TrackingInfo
   {
      [JsonProperty("shipDateBegin")]
      public string ShipDateBegin { get; set; }

      [JsonProperty("shipDateEnd")]
      public string ShipDateEnd { get; set; }

      [JsonProperty("trackingNumberInfo")]
      public TrackingNumberInfo TrackingNumberInfo { get; set; }
   }
}
