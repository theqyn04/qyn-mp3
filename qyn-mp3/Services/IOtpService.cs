namespace qyn_mp3.Services
{
    public interface IOtpService
    {
        string GenerateOtp(string email);
        bool ValidateOtp(string email, string otp);
        int GetOtpAttempts(string email);
        int GetRemainingLockTime(string email);
        void ClearOtpAttempts(string email);
    }

}
