using Common.Exceptions;
using Common.Sabre.Hotels.CloseSession;
//using TE.HotelBeds.ServiceCatalogues.HotelCatalog.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.HotelBeds.ServiceCatalogues.HotelCatalog.Enums;

namespace TE.HotelBeds.Provider.Exceptions
{
    public class HotelBedsProviderException : DynamicProviderException
    {
        public HotelBedsProviderException(Fault fault)
            : base(ProviderTypes.HotelBeds.ToString(), fault.faultcode.Name, fault.faultstring, null, null)
        {
            InitializeSingleMessage();
        }

        public HotelBedsProviderException(string errorCode, string errorMessage)
            : base(ProviderTypes.HotelBeds.ToString(), errorCode, errorMessage, null, null)
        {
            InitializeSingleMessage();
        }

        private void InitializeSingleMessage()
        {
            var messages2 = new List<ExceptionMessage>();

            messages2.Add(
                new ExceptionMessage
                { Code = ProviderCode, Message = ProviderMessage, Severity = MessageSeverity.Error });

            messages = messages2;
        }

        //public HotelBedsProviderException(IApplicationResults applicationResults)
        //    : base(ProviderTypes.Sabre.ToString(), null, null)
        //{
        //    // Check if there is a single error
        //    if (applicationResults.Error != null && applicationResults.Error.Length == 1
        //        && applicationResults.Error.Single().SystemSpecificResults != null
        //        && applicationResults.Error.Single().SystemSpecificResults.Length == 1
        //        && applicationResults.Error.Single().SystemSpecificResults.Single().Message != null
        //        && applicationResults.Error.Single().SystemSpecificResults.Single().Message.Length == 1)
        //    {
        //        var errorMessage =
        //            applicationResults.Error.Single().SystemSpecificResults.Single().Message.Single();
        //        ProviderCode = errorMessage.code;
        //        ProviderMessage = errorMessage.Value;
        //        this.InitializeSingleMessage();
        //        return;
        //    }

        //    // Check if there are only many of the same message
        //    if (applicationResults.Error != null
        //        && applicationResults.Error.GroupBy(
        //            e =>
        //            e.SystemSpecificResults.Single().Message.Single().code
        //            + e.SystemSpecificResults.Single().Message.Single().Value).Count() == 1)
        //    {
        //        var errorMessage =
        //            applicationResults.Error.First().SystemSpecificResults.Single().Message.Single();
        //        ProviderCode = errorMessage.code;
        //        ProviderMessage = errorMessage.Value;
        //        this.InitializeSingleMessage();
        //        return;
        //    }

        //    // Convert all messages since there are multiple
        //    var uniqueMessages = new List<ExceptionMessage>();

        //    if (applicationResults.Error != null)
        //    {
        //        var errors =
        //        applicationResults.Error.Where(
        //            s => s.SystemSpecificResults?.FirstOrDefault()?.Message?.FirstOrDefault() != null);
        //        foreach (var g in
        //            errors.GroupBy(
        //                e =>
        //                e.SystemSpecificResults.Single().Message.Single().code
        //                + e.SystemSpecificResults.Single().Message.Single().Value))
        //        {
        //            var message2 = g.First().SystemSpecificResults.Single().Message.Single();
        //            uniqueMessages.Add(
        //                new ExceptionMessage
        //                { Code = message2.code, Message = message2.Value, Severity = MessageSeverity.Error });
        //        }
        //    }

        //    if (applicationResults.Warning != null)
        //    {
        //        var warnings =
        //        applicationResults.Warning.Where(
        //            s => s.SystemSpecificResults?.FirstOrDefault()?.Message?.FirstOrDefault() != null);
        //        foreach (var g in
        //            warnings.GroupBy(
        //                e =>
        //                e.SystemSpecificResults.Single().Message.Single().code
        //                + e.SystemSpecificResults.Single().Message.Single().Value))
        //        {
        //            var message2 = g.First().SystemSpecificResults.Single().Message.Single();
        //            uniqueMessages.Add(
        //                new ExceptionMessage
        //                { Code = message2.code, Message = message2.Value, Severity = MessageSeverity.Warning });
        //        }
        //    }

        //    if (applicationResults.Success != null)
        //    {
        //        var successes =
        //        applicationResults.Success.Where(
        //            s => s.SystemSpecificResults?.FirstOrDefault()?.Message?.FirstOrDefault() != null);
        //        foreach (var g in
        //            successes.GroupBy(
        //                e =>
        //                e.SystemSpecificResults.Single().Message.Single().code
        //                + e.SystemSpecificResults.Single().Message.Single().Value))
        //        {
        //            var message2 = g.First().SystemSpecificResults.Single().Message.Single();
        //            uniqueMessages.Add(
        //                new ExceptionMessage
        //                { Code = message2.code, Message = message2.Value, Severity = MessageSeverity.Message });
        //        }
        //    }
        //    messages = uniqueMessages;
        //}

