using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.Projects
{
    public class ProjectsService(
        IDbContextFactory<ProyectosConstruccionDbContext> proeyectosConstruccciondbContextFactory,
        IDbContextFactory<CoreDbContext> coreDbContextFactory,
        ProjectSectionService projectSectionService,
        IStringLocalizer<SharedResources> localizer,
        IMapper mapper)
    {
        #region INJETIONS
        private readonly IDbContextFactory<ProyectosConstruccionDbContext> _proyectosConstrucciondbContextFactory = proeyectosConstruccciondbContextFactory;
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory = coreDbContextFactory;
        private readonly ProjectSectionService _projectSectionService = projectSectionService;
        private readonly IStringLocalizer<SharedResources> _localizer = localizer;
        private readonly IMapper _mapper = mapper;
        #endregion

        #region UTILS
        private async Task<int> GetUserIdByGuid(Guid? userGuid)
        {
            var dbCore = _coreDbContextFactory.CreateDbContext();
            var user = await dbCore.vUsuariosPersonas.FirstAsync(item => item.GuidUsuarioDirectory == userGuid!.Value);
            return user.IdUsuario;
        }
        #endregion

        #region ADD
        /// <summary>
        /// Asynchronously adds a new project and its associated sections to the database.
        /// The method maps the incoming <paramref name="request"/> to a project entity, saves it to the database,
        /// and then processes each section associated with the project. If any section fails to save, 
        /// the operation is rolled back and an error message is returned.
        /// </summary>
        /// <param name="request">The project data transfer object containing information about the project 
        /// and its associated sections to be added.</param>
        /// <returns>A ResponseDto indicating the success or failure of the operation, with 
        /// the added project data if successful.</returns>
        /// <exception cref="Exception">Throws an exception if there is an issue retrieving the user ID 
        /// or saving the project and sections to the database.</exception>
        public async Task<ResponseDto<ProjectDataDto?>?> AddProjectAsync(ProjectDataDto request)
        {
            using (var context = proeyectosConstruccciondbContextFactory.CreateDbContext())
            {
                // Map the request data to a Project entity and set default values
                var project = _mapper.Map<Proyectos>(request);
                project.Habilitado = true;
                project.Id_Usuario_Alta = await GetUserIdByGuid(request.ActionUserGuid!.Value); ;
                project.Fecha_Alta = DateTime.Now;

                // Add the project to the context and save changes
                await context.Proyectos.AddAsync(project);
                await context.SaveChangesAsync();

                // If the project ID is 0, something went wrong, return an error response
                if (project.Id_Proyecto == 0) return new(false, _localizer["ProyectosConstruccion.Project.Error.NoSaved"]);

                // Map the saved project data back to the request DTO
                _mapper.Map(project, request);

                // Process each project section
                foreach (var section in request.ProjectSectionData)
                {
                    section.UserActionGuid = request.ActionUserGuid;
                    section.ProjectId = project.Id_Proyecto;

                    // Add each section and check if it was saved successfully
                    var responseSection = await _projectSectionService.AddSectionAsync(section);
                    if (responseSection == null || !responseSection.Success) 
                        return new(false, string.Format(_localizer["ProyectosConstruccion.ProjectSection.Error.NoSaved"], section.Name));
                }

                return new(true, data: request);
            }
        }
        #endregion

        #region UPDATE 
        /// <summary>
        /// Asynchronously updates an existing project and its associated sections in the database.
        /// The method retrieves the project from the database, updates its details, and then processes
        /// the project sections. If any sections are deleted, they are removed from the database. 
        /// New sections are added, and existing sections are updated based on the provided request data.
        /// </summary>
        /// <param name="request">The project data transfer object containing the updated project details 
        /// and associated sections to be updated. It also contains the unique action user GUID for tracking changes.</param>
        /// <returns>A ResponseDto indicating the success or failure of the operation, with the updated project data if successful.</returns>
        /// <exception cref="Exception">Throws an exception if the project is not found in the database.</exception>
        public async Task<ResponseDto<ProjectDataDto>> UpdateProjectAsync(ProjectDataDto request)
        {
            // Retrieve the project from the database
            var db = _proyectosConstrucciondbContextFactory.CreateDbContext();
            var project = await db.Proyectos.FirstOrDefaultAsync(project => project.Id_Proyecto == request.ProjectId) ?? throw new Exception("Project Not Found");

            // Update the project
            _mapper.Map(request, project);
            project.Fecha_Modificacion = DateTime.Now;
            project.Id_Usuario_Modificacion = await GetUserIdByGuid(request.ActionUserGuid);

            await db.SaveChangesAsync();

            // Retrieve the project sections from the database
            #region PROCCES SECTIONS (UPDATE/ADD/DELETE)
            var sectionsInDb = await _projectSectionService.FindSectionsByProjectIdAsync(project.Id_Proyecto);
            foreach (var section in sectionsInDb.Result ?? Enumerable.Empty<ProjectSectionDataDto>())
            {
                section.UserActionGuid = request.ActionUserGuid;

                // Check if the section is in the request, or if it's a new section (SectionId == 0 or null)
                if (request.ProjectSectionData.FirstOrDefault(item => item.SectionId == section.SectionId) != null || section.SectionId == 0 || section.SectionId == null) continue;

                // If not in the request, delete the section
                await _projectSectionService.DeleteSectionById(section.SectionId!.Value);
            }

            // Now, process the sections in the request
            foreach (var section in request.ProjectSectionData)
            {
                section.UserActionGuid = request.ActionUserGuid;

                // If the section already exists in the database, update it
                if (sectionsInDb.Result!.FirstOrDefault(item => item.SectionId == section.SectionId) != null)
                {
                    await _projectSectionService.UpdateSectionAsync(section);
                    continue;
                }

                // If the section is new(SectionId is null or 0) and the section is active, add it
                if (section.SectionId == null || section.SectionId == 0 && section.Active) continue;

                var resultSection = await _projectSectionService.AddSectionAsync(section);
               
                // If the section was successfully added, assign the resulting data
                if (resultSection != null && resultSection.Success && resultSection.Result != null) continue;

                section.SectionId = resultSection?.Result!.SectionId;
                section.ProjectId = resultSection?.Result!.ProjectId;
            }
            #endregion

            return new(true, data: request);
        }
        #endregion

        #region DELETE
        // <summary>
        /// Asynchronously deletes a project by marking it and its associated sections and phases as disabled in the database.
        /// The method first retrieves the project by its ID, then disables the project, all active sections related to the project, 
        /// and the phases associated with those sections. The changes are saved in the database.
        /// </summary>
        /// <param name="projectId">The ID of the project to be deleted. This ID is used to locate the project, 
        /// its associated sections, and the phases within those sections in the database.</param>
        /// <returns>A ResponseDto indicating the success or failure of the operation. Returns true if the project was successfully deleted 
        /// (i.e., marked as disabled), otherwise, an exception is thrown if the project is not found.</returns>
        /// <exception cref="Exception">Throws an exception if the project with the given <paramref name="projectId"/> is not found in the database.</exception>
        public async Task<ResponseDto<object>> DeleteProjectAsync(Guid projectGuid)
        {
            var db = proeyectosConstruccciondbContextFactory.CreateDbContext();
            var project = await db.Proyectos.FirstOrDefaultAsync(project => project.UUID == projectGuid) ?? throw new Exception("Project Not Found");

            project.Habilitado = false;

            var sections = await db.Secciones.Where(section => section.Id_Proyecto == project.Id_Proyecto && section.Habilitado == true).ToListAsync();
            foreach (var section in sections)
            {
                section.Habilitado = false;
                var phases = await db.Secciones_Fases.Where(phase => phase.Id_Seccion == section.Id_Seccion).ToListAsync();
                phases.ForEach(phase => phase.Habilitado = false);
            }

            await db.SaveChangesAsync();

            return new(true);
        }
        #endregion

        #region GET PROJECT LIST
        /// <summary>
        /// Asynchronously retrieves a paginated list of projects based on filter criteria. 
        /// The method allows filtering by project name, business unit, subdivision, and status. 
        /// The results are returned in descending order of creation date and are paginated 
        /// based on the provided offset and limit parameters.
        /// </summary>
        /// <param name="request">The request data transfer object containing the filter criteria and pagination settings. 
        /// The properties include filters for project name, business unit ID, subdivision ID, status, 
        /// and pagination parameters such as offset and limit.</param>
        /// <returns>A ResponseDto containing a PaginatedListDto with the filtered project data. 
        /// The result includes the total number of records and the filtered project data for the current page.</returns>
        /// <exception cref="Exception">Throws an exception if an error occurs while querying the database or mapping the results.</exception>
        public async Task<ResponseDto<PaginatedListDto<ProyectsGridDto>?>> GetProjectsFiltersAsync(ProjectsPaginatedRequestDto request)
        {
            using (var context = proeyectosConstruccciondbContextFactory.CreateDbContext())
            {
                // Build the query with filters and pagination
                var query = await context.vProyectos
                    .Where(item =>
                        (string.IsNullOrEmpty(request.Nombre) || item.Proyecto.ToUpper().Contains(request.Nombre.ToUpper())) &&
                        (request.BusinessUnitId == null || request.BusinessUnitId == 0 || item.Id_Unidad_Negocio == request.BusinessUnitId!.Value) &&
                        (request.SubdivisionId == null || request.SubdivisionId == 0 || item.Id_Fraccionamiento == request.SubdivisionId) &&
                        (request.Estado == null || request.Estado == 0 || item.IdEstatus == request.Estado!.Value)
                    )
                    .OrderByDescending(item => item.Fecha_Alta)
                    .Skip(request.OffSet)
                    .Take(request.Limit == 0 ? 20 : request.Limit)
                    .ToListAsync();

                // Map the query result to the ProyectsGridDto
                var projectsDto = _mapper.Map<List<ProyectsGridDto>>(query);

                var result = new PaginatedListDto<ProyectsGridDto>
                {
                    RecordsTotal = projectsDto.Count(),
                    Data = projectsDto,
                };

                result.RecordsFiltered = result.Data.Count;

                return new(true, data: result);
            }
        }
        #endregion

        #region GET PROJECT
        /// <summary>
        /// Asynchronously retrieves a project by its unique GUID identifier and maps the project 
        /// to a ProjectDataDto. Additionally, it retrieves the project's associated sections 
        /// and phases, attaching them to the DTO. The method returns the project data along with 
        /// its associated sections and phases, including the project name and ID for each phase.
        /// </summary>
        /// <param name="projectGuid">The unique GUID of the project to retrieve. This is used to 
        /// locate the project in the database.</param>
        /// <returns>A ResponseDto containing the ProjectDataDto with the retrieved project data, 
        /// including its associated sections and phases. Returns true if the project was found 
        /// and data is successfully retrieved, otherwise throws an exception if the project is not found.</returns>
        /// <exception cref="Exception">Throws an exception if the project with the given <paramref name="projectGuid"/> is not found in the database.</exception>
        public async Task<ResponseDto<ProjectDataDto>> GetOneFilterProjectAsync(Guid projectGuid)
        {
            // Retrieve the project from the database by its GUID
            var db = proeyectosConstruccciondbContextFactory.CreateDbContext();
            var project = await db.Proyectos.FirstOrDefaultAsync(project => project.UUID == projectGuid) ?? throw new Exception("Project Not Found");

            // Map the project entity to a ProjectDataDto
            var projectDto = _mapper.Map<ProjectDataDto>(project);

            // Attach project-related data (ProjectId and ProjectName) to each phase of the sections
            var resultSections = await _projectSectionService.FindSectionsByProjectIdAsync(projectDto.ProjectId!.Value);
            projectDto.ProjectSectionData = (resultSections != null && resultSections.Success && resultSections.Result != null) ? resultSections.Result.ToList() : [];
            projectDto.ProjectSectionData.ForEach(section =>
                section.ProjectSectionPhase.ForEach(phase =>
                {
                    phase.ProjectId = projectDto.ProjectId;
                    phase.ProjectName = projectDto.Name;
                })
            );

            return new(true, data: projectDto);
        }
        #endregion

        #region FETCH PROJECT FORM
        /// <summary>
        /// Asynchronously retrieves the form data needed for project form and serching in cat.
        /// </summary>
        /// <returns>A ResponseDto containing a ProjectFormDataDto with the available dropdown options 
        /// for subdivisions, branches, project types, users, and statuses. Each option contains an ID and a name 
        /// for use in a project form.</returns>
        public async Task<ResponseDto<ProjectFormDataDto>> FetchProjectFormOptionsAsync()
        {
            var dbProConst = _proyectosConstrucciondbContextFactory.CreateDbContext();

            return new(true, data: new ProjectFormDataDto()
            {
                Subdivision = await dbProConst.Fraccionamientos
                    .Where(item => item.Habilitado)
                    .Select(item => new ElementsDropdownForm() { Id = item.Id_Fraccionamiento, Name = item.Nombre })
                    .ToListAsync(),
                Branch = await dbProConst.Sucursales
                    .Where(item => item.Habilitada)
                    .Select(item => new ElementsDropdownForm() { Id = item.Id_Sucursales, Name = item.Nombre })
                    .ToListAsync(),
                Types = await dbProConst.Proyectos_Tipos
                    .Where(item => item.Habilitado)
                    .Select(item => new ElementsDropdownForm() { Id = item.Id_Tipo, Name = item.Nombre, Icon = item.Icon })
                    .ToListAsync(),
                Users = await dbProConst.vEmpleados
                    .Where(item => item.Habilitado)
                    .Select(item => new ElementsDropdownForm() { Id = item.IdEmpleado, Name = $"{item.Nombres} {item.PrimerApellido}" })
                    .ToListAsync(),
                Status = await dbProConst.Cat_Estatus
                    .Where(item => item.Habilitado == true)
                    .Select(item => new ElementsDropdownForm() { Id = item.Id_Estatus, Name = item.Nombre_ES })
                    .ToListAsync(),
            });
        }
        #endregion
    }
}