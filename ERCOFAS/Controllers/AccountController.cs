using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NetStarter.Helpers;
using NetStarter.Models;
using NetStarter.Resources;
using reCaptcha;
using RestSharp;

namespace NetStarter.Controllers
{
    /// <summary>
    /// The account controller
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        #region Variables

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private GeneralController _general = new GeneralController();
        public DefaultDBContext _db = new DefaultDBContext();
        private const string XsrfKey = "XsrfId";

        #endregion Variables

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        public AccountController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="userManager">The application user manager.</param>
        /// <param name="signInManager">The application sign-in manager.</param>
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        #endregion Constructor

        #region Properties

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion Properties

        #region Get

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            PreRegistrationViewModel model = new PreRegistrationViewModel
            {
                RERClassifications = _db.GlobalOptionSets.Where(x => x.Type == "RERClassification").OrderBy(x => x.OptionOrder).ToList(),
                RequiredDocuments = (from t1 in _db.RequiredDocuments
                                     join t2 in _db.Requirements on t1.RequirementsId equals t2.Id
                                     select new RequiredDocumentsViewModel()
                                     {
                                         Id = t1.Id,
                                         Name = t2.Name,
                                         RERType = t1.RERType
                                     }).ToList()
            };

            model.CountrySelectList = _general.GetCountries("63");

            List<string> documents = new List<string>();
            foreach(var document in model.RequiredDocuments)
                documents.Add(document.Name);

            Session["Documents"] = documents;

