using PrivateAPI.HelperClasses;
namespace PrivateAPI.Entities
{
    public class AuthorizationData : Converter
    {
        public string LoginStr { get { return LoginStr; } set { Login = StrToIntArrayToByteArray(value); } }
        public string PasswordStr { get { return PasswordStr; } set { Password = StrToIntArrayToByteArray(value); } }
        public string EmailStr { get { return EmailStr; } set { Email = StrToIntArrayToByteArray(value); } }
        public string? DeviceIdStr { get { return DeviceIdStr; } set { DeviceId = StrToIntArrayToByteArray(value); } }
        public byte[] Login{ get; private set; }
        public byte[] Password { get; private set; }
        public byte[] Email { get; private set; }
        public byte[] DeviceId{ get; private set; }
    }
}
