using TrueSight.Common;
using Tracnghiem.Common;
using Tracnghiem.Helpers;
using TrueSight.PER.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Tracnghiem.Repositories;
using Tracnghiem.Entities;
using Tracnghiem.Enums;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Tracnghiem.Services.MMail;
using Tracnghiem.Handlers.Configuration;

namespace Tracnghiem.Services.MAppUser
{
    public interface IAppUserService :  IServiceScoped
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<AppUser> Get(long Id);
        Task<AppUser> Create(AppUser AppUser);
        Task<AppUser> UserCreate(AppUser AppUser);
        Task<AppUser> Update(AppUser AppUser);
        Task<AppUser> Delete(AppUser AppUser);
        Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers);
        Task<List<AppUser>> BulkMerge(List<AppUser> AppUsers);
        Task<AppUserFilter> ToFilter(AppUserFilter AppUserFilter);
        Task<AppUser> Login(AppUser AppUser);
        Task<AppUser> ChangePassword(AppUser AppUser);
        Task<AppUser> ForgotPassword(AppUser AppUser);
        Task<AppUser> RecoveryPassword(AppUser AppUser);
        Task<AppUser> RecoveryPasswordByOTP(AppUser AppUser);
        Task<AppUser> AdminChangePassword(AppUser AppUser);
        Task<AppUser> RefreshToken(AppUser AppUser);


    }

    public class AppUserService : BaseService, IAppUserService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IConfiguration Configuration;
        private IMailService MailService;
        private IRabbitManager RabbitManager;

        private IAppUserValidator AppUserValidator;

        public AppUserService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IAppUserValidator AppUserValidator,
            ILogging Logging,
            IConfiguration Configuration,
            IMailService MailService,
            IRabbitManager RabbitManager

        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
            this.Configuration = Configuration;
            this.MailService = MailService;
            this.RabbitManager = RabbitManager;
           
            this.AppUserValidator = AppUserValidator;
        }

        public async Task<int> Count(AppUserFilter AppUserFilter)
        {
            try
            {
                int result = await UOW.AppUserRepository.Count(AppUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return 0;
        }

        public async Task<List<AppUser>> List(AppUserFilter AppUserFilter)
        {
            try
            {
                List<AppUser> AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                return AppUsers;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(Id);
            if (AppUser == null)
                return null;
            await AppUserValidator.Get(AppUser);
            return AppUser;
        }
        
        public async Task<AppUser> Create(AppUser AppUser)
        {
            if (!await AppUserValidator.Create(AppUser))
                return AppUser;

            try
            {
                AppUser.Id = 0;
                //var Password = GeneratePassword();
                AppUser.Password = HashPassword(AppUser.Password);

                await UOW.AppUserRepository.Create(AppUser);

                AppUser = await Get(AppUser.Id);

                //Mail mail = new Mail
                //{
                //    Subject = "Create AppUser",
                //    Body = $"Your account has been created at {StaticParams.DateTimeNow.AddHours(7).ToString("HH:mm:ss dd-MM-yyyy")} Username: {AppUser.Username} Password: {Password}",
                //    Recipients = new List<string> { AppUser.Email },
                //    RowId = Guid.NewGuid()
                //};
                //RabbitManager.PublishSingle(mail, RoutingKeyEnum.MailSend.Code);
                return AppUser;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> UserCreate(AppUser AppUser)
        {

            if (!await AppUserValidator.Create(AppUser))
                return AppUser;

            try
            {
                AppUser.Id = 0;
                //var Password = GeneratePassword();
                AppUser.Password = HashPassword(AppUser.Password);

                await UOW.AppUserRepository.Create(AppUser);
                Role Role = (await UOW.RoleRepository.List(new RoleFilter
                {
                    Code = new StringFilter
                    {
                        Equal = RoleEnum.UserRole.Code
                    },
                    Skip = 0,
                    Take = 1,
                    Selects = RoleSelect.Id
                })).FirstOrDefault();


                if (Role == null)
                    return AppUser;
                Role = await UOW.RoleRepository.Get(Role.Id);
                Role.AppUserRoleMappings.Add(new AppUserRoleMapping
                {
                    AppUserId = AppUser.Id,
                    RoleId = Role.Id
                });
                await UOW.RoleRepository.Update(Role);
                AppUser = await Get(AppUser.Id);


                Mail mail = new Mail
                {
                    Subject = "Đăng ký tài khoản thành công",
                    Body = $"Chúc mừng bạn {AppUser.DisplayName} đă đăng ký tài khoản thành công vào lúc {StaticParams.DateTimeNow.AddHours(7).ToString("HH:mm:ss dd-MM-yyyy")}!",
                    RecipientDisplayName = AppUser.DisplayName,
                    RecipientEmail = AppUser.Email,
                    Id = Guid.NewGuid()
                };
                //await MailService.SendEmails(new List<Mail> { mail });
                RabbitManager.PublishSingle(mail, RoutingKeyEnum.MailSend.Code);
                return AppUser;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }
        public async Task<AppUser> Update(AppUser AppUser)
        {
            if (!await AppUserValidator.Update(AppUser))
                return AppUser;
            try
            {
                var oldData = await UOW.AppUserRepository.Get(AppUser.Id);

                await UOW.AppUserRepository.Update(AppUser);

                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                return AppUser;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Delete(AppUser AppUser)
        {
            if (!await AppUserValidator.Delete(AppUser))
                return AppUser;

            try
            {
                await UOW.AppUserRepository.Delete(AppUser);
                return AppUser;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.BulkDelete(AppUsers))
                return AppUsers;

            try
            {
                await UOW.AppUserRepository.BulkDelete(AppUsers);
                return AppUsers;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<List<AppUser>> BulkMerge(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.Import(AppUsers))
                return AppUsers;
            try
            {
                var Ids = await UOW.AppUserRepository.BulkMerge(AppUsers);
                AppUsers = await UOW.AppUserRepository.List(Ids);
                return AppUsers;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Login(AppUser AppUser)
        {
            if (!await AppUserValidator.Login(AppUser))
                return AppUser;
            //var AppUserDeviceName = AppUser.DeviceName;
            AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
            //AppUser.DeviceName = AppUserDeviceName;
            CurrentContext.UserId = AppUser.Id;
            AppUser.Token = CreateToken(AppUser.Id, AppUser.Username);
            AppUser.RefreshToken = CreateRefreshToken(AppUser);
            //await GetSite(AppUser);
            //AppUserSession AppUserSession = new AppUserSession
            //{
            //    AppUserId = AppUser.Id,
            //    //DeviceName = AppUser.DeviceName,
            //    RefreshToken = AppUser.RefreshToken,
            //    StatusId = StatusEnum.ACTIVE.Id
            //};
            //await UOW.AppUserSessionRepository.Create(AppUserSession);
            return AppUser;
        }
        public async Task<AppUser> ChangePassword(AppUser AppUser)
        {
            //if (!await AppUserValidator.ChangePassword(AppUser))
            //    return AppUser;
            try
            {
                AppUser oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                oldData.Password = HashPassword(AppUser.NewPassword);

                await UOW.AppUserRepository.Update(oldData);

                var newData = await UOW.AppUserRepository.Get(AppUser.Id);

                //Mail mail = new Mail
                //{
                //    Subject = "Change Password AppUser",
                //    Body = $"Your password has been changed at {StaticParams.DateTimeNow.AddHours(7).ToString("HH:mm:ss dd-MM-yyyy")}",
                //    Recipients = new List<string> { newData.Email },
                //    RowId = Guid.NewGuid()
                //};
                //RabbitManager.PublishSingle(mail, RoutingKeyEnum.MailSend.Code);
                return newData;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }
        public async Task<AppUser> ForgotPassword(AppUser AppUser)
        {
            int TimeOTPExpired = 5;
            if (!await AppUserValidator.ForgotPassword(AppUser))
                return AppUser;
            try
            {
                AppUser oldData = (await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = 1,
                    Email = new StringFilter { Equal = AppUser.Email},
                    Selects = AppUserSelect.ALL
                })).FirstOrDefault();

                CurrentContext.UserId = oldData.Id;

                oldData.OtpCode = GenerateOTPCode();
                oldData.OtpExpired = StaticParams.DateTimeNow.AddMinutes(TimeOTPExpired);


                await UOW.AppUserRepository.Update(oldData);

                var newData = await UOW.AppUserRepository.Get(oldData.Id);

                Mail mail = new Mail
                {
                    Subject = "Mã khôi phục mật khẩu",
                    Body = $"Mã khôi phục mật khẩu là <strong>{newData.OtpCode}</strong>, mã sẽ hết hạn trong {TimeOTPExpired} phút, lúc {newData.OtpExpired?.ToString("HH:mm:ss dd-MM-yyyy")}",
                    RecipientDisplayName = newData.DisplayName,
                    RecipientEmail = newData.Email,
                    Id = Guid.NewGuid()
                };
                //await MailService.SendEmails(new List<Mail> { mail });
                RabbitManager.PublishSingle(mail, RoutingKeyEnum.MailSend.Code);

                return newData;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }
        public async Task<AppUser> RecoveryPassword(AppUser AppUser)
        {
            if (AppUser.Id == 0)
                return null;
            try
            {
                AppUser oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                CurrentContext.UserId = AppUser.Id;
                oldData.Password = HashPassword(AppUser.Password);

                await UOW.AppUserRepository.Update(oldData);


                var newData = await UOW.AppUserRepository.Get(oldData.Id);

                //Mail mail = new Mail
                //{
                //    Subject = "Recovery Password",
                //    Body = $"Your password has been recovered.",
                //    Recipients = new List<string> { newData.Email },
                //    RowId = Guid.NewGuid()
                //};
                //RabbitManager.PublishSingle(mail, RoutingKeyEnum.MailSend.Code);
                return newData;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }
        public async Task<AppUser> RecoveryPasswordByOTP(AppUser AppUser)
        {
            if (!await AppUserValidator.RecoveryPasswordByOTP(AppUser))
            {
                return AppUser;
            }
            try
            {
                AppUserFilter AppUserFilter = new AppUserFilter();
                AppUserFilter.Email = new StringFilter { Equal = AppUser.Email };
                AppUserFilter.Skip = 0;
                AppUserFilter.Take = 1;
                AppUserFilter.Selects = AppUserSelect.Id;
                
                var AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                AppUser = AppUsers.FirstOrDefault();
                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                CurrentContext.UserId = AppUser.Id;
                AppUser.Token = CreateToken(AppUser.Id, AppUser.Username);
                AppUser.RefreshToken = CreateRefreshToken(AppUser);
                return AppUser;

            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }

            return null;
        }

        public async Task<AppUser> AdminChangePassword(AppUser AppUser)
        {
            //if (!await AppUserValidator.AdminChangePassword(AppUser))
            //    return AppUser;
            try
            {
                var oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                oldData.Password = HashPassword(AppUser.NewPassword);

                await UOW.AppUserRepository.Update(oldData);


                //Mail mail = new Mail
                //{
                //    Subject = "Change Password AppUser",
                //    Body = $"Your new password is {AppUser.NewPassword}",
                //    Recipients = new List<string> { AppUser.Email },
                //    RowId = Guid.NewGuid()
                //};
                //RabbitManager.PublishSingle(mail, RoutingKeyEnum.MailSend.Code);
                return AppUser;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }
        public async Task<AppUser> RefreshToken(AppUser AppUser)
        {
            string encryptedRefreshToken = AppUser.RefreshToken;
            RefreshTokenObject RefreshTokenObject = ReadRefreshToken(encryptedRefreshToken);
            if (RefreshTokenObject != null)
            {
                AppUser.Id = RefreshTokenObject.AppUserId;
                AppUser.Username = RefreshTokenObject.AppUserUsername;
                //AppUser.DeviceName = RefreshTokenObject.DeviceName;
            }
            AppUser.RefreshTokenObject = RefreshTokenObject;
            //if (!await AppUserValidator.RefreshToken(AppUser))
            //{
            //    return AppUser;
            //}
            AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
            CurrentContext.UserId = AppUser.Id;
            AppUser.Token = CreateToken(AppUser.Id, AppUser.Username);
            return AppUser;
        }
        public async Task<AppUserFilter> ToFilter(AppUserFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<AppUserFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                AppUserFilter subFilter = new AppUserFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == "AppUserId")
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Username))
                        subFilter.Username = FilterBuilder.Merge(subFilter.Username, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DisplayName))
                        subFilter.DisplayName = FilterBuilder.Merge(subFilter.DisplayName, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Password))
                        subFilter.Password = FilterBuilder.Merge(subFilter.Password, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RefreshToken))
                        subFilter.RefreshToken = FilterBuilder.Merge(subFilter.RefreshToken, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RoleId))
                        subFilter.RoleId = FilterBuilder.Merge(subFilter.RoleId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ImageId))
                        subFilter.ImageId = FilterBuilder.Merge(subFilter.ImageId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }


        private string CreateToken(long id, string userName, double? expiredTime = null)
        {
            if (expiredTime == null)
                expiredTime = double.TryParse(Configuration["Config:ExpiredTime"], out double time) ? time : 0;

            string PrivateRSAKeyBase64 = Configuration["Config:PrivateRSAKey"];
            byte[] PrivateRSAKeyBytes = Convert.FromBase64String(PrivateRSAKeyBase64);
            string PrivateRSAKey = Encoding.Default.GetString(PrivateRSAKeyBytes);

            RSAParameters rsaParams;
            using (var tr = new StringReader(PrivateRSAKey))
            {
                var pemReader = new PemReader(tr);
                var keyPair = pemReader.ReadObject() as AsymmetricCipherKeyPair;
                if (keyPair == null)
                {
                    throw new Exception("Could not read RSA private key");
                }
                var privateRsaParams = keyPair.Private as RsaPrivateCrtKeyParameters;
                rsaParams = DotNetUtilities.ToRSAParameters(privateRsaParams);
            }

            RSA rsa = RSA.Create();
            rsa.ImportParameters(rsaParams);

            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };
            var jwt = new JwtSecurityToken(
                claims: new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Name, userName),
                    //new Claim(UserTypeEnum.USER_TYPE_CLAIM_TYPE, UserTypeEnum.APP_USER.Id.ToString()),
                },
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddSeconds(expiredTime.Value),
                signingCredentials: signingCredentials
            );

            string Token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Token;
        }
        private string CreateRefreshToken(AppUser AppUser)
        {
            var AesKey = Configuration["Config:AesKey"];
            var AesIV = Configuration["Config:AesIV"];
            var expiredTime = double.TryParse(Configuration["Config:RefreshTokenExpiredTime"], out double time) ? time : 0;

            RefreshTokenObject RefreshTokenObject = new RefreshTokenObject
            {
                AppUserId = AppUser.Id,
                AppUserUsername = AppUser.Username,
                //DeviceName = AppUser.DeviceName,
                Issues = StaticParams.DateTimeNow,
                Expires = StaticParams.DateTimeNow.AddSeconds(expiredTime),
            };
            string refreshTokenJson = Newtonsoft.Json.JsonConvert.SerializeObject(RefreshTokenObject);
            string encryptedRefreshToken = AesEncryptString(refreshTokenJson, AesKey, AesIV);
            return encryptedRefreshToken;
        }
        private string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
        private string GeneratePassword()
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "1234567890";
            const string special = "!@#$%^&*_-=+";

            Random _rand = new Random();
            var bytes = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(bytes);

            var res = new StringBuilder();
            foreach (byte b in bytes)
            {
                switch (_rand.Next(4))
                {
                    case 0:
                        res.Append(lower[b % lower.Count()]);
                        break;
                    case 1:
                        res.Append(upper[b % upper.Count()]);
                        break;
                    case 2:
                        res.Append(number[b % number.Count()]);
                        break;
                    case 3:
                        res.Append(special[b % special.Count()]);
                        break;
                }
            }
            return res.ToString();
        }
        private string GenerateOTPCode()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }
        private RefreshTokenObject ReadRefreshToken(string encryptedRefreshToken)
        {
            if (encryptedRefreshToken == null)
            {
                return null;
            }
            var AesKey = Configuration["Config:AesKey"];
            var AesIV = Configuration["Config:AesIV"];
            var RefreshTokenJson = AesDecryptString(encryptedRefreshToken, AesKey, AesIV);
            RefreshTokenObject res = Newtonsoft.Json.JsonConvert.DeserializeObject<RefreshTokenObject>(RefreshTokenJson);
            res.RefreshToken = encryptedRefreshToken;
            return res;
        }
        private static string AesEncryptString(string plainText, string Key, string IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(Key);
                aesAlg.IV = Convert.FromBase64String(IV);
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }
        private static string AesDecryptString(string cipherText, string Key, string IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            string plaintext = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(Key);
                aesAlg.IV = Convert.FromBase64String(IV);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
