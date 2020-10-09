using Api.Engine;
using Autofac;
using Autofac.Core;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service;

namespace Api
{
    public class AutofacModule : Module
    {
        private IConfiguration _configuration;
        public AutofacModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).WithParameter(
         new ResolvedParameter(
           (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "connectionString",
           (pi, ctx) => _configuration["ConnectionString"]))
           .WithParameter(
         new ResolvedParameter(
           (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "dataBaseName",
           (pi, ctx) => _configuration["DatabaseName"]));

            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<MessageService>().As<IMessageService>();
            builder.RegisterType<WorkContext>().As<IWorkContext>();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>();
        }
    }
}
