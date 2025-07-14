namespace Nubetico.Shared.Dto.ProyectosConstruccion
{
	public class Categorias
	{
		public int ID { get; set; }
		public int ID_Categoria_Padre { get; set; }
		public string Descripcion { get; set; }
		public bool Habilitado { get; set; }
	}
}
