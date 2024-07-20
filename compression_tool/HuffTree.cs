using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace compression_tool
{
    /** A Huffman coding tree */
    public class HuffTree : IComparable<HuffTree> {
        private IHuffBaseNode root;

        /** Constructors */
        public HuffTree(char el, int wt) {
            root = new HuffLeafNode(el, wt);
        }
        public HuffTree(IHuffBaseNode l, IHuffBaseNode r, int wt) {
            root = new HuffInternalNode(l, r, wt);
        }

        public IHuffBaseNode Root() { return root; }
        public int Weight() { return root.Weight(); }

        public int CompareTo(HuffTree that) {
            if (root.Weight() < that.Weight()) { return -1; }
            else if (root.Weight() == that.Weight()) { return 0; }
            else { return 1; }
        }
    }
}