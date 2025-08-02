using NencerApi.Modules.User.Model;
using System.Reflection;

namespace NencerApi.Extentions
{
    public static class PermissionExtension
    {
        public static Dictionary<string, object> LoadPermissionsTree()
        {
            var permissionsTree = new Dictionary<string, List<string>>();
            // Lấy tất cả các lớp con của PermissionDefined
            var subclasses = typeof(PermissionDefined).GetNestedTypes();
            foreach (var subclass in subclasses)
            {
                var permissions = subclass.GetFields(BindingFlags.Public | BindingFlags.Static) //lấy những filed static hoặc public
                    .Where(field => field.IsLiteral && !field.IsInitOnly) //field là hằng số
                    .Select(field => (string)field.GetValue(null))
                    .ToList();
                permissionsTree[subclass.Name.ToUpper()] = permissions;
            }

            var modifiedDictionary = TransformPermissions(permissionsTree);

            return modifiedDictionary;
        }

        private static Dictionary<string, object> TransformPermissions(Dictionary<string, List<string>> permissionsTree)
        {
            var modifiedDictionary = new Dictionary<string, object>();
            var listRemoveKeys = new List<string>();

            foreach (var keyValuePair in permissionsTree)
            {
                var result = TransformPermissionList(keyValuePair.Value, permissionsTree, new HashSet<string>(), keyValuePair.Key);
                modifiedDictionary.Add(keyValuePair.Key, result.Item2);

                if (result.Item1.Count > 0)
                {
                    listRemoveKeys.AddRange(result.Item1);
                }
            }

            foreach (var item in listRemoveKeys)
            {
                modifiedDictionary.Remove(item);
            }
            return modifiedDictionary;
        }

        /// <summary>
        /// check key có trong value của dictionary hay không để lấy
        /// </summary>
        /// <param name="permissions">permissions</param>
        /// <param name="permissionsTree">Dictionary</param>
        /// <param name="processedKeys">HashSet để kiểm tra để tránh vòng lặp vô hạn</param>
        /// <param name="currentKey">Có phải value trùng với key hiện tại không</param>
        /// <returns>Item1: list key có trong value, item2: lấy value từ key có trong value</returns>
        private static (List<string>, List<object>) TransformPermissionList(List<string> permissions, Dictionary<string, List<string>> permissionsTree, HashSet<string> processedKeys, string currentKey = null)
        {
            var transformedList = new List<object>();
            var listRemoveKeys = new List<string>();

            foreach (var permission in permissions)
            {
                // Bỏ qua nếu permission trùng với key gốc của lần đệ quy này
                if (permission == currentKey)
                {
                    //transformedList.Add(permission);
                    continue;
                }

                if (permissionsTree.ContainsKey(permission))
                {
                    // Kiểm tra để tránh vòng lặp vô hạn
                    if (!processedKeys.Contains(permission))
                    {
                        processedKeys.Add(permission); // Đánh dấu key này đã được xử lý
                                                       // Tạo bản sao của processedKeys để tránh ảnh hưởng đến các nhánh khác
                        var nestedPermissions = TransformPermissionList(permissionsTree[permission], permissionsTree, new HashSet<string>(processedKeys), permission);
                        transformedList.Add(new Dictionary<string, object> { { permission, nestedPermissions.Item2 } });
                        listRemoveKeys.Add(permission);
                    }
                }
                else
                {
                    transformedList.Add(permission);
                }
            }
            return (listRemoveKeys, transformedList);
        }

        /// <summary>
        /// Lấy mô tả cho một constant từ PermissionDefined
        /// </summary>
        /// <param name="permissionConstant">Tên constant của quyền</param>
        /// <returns>Mô tả của constant hoặc chuỗi rỗng nếu không tìm thấy mô tả</returns>
        public static string GetDescriptionByValue(string permissionValue)
        {
            var nestedTypes = typeof(PermissionDefined).GetNestedTypes(); // Lấy tất cả các lớp con

            foreach (var type in nestedTypes)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static); // Lấy tất cả các trường
                foreach (var field in fields)
                {
                    if ((string)field.GetValue(null) == permissionValue) // Kiểm tra giá trị trường
                    {
                        var attribute = field.GetCustomAttribute<DescriptionAttribute>(); // Lấy thuộc tính Description
                        if (!string.IsNullOrEmpty(attribute?.Description))
                        {
                            return attribute?.Description;
                        }
                    }
                }
            }
            return permissionValue; // Trả về thông báo không tìm thấy nếu không có trường nào khớp
        }

    }
}
