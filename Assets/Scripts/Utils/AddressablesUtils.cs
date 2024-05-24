// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;

// public class AddressablesUtils : MonoBehaviour
// {
//     #region Loading Assets
//     /// <summary>
//     /// Get the given AddressableAsset by path and return it.
//     /// </summary>
//     /// <param name="addressableAssetPath">The path of the AddressableAsset you want to get.</param>
//     /// <returns>Asset of type T.</returns>
//     public static T Load<T>(string addressableAssetPath)
//     {
//         var loadAsset = Addressables.LoadAssetAsync<T>(addressableAssetPath);
//         var result = loadAsset.WaitForCompletion();
//         return result;
//     }

//     /// <summary>
//     /// Get the given AddressableAsset by path and call a function with the result.
//     /// </summary>
//     /// <param name="addressableAssetPath">The path of the AddressableAsset you want to get.</param>
//     /// <param name="resultFunction">The function called with the result.</param>
//     public static void Load<T>(string addressableAssetPath, Action<T> resultFunction)
//     {
//         Addressables.LoadAssetAsync<T>(addressableAssetPath).Completed += (asyncOperationHandle) =>
//         {
//             if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
//             {
//                 resultFunction(asyncOperationHandle.Result);
//             }
//         };
//     }

//     /// <summary>
//     /// Get the given AddressableAsset by reference and return it.
//     /// </summary>
//     /// <param name="addressableAssetReference">A reference to the AddressableAsset you want to get.</param>
//     /// <returns>Asset of type T.</returns>
//     public static T Load<T>(AssetReference addressableAssetReference)
//     {
//         var loadAsset = Addressables.LoadAssetAsync<T>(addressableAssetReference);
//         var result = loadAsset.WaitForCompletion();
//         return result;
//     }

//     /// <summary>
//     /// Get the given AddressableAsset by reference and call a function with the result.
//     /// </summary>
//     /// <param name="addressableAssetReference">A reference to the AddressableAsset you want to get.</param>
//     /// <param name="resultFunction">The function called with the result.</param>
//     public static void Load<T>(AssetReference addressableAssetReference, Action<T> resultFunction)
//     {
//         Addressables.LoadAssetAsync<T>(addressableAssetReference).Completed += (asyncOperationHandle) =>
//         {
//             if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
//             {
//                 resultFunction(asyncOperationHandle.Result);
//             }
//         };
//     }

//     /// <summary>
//     /// Get given AddressableAssets by label and return them.
//     /// </summary>
//     /// <param name="addressableAssetLabel">A reference to the AddressableAssetLabel you want to get.</param>
//     /// <returns>List of Assets of type T.</returns>
//     public static IList<T> Load<T>(AssetLabelReference addressableAssetLabel)
//     {
//         var loadAsset = Addressables.LoadAssetsAsync<T>(addressableAssetLabel, (T) => {});
//         var result = loadAsset.WaitForCompletion();
//         return result;
//     }

//     /// <summary>
//     /// Get given AddressableAssets by label and call a function with the results.
//     /// </summary>
//     /// <param name="addressableAssetLabel">A reference to the AddressableAssetLabel you want to get.</param>
//     /// <param name="resultFunction">The function called with the results.</param>
//     public static void Load<T>(AssetLabelReference addressableAssetLabel, Action<IList<T>> resultFunction)
//     {
//         Addressables.LoadAssetsAsync<T>(addressableAssetLabel, (T) => {}).Completed += (asyncOperationHandle) =>
//         {
//             if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
//             {
//                 resultFunction(asyncOperationHandle.Result);
//             }
//         };
//     }
//     #endregion

//     /// <summary>
//     /// Get the address of a given AssetReference.
//     /// </summary>
//     /// <param name="reference">The AssetReference you want to find the address of.</param>
//     /// <returns>The address of a given AssetReference.</returns>
//     public static string GetAddressFromAssetReference(AssetReference reference)
//     {
//         var loadResourceLocations = Addressables.LoadResourceLocationsAsync(reference);
//         var result = loadResourceLocations.WaitForCompletion();
//         if (result.Count > 0)
//         {
//             string key = result[0].PrimaryKey;
//             Addressables.Release(loadResourceLocations);
//             return key;
//         }

//         Addressables.Release(loadResourceLocations);
//         return string.Empty;
//     }
// }
