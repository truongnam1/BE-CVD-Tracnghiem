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
            AppUser.ImageId = AppUser_AppUserDTO.ImageId;
            AppUser.Image = AppUser_AppUserDTO.Image == null ? null : new Image
            {
                Id = AppUser_AppUserDTO.Image.Id,
                Name = AppUser_AppUserDTO.Image.Name,
                Url = AppUser_AppUserDTO.Image.Url,
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
            AppUserFilter.ImageId = AppUser_AppUserFilterDTO.ImageId;
            return AppUserFilter;
        }
    }
}

