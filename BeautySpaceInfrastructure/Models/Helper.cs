namespace BeautySpaceInfrastructure.Models
{
    public static class Helper
    {
        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                return $"({phoneNumber.Substring(3, 3)}) {phoneNumber.Substring(6, 3)}-{phoneNumber.Substring(9, 2)}-{phoneNumber.Substring(11)}";
            }
            return string.Empty;
        }
    }
}
