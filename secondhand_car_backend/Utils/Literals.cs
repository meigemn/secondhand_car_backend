namespace secondhand_car_backend.Utils
{
    public static class Literals
    {
        public static class UserMessages
        {
            public const string UserNotFound = "El usuario no existe.";
            public const string InvalidPassword = "La contraseña es incorrecta.";
            public const string EmailAlreadyRegistered = "Este email ya está en uso.";
            public const string UserCreatedSuccess = "Usuario creado correctamente.";
        }

        public static class Auth
        {
            public const string InvalidToken = "Token no válido.";
        }

        public static class  Credentials
        {
            public const string InvalidCredentials = "Credenciales inválidas";

        }
        public static class UserServiceErrors
        {
            public const string LoginError = "Error en el Login. Services->UserService->Login";
            public const string GetAllError = "Error en GetAll. Services->UserService->GetAll";
            public const string GetByIdError = "Error en GetById. Services->UserService->GetById";
            public const string CreateAdminError = "Error en CreateAdmin. Services->UserService->CreateAdmin";
            public const string RemoveError = "Error en Remove. Services->UserService->Remove";
            public const string UpdateError = "Error en Update. Services->UserService->Update";


        }
    }
}