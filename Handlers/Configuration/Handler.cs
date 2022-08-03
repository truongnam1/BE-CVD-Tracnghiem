using Tracnghiem.Entities;
using Tracnghiem.Enums;
using Tracnghiem.Helpers;
using RabbitMQ.Client;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Tracnghiem.Handlers.Configuration
{
    public interface IHandler
    {
        string Name { get; }
        IServiceProvider ServiceProvider { get; set; }
        string SourceModuleName { get; set; }
        void QueueBind(IModel channel, string queue, string exchange);
        Task Handle(string routingKey, string content);
    }

    public abstract class Handler : IHandler
    {
        public abstract string Name { get; }
        public IServiceProvider ServiceProvider { get; set; }
        public string SourceModuleName { get; set; }

        public abstract Task Handle(string routingKey, string content);

        public abstract void QueueBind(IModel channel, string queue, string exchange);

        protected void Log(Exception ex, string className, [CallerMemberName] string methodName = "")
        {
            IRabbitManager RabbitManager = ServiceProvider.GetService<IRabbitManager>();
            SystemLog SystemLog = new SystemLog
            {
                AppUserId = null,
                AppUser = "RABBITMQ",
                ClassName = className,
                MethodName = methodName,
                SourceModuleName = SourceModuleName,
                ModuleName = StaticParams.ModuleName,
                Exception = ex.ToString(),
                Time = StaticParams.DateTimeNow,
            };
            RabbitManager.PublishSingle(SystemLog, RoutingKeyEnum.SystemLogSend.Code);
        }
    }
}
