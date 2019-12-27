using System;
using System.Collections.Generic;
using Aeon.Core.Repository.Infrastructure;
using Aeon.Core.Repository;
using TodoApp.Repository.Infrastructure;
using AutoMapper;
using VM = TodoApp.ViewModel;
using System.Threading.Tasks;
using TodoApp.Service.Infrastructure;
using System.ComponentModel;
using System.Linq;

namespace TodoApp.Service {
    public class NoteService : Infrastructure.INoteService {
        private readonly ICategoryService _categoryService;
        private readonly IRepository<Model.Note> _noteRepository;
        private readonly IRepository<Model.NoteItem> _itemRepository;
        private readonly IMapper _mapper;
        private readonly ITodoAppDbUnitOfWork _unitOfWork;

        public NoteService(ICategoryService categoryService,
                               IRepository<Model.Note> noteRepository,
                               IRepository<Model.NoteItem> itemRepository,
                               IMapper mapper,
                               ITodoAppDbUnitOfWork unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _categoryService = categoryService;
            _noteRepository = noteRepository;
            _itemRepository = itemRepository;
        }

        public async Task<IEnumerable<ViewModel.Note_ListItem>> GetNotes() {
            var filter = Repository.Filter.NoteFilter.All();
            var data = _noteRepository.GetWithFilter(filter).Data;

            return await Task.FromResult(_mapper.Map<IEnumerable<ViewModel.Note_ListItem>>(data));
        }

        private async Task<ViewModel.NoteItem_ListItem> GetItem(long itemId) {
            var dbItem = await _itemRepository.GetAsync(itemId);
            return await Task.FromResult(_mapper.Map<ViewModel.NoteItem_ListItem>(dbItem));
        }

        private async Task<ViewModel.Note_ListItem> GetNote(long noteId) {
            var filter = Repository.Filter.NoteFilter.ById(noteId);

            var (Data, _) = await _noteRepository.GetWithFilterAsync(filter);
            return await Task.FromResult(_mapper.Map<ViewModel.Note_ListItem>(Data.FirstOrDefault()));
        }


        private const string DEFAULT_NOTE_TITLE = "New note ";
        public async Task<ViewModel.Note_ListItem> AddNote(ViewModel.Note_ListItem note) {
            //figure out the next Title index/suffix
            var filter = new RepositoryFilter<Model.Note>(n => n.Title.StartsWith(DEFAULT_NOTE_TITLE));
            var existingUntitled = _noteRepository.GetWithFilter(filter).Data;

            int maxIndex = 0;
            if (existingUntitled.Any()) {
                _noteRepository.GetWithFilter(filter).Data.Max(c => Int32.Parse("0" + c.Title.Substring(DEFAULT_NOTE_TITLE.Length)));
            }

            //save note
            var dbNote = _mapper.Map<Model.Note>(note);
            dbNote.Title = DEFAULT_NOTE_TITLE + (maxIndex + 1).ToString();
            var newEntry = _noteRepository.Add(dbNote);
            _unitOfWork.Commit();

            return await GetNote(newEntry.Entity.NoteId);
        }

        public async Task<ViewModel.Note_ListItem> EditNote(ViewModel.Note_ListItem item) {
            var filter = Repository.Filter.NoteFilter.ById(item.Id.Value);
            var dbItem = _noteRepository.GetWithFilter(filter).Data.FirstOrDefault();
            _mapper.Map(item, dbItem);
            _unitOfWork.Commit();

            return await GetNote(item.Id.Value);
        }

        public async Task<bool> DeleteNote(ViewModel.Note_ListItem note) {
            //NOTE: VM TO M mapping should probably suffice and Delete(M), since Delete only uses the Keys/PK's further down the road
            var dbNote = _noteRepository.Get(note.Id);
            _noteRepository.Delete(dbNote);
            _unitOfWork.Commit();
            return await Task.FromResult(true);
        }

        #region Items
        public async Task<ViewModel.NoteItem_ListItem> EditItem(ViewModel.NoteItem_ListItem item) {
            var dbItem = _itemRepository.Get(item.Id);
            _mapper.Map(item, dbItem);
            _unitOfWork.Commit();

            return await GetItem(item.Id.Value);
        }

        public async Task<ViewModel.NoteItem_ListItem> AddItem(long noteId, ViewModel.NoteItem_ListItem item) {
            var dbItem = _mapper.Map<Model.NoteItem>(item);
            dbItem.NoteId = noteId;
            _itemRepository.Add(dbItem);
            _unitOfWork.Commit();

            return await GetItem(dbItem.NoteItemId);
        }

        public async Task<bool> DeleteItem(ViewModel.NoteItem_ListItem item) {
            //NOTE: VM TO M mapping should probably suffice and Delete(M), since Delete only uses the Keys/PK's further down the road
            var dbItem = _itemRepository.Get(item.Id);

            _itemRepository.Delete(dbItem);
            _unitOfWork.Commit();
            return await Task.FromResult(true);
        }
        #endregion
    }
}
