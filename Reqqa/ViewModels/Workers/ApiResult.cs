using System;
using System.Collections.Generic;

namespace Salony.ViewModels.Workers
{
    public class ApiResult<T>
    {
        public Boolean Result { get; set; }
        public List<T> Data { get; set; }
        public List<String> Errors { get; set; }
    }
}
