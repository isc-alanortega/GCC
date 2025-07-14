using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Queries
{
    public static class ModelsQueries
    {
        public static IQueryable<ElementsDropdownForm> GetDropdownModels(this IQueryable<Modelos> models)
            => models.Select(item => new ElementsDropdownForm
            {
                Id = item.ID_Modelo,
                Name = item.Nombre
            });
    }
}
