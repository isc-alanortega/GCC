using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Queries
{
    public static class CatEstatusQueries
    {
        public static IQueryable<ElementsDropdownForm> GetCatEstatusDropDown(this IQueryable<Cat_Estatus> catStatus) 
            => catStatus
                .Where(item => item.Habilitado)
                .Select(item => new ElementsDropdownForm() 
                { 
                    Id = item.Id_Estatus, 
                    Name = item.Nombre_ES 
                });
    }
}
