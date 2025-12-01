using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ITLHealthWeb.FedExRest.Models
{
   /// <summary>
   /// FedEx Tracking Response - Main response model
   /// </summary>
   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class FedExTrackingResponse
   {
      [JsonProperty("transactionId", NullValueHandling = NullValueHandling.Ignore)]
      public string TransactionId { get; set; }

      [JsonProperty("customerTransactionId", NullValueHandling = NullValueHandling.Ignore)]
      public string CustomerTransactionId { get; set; }

      [JsonProperty("output", NullValueHandling = NullValueHandling.Ignore)]
      public TrackingOutput Output { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class TrackingOutput
   {
      [JsonProperty("completeTrackResults", NullValueHandling = NullValueHandling.Ignore)]
      public List<CompleteTrackResult> CompleteTrackResults { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class CompleteTrackResult
   {
      [JsonProperty("trackingNumber", NullValueHandling = NullValueHandling.Ignore)]
      public string TrackingNumber { get; set; }

      [JsonProperty("trackResults", NullValueHandling = NullValueHandling.Ignore)]
      public List<TrackResult> TrackResults { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class TrackResult
   {
      [JsonProperty("trackingNumberInfo", NullValueHandling = NullValueHandling.Ignore)]
      public TrackingNumberInfo TrackingNumberInfo { get; set; }

      [JsonProperty("serviceDetail", NullValueHandling = NullValueHandling.Ignore)]
      public ServiceDetail ServiceDetail { get; set; }

      [JsonProperty("latestStatusDetail", NullValueHandling = NullValueHandling.Ignore)]
      public LatestStatusDetail LatestStatusDetail { get; set; }

      [JsonProperty("dateAndTimes", NullValueHandling = NullValueHandling.Ignore)]
      public List<DateAndTime> DateAndTimes { get; set; }

      [JsonProperty("deliveryDetails", NullValueHandling = NullValueHandling.Ignore)]
      public DeliveryDetails DeliveryDetails { get; set; }

      [JsonProperty("lastUpdatedDestinationAddress", NullValueHandling = NullValueHandling.Ignore)]
      public LastUpdatedDestinationAddress LastUpdatedDestinationAddress { get; set; }

      [JsonProperty("recipientInformation", NullValueHandling = NullValueHandling.Ignore)]
      public RecipientInformation RecipientInformation { get; set; }

      [JsonProperty("packageDetails", NullValueHandling = NullValueHandling.Ignore)]
      public PackageDetails PackageDetails { get; set; }

      [JsonProperty("scanEvents", NullValueHandling = NullValueHandling.Ignore)]
      public List<ScanEvent> ScanEvents { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class TrackingNumberInfo
   {
      [JsonProperty("trackingNumber", NullValueHandling = NullValueHandling.Ignore)]
      public string TrackingNumber { get; set; }

      [JsonProperty("carrierCode", NullValueHandling = NullValueHandling.Ignore)]
      public string CarrierCode { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class ServiceDetail
   {
      [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
      public string Description { get; set; }

      [JsonProperty("shortDescription", NullValueHandling = NullValueHandling.Ignore)]
      public string ShortDescription { get; set; }

      [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
      public string Type { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class LatestStatusDetail
   {
      [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
      public string Code { get; set; }

      [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
      public string Description { get; set; }

      [JsonProperty("scanLocation", NullValueHandling = NullValueHandling.Ignore)]
      public ScanLocation ScanLocation { get; set; }
   }

   /// <summary>
   /// Date and time information for the tracked shipment
   /// </summary>
   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class DateAndTime
   {
      [JsonProperty("dateTime", NullValueHandling = NullValueHandling.Ignore)]
      public DateTime DateTimeVal { get; set; }

      /// <summary>
      /// Type: "ACTUAL_DELIVERY", "ESTIMATED_DELIVERY", "SHIP", etc.
      /// </summary>
      [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
      public string Type { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class DeliveryDetails
   {
      [JsonProperty("signedByName", NullValueHandling = NullValueHandling.Ignore)]
      public string SignedByName { get; set; }

      [JsonProperty("actualDeliveryAddress", NullValueHandling = NullValueHandling.Ignore)]
      public ActualDeliveryAddress ActualDeliveryAddress { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class ActualDeliveryAddress
   {
      [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
      public string City { get; set; }

      [JsonProperty("stateOrProvinceCode", NullValueHandling = NullValueHandling.Ignore)]
      public string StateOrProvinceCode { get; set; }

      [JsonProperty("postalCode", NullValueHandling = NullValueHandling.Ignore)]
      public string PostalCode { get; set; }

      [JsonProperty("countryCode", NullValueHandling = NullValueHandling.Ignore)]
      public string CountryCode { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class LastUpdatedDestinationAddress
   {
      [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
      public string City { get; set; }

      [JsonProperty("stateOrProvinceCode", NullValueHandling = NullValueHandling.Ignore)]
      public string StateOrProvinceCode { get; set; }

      [JsonProperty("postalCode", NullValueHandling = NullValueHandling.Ignore)]
      public string PostalCode { get; set; }

      [JsonProperty("countryCode", NullValueHandling = NullValueHandling.Ignore)]
      public string CountryCode { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class RecipientInformation
   {
      [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
      public TrackingAddress Address { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class TrackingAddress
   {
      [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
      public string City { get; set; }

      [JsonProperty("stateOrProvinceCode", NullValueHandling = NullValueHandling.Ignore)]
      public string StateOrProvinceCode { get; set; }

      [JsonProperty("postalCode", NullValueHandling = NullValueHandling.Ignore)]
      public string PostalCode { get; set; }

      [JsonProperty("countryCode", NullValueHandling = NullValueHandling.Ignore)]
      public string CountryCode { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class PackageDetails
   {
      [JsonProperty("sequenceNumber", NullValueHandling = NullValueHandling.Ignore)]
      public string SequenceNumber { get; set; }

      [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
      public string Count { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class ScanEvent
   {
      [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
      public DateTime DateOf { get; set; }

      [JsonProperty("eventDescription", NullValueHandling = NullValueHandling.Ignore)]
      public string EventDescription { get; set; }

      [JsonProperty("scanLocation", NullValueHandling = NullValueHandling.Ignore)]
      public ScanLocation ScanLocation { get; set; }

      [JsonProperty("eventType", NullValueHandling = NullValueHandling.Ignore)]
      public string EventType { get; set; }
   }

   [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
   public class ScanLocation
   {
      [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
      public string City { get; set; }

      [JsonProperty("stateOrProvinceCode", NullValueHandling = NullValueHandling.Ignore)]
      public string StateOrProvinceCode { get; set; }

      [JsonProperty("countryCode", NullValueHandling = NullValueHandling.Ignore)]
      public string CountryCode { get; set; }
   }
}
