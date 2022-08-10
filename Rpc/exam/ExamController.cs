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
using Tracnghiem.Services.MExam;
using Tracnghiem.Services.MAppUser;
using Tracnghiem.Services.MExamLevel;
using Tracnghiem.Services.MExamStatus;
using Tracnghiem.Services.MGrade;
using Tracnghiem.Services.MImage;
using Tracnghiem.Services.MStatus;
using Tracnghiem.Services.MSubject;
using Tracnghiem.Services.MQuestion;
using Tracnghiem.Enums;
using Tracnghiem.Services.MDoingExam;

namespace Tracnghiem.Rpc.exam
{
    public partial class ExamController : RpcController
    {
        private IAppUserService AppUserService;
        private IExamLevelService ExamLevelService;
        private IExamStatusService ExamStatusService;
        private IGradeService GradeService;
        private IImageService ImageService;
        private IStatusService StatusService;
        private ISubjectService SubjectService;
        private IQuestionService QuestionService;
        private IExamService ExamService;
        private IDoingExamService DoingExamService;
        private ICurrentContext CurrentContext;
        public ExamController(
            IAppUserService AppUserService,
            IExamLevelService ExamLevelService,
            IExamStatusService ExamStatusService,
            IGradeService GradeService,
            IImageService ImageService,
            IStatusService StatusService,
            ISubjectService SubjectService,
            IQuestionService QuestionService,
            IExamService ExamService,
            IDoingExamService DoingExamService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.ExamLevelService = ExamLevelService;
            this.ExamStatusService = ExamStatusService;
            this.GradeService = GradeService;
            this.ImageService = ImageService;
            this.StatusService = StatusService;
            this.SubjectService = SubjectService;
            this.QuestionService = QuestionService;
            this.ExamService = ExamService;
            this.DoingExamService = DoingExamService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ExamRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Exam_ExamFilterDTO Exam_ExamFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamFilter ExamFilter = ConvertFilterDTOToFilterEntity(Exam_ExamFilterDTO);
            ExamFilter = await ExamService.ToFilter(ExamFilter);
            int count = await ExamService.Count(ExamFilter);
            return count;
        }

        [Route(ExamRoute.List), HttpPost]
        public async Task<ActionResult<List<Exam_ExamDTO>>> List([FromBody] Exam_ExamFilterDTO Exam_ExamFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamFilter ExamFilter = ConvertFilterDTOToFilterEntity(Exam_ExamFilterDTO);
            ExamFilter = await ExamService.ToFilter(ExamFilter);
            List<Exam> Exams = await ExamService.List(ExamFilter);
            List<Exam_ExamDTO> Exam_ExamDTOs = Exams
                .Select(c => new Exam_ExamDTO(c)).ToList();
            return Exam_ExamDTOs;
        }

        [AllowAnonymous]
        [Route(ExamRoute.PublicList), HttpPost]
        public async Task<ActionResult<List<Exam_ExamDTO>>> PublicList([FromBody] Exam_ExamFilterDTO Exam_ExamFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamFilter ExamFilter = ConvertFilterDTOToFilterEntity(Exam_ExamFilterDTO);
            ExamFilter.ExamStatusId = new IdFilter { Equal = ExamStatusEnum.Public.Id };
            ExamFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ExamFilter = await ExamService.ToFilter(ExamFilter);
            List<Exam> Exams = await ExamService.List(ExamFilter);
            List<Exam_ExamDTO> Exam_ExamDTOs = Exams
                .Select(c => new Exam_ExamDTO(c)).ToList();
            return Exam_ExamDTOs;
        }

        [Route(ExamRoute.ListExamSearchQuestion), HttpPost]
        public async Task<ActionResult<List<Exam_ExamDTO>>> ListExamSearchQuestion([FromBody] Exam_ExamFilterDTO Exam_ExamFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamFilter ExamFilter = ConvertFilterDTOToFilterEntity(Exam_ExamFilterDTO);
            ExamFilter = await ExamService.ToFilter(ExamFilter);
            List<Exam> Exams = await ExamService.ListExamSearchQuestion(ExamFilter);
            List<Exam_ExamDTO> Exam_ExamDTOs = Exams
                .Select(c => new Exam_ExamDTO(c)).ToList();
            return Exam_ExamDTOs;
        }

