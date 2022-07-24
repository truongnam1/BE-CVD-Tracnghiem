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
using Tracnghiem.Services.MExamHistory;
using Tracnghiem.Services.MAppUser;
using Tracnghiem.Services.MExam;

namespace Tracnghiem.Rpc.exam_history
{
    public partial class ExamHistoryController : RpcController
    {
        private IAppUserService AppUserService;
        private IExamService ExamService;
        private IExamHistoryService ExamHistoryService;
        private ICurrentContext CurrentContext;
        public ExamHistoryController(
            IAppUserService AppUserService,
            IExamService ExamService,
            IExamHistoryService ExamHistoryService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.ExamService = ExamService;
            this.ExamHistoryService = ExamHistoryService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ExamHistoryRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ExamHistory_ExamHistoryFilterDTO ExamHistory_ExamHistoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamHistoryFilter ExamHistoryFilter = ConvertFilterDTOToFilterEntity(ExamHistory_ExamHistoryFilterDTO);
            ExamHistoryFilter = await ExamHistoryService.ToFilter(ExamHistoryFilter);
            int count = await ExamHistoryService.Count(ExamHistoryFilter);
            return count;
        }

        [Route(ExamHistoryRoute.List), HttpPost]
        public async Task<ActionResult<List<ExamHistory_ExamHistoryDTO>>> List([FromBody] ExamHistory_ExamHistoryFilterDTO ExamHistory_ExamHistoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamHistoryFilter ExamHistoryFilter = ConvertFilterDTOToFilterEntity(ExamHistory_ExamHistoryFilterDTO);
            ExamHistoryFilter = await ExamHistoryService.ToFilter(ExamHistoryFilter);
            List<ExamHistory> ExamHistories = await ExamHistoryService.List(ExamHistoryFilter);
            List<ExamHistory_ExamHistoryDTO> ExamHistory_ExamHistoryDTOs = ExamHistories
                .Select(c => new ExamHistory_ExamHistoryDTO(c)).ToList();
            return ExamHistory_ExamHistoryDTOs;
        }

        [Route(ExamHistoryRoute.Get), HttpPost]
        public async Task<ActionResult<ExamHistory_ExamHistoryDTO>> Get([FromBody]ExamHistory_ExamHistoryDTO ExamHistory_ExamHistoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ExamHistory_ExamHistoryDTO.Id))
                return Forbid();

