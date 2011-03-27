using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreTypes;
using XRouter.Prototype.CoreServices;

namespace XRouter.Prototype.Processor
{    
    /// <summary>
    /// Toto rozhrani muze implementovat kliensty programator jako akci. Toto bude samozrejme 
    /// reseno mnohem vice sofistikovaneji, C# atributy, klient musi take obdrzet API, ktere mu 
    /// umozni napr. odeslat zpravu na GW, aktualizovat token, logovat apod. 
    /// </summary>
    interface INodeFunction
    {
        /// <summary>
        /// Jednoznacny user-friendly pojmenovani funkce.
        /// </summary>
        String Name { get; }

        void Initialize(ApplicationConfiguration config);

        void Evaluate(Token token);
    }
}
