using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Moq;
using Ninject;
using MovieStore.Domain.Abstract;
using MovieStore.Domain.Entities;
using MovieStore.Domain.Concrete;
using System.Configuration;

namespace MovieStore.WebUI.Infrastructure {

    public class NinjectDependencyResolver : IDependencyResolver {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam) {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType) {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings() {
            kernel.Bind<IMovieRepository>().To<EFMovieRepository>();
            kernel.Bind<IOrderRepository>().To<EFOrderRepository>();

            EmailSettings emailSettings = new EmailSettings { 
                WriteAsFile = bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false") 
            };

            kernel.Bind<IOrderProcessor>().To<EmailOrderProcessor>().WithConstructorArgument("settings", emailSettings);
        }
    }
}