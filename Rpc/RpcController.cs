using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Tracnghiem.Repositories;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc
{
    public class Root
    {
        protected const string Module = "tracnghiem";
        protected const string Rpc = "rpc/";
        protected const string Rest = "rest/";
    }
    [Authorize]
    [Authorize(Policy ="Permission")]
    public class RpcController : ControllerBase
    {
    }

    [Authorize]
    [Authorize(Policy = "Simple")]
    public class SimpleController : ControllerBase
    {
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement()
        {
        }
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private ICurrentContext CurrentContext;
        //private DataContext DataContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        private IUOW UOW;
        public PermissionHandler(
            ICurrentContext CurrentContext, 
            //DataContext DataContext,
            IUOW UOW,
            IHttpContextAccessor httpContextAccessor
            )
        {
            this.CurrentContext = CurrentContext;
            //this.DataContext = DataContext;
            this.UOW = UOW;
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var types = context.User.Claims.Select(c => c.Type).ToList();
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                context.Fail();
                return;
            }
            long UserId = long.TryParse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, out long u) ? u : 0;
            string UserName = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;
            var HttpContext = httpContextAccessor.HttpContext;
            string url = HttpContext.Request.Path.Value.ToLower().Substring(1);
            string TimeZone = HttpContext.Request.Headers["X-TimeZone"];
            string Language = HttpContext.Request.Headers["X-Language"];
            CurrentContext.Token = HttpContext.Request.Cookies["Token"];
            CurrentContext.UserId = UserId;
            CurrentContext.TimeZone = int.TryParse(TimeZone, out int t) ? t : 0;
            CurrentContext.Language = Language ?? "vi";
            context.Succeed(requirement);
            List<CurrentPermission> CurrentPermissions = await UOW.PermissionRepository.ListByUserAndPath(UserId, url);

            if (CurrentPermissions.Count == 0)
            {
                context.Fail();
                return;
            }
            CurrentContext.RoleIds = CurrentPermissions.Select(p => p.RoleId).Distinct().ToList();
            CurrentContext.Filters = new Dictionary<long, List<FilterPermissionDefinition>>();
            foreach (CurrentPermission CurrentPermission in CurrentPermissions)
            {
                List<FilterPermissionDefinition> FilterPermissionDefinitions = new List<FilterPermissionDefinition>();
                CurrentContext.Filters.Add(CurrentPermission.Id, FilterPermissionDefinitions);
                foreach (CurrentPermissionContent CurrentPermissionContent in CurrentPermission.CurrentPermissionContents)
                {
                    FilterPermissionDefinition FilterPermissionDefinition = FilterPermissionDefinitions.Where(f => f.Name == CurrentPermissionContent.FieldName).FirstOrDefault();
                    if (FilterPermissionDefinition == null)
                    {
                        FilterPermissionDefinition = new FilterPermissionDefinition(CurrentPermissionContent.FieldName);
                        FilterPermissionDefinitions.Add(FilterPermissionDefinition);
                    }
                    FilterPermissionDefinition.SetValue(CurrentPermissionContent.FieldTypeId, CurrentPermissionContent.PermissionOperatorId, CurrentPermissionContent.Value);
                }
            }
            context.Succeed(requirement);
        }
    }

    public class SimpleRequirement : IAuthorizationRequirement
    {
        public SimpleRequirement()
        {
        }
    }
    public class SimpleHandler : AuthorizationHandler<SimpleRequirement>
    {
        private ICurrentContext CurrentContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        public SimpleHandler(ICurrentContext CurrentContext, IHttpContextAccessor httpContextAccessor)
        {
            this.CurrentContext = CurrentContext;
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SimpleRequirement requirement)
        {
            var types = context.User.Claims.Select(c => c.Type).ToList();
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                context.Fail();
                return;
            }
            long UserId = long.TryParse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, out long u) ? u : 0;
            Guid UserRowId = Guid.TryParse(context.User.FindFirst(c => c.Type == ClaimTypes.PrimarySid).Value, out Guid rowId) ? rowId : Guid.Empty;
            string UserName = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;
            var HttpContext = httpContextAccessor.HttpContext;
            string url = HttpContext.Request.Path.Value.ToLower().Substring(1);
            string TimeZone = HttpContext.Request.Headers["X-TimeZone"];
            string Language = HttpContext.Request.Headers["X-Language"];
            string Latitude = HttpContext.Request.Headers["X-Latitude"];
            string Longitude = HttpContext.Request.Headers["X-Longitude"];
            CurrentContext.Token = HttpContext.Request.Cookies["Token"];
            CurrentContext.UserId = UserId;
            //CurrentContext.UserRowId = UserRowId;
            CurrentContext.TimeZone = int.TryParse(TimeZone, out int t) ? t : 0;
            CurrentContext.Language = Language ?? "vi";

            //CurrentContext.RoleIds = await UOW.RoleRepository.ListIds(UserId);
            context.Succeed(requirement);
        }
    }
}