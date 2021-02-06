using GraphQL;
using GraphQL.Types;
using GraphQlProject.Interfaces;
using GraphQlProject.Type;

namespace GraphQlProject.Query
{
    public class ProductQuery : ObjectGraphType
    {
        public ProductQuery(IProduct productServices)
        {
            Field<ListGraphType<ProductType>>("products", resolve: context => { return productServices.GetAllProducts(); });

            Field<ProductType>("product", arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                    return productServices.GetProductId(context.GetArgument<int>("id"));
                });
        }
    }
}
