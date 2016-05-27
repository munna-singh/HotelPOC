using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBeds.ServiceCatalogues.HotelCatalog.Enums
{
    class JSonConverters
    {
        public class HotelbedsCustomerTypeConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                SimpleTypes.HotelbedsCustomerType type = (SimpleTypes.HotelbedsCustomerType)value;
                switch (type)
                {
                    case SimpleTypes.HotelbedsCustomerType.AD:
                        writer.WriteValue("AD");
                        break;
                    case SimpleTypes.HotelbedsCustomerType.CH:
                        writer.WriteValue("CH");
                        break;

                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var enumString = (string)reader.Value;
                SimpleTypes.HotelbedsCustomerType? type = null;

                if (enumString == "AD")
                    type = SimpleTypes.HotelbedsCustomerType.AD;
                else if (enumString == "CH")
                    type = SimpleTypes.HotelbedsCustomerType.CH;

                return type;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }
        }

        public class ShowDirectPaymentTypeConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                SimpleTypes.ShowDirectPaymentType type = (SimpleTypes.ShowDirectPaymentType)value;

                switch (type)
                {
                    case SimpleTypes.ShowDirectPaymentType.AT_HOTEL:
                        writer.WriteValue("AT_HOTEL");
                        break;
                    case SimpleTypes.ShowDirectPaymentType.AT_WEB:
                        writer.WriteValue("AT_WEB");
                        break;
                    case SimpleTypes.ShowDirectPaymentType.BOTH:
                        writer.WriteValue("BOTH");
                        break;
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string value = (string)reader.Value;
                SimpleTypes.ShowDirectPaymentType? type = null;

                if (value == "S")
                    type = SimpleTypes.ShowDirectPaymentType.AT_HOTEL;
                else if (value == "N")
                    type = SimpleTypes.ShowDirectPaymentType.AT_WEB;
                else
                    type = SimpleTypes.ShowDirectPaymentType.BOTH;

                return type;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }
        }

        public class PaymentTypeConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                SimpleTypes.PaymentType type = (SimpleTypes.PaymentType)value;

                switch (type)
                {
                    case SimpleTypes.PaymentType.AT_HOTEL:
                        writer.WriteValue("AT_HOTEL");
                        break;
                    case SimpleTypes.PaymentType.AT_WEB:
                        writer.WriteValue("AT_WEB");
                        break;
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string value = (string)reader.Value;
                SimpleTypes.PaymentType? type = null;

                if (value == "AT_HOTEL")
                    type = SimpleTypes.PaymentType.AT_HOTEL;
                else if (value == "AT_WEB")
                    type = SimpleTypes.PaymentType.AT_WEB;

                return type;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }
        }

        public class TaxTypeConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                SimpleTypes.TaxType type = (SimpleTypes.TaxType)value;

                switch (type)
                {
                    case SimpleTypes.TaxType.TAX:
                        writer.WriteValue("TAX");
                        break;
                    case SimpleTypes.TaxType.FEE:
                        writer.WriteValue("FEE");
                        break;
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string value = (string)reader.Value;
                SimpleTypes.TaxType? type = null;

                if (value == "TAX")
                    type = SimpleTypes.TaxType.TAX;
                else if (value == "FEE")
                    type = SimpleTypes.TaxType.FEE;

                return type;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }
        }

        public class BookingStatusConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                SimpleTypes.TaxType type = (SimpleTypes.TaxType)value;

                switch (type)
                {
                    case SimpleTypes.TaxType.TAX:
                        writer.WriteValue("TAX");
                        break;
                    case SimpleTypes.TaxType.FEE:
                        writer.WriteValue("FEE");
                        break;
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string value = (string)reader.Value;
                SimpleTypes.TaxType? type = null;

                if (value == "TAX")
                    type = SimpleTypes.TaxType.TAX;
                else if (value == "FEE")
                    type = SimpleTypes.TaxType.FEE;

                return type;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }
        }

        public class HotelPackageConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                SimpleTypes.HotelPackage type = (SimpleTypes.HotelPackage)value;

                switch (type)
                {
                    case SimpleTypes.HotelPackage.YES:
                        writer.WriteValue("YES");
                        break;
                    case SimpleTypes.HotelPackage.NO:
                        writer.WriteValue("NO");
                        break;
                    case SimpleTypes.HotelPackage.BOTH:
                        writer.WriteValue("BOTH");
                        break;
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string value = (string)reader.Value;
                SimpleTypes.HotelPackage? type = null;

                if (value == "YES")
                    type = SimpleTypes.HotelPackage.YES;
                else if (value == "NO")
                    type = SimpleTypes.HotelPackage.NO;
                else
                    type = SimpleTypes.HotelPackage.BOTH;

                return type;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }
        }

        public class HotelCodeTypeConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                SimpleTypes.HotelCodeType type = (SimpleTypes.HotelCodeType)value;

                switch (type)
                {
                    case SimpleTypes.HotelCodeType.HOTELBEDS:
                        writer.WriteValue("HOTELBEDS");
                        break;
                    case SimpleTypes.HotelCodeType.GIATA:
                        writer.WriteValue("GIATA");
                        break;
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string value = (string)reader.Value;
                SimpleTypes.HotelCodeType? type = null;

                if (value == "HOTELBEDS")
                    type = SimpleTypes.HotelCodeType.HOTELBEDS;
                else if (value == "GIATA")
                    type = SimpleTypes.HotelCodeType.GIATA;

                return type;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }
        }

        public class ChannelTypeConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                SimpleTypes.ChannelType type = (SimpleTypes.ChannelType)value;

                switch (type)
                {
                    case SimpleTypes.ChannelType.B2B:
                        writer.WriteValue("B2B");
                        break;
                    case SimpleTypes.ChannelType.B2C:
                        writer.WriteValue("B2C");
                        break;
                    case SimpleTypes.ChannelType.META:
                        writer.WriteValue("META");
                        break;
                    case SimpleTypes.ChannelType.NEWSLETTER:
                        writer.WriteValue("NEWSLETTER");
                        break;
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string value = (string)reader.Value;
                SimpleTypes.ChannelType? type = null;

                if (value == "B2B")
                    type = SimpleTypes.ChannelType.B2B;
                else if (value == "B2C")
                    type = SimpleTypes.ChannelType.B2C;
                else if (value == "META")
                    type = SimpleTypes.ChannelType.META;
                else
                    type = SimpleTypes.ChannelType.NEWSLETTER;

                return type;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }
        }

        public class DeviceTypeConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                SimpleTypes.DeviceType type = (SimpleTypes.DeviceType)value;

                switch (type)
                {
                    case SimpleTypes.DeviceType.MOBILE:
                        writer.WriteValue("MOBILE");
                        break;
                    case SimpleTypes.DeviceType.TABLET:
                        writer.WriteValue("TABLET");
                        break;
                    case SimpleTypes.DeviceType.WEB:
                        writer.WriteValue("WEB");
                        break;
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string value = (string)reader.Value;
                SimpleTypes.DeviceType? type = null;

                if (value == "MOBILE")
                    type = SimpleTypes.DeviceType.MOBILE;
                else if (value == "TABLET")
                    type = SimpleTypes.DeviceType.TABLET;
                else if (value == "WEB")
                    type = SimpleTypes.DeviceType.WEB;

                return type;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }
        }

        public class RateTypeConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                SimpleTypes.RateType type = (SimpleTypes.RateType)value;

                switch (type)
                {
                    case SimpleTypes.RateType.BOOKABLE:
                        writer.WriteValue("BOOKABLE");
                        break;
                    case SimpleTypes.RateType.RECHECK:
                        writer.WriteValue("RECHECK");
                        break;
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string value = (string)reader.Value;
                SimpleTypes.RateType? type = null;

                if (value == "BOOKABLE")
                    type = SimpleTypes.RateType.BOOKABLE;
                else if (value == "RECHECK")
                    type = SimpleTypes.RateType.RECHECK;

                return type;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }
        }

        public class AccommodationTypeConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                List<SimpleTypes.AccommodationType> listType = (List<SimpleTypes.AccommodationType>)value;
                writer.WriteStartArray();
                foreach (SimpleTypes.AccommodationType type in listType)
                {
                    switch (type)
                    {
                        case SimpleTypes.AccommodationType.HOTEL:
                            writer.WriteValue("HOTEL");
                            break;
                        case SimpleTypes.AccommodationType.APARTMENT:
                            writer.WriteValue("APARTMENT");
                            break;
                        case SimpleTypes.AccommodationType.RURAL:
                            writer.WriteValue("RURAL");
                            break;
                        case SimpleTypes.AccommodationType.HOSTEL:
                            writer.WriteValue("HOSTEL");
                            break;
                        case SimpleTypes.AccommodationType.APTHOTEL:
                            writer.WriteValue("APTHOTEL");
                            break;
                        case SimpleTypes.AccommodationType.CAMPING:
                            writer.WriteValue("CAMPING");
                            break;
                        case SimpleTypes.AccommodationType.HISTORIC:
                            writer.WriteValue("HISTORIC");
                            break;
                        case SimpleTypes.AccommodationType.PENDING:
                            writer.WriteValue("PENDING");
                            break;
                        case SimpleTypes.AccommodationType.RESORT:
                            writer.WriteValue("RESORT");
                            break;
                        case SimpleTypes.AccommodationType.HOMES:
                            writer.WriteValue("HOMES");
                            break;
                    }
                }
                writer.WriteEndArray();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string value = (string)reader.Value;
                SimpleTypes.AccommodationType? type = null;

                if (value == "HOTEL")
                    type = SimpleTypes.AccommodationType.HOTEL;
                else if (value == "APARTMENT")
                    type = SimpleTypes.AccommodationType.APARTMENT;
                else if (value == "RURAL")
                    type = SimpleTypes.AccommodationType.RURAL;
                else if (value == "HOSTEL")
                    type = SimpleTypes.AccommodationType.HOSTEL;
                else if (value == "APTHOTEL")
                    type = SimpleTypes.AccommodationType.APTHOTEL;
                else if (value == "CAMPING")
                    type = SimpleTypes.AccommodationType.CAMPING;
                else if (value == "HISTORIC")
                    type = SimpleTypes.AccommodationType.HISTORIC;
                else if (value == "PENDING")
                    type = SimpleTypes.AccommodationType.PENDING;
                else if (value == "RESORT")
                    type = SimpleTypes.AccommodationType.RESORT;
                else if (value == "HOMES")
                    type = SimpleTypes.AccommodationType.HOMES;

                return type;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }
        }

        public class ReviewsTypeConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                SimpleTypes.ReviewsType type = (SimpleTypes.ReviewsType)value;

                switch (type)
                {
                    case SimpleTypes.ReviewsType.TRIPDAVISOR:
                        writer.WriteValue("TRIPDAVISOR");
                        break;
                    case SimpleTypes.ReviewsType.HOTELBEDS:
                        writer.WriteValue("HOTELBEDS");
                        break;
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string value = (string)reader.Value;
                SimpleTypes.ReviewsType? type = null;

                if (value == "TRIPDAVISOR")
                    type = SimpleTypes.ReviewsType.TRIPDAVISOR;
                else if (value == "HOTELBEDS")
                    type = SimpleTypes.ReviewsType.HOTELBEDS;

                return type;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }
        }

        public class BooleanConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                bool type = (bool)value;
                switch (type)
                {
                    case true:
                        writer.WriteValue("true");
                        break;
                    case false:
                        writer.WriteValue("false");
                        break;
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var included = (string)reader.Value;
                bool? type = null;

                if (included == "true")
                    type = true;
                else if (included == "false")
                    type = false;

                return type;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }
        }
    }
}
