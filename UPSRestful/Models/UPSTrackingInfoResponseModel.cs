using Newtonsoft.Json;
using System.Collections.Generic;

namespace ITLHealthWeb.UPSRestful.Models
{
   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class UPSTrackingInfoResponseModel
   {
      [JsonProperty("trackResponse", NullValueHandling = NullValueHandling.Ignore)]
      public TrackResponse TrackResponse { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class TrackActivity
   {
      [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
      public string Date { get; set; }

      [JsonProperty("gmtDate", NullValueHandling = NullValueHandling.Ignore)]
      public string GmtDate { get; set; }

      [JsonProperty("gmtOffset", NullValueHandling = NullValueHandling.Ignore)]
      public string GmtOffset { get; set; }

      [JsonProperty("gmtTime", NullValueHandling = NullValueHandling.Ignore)]
      public string GmtTime { get; set; }

      [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
      public Location Location { get; set; }

      [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
      public TrackStatus Status { get; set; }

      [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
      public string Time { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class TrackStatus
   {
      [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
      public string Code { get; set; }

      [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
      public string Description { get; set; }

      [JsonProperty("simplifiedTextDescription", NullValueHandling = NullValueHandling.Ignore)]
      public string SimplifiedTextDescription { get; set; }

      [JsonProperty("statusCode", NullValueHandling = NullValueHandling.Ignore)]
      public string StatusCode { get; set; }

      [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
      public string Type { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class Location
   {
      [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
      public LocAddress Address { get; set; }

      [JsonProperty("slic", NullValueHandling = NullValueHandling.Ignore)]
      public string SLIC { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class LocAddress
   {
      [JsonProperty(PropertyName = "addressLine1", NullValueHandling = NullValueHandling.Ignore)]
      public string AddressLine1 { get; set; }

      [JsonProperty(PropertyName = "addressLine2", NullValueHandling = NullValueHandling.Ignore)]
      public string AddressLine2 { get; set; }

      [JsonProperty(PropertyName = "addressLine3", NullValueHandling = NullValueHandling.Ignore)]
      public string AddressLine3 { get; set; }

      [JsonProperty(PropertyName = "city", NullValueHandling = NullValueHandling.Ignore)]
      public string City { get; set; }

      [JsonProperty(PropertyName = "countryCode", NullValueHandling = NullValueHandling.Ignore)]
      public string Country { get; set; }

      [JsonProperty(PropertyName = nameof(CountryCode), NullValueHandling = NullValueHandling.Ignore)]
      public string CountryCode { get; set; }

      [JsonProperty(PropertyName = "postalCode", NullValueHandling = NullValueHandling.Ignore)]
      public string PostalCode { get; set; }

      [JsonProperty(PropertyName = "stateProvince", NullValueHandling = NullValueHandling.Ignore)]
      public string StateProvince { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class DeliveryInformation
   {
      [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
      public string Location { get; set; }

      [JsonProperty("receivedBy", NullValueHandling = NullValueHandling.Ignore)]
      public string ReceivedBy { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class DeliveryTime
   {
      [JsonProperty("endTime", NullValueHandling = NullValueHandling.Ignore)]
      public string EndTime { get; set; }

      [JsonProperty("startTime", NullValueHandling = NullValueHandling.Ignore)]
      public string StartTime { get; set; }

      [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
      public string Type { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class DeliveryDate
   {
      [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
      public string Date { get; set; }

      [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
      public string Type { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class TrackPackage
   {
      [JsonProperty("activity", NullValueHandling = NullValueHandling.Ignore)]
      public List<TrackActivity> Activity { get; set; }

      [JsonProperty("deliveryDate", NullValueHandling = NullValueHandling.Ignore)]
      public List<DeliveryDate> DeliveryDate { get; set; }

      [JsonProperty("deliveryInformation", NullValueHandling = NullValueHandling.Ignore)]
      public DeliveryInformation DeliveryInformation { get; set; }

      [JsonProperty("deliveryTime", NullValueHandling = NullValueHandling.Ignore)]
      public DeliveryTime DeliveryTime { get; set; }

      [JsonProperty("packageAddress", NullValueHandling = NullValueHandling.Ignore)]
      public List<PackageAddress> PackageAddress { get; set; }

      [JsonProperty("referenceNumber", NullValueHandling = NullValueHandling.Ignore)]
      public List<TrackReferenceNumber> ReferenceNumber { get; set; }

      [JsonProperty("service", NullValueHandling = NullValueHandling.Ignore)]
      public TrackService Service { get; set; }

      [JsonProperty("trackingNumber", NullValueHandling = NullValueHandling.Ignore)]
      public string TrackingNumber { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class PackageAddress
   {
      [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
      public LocAddress Address { get; set; }

      [JsonProperty("attentionName", NullValueHandling = NullValueHandling.Ignore)]
      public string AttentionName { get; set; }

      [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
      public string Name { get; set; }

      [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
      public string Type { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class TrackReferenceNumber
   {
      [JsonProperty("number", NullValueHandling = NullValueHandling.Ignore)]
      public string Number { get; set; }

      [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
      public string Type { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class TrackService
   {
      [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
      public string Code { get; set; }

      [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
      public string Description { get; set; }

      [JsonProperty("levelCode", NullValueHandling = NullValueHandling.Ignore)]
      public string LevelCode { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class TrackShipment
   {
      [JsonProperty("inquiryNumber", NullValueHandling = NullValueHandling.Ignore)]
      public string InquiryNumber { get; set; }

      [JsonProperty("package", NullValueHandling = NullValueHandling.Ignore)]
      public List<TrackPackage> Package { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class TrackResponse
   {
      [JsonProperty("shipment", NullValueHandling = NullValueHandling.Ignore)]
      public List<TrackShipment> Shipment { get; set; }
   }
}