            return View(model);
        }

        // GET: /Account/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }


        // GET: /Account/EmailVerification
        [AllowAnonymous]
        public ActionResult EmailVerification()
        {
            return View();
        }

        // GET: /Account/OTP
        [AllowAnonymous]
        public ActionResult OTP(string key)
        {
            try
            {
                if (Session["request-otp"] != null)
                {
                    if ((bool)Session["request-otp"] == true)
                    {
                        TempData["NotifySuccess"] = "An OTP code has been sent to your mobile.";
                        Session.Remove("request-otp");
                    }
                }

                var preRegistrationEmail = _db.PreRegistrationEmails.Where(a => a.Id == key).FirstOrDefault();
                if (preRegistrationEmail != null)
                {
                    if (!preRegistrationEmail.IsVerified)
                    {
                        PreRegistrationEmails email = _db.PreRegistrationEmails.Where(a => a.Id == key).FirstOrDefault();
                        email.IsVerified = true;
                        email.VerifiedOn = DateTime.Now;
                        _db.Entry(email).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var otpNotification = _db.Notifications.FirstOrDefault(x => x.NotificationTypeId == "C9EF9CFD-A92F-4E26-99D3-903CD2E3096D");
                    if (otpNotification != null)
                    {
                        var preRegistrationMobile = _db.PreRegistrationMobiles.FirstOrDefault(x => x.Id == key);
                        if (preRegistrationMobile != null)
                        {
                            var preRegistration = _db.PreRegistration.Where(a => a.Id == preRegistrationMobile.PreRegistrationId).FirstOrDefault();

                            if (!preRegistrationMobile.IsSent)
                            {
                                string name = preRegistration.RERTypeId == "AE2DCD91-DACC-4C75-A2A3-51644ABE69BB" ? preRegistration.JuridicalEntityName : preRegistration.FirstName;
                                string content = otpNotification.Content;
                                content = content.Replace("{stakeholder}", name);
                                content = content.Replace("{code}", preRegistrationMobile.OneTimePassword);
                                SMSHelpers.SendOneTimePassword(string.Format("{0}{1}", preRegistrationMobile.CountryCode, preRegistrationMobile.MobileNumber), content);

                                preRegistrationMobile.IsSent = true;
                                _db.Entry(preRegistrationMobile).State = EntityState.Modified;
                                _db.SaveChanges();
                            }

                            ViewBag.MobileNumber = preRegistrationMobile.MobileNumber;

                            if (preRegistrationEmail.IsVerified && preRegistrationMobile.IsVerified)
                                return Redirect("/Account/ThankYou");
                        }
                    }
                    Session["key"] = key;
                }
                else
                    return Redirect("/Account/InvalidLink");
            }
            catch (Exception ex)
            {
                LoggerHelpers.Log(string.Format("{0} ERROR | {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"), ex.InnerException));
            }
            return View();
        }

        // GET: /Account/OTP
        [AllowAnonymous]
        public ActionResult RequestOTP()
        {
            string key = (string)Session["key"];
            string oneTimePassword = CodeGenerator.GenerateOneTimePassword(6);

            try
            {
                var otpNotification = _db.Notifications.FirstOrDefault(x => x.NotificationTypeId == "C9EF9CFD-A92F-4E26-99D3-903CD2E3096D");
                if (otpNotification != null)
                {
                    var preRegistrationMobile = _db.PreRegistrationMobiles.FirstOrDefault(x => x.Id == key);
                    if (preRegistrationMobile != null)
                    {
                        var preRegistration = _db.PreRegistration.Where(a => a.Id == preRegistrationMobile.PreRegistrationId).FirstOrDefault();
                        string name = preRegistration.RERTypeId == "AE2DCD91-DACC-4C75-A2A3-51644ABE69BB" ? preRegistration.JuridicalEntityName : preRegistration.FirstName;
                        string content = otpNotification.Content;
                        content = content.Replace("{stakeholder}", name);
                        content = content.Replace("{code}", oneTimePassword);
                        SMSHelpers.SendOneTimePassword(string.Format("{0}{1}", preRegistrationMobile.CountryCode, preRegistrationMobile.MobileNumber), content);

                        preRegistrationMobile.OneTimePassword = oneTimePassword;
                        _db.Entry(preRegistrationMobile).State = EntityState.Modified;
                        _db.SaveChanges();

                        Session["request-otp"] = true;
                    }
                }
                   
            }
            catch (Exception ex)
            {
                LoggerHelpers.Log(string.Format("{0} ERROR | {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"), ex.InnerException));
            }

            return Redirect(string.Format("/Account/OTP?key={0}", key));
        }

        // GET: /Account/EmailVerification
        [AllowAnonymous]
        public ActionResult InvalidLink()
        {
            return View();
        }

        // GET: /Account/ThankYou
        [AllowAnonymous]
        public ActionResult ThankYou()
        {
            return View();
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewBag.Recaptcha = ReCaptcha.GetHtml(ConfigurationManager.AppSettings["ReCaptcha:SiteKey"]);
            ViewBag.publicKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];

            return View();
        }

        // GET: /Account/ConfirmResetPassword
        [AllowAnonymous]
        public ActionResult ConfirmResetPassword()
        {
            return View();
        }

        // GET: /Account/ConfirmResetPassword
        [AllowAnonymous]
        public ActionResult InvalidResetPasswordLink()
        {
            return View();
        }

        // GET: /Account/MyProfile
        public ActionResult MyProfile()
        {
            UserProfileViewModel model = new UserProfileViewModel();
            string currentUserId = User.Identity.GetUserId();
            model = _general.GetCurrentUserProfile(currentUserId);
            return View(model);
        }

        // GET: /Account/EditMyProfile
        public ActionResult EditMyProfile()
        {
            UserProfileViewModel model = new UserProfileViewModel();
            string currentUserId = User.Identity.GetUserId();
            model = _general.GetCurrentUserProfile(currentUserId);
            ViewBag.DateFormat = _general.GetAppSettingsValue("dateFormat");
            ViewBag.dateFormatJs = _general.GetAppSettingsValue("dateFormatJs");
            SetupSelectLists(model);
            return View(model);
        }

        #endregion Get

        #region Post

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string userid = "";

            // if username input contains @ sign, means that user use email to login
            if (model.UserName.Contains("@"))
            {
                // select the UserName of the user from AspNetUsers table and assign to model.UserName because instead of email, SignInManager use username to sign in 
                model.UserName = _db.AspNetUsers.Where(a => a.Email == model.UserName).Select(a => a.UserName).DefaultIfEmpty("").FirstOrDefault();
                userid = _db.AspNetUsers.Where(a => a.Email == model.UserName).Select(a => a.Id).DefaultIfEmpty("").FirstOrDefault();
            }
            else
            {
                userid = _db.AspNetUsers.Where(a => a.UserName == model.UserName).Select(a => a.Id).DefaultIfEmpty("").FirstOrDefault();
            }

            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    //save login history
                    _general.SaveLoginHistory(userid);
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    var userTypesId = _db.AspNetUserTypes.Where(x => x.UserId == userid).ToList();
                    if (userTypesId != null)
                    {
                        string roleId = userTypesId.Select(x => x.UserTypeId).FirstOrDefault();
                        string roleName = _db.GlobalOptionSets.FirstOrDefault(x => x.Type == "UserType" && x.Id == roleId).DisplayName;

                        Session["UserTypes"] = userTypesId;
                    }
                    return RedirectToAction("index", "dashboard");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", Resource.InvalidLoginAttempt);
                    return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (_general.GetAppSettingsValue("environment") == "prod" && User.Identity.GetUserName() == "nsadmin")
            {
                TempData["NotifyFailed"] = Resource.NotAllowToChangePasswordForDemoAccount;
                return RedirectToAction("index", "dashboard");
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                TempData["NotifySuccess"] = Resource.PasswordChangedSuccessfully;
                return RedirectToAction("index", "dashboard");
            }
            AddErrors(result);
            return View(model);
        }

        // POST: /Account/PreRegister
        /// <summary>
        /// Pre-register the individual or juridical entity.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PreRegister(PreRegistrationViewModel model, FormCollection collection)
        {
            string preRegistrationId = Guid.NewGuid().ToString();

            if (model.RERTypeId == "CA4ECCA6-63E0-4F84-92CC-301323C1D4F9")
            {
                PreRegistration preRegistration = new PreRegistration
                {
                    Id = preRegistrationId,
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    RERTypeId = model.RERTypeId,
                    CreatedOn = DateTime.Now
                };
                _db.PreRegistration.Add(preRegistration);
                await _db.SaveChangesAsync();

                int index = 0;
                foreach (HttpPostedFileBase file in model.File)
                {
                    if (file != null)
                    {
                        var attachment = AttachmentHelpers.SaveToDirectory(file);
                        await SaveAttachment(preRegistrationId, attachment.Item3, attachment.Item1, attachment.Item2, index);
                    }
                    index++;
                }

                string emailId = Guid.NewGuid().ToString();
                string emailAddress = string.Empty;

                var emailNotification = await _db.Notifications.FirstOrDefaultAsync(x => x.NotificationTypeId == "CC764087-5B80-480A-B148-EBFF0969C6A2");
                if (emailNotification != null)
                {
                    string oneTimePassword = CodeGenerator.GenerateOneTimePassword(6);
                    await SaveEmailAddress(preRegistrationId, emailId, model.EmailAddress);
                    await SaveMobileNumber(preRegistrationId, emailId, model.CountryCode, model.MobileNumber, oneTimePassword);

                    emailAddress = model.EmailAddress;
                }

                emailNotification.Content = emailNotification.Content.Replace("{stakeholder}", preRegistration.FirstName);
                emailNotification.Content = emailNotification.Content.Replace("{baseurl}", _db.Settings.FirstOrDefault(x => x.Id == "FA85FB3A-2A1E-47F8-9B76-6536F6A95ABB").BaseUrl);
                emailNotification.Content = emailNotification.Content.Replace("{key}", emailId);
                EmailHelpers.SendEmail(emailAddress, emailNotification.Subject, emailNotification.Content);
            }
            else
            {
                PreRegistration preRegistration = new PreRegistration
                {
                    Id = preRegistrationId,
                    RERTypeId = model.RERTypeId,
                    RERClassificationId = model.RERClassificationId,
                    JuridicalEntityName = model.JuridicalEntityName,
                    OfficeAddress = model.OfficeAddress,
                    OfficeTelephone = model.OfficeTelephone,
                    LiaisonOfficer1 = model.LiaisonOfficer1,
                    LiaisonOfficer2 = model.LiaisonOfficer2,
                    CreatedOn = DateTime.Now
                };
                _db.PreRegistration.Add(preRegistration);
                await _db.SaveChangesAsync();

                int index = 0;
                foreach (HttpPostedFileBase file in model.File)
                {
                    if (file != null)
                    {
                        var attachment = AttachmentHelpers.SaveToDirectory(file);
                        await SaveAttachment(preRegistrationId, attachment.Item3, attachment.Item1, attachment.Item2, index);
                    }
                    index++;
                }
                            
                List<string> emailAddresses = new List<string>
                {
                    model.EmailAddress1,
                    model.EmailAddress2,
                    model.EmailAddress3,
                    model.EmailAddress4,
                    model.EmailAddress5
                };

                Dictionary<string, string> emailDictionary = new Dictionary<string, string>();

                int emailAddressOrder = 1;
                foreach (var emailAddress in emailAddresses)
                {
                    string emailId = Guid.NewGuid().ToString();
                    string oneTimePassword = CodeGenerator.GenerateOneTimePassword(6);

                    await SaveEmailAddress(preRegistrationId, emailId, emailAddress, emailAddressOrder);

                    if (emailAddressOrder == 1)
                        await SaveMobileNumber(preRegistrationId, emailId, model.CountryCode, model.MobileNumber1, oneTimePassword, emailAddressOrder);

                    if (emailAddressOrder == 2)
                        await SaveMobileNumber(preRegistrationId, emailId, model.CountryCode, model.MobileNumber2, oneTimePassword, emailAddressOrder);

                    if (emailAddressOrder == 1 || emailAddressOrder == 2)
                        emailDictionary.Add(emailId, emailAddress);

                    emailAddressOrder++;
                }

                int emailDictionaryOrder = 1;
                foreach (var item in emailDictionary)
                {
                    var emailNotification = await _db.Notifications.FirstOrDefaultAsync(x => x.NotificationTypeId == "CC764087-5B80-480A-B148-EBFF0969C6A2");
                    if (emailNotification != null)
                    {
                        string content = emailNotification.Content;
                        content = content.Replace("{stakeholder}", preRegistration.JuridicalEntityName);
                        content = content.Replace("{baseurl}", _db.Settings.FirstOrDefault(x => x.Id == "FA85FB3A-2A1E-47F8-9B76-6536F6A95ABB").BaseUrl);
                        content = content.Replace("{key}", item.Key);
                        EmailHelpers.SendEmail(item.Value, emailNotification.Subject, content);
                    }
                    emailDictionaryOrder++;
                }
            }
            return Redirect("/Account/EmailVerification");
        }

        // POST: /Account/ValidateOTP
        /// <summary>
        /// Validates the one time password entered.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ValidateOTP(FormCollection collection)
        {
            string key = (string)Session["key"];
            string oneTimePassword = string.Format("{0}{1}{2}{3}{4}{5}", collection["digit-1"], collection["digit-2"], collection["digit-3"],
                collection["digit-4"], collection["digit-5"], collection["digit-6"]);

            var preRegistrationMobile = _db.PreRegistrationMobiles.FirstOrDefault(x => x.Id == key);
            if (preRegistrationMobile.OneTimePassword == oneTimePassword)
            {
                PreRegistrationMobiles mobile = _db.PreRegistrationMobiles.Where(a => a.Id == key).FirstOrDefault();
                mobile.IsVerified = true;
                mobile.VerifiedOn = DateTime.Now;
                _db.Entry(mobile).State = EntityState.Modified;
                _db.SaveChanges();

                var emailNotification = _db.Notifications.FirstOrDefault(x => x.NotificationTypeId == "A8B44931-29EE-49A4-80F0-957CE684FBAF");
                if (emailNotification != null)
                {
                    var preRegistration = _db.PreRegistration.Where(a => a.Id == preRegistrationMobile.PreRegistrationId).FirstOrDefault();
                    var preRegistrationEmail = _db.PreRegistrationEmails.Where(a => a.Id == key).FirstOrDefault();
                    string name = preRegistration.RERTypeId == "AE2DCD91-DACC-4C75-A2A3-51644ABE69BB" ? preRegistration.JuridicalEntityName : preRegistration.FirstName;
                    string content = emailNotification.Content;
                    content = content.Replace("{stakeholder}", name);
                    content = content.Replace("{emailaddress}", preRegistrationEmail.EmailAddress);
                    content = content.Replace("{mobilenumber}", preRegistrationMobile.MobileNumber);
                    content = content.Replace("{baseurl}", _db.Settings.FirstOrDefault(x => x.Id == "FA85FB3A-2A1E-47F8-9B76-6536F6A95ABB").BaseUrl);
                    EmailHelpers.SendEmail(preRegistrationEmail.EmailAddress, emailNotification.Subject, content);
                }

                return Redirect("/Account/ThankYou");
            }
            else
                TempData["NotifyFailed"] = "Invalid OTP code entered. Please try again.";

            return Redirect(String.Format("/Account/OTP?key={0}", key));
        }

        [HttpPost]
        public ActionResult EditMyProfile(UserProfileViewModel model, HttpPostedFileBase ProfilePicture)
        {
            ValidateEditMyProfile(model);

            //These 2 fields can only be edited by system admin in user management section. When normal user edit profile from here, these 2 fields are not required.
            ModelState.Remove("UserStatusId");
            ModelState.Remove("UserRoleIdList");
            ModelState.Remove("Password");

            if (!ModelState.IsValid)
            {
                ViewBag.DateFormat = _general.GetAppSettingsValue("dateFormat");
                ViewBag.dateFormatJs = _general.GetAppSettingsValue("dateFormatJs");
                SetupSelectLists(model);
                return View(model);
            }

            bool result = SaveMyProfile(model);
            if (result == false)
            {
                TempData["NotifyFailed"] = Resource.FailedExceptionError;
            }
            else
            {
                ModelState.Clear();
                TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            }
            return RedirectToAction("myprofile");
        }

        #endregion Post

        public void SetupSelectLists(UserProfileViewModel model)
        {
            model.GenderSelectList = _general.GetGlobalOptionSets("Gender", model.GenderId);
            model.CountrySelectList = _general.GetCountryList(model.CountryName);
        }

        public void ValidateEditMyProfile(UserProfileViewModel model)
        {
            if (model != null)
            {
                bool usernameExist = _general.UsernameExists(model.Username, model.AspNetUserId);
                bool emailExist = _general.EmailExists(model.EmailAddress, model.AspNetUserId);
                if (usernameExist)
                {
                    ModelState.AddModelError("UserName", Resource.UsernameTaken);
                }
                if (emailExist)
                {
                    ModelState.AddModelError("EmailAddress", Resource.EmailAddressTaken);
                }
            }
        }

        public void AssignUserProfileValues(UserProfile userProfile, UserProfileViewModel model)
        {
            userProfile.FullName = model.FullName;
            userProfile.FirstName = model.FirstName;
            userProfile.LastName = model.LastName;
            userProfile.DateOfBirth = model.DateOfBirth;
            userProfile.PhoneNumber = model.PhoneNumber;
            userProfile.IDCardNumber = model.IDCardNumber;
            userProfile.GenderId = model.GenderId;
            userProfile.CountryName = model.CountryName;
            userProfile.PostalCode = model.PostalCode;
            userProfile.Address = model.Address;
            userProfile.ModifiedBy = model.ModifiedBy;
            userProfile.ModifiedOn = _general.GetSystemTimeZoneDateTimeNow();
            userProfile.IsoUtcModifiedOn = _general.GetIsoUtcNow();
        }

        public string GetMyProfilePictureName(string userid)
        {
            string fileName = "";
            if (!string.IsNullOrEmpty(userid))
            {
                string upId = _general.GetUserProfileId(userid);
                string profilePictureTypeId = _general.GetGlobalOptionSetId(ProjectEnum.UserAttachment.ProfilePicture.ToString(), "UserAttachment");
                fileName = _db.UserAttachments.Where(a => a.UserProfileId == upId && a.AttachmentTypeId == profilePictureTypeId).OrderByDescending(o => o.CreatedOn).Select(a => a.UniqueFileName).FirstOrDefault();
            }
            return fileName;
        }

        public bool SaveMyProfile(UserProfileViewModel model)
        {
            bool result = true;
            if (model != null)
            {
                try
                {
                    model.ModifiedBy = User.Identity.GetUserId();
                    //edit
                    if (model.Id != null)
                    {
                        //save user profile
                        UserProfile userProfile = _db.UserProfiles.FirstOrDefault(a => a.Id == model.Id);
                        AssignUserProfileValues(userProfile, model);
                        _db.Entry(userProfile).State = EntityState.Modified;
                        //save AspNetUsers and UserProfile
                        _db.SaveChanges();
                        if (model.ProfilePicture != null)
                        {
                            string profilePicture = _general.GetGlobalOptionSetId(ProjectEnum.UserAttachment.ProfilePicture.ToString(), "UserAttachment");
                            _general.SaveUserAttachment(model.ProfilePicture, userProfile.Id, profilePicture, User.Identity.GetUserId());
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return result;
        }

        public void RegisterUserProfile(string userId)
        {
            UserProfile userProfile = new UserProfile();
            userProfile.Id = Guid.NewGuid().ToString();
            userProfile.AspNetUserId = userId;
            userProfile.UserStatusId = _general.GetGlobalOptionSetId(ProjectEnum.UserStatus.Registered.ToString(), "UserStatus");
            userProfile.CreatedBy = userId;
            userProfile.CreatedOn = _general.GetSystemTimeZoneDateTimeNow();
            userProfile.IsoUtcCreatedOn = _general.GetIsoUtcNow();
            _db.UserProfiles.Add(userProfile);
            _db.SaveChanges();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.AspNetUsers.FirstOrDefault(a => a.Email == model.EmailAddress);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "We couldn't find any account with that email address. Please double check and try again.";
                    ViewBag.publicKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];

                    return View(model);
                }

                if (!ReCaptcha.Validate(ConfigurationManager.AppSettings["ReCaptcha:SecretKey"]))
                {
                    ViewBag.RecaptchaLastErrors = ReCaptcha.GetLastErrors(this.HttpContext);
                    ViewBag.publicKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];

                    return View(model);
                }

                var settings = _db.Settings.FirstOrDefault(x => x.Id == "FA85FB3A-2A1E-47F8-9B76-6536F6A95ABB");
                string code = Guid.NewGuid().ToString();
                string url = string.Format("{0}/Account/ResetPassword?userId={1}&code={2}", settings.BaseUrl, user.Id, code);

                user.PasswordToken = code;
                _db.SaveChanges();

                var emailNotification = _db.Notifications.FirstOrDefault(x => x.NotificationTypeId == "57217B2C-9F2A-4FD1-A973-8A0CD92F0F79");
                if (emailNotification != null)
                {
                    var profile = _db.UserProfiles.FirstOrDefault(x => x.AspNetUserId == user.Id);
                    string content = emailNotification.Content;
                    content = content.Replace("{firstname}", profile.FirstName);
                    content = content.Replace("{url}", url);
                    content = content.Replace("{supportemail}", settings.EmailAddress);
                    content = content.Replace("{baseurl}", settings.BaseUrl);
                    EmailHelpers.SendEmail(user.Email, emailNotification.Subject, content);
                }
                return RedirectToAction("ConfirmResetPassword", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
                return RedirectToAction("InvalidResetPasswordLink", "Account");

            var profile = _db.AspNetUsers.FirstOrDefault(a => a.Id == userId);
            if (profile != null)
            {
                if (profile.PasswordToken != code)
                    return RedirectToAction("InvalidResetPasswordLink", "Account");

                var settings = _db.Settings.FirstOrDefault(x => x.Id == "FA85FB3A-2A1E-47F8-9B76-6536F6A95ABB");

                ViewBag.MinPasswordLength = settings.MinPasswordLength;
                ViewBag.MinSpecialCharacters = settings.MinSpecialCharacters;
                Session["UserId"] = userId;
            } 
            else
                return RedirectToAction("InvalidResetPasswordLink", "Account");

            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            var settings = _db.Settings.FirstOrDefault(x => x.Id == "FA85FB3A-2A1E-47F8-9B76-6536F6A95ABB");
            string userId = (string)Session["UserId"];
            ViewBag.MinPasswordLength = settings.MinPasswordLength;
            ViewBag.MinSpecialCharacters = settings.MinSpecialCharacters;

            if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword))
            {
                ViewBag.ErrorMessage = "Please fill out required fields below.";

                return View();
            }

            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.ErrorMessage = "Password and confirm password mus be match.";

                return View();
            }

            if (model.Password.Length < settings.MinPasswordLength)
            {
                ViewBag.ErrorMessage = string.Format("Password must be {0} characters long.", settings.MinPasswordLength);

                return View();
            }

            if (!model.Password.Any(char.IsLower))
            {
                ViewBag.ErrorMessage = "Password must have at least 1 lower case letter.";

                return View();
            }

            if (!model.Password.Any(char.IsUpper))
            {
                ViewBag.ErrorMessage = "Password must have at least 1 upper case letter.";

                return View();
            }

            if (CountSpecialCharacters(model.Password) < settings.MinSpecialCharacters)
            {
                ViewBag.ErrorMessage = string.Format("Password must have a minimum of {0} special character(s).", settings.MinSpecialCharacters);

                return View();
            }

            var user = _db.AspNetUsers.FirstOrDefault(a => a.Id == userId);
            user.PasswordHash = PasswordHelpers.HashPassword(model.Password);
            user.PasswordToken = null;
            _db.SaveChanges();

            ModelState.Clear();
            TempData["NotifySuccess"] = Resource.YourPasswordResetSuccessfully;
            Session["UserId"] = null;
            return RedirectToAction("Login", "Account");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("login", "account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }

                if (_db != null)
                {
                    _db.Dispose();
                    _db = null;
                }

                if (_general != null)
                {
                    _general.Dispose();
                    _general = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Private 

        public async Task<bool> SaveEmailAddress(string preRegistrationId, string emailId, string emailAddress, int? order = null)
        {
            PreRegistrationEmails email = new PreRegistrationEmails()
            {
                Id = emailId,
                PreRegistrationId = preRegistrationId,
                EmailAddress = emailAddress,
                Order = order,
                IsVerified = false,
                CreatedOn = DateTime.Now
            };
            _db.PreRegistrationEmails.Add(email);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SaveMobileNumber(string preRegistrationId, string emailId, string countryCode, string mobileNumber, string oneTimePassword, int? order = null)
        {
            PreRegistrationMobiles mobile = new PreRegistrationMobiles()
            {
                Id = emailId,
                PreRegistrationId = preRegistrationId,
                CountryCode = countryCode,
                MobileNumber = mobileNumber,
                OneTimePassword = oneTimePassword,
                IsVerified = false,
                IsSent = false,
                Order = order,
                CreatedOn = DateTime.Now
            };
            _db.PreRegistrationMobiles.Add(mobile);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SaveAttachment(string preRegistrationId, string fileUrl, string fileName, string uniqueFileName, int index)
        {
            List<string> documents = (List<string>)Session["Documents"];

            PreRegistrationAttachment regAttachment = new PreRegistrationAttachment
            {
                Id = Guid.NewGuid().ToString(),
                PreRegistrationId = preRegistrationId,
                DocumentName = documents[index],
                FileUrl = fileUrl,
                FileName = fileName,
                UniqueFileName = uniqueFileName,
                IsApproved = null,
                CreatedOn = DateTime.Now
            };
            _db.PreRegistrationAttachments.Add(regAttachment);
            await _db.SaveChangesAsync();
            return true;
        }

        private int CountSpecialCharacters(string text)
        {
            char[] strArray = text.ToCharArray();
            int totalSpecialChar = 0;

            foreach (var item in strArray)
            {
                if (!char.IsLetterOrDigit(item))
                    totalSpecialChar++;
            }

            return totalSpecialChar;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Internal

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion Internal
    }
}