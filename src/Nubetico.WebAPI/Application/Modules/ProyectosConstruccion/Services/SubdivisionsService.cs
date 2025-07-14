using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services
{
	public class SubdivisionsService
	{
		private readonly IDbContextFactory<ProyectosConstruccionDbContext> _dbContextFactory;
		private readonly IDbContextFactory<CoreDbContext> _dbContextFactoryCore;

		public SubdivisionsService(IDbContextFactory<ProyectosConstruccionDbContext> dbContextFactory, IDbContextFactory<CoreDbContext> dbContextFactoryCore)
		{
			_dbContextFactory = dbContextFactory;
			_dbContextFactoryCore = dbContextFactoryCore;
		}

        public async Task<PaginatedListDto<SubdivisionsDto>> GetPaginatedSubdivisions(int limit, int offset, string? filtername)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				var query = context.Fraccionamientos.AsQueryable();
				var result = new PaginatedListDto<SubdivisionsDto>
				{
					RecordsTotal = await query.CountAsync(),
					Data = await query
						.Skip(offset)
						.Take(limit)
						.Where(query => query.Habilitado && (string.IsNullOrEmpty(filtername) || query.Nombre.ToUpper().Contains(filtername.ToUpper())))
						.Select(query => new SubdivisionsDto
						{
							UUID = query.UUID,
							Folio = query.Folio,
							Subdivision = query.Nombre,
							PostalCode = query.CodigoPostal
						}).ToListAsync()
				};
				result.RecordsFiltered = result.Data.Count;

				return result;
			}
		}

        // After filter by IDSucursal
        public async Task<IEnumerable<KeyValuePair<int, string>>> GetSubdivisionsKeyValue()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var result = await context.Fraccionamientos
                    .Where(subdivision => subdivision.Habilitado)
                    .Select(subdivision => new KeyValuePair<int, string>
					(
						subdivision.Id_Fraccionamiento,
						subdivision.Nombre.ToUpper()
					)).ToListAsync();

                return result;
            }
        }

        // After filter by IDSucursal
        public async Task<IEnumerable<KeyValuePair<int, string>>> GetStagesKeyValue(int? subdivisionID)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
				var result = await context.Fraccionamientos_Etapas
                    .Where(stage => stage.Habilitado && (!subdivisionID.HasValue || stage.Id_Fraccionamiento == subdivisionID))
                    .Select(stage => new KeyValuePair<int, string>
					(
						stage.Id_Fraccionamiento_Etapa,
						stage.Nombre.ToUpper()
					)).ToListAsync();

                return result;
            }
        }

        // After filter by IDSucursal
        public async Task<IEnumerable<KeyValuePair<int, string>>> GetBlocksKeyValue(int? subdivisionID, int? stageID)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var result = await context.Fraccionamientos_Etapas_Manzanas
                    .Where(block => block.Habilitado && 
						  (!subdivisionID.HasValue || block.Id_Fraccionamiento == subdivisionID) &&
						  (!stageID.HasValue || block.Id_Fraccionamiento_Etapa == stageID))
                    .Select(block => new KeyValuePair<int, string>
					(
						block.Id_Fraccionamiento_Etapa_Manzana,
						block.Nombre.ToUpper()
					)).ToListAsync();

                return result;
            }
        }

        public async Task<SubdivisionsDto?> GetSubdivisionByGuid(Guid UUID)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				var subdivisionResult = await context.Fraccionamientos.FirstOrDefaultAsync(sub => sub.UUID == UUID && sub.Habilitado);
				if (subdivisionResult == null)
				{
					return null;
				}

				var subdivision = new SubdivisionsDto
				{
					UUID = subdivisionResult.UUID,
					ID = subdivisionResult.Id_Fraccionamiento,
					Subdivision = subdivisionResult.Nombre,
					Folio = subdivisionResult.Folio,
					PathLogo = subdivisionResult.Logotipo,
					PostalCode = subdivisionResult.CodigoPostal,
					Enabled = subdivisionResult.Habilitado
				};

				subdivision.Stages = await context.Fraccionamientos_Etapas
					.Where(stage => stage.Id_Fraccionamiento == subdivisionResult.Id_Fraccionamiento && stage.Habilitado)
					.Select(stage => new SubdivisionStage
					{
						ID = stage.Id_Fraccionamiento_Etapa,
						ID_Subdivision = stage.Id_Fraccionamiento,
						Stage = stage.Nombre,
						Description = stage.Descripcion,
						Sequence = stage.Secuencia
					}).OrderBy(stage => stage.Sequence).ToListAsync();

				if (subdivision.Stages != null && subdivision.Stages.Any())
				{
					foreach (var stage in subdivision.Stages)
					{
						stage.Blocks = await context.Fraccionamientos_Etapas_Manzanas
												.Where(block => block.Id_Fraccionamiento == subdivisionResult.Id_Fraccionamiento && block.Id_Fraccionamiento_Etapa == stage.ID && block.Habilitado)
												.Select(block => new SubdivisionBlock
												{
													ID = block.Id_Fraccionamiento_Etapa_Manzana,
													ID_Subdivision = block.Id_Fraccionamiento,
													ID_Subdivision_Stage = block.Id_Fraccionamiento_Etapa,
													Block = block.Nombre
												}).ToListAsync();
					}
				}

				return subdivision;
			}
		}

		public async Task<string?> PostSubdivision(SubdivisionsDto subdivision, string userGuid)
		{
			int userID = await GetLoggedUserID(userGuid);

			using (var context = _dbContextFactory.CreateDbContext())
			{
				var dateNow = DateTime.Now;

				// Add the Subdivision
				var newSubdivision = new Fraccionamientos
				{
					Nombre = subdivision.Subdivision.ToUpper().Trim(),
					CodigoPostal = subdivision.PostalCode.Value,
					Fecha_Alta = dateNow,
					Id_Usuario_Alta = userID,
					Id_Sucursal = 1,// PENDIENTE CAMBIAR
					Folio = "-",
					Habilitado = true
				};

				context.Fraccionamientos.Add(newSubdivision);
				int saveSubdivision = await context.SaveChangesAsync();

				// Disable Subdivision if an error occurred
				if (saveSubdivision <= 0)
				{
					newSubdivision = await context.Fraccionamientos.FirstOrDefaultAsync(sub => sub.Id_Fraccionamiento == newSubdivision.Id_Fraccionamiento);
					if (newSubdivision != null)
					{
						newSubdivision.Habilitado = false;
						await context.SaveChangesAsync();
					}
					
					throw new Exception("No se guardó correctamente el fraccionamiento.");
				}

				// Add Stages
				if (subdivision.Stages != null && subdivision.Stages.Any())
				{
					for (int i = 0; i < subdivision.Stages.Count(); i++)
					{
						var stage = subdivision.Stages.ElementAt(i);
						var newStage = new Fraccionamientos_Etapas
						{
							Id_Fraccionamiento = newSubdivision.Id_Fraccionamiento,
							Nombre = stage.Stage.ToUpper().Trim(),
							Descripcion = string.IsNullOrEmpty(stage.Description) ? null : stage.Description.ToUpper().Trim(),
							Secuencia = i + 1,
							Habilitado = true
						};

						context.Fraccionamientos_Etapas.Add(newStage);
						await context.SaveChangesAsync();

						// Add Blocks
						if (stage.Blocks != null && stage.Blocks.Any())
						{
							foreach (var block in stage.Blocks)
							{
								var newBlock = new Fraccionamientos_Etapas_Manzanas
								{
									Id_Fraccionamiento = newSubdivision.Id_Fraccionamiento,
									Id_Fraccionamiento_Etapa = newStage.Id_Fraccionamiento_Etapa,
									Nombre = block.Block.ToUpper().Trim(),
									Habilitado = true,
									Fecha_Alta = dateNow,
									Id_Usuario_Alta = userID
								};

								context.Fraccionamientos_Etapas_Manzanas.Add(newBlock);
								await context.SaveChangesAsync();
							}
						}
					}
				}

				return newSubdivision.UUID.ToString();
			}
		}

		public async Task UpdateSubdivision(SubdivisionsDto subdivision, string userGuid)
		{
			int userID = await GetLoggedUserID(userGuid);
			var dateNow = DateTime.Now;

			using (var context = _dbContextFactory.CreateDbContext())
			{
				// Update Subdivision
				var updateSubdivision = await context.Fraccionamientos.FirstOrDefaultAsync(sub => sub.UUID == subdivision.UUID);
				if (updateSubdivision == null)
				{
					throw new Exception("No se encontró el Fraccionamiento.");
				}

				updateSubdivision.Nombre = subdivision.Subdivision;
				updateSubdivision.CodigoPostal = subdivision.PostalCode.Value;
				updateSubdivision.Fecha_Modificacion = dateNow;
				updateSubdivision.Id_Usuario_Modificacion = userID;

				/****************************************************
				 * STAGES
				*****************************************************/

				// Get registered Stages
				var registeredStages = await (from stage in context.Fraccionamientos_Etapas
											  where stage.Id_Fraccionamiento == updateSubdivision.Id_Fraccionamiento && stage.Habilitado
											  orderby stage.Secuencia
											  select stage).ToListAsync();
				
				if (subdivision.Stages != null && subdivision.Stages.Any())
				{
					// Order Stages by sequence
					subdivision.Stages = subdivision.Stages.OrderBy(stage => stage.Sequence);

					for (int i = 0; i < subdivision.Stages.Count(); i++)
					{
						var stage = subdivision.Stages.ElementAt(i);
						var updateStage = await context.Fraccionamientos_Etapas.FirstOrDefaultAsync(s => s.Id_Fraccionamiento_Etapa == stage.ID);
						int stageID = 0;

						if (updateStage == null)
						{
							// Add new Stages
							var newStage = new Fraccionamientos_Etapas
							{
								Id_Fraccionamiento = updateSubdivision.Id_Fraccionamiento,
								Nombre = stage.Stage.ToUpper().Trim(),
								Descripcion = string.IsNullOrEmpty(stage.Description) ? null : stage.Description.ToUpper().Trim(),
								Secuencia = i + 1,
								Habilitado = true
							};
							context.Fraccionamientos_Etapas.Add(newStage);
							await context.SaveChangesAsync();

							stageID = newStage.Id_Fraccionamiento_Etapa;
						}
						else
						{
							// Update Stage
							updateStage.Nombre = stage.Stage.ToUpper().Trim();
							updateStage.Descripcion = string.IsNullOrEmpty(stage.Description) ? null : stage.Description.ToUpper().Trim();
							updateStage.Secuencia = i + 1;
							updateStage.Habilitado = true;

							stageID = updateStage.Id_Fraccionamiento_Etapa;
						}

						/****************************************************
						 * BLOCKS
						*****************************************************/
						// Get registered Blocks
						var registeredBlocks = await (from block in context.Fraccionamientos_Etapas_Manzanas
													  where block.Id_Fraccionamiento == updateSubdivision.Id_Fraccionamiento &&
															block.Id_Fraccionamiento_Etapa == stageID &&
															block.Habilitado
													  select block).ToListAsync();

						if (stage.Blocks != null && stage.Blocks.Any())
						{
							foreach (var block in stage.Blocks)
							{
								var updateBlock = await context.Fraccionamientos_Etapas_Manzanas.FirstOrDefaultAsync(s => s.Id_Fraccionamiento_Etapa_Manzana == block.ID);
								if (updateBlock == null)
								{
									// Add new Block
									var newBlock = new Fraccionamientos_Etapas_Manzanas
									{
										Id_Fraccionamiento = updateSubdivision.Id_Fraccionamiento,
										Id_Fraccionamiento_Etapa = stageID,
										Nombre = block.Block.ToUpper().Trim(),
										Habilitado = true,
										Fecha_Alta = dateNow,
										Id_Usuario_Alta = userID
									};
									context.Fraccionamientos_Etapas_Manzanas.Add(newBlock);
								}
								else
								{
									// Update Block
									updateBlock.Nombre = block.Block.ToUpper().Trim();
									updateBlock.Habilitado = true;
									updateBlock.Fecha_Modificacion = dateNow;
									updateBlock.Id_Usuario_Modificacion = userID;
								}
							}

							// Disabled registared blocks that where removed from the list
							if (registeredBlocks != null && registeredBlocks.Any())
							{
								foreach (var block in registeredBlocks)
								{
									if (!stage.Blocks.Any(b => b.ID == block.Id_Fraccionamiento_Etapa_Manzana))
									{
										block.Fecha_Modificacion = dateNow;
										block.Id_Usuario_Modificacion = userID;
										block.Habilitado = false;
									}
								}
							}
						}
						else if (registeredBlocks != null && registeredBlocks.Any())
						{
							// Disabled registared blocks that where removed from the list
							registeredBlocks.ForEach(registered =>
							{
								registered.Fecha_Modificacion = dateNow;
								registered.Id_Usuario_Modificacion = userID;
								registered.Habilitado = false;
							});
						}
					}

					// Disabled registared stages that where removed from the list
					if (registeredStages != null && registeredStages.Any())
					{
						foreach (var registered in registeredStages)
						{
							bool removeStage = !subdivision.Stages.Any(stage => stage.ID == registered.Id_Fraccionamiento_Etapa);
							if (removeStage)
							{
								registered.Habilitado = false;

								var registeredStageBlocks = await (from block in context.Fraccionamientos_Etapas_Manzanas
																   where block.Id_Fraccionamiento == registered.Id_Fraccionamiento &&
																		 block.Id_Fraccionamiento_Etapa == registered.Id_Fraccionamiento_Etapa &&
																		 block.Habilitado
																   select block).ToListAsync();

								if (registeredStageBlocks != null && registeredStageBlocks.Any())
								{
									registeredStageBlocks.ForEach(registeredBlock =>
									{
										registeredBlock.Habilitado = false;
										registeredBlock.Fecha_Modificacion = dateNow;
										registeredBlock.Id_Usuario_Modificacion = userID;
									});
								}
							}
						}
					}
				}
				else if (registeredStages != null && registeredStages.Any())
				{
					// Disabled registared stages that where removed from the list
					foreach (var stage in registeredStages)
					{
						stage.Habilitado = false;

						var registeredBlocks = await (from block in context.Fraccionamientos_Etapas_Manzanas
											where block.Id_Fraccionamiento == stage.Id_Fraccionamiento &&
												  block.Id_Fraccionamiento_Etapa == stage.Id_Fraccionamiento_Etapa
											select block).ToListAsync();

						// Disabled registared blocks that where removed from the list
						if (registeredBlocks != null && registeredBlocks.Any())
						{
							registeredBlocks.ForEach(block =>
							{
								block.Fecha_Modificacion = dateNow;
								block.Id_Usuario_Modificacion = userID;
								block.Habilitado = false;
							});
						}
					}
				}

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
	}
}
