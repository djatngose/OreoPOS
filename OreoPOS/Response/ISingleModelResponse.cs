using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OreoPOS.Responses
{
   public interface ISingleModelResponse<TModel> : IResponse
    {
        TModel Model { get; set; }
    }
}
