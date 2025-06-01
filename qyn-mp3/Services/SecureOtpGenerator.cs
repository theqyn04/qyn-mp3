namespace qyn_mp3.Services
{
    public class SecureOtpGenerator : IOtpGenerator
    {
        public string GenerateOtp()
        {
            var bytes = new byte[4];
            System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
            return (BitConverter.ToUInt32(bytes, 0) % 900000 + 100000).ToString();
        }
    }

}
