using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Netick.Unity
{
	public static unsafe partial class NetworkLinkedListExtensions
	{
		
        // ===== 批量操作 =====
        /// <summary> 高效批量添加元素 </summary>
        public static void AddRangeUnsafe<T>(
            this NetworkLinkedList<T> list, 
            ReadOnlySpan<T> items) 
            where T : unmanaged
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }
		
		/// <summary> 在指定索引处插入元素 </summary>
		public static bool Insert<T>(this NetworkLinkedList<T> list, int index, T item) where T : unmanaged
		{
			if (index == 0)
				return list.AddFirst(item, out _);
			
			if (index == list.Count)
				return list.Add(item);
			
			var targetNode = list.GetNodeAt(index);
			return list.AddBefore(targetNode, item, out _);
		}
		
		// ===== 删除操作 =====
		/// <summary> 删除指定索引的元素 </summary>
		public static bool RemoveAt<T>(this NetworkLinkedList<T> list, int index) where T : unmanaged
		{
			var node = list.GetNodeAt(index);
			return node.Index != -1 && list.Remove(node);
		}
		
		/// <summary> 批量删除满足条件的元素 </summary>
		// 条件删除（避免遍历时修改）
		public static int RemoveAll<T>(this NetworkLinkedList<T> list, Predicate<T> match)
			where T : unmanaged
		{
			var nodesToRemove = new List<NetworkLinkedListNode<T>>();
			foreach (var node in list.Nodes)
				if (match(node.Item))
					nodesToRemove.Add(node);
			
			foreach (var node in nodesToRemove)
				list.Remove(node);
			
			return nodesToRemove.Count;
		}
		// ===== 查询操作 =====
		/// <summary> 查找元素首次出现的索引 </summary>
		public static int IndexOf<T>(this NetworkLinkedList<T> list, T item) where T : unmanaged
		{
			int index = 0;
			for (var node = list.Head; node.Index != -1; node = list.GetNode(node.Next))
			{
				if (EqualityComparer<T>.Default.Equals(node.Item, item))
					return index;
				index++;
			}
			return -1;
		}
		
		/// <summary> 检查元素是否存在 </summary>
		public static bool Contains<T>(this NetworkLinkedList<T> list, T item) where T : unmanaged
		{
			return list.IndexOf(item) >= 0;
		}
		
		// 安全获取节点（避免越界）
		public static bool TryGetNode<T>(
			this NetworkLinkedList<T> list, int index, out NetworkLinkedListNode<T> node
			) where T : unmanaged
		{
			node = default;
			if (index < 0 || index >= list.Count)
				return false;
			
			var cur = list.Head;
			for (int i = 0; i < index; i++)
				cur = list.GetNode(cur.Next);
			
			node = cur;
			return true;
		}
	
		
		private static NetworkLinkedListNode<T> GetNodeAt<T>(
			this NetworkLinkedList<T> list, int index) where T : unmanaged
		{
			if (index < 0 || index >= list.Count)
				return default;
			
			var node = list.Head;
			for (int i = 0; i < index; i++)
			{
				if (node.Next == -1) break;
				node = list.GetNode(node.Next);
			}
			return node;
		}
	}
}