using System.Text.Json.Serialization;

namespace secondhand_car_backend.Utils
{

    /// <summary>
    ///     Tipos de orden
    /// </summary>
    public enum OrderType
    {
        up = 0,
        down = 1
    }

    /// <summary>
    ///     Tipos de filtrado para las tablas
    /// </summary>
    public enum FilterType
    {
        contains,
        equals,
        greatherThan,
        greatherThanEqual,
        isNullOrEmpty,
        lessThan,
        lessThanEqual,
        notEquals
    }

    /// <summary>
    ///		Especificación de los códigos de error
    /// </summary>
    public enum ResponseCodes
    {

        Ok = 0,
        
        ErrorToken = 1,
        
		OriginAccess = 2,
        
		InvalidUserName = 3,
        
        InvalidPassword = 4,
        
        BdConectionFailed = 5,
        
        NoDataFound = 6,
        
        SaveDataFailed = 7,
        
        SinEmpresa = 8,
        
        AccesoNoAutorizado = 9,
        
        DataError = 10,
       
        OtherError = 11,
      
        InvalidModel = 12,
        
        InvalidAccessType = 13,
   
        InvalidToken = 14
    }

    
    
}