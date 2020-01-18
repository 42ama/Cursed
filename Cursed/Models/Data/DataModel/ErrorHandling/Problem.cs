

namespace Cursed.Models.DataModel.ErrorHandling
{
    /// <summary>
    /// Stores information about problem
    /// </summary>
    public class Problem
    {
        /// <summary>
        /// Entity at which problem occured
        /// </summary>
        public string Entity { get; set; }

        /// <summary>
        /// Unique entity key
        /// </summary>
        public string EntityKey { get; set; }

        /// <summary>
        /// Custom message providing information about problem
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Route to return from problem
        /// </summary>
        public string RedirectRoute { get; set; }

        /// <summary>
        /// If set to true, EntityKey is used with RedirectRoute as <c>key</c> parameter, 
        /// otherwise RedirectRoute is used without parameters  
        /// </summary>
        public bool UseKeyWithRoute { get; set; } = true;
    }
}
