﻿using Tracnghiem.Common;
using Tracnghiem.Entities;
using Tracnghiem.Enums;
using Tracnghiem.Helpers;
using Tracnghiem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueSight.PER;
using TrueSight.PER.Entities;
using Action = TrueSight.PER.Entities.Action;
using Role = TrueSight.PER.Entities.Role;
using Microsoft.Extensions.Configuration;
using Tracnghiem.Repositories;
using System.Security.Cryptography;
using Tracnghiem.Services.MSite;
using Tracnghiem.Services.MRole;

namespace Tracnghiem.Rpc
{
    public partial class SetupController : ControllerBase
    {
        private ConnectionMultiplexer RedisConnection;
        private DataContext DataContext;
        private IConfiguration Configuration;
        private IRedisStore RedisStore;
        private ISiteService SiteService;
        private IRoleService RoleService;

        private string AdminPassword;

        private IUOW UOW;
        public SetupController(
            DataContext DataContext,
            IConfiguration Configuration,
            IRedisStore RedisStore,
            ISiteService SiteService,
            IRoleService RoleService,

            IUOW UOW
            )
        {
            this.DataContext = DataContext;
            AdminPassword = Configuration["Config:AdminPassword"];
            this.UOW = UOW;
            this.RedisStore = RedisStore;
            this.SiteService = SiteService;
            this.RoleService = RoleService;
        }
        [HttpGet, Route("rpc/tracnghiem/setup/init")]
        public async Task<ActionResult> Init()
        {
            await InitEnums();
            await SendMenu();
            InitAdmin();
            return Ok();
        }

        private ActionResult InitAdmin()
        {
            AppUserDAO AppUser = DataContext.AppUser
                .Where(au => au.Username.ToLower() == "Administrator".ToLower())
                .FirstOrDefault();
            if (AppUser == null)
            {    
                AppUser = new AppUserDAO()
                {
                    Username = "Administrator",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null,
                    DisplayName = "Administrator",
                    Email = "namtao100@gmail.com",
                    Password = HashPassword(AdminPassword),
                    StatusId = 1,
                };
                DataContext.AppUser.Add(AppUser);
                DataContext.SaveChanges();
            }

            return Ok();
        }
       

