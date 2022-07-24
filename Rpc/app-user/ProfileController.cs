using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tracnghiem.Common;
using Tracnghiem.Entities;
using Tracnghiem.Enums;
using Tracnghiem.Services.MAppUser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueSight.PER.Entities;

namespace Tracnghiem.Rpc.app_user
{
    public class ProfileRoot
    {
        public const string Login = "rpc/tracnghiem/account/login";
        public const string LoginByGmail = "rpc/tracnghiem/account/login-by-gmail";
        public const string LDAPLogin = "rpc/tracnghiem/account/ldap-login";
        public const string Logged = "rpc/tracnghiem/account/logged";
        public const string GetForWeb = "rpc/tracnghiem/profile-web/get";
        public const string Get = "rpc/tracnghiem/profile/get";
        public const string GetDraft = "rpc/tracnghiem/profile/get-draft";
        public const string Update = "rpc/tracnghiem/profile/update";
        public const string SaveImage = "rpc/tracnghiem/profile/save-image";
        public const string ChangePassword = "rpc/tracnghiem/profile/change-password";
        public const string ForgotPassword = "rpc/tracnghiem/profile/forgot-password";
        public const string VerifyOtpCode = "rpc/tracnghiem/profile/verify-otp-code";
        public const string RecoveryPassword = "rpc/tracnghiem/profile/recovery-password";
        public const string SingleListSex = "rpc/tracnghiem/profile/single-list-sex";
        public const string SingleListProvince = "rpc/tracnghiem/profile/single-list-province";
        public const string ListSite = "rpc/tracnghiem/profile/list-site";
        public const string RefreshToken = "rpc/tracnghiem/profile/refresh-token";
        public const string CreateSession = "rpc/tracnghiem/profile/create-session";
        public const string UpdateCurrentProject = "rpc/tracnghiem/profile/update-current-project";
    }

    [Authorize]
    public class ProfileController : ControllerBase
    {
        private IAppUserService AppUserService;
        //private ISexService SexService;
        //private ISiteService SiteService;
        private ICurrentContext CurrentContext;
        public ProfileController(
            IAppUserService AppUserService,
            //ISexService SexService,
            //ISiteService SiteService,
            ICurrentContext CurrentContext
            )
        {
            this.AppUserService = AppUserService;
            //this.SexService = SexService;
            //this.SiteService = SiteService;
            this.CurrentContext = CurrentContext;
        }

