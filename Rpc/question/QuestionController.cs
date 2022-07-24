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
using Tracnghiem.Services.MQuestion;
using Tracnghiem.Services.MGrade;
using Tracnghiem.Services.MQuestionGroup;
using Tracnghiem.Services.MQuestionType;
using Tracnghiem.Services.MStatus;
using Tracnghiem.Services.MSubject;
using Tracnghiem.Services.MQuestionContent;

namespace Tracnghiem.Rpc.question
{
    public partial class QuestionController : RpcController
    {
        private IGradeService GradeService;
        private IQuestionGroupService QuestionGroupService;
        private IQuestionTypeService QuestionTypeService;
        private IStatusService StatusService;
        private ISubjectService SubjectService;
        private IQuestionContentService QuestionContentService;
        private IQuestionService QuestionService;
        private ICurrentContext CurrentContext;
        public QuestionController(
            IGradeService GradeService,
            IQuestionGroupService QuestionGroupService,
            IQuestionTypeService QuestionTypeService,
            IStatusService StatusService,
            ISubjectService SubjectService,
            IQuestionContentService QuestionContentService,
            IQuestionService QuestionService,
            ICurrentContext CurrentContext
        )
        {
            this.GradeService = GradeService;
            this.QuestionGroupService = QuestionGroupService;
            this.QuestionTypeService = QuestionTypeService;
            this.StatusService = StatusService;
            this.SubjectService = SubjectService;
            this.QuestionContentService = QuestionContentService;
            this.QuestionService = QuestionService;
            this.CurrentContext = CurrentContext;
        }

        [Route(QuestionRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Question_QuestionFilterDTO Question_QuestionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionFilter QuestionFilter = ConvertFilterDTOToFilterEntity(Question_QuestionFilterDTO);
            QuestionFilter = await QuestionService.ToFilter(QuestionFilter);
            int count = await QuestionService.Count(QuestionFilter);
            return count;
        }

        [Route(QuestionRoute.List), HttpPost]
        public async Task<ActionResult<List<Question_QuestionDTO>>> List([FromBody] Question_QuestionFilterDTO Question_QuestionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionFilter QuestionFilter = ConvertFilterDTOToFilterEntity(Question_QuestionFilterDTO);
            QuestionFilter = await QuestionService.ToFilter(QuestionFilter);
            List<Question> Questions = await QuestionService.List(QuestionFilter);
            List<Question_QuestionDTO> Question_QuestionDTOs = Questions
                .Select(c => new Question_QuestionDTO(c)).ToList();
            return Question_QuestionDTOs;
        }

        [Route(QuestionRoute.Get), HttpPost]
        public async Task<ActionResult<Question_QuestionDTO>> Get([FromBody]Question_QuestionDTO Question_QuestionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Question_QuestionDTO.Id))
                return Forbid();

            Question Question = await QuestionService.Get(Question_QuestionDTO.Id);
            return new Question_QuestionDTO(Question);
        }