        [HttpGet, Route("rpc/tracnghiem/setup/init-enum")]
        public async Task<ActionResult> InitEnums()
        {
            InitStatusEnum();
            InitExamLevelEnum();
            InitExamStatusEnum();
            InitGradeEnum();
            InitQuestionGroupEnum();
            InitQuestionTypeEnum();
            InitSubjectEnum();
            InitSiteEnum();
            InitPermissionEnum();
            DataContext.SaveChanges();
            return Ok();
        }
        private void InitStatusEnum()
        {
            List<StatusDAO> StatusEnumList = StatusEnum.StatusEnumList.Select(item => new StatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.Status.BulkSynchronize(StatusEnumList);
        }
        private void InitExamLevelEnum()
        {
            List<ExamLevelDAO> ExamLevelList = ExamLevelEnum.ExamLevelEnumList.Select(item => new ExamLevelDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.ExamLevel.BulkSynchronize(ExamLevelList);
        }
        private void InitExamStatusEnum()
        {
            List<ExamStatusDAO> ExamStatusList = ExamStatusEnum.ExamStatusEnumList.Select(item => new ExamStatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.ExamStatus.BulkSynchronize(ExamStatusList);
        }
        private void InitGradeEnum()
        {
            List<GradeDAO> GradeList = GradeEnum.GradeEnumList.Select(item => new GradeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.Grade.BulkSynchronize(GradeList);
        }
        private void InitQuestionGroupEnum()
        {
            List<QuestionGroupDAO> QuestionGroupList = QuestionGroupEnum.QuestionGroupEnumList.Select(item => new QuestionGroupDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.QuestionGroup.BulkSynchronize(QuestionGroupList);
        }
        private void InitQuestionTypeEnum()
        {
            List<QuestionTypeDAO> QuestionTypeList = QuestionTypeEnum.QuestionTypeEnumList.Select(item => new QuestionTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.QuestionType.BulkSynchronize(QuestionTypeList);
        }
        private void InitSubjectEnum()
        {
            List<SubjectDAO> SubjectList = SubjectEnum.SubjectEnumList.Select(item => new SubjectDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                Avatar = item.Avatar,
            }).ToList();
            DataContext.Subject.BulkSynchronize(SubjectList);
        }
        private void InitSiteEnum()
        {
            List<SiteDAO> SiteList = SiteEnum.SiteEnumList.Select(item => new SiteDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                IsDisplay = false
            }).ToList();
            DataContext.Site.BulkSynchronize(SiteList);
        }
        private void InitPermissionEnum()
        {
            List<FieldTypeDAO> FieldTypeDAOs = FieldTypeEnum.List.Select(item => new FieldTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.FieldType.BulkSynchronize(FieldTypeDAOs);
            List<PermissionOperatorDAO> PermissionOperatorDAOs = new List<PermissionOperatorDAO>();
            List<PermissionOperatorDAO> ID = PermissionOperatorEnum.PermissionOperatorEnumForID.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.ID.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(ID);
            List<PermissionOperatorDAO> STRING = PermissionOperatorEnum.PermissionOperatorEnumForSTRING.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.STRING.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(STRING);

            List<PermissionOperatorDAO> LONG = PermissionOperatorEnum.PermissionOperatorEnumForLONG.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.LONG.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(LONG);

            List<PermissionOperatorDAO> DECIMAL = PermissionOperatorEnum.PermissionOperatorEnumForDECIMAL.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.DECIMAL.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(DECIMAL);

            List<PermissionOperatorDAO> DATE = PermissionOperatorEnum.PermissionOperatorEnumForDATE.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.DATE.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(DATE);

            DataContext.PermissionOperator.BulkSynchronize(PermissionOperatorDAOs);
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


        [HttpGet(), Route("rpc/tracnghiem/setup/send-permission")]
        public async Task<ActionResult> SendRole()
        {
            Site Site = await BuildRole();
            await RoleService.BulkMerge(Site);

            return Ok();
        }

        private async Task<Site> BuildRole()
        {
            Site Site = new Site
            {
                Code = "/tracnghiem/",
                Name = "Tracnghiem",
                IsDisplay = true
            };

            var RoleDAOs = DataContext.Role.AsNoTracking();
            var PermissionDAOs = DataContext.Permission.AsNoTracking();
            var AppUserRoleMappingDAOs = DataContext.AppUserRoleMapping.AsNoTracking();
            var PermissionActionMappingDAOs = DataContext.PermissionActionMapping.AsNoTracking();
            var PermissionContentDAOs = DataContext.PermissionContent.AsNoTracking();

            List<Role> Roles = new List<Role>();

            //Build Role
            foreach (var RoleDAO in RoleDAOs)
            {
                Role Role = new Role()
                {
                    Id = RoleDAO.Id,
                    Code = RoleDAO.Code,
                    Name = RoleDAO.Name,
                    StatusId = RoleDAO.StatusId,
                    Used = RoleDAO.Used
                };

                //Build AppUserRoleMapping
                List<long> AppUserIds = AppUserRoleMappingDAOs
                    .Where(x => x.RoleId == RoleDAO.Id)
                    .Select(x => x.AppUserId).Distinct().ToList();
                List<AppUserRoleMapping> AppUserRoleMappings = AppUserIds
                    .Select(ar => new AppUserRoleMapping
                    {
                        RoleId = RoleDAO.Id,
                        AppUserId = ar,
                    }).ToList();


                //Build Permission
                List<Permission> Permissions = PermissionDAOs.Where(x => x.RoleId == RoleDAO.Id)
                    .Select(p => new Permission()
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                        StatusId = p.StatusId,

                        //Get MenuId
                        MenuId = p.MenuId,
                        Menu = new Menu()
                        {
                            Id = p.Menu.Id,
                            Code = p.Menu.Code,
                            Name = p.Menu.Name,
                        },

                        //Build PermissionActionMapping
                        PermissionActionMappings = PermissionActionMappingDAOs.Where(pa => pa.PermissionId == p.Id)
                        .Select(pam => new PermissionActionMapping()
                        {
                            PermissionId = p.Id,
                            Action = new Action()
                            {
                                Name = pam.Action.Name
                            }
                        }).ToList(),

                        //Build PermissionContent
                        PermissionContents = PermissionContentDAOs.Where(pc => pc.PermissionId == p.Id)
                        .Select(pec => new PermissionContent()
                        {
                            Id = pec.Id,
                            PermissionOperatorId = pec.PermissionOperatorId,
                            Value = pec.Value,
                            Field = new Field()
                            {
                                Name = pec.Field.Name
                            },
                        }).ToList(),
                    }).ToList();
                Role.Permissions = Permissions;
                Roles.Add(Role);
            }
            Site.Roles = Roles;
            return Site;
        }

        public async Task<ActionResult> SendMenu()
        {
            Site Site = await BuildMenu();
            await SiteService.BulkMerge(Site);
            return Ok();
        }

        private async Task<Site> BuildMenu()
        {
            Site Site = new Site
            {
                Code = "/tracnghiem/",
                Name = "Tracnghiem",
                IsDisplay = true
            };
            List<Type> routeTypes = typeof(SetupController).Assembly.GetTypes()
                .Where(x => typeof(Root).IsAssignableFrom(x) && x.IsClass && x.Name != "Root")
                .ToList();

            List<Menu> Menus = PermissionBuilder.GenerateMenu(routeTypes, SiteEnum.Tracnghiem.Id);
            Site.Menus = Menus;
            return Site;
        }

        [Route("rpc/tracnghiem/setup/master-entity-register"), HttpGet]
        public async Task<ActionResult> MasterEntityRegister()
        {
            List<MasterEntity> MasterEntities = GenerateMasterEntity();

            //RabbitManager.PublishList(MasterEntities, RoutingKeyEnum.MasterEntityRegister.Code);
            return Ok();
        }

        private static List<MasterEntity> GenerateMasterEntity()
        {
            List<MasterEntity> MasterEntities = new List<MasterEntity>();
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(AppUser),
                Name = $"{nameof(AppUser)}",
                IsTree = 0,
            });
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(Exam),
                Name = $"{nameof(Exam)}",
                IsTree = 0,
            });
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(ExamHistory),
                Name = $"{nameof(ExamHistory)}",
                IsTree = 0,
            });
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(ExamLevel),
                Name = $"{nameof(ExamLevel)}",
                IsTree = 0,
            });
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(ExamStatus),
                Name = $"{nameof(ExamStatus)}",
                IsTree = 0,
            });
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(Grade),
                Name = $"{nameof(Grade)}",
                IsTree = 0,
            });
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(Image),
                Name = $"{nameof(Image)}",
                IsTree = 0,
            });
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(QuestionContent),
                Name = $"{nameof(QuestionContent)}",
                IsTree = 0,
            });
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(Question),
                Name = $"{nameof(Question)}",
                IsTree = 0,
            });
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(QuestionGroup),
                Name = $"{nameof(QuestionGroup)}",
                IsTree = 0,
            });
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(QuestionType),
                Name = $"{nameof(QuestionType)}",
                IsTree = 0,
            });
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(Role),
                Name = $"{nameof(Role)}",
                IsTree = 0,
            });
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(Status),
                Name = $"{nameof(Status)}",
                IsTree = 0,
            });
            MasterEntities.Add(new MasterEntity
            {
                Code = nameof(Subject),
                Name = $"{nameof(Subject)}",
                IsTree = 0,
            });

            MasterEntities.ForEach(x => x.CreatedAt = DateTime.Now);
            MasterEntities.ForEach(x => x.UpdatedAt = DateTime.Now);
            MasterEntities.ForEach(x => x.StatusId = StatusEnum.ACTIVE.Id);
            return MasterEntities;
        }
    }
}
