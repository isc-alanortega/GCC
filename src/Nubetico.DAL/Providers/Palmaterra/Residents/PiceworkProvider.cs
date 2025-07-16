//using Microsoft.EntityFrameworkCore;
//using Nubetico.DAL.Models.Core;
//using Nubetico.DAL.Models.ProyectosConstruccion;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Nubetico.DAL.Providers.Palmaterra.Residents
//{
//    public class PiceworkProvider
//    {

//        private readonly IDbContextFactory<object> _dbContextFactory;

//        public PiceworkProvider(IDbContextFactory<object> dbContextFactory)
//        {
//            _dbContextFactory = dbContextFactory;
//        }

//        public async Task<(int, string)> CreatePiceworkHeader(object piceworkHeaderDb)
//        {
//            using (var context = _dbContextFactory.CreateDbContext())
//            {

//                return (0, "folio");
//            }
//        }

//        public async Task CreatePiceworkElement(object pickeworkElemntDb)
//        {
//            using (var context = _dbContextFactory.CreateDbContext())
//            {

//            }
//        }

//        public async Task GetPiceworkCart(int residentId)
//        {
//            //using (var context = _dbContextFactory.CreateDbContext())
//            //{
//            //    // Aquí se implementaría la lógica para obtener el carrito de destajos del residente
//            //    // Por ejemplo, se podría consultar una tabla que almacene los destajos asociados al residente
//            //}
//        }


//    }
//}
