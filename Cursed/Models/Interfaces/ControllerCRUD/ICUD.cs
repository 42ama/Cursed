

namespace Cursed.Models.Interfaces.ControllerCRUD
{
    /// <summary>
    /// Collection of Create, Update and Delete actions of CRUD model.
    /// </summary>
    /// <typeparam name="T">Data model type</typeparam>
    // Create, Update and Delete used only in bunch, ICUD simplifies access to this interfaces
    public interface ICUD<T> : ICreate<T>, IUpdate<T>, IDelete
    { 
    }
}
