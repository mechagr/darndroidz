using DarnDroidz.Core;
using DarnDroidz.Systems; 

namespace DarnDroidz.Interfaces 
{
    public interface IBehavior
    {
        bool Act(Droid droid, CommandSystem commandSystem); 
    }
}