        [AllowAnonymous]
        [Route(ProfileRoot.Login), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Login([FromBody] AppUser_LoginDTO AppUser_LoginDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUser AppUser = new AppUser
            {
                Username = AppUser_LoginDTO.Username,
                Password = AppUser_LoginDTO.Password,
                //DeviceName = AppUser_LoginDTO.DeviceName,
                //IdToken = AppUser_LoginDTO.IdToken,
                BaseLanguage = "vi",
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.Login(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);

            if (AppUser.IsValidated)
            {
                Response.Cookies.Append("Token", AppUser.Token);
                Response.Cookies.Append("RefreshToken", AppUser.RefreshToken);
                AppUser_AppUserDTO.Token = AppUser.Token;
                AppUser_AppUserDTO.RefreshToken = AppUser.RefreshToken;
                return AppUser_AppUserDTO;
            }
            else
                return BadRequest(AppUser_AppUserDTO);
        }


        //[AllowAnonymous]
        //[Route(ProfileRoot.LoginByGmail), HttpPost]
        //public async Task<ActionResult<AppUser_AppUserDTO>> LoginByGmail([FromBody] AppUser_AuthDTO AppUser_AuthDTO)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);
        //    if (ModelState.IsValid && !string.IsNullOrEmpty(AppUser_AuthDTO.IdToken))
        //    {
        //        AppUser AppUser = await AppUserService.LoginByGmail(AppUser_AuthDTO.IdToken);
        //        AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);

        //        if (AppUser.IsValidated)
        //        {
        //            Response.Cookies.Append("Token", AppUser.Token);
        //            Response.Cookies.Append("RefreshToken", AppUser.RefreshToken);
        //            AppUser_AppUserDTO.Token = AppUser.Token;
        //            AppUser_AppUserDTO.RefreshToken = AppUser.RefreshToken;
        //            return AppUser_AppUserDTO;
        //        }
        //        else
        //            return BadRequest(new AppUser_AppUserDTO());
        //    }

        //    return BadRequest();
        //}

        [Route(ProfileRoot.Logged), HttpPost]
        public bool Logged()
        {
            return true;
        }
        [Route(ProfileRoot.ChangePassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> ChangePassword([FromBody] AppUser_ProfileChangePasswordDTO AppUser_ProfileChangePasswordDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (!IsAppUser())
                return Unauthorized();
            this.CurrentContext.UserId = ExtractUserId();
            AppUser AppUser = new AppUser
            {
                Id = CurrentContext.UserId,
                Password = AppUser_ProfileChangePasswordDTO.OldPassword,
                NewPassword = AppUser_ProfileChangePasswordDTO.NewPassword,
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.ChangePassword(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        #region Forgot Password
        [AllowAnonymous]
        [Route(ProfileRoot.ForgotPassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> ForgotPassword([FromBody] AppUser_ForgotPassword AppUser_ForgotPassword)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser AppUser = new AppUser
            {
                Email = AppUser_ForgotPassword.Email,
            };
            AppUser.BaseLanguage = CurrentContext.Language;

            AppUser = await AppUserService.ForgotPassword(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
            {
                return AppUser_AppUserDTO;
            }
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(ProfileRoot.RecoveryPassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> RecoveryPassword([FromBody] AppUser_RecoveryPassword AppUser_RecoveryPassword)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (!IsAppUser())
                return Unauthorized();
            var UserId = ExtractUserId();
            AppUser AppUser = new AppUser
            {
                Id = UserId,
                Password = AppUser_RecoveryPassword.Password,
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.RecoveryPassword(AppUser);
            if (AppUser == null)
                return Unauthorized();
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            return AppUser_AppUserDTO;
        }
        #endregion

        //[Route(ProfileRoot.SaveImage), HttpPost]
        //public async Task<ActionResult<Image>> SaveImage(IFormFile file)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);
        //    if (!IsAppUser())
        //        return Unauthorized();
        //    MemoryStream memoryStream = new MemoryStream();
        //    file.CopyTo(memoryStream);
        //    Image Image = new Image
        //    {
        //        Name = file.FileName,
        //        Content = memoryStream.ToArray()
        //    };
        //    CurrentContext.Token = Request.Cookies["Token"];
        //    Image = await AppUserService.SaveImage(Image);
        //    return Image;
        //}

        [Route(ProfileRoot.GetForWeb), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> GetForWeb()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (!IsAppUser())
                return Unauthorized();
            var UserId = ExtractUserId();
            AppUser AppUser = await AppUserService.Get(UserId);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            return AppUser_AppUserDTO;
        }

        [Route(ProfileRoot.Get), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> GetMe()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (!IsAppUser())
                return Unauthorized();
            var UserId = ExtractUserId();
            AppUser AppUser = await AppUserService.Get(UserId);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            //if (AppUser_AppUserDTO.AppUserSiteMappings != null)
            //    foreach (var AppUserSiteMapping in AppUser_AppUserDTO.AppUserSiteMappings)
            //    {
            //        if (AppUserSiteMapping.Site != null)
            //        {
            //            AppUserSiteMapping.Site.LightIcon = "";
            //            AppUserSiteMapping.Site.LightLogo = "";
            //            AppUserSiteMapping.Site.DarkIcon = "";
            //            AppUserSiteMapping.Site.DarkLogo = "";
            //        }
            //    }
            return AppUser_AppUserDTO;
        }

        [Route(ProfileRoot.Update), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> UpdateMe([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (!IsAppUser())
                return Unauthorized();
            this.CurrentContext.UserId = ExtractUserId();
            AppUser OldData = await AppUserService.Get(this.CurrentContext.UserId);
            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser.Id = CurrentContext.UserId;
            //AppUser.AppUserSiteMappings = OldData.AppUserSiteMappings;
            AppUser = await AppUserService.Update(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        private bool IsAppUser()
        {
            long UserTypeId = long.TryParse(User.FindFirst(c => c.Type == UserTypeEnum.USER_TYPE_CLAIM_TYPE).Value, out long u) ? u : 0;
            return UserTypeId == UserTypeEnum.APP_USER.Id;
        }
        private long ExtractUserId()
        {
            return long.TryParse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, out long u) ? u : 0;
        }
        private AppUser ConvertDTOToEntity(AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            AppUser AppUser = new AppUser();
            AppUser.Id = AppUser_AppUserDTO.Id;
            AppUser.Username = AppUser_AppUserDTO.Username;
            AppUser.Password = AppUser_AppUserDTO.Password;
            AppUser.DisplayName = AppUser_AppUserDTO.DisplayName;
            AppUser.ImageId = AppUser_AppUserDTO.ImageId;
            //AppUser.Avatar = AppUser_AppUserDTO.Avatar;
            //AppUser.Birthday = AppUser_AppUserDTO.Birthday;
            //AppUser.Email = AppUser_AppUserDTO.Email;
            //AppUser.Phone = AppUser_AppUserDTO.Phone;
            //AppUser.OrganizationId = AppUser_AppUserDTO.OrganizationId;
            //AppUser.SexId = AppUser_AppUserDTO.SexId;
            //AppUser.StatusId = AppUser_AppUserDTO.StatusId;
          
            //AppUser.Status = AppUser_AppUserDTO.Status == null ? null : new Status
            //{
            //    Id = AppUser_AppUserDTO.Status.Id,
            //    Code = AppUser_AppUserDTO.Status.Code,
            //    Name = AppUser_AppUserDTO.Status.Name,
            //};
            //AppUser.AppUserRoleMappings = AppUser_AppUserDTO.AppUserRoleMappings?
            //    .Select(x => new AppUserRoleMapping
            //    {
            //        RoleId = x.RoleId,
            //        Role = x.Role == null ? null : new Role
            //        {
            //            Id = x.Role.Id,
            //            Code = x.Role.Code,
            //            Name = x.Role.Name,
            //            StatusId = x.Role.StatusId,
            //        },
            //    }).ToList();
            //AppUser.AppUserSiteMappings = AppUser_AppUserDTO.AppUserSiteMappings?
            //    .Select(x => new AppUserSiteMapping
            //    {
            //        AppUserId = x.AppUserId,
            //        SiteId = x.SiteId,
            //        Enabled = x.Enabled
            //    }).ToList();
            AppUser.BaseLanguage = CurrentContext.Language;
            return AppUser;
        }

        [AllowAnonymous]
        [Route(ProfileRoot.RefreshToken), HttpPost]
        public async Task<ActionResult<AppUser_RefreshTokenResultDTO>> RefreshToken()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUser AppUser = new AppUser
            {
                RefreshToken = Request.Cookies["RefreshToken"],
            };
            AppUser.BaseLanguage = "vi";
            AppUser = await AppUserService.RefreshToken(AppUser);
            AppUser_RefreshTokenResultDTO AppUser_RefreshTokenResultDTO = new AppUser_RefreshTokenResultDTO
            {
                Token = AppUser.Token,
                Errors = AppUser.Errors
            };
            if (AppUser.IsValidated)
            {
                Response.Cookies.Append("Token", AppUser.Token);
                return AppUser_RefreshTokenResultDTO;
            }
            else
                return BadRequest(AppUser_RefreshTokenResultDTO);
        }

        //[Route(ProfileRoot.CreateSession), HttpPost]
        //public async Task<ActionResult<AppUser_RefreshTokenResultDTO>> CreateSession()
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);
        //    long UserId = long.TryParse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, out long u) ? u : 0;
        //    AppUser AppUser = await AppUserService.CreateSession(UserId);
        //    AppUser_RefreshTokenResultDTO AppUser_RefreshTokenResultDTO = new AppUser_RefreshTokenResultDTO
        //    {
        //        RefreshToken = AppUser.RefreshToken,
        //        Token = AppUser.Token,
        //        Errors = AppUser.Errors
        //    };
        //    if (AppUser.IsValidated)
        //    {
        //        return AppUser_RefreshTokenResultDTO;
        //    }
        //    else
        //        return BadRequest(AppUser_RefreshTokenResultDTO);
        //}
    }
}
