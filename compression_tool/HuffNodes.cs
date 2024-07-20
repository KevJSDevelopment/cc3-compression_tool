using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace compression_tool
{
    /** Huffman tree node interface */
    public interface IHuffBaseNode {
        bool IsLeaf();
        int Weight();
    }

    /** Huffman tree node: Leaf class */
    public class HuffLeafNode : IHuffBaseNode {
        private char element;      // Element for this node
        private int weight;        // Weight for this node

        /** Constructor */
        public HuffLeafNode(char el, int wt) {
            element = el;
            weight = wt;
        }

        /** @return The element value */
        public char Value() { return element; }

        /** @return The weight */
        public int Weight() { return weight; }

        /** Return true */
        public bool IsLeaf() { return true; }
    }

    /** Huffman tree node: Internal class */
    public class HuffInternalNode : IHuffBaseNode {
        private int weight;
        private IHuffBaseNode left;
        private IHuffBaseNode right;

        /** Constructor */
        public HuffInternalNode(IHuffBaseNode l, IHuffBaseNode r, int wt) {
            left = l;
            right = r;
            weight = wt;
        }

        /** @return The left child */
        public IHuffBaseNode Left() { return left; }

        /** @return The right child */
        public IHuffBaseNode Right() { return right; }

        /** @return The weight */
        public int Weight() { return weight; }

        /** Return false */
        public bool IsLeaf() { return false; }
    }
}