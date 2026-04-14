using Microsoft.AspNetCore.Mvc;
using secondhand_car_backend.Services;

namespace secondhand_car_backend.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly IServiceProvider _serviceCollection;

        public BaseController(IServiceProvider serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        
        // Usamos ILoggerFactory para crear un logger basado en el nombre de la clase que esté ejecutándose
        internal ILogger Logger => _serviceCollection.GetRequiredService<ILoggerFactory>()
                                    .CreateLogger(GetType().Name);

        internal IConfiguration Configuration => _serviceCollection.GetRequiredService<IConfiguration>();

        internal IUsersService ServiceUsers => _serviceCollection.GetRequiredService<IUsersService>();
    }
}