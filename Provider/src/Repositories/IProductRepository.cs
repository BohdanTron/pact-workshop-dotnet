using provider.Model;
using System.Collections.Generic;

namespace provider.Repositories
{
    public interface IProductRepository
    {
        public List<Product> List();
        public Product Get(int id);

        public void SetState(List<Product> product);
    }
}
