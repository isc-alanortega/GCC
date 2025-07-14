using Nubetico.DAL.Models.ProyectosConstruccion;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Queries
{
    public static class LotsQueries
    {
        public static IQueryable<Lotes> LotsBySubdivisionId(this IQueryable<Lotes> lots, int subdivisionId)
            => lots.Where(item => item.Id_Fraccionamiento == subdivisionId && item.Habilitado);

        public static IQueryable<Lotes> LotsByStepId(this IQueryable<Lotes> lots, int stepId)
            => lots.Where(item => item.Id_Etapa == stepId && item.Habilitado);

        public static IQueryable<vLotes> LotsByBlockId(this IQueryable<vLotes> lots, int blockId)
            => lots.Where(item => item.Id_Manzana == blockId && item.Habilitado);

        public static IQueryable<vLotes> LotsByStageId(this IQueryable<vLotes> lots, int stageId)
           => lots.Where(item => item.Id_Etapa == stageId && item.Habilitado);
    }
}
