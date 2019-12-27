namespace TodoApp.Model.Infrastructure {
    public interface ISoftDelete {
        bool Deleted { get; set; }
    }
}