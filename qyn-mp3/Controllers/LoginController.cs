using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using qyn_mp3.Models;
using qyn_mp3.Models.Binding;
using qyn_mp3.Models.ViewModels;
using qyn_mp3.Repositories;
using qyn_mp3.Services;

namespace qyn_mp3.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;      //Quản lý người dùng (tạo, xóa, tìm kiếm...)
        private readonly SignInManager<ApplicationUser> _signInManager;  //Quản lý đăng nhập/đăng xuất
        private readonly DBContext _dbContext;
        private readonly ILogger<LoginController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IOtpService _otpService;

        public LoginController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            DBContext dbContext,
            ILogger<LoginController> logger,
            IEmailSender emailSender,
            IOtpService otpService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _logger = logger;
            _emailSender = emailSender;
            _otpService = otpService;
        }


        //GET Login: Hiển thị form đăng nhập, xử lý returnUrl nếu có
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Truyền ReturnUrl vào ViewModel
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl ?? Url.Content("~/") // Nếu null thì mặc định về trang chủ
            };
            return View(model);
        }

        //POST Login: Xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                // 1. Xử lý ReturnUrl
                model.ReturnUrl = model.ReturnUrl ?? Url.Content("~/");

                // 2. Validate ModelState
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"ModelState invalid. Errors: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
                    return View(model);
                }

                // 3. Tìm user bằng username hoặc email
                var user = await _userManager.FindByNameAsync(model.Username) ??
                          await _userManager.FindByEmailAsync(model.Username);

                if (user == null)
                {
                    _logger.LogWarning($"User not found: {model.Username}");
                    ModelState.AddModelError(string.Empty, "Incorrect account or password.");
                    TempData["error"] = JsonSerializer.Serialize("Incorrect account or password.");
                    return View(model);
                }

                // 4. Thử đăng nhập
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    model.Password,
                    isPersistent: model.RememberMe, // Sử dụng giá trị từ checkbox
                    lockoutOnFailure: false);

                // 5. Xử lý kết quả
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {user.UserName} logged in successfully");
                    //TempData["success"] = JsonSerializer.Serialize("Đăng nhập thành công");

                    // Kiểm tra nếu ReturnUrl là trang Login thì chuyển hướng sang Home
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        var isLoginUrl = model.ReturnUrl.Equals("/Login/Login", StringComparison.OrdinalIgnoreCase) ||
                                        model.ReturnUrl.Equals("/", StringComparison.OrdinalIgnoreCase);

                        if (isLoginUrl)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning($"User {user.UserName} locked out");
                    ModelState.AddModelError(string.Empty, "Account temporarily locked due to too many failed login attempts.");
                    return View(model);
                }
                else
                {
                    _logger.LogWarning($"Invalid password for user {user.UserName}");
                    ModelState.AddModelError(string.Empty, "Incorrect account or password");
                    TempData["error"] = JsonSerializer.Serialize("Incorrect account or password");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ModelState.AddModelError(string.Empty, "An error occurred while logging in.");
                TempData["error"] = JsonSerializer.Serialize("An error occurred while logging in.");
                return View(model);
            }
        }


        [HttpGet]
        //Đăng kí tài khoản
        public IActionResult SignUp()
        {
            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning($"ModelState invalid: {string.Join(", ", errors)}");
                return View(model);
            }

            // Kiểm tra OTP trước khi đăng ký
            if (!_otpService.ValidateOtp(model.Email, model.Otp))
            {
                ModelState.AddModelError("Otp", "OTP code is invalid or expired");
                _logger.LogWarning($"OTP invalid email: {model.Email}");
                TempData["error"] = JsonSerializer.Serialize("OTP code is invalid or expired.");
                return View(model);
            }

            // Kiểm tra email đã tồn tại chưa (thêm để chắc chắn)
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            var existingUsername = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser != null || existingUsername != null)
            {
                ModelState.AddModelError("Email", "Email or username already in used.");
                TempData["error"] = JsonSerializer.Serialize("Email or username already in used.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,

            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("SignUpSuccess"); // Chuyển hướng đến trang thành công
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                _logger.LogError($"Error: {error.Description}");
                TempData["error"] = JsonSerializer.Serialize("Error");
            }

            // Xử lý đăng ký...
            return RedirectToAction("SignUpSuccess");
        }


        // GET: Trang thành công
        [HttpGet]
        public IActionResult SignUpSuccess()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendSignUpOtp([FromBody] EmailRequest request)
        {
            try
            {
                _logger.LogInformation($"Data received: {JsonSerializer.Serialize(request)}");

                // Kiểm tra request null
                if (request == null)
                {
                    _logger.LogWarning("Request body is null");
                    return BadRequest(new { success = false, message = "Invalid request body" });
                }

                // Kiểm tra ModelState
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning($"ModelState invalid: {string.Join(", ", errors)}");
                    return BadRequest(new { success = false, message = "Data invalid", errors });
                }

                // Kiểm tra email đã tồn tại chưa
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user != null)
                {
                    return Ok(new { success = false, message = "Email are used." });
                }

                // Gửi OTP
                var otp = _otpService.GenerateOtp(request.Email);
                await _emailSender.SendEmailAsync(
                    request.Email,
                    "Mã OTP đăng ký tài khoản",
                    $"Mã OTP của bạn là: {otp}. Mã có hiệu lực trong 5 phút.");

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi OTP");
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }


        // GET: Giao diện nhập email để gửi OTP
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Xử lý yêu cầu gửi OTP
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Không tiết lộ user không tồn tại
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            // Tạo và gửi OTP
            var otp = _otpService.GenerateOtp(model.Email);

            // Gửi email chứa OTP
            await _emailSender.SendEmailAsync(
                model.Email,
                "Mã OTP đặt lại mật khẩu",
                $"Mã OTP của bạn là: {otp}. Mã có hiệu lực trong 5 phút.");

            return RedirectToAction("VerifyOtp", new { email = model.Email });
        }

        // GET: Giao diện nhập OTP
        [HttpGet]
        public IActionResult VerifyOtp(string email)
        {
            var model = new VerifyOtpViewModel { Email = email };
            return View(model);
        }

        // POST: Xác thực OTP
        [HttpPost]
        public IActionResult VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng nhập mã OTP hợp lệ."
                });
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                return Json(new
                {
                    success = false,
                    message = "Phiên làm việc đã hết hạn. Vui lòng thử lại."
                });
            }

            if (!_otpService.ValidateOtp(model.Email, model.Otp))
            {
                _logger.LogWarning($"OTP không hợp lệ cho email: {model.Email}");
                return Json(new
                {
                    success = false,
                    message = "Mã OTP không hợp lệ hoặc đã hết hạn!"
                });
            }

            HttpContext.Session.SetString("OTP_Verified", model.Email);
            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("ResetPassword", new { email = model.Email })
            });
        }


        [HttpPost]
        public async Task<IActionResult> ResendOtp(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return Json(new { success = false, message = "Email không hợp lệ" });
                }

                // Kiểm tra email có tồn tại trong hệ thống không (cho chức năng quên mật khẩu)
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Json(new { success = false, message = "Email không tồn tại trong hệ thống" });
                }

                // Gửi OTP mới
                var otp = _otpService.GenerateOtp(email);
                await _emailSender.SendEmailAsync(
                    email,
                    "Mã OTP đặt lại mật khẩu",
                    $"Mã OTP mới của bạn là: {otp}. Mã có hiệu lực trong 5 phút.");

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi lại OTP");
                return Json(new { success = false, message = "Đã xảy ra lỗi hệ thống" });
            }
        }


        // GET: Giao diện đặt lại mật khẩu
        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            // Kiểm tra xác thực OTP
            if (HttpContext.Session.GetString("OTP_Verified") != email)
            {
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel { Email = email };
            return View(model);
        }

        // POST: Xử lý đặt lại mật khẩu
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Kiểm tra xác thực OTP
            if (HttpContext.Session.GetString("OTP_Verified") != model.Email)
            {
                return RedirectToAction("ForgotPassword");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            // Đặt lại mật khẩu
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

            if (result.Succeeded)
            {
                // Xóa session sau khi đổi mật khẩu thành công
                HttpContext.Session.Remove("OTP_Verified");
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }




        // Các action confirmation
        public IActionResult ForgotPasswordConfirmation() => View();
        public IActionResult ResetPasswordConfirmation() => View();

    }



}

