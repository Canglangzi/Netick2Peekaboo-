/*using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Netick.Unity
{
    public static  unsafe partial class NetworkLinkedListExtensions
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct NodeData<T> where T : unmanaged
        {
            public int Previous;
            public int Next;
            public T Value;
        }

 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NodeData<T>* GetNodePtr<T>(this NetworkLinkedList<T> list, int index)
            where T : unmanaged
        {
            return (NodeData<T>*)(list._intS + 4 + index * list._linkedListItemSizeWords);
        }

        // ===== 元素访问 =====
        /// <summary> 获取索引处的元素 (O(n)遍历) </summary>
        public static unsafe T Get<T>(this NetworkLinkedList<T> list, int index) where T : unmanaged
        {
            if (index < 0 || index >= list.Count) 
                throw new IndexOutOfRangeException();
            
            var node = list.GetNodePtr(list.HeadNode);
            for (int i = 0; i < index && node->Next != -1; i++)
            {
                node = list.GetNodePtr(node->Next);
            }
            return node->Value;
        }

        /// <summary> 设置索引处的元素 </summary>
        public static unsafe void Set<T>(this NetworkLinkedList<T> list, int index, T value) 
            where T : unmanaged
        {
            if (index < 0 || index >= list.Count) 
                throw new IndexOutOfRangeException();
            
            var node = list.GetNodePtr(list.HeadNode);
            for (int i = 0; i < index && node->Next != -1; i++)
            {
                node = list.GetNodePtr(node->Next);
            }
            
            list.SetElementData(node->Value, value);
            node->Value = value;
        }

        // ===== 批量操作 =====
        /// <summary> 批量添加元素 </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this NetworkLinkedList<T> list, IEnumerable<T> items)
            where T : unmanaged
        {
            foreach (var item in items)
                list.Add(item);
        }

        /// <summary> 批量设置元素值 </summary>
        public static unsafe void SetRange<T>(this NetworkLinkedList<T> list, T[] values) 
            where T : unmanaged
        {
            int count = Math.Min(list.Count, values.Length);
            var node = list.GetNodePtr(list.HeadNode);
            int i = 0;
            
            while (node != null && i < count)
            {
                list.SetElementData(node->Value, values[i]);
                node->Value = values[i];
                i++;
                
                node = node->Next != -1 ? list.GetNodePtr(node->Next) : null;
            }
        }

   
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ForEach<T>(
            this NetworkLinkedList<T> list, 
            Action<T> action) 
            where T : unmanaged
        {
            var node = list.GetNodePtr(list.HeadNode);
            while (node != null)
            {
                action(node->Value);
                node = node->Next != -1 ? list.GetNodePtr(node->Next) : null;
            }
        }
        
        public static unsafe void ForEachIndexed<T>(
            this NetworkLinkedList<T> list, 
            Action<T, int> action) 
            where T : unmanaged
        {
            var node = list.GetNodePtr(list.HeadNode);
            int index = 0;
            
            while (node != null)
            {
                action(node->Value, index++);
                node = node->Next != -1 ? list.GetNodePtr(node->Next) : null;
            }
        }
        public static unsafe int IndexOf<T>(
            this NetworkLinkedList<T> list, 
            T item, 
            IEqualityComparer<T> comparer = null) 
            where T : unmanaged
        {
            comparer ??= EqualityComparer<T>.Default;
            var node = list.GetNodePtr(list.HeadNode);
            int index = 0;
            
            while (node != null)
            {
                if (comparer.Equals(node->Value, item))
                    return index;
                
                index++;
                node = node->Next != -1 ? list.GetNodePtr(node->Next) : null;
            }
            
            return -1;
        }
        public static unsafe T? Find<T>(
            this NetworkLinkedList<T> list, 
            Predicate<T> match) 
            where T : unmanaged
        {
            var node = list.GetNodePtr(list.HeadNode);
            while (node != null)
            {
                if (match(node->Value))
                    return node->Value;
                
                node = node->Next != -1 ? list.GetNodePtr(node->Next) : null;
            }
            
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T[] ToArray<T>(this NetworkLinkedList<T> list) 
            where T : unmanaged
        {
            T[] array = new T[list.Count];
            var node = list.GetNodePtr(list.HeadNode);
            int i = 0;
            
            while (node != null && i < array.Length)
            {
                array[i++] = node->Value;
                node = node->Next != -1 ? list.GetNodePtr(node->Next) : null;
            }
            
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void CopyTo<T>(
            this NetworkLinkedList<T> list, 
            T[] array, 
            int arrayIndex = 0) 
            where T : unmanaged
        {
            var node = list.GetNodePtr(list.HeadNode);
            int i = arrayIndex;
            int end = arrayIndex + Math.Min(list.Count, array.Length - arrayIndex);
            
            while (node != null && i < end)
            {
                array[i++] = node->Value;
                node = node->Next != -1 ? list.GetNodePtr(node->Next) : null;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Preallocate<T>(
            this NetworkLinkedList<T> list, 
            int count) 
            where T : unmanaged
        {
            for (int i = 0; i < count; i++)
            {
                list.Add(default);
                list.Remove(list.Tail);
            }
        }
        
  
        private static unsafe void SetElementData<T>(
            this NetworkLinkedList<T> list, 
            T oldValue, 
            T newValue) 
            where T : unmanaged
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return;
            
            var nodePtr = list.GetNodePtr(list.HeadNode);
            while (nodePtr != null)
            {
                if (EqualityComparer<T>.Default.Equals(nodePtr->Value, oldValue))
                {
                    list.SetElementData(list.GetNodeIndex(nodePtr), newValue);
                    return;
                }
                nodePtr = nodePtr->Next != -1 ? list.GetNodePtr(nodePtr->Next) : null;
            }
        }
        
        private static int GetNodeIndex<T>(
            this NetworkLinkedList<T> list, 
            NodeData<T>* nodePtr) 
            where T : unmanaged
        {
            long basePtr = (long)list._intS + 16; // 4 * sizeof(int)
            long nodeAddr = (long)nodePtr;
            return (int)((nodeAddr - basePtr) / list._linkedListItemSizeWords);
        }
    }
}*/