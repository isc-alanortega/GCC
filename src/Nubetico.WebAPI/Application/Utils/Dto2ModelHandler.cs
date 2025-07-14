using System.Linq.Expressions;

namespace Nubetico.WebAPI.Application.Utils
{
    public class Dto2ModelHandler<TModel>
    {
        public string FieldName { get; set; }
        public Expression<Func<TModel, object>> Expression { get; set; }

        public Dto2ModelHandler(string fieldName, Expression<Func<TModel, object>> expression)
        {
            FieldName = fieldName;
            Expression = expression;
        }
    }
}
