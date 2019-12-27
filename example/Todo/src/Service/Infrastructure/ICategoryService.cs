using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApp.Service.Infrastructure {
    public interface ICategoryService {
        Task<IEnumerable<ViewModel.Category>> GetCategories(bool includeDeleted=false);

        Task<ViewModel.Category> Get(long? id);
    }
}