//using SID.CanparTracking;
//using SID.Classes;
//using SID.PurolatorTrackingSvc;
using ITLHealthWeb.UPSRestful;
using ITLHealthWeb.UPSRestful.Models;
using ITLHealthWeb.FedExRest;
using ITLHealthWeb.FedExRest.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITLHealthWeb.Pages
{
   public partial class Tracking : System.Web.UI.Page
   {
      //internal DBAccess _DB = new DBAccess(Program.Key);
      internal DBAccess _DB = new DBAccess("");
      internal DataTable _dtTrack;
      internal DataTable _dtItems;
      internal string _sTrackingNumber = "";
      internal int _iBusID = 1;
      internal int _iSiteID = 1;
      internal int _iDelMode = 4; // Default to UPS
      internal Guid _gOrdID;
      //internal SID.Classes.Scan _Scan;

      protected void Page_Load(object sender, EventArgs e)
      {
         try
         {
            if (!IsPostBack)
            {
               // Get tracking number from query string
               _sTrackingNumber = Request.QueryString["trackingNo"];

               // Initialize data tables
               InitializeDataTables();

               if (string.IsNullOrEmpty(_sTrackingNumber))
               {
                  // Load mock data for demo
                  LoadMockData();
                  return;
               }

               // Initialize Scan object for carrier API calls
               //_Scan = new SID.Classes.Scan(System.Windows.Forms.Cursors.Default);

               // Load tracking data
               SetData(_sTrackingNumber);
            }
         }
         catch (Exception ex)
         {
            ShowMessage($"Error loading tracking: {ex.Message}", "error");
            System.Diagnostics.Debug.WriteLine($"Page_Load Error: {ex.Message}");
         }
      }

      private void InitializeDataTables()
      {
         _dtTrack = new DataTable("Track");
         _dtTrack.Columns.Add("ID", typeof(int));
         _dtTrack.Columns.Add("TrackingNo", typeof(string));
         _dtTrack.Columns.Add("Status", typeof(string));
         _dtTrack.Columns.Add("ActivityDate", typeof(DateTime));
         _dtTrack.Columns.Add("Notes", typeof(string));
         _dtTrack.Columns.Add("Level", typeof(int));
         _dtTrack.AcceptChanges();

         _dtItems = new DataTable("Item");
         _dtItems.Columns.Add("ID", typeof(int));
         _dtItems.Columns.Add("TrackingNo", typeof(string));
         _dtItems.Columns.Add("Status", typeof(string));
         _dtItems.Columns.Add("ActivityDate", typeof(DateTime));
         _dtItems.Columns.Add("Notes", typeof(string));
         _dtItems.Columns.Add("Level", typeof(int));
         _dtItems.AcceptChanges();
      }

      /// <summary>
      /// Load tracking data based on tracking number
      /// Based on frmTracking.cs SetData method (lines 76-170)
      /// </summary>
      private void SetData(string sTrackingNumber)
      {
         DataTable dt = new DataTable();
         try
         {
            _sTrackingNumber = sTrackingNumber;

            // Clear existing data
            if (_dtTrack.Rows.Count > 0)
            {
               _dtItems.Rows.Clear();
               _dtTrack.Rows.Clear();
            }

            // Get label information from database
            using (SqlConnection con = _DB.GetConnection())
            {
               using (SqlCommand cmd = new SqlCommand("[dbo].[GetLabelByTrackingNo]", con)
               {
                  CommandTimeout = _DB.CommandTimeout,
                  CommandType = CommandType.StoredProcedure
               })
               {
                  cmd.Parameters.Add(new SqlParameter()
                  {
                     ParameterName = "@TrackingNo",
                     SqlDbType = SqlDbType.VarChar,
                     Size = 25,
                     Direction = ParameterDirection.Input,
                     Value = _sTrackingNumber
                  });

                  using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                  {
                     ad.Fill(dt);
                  }
               }
            }

            if (dt.Rows.Count > 0)
            {
               _iBusID = 1;
               _iSiteID = Convert.ToInt32(dt.Rows[0]["SiteID"]);
               _iDelMode = Convert.ToInt32(dt.Rows[0]["DelMode"]);
               _gOrdID = (Guid)dt.Rows[0]["OrdID"];
            }

            // Set tracking number
            TxtTrackingNo.Text = _sTrackingNumber;

            // Call appropriate carrier tracking method based on DelMode
            // DelMode values: 4=UPS, 8=FedEx, 64=Purolator, 512=CanadaPost, 2048=CanPar, 4096=Loomis, 32=USPS, 2=CPU
            if ((_iDelMode & 4) == 4) // UPS
            {
               BtnWebSite.Text = "View on UPS Website";
               GetUPSTrackInfo();
            }
            else if (_iDelMode == 8) // FedEx
            {
               BtnWebSite.Text = "View on FedEx Website";
               GetFedExTrackInfo(dt);
            }
            else if (_iDelMode == 64) // Purolator
            {
               BtnWebSite.Text = "View on Purolator Website";
               GetPurolatorTrackInfo(dt);
            }
            else if (_iDelMode == 512) // Canada Post
            {
               BtnWebSite.Text = "View on Canada Post Website";
               GetCaPostTrackInfo(_sTrackingNumber, _iBusID, _iSiteID, _iDelMode);
            }
            else if (_iDelMode == 2048) // CanPar
            {
               BtnWebSite.Text = "View on CanPar Website";
               GetCanparTrackInfo(dt);
            }
            else if (_iDelMode == 4096) // Loomis
            {
               BtnWebSite.Text = "View on Loomis Website";
               GetLoomisTrackInfo(_sTrackingNumber, _iBusID, _iSiteID, _iDelMode);
            }
            else if (_iDelMode == 32) // USPS
            {
               BtnWebSite.Text = "View on USPS Website";
               GetUSPSTrackInfo(_sTrackingNumber, _iBusID, _iSiteID, _iDelMode);
            }
            else if (_iDelMode == 2) // CPU
            {
               BtnWebSite.Text = "CPU";
               GetCPU(dt);
            }

            // Bind tracking activity to grid
            GvActivity.DataSource = _dtItems;
            GvActivity.DataBind();
         }
         catch (Exception ex)
         {
            ShowMessage($"❌ Error: {ex.Message}", "error");
            System.Diagnostics.Debug.WriteLine($"SetData Error: {ex.Message}");
         }
      }

      // Add carrier-specific tracking methods (stubs - implement based on frmTracking.cs)
      // See frmTracking.cs lines 664-794 for GetUPSTrackInfo implementation
      // See frmTracking.cs lines 529-662 for GetFedExTrackInfo implementation
      // See frmTracking.cs lines 319-381 for GetPurolatorTrackInfo implementation
      // See frmTracking.cs lines 413-503 for GetCanparTrackInfo implementation

      /// <summary>
      /// Get UPS tracking information
      /// Based on frmTracking.cs lines 664-794
      /// </summary>
      private void GetUPSTrackInfo()
      {
         DataRow dr;
         try
         {
            UPSWeb ups = new UPSWeb(_iBusID, _iSiteID);
            object response = ups.UPSGetTrackingInfo(_gOrdID.ToString(), _sTrackingNumber);
            
            if (response is UPSTrackingInfoResponseModel res)
            {
               TrackShipment rspShipment = res.TrackResponse.Shipment[0];
               TxtTrackingNo.Text = rspShipment.InquiryNumber;
               TxtShipperAcctNo.Text = ups.AccountNo;
               TxtServiceType.Text = rspShipment.Package[0].Service.Description;
               
               // Set last location
               if (rspShipment.Package[0].DeliveryInformation != null && 
                   rspShipment.Package[0].DeliveryInformation.ReceivedBy != null)
               {
                  TxtLastLocation.Text = "Signed by " + rspShipment.Package[0].DeliveryInformation.ReceivedBy + "\r\n";
               }
               else
               {
                  TxtLastLocation.Text = rspShipment.Package[0].Activity[0].Location.Address.City + " "
                     + rspShipment.Package[0].Activity[0].Location.Address.StateProvince + " "
                     + (rspShipment.Package[0].Activity[0].Location.Address.CountryCode ?? 
                        rspShipment.Package[0].Activity[0].Location.Address.Country);
               }
               
               // Set status
               TxtStatus.Text = rspShipment.Package[0].Activity[0].Status.Description;
               
               // Set PO and Order ID from reference numbers
               if (rspShipment.Package.Count > 0 && rspShipment.Package[0].ReferenceNumber != null)
               {
                  if (rspShipment.Package.Count == 1)
                  {
                     TxtOrderID.Text = rspShipment.Package[0].ReferenceNumber[0].Number;
                  }
                  else
                  {
                     TxtPO.Text = rspShipment.Package[0].ReferenceNumber[0].Number;
                  }
               }
               else
               {
                  TxtPO.Text = "";
               }
               
               if (rspShipment.Package.Count > 1 && rspShipment.Package[1].ReferenceNumber != null)
               {
                  TxtOrderID.Text = rspShipment.Package[0].ReferenceNumber[1].Number;
                  if (TxtOrderID.Text.Equals(TxtPO.Text))
                  {
                     TxtPO.Text = "";
                  }
               }
               
               // Set delivery date
               if (rspShipment.Package[0].DeliveryTime == null)
               {
                  string dateStr = rspShipment.Package[0].Activity[0].Date;
                  string timeStr = rspShipment.Package[0].Activity[0].Time;
                  DateTime activityDate = DateTime.Parse($"{dateStr.Substring(0, 4)}-{dateStr.Substring(4, 2)}-{dateStr.Substring(6, 2)} " +
                     $"{timeStr.Substring(0, 2)}:{timeStr.Substring(2, 2)}:{timeStr.Substring(4, 2)}");
                  TxtDeliveryDate.Text = activityDate.ToString("dd-MMM-yyyy");
               }
               else
               {
                  string dateStr = rspShipment.Package[0].DeliveryDate[0].Date;
                  string timeStr = rspShipment.Package[0].DeliveryTime.EndTime;
                  DateTime deliveryDate = DateTime.Parse($"{dateStr.Substring(0, 4)}-{dateStr.Substring(4, 2)}-{dateStr.Substring(6, 2)} " +
                     $"{timeStr.Substring(0, 2)}:{timeStr.Substring(2, 2)}:{timeStr.Substring(4, 2)}");
                  TxtDeliveryDate.Text = deliveryDate.ToString("dd-MMM-yyyy");
               }
               
               // Set ship to address
               int iShipmentAddressCnt = rspShipment.Package[0].PackageAddress.Count;
               PackageAddress shpAddress = rspShipment.Package[0].PackageAddress[iShipmentAddressCnt - 1];
               
               if (shpAddress.Address.AddressLine1 != null && shpAddress.Address.AddressLine1.Length > 0)
               {
                  TxtShipToAddress.Text += $"{shpAddress.Address.AddressLine1}\r\n";
               }
               if (shpAddress.Address.AddressLine2 != null && shpAddress.Address.AddressLine2.Length > 0)
               {
                  TxtShipToAddress.Text += $"{shpAddress.Address.AddressLine2}\r\n";
               }
               if (shpAddress.Address.AddressLine3 != null && shpAddress.Address.AddressLine3.Length > 0)
               {
                  TxtShipToAddress.Text += $"{shpAddress.Address.AddressLine3}\r\n";
               }
               
               TxtShipToAddress.Text += shpAddress.Address.City
                  + " " + shpAddress.Address.StateProvince
                  + " " + shpAddress.Address.PostalCode
                  + " " + shpAddress.Address.Country;
               
               // Build tracking activity grid
               int j = 0;
               int k = 0;
               foreach (TrackPackage pkg in rspShipment.Package)
               {
                  j++;
                  dr = _dtTrack.NewRow();
                  dr["ID"] = j.ToString();
                  dr["TrackingNo"] = pkg.TrackingNumber;
                  dr["Status"] = pkg.Activity[0].Status.Description ?? "";
                  string pkgDateStr = pkg.Activity[0].Date;
                  string pkgTimeStr = pkg.Activity[0].Time;
                  dr["ActivityDate"] = DateTime.Parse($"{pkgDateStr.Substring(0, 4)}-{pkgDateStr.Substring(4, 2)}-{pkgDateStr.Substring(6, 2)} " +
                     $"{pkgTimeStr.Substring(0, 2)}:{pkgTimeStr.Substring(2, 2)}:{pkgTimeStr.Substring(4, 2)}");
                  dr["Notes"] = "";
                  dr["Level"] = 0;
                  _dtTrack.Rows.Add(dr);
                  
                  foreach (TrackActivity item in pkg.Activity)
                  {
                     k++;
                     dr = _dtItems.NewRow();
                     dr["ID"] = k;
                     dr["TrackingNo"] = pkg.TrackingNumber;
                     dr["Status"] = item.Status.Description;
                     string itemDateStr = item.Date;
                     string itemTimeStr = item.Time;
                     dr["ActivityDate"] = DateTime.Parse($"{itemDateStr.Substring(0, 4)}-{itemDateStr.Substring(4, 2)}-{itemDateStr.Substring(6, 2)} " +
                        $"{itemTimeStr.Substring(0, 2)}:{itemTimeStr.Substring(2, 2)}:{itemTimeStr.Substring(4, 2)}");
                     dr["Notes"] = "";
                     dr["Level"] = 1;
                     _dtItems.Rows.Add(dr);
                  }
               }
            }
            else
            {
               TxtStatus.Text = "Invalid UPS Tracking Number";
               ShowMessage("Invalid UPS Tracking Number", "error");
               return;
            }
         }
         catch (Exception ex)
         {
            ShowMessage($"Error getting UPS tracking info: {ex.Message}", "error");
            System.Diagnostics.Debug.WriteLine($"GetUPSTrackInfo Error: {ex.Message}");
         }
      }
      /// <summary>
      /// Get FedEx tracking information
      /// Based on frmTracking.cs lines 529-662
      /// </summary>
      private void GetFedExTrackInfo(DataTable dt)
      {
         DataRow dr;
         try
         {
            if (dt.Rows.Count == 0)
            {
               return;
            }
            
            dr = dt.Rows[0];
            
            // Determine carrier code based on service type
            string sCarrierCode = "";
            if (dr["ServiceType"].ToString() == "FEDEX_GROUND")
            {
               sCarrierCode = "FDXG";
            }
            else
            {
               sCarrierCode = "FDXE";
            }
            
            // Call FedEx tracking API
            FedExWeb fedex = new FedExWeb(_iBusID, _iSiteID);
            object Res = fedex.GetTrackingByTrackingNumberRequest(
               _sTrackingNumber, 
               sCarrierCode, 
               Convert.ToDateTime(dr["dtmCreate"]), 
               dr["PONumber"].ToString()
            );
            
            if (Res is FedExTrackingResponse rsp)
            {
               TrackResult trackResult = rsp.Output.CompleteTrackResults[0].TrackResults[0];
               
               // Set shipper account (carrier code)
               TxtShipperAcctNo.Text = trackResult.TrackingNumberInfo.CarrierCode;
               
               // Set service type
               if (trackResult.ServiceDetail != null)
               {
                  TxtServiceType.Text = trackResult.ServiceDetail.Description;
               }
               else
               {
                  TxtServiceType.Text = dt.Rows[0]["ServiceType"].ToString();
               }
               
               // Set PO number
               TxtPO.Text = rsp.CustomerTransactionId ?? dr["PONumber"].ToString();
               
               // Set status
               TxtStatus.Text = trackResult.LatestStatusDetail.Description;
               
               // Set delivery date
               if (trackResult.DateAndTimes != null)
               {
                  foreach (DateAndTime dtm in trackResult.DateAndTimes)
                  {
                     if (dtm.Type == "ACTUAL_DELIVERY")
                     {
                        TxtDeliveryDate.Text = dtm.DateTimeVal.ToString("dd-MMM-yyyy hh:mm");
                        break;
                     }
                     else if (dtm.Type == "ESTIMATED_DELIVERY")
                     {
                        TxtDeliveryDate.Text = dtm.DateTimeVal.ToString("dd-MMM-yyyy hh:mm");
                        break;
                     }
                     else
                     {
                        TxtDeliveryDate.Text = "Unknown";
                     }
                  }
               }
               
               // Set last location
               if (trackResult.DeliveryDetails != null && 
                   trackResult.DeliveryDetails.SignedByName != null)
               {
                  TxtLastLocation.Text = "Signed by " + trackResult.DeliveryDetails.SignedByName + "\r\n";
               }
               else
               {
                  TxtLastLocation.Text = trackResult.LastUpdatedDestinationAddress.City + " "
                     + trackResult.LastUpdatedDestinationAddress.StateOrProvinceCode + " "
                     + trackResult.LastUpdatedDestinationAddress.CountryCode;
               }
               
               // Set ship to address
               if (trackResult.ScanEvents != null && trackResult.ScanEvents.Count != 0)
               {
                  if (trackResult.DeliveryDetails != null && 
                      trackResult.DeliveryDetails.ActualDeliveryAddress != null)
                  {
                     TxtShipToAddress.Text = trackResult.DeliveryDetails.ActualDeliveryAddress.City
                        + " " + trackResult.DeliveryDetails.ActualDeliveryAddress.StateOrProvinceCode
                        + " " + trackResult.DeliveryDetails.ActualDeliveryAddress.PostalCode
                        + " " + trackResult.DeliveryDetails.ActualDeliveryAddress.CountryCode;
                  }
                  else if (trackResult.RecipientInformation != null)
                  {
                     TxtShipToAddress.Text = trackResult.RecipientInformation.Address.City
                        + " " + trackResult.RecipientInformation.Address.StateOrProvinceCode
                        + " " + trackResult.RecipientInformation.Address.PostalCode
                        + " " + trackResult.RecipientInformation.Address.CountryCode;
                  }
                  
                  // Build tracking activity grid
                  foreach (TrackResult track in rsp.Output.CompleteTrackResults[0].TrackResults)
                  {
                     dr = _dtTrack.NewRow();
                     dr["ID"] = int.Parse(track.PackageDetails.SequenceNumber);
                     dr["TrackingNo"] = track.TrackingNumberInfo.TrackingNumber;
                     dr["Status"] = track.LatestStatusDetail.Description ?? "";
                     if (track.DateAndTimes != null && track.DateAndTimes.Count > 0)
                     {
                        dr["ActivityDate"] = track.DateAndTimes[0].DateTimeVal;
                     }
                     dr["Notes"] = "";
                     dr["Level"] = 0;
                     _dtTrack.Rows.Add(dr);
                     
                     // Add scan events (reverse order - newest first)
                     int y = track.ScanEvents.Count;
                     foreach (ScanEvent evnt in track.ScanEvents)
                     {
                        dr = _dtItems.NewRow();
                        dr["ID"] = y;
                        dr["TrackingNo"] = track.TrackingNumberInfo.TrackingNumber;
                        dr["Status"] = evnt.EventDescription;
                        dr["ActivityDate"] = evnt.DateOf;
                        dr["Notes"] = "";
                        dr["Level"] = 1;
                        _dtItems.Rows.Add(dr);
                        y--;
                     }
                  }
               }
            }
            else
            {
               ShowMessage("Error in GetFedExTrackInfo", "error");
            }
         }
         catch (Exception ex)
         {
            ShowMessage($"Error getting FedEx tracking info: {ex.Message}", "error");
            System.Diagnostics.Debug.WriteLine($"GetFedExTrackInfo Error: {ex.Message}");
         }
      }
      private void GetPurolatorTrackInfo(DataTable dt) { /* Implement from frmTracking.cs lines 319-381 */ }
      private void GetCanparTrackInfo(DataTable dt) { /* Implement from frmTracking.cs lines 413-503 */ }
      private void GetCaPostTrackInfo(string sTrackingNumber, int iBusID, int iSiteID, int iDelmode) { }
      private void GetLoomisTrackInfo(string sTrackingNumber, int iBusID, int iSiteID, int iDelmode) { }
      private void GetUSPSTrackInfo(string sTrackingNumber, int iBusID, int iSiteID, int iDelmode) { }
      private void GetCPU(DataTable dt) { /* Implement from frmTracking.cs lines 382-400 */ }

      /// <summary>
      /// Handle website button click - opens carrier tracking page
      /// Based on frmTracking.cs BtnExWebSite_Click (lines 874-906)
      /// </summary>
      protected void BtnWebSite_Click(object sender, EventArgs e)
      {
         string sURL = "";
         try
         {
            switch (_iDelMode)
            {
               case 4: // UPS
                  sURL = $"https://www.ups.com/track?loc=en_US&tracknum={TxtTrackingNo.Text}";
                  break;
               case 8: // FedEx
                  sURL = $"https://www.fedex.com/apps/fedextrack/?action=track&trackingnumber={TxtTrackingNo.Text}";
                  break;
               case 64: // Purolator
                  sURL = $"https://www.purolator.com/en/shipping/tracker?pins={TxtTrackingNo.Text}";
                  break;
               case 2048: // CanPar
                  sURL = $"https://www.canpar.com/en/tracking/track.htm?barcode={TxtTrackingNo.Text}";
                  break;
               case 512: // Canada Post
                  sURL = $"https://www.canadapost-postescanada.ca/track-reperage/en#/search?searchFor={TxtTrackingNo.Text}";
                  break;
               case 32: // USPS
                  sURL = $"https://tools.usps.com/go/TrackConfirmAction?tLabels={TxtTrackingNo.Text}";
                  break;
               default:
                  sURL = "";
                  break;
            }

            if (sURL.Length > 0)
            {
               // Open in new window
               string script = $"window.open('{sURL}', '_blank');";
               ScriptManager.RegisterStartupScript(this, GetType(), "OpenCarrierSite", script, true);
            }
         }
         catch (Exception ex)
         {
            ShowMessage($"❌ Error: {ex.Message}", "error");
         }
      }

      /// <summary>
      /// Show message to user
      /// </summary>
      private void ShowMessage(string message, string type)
      {
         LblMessage.Text = message;
         LblMessage.CssClass = $"message-label {type}";
         LblMessage.Visible = true;
      }

      /// <summary>
      /// Load mock tracking data for demo purposes
      /// Based on screenshot from frmTracking.cs
      /// </summary>
      private void LoadMockData()
      {
         // Set tracking information fields
         TxtTrackingNo.Text = "D42510139000001525001";
         TxtShipperAcctNo.Text = "";
         TxtServiceType.Text = "GROUND";
         TxtDeliveryDate.Text = "04-Nov-2025";
         TxtStatus.Text = "Departing sort facility";
         TxtPO.Text = "#1773ITL-NA";
         TxtOrderID.Text = "1 - 251021030016 CA1211M";
         TxtShipToAddress.Text = "";
         TxtLastLocation.Text = "Departing sort facility";

         // Set button text for CanPar
         BtnWebSite.Text = "CanPar Website";
         _iDelMode = 2048; // CanPar

         // Add mock tracking activity
         _dtItems.Rows.Add(1, "D42510139000001525001", "Departing sort facility", DateTime.Parse("2025-10-30 12:00"), "");

         // Bind to grid
         GvActivity.DataSource = _dtItems;
         GvActivity.DataBind();

         ShowMessage("Mock tracking data loaded for demo", "info");
      }
   }
}