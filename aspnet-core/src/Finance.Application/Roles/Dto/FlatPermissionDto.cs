namespace Finance.Roles.Dto
{
    public class FlatPermissionDto
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 权限显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; set; }
    }
}