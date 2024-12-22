using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.business.Services.Interfaces
{
    public interface ISyncService
    {
        Task<ApiResponse<int>> SyncNormal(int moduleId);

        Task<ApiResponse<int>> SyncOperation(int moduleId);
    }   

}




