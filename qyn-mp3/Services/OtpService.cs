namespace qyn_mp3.Services
{
    public class OtpService : IOtpService
    {
        private readonly ICacheService _cache;
        private readonly ILogger<OtpService> _logger;
        private readonly IOtpGenerator _otpGenerator;

        public OtpService(ICacheService cache, ILogger<OtpService> logger, IOtpGenerator otpGenerator)
        {
            _cache = cache;
            _logger = logger;
            _otpGenerator = otpGenerator;
        }

        public string GenerateOtp(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            _cache.Set($"otp_{email}", otp, TimeSpan.FromMinutes(5));

            var attempts = GetOtpAttempts(email) + 1;
            _cache.Set($"otp_attempts_{email}", attempts, TimeSpan.FromMinutes(30));
            _cache.Set($"otp_attempts_time_{email}", DateTime.Now, TimeSpan.FromMinutes(30));

            return otp;
        }

        public bool ValidateOtp(string email, string otp)
        {
            var savedOtp = _cache.Get<string>($"otp_{email}");
            return savedOtp == otp;
        }

        public int GetOtpAttempts(string email)
        {
            return _cache.Get<int>($"otp_attempts_{email}");
        }

        public int GetRemainingLockTime(string email)
        {
            if (_cache.TryGetValue($"otp_attempts_time_{email}", out DateTime creationTime))
            {
                var remaining = creationTime.AddMinutes(30) - DateTime.Now;
                return (int)Math.Max(0, remaining.TotalMinutes);
            }
            return 0;
        }

        public void ClearOtpAttempts(string email)
        {
            _cache.Remove($"otp_attempts_{email}");
            _cache.Remove($"otp_attempts_time_{email}");
        }

    }
}
