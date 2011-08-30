using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectConfigurator;
using ObjectConfigurator.ValueValidators;

namespace SimpleDiagrammer.Layouts.ForceDirected
{
    internal class Configuration
    {
        [ConfigurationItem("Apply repulsion force", "Applies repulsion between nodes.", true)]
        internal bool ApplyRepulsionForce { get; private set; }

        [ConfigurationItem("Apply attraction force", "Applies attraction caused by connections.", true)]
        internal bool ApplyAttractionForce { get; private set; }

        [ConfigurationItem("Length of attraction spring", "Lower this number is, closer the attracted nodes are.", 180)]
        [RangeValidator(1, 10000)]
        internal int AttractionSpringLength { get; private set; }

        internal Configuration()
        {
            ApplyRepulsionForce = true;
            ApplyAttractionForce = true;
            AttractionSpringLength = 180;
        }
    }
}
