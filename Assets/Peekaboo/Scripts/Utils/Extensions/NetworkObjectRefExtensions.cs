using System;
using System.Runtime.CompilerServices;

namespace Netick.Unity.Extensions
{
	public static class NetworkObjectRefExtensions
	{
		/// <summary>
        /// 尝试获取网络对象并执行操作
        /// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWithObject(this NetworkObjectRef @ref, NetworkSandbox sandbox, Action<NetworkObject> action)
		{
			if (@ref.TryGetObject(sandbox, out var obj))
			{
				action(obj);
				return true;
			}
			
			return false;
		}
		
		/// <summary>
        /// 尝试获取网络对象并执行带返回值的操作
        /// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T TryWithObject<T>(this NetworkObjectRef @ref, NetworkSandbox sandbox, Func<NetworkObject, T> func, T defaultValue = default)
		{
			if (@ref.TryGetObject(sandbox, out var obj))
			{
				return func(obj);
			}
			
			return defaultValue;
		}
		
		/// <summary>
        /// 如果引用有效则执行操作
        /// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NetworkObjectRef WithValid(this NetworkObjectRef @ref, Action<NetworkObjectRef> action)
		{
			if (@ref.IsValid)
			{
				action(@ref);
			}
			
			return @ref;
		}
		
		/// <summary>
        /// 获取网络对象，如果无效则返回默认值
        /// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NetworkObject GetOr(this NetworkObjectRef @ref, NetworkSandbox sandbox, NetworkObject defaultValue = null)
		{
			return @ref.TryGetObject(sandbox, out var obj) ? obj : defaultValue;
		}
		
		/// <summary>
        /// 获取网络对象的组件，如果无效则返回默认值
        /// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetComponentOr<T>(this NetworkObjectRef @ref, NetworkSandbox sandbox, T defaultValue = default) where T : class
		{
			return @ref.TryGetObject(sandbox, out var obj) ? obj.GetComponent<T>() : defaultValue;
		}
	}
}