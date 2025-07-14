using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.ProjectSectionDetails;
using System.Linq;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.ProjectSectionDetails
{
    public class ProjectSectionDetailsService(
        IDbContextFactory<ProyectosConstruccionDbContext> proyectosConstruccionDbContextFactory,
        IStringLocalizer<SharedResources>? LocalizerService
    )
    {
        #region INJECTIONS
        private readonly IDbContextFactory<ProyectosConstruccionDbContext> _pcContext = proyectosConstruccionDbContextFactory;
        private readonly IStringLocalizer<SharedResources> _localizer = LocalizerService!;
        #endregion

        public async Task<ResponseDto<PaginatedListDto<SectionDetailsDto>>> GetSectionDetailsPaginatedListAsync(RequestSectionDetailsCatDto request)
        {
            using var db = _pcContext.CreateDbContext();
            var query = db.vProyectos_Secciones.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Project))
                query = query.Where(item => item.Proyecto.Contains(request.Project));

            if (!string.IsNullOrWhiteSpace(request.Section))
                query = query.Where(item => item.Seccion.Contains(request.Section));

            query = query.OrderByDescending(item => item.Proyecto).ThenByDescending(item => item.Seccion);

            // Aplicar ordenamiento
            //query = sortDirection == "desc"
            //    ? query.OrderByDescending(sortExpression)
            //    : query.OrderBy(sortExpression);

            return new(success: true, data: await ProjectSectionDetailsMapper.GetSectionsPaginetedListMap(query, request.OffSet, request.Limit));
        }

        public async Task<ResponseDto<ResponseSectionDetailsDto>> GetSectionDetailsByGuidAsync(Guid sectionGuid)
        {
            using (var db = _pcContext.CreateDbContext())
            {
                var sectionDetails = await db.vProyectos_Secciones.FirstOrDefaultAsync(item => item.SeccionGuid == sectionGuid);
                if (sectionDetails == null)
                    return new(success: false, _localizer["ProjectSectionDetails.Error.NotFoundSectionDetails"]);

                var sectionLots = await db.vSecciones_Lotes.Where(item => item.SeccionGuid == sectionGuid).ToListAsync();

                return new(success: true, data: new ResponseSectionDetailsDto()
                {
                    SectionDetails = ProjectSectionDetailsMapper.GetSectionDetailsMap(sectionDetails)!,
                    SectionLots = ProjectSectionDetailsMapper.GetSectionLotsListMap(sectionLots)!
                });
            }
        }
    }
}
