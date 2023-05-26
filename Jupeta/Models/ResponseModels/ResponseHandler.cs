namespace Jupeta.Models.ResponseModels
{
    public class ResponseHandler
    {
        public static ApiResponse GetExceptionResponse(Exception ex)
        {
            ApiResponse response = new ApiResponse();
            response.Code = "1";
            response.ResponseData = ex.Message;
            return response;
        }

        public static ApiResponse GetAppResponse(ResponseType type, object? contract)
        {
            ApiResponse response;

            response = new ApiResponse { ResponseData = contract };
            switch (type)
            {
                case ResponseType.Success:
                    response.Code = "0";
                    response.Message = "Success";
                    break;
                case ResponseType.NotFound:
                    response.Code = "1";
                    response.Message = "No Record Available";
                    break;
                case ResponseType.Failure:
                    response.Code = "1";
                    response.Message = "Failure";
                    break;
                case ResponseType.Error:
                    response.Code = "1";
                    response.Message = "Error";
                    break;
                case ResponseType.Unauthorized:
                    response.Code = "1";
                    response.Message = "Unauthorized";
                    break;
                default:
                    break;
            }

            return response;
        }
    }
}
