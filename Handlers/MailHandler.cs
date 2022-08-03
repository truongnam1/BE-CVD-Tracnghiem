using Tracnghiem.Entities;
using Tracnghiem.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Tracnghiem.Handlers.Configuration;
using TrueSight.PER.Entities;
using Tracnghiem.Enums;
using Role = TrueSight.PER.Entities.Role;
using Tracnghiem.Services.MMail;

namespace Tracnghiem.Handlers
{
    public class RoleHandler : Handler
    {
        private string SendKey => Name + ".Send";
        private string SyncKey => Name + ".Sync";

        public override string Name => nameof(Mail);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(string routingKey, string content)
        {
            IUOW UOW = ServiceProvider.GetService<IUOW>();
            IMailService MailService = ServiceProvider.GetService<IMailService>();
            //if (routingKey == SyncKey)
            //    await Sync(UOW, content);
            if (routingKey == SendKey)
            {
                await Send(MailService, content);
            }
        }
        private async Task Used(IUOW UOW, string json)
        {
            try
            {
                List<Role> Role = JsonConvert.DeserializeObject<List<Role>>(json);
                List<long> Ids = Role.Select(a => a.Id).ToList();
                await UOW.RoleRepository.Used(Ids);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(RoleHandler));
            }
        }
        
        private async Task Send(IMailService MailService, string json)
        {
            try
            {
                List<Mail> Mails = JsonConvert.DeserializeObject<List<Mail>>(json);
                await MailService.SendEmails(Mails);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(RoleHandler));
            }
        }
    }
}