        [AllowAnonymous]
        [Route(ExamRoute.PublicCount), HttpPost]
        public async Task<ActionResult<int>> PublicCount([FromBody] Exam_ExamFilterDTO Exam_ExamFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamFilter ExamFilter = ConvertFilterDTOToFilterEntity(Exam_ExamFilterDTO);
            ExamFilter.ExamStatusId = new IdFilter { Equal = ExamStatusEnum.Public.Id };
            ExamFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            //ExamFilter = await ExamService.ToFilter(ExamFilter);
            int count = await ExamService.Count(ExamFilter);
            return count;
        }

        [AllowAnonymous]
        [Route(ExamRoute.MonthMostTested), HttpPost]
        public async Task<ActionResult<List<Exam_ExamDTO>>> MonthMostTested()
        {
            List<Exam> Exams = await ExamService.MonthMostTested();
            List<Exam_ExamDTO> Exam_ExamDTOs = Exams
                .Select(c => new Exam_ExamDTO(c)).ToList();
            return Exam_ExamDTOs;
        }
        [Route(ExamRoute.Get), HttpPost]
        public async Task<ActionResult<Exam_ExamDTO>> Get([FromBody]Exam_ExamDTO Exam_ExamDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Exam_ExamDTO.Id))
                return Forbid();

