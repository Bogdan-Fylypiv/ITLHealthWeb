// Carrier-Specific Tracking Methods for Tracking.aspx.cs
// Based on SIDITL\Forms\frmTracking.cs implementation
// Copy these methods into your Tracking.aspx.cs file

/// <summary>
/// Get UPS tracking information
/// Based on frmTracking.cs lines 664-794
/// </summary>
private void GetUPSTrackInfo()
{
    DataRow dr;
    try
    {
        UPSWeb ups = new UPSWeb(1, 1);
        object response = ups.UPSGetTrackingInfo(_gOrdID.ToString(), _sTrackingNumber);
        
        if (response is UPSTrackingInfoResponseModel res)
        {
            TrackShipment rspShipment = res.TrackResponse.Shipment[0];
            TxtTrackingNo.Text = rspShipment.InquiryNumber;
            TxtServiceType.Text = rspShipment.Package[0].Service.Description;
            
            // Last location / signed by
            if (rspShipment.Package[0].DeliveryInformation.ReceivedBy != null)
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
            
            TxtStatus.Text = rspShipment.Package[0].Activity[0].Status.Description;
            
            // Reference numbers
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
            
            // Delivery date
            if (rspShipment.Package[0].DeliveryTime == null)
            {
                TxtDeliveryDate.Text = DateTime.ParseExact(
                    $"{rspShipment.Package[0].Activity[0].Date.Substring(0, 4)}-" +
                    $"{rspShipment.Package[0].Activity[0].Date.Substring(4, 2)}-" +
                    $"{rspShipment.Package[0].Activity[0].Date.Substring(6, 2)} " +
                    $"{rspShipment.Package[0].Activity[0].Time.Substring(0, 2)}:" +
                    $"{rspShipment.Package[0].Activity[0].Time.Substring(2, 2)}:" +
                    $"{rspShipment.Package[0].Activity[0].Time.Substring(4, 2)}",
                    "yyyy-MM-dd HH:mm:ss", null).ToString("dd-MMM-yyyy");
            }
            else
            {
                TxtDeliveryDate.Text = DateTime.ParseExact(
                    $"{rspShipment.Package[0].DeliveryDate[0].Date.Substring(0, 4)}-" +
                    $"{rspShipment.Package[0].DeliveryDate[0].Date.Substring(4, 2)}-" +
                    $"{rspShipment.Package[0].DeliveryDate[0].Date.Substring(6, 2)} " +
                    $"{rspShipment.Package[0].DeliveryTime.EndTime.Substring(0, 2)}:" +
                    $"{rspShipment.Package[0].DeliveryTime.EndTime.Substring(2, 2)}:" +
                    $"{rspShipment.Package[0].DeliveryTime.EndTime.Substring(4, 2)}",
                    "yyyy-MM-dd HH:mm:ss", null).ToString("dd-MMM-yyyy");
            }
            
            // Ship to address
            int iShipmentAddressCnt = rspShipment.Package[0].PackageAddress.Count;
            PackageAddress shpAddress = rspShipment.Package[0].PackageAddress[iShipmentAddressCnt - 1];
            
            if (shpAddress.Address.AddressLine1 != null && shpAddress.Address.AddressLine1.Length > 0)
                TxtShipToAddress.Text += $"{shpAddress.Address.AddressLine1}\r\n";
            if (shpAddress.Address.AddressLine2 != null && shpAddress.Address.AddressLine2.Length > 0)
                TxtShipToAddress.Text += $"{shpAddress.Address.AddressLine2}\r\n";
            if (shpAddress.Address.AddressLine3 != null && shpAddress.Address.AddressLine3.Length > 0)
                TxtShipToAddress.Text += $"{shpAddress.Address.AddressLine3}\r\n";
            
            TxtShipToAddress.Text += shpAddress.Address.City + " " + shpAddress.Address.StateProvince
                + " " + shpAddress.Address.PostalCode + " " + shpAddress.Address.Country;
            
            // Build tracking activity data
            int j = 0, k = 0;
            foreach (TrackPackage pkg in rspShipment.Package)
            {
                j++;
                dr = _dtTrack.NewRow();
                dr["ID"] = j;
                dr["TrackingNo"] = pkg.TrackingNumber;
                dr["Status"] = pkg.Activity[0].Status.Description ?? "";
                dr["ActivityDate"] = DateTime.ParseExact(
                    $"{pkg.Activity[0].Date.Substring(0, 4)}-{pkg.Activity[0].Date.Substring(4, 2)}-{pkg.Activity[0].Date.Substring(6, 2)} " +
                    $"{pkg.Activity[0].Time.Substring(0, 2)}:{pkg.Activity[0].Time.Substring(2, 2)}:{pkg.Activity[0].Time.Substring(4, 2)}",
                    "yyyy-MM-dd HH:mm:ss", null);
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
                    dr["ActivityDate"] = DateTime.ParseExact(
                        $"{item.Date.Substring(0, 4)}-{item.Date.Substring(4, 2)}-{item.Date.Substring(6, 2)} " +
                        $"{item.Time.Substring(0, 2)}:{item.Time.Substring(2, 2)}:{item.Time.Substring(4, 2)}",
                        "yyyy-MM-dd HH:mm:ss", null);
                    dr["Notes"] = "";
                    dr["Level"] = 1;
                    _dtItems.Rows.Add(dr);
                }
            }
        }
        else
        {
            TxtStatus.Text = "Invalid UPS Tracking Number";
            ShowMessage("⚠️ Invalid UPS tracking number", "error");
        }
    }
    catch (Exception ex)
    {
        ShowMessage($"❌ UPS Tracking Error: {ex.Message}", "error");
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
        if (dt.Rows.Count == 0) return;
        
        dr = dt.Rows[0];
        string sCarrierCode = dr["ServiceType"].ToString() == "FEDEX_GROUND" ? "FDXG" : "FDXE";
        
        if (_Scan == null)
            _Scan = new SID.Classes.Scan(System.Windows.Forms.Cursors.Default);
        
        object Res = _Scan.GetFedExTrackingInfo(_iBusID, 1, _sTrackingNumber, sCarrierCode, 
            Convert.ToDateTime(dr["dtmCreate"]), dr["PONumber"].ToString());
            
        if (Res is FedExTrackingResponse rsp)
        {
            var trackResult = rsp.Output.CompleteTrackResults[0].TrackResults[0];
            
            TxtServiceType.Text = trackResult.ServiceDetail != null ? 
                trackResult.ServiceDetail.Description : dt.Rows[0]["ServiceType"].ToString();
            TxtPO.Text = rsp.CustomerTransactionId ?? dr["PONumber"].ToString();
            TxtStatus.Text = trackResult.LatestStatusDetail.Description;
            
            // Delivery date
            if (trackResult.DateAndTimes != null)
            {
                foreach (DateAndTime dtm in trackResult.DateAndTimes)
                {
                    if (dtm.Type == "ACTUAL_DELIVERY" || dtm.Type == "ESTIMATED_DELIVERY")
                    {
                        TxtDeliveryDate.Text = dtm.DateTimeVal.ToString("dd-MMM-yyyy HH:mm");
                        break;
                    }
                }
            }
            
            // Last location
            if (trackResult.DeliveryDetails != null && trackResult.DeliveryDetails.SignedByName != null)
            {
                TxtLastLocation.Text = "Signed by " + trackResult.DeliveryDetails.SignedByName + "\r\n";
            }
            else
            {
                TxtLastLocation.Text = trackResult.LastUpdatedDestinationAddress.City + " "
                    + trackResult.LastUpdatedDestinationAddress.StateOrProvinceCode + " "
                    + trackResult.LastUpdatedDestinationAddress.CountryCode;
            }
            
            // Ship to address
            if (trackResult.ScanEvents.Count != 0)
            {
                if (trackResult.DeliveryDetails.ActualDeliveryAddress != null)
                {
                    var addr = trackResult.DeliveryDetails.ActualDeliveryAddress;
                    TxtShipToAddress.Text = $"{addr.City} {addr.StateOrProvinceCode} {addr.PostalCode} {addr.CountryCode}";
                }
                else
                {
                    var addr = trackResult.RecipientInformation.Address;
                    TxtShipToAddress.Text = $"{addr.City} {addr.StateOrProvinceCode} {addr.PostalCode} {addr.CountryCode}";
                }
                
                // Build tracking activity
                foreach (TrackResult track in rsp.Output.CompleteTrackResults[0].TrackResults)
                {
                    dr = _dtTrack.NewRow();
                    dr["ID"] = Convert.ToInt32(track.PackageDetails.SequenceNumber);
                    dr["TrackingNo"] = track.TrackingNumberInfo.TrackingNumber;
                    dr["Status"] = track.LatestStatusDetail.Description ?? "";
                    if (track.DateAndTimes != null)
                        dr["ActivityDate"] = track.DateAndTimes[0].DateTimeVal;
                    dr["Notes"] = "";
                    dr["Level"] = 0;
                    _dtTrack.Rows.Add(dr);
                    
                    int y = track.ScanEvents.Count;
                    foreach (ScanEvent evnt in track.ScanEvents)
                    {
                        dr = _dtItems.NewRow();
                        dr["ID"] = y--;
                        dr["TrackingNo"] = track.TrackingNumberInfo.TrackingNumber;
                        dr["Status"] = evnt.EventDescription;
                        dr["ActivityDate"] = evnt.DateOf;
                        dr["Notes"] = "";
                        dr["Level"] = 1;
                        _dtItems.Rows.Add(dr);
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        ShowMessage($"❌ FedEx Tracking Error: {ex.Message}", "error");
        System.Diagnostics.Debug.WriteLine($"GetFedExTrackInfo Error: {ex.Message}");
    }
}

/// <summary>
/// Get Purolator tracking information
/// Based on frmTracking.cs lines 319-381
/// </summary>
private void GetPurolatorTrackInfo(DataTable dt)
{
    int x = 0;
    List<string> lstPins = new List<string>();
    int iVoidReply = 0;
    string sVoidMsg = "";
    DataRow dr;
    
    try
    {
        foreach (DataRow item in dt.Rows)
            lstPins.Add(item["TrackingNo"].ToString());
        
        var response = _Scan.GetPurolatorTracking(lstPins, _iBusID, _iSiteID, _iDelMode, "", ref iVoidReply, ref sVoidMsg);
            
        if (response != null)
        {
            TxtTrackingNo.Text = _sTrackingNumber;
            TxtServiceType.Text = dt.Rows[0]["ServiceType"].ToString();
            TxtStatus.Text = response.TrackingInformationList[0].Scans[0].Description;
            TxtLastLocation.Text = $"{response.TrackingInformationList[0].Scans[0].Depot.Name}\r\n" +
                $"{response.TrackingInformationList[0].Scans[0].Description}";
            TxtPO.Text = dt.Rows[0]["PONumber"].ToString();
            TxtOrderID.Text = dt.Rows[0]["OrderID"].ToString();
            
            foreach (TrackingInformation trkInfo in response.TrackingInformationList)
            {
                x++;
                dr = _dtTrack.NewRow();
                dr["ID"] = x;
                dr["TrackingNo"] = trkInfo.Scans[0].PIN.Value;
                dr["Status"] = trkInfo.Scans[0].Description;
                dr["ActivityDate"] = DateTime.ParseExact(
                    trkInfo.Scans[0].ScanDate.ToString() + " " + 
                    trkInfo.Scans[0].ScanTime.Substring(0, 2) + ":" + 
                    trkInfo.Scans[0].ScanTime.Substring(2, 2) + ":" + 
                    trkInfo.Scans[0].ScanTime.Substring(4, 2),
                    "yyyyMMdd HH:mm:ss", null);
                dr["Notes"] = "";
                dr["Level"] = 0;
                _dtTrack.Rows.Add(dr);
                
                for (int i = trkInfo.Scans.Length - 1; i >= 0; i--)
                {
                    var scn = trkInfo.Scans[i];
                    dr = _dtItems.NewRow();
                    dr["ID"] = i + 1;
                    dr["TrackingNo"] = scn.PIN.Value;
                    dr["Status"] = scn.Description;
                    dr["ActivityDate"] = DateTime.ParseExact(
                        scn.ScanDate.ToString() + " " + 
                        scn.ScanTime.Substring(0, 2) + ":" + 
                        scn.ScanTime.Substring(2, 2) + ":" + 
                        scn.ScanTime.Substring(4, 2),
                        "yyyyMMdd HH:mm:ss", null);
                    dr["Notes"] = "";
                    dr["Level"] = 1;
                    _dtItems.Rows.Add(dr);
                }
            }
        }
        else
        {
            ShowMessage($"⚠️ {sVoidMsg}", "error");
        }
    }
    catch (Exception ex)
    {
        ShowMessage($"❌ Purolator Tracking Error: {ex.Message}", "error");
    }
}

/// <summary>
/// Get CanPar tracking information
/// Based on frmTracking.cs lines 413-503
/// </summary>
private void GetCanparTrackInfo(DataTable dt)
{
    string endpoint = "https://canship.canpar.com/canshipws/services/CanparAddonsService.CanparAddonsServiceHttpSoap12Endpoint/";
    DataRow dr;
    int x = 0;
    
    try
    {
        var m_CanParTrack = new CanparAddonsServicePortTypeClient("CanparAddonsServiceHttpSoap12Endpoint");
        m_CanParTrack.Endpoint.Address = new System.ServiceModel.EndpointAddress(endpoint);
        
        var request = new TrackByBarcodeRq()
        {
            barcode = _sTrackingNumber,
            track_shipment = true,
            track_shipmentSpecified = true
        };
        
        var response = m_CanParTrack.trackByBarcode(request);
        
        if (response.error != null)
            throw new Exception(response.error);
        else if (response.result != null)
        {
            TxtTrackingNo.Text = _sTrackingNumber;
            TxtServiceType.Text = response.result[0].service_description_en;
            
            if ((response.result[0].estimated_delivery_date ?? "").Length == 8)
            {
                DateTime dtmDate = DateTime.ParseExact(response.result[0].estimated_delivery_date, "yyyyMMdd", null);
                TxtDeliveryDate.Text = dtmDate.ToString("dd-MMM-yyyy");
                TxtStatus.Text = response.result[0].events[0].code_description_en;
            }
            
            TxtOrderID.Text = response.result[0].reference_num;
            TxtPO.Text = dt.Rows[0]["PONumber"].ToString();
            
            if (response.result[0].consignee_address != null)
            {
                var addr = response.result[0].consignee_address;
                TxtShipToAddress.Text = $"{addr.address_line_1}\r\n{addr.city} {addr.province} {addr.postal_code}";
            }
            
            TxtLastLocation.Text = response.result[0].events == null ? 
                "No Information Yet" : response.result[0].events[0].code_description_en;
            if (response.result[0].signed_by != null)
                TxtLastLocation.Text += $"\r\nSigned for by {response.result[0].signed_by}";
            
            foreach (TrackingResult rslt in response.result)
            {
                x++;
                if (rslt.events != null)
                {
                    dr = _dtTrack.NewRow();
                    dr["ID"] = x;
                    dr["TrackingNo"] = rslt.barcode;
                    dr["Status"] = rslt.events[0].code_description_en;
                    dr["ActivityDate"] = DateTime.ParseExact(
                        $"{rslt.events[0].local_date_time.Substring(0, 4)}-{rslt.events[0].local_date_time.Substring(4, 2)}-" +
                        $"{rslt.events[0].local_date_time.Substring(6, 2)} {rslt.events[0].local_date_time.Substring(9, 2)}:" +
                        $"{rslt.events[0].local_date_time.Substring(11, 2)}:{rslt.events[0].local_date_time.Substring(13, 2)}",
                        "yyyy-MM-dd HH:mm:ss", null);
                    dr["Notes"] = "";
                    dr["Level"] = 0;
                    _dtTrack.Rows.Add(dr);
                    
                    int y = rslt.events.Length;
                    foreach (TrackingEvent evnt in rslt.events)
                    {
                        dr = _dtItems.NewRow();
                        dr["ID"] = y--;
                        dr["TrackingNo"] = rslt.barcode;
                        dr["Status"] = evnt.code_description_en;
                        dr["ActivityDate"] = DateTime.ParseExact(
                            $"{evnt.local_date_time.Substring(0, 4)}-{evnt.local_date_time.Substring(4, 2)}-" +
                            $"{evnt.local_date_time.Substring(6, 2)} {evnt.local_date_time.Substring(9, 2)}:" +
                            $"{evnt.local_date_time.Substring(11, 2)}:{evnt.local_date_time.Substring(13, 2)}",
                            "yyyy-MM-dd HH:mm:ss", null);
                        dr["Notes"] = "";
                        dr["Level"] = 1;
                        _dtItems.Rows.Add(dr);
                    }
                }
            }
        }
        else
        {
            TxtLastLocation.Text = "No response from CanPar yet.";
        }
    }
    catch (Exception ex)
    {
        ShowMessage($"❌ CanPar Tracking Error: {ex.Message}", "error");
    }
}

/// <summary>
/// Get CPU tracking information (basic info only)
/// Based on frmTracking.cs lines 382-400
/// </summary>
private void GetCPU(DataTable dt)
{
    try
    {
        TxtTrackingNo.Text = _sTrackingNumber;
        TxtServiceType.Text = dt.Rows[0]["ServiceType"].ToString();
        TxtStatus.Text = "";
        TxtShipToAddress.Text = "";
        TxtLastLocation.Text = "";
        TxtPO.Text = dt.Rows[0]["PONumber"].ToString();
        TxtOrderID.Text = dt.Rows[0]["OrderID"].ToString();
    }
    catch (Exception ex)
    {
        ShowMessage($"❌ CPU Info Error: {ex.Message}", "error");
    }
}

// Placeholders for carriers not yet implemented
private void GetCaPostTrackInfo(string sTrackingNumber, int iBusID, int iSiteID, int iDelmode)
{
    TxtTrackingNo.Text = sTrackingNumber;
    TxtStatus.Text = "Canada Post tracking not yet implemented";
    ShowMessage("⚠️ Canada Post API integration pending", "error");
}

private void GetLoomisTrackInfo(string sTrackingNumber, int iBusID, int iSiteID, int iDelmode)
{
    TxtTrackingNo.Text = sTrackingNumber;
    TxtStatus.Text = "Loomis tracking not yet implemented";
    ShowMessage("⚠️ Loomis API integration pending", "error");
}

private void GetUSPSTrackInfo(string sTrackingNumber, int iBusID, int iSiteID, int iDelmode)
{
    TxtTrackingNo.Text = sTrackingNumber;
    TxtStatus.Text = "USPS tracking not yet implemented";
    ShowMessage("⚠️ USPS API integration pending", "error");
}
