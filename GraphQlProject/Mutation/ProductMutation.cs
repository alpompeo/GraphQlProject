using GraphQL;
using GraphQL.Types;
using GraphQlProject.Interfaces;
using GraphQlProject.Models;
using GraphQlProject.Type;

namespace GraphQlProject.Mutation
{
    public class ProductMutation : ObjectGraphType
    {
        public ProductMutation(IProduct productServices)
        {
            Field<ProductType>("createProduct", arguments: new QueryArguments(new QueryArgument<ProductInputType> { Name = "product" }),
               resolve: context =>
               {
                   return productServices.AddProduct(context.GetArgument<Product>("product"));
               });

            Field<ProductType>("updateProduct", arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" },
                                                                              new QueryArgument<ProductInputType> { Name = "product" }),
              resolve: context =>
              {
                  var productObj = context.GetArgument<Product>("product");
                  var producId = context.GetArgument<int>("id");
                  return productServices.UpdateProduct(producId, productObj);
              });

            Field<ProductType>("deleteProduct", arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
             resolve: context =>
             {
                 var producId = context.GetArgument<int>("id");
                  productServices.DeleteProduct(producId);
                 return $"The product against the {producId} has been deleted";
             });
        }
    }
}
