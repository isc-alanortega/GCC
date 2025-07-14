using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.Shared.Dto.Core;

namespace Nubetico.WebAPI.Application.Modules.Core.Services
{
	public class DomiciliosService
	{
		private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;

		public DomiciliosService(IDbContextFactory<CoreDbContext> coreDbContextFactory)
		{
			_coreDbContextFactory = coreDbContextFactory;
		}

		public async Task<DomicilioDto?> GetDomicilioByID(int ID)
		{
			using (var context = _coreDbContextFactory.CreateDbContext())
			{
				var domicilioResult = await context.Domicilios.FirstOrDefaultAsync(dom => dom.ID_Domicilio == ID);
				if (domicilioResult == null)
				{
					return null;
				}

				var domicilio = new DomicilioDto
				{
					ID = domicilioResult.ID_Domicilio,
					IsInvoincing = domicilioResult.Es_Facturacion,
					Description = domicilioResult.Descripcion,
					Street = domicilioResult.Calle,
					StreetNumber = domicilioResult.Numero_Exterior,
					UnitNumber = domicilioResult.Numero_Interior,
					ZipCode = domicilioResult.Codigo_Postal,
					c_State = domicilioResult.c_Estado,
					c_City = domicilioResult.c_Municipio,
					c_Neighborhood = domicilioResult.c_Colonia,
					BetweenStreet1 = domicilioResult.Entre_Calle1,
					BetweenStreet2 = domicilioResult.Entre_Calle2
				};

				return domicilio;
			}
		}

		public async Task<IEnumerable<KeyValuePair<string, string>>> GetEstadosKeyValueAsync()
		{
			using (var context = _coreDbContextFactory.CreateDbContext())
			{
				var result = await context.Domicilios_Estados.Select(estado => new KeyValuePair<string, string>
				(
					estado.c_Estado,
					estado.Descripcion.ToUpper()
				)).ToListAsync();

				return result;
			}
		}

		public async Task<IEnumerable<TripletValueSAT>> GetMunicipiosListAsync(string? c_Estado = null, string? c_Municipio = null)
		{
			using (var context = _coreDbContextFactory.CreateDbContext())
			{
				var result = await context.Domicilios_Municipios
									.Where(municipio => (string.IsNullOrEmpty(c_Estado) || municipio.c_Estado == c_Estado) &&
														(string.IsNullOrEmpty(c_Municipio) || municipio.c_Municipio == c_Municipio))
									.Select(municipio => new TripletValueSAT
									{
										ID = municipio.c_Municipio,
										Codigo = municipio.c_Estado,
										Descripcion = municipio.Descripcion.ToUpper()
									}).Take(1000).ToListAsync();

				return result;
			}
		}

		public async Task<IEnumerable<TripletValueSAT>> GetColoniasListAsync(string? codigoPostal = null, string? filtro = null)
		{
			using (var context = _coreDbContextFactory.CreateDbContext())
			{
				var result = await context.Domicilios_Colonias
									.Where(colonia => (string.IsNullOrEmpty(codigoPostal) || colonia.Codigo_Postal == codigoPostal) &&
													  (string.IsNullOrEmpty(filtro) || colonia.Descripcion.Contains(filtro)))
									.Select(colonia => new TripletValueSAT
									{
										ID = colonia.c_Colonia,
										Codigo = colonia.Codigo_Postal,
										Descripcion = colonia.Descripcion.ToUpper()
									}).Take(100).ToListAsync();

				return result;
			}
		}

		public async Task<CodigoPostalDto?> GetCodigoPostalInfoAsync(string CodigoPostal)
		{
			using (var context = _coreDbContextFactory.CreateDbContext())
			{
				var result = await context.Domicilios_CP
									.Where(cp => cp.Codigo_Postal == CodigoPostal)
									.Select(cp => new CodigoPostalDto
									{
										CodigoPostal = cp.Codigo_Postal,
										c_Estado = cp.c_Estado,
										c_Municipio = cp.c_Municipio
									}).FirstOrDefaultAsync();

				return result;
			}
		}

