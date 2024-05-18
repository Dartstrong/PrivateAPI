using PrivateAPI.HelperClasses;
namespace PrivateAPI.Entities
{
    public class AuthorizationData : Converter
    {
        public string LoginStr { get; set; }
        public string PasswordStr { get; set; }
        public string EmailStr { get; set; }
        public string DeviceIdStr { get; set; }
    }
}
