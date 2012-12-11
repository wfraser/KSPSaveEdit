using System.Collections.Generic;
using System.Text;

namespace KSPSaveEdit
{
    public class SFSNode
    {
        public SFSNode(string name = null)
        {
            if (name != null)
            {
                NodeName = name;
            }

            Attributes = new Dictionary<string, string>();
            Children = new List<SFSNode>();
        }

        public string NodeName { get; set; }
        public Dictionary<string, string> Attributes { get; private set; }
        public List<SFSNode> Children { get; private set; }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} Node: ", NodeName);

            if (Attributes.Count == 0)
            {
                sb.Append("[empty]");
            }

            foreach (var pair in Attributes)
            {
                sb.AppendFormat("{0} = {1}; ", pair.Key, pair.Value);
            }

            return sb.ToString();
        }
#endif
    }
}
