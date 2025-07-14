using Nubetico.Shared.Dto.Common;

namespace Nubetico.WebAPI.Application.Utils
{
    public static class ResponseService
    {
        public static BaseResponseDto<T> Response<T>(int statusCode, T? data = default, string? message = null)
        {
            bool success = statusCode >= 200 && statusCode < 300;

            var result = new BaseResponseDto<T>
            {
                StatusCode = statusCode,
                Success = success,
                Data = data,
                Message = message ?? string.Empty
            };

            return result;
        }
    }
}