            ExamHistory ExamHistory = await ExamHistoryService.Get(ExamHistory_ExamHistoryDTO.Id);
            return new ExamHistory_ExamHistoryDTO(ExamHistory);
        }

        [Route(ExamHistoryRoute.Create), HttpPost]
        public async Task<ActionResult<ExamHistory_ExamHistoryDTO>> Create([FromBody] ExamHistory_ExamHistoryDTO ExamHistory_ExamHistoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ExamHistory_ExamHistoryDTO.Id))
                return Forbid();

            ExamHistory ExamHistory = ConvertDTOToEntity(ExamHistory_ExamHistoryDTO);
            ExamHistory = await ExamHistoryService.Create(ExamHistory);
            ExamHistory_ExamHistoryDTO = new ExamHistory_ExamHistoryDTO(ExamHistory);
            if (ExamHistory.IsValidated)
                return ExamHistory_ExamHistoryDTO;
            else
                return BadRequest(ExamHistory_ExamHistoryDTO);
        }

        [Route(ExamHistoryRoute.Update), HttpPost]
        public async Task<ActionResult<ExamHistory_ExamHistoryDTO>> Update([FromBody] ExamHistory_ExamHistoryDTO ExamHistory_ExamHistoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ExamHistory_ExamHistoryDTO.Id))
                return Forbid();

            ExamHistory ExamHistory = ConvertDTOToEntity(ExamHistory_ExamHistoryDTO);
            ExamHistory = await ExamHistoryService.Update(ExamHistory);
            ExamHistory_ExamHistoryDTO = new ExamHistory_ExamHistoryDTO(ExamHistory);
            if (ExamHistory.IsValidated)
                return ExamHistory_ExamHistoryDTO;
            else
                return BadRequest(ExamHistory_ExamHistoryDTO);
        }

        [Route(ExamHistoryRoute.Delete), HttpPost]
        public async Task<ActionResult<ExamHistory_ExamHistoryDTO>> Delete([FromBody] ExamHistory_ExamHistoryDTO ExamHistory_ExamHistoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ExamHistory_ExamHistoryDTO.Id))
                return Forbid();

            ExamHistory ExamHistory = ConvertDTOToEntity(ExamHistory_ExamHistoryDTO);
            ExamHistory = await ExamHistoryService.Delete(ExamHistory);
            ExamHistory_ExamHistoryDTO = new ExamHistory_ExamHistoryDTO(ExamHistory);
            if (ExamHistory.IsValidated)
                return ExamHistory_ExamHistoryDTO;
            else
                return BadRequest(ExamHistory_ExamHistoryDTO);
        }
        
        [Route(ExamHistoryRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamHistoryFilter ExamHistoryFilter = new ExamHistoryFilter();
            ExamHistoryFilter = await ExamHistoryService.ToFilter(ExamHistoryFilter);
            ExamHistoryFilter.Id = new IdFilter { In = Ids };
            ExamHistoryFilter.Selects = ExamHistorySelect.Id;
            ExamHistoryFilter.Skip = 0;
            ExamHistoryFilter.Take = int.MaxValue;

            List<ExamHistory> ExamHistories = await ExamHistoryService.List(ExamHistoryFilter);
            ExamHistories = await ExamHistoryService.BulkDelete(ExamHistories);
            if (ExamHistories.Any(x => !x.IsValidated))
                return BadRequest(ExamHistories.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(ExamHistoryRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            ExamFilter ExamFilter = new ExamFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ExamSelect.ALL
            };
            List<Exam> Exams = await ExamService.List(ExamFilter);
            List<ExamHistory> ExamHistories = new List<ExamHistory>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(ExamHistories);
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int AppUserIdColumn = 1 + StartColumn;
                int ExamIdColumn = 2 + StartColumn;
                int TimesColumn = 3 + StartColumn;
                int CorrectAnswerQuantityColumn = 4 + StartColumn;
                int TotalQuestionQuantityColumn = 5 + StartColumn;
                int MarkColumn = 6 + StartColumn;
                int ExamedAtColumn = 7 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string AppUserIdValue = worksheet.Cells[i, AppUserIdColumn].Value?.ToString();
                    string ExamIdValue = worksheet.Cells[i, ExamIdColumn].Value?.ToString();
                    string TimesValue = worksheet.Cells[i, TimesColumn].Value?.ToString();
                    string CorrectAnswerQuantityValue = worksheet.Cells[i, CorrectAnswerQuantityColumn].Value?.ToString();
                    string TotalQuestionQuantityValue = worksheet.Cells[i, TotalQuestionQuantityColumn].Value?.ToString();
                    string MarkValue = worksheet.Cells[i, MarkColumn].Value?.ToString();
                    string ExamedAtValue = worksheet.Cells[i, ExamedAtColumn].Value?.ToString();
                    
                    ExamHistory ExamHistory = new ExamHistory();
                    ExamHistory.Times = long.TryParse(TimesValue, out long Times) ? Times : 0;
                    ExamHistory.CorrectAnswerQuantity = long.TryParse(CorrectAnswerQuantityValue, out long CorrectAnswerQuantity) ? CorrectAnswerQuantity : 0;
                    ExamHistory.TotalQuestionQuantity = long.TryParse(TotalQuestionQuantityValue, out long TotalQuestionQuantity) ? TotalQuestionQuantity : 0;
                    ExamHistory.Mark = decimal.TryParse(MarkValue, out decimal Mark) ? Mark : 0;
                    ExamHistory.ExamedAt = DateTime.TryParse(ExamedAtValue, out DateTime ExamedAt) ? ExamedAt : DateTime.Now;
                    AppUser AppUser = AppUsers.Where(x => x.Id.ToString() == AppUserIdValue).FirstOrDefault();
                    ExamHistory.AppUserId = AppUser == null ? 0 : AppUser.Id;
                    ExamHistory.AppUser = AppUser;
                    Exam Exam = Exams.Where(x => x.Id.ToString() == ExamIdValue).FirstOrDefault();
                    ExamHistory.ExamId = Exam == null ? 0 : Exam.Id;
                    ExamHistory.Exam = Exam;
                    
                    ExamHistories.Add(ExamHistory);
                }
            }
            ExamHistories = await ExamHistoryService.BulkMerge(ExamHistories);
            if (ExamHistories.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < ExamHistories.Count; i++)
                {
                    ExamHistory ExamHistory = ExamHistories[i];
                    if (!ExamHistory.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (ExamHistory.Errors.ContainsKey(nameof(ExamHistory.Id).Camelize()))
                            Error += ExamHistory.Errors[nameof(ExamHistory.Id).Camelize()];
                        if (ExamHistory.Errors.ContainsKey(nameof(ExamHistory.AppUserId).Camelize()))
                            Error += ExamHistory.Errors[nameof(ExamHistory.AppUserId).Camelize()];
                        if (ExamHistory.Errors.ContainsKey(nameof(ExamHistory.ExamId).Camelize()))
                            Error += ExamHistory.Errors[nameof(ExamHistory.ExamId).Camelize()];
                        if (ExamHistory.Errors.ContainsKey(nameof(ExamHistory.Times).Camelize()))
                            Error += ExamHistory.Errors[nameof(ExamHistory.Times).Camelize()];
                        if (ExamHistory.Errors.ContainsKey(nameof(ExamHistory.CorrectAnswerQuantity).Camelize()))
                            Error += ExamHistory.Errors[nameof(ExamHistory.CorrectAnswerQuantity).Camelize()];
                        if (ExamHistory.Errors.ContainsKey(nameof(ExamHistory.TotalQuestionQuantity).Camelize()))
                            Error += ExamHistory.Errors[nameof(ExamHistory.TotalQuestionQuantity).Camelize()];
                        if (ExamHistory.Errors.ContainsKey(nameof(ExamHistory.Mark).Camelize()))
                            Error += ExamHistory.Errors[nameof(ExamHistory.Mark).Camelize()];
                        if (ExamHistory.Errors.ContainsKey(nameof(ExamHistory.ExamedAt).Camelize()))
                            Error += ExamHistory.Errors[nameof(ExamHistory.ExamedAt).Camelize()];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ExamHistoryRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ExamHistory_ExamHistoryFilterDTO ExamHistory_ExamHistoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            var ExamHistoryFilter = ConvertFilterDTOToFilterEntity(ExamHistory_ExamHistoryFilterDTO);
            ExamHistoryFilter.Skip = 0;
            ExamHistoryFilter.Take = int.MaxValue;
            ExamHistoryFilter = await ExamHistoryService.ToFilter(ExamHistoryFilter);
            List<ExamHistory> ExamHistories = await ExamHistoryService.List(ExamHistoryFilter);
            List<ExamHistory_ExamHistoryExportDTO> ExamHistory_ExamHistoryExportDTOs = ExamHistories.Select(x => new ExamHistory_ExamHistoryExportDTO(x)).ToList();  
            var STT = 1;
            foreach (var ExamHistory_ExamHistoryExportDTO in ExamHistory_ExamHistoryExportDTOs)
            {
                ExamHistory_ExamHistoryExportDTO.STT = STT++;
            }
            string path = "Templates/ExamHistory_Export.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();            
            Data.Data = ExamHistory_ExamHistoryExportDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {    
                document.Process(Data);
            }
            return File(output.ToArray(), "application/octet-stream", "ExamHistory.xlsx");
        }

        [Route(ExamHistoryRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] ExamHistory_ExamHistoryFilterDTO ExamHistory_ExamHistoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/ExamHistory_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "ExamHistory.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ExamHistoryFilter ExamHistoryFilter = new ExamHistoryFilter();
            ExamHistoryFilter = await ExamHistoryService.ToFilter(ExamHistoryFilter);
            if (Id == 0)
            {

            }
            else
            {
                ExamHistoryFilter.Id = new IdFilter { Equal = Id };
                int count = await ExamHistoryService.Count(ExamHistoryFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ExamHistory ConvertDTOToEntity(ExamHistory_ExamHistoryDTO ExamHistory_ExamHistoryDTO)
        {
            ExamHistory_ExamHistoryDTO.TrimString();
            ExamHistory ExamHistory = new ExamHistory();
            ExamHistory.Id = ExamHistory_ExamHistoryDTO.Id;
            ExamHistory.AppUserId = ExamHistory_ExamHistoryDTO.AppUserId;
            ExamHistory.ExamId = ExamHistory_ExamHistoryDTO.ExamId;
            ExamHistory.Times = ExamHistory_ExamHistoryDTO.Times;
            ExamHistory.CorrectAnswerQuantity = ExamHistory_ExamHistoryDTO.CorrectAnswerQuantity;
            ExamHistory.TotalQuestionQuantity = ExamHistory_ExamHistoryDTO.TotalQuestionQuantity;
            ExamHistory.Mark = ExamHistory_ExamHistoryDTO.Mark;
            ExamHistory.ExamedAt = ExamHistory_ExamHistoryDTO.ExamedAt;
            ExamHistory.AppUser = ExamHistory_ExamHistoryDTO.AppUser == null ? null : new AppUser
            {
                Id = ExamHistory_ExamHistoryDTO.AppUser.Id,
                Username = ExamHistory_ExamHistoryDTO.AppUser.Username,
                DisplayName = ExamHistory_ExamHistoryDTO.AppUser.DisplayName,
                Password = ExamHistory_ExamHistoryDTO.AppUser.Password,
                RefreshToken = ExamHistory_ExamHistoryDTO.AppUser.RefreshToken,
                RoleId = ExamHistory_ExamHistoryDTO.AppUser.RoleId,
                ImageId = ExamHistory_ExamHistoryDTO.AppUser.ImageId,
            };
            ExamHistory.Exam = ExamHistory_ExamHistoryDTO.Exam == null ? null : new Exam
            {
                Id = ExamHistory_ExamHistoryDTO.Exam.Id,
                Code = ExamHistory_ExamHistoryDTO.Exam.Code,
                Name = ExamHistory_ExamHistoryDTO.Exam.Name,
                SubjectId = ExamHistory_ExamHistoryDTO.Exam.SubjectId,
                ExamLevelId = ExamHistory_ExamHistoryDTO.Exam.ExamLevelId,
                StatusId = ExamHistory_ExamHistoryDTO.Exam.StatusId,
                CreatorId = ExamHistory_ExamHistoryDTO.Exam.CreatorId,
                GradeId = ExamHistory_ExamHistoryDTO.Exam.GradeId,
                ExamStatusId = ExamHistory_ExamHistoryDTO.Exam.ExamStatusId,
                TotalMark = ExamHistory_ExamHistoryDTO.Exam.TotalMark,
                TotalQuestion = ExamHistory_ExamHistoryDTO.Exam.TotalQuestion,
                ImageId = ExamHistory_ExamHistoryDTO.Exam.ImageId,
                Time = ExamHistory_ExamHistoryDTO.Exam.Time,
            };
            ExamHistory.BaseLanguage = CurrentContext.Language;
            return ExamHistory;
        }

        private ExamHistoryFilter ConvertFilterDTOToFilterEntity(ExamHistory_ExamHistoryFilterDTO ExamHistory_ExamHistoryFilterDTO)
        {
            ExamHistoryFilter ExamHistoryFilter = new ExamHistoryFilter();
            ExamHistoryFilter.Selects = ExamHistorySelect.ALL;
            ExamHistoryFilter.SearchBy = ExamHistorySearch.ALL;
            ExamHistoryFilter.Skip = ExamHistory_ExamHistoryFilterDTO.Skip;
            ExamHistoryFilter.Take = ExamHistory_ExamHistoryFilterDTO.Take;
            ExamHistoryFilter.OrderBy = ExamHistory_ExamHistoryFilterDTO.OrderBy;
            ExamHistoryFilter.OrderType = ExamHistory_ExamHistoryFilterDTO.OrderType;

            ExamHistoryFilter.Id = ExamHistory_ExamHistoryFilterDTO.Id;
            ExamHistoryFilter.AppUserId = ExamHistory_ExamHistoryFilterDTO.AppUserId;
            ExamHistoryFilter.ExamId = ExamHistory_ExamHistoryFilterDTO.ExamId;
            ExamHistoryFilter.Times = ExamHistory_ExamHistoryFilterDTO.Times;
            ExamHistoryFilter.CorrectAnswerQuantity = ExamHistory_ExamHistoryFilterDTO.CorrectAnswerQuantity;
            ExamHistoryFilter.TotalQuestionQuantity = ExamHistory_ExamHistoryFilterDTO.TotalQuestionQuantity;
            ExamHistoryFilter.Mark = ExamHistory_ExamHistoryFilterDTO.Mark;
            ExamHistoryFilter.ExamedAt = ExamHistory_ExamHistoryFilterDTO.ExamedAt;
            return ExamHistoryFilter;
        }
    }
}

