using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace Netick.Unity
{
	public static class NetworkArrayExtensions
	{
		/// <summary>
		/// 高效迭代所有元素 (指针访问)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void ForEach<T>(
			this NetworkArray<T> array, 
			delegate*<T, void> action
		) where T : unmanaged
		{
			for (int i = 0; i < array.Length; i++)
			{
				
				T* ptr = array.GetElementPtr(i);
				action(*ptr);
			}
		}
		/// <summary>
		/// 高效转换为数组 (内存复制)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] ToArrayUnsafe<T>(this NetworkArray<T> array) 
			where T : unmanaged
		{
			T[] result = new T[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				unsafe
				{
					T* ptr = array.GetElementPtr(i);
					result[i] = *ptr;
				}
			}
			return result;
		}
		/// <summary>
		/// 高效转换为Span (仅用于局部作用域)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe Span<T> AsSpan<T>(this NetworkArray<T> array)
			where T : unmanaged
		{
	
			T[] temp = new T[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				temp[i] = array[i];
			}
			return new Span<T>(temp);
		}
		
		/// <summary> 批量设置数组值（带起始索引） </summary>
		public static void SetRange<T>(this NetworkArray<T> array, int startIndex, IEnumerable<T> values)
			where T : unmanaged
		{
			if (startIndex < 0 || startIndex >= array.Length)
				throw new IndexOutOfRangeException($"Invalid start index: {startIndex}");
			
			int index = startIndex;
			foreach (var value in values)
			{
				if (index >= array.Length) break;
				array[index] = value;
				index++;
			}
		}
		
		// ===== 条件操作 =====
		/// <summary>
		/// 仅当条件满足时更新值 (函数指针优化)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool SetIf<T>(
			this NetworkArray<T> array, 
			int index, 
			T value, 
			Func<T, bool> condition
		) where T : unmanaged
		{
			if (condition(array[index]))
			{
				array[index] = value;
				return true;
			}
			return false;
		}
		/// <summary>
		/// 仅当值改变时更新 (内存比较)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool SetIfChanged<T>(this NetworkArray<T> array, int index, T value)
			where T : unmanaged, IEquatable<T>
		{
			if (!array[index].Equals(value))
			{
				array[index] = value;
				return true;
			}
			return false;
		}
		/// <summary>
		/// 查找第一个满足条件的元素 (指针优化)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? Find<T>(this NetworkArray<T> array, Predicate<T> match)
			where T : unmanaged
		{
			for (int i = 0; i < array.Length; i++)
			{
				unsafe
				{
					T* ptr = array.GetElementPtr(i);
					if (match(*ptr)) 
						return *ptr;
				}
			}
			return default;
		}
		
		/// <summary> 查找满足条件的第一个索引 </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindIndex<T>(this NetworkArray<T> array, Predicate<T> match)
			where T : unmanaged
		{
			for (int i = 0; i < array.Length; i++)
			{
				unsafe
				{
					T* ptr = array.GetElementPtr(i);
					if (match(*ptr)) 
						return i;
				}
			}
			return -1;
		}

		
		/// <summary> 检查是否包含指定元素 </summary>
		public static bool Contains<T>(this NetworkArray<T> array, T value) where T : unmanaged
		{
			var comparer = EqualityComparer<T>.Default;
			for (int i = 0; i < array.Length; i++)
			{
				if (comparer.Equals(array[i], value)) return true;
			}
			return false;
		}
		
		// ===== 转换操作 =====
		/// <summary> 转换为标准数组（副本） </summary>
		public static T[] ToArray<T>(this NetworkArray<T> array) where T : unmanaged
		{
			var result = new T[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				result[i] = array[i];
			}
			return result;
		}
		
		/// <summary> 转换为列表（副本） </summary>
		public static List<T> ToList<T>(this NetworkArray<T> array) where T : unmanaged
		{
			var list = new List<T>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(array[i]);
			}
			return list;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<T>(
			this NetworkArray<T> array, 
			Func<int, T> generator
		) where T : unmanaged
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = generator(i);
			}
		}

		/// <summary>
		/// 交换两个元素位置 (指针操作)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Swap<T>(this NetworkArray<T> array, int indexA, int indexB)
			where T : unmanaged
		{
			T temp = array[indexA];
			array[indexA] = array[indexB];
			array[indexB] = temp;
		}

		// 批量设置值（减少脏数据标记次数）
		public static void SetRange<T>(
			this NetworkArray<T> array, int startIndex, Span<T> values
			) where T : unmanaged
		{
			for (int i = 0; i < values.Length; i++)
			array[startIndex + i] = values[i]; // 逐元素触发同步
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetRange<T>(
			this NetworkArray<T> array,
			int startIndex,
			ReadOnlySpan<T> values
			) where T : unmanaged
		{
			if (startIndex < 0 || startIndex + values.Length > array.Length)
				throw new IndexOutOfRangeException();
			
			for (int i = 0; i < values.Length; i++)
			{
				array[startIndex + i] = values[i];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static unsafe T* GetElementPtr<T>(this NetworkArray<T> array, int index)
			where T : unmanaged
		{
		
			if (index < 0 || index >= array.Length)
				throw new IndexOutOfRangeException($"Index {index} out of range [0, {array.Length - 1}]");
			
			// 通过索引器安全获取地址
			T value = array[index];
			return &value;
		}
	
		/// <summary>
		/// 快速复制到目标数组 (内存复制)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(
			this NetworkArray<T> source, 
			NetworkArray<T> destination, 
			int sourceIndex = 0, 
			int destinationIndex = 0, 
			int count = -1
		) where T : unmanaged
		{
			if (count < 0) count = source.Length;
        
			int end = Math.Min(sourceIndex + count, source.Length);
			for (int i = 0; i < end - sourceIndex; i++)
			{
				destination[destinationIndex + i] = source[sourceIndex + i];
			}
		}
		
	}
}