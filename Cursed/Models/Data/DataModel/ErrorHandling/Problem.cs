

namespace Cursed.Models.DataModel.ErrorHandling
{
    public class Problem
    {
        public string Entity { get; set; }
        public string EntityKey { get; set; }
        public string Message { get; set; }
        public string RedirectRoute { get; set; }
        public bool UseKeyWithRoute { get; set; } = true;
    }
}
