using DarnDroidz.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarnDroidz.Interfaces
{
    public interface ITreasure
    {
        bool PickUp(IActor actor);
    }
}