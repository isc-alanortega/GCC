using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.WebAPI.Application.Modules.Core.Services;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Queries;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services
{
    public class LotsService
    {
        private readonly IDbContextFactory<ProyectosConstruccionDbContext> _dbContextFactory;
        private readonly IDbContextFactory<CoreDbContext> _dbContextFactoryCore;
        private readonly DomiciliosService _addressesService;

        public LotsService(IDbContextFactory<ProyectosConstruccionDbContext> dbContextFactory, IDbContextFactory<CoreDbContext> dbContextFactoryCore, DomiciliosService addressesService)
        {
            _dbContextFactory = dbContextFactory;
            _dbContextFactoryCore = dbContextFactoryCore;
            _addressesService = addressesService;
        }

        public async Task<PaginatedListDto<LotsDto>> GetPaginatedLots(int limit, int offset, FilterLots filter)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var query = context.vLotes.AsQueryable();
                var data = await query
                        .Skip(offset)
                        .Take(limit)
                        .Where(query => query.Habilitado &&
                                        (!filter.SubdivisionID.HasValue || filter.SubdivisionID == 0 || query.Id_Fraccionamiento == filter.SubdivisionID) &&
                                        (!filter.StageID.HasValue || filter.StageID == 0 || query.Id_Etapa == filter.StageID) &&
                                        (!filter.BlockID.HasValue || filter.BlockID == 0 || query.Id_Manzana == filter.BlockID) &&
                                        (!filter.ModelID.HasValue || filter.ModelID == 0 || query.Id_Modelo == filter.ModelID) &&
                                        (!filter.LotNumber.HasValue || query.Numero_Lote == filter.LotNumber))
                        .Select(query => new LotsDto
                        {
                            UUID = query.UUID,
                            ID = query.Id_Lote,
                            Folio = query.Folio,
                            Number = query.Numero_Lote,
                            RegistrationDate = query.Fecha_Alta,
                            SubdivisionID = query.Id_Fraccionamiento,
                            Subdivision = query.Fraccionamieno,
                            StageID = query.Id_Etapa,
                            Stage = query.Etapa,
                            BlockID = query.Id_Manzana,
                            Block = query.Manzana,
                            Model = query.Modelo,
                            FrontMeasure = $"{query.Frente:0.###} m2",
                            BottomMeasure = $"{query.Fondo:0.###} m2",
                            SurfaceMeasure = $"{query.Superficia:0.###} m2",
                            Status = "PENDIENTE"// PENDIENTE SABER DE DÓNDE SE OBTIENE
                        }).OrderBy(query => query.RegistrationDate).ToListAsync();


                var result = new PaginatedListDto<LotsDto>
                {
                    RecordsTotal = await query.CountAsync(),
                    Data = data,
                    RecordsFiltered = data.Count
                };

                return result;
            }
        }

        public async Task<LotsDetail?> GetLotByGuid(Guid UUID)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var lotResult = await context.vLotes.FirstOrDefaultAsync(lot => lot.UUID == UUID && lot.Habilitado);
                if (lotResult == null)
                {
                    return null;
                }

                var lot = new LotsDetail
                {
                    UUID = lotResult.UUID,
                    ID = lotResult.Id_Lote,
                    Folio = lotResult.Folio,
                    Number = lotResult.Numero_Lote,
                    SubdivisionID = lotResult.Id_Fraccionamiento,
                    StageID = lotResult.Id_Etapa,
                    BlockID = lotResult.Id_Manzana,
                    FrontMeasure = lotResult.Frente,
                    BottomMeasure = lotResult.Fondo,
                    SurfaceMeasure = lotResult.Superficia,
                    ModelID = lotResult.Id_Modelo,
                    AddressID = lotResult.Id_Domicilio,
                    Enabled = lotResult.Habilitado
                };

                return lot;
            }
        }

		// After filter by IDSucursal
		public async Task<IEnumerable<KeyValuePair<int, string>>> GetModelsKeyValue()
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				var result = await context.Modelos
					.Select(block => new KeyValuePair<int, string>
					(
						block.ID_Modelo,
						block.Nombre
					)).ToListAsync();

				return result;
			}
		}

		// After filter by IDSucursal
		public async Task<IEnumerable<KeyValuePair<int, string>>> GetStatusLotKeyValue()
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				var result = await context.Cat_Estatus
					.Select(status => new KeyValuePair<int, string>
					(
						status.Id_Estatus,
						status.Nombre_ES
					)).ToListAsync();

				return result;
			}
		}

		public async Task<string?> PostLot(LotsDetail lot, string userGuid)
        {
            int userID = await GetLoggedUserID(userGuid);
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var dateNow = DateTime.Now;
                int? addressID = null;

                if (lot.Address != null)
                {
                    addressID = await _addressesService.PostDomicilio(lot.Address, userGuid);
                    if (addressID == null)
                    {
                        throw new Exception("Ocurrió un error al guardar el Domicilio.");
                    }
                }

				// Creat the new Lot
				var newLot = new Lotes
                {
                    Id_Fraccionamiento = lot.SubdivisionID.Value,
                    Id_Etapa = lot.StageID.Value,
                    Id_Manzana = lot.BlockID.Value,
                    Numero_Lote = lot.Number.Value,
                    Superficia = lot.SurfaceMeasure.Value,
                    Frente = lot.FrontMeasure.Value,
                    Fondo = lot.BottomMeasure.Value,
                    Id_Uso = 1,
                    Id_Estado = 1,
                    Id_Modelo = 1,
                    Fecha_Alta = dateNow,
                    Id_Usuario_Alta = userID,
                    Folio = "-",
                    Id_Domicilio = addressID,
                    Habilitado = true
                };
                context.Lotes.Add(newLot);
                await context.SaveChangesAsync();

                return newLot.UUID.ToString();
            }
        }

		public async Task UpdateLot(LotsDetail lot, string userGuid)
		{
			int userID = await GetLoggedUserID(userGuid);
			using (var context = _dbContextFactory.CreateDbContext())
			{
				var updateLot = await context.Lotes.FirstOrDefaultAsync(l => l.UUID == lot.UUID);
				if (updateLot == null)
				{
					throw new Exception("No se encontró el Lote.");
				}

                // Update address
                int? addressID = lot.AddressID;
                if (addressID.HasValue)
                {
					await _addressesService.UpdateDomicilio(lot.Address, userGuid);
                }
                else
                {
					addressID = await _addressesService.PostDomicilio(lot.Address, userGuid);
				}

				// Update lot
				updateLot.Numero_Lote = lot.Number.Value;
				updateLot.Id_Etapa = lot.StageID.Value;
				updateLot.Id_Manzana = lot.BlockID.Value;
				updateLot.Frente = lot.FrontMeasure.Value;
				updateLot.Fondo = lot.BottomMeasure.Value;
				updateLot.Superficia = lot.SurfaceMeasure.Value;
                updateLot.Id_Domicilio = addressID;

				await context.SaveChangesAsync();
			}
		}

		public async Task<int> GetLoggedUserID(string userGuid)
        {
            using (var contextCore = _dbContextFactoryCore.CreateDbContext())
            {
                var user = await contextCore.vUsuariosPersonas.FirstOrDefaultAsync(user => user.GuidUsuarioDirectory.ToString() == userGuid);
                if (user == null)
                {
                    throw new Exception("No se encontró el ID del usuario en sesión.");
                }

                return user.IdUsuario;
            }
        }

        public async Task<bool> CheckBlockInLotsByIdAsync(int blockId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var response = await context.vLotes.LotsByBlockId(blockId).FirstOrDefaultAsync();
            return response is not null;
        }

        public async Task<bool> CheckStageInLotsByIdAsync(int stageId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var response = await context.vLotes.LotsByStageId(stageId).FirstOrDefaultAsync();
            return response is not null;
        }
    }
}
