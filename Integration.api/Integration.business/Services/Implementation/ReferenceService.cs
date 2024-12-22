using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration.business.DTOs.ModuleDTOs;
using Integration.business.DTOs.ReferenceDTos;
using Integration.business.Services.Interfaces;
using Integration.data.Data;
using Integration.data.Models;
using Microsoft.EntityFrameworkCore;

namespace Integration.business.Services.Implementation
{
    public class ReferenceService : IReferenceService
    {
        private readonly AppDbContext _appDbContext;

        public ReferenceService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<ApiResponse<bool>> AddReference(ReferenceForCreateDTO referenceForCreate)
        {
            var Module=await _appDbContext.modules.FirstOrDefaultAsync(c=>c.Id==referenceForCreate.moduleId);

            if (Module is null)
                return new ApiResponse<bool>(false, "Module Not Found");

            var Reference = new TableReference()
            {
                Alter= referenceForCreate.Alter,
                LocalName= referenceForCreate.LocalName,
                PrimaryName= referenceForCreate.PrimaryName,
                TableFromName= referenceForCreate.TableFromName,
                ModuleId= referenceForCreate.moduleId,
            };

            await _appDbContext.References.AddAsync(Reference);
            var isAdded=await _appDbContext.SaveChangesAsync();
            if(isAdded <= 0)
                return new ApiResponse<bool>(false, "Error When Adding References");

            return new ApiResponse<bool>(true, "References Added Successfully");

        }
    }
}
