using System.Collections.ObjectModel;

namespace TaskSharp.Classes
{
    public class User
    {

        public byte UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        // reference on user's notes
        public virtual ICollection<BaseNote> TaskList
        { get; private set; } = new ObservableCollection<BaseNote>();
    }
}
