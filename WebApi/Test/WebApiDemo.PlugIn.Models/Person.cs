namespace WebApiDemo.PlugIn.Models
{
    /// <summary>
    /// 人员信息类
    /// </summary>
    public class Person
    {
        /// <summary>
        /// 获取或设置人员编号。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 获取或设置人员名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置人员性别信息。
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        /// 获取或设置地址信息。
        /// </summary>
        public Addresses Address { get; set; }
    }

    /// <summary>
    /// 地址信息
    /// </summary>
    public class Addresses
    {
        /// <summary>
        /// 获取或设置省信息。
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 获取或设置市信息。
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 获取或设置详细地址。
        /// </summary>
        public string Address { get; set; }
    }

    /// <summary>
    /// 性别枚举
    /// </summary>
    public enum Sex
    {
        /// <summary>
        /// 女人
        /// </summary>
        Woman = 0,

        /// <summary>
        /// 男人
        /// </summary>
        Man = 1,
    }
}