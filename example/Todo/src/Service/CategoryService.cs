using System.Collections.Generic;
using System.Threading.Tasks;
using Aeon.Core.Repository.Infrastructure;
using AutoMapper;
using TodoApp.Repository.Infrastructure;
using TodoApp.Service.Infrastructure;

namespace TodoApp.Service {
    public class CategoryService : ICategoryService {
        private readonly IRepository<Model.Category> _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ITodoAppDbUnitOfWork _unitOfWork;

        public CategoryService(IRepository<Model.Category> categoryRepository,
                               IMapper mapper,
                               ITodoAppDbUnitOfWork unitOfWork) {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<ViewModel.Category>> GetCategories(bool includeDeleted = false) {
            var sortedFilter = Repository.Filter.CategoryFilter.AllAlphabetical(includeDeleted);
            var (Data, _) = _categoryRepository.GetWithFilter(sortedFilter);

            return await Task.FromResult(_mapper.Map<IEnumerable<ViewModel.Category>>(Data));
        }

        public async Task<ViewModel.Category> Get(long? id) {
            ViewModel.Category data;

            if (id.HasValue) {
                var dbData = await _categoryRepository.GetAsync(id);
                data = _mapper.Map<ViewModel.Category>(dbData);
            } else {
                data = new ViewModel.Category();
            }
            return data;
        }

    }


}