        [Route(QuestionRoute.Create), HttpPost]
        public async Task<ActionResult<Question_QuestionDTO>> Create([FromBody] Question_QuestionDTO Question_QuestionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Question_QuestionDTO.Id))
                return Forbid();

            Question Question = ConvertDTOToEntity(Question_QuestionDTO);
            Question = await QuestionService.Create(Question);
            Question_QuestionDTO = new Question_QuestionDTO(Question);
            if (Question.IsValidated)
                return Question_QuestionDTO;
            else
                return BadRequest(Question_QuestionDTO);
        }

        [Route(QuestionRoute.Update), HttpPost]
        public async Task<ActionResult<Question_QuestionDTO>> Update([FromBody] Question_QuestionDTO Question_QuestionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Question_QuestionDTO.Id))
                return Forbid();

            Question Question = ConvertDTOToEntity(Question_QuestionDTO);
            Question = await QuestionService.Update(Question);
            Question_QuestionDTO = new Question_QuestionDTO(Question);
            if (Question.IsValidated)
                return Question_QuestionDTO;
            else
                return BadRequest(Question_QuestionDTO);
        }

        [Route(QuestionRoute.Delete), HttpPost]
        public async Task<ActionResult<Question_QuestionDTO>> Delete([FromBody] Question_QuestionDTO Question_QuestionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Question_QuestionDTO.Id))
                return Forbid();

            Question Question = ConvertDTOToEntity(Question_QuestionDTO);
            Question = await QuestionService.Delete(Question);
            Question_QuestionDTO = new Question_QuestionDTO(Question);
            if (Question.IsValidated)
                return Question_QuestionDTO;
            else
                return BadRequest(Question_QuestionDTO);
        }
        
        [Route(QuestionRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionFilter QuestionFilter = new QuestionFilter();
            QuestionFilter = await QuestionService.ToFilter(QuestionFilter);
            QuestionFilter.Id = new IdFilter { In = Ids };
            QuestionFilter.Selects = QuestionSelect.Id;
            QuestionFilter.Skip = 0;
            QuestionFilter.Take = int.MaxValue;

            List<Question> Questions = await QuestionService.List(QuestionFilter);
            Questions = await QuestionService.BulkDelete(Questions);
            if (Questions.Any(x => !x.IsValidated))
                return BadRequest(Questions.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(QuestionRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            GradeFilter GradeFilter = new GradeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = GradeSelect.ALL
            };
            List<Grade> Grades = await GradeService.List(GradeFilter);
            QuestionGroupFilter QuestionGroupFilter = new QuestionGroupFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = QuestionGroupSelect.ALL
            };
            List<QuestionGroup> QuestionGroups = await QuestionGroupService.List(QuestionGroupFilter);
            QuestionTypeFilter QuestionTypeFilter = new QuestionTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = QuestionTypeSelect.ALL
            };
            List<QuestionType> QuestionTypes = await QuestionTypeService.List(QuestionTypeFilter);
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
            List<Question> Questions = new List<Question>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Questions);
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int SubjectIdColumn = 3 + StartColumn;
                int QuestionGroupIdColumn = 4 + StartColumn;
                int QuestionTypeIdColumn = 5 + StartColumn;
                int ContentColumn = 6 + StartColumn;
                int StatusIdColumn = 7 + StartColumn;
                int CreatorIdColumn = 8 + StartColumn;
                int GradeIdColumn = 13 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i, NameColumn].Value?.ToString();
                    string SubjectIdValue = worksheet.Cells[i, SubjectIdColumn].Value?.ToString();
                    string QuestionGroupIdValue = worksheet.Cells[i, QuestionGroupIdColumn].Value?.ToString();
                    string QuestionTypeIdValue = worksheet.Cells[i, QuestionTypeIdColumn].Value?.ToString();
                    string ContentValue = worksheet.Cells[i, ContentColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i, StatusIdColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i, CreatorIdColumn].Value?.ToString();
                    string GradeIdValue = worksheet.Cells[i, GradeIdColumn].Value?.ToString();
                    
                    Question Question = new Question();
                    Question.Code = CodeValue;
                    Question.Name = NameValue;
                    Question.Content = ContentValue;
                    Grade Grade = Grades.Where(x => x.Id.ToString() == GradeIdValue).FirstOrDefault();
                    Question.GradeId = Grade == null ? 0 : Grade.Id;
                    Question.Grade = Grade;
                    QuestionGroup QuestionGroup = QuestionGroups.Where(x => x.Id.ToString() == QuestionGroupIdValue).FirstOrDefault();
                    Question.QuestionGroupId = QuestionGroup == null ? 0 : QuestionGroup.Id;
                    Question.QuestionGroup = QuestionGroup;
                    QuestionType QuestionType = QuestionTypes.Where(x => x.Id.ToString() == QuestionTypeIdValue).FirstOrDefault();
                    Question.QuestionTypeId = QuestionType == null ? 0 : QuestionType.Id;
                    Question.QuestionType = QuestionType;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    Question.StatusId = Status == null ? 0 : Status.Id;
                    Question.Status = Status;
                    Subject Subject = Subjects.Where(x => x.Id.ToString() == SubjectIdValue).FirstOrDefault();
                    Question.SubjectId = Subject == null ? 0 : Subject.Id;
                    Question.Subject = Subject;
                    
                    Questions.Add(Question);
                }
            }
            Questions = await QuestionService.BulkMerge(Questions);
            if (Questions.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Questions.Count; i++)
                {
                    Question Question = Questions[i];
                    if (!Question.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Question.Errors.ContainsKey(nameof(Question.Id).Camelize()))
                            Error += Question.Errors[nameof(Question.Id).Camelize()];
                        if (Question.Errors.ContainsKey(nameof(Question.Code).Camelize()))
                            Error += Question.Errors[nameof(Question.Code).Camelize()];
                        if (Question.Errors.ContainsKey(nameof(Question.Name).Camelize()))
                            Error += Question.Errors[nameof(Question.Name).Camelize()];
                        if (Question.Errors.ContainsKey(nameof(Question.SubjectId).Camelize()))
                            Error += Question.Errors[nameof(Question.SubjectId).Camelize()];
                        if (Question.Errors.ContainsKey(nameof(Question.QuestionGroupId).Camelize()))
                            Error += Question.Errors[nameof(Question.QuestionGroupId).Camelize()];
                        if (Question.Errors.ContainsKey(nameof(Question.QuestionTypeId).Camelize()))
                            Error += Question.Errors[nameof(Question.QuestionTypeId).Camelize()];
                        if (Question.Errors.ContainsKey(nameof(Question.Content).Camelize()))
                            Error += Question.Errors[nameof(Question.Content).Camelize()];
                        if (Question.Errors.ContainsKey(nameof(Question.StatusId).Camelize()))
                            Error += Question.Errors[nameof(Question.StatusId).Camelize()];
                        if (Question.Errors.ContainsKey(nameof(Question.CreatorId).Camelize()))
                            Error += Question.Errors[nameof(Question.CreatorId).Camelize()];
                        if (Question.Errors.ContainsKey(nameof(Question.GradeId).Camelize()))
                            Error += Question.Errors[nameof(Question.GradeId).Camelize()];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(QuestionRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Question_QuestionFilterDTO Question_QuestionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            var QuestionFilter = ConvertFilterDTOToFilterEntity(Question_QuestionFilterDTO);
            QuestionFilter.Skip = 0;
            QuestionFilter.Take = int.MaxValue;
            QuestionFilter = await QuestionService.ToFilter(QuestionFilter);
            List<Question> Questions = await QuestionService.List(QuestionFilter);
            List<Question_QuestionExportDTO> Question_QuestionExportDTOs = Questions.Select(x => new Question_QuestionExportDTO(x)).ToList();  
            var STT = 1;
            foreach (var Question_QuestionExportDTO in Question_QuestionExportDTOs)
            {
                Question_QuestionExportDTO.STT = STT++;
            }
            string path = "Templates/Question_Export.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();            
            Data.Data = Question_QuestionExportDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {    
                document.Process(Data);
            }
            return File(output.ToArray(), "application/octet-stream", "Question.xlsx");
        }

        [Route(QuestionRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Question_QuestionFilterDTO Question_QuestionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Question_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Question.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            QuestionFilter QuestionFilter = new QuestionFilter();
            QuestionFilter = await QuestionService.ToFilter(QuestionFilter);
            if (Id == 0)
            {

            }
            else
            {
                QuestionFilter.Id = new IdFilter { Equal = Id };
                int count = await QuestionService.Count(QuestionFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Question ConvertDTOToEntity(Question_QuestionDTO Question_QuestionDTO)
        {
            Question_QuestionDTO.TrimString();
            Question Question = new Question();
            Question.Id = Question_QuestionDTO.Id;
            Question.Code = Question_QuestionDTO.Code;
            Question.Name = Question_QuestionDTO.Name;
            Question.SubjectId = Question_QuestionDTO.SubjectId;
            Question.QuestionGroupId = Question_QuestionDTO.QuestionGroupId;
            Question.QuestionTypeId = Question_QuestionDTO.QuestionTypeId;
            Question.Content = Question_QuestionDTO.Content;
            Question.StatusId = Question_QuestionDTO.StatusId;
            Question.CreatorId = Question_QuestionDTO.CreatorId;
            Question.GradeId = Question_QuestionDTO.GradeId;
            Question.Grade = Question_QuestionDTO.Grade == null ? null : new Grade
            {
                Id = Question_QuestionDTO.Grade.Id,
                Code = Question_QuestionDTO.Grade.Code,
                Name = Question_QuestionDTO.Grade.Name,
            };
            Question.QuestionGroup = Question_QuestionDTO.QuestionGroup == null ? null : new QuestionGroup
            {
                Id = Question_QuestionDTO.QuestionGroup.Id,
                Code = Question_QuestionDTO.QuestionGroup.Code,
                Name = Question_QuestionDTO.QuestionGroup.Name,
            };
            Question.QuestionType = Question_QuestionDTO.QuestionType == null ? null : new QuestionType
            {
                Id = Question_QuestionDTO.QuestionType.Id,
                Code = Question_QuestionDTO.QuestionType.Code,
                Name = Question_QuestionDTO.QuestionType.Name,
            };
            Question.Status = Question_QuestionDTO.Status == null ? null : new Status
            {
                Id = Question_QuestionDTO.Status.Id,
                Code = Question_QuestionDTO.Status.Code,
                Name = Question_QuestionDTO.Status.Name,
            };
            Question.Subject = Question_QuestionDTO.Subject == null ? null : new Subject
            {
                Id = Question_QuestionDTO.Subject.Id,
                Code = Question_QuestionDTO.Subject.Code,
                Name = Question_QuestionDTO.Subject.Name,
            };
            Question.QuestionContents = Question_QuestionDTO.QuestionContents?
                .Select(x => new QuestionContent
                {
                    Id = x.Id,
                    AnswerContent = x.AnswerContent,
                    IsRight = x.IsRight,
                }).ToList();
            Question.BaseLanguage = CurrentContext.Language;
            return Question;
        }

        private QuestionFilter ConvertFilterDTOToFilterEntity(Question_QuestionFilterDTO Question_QuestionFilterDTO)
        {
            QuestionFilter QuestionFilter = new QuestionFilter();
            QuestionFilter.Selects = QuestionSelect.ALL;
            QuestionFilter.SearchBy = QuestionSearch.ALL;
            QuestionFilter.Skip = Question_QuestionFilterDTO.Skip;
            QuestionFilter.Take = Question_QuestionFilterDTO.Take;
            QuestionFilter.OrderBy = Question_QuestionFilterDTO.OrderBy;
            QuestionFilter.OrderType = Question_QuestionFilterDTO.OrderType;

            QuestionFilter.Id = Question_QuestionFilterDTO.Id;
            QuestionFilter.Code = Question_QuestionFilterDTO.Code;
            QuestionFilter.Name = Question_QuestionFilterDTO.Name;
            QuestionFilter.SubjectId = Question_QuestionFilterDTO.SubjectId;
            QuestionFilter.QuestionGroupId = Question_QuestionFilterDTO.QuestionGroupId;
            QuestionFilter.QuestionTypeId = Question_QuestionFilterDTO.QuestionTypeId;
            QuestionFilter.Content = Question_QuestionFilterDTO.Content;
            QuestionFilter.StatusId = Question_QuestionFilterDTO.StatusId;
            QuestionFilter.CreatorId = Question_QuestionFilterDTO.CreatorId;
            QuestionFilter.GradeId = Question_QuestionFilterDTO.GradeId;
            QuestionFilter.CreatedAt = Question_QuestionFilterDTO.CreatedAt;
            QuestionFilter.UpdatedAt = Question_QuestionFilterDTO.UpdatedAt;
            QuestionFilter.Search = Question_QuestionFilterDTO.Search;
            return QuestionFilter;
        }
    }
}