        public enum KnownError
        {
            PnrNotFound = 1,
            VerifyAccountLineCount = 2,
            FlightUnavailable = 3,
            FareBasisNotFound = 4,
            PnrUpdatedIgnoreAndRetry
        };

        private const string PnrNotFoundErrorCode = "700201";

        private const string PnrNotFoundMessage = "PNR not found";

        private const string FareBasisNotFoundMessage = "FARE BASIS NOT FOUND FOR CITYPAIR/DATE";

        private const string PnrUpdatedIgnoreAndRetryMessage = "PNR HAS BEEN UPDATED-IGN AND RETRY?";

        /// <summary>
        /// This error can occur when end-transacting after ticketing, it occurs if not all passengers are ticketed at the same time.
        /// </summary>
        private const string VerifyAccountingLineCountMessageWarning = "*PAC TO VERIFY CORRECT NBR OF ACCTG LINES - THEN ET TO CONTINUE";

        public enum HotelKnownError
        {
            NoListings = 1
        };

        /// <summary>
        /// When no results are available Sabre returns this as an error message.
        /// </summary>
        private const string NoListingsMessage = "NO LISTING THIS CTY";

        /// <summary>
        /// Checks if the response has a known error/warning/message.
        /// </summary>
        /// <param name="errorType">Expected error.</param>
        /// <param name="onlyErrors">Only return true if an error occured, ignore warnings/messages.</param>
        /// <returns>True if the error/warning/message occured.</returns>
        public override bool HasKnownError(Enum errorType, bool onlyErrors = true)
        {
            // Match on error code & partial message
            if (Equals(errorType, KnownError.PnrNotFound)
                && messages.Any(
                    m =>
                    (!onlyErrors || m.Severity == MessageSeverity.Error)
                    && m.Code.Equals(PnrNotFoundErrorCode, StringComparison.InvariantCultureIgnoreCase)
                    && m.Message.IndexOf(PnrNotFoundMessage, 0, StringComparison.InvariantCultureIgnoreCase) != -1))
            {
                return true;
            }

            //if (Equals(errorType, KnownError.FlightUnavailable)
            //    && messages.Any(
            //        m =>
            //        (!onlyErrors || m.Severity == MessageSeverity.Error)
            //        && m.Code.Equals(
            //            SabreAirConstants.UnavailableFlightsWarningCode,
            //            StringComparison.InvariantCultureIgnoreCase)
            //        && m.Message.Equals(
            //            SabreAirConstants.UnavailableFlightsWarningMessage,
            //            StringComparison.InvariantCultureIgnoreCase)))
            //{
            //    return true;
            //}

            // Match on message, there is no code
            if (Equals(errorType, KnownError.VerifyAccountLineCount)
                && messages.Any(
                    m =>
                    (!onlyErrors || m.Severity == MessageSeverity.Error)
                    && m.Message.Equals(
                        VerifyAccountingLineCountMessageWarning,
                        StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }

            if (Equals(errorType, KnownError.FareBasisNotFound)
                && messages.Any(
                    m =>
                    (!onlyErrors || m.Severity == MessageSeverity.Error)
                    && m.Message.IndexOf(FareBasisNotFoundMessage, StringComparison.InvariantCultureIgnoreCase) != -1))
            {
                return true;
            }

            if (Equals(errorType, KnownError.PnrUpdatedIgnoreAndRetry)
                && messages.Any(
                    m =>
                    (!onlyErrors || m.Severity == MessageSeverity.Error)
                    && m.Message.StartsWith(
                        PnrUpdatedIgnoreAndRetryMessage,
                        StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }

            if (Equals(errorType, HotelKnownError.NoListings)
                && messages.Any(
                    m =>
                    (!onlyErrors || m.Severity == MessageSeverity.Error)
                    && m.Message.IndexOf(NoListingsMessage, StringComparison.InvariantCultureIgnoreCase) != -1))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Throws the exception if the application response contains a error, warning and/or message which matches the specified type.
        /// </summary>
        /// <param name="errorType">Expected error type.</param>
        public void ThrowForKnownWarning(KnownError errorType)
        {
            if (HasKnownError(errorType, false))
            {
                throw this;
            }
        }

        /// <summary>
        /// Factory like method for reading application results so warnings can be thrown as exceptions.
        /// </summary>
        /// <param name="results">ApplicationResults from Sabre response.</param>
        /// <returns>ProviderException.</returns>
        //public static HotelBedsProviderException ParseApplicationResults(IApplicationResults results)
        //{
        //    return new HotelBedsProviderException(results);
        //}
    }
}
