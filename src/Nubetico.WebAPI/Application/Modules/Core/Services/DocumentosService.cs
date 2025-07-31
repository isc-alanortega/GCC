using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion;

namespace Nubetico.WebAPI.Application.Modules.Core.Services
{
	public class DocumentosService
	{
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;

        public DocumentosService(IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            _coreDbContextFactory = coreDbContextFactory;
        }

        public (string? Error, string? Exception, string? Additional_Error_Data, object? Result) ValidateExcel(IFormFile excelFile, string excelType)
		{
			if (excelFile == null || excelType.Length == 0)
							return ("Core.Files.Errors.EmptyFile", null, null, null);


			using (var workbook = new XLWorkbook(excelFile.OpenReadStream()))
			{
				var workSheet = workbook.Worksheet(1);

				return excelType switch
				{
                    "MODELEXCEL" => ValidateModel(workSheet),
					_ => (null, "No se encontró el tipo de excel [WebApi.Service].", null, null)
                };
			}
		}

		public (string? Error, string? Exception, string? Additional_Error_Data, IEnumerable<InsumosModelos>? Result) ValidateModel(IXLWorksheet workSheet)
		{
			try
			{
				var suppliesList = new List<InsumosModelos>();

				// Retrieve rows from Supplies Table
				var suppliesTable = workSheet!.RangeUsed()!.RowsUsed().Where(row => row.RowNumber() >= 12);
				if (suppliesTable == null || !suppliesTable.Any())
				{
					return ("Supplies.Missing.Data", null, null, null);
				}

				string currentGroup = string.Empty;
				string currentCategory = string.Empty;
				string currentPU = string.Empty;
				string currentSupply = string.Empty;

				int secuenceGruop = 0;
				int secuenceCategory = 0;
				int secuenceUnitPrice = 0;

                for (int i = 0; i < suppliesTable.Count(); i++)
				{
					var row = GetRowNumber(i, suppliesTable);

					// Read the values
					string cellGroup = GetCellValue(workSheet, row, column: 1);
					if (!string.IsNullOrWhiteSpace(cellGroup))
					{
                        if (cellGroup.Contains("TOTAL"))
                        {
                            cellGroup = string.Empty;
							secuenceCategory = 0;
                            break;
                        }

                        currentGroup = cellGroup.Contains('*')
							? cellGroup
							: string.Concat(currentGroup, " ", cellGroup);

						currentGroup = currentGroup.Replace("*", string.Empty);
						secuenceGruop++;
						continue;
					}

					// Validate isn't a Total row
					
					string cellCategory = GetCellValue(workSheet, row, column: 2);
					if (!string.IsNullOrEmpty(cellCategory))
					{
                        // Validate isn't a Total row
                        if (cellCategory.Contains("TOTAL"))
                        {
                            cellCategory = string.Empty;
							secuenceUnitPrice = 0;
                            continue;
                        }

                        bool isCompletedCategoryName = false;
						
                        while (!isCompletedCategoryName) 
						{
                            var rowTest = GetRowNumber(i + 1, suppliesTable);
                            string cell = GetCellValue(workSheet, rowTest, column: 2);
							if(string.IsNullOrEmpty(cell) || cellCategory.Contains("TOTAL"))
							{
                                isCompletedCategoryName = true;
                                break;
                            }

							cellCategory += cell;
							i++;
                        }

                        currentCategory = cellCategory;
						secuenceCategory++;
                        continue;
					}

					string cellPU = GetCellValue(workSheet, row, column: 3);
					if (!string.IsNullOrEmpty(cellPU))
					{
                        // Validate isn't a Total row
                        if (cellPU.Contains("TOTAL"))
                        {
                            bool isCompletedUnitPriceCellName = false;

                            while (!isCompletedUnitPriceCellName)
                            {
                                int rowTest = GetRowNumber(i + 1, suppliesTable);
                                string cell = GetCellValue(workSheet, rowTest, column: 3);
                                if (string.IsNullOrEmpty(cell) || cell.Contains("*"))
                                {
                                    isCompletedUnitPriceCellName = true;
                                    break;
                                }

                                cellPU = string.Empty;
                                i++;
                            }

                            cellPU = string.Empty;
                            continue;
                        }

                        bool isCompletedUnitPriceName = false;

                        while (!isCompletedUnitPriceName)
                        {
                            int rowTest = GetRowNumber(i + 1, suppliesTable);
                            string cell = GetCellValue(workSheet, rowTest, column: 3);
                            if (string.IsNullOrEmpty(cell) || currentPU.Contains("TOTAL"))
                            {
                                isCompletedUnitPriceName = true;
                                break;
                            }

                            cellPU += cell;
                            i++;
                        }

                        currentPU = cellPU.Contains('*')
                            ? cellPU
                            : string.Concat(currentPU, " ", cellPU);

                        currentPU = currentPU.Replace("*", string.Empty);
						secuenceUnitPrice ++;

                        continue;
					}

					bool cellSupplyCleaned = false;
					int totalRowsCleaned = 0;

                    string cellSupply = GetCellValue(workSheet, row, column: 4);
					if (!string.IsNullOrEmpty(cellSupply))
					{
                        bool isCompletedSupplyName = false;

                        while (!isCompletedSupplyName)
						{
							int rowTest = GetRowNumber(i + 1, suppliesTable);
							string cell = GetCellValue(workSheet, rowTest, column: 4);
							if (string.IsNullOrEmpty(cell) || cell.Contains("**"))
							{
                                isCompletedSupplyName = true;
								break;
							}

                            cellSupply += $" {cell}";
							i++;
							totalRowsCleaned++;
                            row = rowTest;
							cellSupplyCleaned = true;
                        }

						currentSupply = cellSupply.Contains('*')
							? cellSupply
							: string.Concat(currentSupply, " ", cellSupply);

						currentSupply = currentSupply.Replace("*", string.Empty);
					}

					int rowPU = GetRowIsCleaned(row, cellSupplyCleaned, totalRowsCleaned);
                    string cellUnit = GetCellValue(workSheet, rowPU, column: 11);
					string cellVolume = GetCellValue(workSheet, rowPU, column: 12);
					string cellPrice = GetCellValue(workSheet, rowPU, column: 13);

					// Validate each cell
					if (string.IsNullOrEmpty(currentGroup))
					{
						return ("Supplies.Missing.Group", null, null, null);
					}

					if (string.IsNullOrEmpty(currentCategory))
					{
						return ("Supplies.Missing.Category", null, null, null);
					}

					if (string.IsNullOrEmpty(currentPU))
					{
						return ("Supplies.Missing.PU", null, null, null);
					}

					if (string.IsNullOrEmpty(currentSupply))
					{
						return ("Supplies.Missing.Supply", null, $"{row}D", null);
					}

					if (string.IsNullOrEmpty(cellUnit))
					{
						return ("Supplies.Missing.Unit", null, $"{row}K", null);
					}

					if (string.IsNullOrEmpty(cellVolume))
					{
						return ("Supplies.Missing.Volume", null, $"{row}L", null);
					}

					if (!decimal.TryParse(cellVolume, out var _volume))
					{
						return ("Supplies.Error.InvalidVolume", null, $"{row}L", null);
					}

					if (string.IsNullOrEmpty(cellPrice))
					{
						return ("Supplies.Missing.Price", null, $"{row}M", null);
					}

					if (!decimal.TryParse(cellPrice, out var _price))
					{
						return ("Supplies.Error.InvalidPrice", null, $"{row}M", null);
					}

					var supplyData = new InsumosModelos
					{
						Group = $"{secuenceGruop:D3}-{currentGroup}",
						GroupSecuence = secuenceGruop,
						Category = $"{secuenceGruop:D3}-{secuenceCategory:D3}-{currentCategory}",
						CategorySecuence = secuenceCategory,
						Unit_Price = $"{secuenceGruop:D3}-{secuenceCategory:D3}-{secuenceUnitPrice:D3}-{currentPU}",
						UnitPriceSecuence = secuenceUnitPrice,
						Supply = currentSupply,
						Unit = cellUnit,
						Type = cellUnit switch
						{
                            "JOR" => "MANO DE OBRA",
                            "HR" => "MAQUINARIA",
							_ => "MATERIALES"
                        },
						Volume = _volume,
						Price = _price
					};

					suppliesList.Add(supplyData);
				}

				return (null, null, null,  suppliesList);
			}
			catch (Exception ex)
			{
				return (null, string.Concat(ex.Message, "[WebApi.Service]."), null, null);
			}
		}

		private int GetRowNumber(int i, IEnumerable<IXLRangeRow> suppliesTable) => suppliesTable.ElementAt(i).RowNumber();

		private string GetCellValue(IXLWorksheet workSheet, int row, int column) => workSheet.Cell(row, column).Value.ToString().ToUpper().Trim();
    
		private int GetRowIsCleaned(int row, bool isCleanned, int toalCleanedRows) => isCleanned ? row - toalCleanedRows : row;

		public async Task<string?> GetInvoiceUrlBasePath(string serial, int? numericFolio, bool getPDF)
		{
			try
			{
                using (var context = _coreDbContextFactory.CreateDbContext())
                {
					var basePath = await context.Parametros.FirstAsync(parameter => parameter.IdParametro == 13);
                    if (basePath is null || string.IsNullOrEmpty(basePath.Valor1))
					{
                        throw new InvalidOperationException();
                    }

                    var fileName = $"00001000000516005087_{serial}_{numericFolio}.{(getPDF ? "pdf" : "xml")}";
                    var completPath = Path.Combine(basePath.Valor1, fileName);

                    return completPath;
                }
            }
            catch (Exception ex)
			{
				return null;
			}
		}
    }
}
