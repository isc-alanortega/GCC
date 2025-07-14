using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.Common
{
    public class ResponseDto<T>
    {
        public string? Message { get; set; }
        public T? Result { get; set; }
        public bool Success { get; set; }

        public ResponseDto() { }

        public ResponseDto(bool success, string? message = null, T? data = default) 
        {
            this.Success = success;
            this.Message = message;
            this.Result = data;
        }
    }
}
