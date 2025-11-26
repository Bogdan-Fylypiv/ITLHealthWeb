//using SID.CanparTracking;
//using SID.Classes;
//using SID.FedExRest.Models;
//using SID.PurolatorTrackingSvc;
//using SID.UPSRestful;
//using SID.UPSRestful.Models;
using System;
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

               if (string.IsNullOrEmpty(_sTrackingNumber))
               {
                  ShowMessage("No tracking number provided.", "error");
                  return;
               }

               // Initialize data tables
               InitializeDataTables();

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

      private void GetUPSTrackInfo() { /* Implement from frmTracking.cs lines 664-794 */ }
      private void GetFedExTrackInfo(DataTable dt) { /* Implement from frmTracking.cs lines 529-662 */ }
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
   }
}