            Exam Exam = await ExamService.Get(Exam_ExamDTO.Id);
            return new Exam_ExamDTO(Exam);
        }

        [Route(ExamRoute.GetDetailExam), HttpPost]
        public async Task<ActionResult<Exam_ExamDTO>> GetDetailExam([FromBody] Exam_ExamDTO Exam_ExamDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Exam_ExamDTO.Id))
                return Forbid();

            Exam Exam = await DoingExamService.GetExam(Exam_ExamDTO.Id);
            return new Exam_ExamDTO(Exam);
        }

        [AllowAnonymous]
        [Route(ExamRoute.PublicGet), HttpPost]
        public async Task<ActionResult<Exam_ExamDTO>> PublicGet([FromBody] Exam_ExamDTO Exam_ExamDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Exam Exam = await ExamService.PublicGet(Exam_ExamDTO.Id);
            return new Exam_ExamDTO(Exam);
        }

        [Route(ExamRoute.Create), HttpPost]
        public async Task<ActionResult<Exam_ExamDTO>> Create([FromBody] Exam_ExamDTO Exam_ExamDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Exam_ExamDTO.Id))
                return Forbid();

            Exam Exam = ConvertDTOToEntity(Exam_ExamDTO);
            Exam = await ExamService.Create(Exam);
            Exam_ExamDTO = new Exam_ExamDTO(Exam);
            if (Exam.IsValidated)
                return Exam_ExamDTO;
            else
                return BadRequest(Exam_ExamDTO);
        }

        //[Route(ExamRoute.Send), HttpPost]

        //public async Task<ActionResult<Exam_ExamDTO>> Send([FromBody] Exam_ExamDTO Exam_ExamDTO)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);

        //    if (!await HasPermission(Exam_ExamDTO.Id))
        //        return Forbid();

        //    Exam Exam = ConvertDTOToEntity(Exam_ExamDTO);
        //    Exam = await ExamService.Send(Exam);
        //    Exam_ExamDTO = new Exam_ExamDTO(Exam);
        //    if (Exam.IsValidated)
        //        return Exam_ExamDTO;
        //    else
        //        return BadRequest(Exam_ExamDTO);
        //}

        [Route(ExamRoute.Update), HttpPost]
        public async Task<ActionResult<Exam_ExamDTO>> Update([FromBody] Exam_ExamDTO Exam_ExamDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Exam_ExamDTO.Id))
                return Forbid();

            Exam Exam = ConvertDTOToEntity(Exam_ExamDTO);
            Exam = await ExamService.Update(Exam);
            Exam_ExamDTO = new Exam_ExamDTO(Exam);
            if (Exam.IsValidated)
                return Exam_ExamDTO;
            else
                return BadRequest(Exam_ExamDTO);
        }

        [Route(ExamRoute.Delete), HttpPost]
        public async Task<ActionResult<Exam_ExamDTO>> Delete([FromBody] Exam_ExamDTO Exam_ExamDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Exam_ExamDTO.Id))
                return Forbid();

            Exam Exam = ConvertDTOToEntity(Exam_ExamDTO);
            Exam = await ExamService.Delete(Exam);
            Exam_ExamDTO = new Exam_ExamDTO(Exam);
            if (Exam.IsValidated)
                return Exam_ExamDTO;
            else
                return BadRequest(Exam_ExamDTO);
        }
        
        [Route(ExamRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamFilter ExamFilter = new ExamFilter();
            ExamFilter = await ExamService.ToFilter(ExamFilter);
            ExamFilter.Id = new IdFilter { In = Ids };
            ExamFilter.Selects = ExamSelect.Id;
            ExamFilter.Skip = 0;
            ExamFilter.Take = int.MaxValue;

            List<Exam> Exams = await ExamService.List(ExamFilter);
            Exams = await ExamService.BulkDelete(Exams);
            if (Exams.Any(x => !x.IsValidated))
                return BadRequest(Exams.Where(x => !x.IsValidated));
            return true;
        }

        [Route(ExamRoute.SubmitExam), HttpPost]
        public async Task<ActionResult<Exam_ExamHistoryDTO>> SubmitExam([FromBody] Exam_ExamDTO Exam_ExamDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Exam_ExamDTO.Id))
                return Forbid();

            Exam Exam = ConvertDTOToEntity(Exam_ExamDTO);
            var ExamHistory = await DoingExamService.SubmitExam(Exam);
            Exam_ExamHistoryDTO Exam_ExamHistoryDTO = new Exam_ExamHistoryDTO(ExamHistory);
            if (ExamHistory.IsValidated)
                return Exam_ExamHistoryDTO;
            else
                return BadRequest(Exam_ExamDTO);
        }
        [Route(ExamRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter CreatorFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Creators = await AppUserService.List(CreatorFilter);
            ExamLevelFilter ExamLevelFilter = new ExamLevelFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ExamLevelSelect.ALL
            };
            List<ExamLevel> ExamLevels = await ExamLevelService.List(ExamLevelFilter);
            ExamStatusFilter ExamStatusFilter = new ExamStatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ExamStatusSelect.ALL
            };
            List<ExamStatus> ExamStatuses = await ExamStatusService.List(ExamStatusFilter);
            GradeFilter GradeFilter = new GradeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = GradeSelect.ALL
            };
            List<Grade> Grades = await GradeService.List(GradeFilter);
            ImageFilter ImageFilter = new ImageFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ImageSelect.ALL
            };
            List<Image> Images = await ImageService.List(ImageFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            SubjectFilter SubjectFilter = new SubjectFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SubjectSelect.ALL
            };
            List<Subject> Subjects = await SubjectService.List(SubjectFilter);
            List<Exam> Exams = new List<Exam>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Exams);
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int SubjectIdColumn = 3 + StartColumn;
                int ExamLevelIdColumn = 4 + StartColumn;
                int StatusIdColumn = 5 + StartColumn;
                int CreatorIdColumn = 6 + StartColumn;
                int GradeIdColumn = 11 + StartColumn;
                int ExamStatusIdColumn = 12 + StartColumn;
                int TotalMarkColumn = 13 + StartColumn;
                int TotalQuestionColumn = 14 + StartColumn;
                int ImageIdColumn = 15 + StartColumn;
                int TimeColumn = 16 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i, NameColumn].Value?.ToString();
                    string SubjectIdValue = worksheet.Cells[i, SubjectIdColumn].Value?.ToString();
                    string ExamLevelIdValue = worksheet.Cells[i, ExamLevelIdColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i, StatusIdColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i, CreatorIdColumn].Value?.ToString();
                    string GradeIdValue = worksheet.Cells[i, GradeIdColumn].Value?.ToString();
                    string ExamStatusIdValue = worksheet.Cells[i, ExamStatusIdColumn].Value?.ToString();
                    string TotalMarkValue = worksheet.Cells[i, TotalMarkColumn].Value?.ToString();
                    string TotalQuestionValue = worksheet.Cells[i, TotalQuestionColumn].Value?.ToString();
                    string ImageIdValue = worksheet.Cells[i, ImageIdColumn].Value?.ToString();
                    string TimeValue = worksheet.Cells[i, TimeColumn].Value?.ToString();
                    
                    Exam Exam = new Exam();
                    Exam.Code = CodeValue;
                    Exam.Name = NameValue;
                    Exam.TotalMark = decimal.TryParse(TotalMarkValue, out decimal TotalMark) ? TotalMark : 0;
                    Exam.TotalQuestion = long.TryParse(TotalQuestionValue, out long TotalQuestion) ? TotalQuestion : 0;
                    Exam.Time = long.TryParse(TimeValue, out long Time) ? Time : 0;
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    Exam.CreatorId = Creator == null ? 0 : Creator.Id;
                    Exam.Creator = Creator;
                    ExamLevel ExamLevel = ExamLevels.Where(x => x.Id.ToString() == ExamLevelIdValue).FirstOrDefault();
                    Exam.ExamLevelId = ExamLevel == null ? 0 : ExamLevel.Id;
                    Exam.ExamLevel = ExamLevel;
                    ExamStatus ExamStatus = ExamStatuses.Where(x => x.Id.ToString() == ExamStatusIdValue).FirstOrDefault();
                    Exam.ExamStatusId = ExamStatus == null ? 0 : ExamStatus.Id;
                    Exam.ExamStatus = ExamStatus;
                    Grade Grade = Grades.Where(x => x.Id.ToString() == GradeIdValue).FirstOrDefault();
                    Exam.GradeId = Grade == null ? 0 : Grade.Id;
                    Exam.Grade = Grade;
                    Image Image = Images.Where(x => x.Id.ToString() == ImageIdValue).FirstOrDefault();
                    Exam.ImageId = Image == null ? 0 : Image.Id;
                    Exam.Image = Image;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    Exam.StatusId = Status == null ? 0 : Status.Id;
                    Exam.Status = Status;
                    Subject Subject = Subjects.Where(x => x.Id.ToString() == SubjectIdValue).FirstOrDefault();
                    Exam.SubjectId = Subject == null ? 0 : Subject.Id;
                    Exam.Subject = Subject;
                    
                    Exams.Add(Exam);
                }
            }
            Exams = await ExamService.BulkMerge(Exams);
            if (Exams.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Exams.Count; i++)
                {
                    Exam Exam = Exams[i];
                    if (!Exam.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Exam.Errors.ContainsKey(nameof(Exam.Id).Camelize()))
                            Error += Exam.Errors[nameof(Exam.Id).Camelize()];
                        if (Exam.Errors.ContainsKey(nameof(Exam.Code).Camelize()))
                            Error += Exam.Errors[nameof(Exam.Code).Camelize()];
                        if (Exam.Errors.ContainsKey(nameof(Exam.Name).Camelize()))
                            Error += Exam.Errors[nameof(Exam.Name).Camelize()];
                        if (Exam.Errors.ContainsKey(nameof(Exam.SubjectId).Camelize()))
                            Error += Exam.Errors[nameof(Exam.SubjectId).Camelize()];
                        if (Exam.Errors.ContainsKey(nameof(Exam.ExamLevelId).Camelize()))
                            Error += Exam.Errors[nameof(Exam.ExamLevelId).Camelize()];
                        if (Exam.Errors.ContainsKey(nameof(Exam.StatusId).Camelize()))
                            Error += Exam.Errors[nameof(Exam.StatusId).Camelize()];
                        if (Exam.Errors.ContainsKey(nameof(Exam.CreatorId).Camelize()))
                            Error += Exam.Errors[nameof(Exam.CreatorId).Camelize()];
                        if (Exam.Errors.ContainsKey(nameof(Exam.GradeId).Camelize()))
                            Error += Exam.Errors[nameof(Exam.GradeId).Camelize()];
                        if (Exam.Errors.ContainsKey(nameof(Exam.ExamStatusId).Camelize()))
                            Error += Exam.Errors[nameof(Exam.ExamStatusId).Camelize()];
                        if (Exam.Errors.ContainsKey(nameof(Exam.TotalMark).Camelize()))
                            Error += Exam.Errors[nameof(Exam.TotalMark).Camelize()];
                        if (Exam.Errors.ContainsKey(nameof(Exam.TotalQuestion).Camelize()))
                            Error += Exam.Errors[nameof(Exam.TotalQuestion).Camelize()];
                        if (Exam.Errors.ContainsKey(nameof(Exam.ImageId).Camelize()))
                            Error += Exam.Errors[nameof(Exam.ImageId).Camelize()];
                        if (Exam.Errors.ContainsKey(nameof(Exam.Time).Camelize()))
                            Error += Exam.Errors[nameof(Exam.Time).Camelize()];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }    

        [Route(ExamRoute.UploadImage), HttpPost]
        public async Task<ActionResult<Exam_ImageDTO>> UploadImage(IFormFile file)
        {

            Image Image = await ImageService.Create(file);
            if (!Image.IsValidated)
            {
                return BadRequest(new Exam_ImageDTO(Image));
            }
            else
                return new Exam_ImageDTO(Image);

        }

        
        private async Task<bool> HasPermission(long Id)
        {
            ExamFilter ExamFilter = new ExamFilter();
            ExamFilter = await ExamService.ToFilter(ExamFilter);
            if (Id == 0)
            {

            }
            else
            {
                ExamFilter.Id = new IdFilter { Equal = Id };
                int count = await ExamService.Count(ExamFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Exam ConvertDTOToEntity(Exam_ExamDTO Exam_ExamDTO)
        {
            Exam_ExamDTO.TrimString();
            Exam Exam = new Exam();
            Exam.Id = Exam_ExamDTO.Id;
            Exam.Code = Exam_ExamDTO.Code;
            Exam.Name = Exam_ExamDTO.Name;
            Exam.SubjectId = Exam_ExamDTO.SubjectId;
            Exam.ExamLevelId = Exam_ExamDTO.ExamLevelId;
            Exam.StatusId = Exam_ExamDTO.StatusId;
            Exam.CreatorId = Exam_ExamDTO.CreatorId;
            Exam.GradeId = Exam_ExamDTO.GradeId;
            Exam.ExamStatusId = Exam_ExamDTO.ExamStatusId;
            Exam.TotalMark = Exam_ExamDTO.TotalMark;
            Exam.TotalQuestion = Exam_ExamDTO.TotalQuestion;
            Exam.ImageId = Exam_ExamDTO.ImageId;
            Exam.Time = Exam_ExamDTO.Time;
            Exam.Creator = Exam_ExamDTO.Creator == null ? null : new AppUser
            {
                Id = Exam_ExamDTO.Creator.Id,
                Username = Exam_ExamDTO.Creator.Username,
                DisplayName = Exam_ExamDTO.Creator.DisplayName,
                Password = Exam_ExamDTO.Creator.Password,
                RefreshToken = Exam_ExamDTO.Creator.RefreshToken,
                RoleId = Exam_ExamDTO.Creator.RoleId,
                ImageId = Exam_ExamDTO.Creator.ImageId,
            };
            Exam.ExamLevel = Exam_ExamDTO.ExamLevel == null ? null : new ExamLevel
            {
                Id = Exam_ExamDTO.ExamLevel.Id,
                Code = Exam_ExamDTO.ExamLevel.Code,
                Name = Exam_ExamDTO.ExamLevel.Name,
            };
            Exam.ExamStatus = Exam_ExamDTO.ExamStatus == null ? null : new ExamStatus
            {
                Id = Exam_ExamDTO.ExamStatus.Id,
                Code = Exam_ExamDTO.ExamStatus.Code,
                Name = Exam_ExamDTO.ExamStatus.Name,
            };
            Exam.Grade = Exam_ExamDTO.Grade == null ? null : new Grade
            {
                Id = Exam_ExamDTO.Grade.Id,
                Code = Exam_ExamDTO.Grade.Code,
                Name = Exam_ExamDTO.Grade.Name,
            };
            Exam.Image = Exam_ExamDTO.Image == null ? null : new Image
            {
                Id = Exam_ExamDTO.Image.Id,
                Name = Exam_ExamDTO.Image.Name,
                Url = Exam_ExamDTO.Image.Url,
            };
            Exam.Status = Exam_ExamDTO.Status == null ? null : new Status
            {
                Id = Exam_ExamDTO.Status.Id,
                Code = Exam_ExamDTO.Status.Code,
                Name = Exam_ExamDTO.Status.Name,
            };
            Exam.Subject = Exam_ExamDTO.Subject == null ? null : new Subject
            {
                Id = Exam_ExamDTO.Subject.Id,
                Code = Exam_ExamDTO.Subject.Code,
                Name = Exam_ExamDTO.Subject.Name,
            };
            Exam.ExamQuestionMappings = Exam_ExamDTO.ExamQuestionMappings?
                .Select(x => new ExamQuestionMapping
                {
                    QuestionId = x.QuestionId,
                    Mark = x.Mark,
                    Question = x.Question == null ? null : new Question
                    {
                        Id = x.Question.Id,
                        Code = x.Question.Code,
                        Name = x.Question.Name,
                        SubjectId = x.Question.SubjectId,
                        QuestionGroupId = x.Question.QuestionGroupId,
                        QuestionTypeId = x.Question.QuestionTypeId,
                        Content = x.Question.Content,
                        StatusId = x.Question.StatusId,
                        CreatorId = x.Question.CreatorId,
                        GradeId = x.Question.GradeId,
                    },
                }).ToList();
            var QuestionContentDTOs = Exam_ExamDTO.ExamQuestionMappings?.SelectMany(x => x.Question.QuestionContents).ToList();
            if (QuestionContentDTOs != null)
            {
                var QuestionContents = QuestionContentDTOs.Select(x => new QuestionContent
                {
                    Id = x.Id,
                    IsRight = x.IsRight,
                    QuestionId = x.QuestionId,
                    AnswerContent = x.AnswerContent
                }).ToList(); 

                foreach (ExamQuestionMapping ExamQuestionMapping in Exam.ExamQuestionMappings)
                {
                    ExamQuestionMapping.Question.QuestionContents = QuestionContents.Where(x => x.QuestionId == ExamQuestionMapping.QuestionId).ToList();
                }

            }
            Exam.BaseLanguage = CurrentContext.Language;
            return Exam;
        }

        private ExamFilter ConvertFilterDTOToFilterEntity(Exam_ExamFilterDTO Exam_ExamFilterDTO)
        {
            ExamFilter ExamFilter = new ExamFilter();
            ExamFilter.Selects = ExamSelect.ALL;
            ExamFilter.SearchBy = ExamSearch.ALL;
            ExamFilter.Skip = Exam_ExamFilterDTO.Skip;
            ExamFilter.Take = Exam_ExamFilterDTO.Take;
            ExamFilter.OrderBy = Exam_ExamFilterDTO.OrderBy;
            ExamFilter.OrderType = Exam_ExamFilterDTO.OrderType;

            ExamFilter.Id = Exam_ExamFilterDTO.Id;
            ExamFilter.Code = Exam_ExamFilterDTO.Code;
            ExamFilter.Name = Exam_ExamFilterDTO.Name;
            ExamFilter.SubjectId = Exam_ExamFilterDTO.SubjectId;
            ExamFilter.ExamLevelId = Exam_ExamFilterDTO.ExamLevelId;
            ExamFilter.StatusId = Exam_ExamFilterDTO.StatusId;
            ExamFilter.CreatorId = Exam_ExamFilterDTO.CreatorId;
            ExamFilter.GradeId = Exam_ExamFilterDTO.GradeId;
            ExamFilter.ExamStatusId = Exam_ExamFilterDTO.ExamStatusId;
            ExamFilter.TotalMark = Exam_ExamFilterDTO.TotalMark;
            ExamFilter.TotalQuestion = Exam_ExamFilterDTO.TotalQuestion;
            ExamFilter.ImageId = Exam_ExamFilterDTO.ImageId;
            ExamFilter.Time = Exam_ExamFilterDTO.Time;
            ExamFilter.CreatedAt = Exam_ExamFilterDTO.CreatedAt;
            ExamFilter.UpdatedAt = Exam_ExamFilterDTO.UpdatedAt;
            ExamFilter.Search = Exam_ExamFilterDTO.Search;
            return ExamFilter;
        }
    }
}