		public async Task<int?> PostDomicilio(DomicilioDto domicilio, string userGuid)
		{
			int userID = await GetLoggedUserID(userGuid);

			using (var context = _coreDbContextFactory.CreateDbContext())
			{
				var nuevoDomicilio = new Domicilios
				{
					Es_Facturacion = domicilio.IsInvoincing,
					Descripcion = !string.IsNullOrEmpty(domicilio.Description) ? domicilio.Description.ToUpper().Trim() : null,
					Calle = !string.IsNullOrEmpty(domicilio.Street) ? domicilio.Street.ToUpper().Trim() : null,
					Numero_Exterior = !string.IsNullOrEmpty(domicilio.StreetNumber) ? domicilio.StreetNumber.ToUpper().Trim() : null,
					Numero_Interior = !string.IsNullOrEmpty(domicilio.UnitNumber) ? domicilio.UnitNumber.ToUpper().Trim() : null,
					Codigo_Postal = domicilio.ZipCode,
					c_Pais = "MEX",
					c_Estado = domicilio.c_State,
					c_Municipio = domicilio.c_City,
					c_Colonia = domicilio.c_Neighborhood,
					Entre_Calle1 = !string.IsNullOrEmpty(domicilio.BetweenStreet1) ? domicilio.BetweenStreet1.ToUpper().Trim() : null,
					Entre_Calle2 = !string.IsNullOrEmpty(domicilio.BetweenStreet2) ? domicilio.BetweenStreet2.ToUpper().Trim() : null,
					Fecha_Alta = DateTime.Now,
					Id_Usuario_Alta = userID
				};

				context.Domicilios.Add(nuevoDomicilio);
				var save = await context.SaveChangesAsync();
				if (save <= 0)
				{
					return null;
				}

				return nuevoDomicilio.ID_Domicilio;
			}
		}

		public async Task UpdateDomicilio(DomicilioDto domicilio, string userGuid)
		{
			int userID = await GetLoggedUserID(userGuid);

			using (var context = _coreDbContextFactory.CreateDbContext())
			{
				var actDomicilio = await context.Domicilios.FirstOrDefaultAsync(dom => dom.ID_Domicilio == domicilio.ID);
				if (actDomicilio == null)
				{
					throw new Exception("No se encontró el Domicilio.");
				}

				actDomicilio.Es_Facturacion = domicilio.IsInvoincing;
				actDomicilio.Descripcion = !string.IsNullOrEmpty(domicilio.Description) ? domicilio.Description.ToUpper().Trim() : null;
				actDomicilio.Calle = !string.IsNullOrEmpty(domicilio.Street) ? domicilio.Street.ToUpper().Trim() : null; ;
				actDomicilio.Numero_Exterior = !string.IsNullOrEmpty(domicilio.StreetNumber) ? domicilio.StreetNumber.ToUpper().Trim() : null; ;
				actDomicilio.Numero_Interior = !string.IsNullOrEmpty(domicilio.UnitNumber) ? domicilio.UnitNumber.ToUpper().Trim() : null; ;
				actDomicilio.Codigo_Postal = domicilio.ZipCode;
				actDomicilio.c_Estado = domicilio.c_State;
				actDomicilio.c_Municipio = domicilio.c_City;
				actDomicilio.c_Colonia = domicilio.c_Neighborhood;
				actDomicilio.Entre_Calle1 = !string.IsNullOrEmpty(domicilio.BetweenStreet1) ? domicilio.BetweenStreet1.ToUpper().Trim() : null; ;
				actDomicilio.Entre_Calle2 = !string.IsNullOrEmpty(domicilio.BetweenStreet2) ? domicilio.BetweenStreet2.ToUpper().Trim() : null; ;
				actDomicilio.Fecha_Modificacion = DateTime.Now;
				actDomicilio.Id_Usuario_Modificacion = userID;
				
				await context.SaveChangesAsync();
			}
		}

		public async Task<int> GetLoggedUserID(string userGuid)
		{
			using (var contextCore = _coreDbContextFactory.CreateDbContext())
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
