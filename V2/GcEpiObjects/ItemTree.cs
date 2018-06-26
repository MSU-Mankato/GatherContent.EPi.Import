using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Core.Internal;

namespace GatherContentImport.GcEpiObjects
{
    public class ItemTree<T> 
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int ParentId { get; set; }
        public ICollection<ItemTree<T>> Children { get; set; }
        private ItemTree<T> _subNode;
        
        public ItemTree()
        {
         }

        public ItemTree(int itemId, string itemName, int parentId)
        {
            this.ItemId = itemId;
            this.ItemName = itemName;
            this.ParentId = parentId;
            this.Children = new LinkedList<ItemTree<T>>();
        }
      
        public void AddChild(int itemId, string itemName, int parentId)
        {
            ItemTree<T> childNode = new ItemTree<T>(itemId, itemName, parentId);

            // Traverse through every item of tree
            Traverse(this, childNode.ParentId);
            var subParent = this._subNode;
            if (subParent != null)
            {
                subParent.Children.Add(childNode);
            }
            else
            {
                this.Children.Add(childNode);
            }
        }

        private void Traverse(ItemTree<T> node, int parentId)
        {
            if (node.ItemId == parentId)
            {
                this._subNode = node;
            }

            foreach (var childNode in node.Children)
            {
                Traverse(childNode, parentId); // recursion to parse every child node
            }
        }
    }

}