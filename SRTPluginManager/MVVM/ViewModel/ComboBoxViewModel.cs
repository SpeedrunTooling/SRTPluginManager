using System.Collections.Generic;

namespace SRTPluginManager.MVVM.ViewModel
{
    public class ComboBoxViewModel
    {
        public List<string> RE6Names { get; set; }

        public ComboBoxViewModel()
        {
            RE6Names = new List<string>()
            {
                "Leon",
                "Helena",
                "Chris",
                "Piers",
                "Jake",
                "Sherry",
                "Ada",
                "Agent",
                "None"
            };
        }
    }
}
