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
using Tracnghiem.Services.MImage;

namespace Tracnghiem.Rpc.app_user
{
    public class ProfileController : RpcController
    {
        private IAppUserService AppUserService;
        private IImageService ImageService;
        private ICurrentContext CurrentContext;
        public ProfileController(
            IAppUserService AppUserService,
            ICurrentContext CurrentContext,
            IImageService ImageService
            )
        {
            this.AppUserService = AppUserService;
            this.ImageService = ImageService;
            this.CurrentContext = CurrentContext;
        }

        [AllowAnonymous]
        [Route(ProfileRoute.Login), HttpPost]
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
        //[Route(ProfileRoute.LoginByGmail), HttpPost]
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

        [Route(ProfileRoute.Logged), HttpPost]
        public bool Logged()
        {
            return true;
        }
        
        [Route(ProfileRoute.ChangePassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> ChangePassword([FromBody] AppUser_ProfileChangePasswordDTO AppUser_ProfileChangePasswordDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
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
        [Route(ProfileRoute.ForgotPassword), HttpPost]
        public async Task<ActionResult<AppUser_ForgotPassword>> ForgotPassword([FromBody] AppUser_ForgotPassword AppUser_ForgotPassword)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser AppUser = new AppUser
            {
                Email = AppUser_ForgotPassword.Email,
            };
            AppUser.BaseLanguage = CurrentContext.Language;

            AppUser = await AppUserService.ForgotPassword(AppUser);
            AppUser_ForgotPassword = new AppUser_ForgotPassword(AppUser);
            if (AppUser.IsValidated)
            {
                return AppUser_ForgotPassword;
            }
            else
                return BadRequest(AppUser_ForgotPassword);
        }

        [AllowAnonymous]
        [Route(ProfileRoute.RecoveryPasswordByOTP), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> RecoveryPasswordByOTP([FromBody] AppUser_RecoveryPasswordByOTPDTO AppUser_RecoveryPasswordByOTPDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUser AppUser = new AppUser
            {
                Email = AppUser_RecoveryPasswordByOTPDTO.Email,
                OtpCode = AppUser_RecoveryPasswordByOTPDTO.OtpCode,
                BaseLanguage = "vi",
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.RecoveryPasswordByOTP(AppUser);
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


        [Route(ProfileRoute.RecoveryPassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> RecoveryPassword([FromBody] AppUser_RecoveryPassword AppUser_RecoveryPassword)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUser AppUser = new AppUser
            {
                Id = CurrentContext.UserId,
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

        //[Route(ProfileRoute.SaveImage), HttpPost]
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

        //[Route(ProfileRoute.GetForWeb), HttpPost]
        //public async Task<ActionResult<AppUser_AppUserDTO>> GetForWeb()
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);
        //    if (!IsAppUser())
        //        return Unauthorized();
        //    var UserId = ExtractUserId();
        //    AppUser AppUser = await AppUserService.Get(UserId);
        //    AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
        //    return AppUser_AppUserDTO;
        //}

        [Route(ProfileRoute.Get), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> GetMe()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUser AppUser = await AppUserService.Get(CurrentContext.UserId);
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

        [Route(ProfileRoute.Update), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> UpdateMe([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            //if (!IsAppUser())
            //    return Unauthorized();
            //this.CurrentContext.UserId = ExtractUserId();
            AppUser OldData = await AppUserService.Get(CurrentContext.UserId);
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

        [Route(ProfileRoute.UpdateLimit), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> UpdateLimit([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            //if (!IsAppUser())
            //    return Unauthorized();
            //this.CurrentContext.UserId = ExtractUserId();
            AppUser OldData = await AppUserService.Get(CurrentContext.UserId);
            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser.Id = CurrentContext.UserId;
            //AppUser.AppUserSiteMappings = OldData.AppUserSiteMappings;
            AppUser = await AppUserService.UpdateLimit(AppUser);
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
            //AppUser.BaseLanguage = CurrentContext.Language;
            return AppUser;
        }

        [AllowAnonymous]
        [Route(ProfileRoute.RefreshToken), HttpPost]
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

        [Route(ProfileRoute.SaveImage), HttpPost]
        public async Task<ActionResult<AppUser_ImageDTO>> UploadImage(IFormFile file)
        {

            Image Image = await ImageService.Create(file);
            if (!Image.IsValidated)
            {
                return BadRequest(new AppUser_ImageDTO(Image));
            }
            else
                return new AppUser_ImageDTO(Image);

        }
        //[Route(ProfileRoute.CreateSession), HttpPost]
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
