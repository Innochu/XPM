namespace XPMTest.Application.Service.Constants
{
    public static class StatusCodes
    {
        public const string Successful = "00";
        public const string GeneralError = "06";
        public const string ModelValidationError = "09";
        public const string FatalError = "96";
        public const string NoRecordFound = "25";
        public const string DuplicateRecord = "26";
        public const string InsufficientFunds = "51";
        public const string UnAuthorized = "401";
        public const string BadRequest = "403";
        public const string TransferFailed = "10";
        public const string ExpiredToken = "489";

    }
}