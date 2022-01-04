using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IRolesRepository
    {
        Role GetAdminRight();
        Role GetUserRight();
    }
}
