using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tracnghiem.Common;
using Tracnghiem.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using System.Dynamic;
using System.Net;
using NGS.Templater;
using Tracnghiem.Entities;
using Tracnghiem.Services.MAppUser;
using Tracnghiem.Services.MImage;
using Tracnghiem.Services.MRole;
using Tracnghiem.Services.MExamHistory;
using Tracnghiem.Services.MExam;

namespace Tracnghiem.Rpc.app_user
{
    public partial class AppUserController : RpcController
    {
        private IImageService ImageService;
        private IRoleService RoleService;
        private IExamHistoryService ExamHistoryService;
        private IExamService ExamService;
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        public AppUserController(
            IImageService ImageService,
            IRoleService RoleService,
            IExamHistoryService ExamHistoryService,
            IExamService ExamService,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext
        )
        {
            this.ImageService = ImageService;
            this.RoleService = RoleService;
            this.ExamHistoryService = ExamHistoryService;
            this.ExamService = ExamService;
            this.AppUserService = AppUserService;
            this.CurrentContext = CurrentContext;
        }

        [Route(AppUserRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter = await AppUserService.ToFilter(AppUserFilter);
            int count = await AppUserService.Count(AppUserFilter);
            return count;
        }

        [Route(AppUserRoute.List), HttpPost]
        public async Task<ActionResult<List<AppUser_AppUserDTO>>> List([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter = await AppUserService.ToFilter(AppUserFilter);
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<AppUser_AppUserDTO> AppUser_AppUserDTOs = AppUsers
                .Select(c => new AppUser_AppUserDTO(c)).ToList();
            return AppUser_AppUserDTOs;
        }

        [Route(AppUserRoute.Get), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Get([FromBody]AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = await AppUserService.Get(AppUser_AppUserDTO.Id);
            return new AppUser_AppUserDTO(AppUser);
        }

        [AllowAnonymous]
        [Route(AppUserRoute.Create), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Create([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Create(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.Update), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Update([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Update(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.Delete), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Delete([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Delete(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }
        
        [Route(AppUserRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter = await AppUserService.ToFilter(AppUserFilter);
            AppUserFilter.Id = new IdFilter { In = Ids };
            AppUserFilter.Selects = AppUserSelect.Id;
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = int.MaxValue;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            AppUsers = await AppUserService.BulkDelete(AppUsers);
            if (AppUsers.Any(x => !x.IsValidated))
                return BadRequest(AppUsers.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(AppUserRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            ImageFilter ImageFilter = new ImageFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ImageSelect.ALL
            };
            List<Image> Images = await ImageService.List(ImageFilter);
            RoleFilter RoleFilter = new RoleFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = RoleSelect.ALL
            };
            List<Role> Roles = await RoleService.List(RoleFilter);
            List<AppUser> AppUsers = new List<AppUser>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(AppUsers);
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int UsernameColumn = 1 + StartColumn;
                int DisplayNameColumn = 2 + StartColumn;
                int PasswordColumn = 3 + StartColumn;
                int RefreshTokenColumn = 4 + StartColumn;
                int RoleIdColumn = 5 + StartColumn;
                int ImageIdColumn = 6 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string UsernameValue = worksheet.Cells[i, UsernameColumn].Value?.ToString();
                    string DisplayNameValue = worksheet.Cells[i, DisplayNameColumn].Value?.ToString();
                    string PasswordValue = worksheet.Cells[i, PasswordColumn].Value?.ToString();
                    string RefreshTokenValue = worksheet.Cells[i, RefreshTokenColumn].Value?.ToString();
                    string RoleIdValue = worksheet.Cells[i, RoleIdColumn].Value?.ToString();
                    string ImageIdValue = worksheet.Cells[i, ImageIdColumn].Value?.ToString();
                    
                    AppUser AppUser = new AppUser();
                    AppUser.Username = UsernameValue;
                    AppUser.DisplayName = DisplayNameValue;
                    AppUser.Password = PasswordValue;
                    AppUser.RefreshToken = RefreshTokenValue;
                    Image Image = Images.Where(x => x.Id.ToString() == ImageIdValue).FirstOrDefault();
                    AppUser.ImageId = Image == null ? 0 : Image.Id;
                    AppUser.Image = Image;
                    Role Role = Roles.Where(x => x.Id.ToString() == RoleIdValue).FirstOrDefault();
                    AppUser.RoleId = Role == null ? 0 : Role.Id;
                    AppUser.Role = Role;
                    
                    AppUsers.Add(AppUser);
                }
            }
            AppUsers = await AppUserService.BulkMerge(AppUsers);
            if (AppUsers.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    AppUser AppUser = AppUsers[i];
                    if (!AppUser.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Id).Camelize()))
                            Error += AppUser.Errors[nameof(AppUser.Id).Camelize()];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Username).Camelize()))
                            Error += AppUser.Errors[nameof(AppUser.Username).Camelize()];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.DisplayName).Camelize()))
                            Error += AppUser.Errors[nameof(AppUser.DisplayName).Camelize()];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Password).Camelize()))
                            Error += AppUser.Errors[nameof(AppUser.Password).Camelize()];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.RefreshToken).Camelize()))
                            Error += AppUser.Errors[nameof(AppUser.RefreshToken).Camelize()];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.RoleId).Camelize()))
                            Error += AppUser.Errors[nameof(AppUser.RoleId).Camelize()];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.ImageId).Camelize()))
                            Error += AppUser.Errors[nameof(AppUser.ImageId).Camelize()];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(AppUserRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            var AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = int.MaxValue;
            AppUserFilter = await AppUserService.ToFilter(AppUserFilter);
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<AppUser_AppUserExportDTO> AppUser_AppUserExportDTOs = AppUsers.Select(x => new AppUser_AppUserExportDTO(x)).ToList();  
            var STT = 1;
            foreach (var AppUser_AppUserExportDTO in AppUser_AppUserExportDTOs)
            {
                AppUser_AppUserExportDTO.STT = STT++;
            }
            string path = "Templates/AppUser_Export.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();            
            Data.Data = AppUser_AppUserExportDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {    
                document.Process(Data);
            }
            return File(output.ToArray(), "application/octet-stream", "AppUser.xlsx");
        }

        [Route(AppUserRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/AppUser_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "AppUser.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter = await AppUserService.ToFilter(AppUserFilter);
            if (Id == 0)
            {

            }
            else
            {
                AppUserFilter.Id = new IdFilter { Equal = Id };
                int count = await AppUserService.Count(AppUserFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private AppUser ConvertDTOToEntity(AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            AppUser_AppUserDTO.TrimString();
            AppUser AppUser = new AppUser();
            AppUser.Id = AppUser_AppUserDTO.Id;
            AppUser.Username = AppUser_AppUserDTO.Username;
            AppUser.DisplayName = AppUser_AppUserDTO.DisplayName;
            AppUser.Email = AppUser_AppUserDTO.Email;
            AppUser.Password = AppUser_AppUserDTO.Password;
            AppUser.RefreshToken = AppUser_AppUserDTO.RefreshToken;
            AppUser.RoleId = AppUser_AppUserDTO.RoleId;
            AppUser.ImageId = AppUser_AppUserDTO.ImageId;
            AppUser.Image = AppUser_AppUserDTO.Image == null ? null : new Image
            {
                Id = AppUser_AppUserDTO.Image.Id,
                Name = AppUser_AppUserDTO.Image.Name,
                Url = AppUser_AppUserDTO.Image.Url,
            };
            AppUser.Role = AppUser_AppUserDTO.Role == null ? null : new Role
            {
                Id = AppUser_AppUserDTO.Role.Id,
                Code = AppUser_AppUserDTO.Role.Code,
                Name = AppUser_AppUserDTO.Role.Name,
            };
            AppUser.ExamHistories = AppUser_AppUserDTO.ExamHistories?
                .Select(x => new ExamHistory
                {
                    Id = x.Id,
                    ExamId = x.ExamId,
                    Times = x.Times,
                    CorrectAnswerQuantity = x.CorrectAnswerQuantity,
                    TotalQuestionQuantity = x.TotalQuestionQuantity,
                    Mark = x.Mark,
                    ExamedAt = x.ExamedAt,
                    Exam = x.Exam == null ? null : new Exam
                    {
                        Id = x.Exam.Id,
                        Code = x.Exam.Code,
                        Name = x.Exam.Name,
                        SubjectId = x.Exam.SubjectId,
                        ExamLevelId = x.Exam.ExamLevelId,
                        StatusId = x.Exam.StatusId,
                        CreatorId = x.Exam.CreatorId,
                        GradeId = x.Exam.GradeId,
                        ExamStatusId = x.Exam.ExamStatusId,
                        TotalMark = x.Exam.TotalMark,
                        TotalQuestion = x.Exam.TotalQuestion,
                        ImageId = x.Exam.ImageId,
                        Time = x.Exam.Time,
                    },
                }).ToList();
            AppUser.BaseLanguage = CurrentContext.Language;
            return AppUser;
        }

        private AppUserFilter ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.SearchBy = AppUserSearch.ALL;
            AppUserFilter.Skip = AppUser_AppUserFilterDTO.Skip;
            AppUserFilter.Take = AppUser_AppUserFilterDTO.Take;
            AppUserFilter.OrderBy = AppUser_AppUserFilterDTO.OrderBy;
            AppUserFilter.OrderType = AppUser_AppUserFilterDTO.OrderType;

            AppUserFilter.Id = AppUser_AppUserFilterDTO.Id;
            AppUserFilter.Username = AppUser_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = AppUser_AppUserFilterDTO.DisplayName;
            AppUserFilter.Password = AppUser_AppUserFilterDTO.Password;
            AppUserFilter.RefreshToken = AppUser_AppUserFilterDTO.RefreshToken;
            AppUserFilter.RoleId = AppUser_AppUserFilterDTO.RoleId;
            AppUserFilter.ImageId = AppUser_AppUserFilterDTO.ImageId;
            return AppUserFilter;
        }
    }
}

