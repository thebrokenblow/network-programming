namespace HttpServer;

public class UserService
{
    private int maxId = 0;
    private readonly List<User> _users;

    public UserService(List<User> users)
    {
        _users = users;

        _users.Add(new User
        {
            Id = ++maxId,
            FirstName = "Магомед",
            LastName = "Дагиров"
        });

        _users.Add(new User
        {
            Id = ++maxId,
            FirstName = "Лиана",
            LastName = "Ганибаева"
        });
    }

    public List<User> GetAll()
    { 
        return _users;
    }

    public User GetById(int id)
    {
        return _users.FirstOrDefault(x => x.Id == id);
    }

    public void Create(User user)
    {
        user.Id = ++maxId;

        _users.Add(user);
    }

    public void Update(int id, User user)
    {
        var updatedUser = GetById(id);

        updatedUser.FirstName = user.FirstName;
        updatedUser.LastName = user.LastName;
    }

    public void Delete(int id)
    {
        var deletedUser = GetById(id);
        _users.Remove(deletedUser);
    }
}