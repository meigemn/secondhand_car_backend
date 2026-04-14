using Microsoft.AspNetCore.Mvc;
using secondhand_car_backend.Services; // Cambiado al nombre de tu proyecto actual

namespace secondhand_car_backend.Controllers
{
    public class BaseController : ControllerBase
    {
        #region Miembros privados

        public readonly IServiceProvider _serviceCollection;

        #endregion

        #region Miembros internos

        // Añadimos el cast correcto y el operador '!' para evitar avisos de nulos si estás en .NET 8
        internal ILogger Logger => (ILogger)_serviceCollection.GetService(typeof(ILogger))!;
        internal IConfiguration Configuration => (IConfiguration)_serviceCollection.GetService(typeof(IConfiguration))!;
        internal IUsersService ServiceUsers => (IUsersService)_serviceCollection.GetService(typeof(IUsersService))!;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor base, almacena el service collection
        /// </summary>
        /// <param name="serviceCollection"></param>
        public BaseController(IServiceProvider serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        #endregion
    }
}